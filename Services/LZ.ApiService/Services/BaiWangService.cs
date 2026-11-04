using Aliyun.Acs.Ram.Model.V20150501;
using LZ.ApiService.Helper;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model.BaiWangModel;
using LZ.Model.BaiWangModel.Taxs;
using LZ.Model.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace LZ.ApiService.Services
{
    public class BaiWangService : IBaiWangService, IScopedDependency
    {
        private readonly BaiWangOption _option;
        private readonly IRedisService _redisService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BaiWangService> _logger;

        public BaiWangService(
            IOptions<BaiWangOption> option,
            IRedisService redisService,
            IHttpClientFactory httpClientFactory,
            ILogger<BaiWangService> logger)
        {
            _option = option.Value;
            _redisService = redisService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Task<BaiWangResponse<BaiWangAddPreInvoiceResponse>> AddPreInvoiceAsync(BaiWangAddPreInvoiceRequest request)
        {
            return ExecuteAsync<BaiWangAddPreInvoiceRequest, BaiWangAddPreInvoiceResponse>(_option.AddPreInvoiceMethod, request, "");
        }

        /// <summary>
        /// 调用百望云发票真伪查验接口；ExecuteAsync 负责获取 token、签名及发送请求。
        /// </summary>
        public Task<BaiWangResponse<BaiWangInvoiceValidationResponse>> VerifyInvoiceAuthenticityAsync(BaiWangInvoiceValidationRequest request)
        {
            return ExecuteAsync<BaiWangInvoiceValidationRequest, BaiWangInvoiceValidationResponse>(
                "baiwang.input.compliance.validate", request, "");
        }

        public Task<BaiWangResponse<BaiWangQueryPreInvoiceResponse>> QueryPreInvoiceAsync(BaiWangQueryPreInvoiceRequest request)
        {
            return ExecuteAsync<BaiWangQueryPreInvoiceRequest, BaiWangQueryPreInvoiceResponse>(_option.QueryPreInvoiceMethod, request, "");
        }

        public Task<BaiWangResponse<BaiWangQueryInvoiceResponse>> QueryInvoiceAsync(BaiWangQueryInvoiceRequest request, string authcode)
        {
            return ExecuteAsync<BaiWangQueryInvoiceRequest, BaiWangQueryInvoiceResponse>(_option.QueryInvoiceMethod, request, authcode);
        }

        public Task<BaiWangResponse<BaiWangIssueInvoiceResponse>> IssueInvoiceAsync(BaiWangIssueInvoiceRequest request, string authcode)
        {
            return ExecuteAsync<BaiWangIssueInvoiceRequest, BaiWangIssueInvoiceResponse>(_option.IssueInvoiceMethod, request, authcode);
        }

        public Task<BaiWangResponse<BaiWangFastRedResponse>> IssueFastRedAsync(BaiWangFastRedRequest request, string authcode)
        {
            return ExecuteAsync<BaiWangFastRedRequest, BaiWangFastRedResponse>(_option.IssueFastRedMethod, request, authcode);
        }

        public Task<BaiWangResponse<BaiWangRedConfirmResponse>> IssueRedConfirmAsync(BaiWangRedConfirmRequest request, string authcode)
        {
            return ExecuteAsync<BaiWangRedConfirmRequest, BaiWangRedConfirmResponse>(_option.IssueRedConfirmMethod, request, authcode);
        }

        public Task<BaiWangResponse<List<BaiWangOpenOrgRelApi>>> OpenOrgRelApiAsync(BaiWangOpenOrgRelApiRequest request)
        {
            return ExecuteAsync<BaiWangOpenOrgRelApiRequest, List<BaiWangOpenOrgRelApi>>("baiwang.usercenter.org.addOpenOrgRelApi", request, _option.OrgAuthCode);
        }

        public async Task<BaiWangTokenResponse> GetTokenAsync(bool cacheToken = false)
        {
            if (cacheToken)
            {
                var cachedToken = await _redisService.GetAsync<string>(_option.CacheKey);
                if (!string.IsNullOrWhiteSpace(cachedToken))
                {
                    return new BaiWangTokenResponse
                    {
                        access_token = cachedToken,
                        token_type = "bearer"
                    };
                }
            }

            var tokenResponse = await RequestTokenByPostAsync("");
            if (cacheToken)
            {
                await CacheTokenAsync(tokenResponse);
            }
            return tokenResponse;
        }

        private async Task<string> GetAccessTokenAsync(string orgAuthCode)
        {
            //var accessToken = await _redisService.GetAsync<string>(_option.CacheKey);
            //if (!string.IsNullOrWhiteSpace(accessToken))
            //{
            //    return accessToken;
            //}
            try
            {
                var tokenResponse = await RequestTokenByPostAsync(orgAuthCode);
                //await CacheTokenAsync(tokenResponse);
                return tokenResponse.access_token;
            }
           catch(Exception e)
            {
                throw new InvalidOperationException(orgAuthCode + "|" +e.ToString());
            }
        }

        private async Task<BaiWangTokenResponse> RequestTokenByPostAsync(string orgAuthCode)
        {
            var jsonData = new Dictionary<string, string>
            {
                ["client_secret"] = _option.AppSecret,
                ["username"] = _option.UserName,
                ["password"] = BaiWangHelper.EncryptPassword(_option.Password, _option.Salt)
            };
            if (!string.IsNullOrEmpty(orgAuthCode))
            {
                jsonData.Add("orgAuthCode", orgAuthCode);
            }
            var tokenUrl = GetApiRootUrl();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            dic.Add("method", "baiwang.oauth.token");
            dic.Add("grant_type", "password");
            dic.Add("version", _option.Version);
            dic.Add("client_id", _option.AppKey);
            dic.Add("encryptScope", "00");
            dic.Add("encryptType", "NONE");
            string aa = JsonConvert.SerializeObject(dic);
            var query = string.Join("&", dic.Select(item =>
                    $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value)}"));
            var url = $"{GetApiRootUrl()}?{query}";
            //var url = tokenUrl + "?" + DictionaryToQueryString(dic);
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(jsonData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("百望获取Token请求 Url={Url}，Response={Response}", tokenUrl, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("百望获取Token失败，StatusCode:{StatusCode}，Response:{Response}", response.StatusCode, responseContent);
                throw new InvalidOperationException($"百望获取Token失败：{responseContent}");
            }
            var result = JsonConvert.DeserializeObject(responseContent) as JObject;
            var tokenResponse = JsonConvert.DeserializeObject<BaiWangTokenResponse>(result["response"].ToString());
            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.access_token))
            {
                var errorMessage = tokenResponse?.error_description ?? tokenResponse?.error ?? responseContent;
                throw new InvalidOperationException($"百望获取Token失败：{errorMessage}");
            }
           
            return tokenResponse;
        }



        private async Task CacheTokenAsync(BaiWangTokenResponse tokenResponse)
        {
            var cacheSeconds = tokenResponse.expires_in > 120 ? tokenResponse.expires_in - 60 : tokenResponse.expires_in;
            await _redisService.SetAsync(_option.CacheKey, tokenResponse.access_token, TimeSpan.FromSeconds(cacheSeconds));
        }

        private string GetApiRootUrl()
        {
            var baseUrl = _option.BaseUrl.TrimEnd('/');
            const string routerSuffix = "/router/rest";
            return baseUrl+ routerSuffix;
        }

        public static string DictionaryToQueryString(Dictionary<string, string> param)
        {
            if (param == null || param.Count == 0)
                return string.Empty;

            var builder = new StringBuilder();
            foreach (var entry in param)
            {
                // 跳过空键或空值的项（按需保留）
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Value == null)
                    continue;

                builder.Append(Uri.EscapeDataString(entry.Key));
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(entry.Value));
                builder.Append('&');
            }

            // 去掉末尾多余的 '&'
            return builder.ToString().TrimEnd('&');
        }



        /// <summary>
        /// 个人养老金下载
        /// </summary>
        public Task<BaiWangResponse<PersonalPensionDownloadResponse>> PersonalPensionDownloadAsync(PersonalPensionDownloadRequest request)
        {
            return ExecuteAsync<PersonalPensionDownloadRequest, PersonalPensionDownloadResponse>("baiwang.cs.personal.pensionDownload", request, "");
        }

        /// <summary>
        /// 个税收入明细删除
        /// </summary>
        public Task<BaiWangResponse<PersonalTaxIncomeFormDeleteResponse>> PersonalTaxIncomeFormDeleteAsync(PersonalTaxIncomeFormDeleteRequest request)
        {
            return ExecuteAsync<PersonalTaxIncomeFormDeleteRequest, PersonalTaxIncomeFormDeleteResponse>("baiwang.cs.personalTaxIncomeFormDelete", request, "");
        }

        /// <summary>
        /// 同步客户信息
        /// </summary>
        public Task<BaiWangResponse<PersonalSyncCustomerInfoResponse>> PersonalSyncCustomerInfoAsync(PersonalSyncCustomerInfoRequest request)
        {
            return ExecuteAsync<PersonalSyncCustomerInfoRequest, PersonalSyncCustomerInfoResponse>("baiwang.cs.personal.syncCustomerInfo", request, "");
        }

        /// <summary>
        /// 个税申报
        /// </summary>
        public Task<BaiWangResponse<PersonalDeclareResponse>> PersonalDeclareAsync(PersonalDeclareRequest request)
        {
            return ExecuteAsync<PersonalDeclareRequest, PersonalDeclareResponse>("baiwang.cs.personal.declare", request, "");
        }

        /// <summary>
        /// 同步客户账号信息
        /// </summary>
        public Task<BaiWangResponse<PersonalSyncCustomerAccountInfoResponse>> PersonalSyncCustomerAccountInfoAsync(PersonalSyncCustomerAccountInfoRequest request)
        {
            return ExecuteAsync<PersonalSyncCustomerAccountInfoRequest, PersonalSyncCustomerAccountInfoResponse>("baiwang.cs.personal.syncCustomerAccountInfo", request, "");
        }

        /// <summary>
        /// 三方协议缴款
        /// </summary>
        public Task<BaiWangResponse<PersonalTripleAgreementPaymentResponse>> PersonalTripleAgreementPaymentAsync(PersonalTripleAgreementPaymentRequest request)
        {
            return ExecuteAsync<PersonalTripleAgreementPaymentRequest, PersonalTripleAgreementPaymentResponse>("baiwang.cs.personal.tripleAgreementPayment", request, "");
        }

        /// <summary>
        /// 三方协议下载
        /// </summary>
        public Task<BaiWangResponse<PersonalTripleAgreementDownloadResponse>> PersonalTripleAgreementDownloadAsync(PersonalTripleAgreementDownloadRequest request)
        {
            return ExecuteAsync<PersonalTripleAgreementDownloadRequest, PersonalTripleAgreementDownloadResponse>("baiwang.cs.personal.tripleAgreementDownload", request, "");
        }

        /// <summary>
        /// 完税证明开具
        /// </summary>
        public Task<BaiWangResponse<PersonalCertificateIssueResponse>> PersonalCertificateIssueAsync(PersonalCertificateIssueRequest request)
        {
            return ExecuteAsync<PersonalCertificateIssueRequest, PersonalCertificateIssueResponse>("baiwang.cs.personal.certificateIssue", request, "");
        }

        /// <summary>
        /// 专项附加扣除下载
        /// </summary>
        public Task<BaiWangResponse<PersonalAdditionDownloadResponse>> PersonalAdditionDownloadAsync(PersonalAdditionDownloadRequest request)
        {
            return ExecuteAsync<PersonalAdditionDownloadRequest, PersonalAdditionDownloadResponse>("baiwang.cs.personal.additionDownload", request, "");
        }

        /// <summary>
        /// 个税收入明细查询
        /// </summary>
        public Task<BaiWangResponse<PersonalTaxIncomeFormQueryResponse>> PersonalTaxIncomeFormQueryAsync(PersonalTaxIncomeFormQueryRequest request)
        {
            return ExecuteAsync<PersonalTaxIncomeFormQueryRequest, PersonalTaxIncomeFormQueryResponse>("baiwang.cs.personalTaxIncomeFormQuery", request, "");
        }

        /// <summary>
        /// 个税收入明细保存
        /// </summary>
        public Task<BaiWangResponse<PersonalTaxIncomeFormSaveResponse>> PersonalTaxIncomeFormSaveAsync(PersonalTaxIncomeFormSaveRequest request)
        {
            return ExecuteAsync<PersonalTaxIncomeFormSaveRequest, PersonalTaxIncomeFormSaveResponse>("baiwang.cs.personalTaxIncomeFormSave", request, "");
        }

        /// <summary>
        /// 人员信息保存
        /// </summary>
        public Task<BaiWangResponse<PersonalUserSaveResponse>> PersonalUserSaveAsync(PersonalUserSaveRequest request)
        {
            return ExecuteAsync<PersonalUserSaveRequest, PersonalUserSaveResponse>("baiwang.cs.personal.userSave", request, "");
        }

        /// <summary>
        /// 开始更正
        /// </summary>
        public Task<BaiWangResponse<PersonalStartCorrectionResponse>> PersonalStartCorrectionAsync(PersonalStartCorrectionRequest request)
        {
            return ExecuteAsync<PersonalStartCorrectionRequest, PersonalStartCorrectionResponse>("baiwang.cs.personal.start_correction", request, "");
        }

        /// <summary>
        /// 作废申报
        /// </summary>
        public Task<BaiWangResponse<PersonalInvalidDeclareResponse>> PersonalInvalidDeclareAsync(PersonalInvalidDeclareRequest request)
        {
            return ExecuteAsync<PersonalInvalidDeclareRequest, PersonalInvalidDeclareResponse>("baiwang.cs.personal.invalid_declare", request, "");
        }

        /// <summary>
        /// 取消更正
        /// </summary>
        public Task<BaiWangResponse<PersonalCancelCorrectionResponse>> PersonalCancelCorrectionAsync(PersonalCancelCorrectionRequest request)
        {
            return ExecuteAsync<PersonalCancelCorrectionRequest, PersonalCancelCorrectionResponse>("baiwang.cs.personal.cancel_correction", request, "");
        }

        /// <summary>
        /// 下载累计数据
        /// </summary>
        public Task<BaiWangResponse<PersonalDownloadCumulativeDataResponse>> PersonalDownloadCumulativeDataAsync(PersonalDownloadCumulativeDataRequest request)
        {
            return ExecuteAsync<PersonalDownloadCumulativeDataRequest, PersonalDownloadCumulativeDataResponse>("baiwang.cs.personal.download_cumulative_data", request, "");
        }

        /// <summary>
        /// 复制上月工资薪金
        /// </summary>
        public Task<BaiWangResponse<PersonalCopyLastMonthSalaryWagesResponse>> PersonalCopyLastMonthSalaryWagesAsync(PersonalCopyLastMonthSalaryWagesRequest request)
        {
            return ExecuteAsync<PersonalCopyLastMonthSalaryWagesRequest, PersonalCopyLastMonthSalaryWagesResponse>("baiwang.cs.personal.copyLastMonthSalaryWages", request, "");
        }

        /// <summary>
        /// 人员列表查询
        /// </summary>
        public Task<BaiWangResponse<InvestorUserListQueryResponse>> InvestorUserListQueryAsync(InvestorUserListQueryRequest request)
        {
            return ExecuteAsync<InvestorUserListQueryRequest, InvestorUserListQueryResponse>("baiwang.cs.investor.user_list_query", request, "");
        }

        /// <summary>
        /// 商业健康险编辑
        /// </summary>
        public Task<BaiWangResponse<InvestorCommercialHealthEditResponse>> InvestorCommercialHealthEditAsync(InvestorCommercialHealthEditRequest request)
        {
            return ExecuteAsync<InvestorCommercialHealthEditRequest, InvestorCommercialHealthEditResponse>("baiwang.cs.investor.commercial_health_edit", request, "");
        }

        /// <summary>
        /// 商业健康险查询
        /// </summary>
        public Task<BaiWangResponse<InvestorCommercialHealthQueryResponse>> InvestorCommercialHealthQueryAsync(InvestorCommercialHealthQueryRequest request)
        {
            return ExecuteAsync<InvestorCommercialHealthQueryRequest, InvestorCommercialHealthQueryResponse>("baiwang.cs.investor.commercial_health_query", request, "");
        }

        /// <summary>
        /// 减免事项查询
        /// </summary>
        public Task<BaiWangResponse<InvestorReduceQueryResponse>> InvestorReduceQueryAsync(InvestorReduceQueryRequest request)
        {
            return ExecuteAsync<InvestorReduceQueryRequest, InvestorReduceQueryResponse>("baiwang.cs.investor.reduce_query", request, "");
        }

        /// <summary>
        /// 公益慈善捐赠编辑
        /// </summary>
        public Task<BaiWangResponse<InvestorCharityDonationEditResponse>> InvestorCharityDonationEditAsync(InvestorCharityDonationEditRequest request)
        {
            return ExecuteAsync<InvestorCharityDonationEditRequest, InvestorCharityDonationEditResponse>("baiwang.cs.investor.charity_donation_edit", request, "");
        }

        /// <summary>
        /// 地区基金下载
        /// </summary>
        public Task<BaiWangResponse<InvestorRegionalFundDownloadResponse>> InvestorRegionalFundDownloadAsync(InvestorRegionalFundDownloadRequest request)
        {
            return ExecuteAsync<InvestorRegionalFundDownloadRequest, InvestorRegionalFundDownloadResponse>("baiwang.cs.investor.regional_fund_download", request, "");
        }

        /// <summary>
        /// 企业信息查询
        /// </summary>
        public Task<BaiWangResponse<InvestorEnterpriseInfoQueryResponse>> InvestorEnterpriseInfoQueryAsync(InvestorEnterpriseInfoQueryRequest request)
        {
            return ExecuteAsync<InvestorEnterpriseInfoQueryRequest, InvestorEnterpriseInfoQueryResponse>("baiwang.cs.investor.enterprise_info_query", request, "");
        }

        /// <summary>
        /// 减免事项编辑
        /// </summary>
        public Task<BaiWangResponse<InvestorReduceEditResponse>> InvestorReduceEditAsync(InvestorReduceEditRequest request)
        {
            return ExecuteAsync<InvestorReduceEditRequest, InvestorReduceEditResponse>("baiwang.cs.investor.reduce_edit", request, "");
        }

        /// <summary>
        /// 公益慈善捐赠查询
        /// </summary>
        public Task<BaiWangResponse<InvestorCharityDonationQueryResponse>> InvestorCharityDonationQueryAsync(InvestorCharityDonationQueryRequest request)
        {
            return ExecuteAsync<InvestorCharityDonationQueryRequest, InvestorCharityDonationQueryResponse>("baiwang.cs.investor.charity_donation_query", request,"");
        }

        private async Task<BaiWangResponse<TResponse>> ExecuteAsync<TRequest, TResponse>(string method, TRequest request,string orgAuthCode)
        {
            if(string.IsNullOrWhiteSpace(orgAuthCode))
            {
                orgAuthCode = _option.OrgAuthCode;
            }
            var accessToken = await GetAccessTokenAsync(orgAuthCode);
            var bodyJson = BaiWangHelper.ToJson(request);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            var signParams = new Dictionary<string, string>
            {
                ["method"] = method,
                ["version"] = _option.Version,
                ["appKey"] = _option.AppKey,
                ["format"] = "json",
                ["timestamp"] = timestamp,
                ["token"] = accessToken,
                ["type"] = "sync",
                ["encryptScope"] = "00",
                ["encryptType"] = "NONE"
            };

            var sign = BaiWangHelper.Sign(signParams, bodyJson, _option.AppSecret);
            var query = string.Join("&", signParams.Select(item =>
                $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value)}"));
            var url = $"{GetApiRootUrl()}?{query}&sign={Uri.EscapeDataString(sign)}";

            var client = _httpClientFactory.CreateClient();
            using var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("百望接口请求 method={Method}，Response={Response}", method, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                return new BaiWangResponse<TResponse>
                {
                    success = false,
                    method = method,
                    errorResponse = new BaiWangErrorResponse
                    {
                        code = ((int)response.StatusCode).ToString(),
                        message = $"百望接口请求异常，HttpStatusCode:{response.StatusCode}"
                    }
                };
            }

            var result = JsonConvert.DeserializeObject<BaiWangResponse<TResponse>>(responseContent);
            return result ?? new BaiWangResponse<TResponse>
            {
                success = false,
                method = method,
                errorResponse = new BaiWangErrorResponse
                {
                    code = "-1",
                    message = "百望接口响应解析失败"
                }
            };
        }
    }
}
