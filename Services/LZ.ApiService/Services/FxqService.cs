using LZ.ApiService.Helper;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using static QRCoder.PayloadGenerator;

namespace LZ.ApiService.Services
{
    public class FxqService : IFxqService, IScopedDependency
    {

        private IRedisService _redisService;
        private IHttpClientFactory _httpClientFactory;
        private IAliyunCommonService _aliyunCommonService;

        private readonly FxqOption _fxqOption;
        private readonly FxqConfigOption _fxqConfigOption;

        private readonly ILogger<FxqService> _logger;



        public FxqService(IHttpClientFactory httpClientFactory, IAliyunOSSService aliyunOSSService, IRedisService redisService, IOptions<FxqOption> fxqOption, IOptions<FxqConfigOption> fxqConfigOption, ILogger<FxqService> logger, IAliyunCommonService aliyunCommonService)
        {
            _httpClientFactory = httpClientFactory;
            _redisService = redisService;
            _fxqOption = fxqOption.Value;
            _fxqConfigOption = fxqConfigOption.Value;
            _logger = logger;
            _aliyunCommonService = aliyunCommonService;
        }

        public async Task<string> GetFxqAccessToken()
        {
            string key = "fxqAccessToken";

            string accessToken = string.Empty;

            accessToken = await _redisService.GetAsync<string>(key);

            TokenRequest requestToken = new TokenRequest(_fxqOption.Key, _fxqOption.Secret);


            // 不存在授权码
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                var client = _httpClientFactory.CreateClient();
                var json = FxqHelper.ToJson(requestToken);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_fxqOption.IdentityUrl}/auth/v1/token", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<FxqResponse<string>>(responseContent);

                    TimeSpan timeSpan = new TimeSpan(1, 50, 0);
                    await _redisService.SetAsync(key, resultData.Data, timeSpan);
                    return resultData.Data;
                }
                else
                {
                    return null;
                }

            }

            return accessToken;


        }

        public async Task<string> GetFxqAccessToken(string appName)
        {

            string fxqKey = _fxqOption.Key;
            string fxqSecret = _fxqOption.Secret;
            string fxqIdentityUrl = _fxqOption.IdentityUrl;
            string key = "fxqAccessToken";

            if (appName!="默认应用")
            {
                key = $"fxqAccessToken-{appName}";
                var config = _fxqConfigOption.Items.Where(f => f.AppName == appName).FirstOrDefault();
                if (config == null)
                {
                    throw new ErrorCodeException(-1,$"放心签应用【{appName}】配置信息缺失");
                }
                fxqKey = config.Key;
                fxqSecret = config.Secret;
                fxqIdentityUrl = config.IdentityUrl;
            }

            string accessToken = string.Empty;

            accessToken = await _redisService.GetAsync<string>(key);

            TokenRequest requestToken = new TokenRequest(fxqKey, fxqSecret);


            // 不存在授权码
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                var client = _httpClientFactory.CreateClient();
                var json = FxqHelper.ToJson(requestToken);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{fxqIdentityUrl}/auth/v1/token", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<FxqResponse<string>>(responseContent);

                    TimeSpan timeSpan = new TimeSpan(1, 50, 0);
                    await _redisService.SetAsync(key, resultData.Data, timeSpan);
                    return resultData.Data;
                }
                else
                {
                    return null;
                }

            }

            return accessToken;


        }

        public async Task<string> GetFxqCaAccessToken(string appName)
        {
            string key = $"fxqCaAccessToken-{appName}";
            string accessToken = string.Empty;
            accessToken = await _redisService.GetAsync<string>(key);

          var  config=  _fxqConfigOption.Items.Where(f => f.AppName == appName).FirstOrDefault();
            if (config==null)
            {
                throw null;
            }
            TokenRequest requestToken = new TokenRequest(config.Key, config.Secret);


            // 不存在授权码
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                var client = _httpClientFactory.CreateClient();
                var json = FxqHelper.ToJson(requestToken);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{config.IdentityUrl}/auth/v1/token", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<FxqResponse<string>>(responseContent);

                    TimeSpan timeSpan = new TimeSpan(1, 50, 0);
                    await _redisService.SetAsync(key, resultData.Data, timeSpan);
                    return resultData.Data;
                }
                else
                {
                    return null;
                }

            }

            return accessToken;


        }




        #region 个人
        /// <summary>
        ///  身份认证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="nonce"></param>
        /// <param name="replaceData"></param>
        /// <returns></returns>
        public async Task<FxqResponse<RealNameStatus>> IdentityAuthentication2(string realName, string idCardNo)
        {

            //string url = $"{_fxqOption.IdentityUrl}/identity/v1/2";
            //RealName2Request request = new RealName2Request(realName, idCardNo);
            //var response = await ExecuteAsync<RealName2Request, RealNameStatus>(request, url);
            //return response;
            var aliResp = await _aliyunCommonService.IdentityAuthentication2(realName, idCardNo);
            if (aliResp.Code == "0" && aliResp.Result?.Res == "1")
            {
                return new FxqResponse<RealNameStatus>() { Data = new RealNameStatus(1), Msg = aliResp.Result.Description };
            }
            else
            {
                return new FxqResponse<RealNameStatus>() { Data = new RealNameStatus(2), Msg = aliResp.Result.Description };
            }


        }
        /// <summary>
        /// 全网三要素
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="idCardNo"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>

        public async Task<FxqResponse<RealNameStatus>> IdentityAuthentication3(string realName, string idCardNo, string mobile)
        {

            string url = $"{_fxqOption.IdentityUrl}/identity/v1/3";
            RealName3Request request = new RealName3Request(realName, idCardNo, mobile);
            var response = await ExecuteAsync<RealName3Request, RealNameStatus>(request, url);
            return response;

        }
        /// <summary>
        /// 银行卡三要素接口
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="idCardNo"></param>
        /// <param name="bankCardNo"></param>
        /// <returns></returns>
        public async Task<FxqResponse<RealNameStatus>> IdentityBankAuthentication3(string realName, string idCardNo, string bankCardNo)
        {

            string url = $"{_fxqOption.IdentityUrl}/bank_card/v1/3";
            RealNameBank3Request request = new RealNameBank3Request(realName, idCardNo, bankCardNo);
            var response = await ExecuteAsync<RealNameBank3Request, RealNameStatus>(request, url);
            return response;

           
        }
        /// <summary>
        /// 一行卡四要素
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="idCardNo"></param>
        /// <param name="bankCardNo"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task<FxqResponse<RealNameStatus>> IdentityBankAuthentication4(string realName, string idCardNo, string bankCardNo, string mobile)
        {

            string url = $"{_fxqOption.IdentityUrl}/bank_card/v1/4";
            RealNameBank4Request request = new RealNameBank4Request(realName, idCardNo, bankCardNo, mobile);
            var response = await ExecuteAsync<RealNameBank4Request, RealNameStatus>(request, url);
            return response;
        }

        /// <summary>
        /// 个人实名全⽹三要素+短信 (直连CA) 新的认证方式
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="idCardNo"></param>
        /// <param name="bankCardNo"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task<FxqResponse<RealNameApply>> PersonIdentityMessageAuthentication(string personName, string idNo, string phone, string idType,string redirectUrl)
        {

            string url = $"{_fxqOption.IdentityUrl}/identity/person/apply/sms";
            RealNameMessageRequest request = new RealNameMessageRequest(personName, idNo, phone, idType, redirectUrl);
            var response = await ExecuteAsync<RealNameMessageRequest, RealNameApply>(request, url);
            return response;
        }

        #endregion

        #region 企业
        /// <summary>
        /// 企业三要素认证
        /// </summary>
        /// <param name="name">公司名称</param>
        /// <param name="keyword">注册号/统一社会信用代码(注册号支持15位，统一社会信用代码支持18位)</param>
        /// <param name="legalPerson">企业法人</param>
        /// <returns></returns>

        public async Task<FxqResponse<CompanyRealName3Response>> CompanyAuthentication3(string name, string keyword, string legalPerson)
        {
            string url = $"{_fxqOption.IdentityUrl}/company/v1/3";
            CompanyRealName3Request request = new CompanyRealName3Request(name, keyword, legalPerson);
            var response = await ExecuteAsync<CompanyRealName3Request, CompanyRealName3Response>(request, url);
            return response;
        }
        /// <summary>
        /// 企业四要素认证
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keyword"></param>
        /// <param name="legalPerson"></param>
        /// <param name="certNo"></param>
        /// <returns></returns>

        public async Task<FxqResponse<CompanyRealName4Response>> CompanyAuthentication4(string name, string keyword, string legalPerson, string certNo)
        {

            string url = $"{_fxqOption.IdentityUrl}/company/v1/4";
            CompanyRealName4Request request = new CompanyRealName4Request(name, keyword, legalPerson, certNo);
            var response = await ExecuteAsync<CompanyRealName4Request, CompanyRealName4Response>(request, url);
            return response;
        }


        /// <summary>
        /// 企业工商照面信息查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FxqResponse<CompanyNameResponse>> CompanyInfo(string name)
        {
            string url = $"{_fxqOption.RestapiUrl}/company/v1/4";
            CompanyNameRequest request = new CompanyNameRequest(name);
            var response = await ExecuteAsync<CompanyNameRequest, CompanyNameResponse>(request, url);
            return response;
        }


        /// <summary>
        /// 对公打款验证
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FxqResponse<CompanyBankPaymentResponse>> CompanyBankPayment(string accountName, string bankName, string bankNo)
        {
            string url = $"{_fxqOption.IdentityUrl}/pub_bank/v1/payment";
            CompanyBankPaymentRequest request = new CompanyBankPaymentRequest(accountName, bankName, bankNo);
            var response = await ExecuteAsync<CompanyBankPaymentRequest, CompanyBankPaymentResponse>(request, url);
            return response;
        }

        /// <summary>
        /// 公司实名全⽹三要素+短信 (直连CA) 新的认证方式
        /// </summary>
        /// <returns></returns>
        public async Task<FxqResponse<RealNameApply>> CompanyIdentityMessageAuthentication(string legalName, string legalIdNo, string companyName, string creditNo, string redirectUrl)
        {

            string url = $"{_fxqOption.IdentityUrl}/identity/person/apply/sms";
            CompanyMessageRequest request = new CompanyMessageRequest(legalName, legalIdNo, companyName, creditNo, redirectUrl);
            var response = await ExecuteAsync<CompanyMessageRequest, RealNameApply>(request, url);
            return response;
        }

        #endregion
        #region 意愿认证
        /// <summary>
        /// 短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<FxqResponse<SmsSendResponse>> SmsSend(string phone, string content)
        {
            string url = $"{_fxqOption.RestapiUrl}/sms/v1/send";
            SmsSendRequest request = new SmsSendRequest(phone, content);
            var response = await ExecuteAsync<SmsSendRequest, SmsSendResponse>(request, url);
            return response;
        }
        #endregion
        #region 签署


        /// <summary>
        /// 区域签署
        /// </summary>
        /// <param name="contract">合同文件(Base64或oss的URL链接)注意：传bases64时，不能传头信息</param>
        /// <param name="signers">签署人信息(具体信息,可以同时传多个签署人)</param>
        /// <param name="type">合同返回类型(0:Base64,1:oss地址,默认为0)</param>
        /// <param name="size">签章的大小(全局)，默认 120(单位为px)</param>
        /// <param name="signDate">签章日期（默认跟随在印章底部）</param>
        /// <returns></returns>
        public async Task<FxqResponse<string>> PdfContractSign(string contract, Signer[] signers, int type, int size, SignDate signDate)
        {
            string url = $"{_fxqOption.RestapiUrl}/contract/v1/port/sign";
            PdfContractSignRequest request = new PdfContractSignRequest(contract, signers, type, size, signDate);
            var response = await ExecuteAsync<PdfContractSignRequest, string>(request, url);
            return response;
        }

        /// <summary>
        /// 关键字签署
        /// </summary>
        /// <param name="contract">合同文件(Base64或oss的URL链接) 注意：传bases64时，不能传头信息</param>
        /// <param name="signers">签署人信息(具体信息,可以同时传多个签署人)</param>
        /// <param name="type">合同返回类型(0:Base64,1:oss的URL链接,默认为0)</param>
        /// <param name="move">移动控制签章x轴运动</param>
        /// <param name="size">签章的大小(全局)， 默认 120(单位为px)</param>
        /// <param name="signDate">签章日期（跟随在印章底部）</param>
        /// <returns></returns>
        public async Task<FxqResponse<string>> KeywordPdfContractSign(string contract, KeywordSigner[] signers, int type,int? move, int? size, SignDate? signDate)
        {
            string url = $"{_fxqOption.RestapiUrl}/contract/v1/key/sign";
            KeywordPdfContractSignRequest request = new KeywordPdfContractSignRequest(contract, signers, type, move, size, signDate);
            var response = await ExecuteAsync<KeywordPdfContractSignRequest, string>(request, url);
            return response;

        }
        #endregion
        #region 合同存证
        /// <summary>
        ///  蚂蚁链存证
        /// </summary>
        /// <param name="Content">文本存证时，表示所存的原文信息（注意：输入值根据实际填写）,文件存证时，表示源文件网络地址或者base64值</param>
        /// <param name="EvidenceType">
        /// 存证类型，默认值为1：
        ///1:文本存证：如实名、意愿等流程证据存证，大小内容需小于100K。
        ///2:文件存证：如合同原文存证。</param>
        /// <param name="TransactionId">
        /// 存证事务id，标识一组关联的存证链路
        ///1.首次存证不需要传(自动创建)
        ///2.关联的一组存证链路请使用同一个transactionId</param>
        /// <param name="Phase">当前所处存证事务中的阶段，请合作方自行定义-文本存证时必传，注意：文件存证时不须要传，阶段默认就是合同签署</param>
        public async Task<FxqResponse<EvidenceFullResponse>> EvidenceFull(string content, string evidenceType, string transactionId =null, string phase=null)
        {
            string url = $"{_fxqOption.RestapiUrl}/evidence/v1/full";
            EvidenceFullRequest request = new EvidenceFullRequest(content, evidenceType, transactionId, phase);
            var response = await ExecuteAsync<EvidenceFullRequest, EvidenceFullResponse>(request, url);
            return response;

        }
        /// <summary>
        /// 存证证明
        /// </summary>
        /// <param name="dna">存证返回的hash</param>
        /// <returns></returns>
        public async Task<FxqResponse<EvidenceLetterResponse>> EvidenceLetter(string dna)
        {
            string url = $"{_fxqOption.RestapiUrl}/evidence/v1/letter";
            EvidenceLetterRequest request = new EvidenceLetterRequest(dna);
            var response = await ExecuteAsync<EvidenceLetterRequest, EvidenceLetterResponse>(request, url);
            return response;

        }


        /// <summary>
        /// 存证证明
        /// </summary>
        /// <param name="dna">存证返回的hash</param>
        /// <returns></returns>
        public async Task<FxqResponse<string>> EvidenceFullFetchRequest(string evidenceType,string txHash)
        {
            string url = $"{_fxqOption.RestapiUrl}/evidence/v1/letter";
            EvidenceFullFetchRequest request = new EvidenceFullFetchRequest(evidenceType, txHash);
            var response = await ExecuteAsync<EvidenceFullFetchRequest, string>(request, url);
            return response;
        }
        #endregion
        #region 辅助接口
        /// <summary>
        /// word转pdf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileBase64"></param>
        /// <returns></returns>
        public async Task<FxqResponse<string>> DocxToPdf(string fileBase64)
        {
            string url = $"{_fxqOption.RestapiUrl}/api/base64";
            WordToPdfRequest request = new WordToPdfRequest(fileBase64);
            var response = await ExecuteAsync<WordToPdfRequest, string>(request, url);
            return response;
        }

        /// <summary>
        /// 个人签章生成
        /// </summary>
        /// <param name="name">签署人名称（不能超过15个字符）</param>
        /// <param name="rtype">返回类型(0:Base64,1:oss地址链接,默认为0)</param>
        /// <param name="color">颜色(0:红色,1:蓝色,2:黑色,默认为0)</param>
        /// <param name="font">字体(0:仿宋,1:楷体,2:黑体,默认为0)</param>
        /// <param name="type">形状(0:长方形,1:正方形,默认为0)</param>
        /// <returns></returns>
        public async Task<FxqResponse<string>> ElectronicSeal(string name,int rtype=0,int color=0,int font=0,int type=0)
        {
            string url = $"{_fxqOption.RestapiUrl}/seal/v1/personal";
            ElectronicSealRequest request = new ElectronicSealRequest(name, rtype, color, font, type);
            var response = await ExecuteAsync<ElectronicSealRequest, string>(request, url);
            return response;
        }


        public async Task<FxqResponse<string>> CompanyElectronicSeal(string name, string? title, string? seqNo, int? type, int? color, int? font, int? rtype, int? isRound)
        {
            string url = $"{_fxqOption.RestapiUrl}/seal/v1/company";
            CompanyElectronicSealRequest request = new CompanyElectronicSealRequest(name, title, seqNo, rtype, color, font, isRound, isRound);
            var response = await ExecuteAsync<CompanyElectronicSealRequest, string>(request, url);
            return response;
        }



        /// <summary>
        /// 关键词替换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content">合同文件(Base64/oss的URL链接)</param>
        /// <param name="replaceData">关键词替换内容，具体参考入参json</param>
        /// <param name="rtype">返回类型(0:Base64,1:oss的URL链接,默认为0)</param>
        /// <returns></returns>
        public async Task<FxqResponse<string>> DocxKeywordReplace<T>(string content, T replaceData, int rtype)
        {
            string url = $"{_fxqOption.RestapiUrl}/template/v1/docx";
            DocxKeywordReplaceRequest<T> request = new DocxKeywordReplaceRequest<T>(content, replaceData, rtype);
            var response = await ExecuteAsync<DocxKeywordReplaceRequest<T>, string>(request, url);
            return response;
        }


        /// <summary>
        /// PDF文件转图片
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public async Task<FxqResponse<PdfToPngResponse>> PdfToPng(string fileUrl)
        {
            string url = $"{_fxqOption.RestapiUrl}/template/v1/pdf2Img/asyn";
            PdfToPngRequest request = new PdfToPngRequest(fileUrl);
            var response = await ExecuteAsync<PdfToPngRequest, PdfToPngResponse>(request, url);
            return response;


        
        }
        /// <summary>
        /// 合同删除
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public async Task<FxqResponse<string>> ContractRemove(string contractUrl)
        {
            string url = $"{_fxqOption.RestapiUrl}/contract/v1/file/remove";
            ContractRemoveRequest request = new ContractRemoveRequest(contractUrl);
            var response = await ExecuteAsync<ContractRemoveRequest, string>(request, url);
            return response;
        }
        /// <summary>
        /// 银行卡信息查询
        /// </summary>
        /// <param name="fileUrl">银行卡信息查询</param>
        /// <returns></returns>
        public async Task<FxqResponse<BankInfoResponse>> BankInfo(string bankCardNo)
        {
            string url = $"{_fxqOption.RestapiUrl}/bank_card/v1/info";
            BankInfoRequest request = new BankInfoRequest(bankCardNo);
            var response = await ExecuteAsync<BankInfoRequest, BankInfoResponse>(request, url);
            return response;
        }
        private async Task<FxqResponse<TResponse>> ExecuteAsync<TRequest, TResponse>(TRequest request, string url,string appName="默认应用")
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                appName = "默认应用";
            }
            string jsonRequest = FxqHelper.ToJson(request);
            string nonce = Guid.NewGuid().ToString("N");
            var accessToken = await GetFxqAccessToken(appName);
            var signture = FxqHelper.SignJson(jsonRequest, accessToken, nonce);
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("fxq-nonce", nonce);
            httpClient.DefaultRequestHeaders.Add("fxq-sign", signture);
            httpClient.DefaultRequestHeaders.Add("token", accessToken);
      
            StringContent stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, stringContent);
            if (response.IsSuccessStatusCode)
            {
                var contentStr = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(contentStr);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<FxqResponse<TResponse>>(contentStr);
            }
            else
            {
                return new FxqResponse<TResponse>()
                {
                    Code = 500,
                    TradeNo = string.Empty,
                    Msg = $"放心签接口请求异常，请求地址：{url}：发送Http请求异常！HttpStatusCode:{response.StatusCode}"
                };
            }
        }
        #endregion



        public async Task<string> DownloadFile(string url)
        {

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                var contentDisposition = response.Content.Headers.ContentDisposition?.ToString();
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileBase64 = Convert.ToBase64String(fileBytes);
                return fileBase64;
            }
            return null;
            
        }

        #region 流程整合
        public async Task<FxqResponse<string>> CombineDocxKeywordToPdf(string docxBase64, Dictionary<string, string> keywords)
        {
            FxqResponse<string> res = new FxqResponse<string>()
            {
                Code = 200,
                Data = string.Empty,
                Msg = "",
                TradeNo = string.Empty,

            };
            var replaceRes = await DocxKeywordReplace(docxBase64, keywords, 0);
            if (replaceRes != null && replaceRes.Code == 10000)
            {
                var pdfRes = await DocxToPdf(replaceRes.Data);
                if (pdfRes != null && pdfRes.Code == 1000)
                {
                    res.Data = pdfRes.Data;

                }
                else
                {
                    res.Code = 500;
                    res.Msg = pdfRes?.Msg;
                }
            }
            else
            {
                res.Code = 500;
                res.Msg = replaceRes?.Msg;
            }
            return res;
        }
        public record SignAndEvidenceFullResponse(string PdfBase64, string TransactionId, string Hash);
        public async Task<FxqResponse<SignAndEvidenceFullResponse>> CombinePdfSigning(string PdfBase64, KeywordSigner[] signers)
        {

            var signDate = new SignDate(1, 2, null, null);
            FxqResponse<SignAndEvidenceFullResponse> res = new FxqResponse<SignAndEvidenceFullResponse>()
            {
                Code = 200,
                Data = null,
                Msg = "",
                TradeNo = string.Empty,
            };
            // 签署
            var signRes = await KeywordPdfContractSign(PdfBase64, signers, 0, 0, 120, signDate);
            if (signRes != null && signRes.Code == 10000)
            {
                // 存证
                var eviRes = await EvidenceFull(signRes.Data, "2", null, null);
                if (eviRes != null && eviRes.Code == 10000)
                {
                    res.Data = new SignAndEvidenceFullResponse(signRes.Data, eviRes.Data.TransactionId, eviRes.Data.Hash);
                }
                else
                {
                    res.Data = new SignAndEvidenceFullResponse(signRes.Data, string.Empty, string.Empty);
                    res.Msg = "文件签署成功，存证失败！";
                }
            }
            else
            {
                res.Data = new SignAndEvidenceFullResponse(string.Empty, string.Empty, string.Empty);
                res.Msg = "文件签署失败，存证失败！";
            }
            return res;
        }
        #endregion

        #region Ca 认证



        /// <summary>
        /// 
        /// </summary>
        /// <param name="redictUrl"></param>
        /// <returns></returns>
        public async Task<FxqResponse<CaAge18ApplyResponse>> CaAge18Apply(string name,string cardID, string redictUrl,string appName)
        {
            string url = $"{_fxqOption.IdentityUrl}/identity/person/apply";
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("idNo", cardID);
            request.Add("idType", "0");
            request.Add("personName", name);
            request.Add("redirectUrl", redictUrl);
            var response = await ExecuteAsync<Dictionary<string, string>, CaAge18ApplyResponse>(request, url, appName);
            return response;
        }


        public async Task<FxqResponse<CaAgreeApplyResponse>> CaAgreementApply(string redictUrl,string appName)
        {
            string url = $"{_fxqOption.IdentityUrl}/face/v1/agreement/apply";
            CaAgreeApplyRequest request = new CaAgreeApplyRequest() { redictUrl = redictUrl };
            var response = await ExecuteAsync<CaAgreeApplyRequest, CaAgreeApplyResponse>(request, url, appName);
            return response;
        }

        public async Task<FxqResponse<string>> CaFaceApply(string name,string cardID,string businessID,string returnUrl,string orderId,string verifyId,string appName)
        {


            //var encodeUrl =HttpUtility.UrlEncode(returnUrl);
            string url = $"{_fxqOption.IdentityUrl}/face/v1/h5/ca";
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("name", name);
            request.Add("idno", cardID);
            request.Add("companyId", businessID);
            request.Add("returnUrl", returnUrl);
            request.Add("orderId", orderId);
            request.Add("verifyId", verifyId);
            Common.StaticLog.Debug("【CaFaceApply】请求参数：" + Newtonsoft.Json.JsonConvert.SerializeObject(request));

            request = request.OrderBy(f => f.Key).ToDictionary() ;
            var response = await ExecuteAsync<Dictionary<string, string>, string>(request, url, appName);
            Common.StaticLog.Debug("【CaFaceApply】请求响应：" + Newtonsoft.Json.JsonConvert.SerializeObject(response.Data));
            return response;
        }


        public async Task<FxqResponse<CaAgreeApplyResponse>> CaAgreementAndFaceApply(string name, string cardID, string businessID, string returnUrl, string orderId,string appName)
        {
            string url = $"{_fxqOption.IdentityUrl}/face/v1/agreement/face/ca";
            Dictionary<string, string> dss = new Dictionary<string, string>();
            dss.Add("companyId", businessID);
            dss.Add("idno", cardID);
            dss.Add("name", name);
            dss.Add("orderNo", orderId);
            dss.Add("returnUrl", returnUrl);

            Common.StaticLog.Debug("【CaAgreementAndFaceApply】请求参数：" + Newtonsoft.Json.JsonConvert.SerializeObject(dss));
            var response = await ExecuteAsync<Dictionary<string,string>, CaAgreeApplyResponse>(dss, url, appName);
            Common.StaticLog.Debug("【CaAgreementAndFaceApply】请求响应：" + Newtonsoft.Json.JsonConvert.SerializeObject(response.Data));
            return response;
        }


        public async Task<FxqResponse<SmsSendResponse>> CaSendSmsApply(string mobile, string? content, string? signature,string appName)
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(content))
            {
                content = "Ca实名认证验证码： %s ，验证码有效期限为10分钟";
            }
            dss.Add("content", content);
            dss.Add("mobile", mobile);
            if (!string.IsNullOrWhiteSpace(signature))
            {
                dss.Add("signature", signature);
            }
      
            string url = $"{_fxqOption.IdentityUrl}/identity/v1/sendsms";
            var response = await ExecuteAsync<Dictionary<string, string>, SmsSendResponse>(dss, url, appName);
            return response;
        }


        public async Task<FxqResponse<CaRealNameStatus>> CaValidateSmsCodeApply(string realName, string idCardNo, string mobile,string code,string verifyId,string appName)
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
         
            dss.Add("code", code);
            dss.Add("idCardNo", idCardNo);
            dss.Add("mobile", mobile);
            dss.Add("realName", realName);
            dss.Add("verifyId", verifyId);
            string url = $"{_fxqOption.IdentityUrl}/identity/v1/3/v2/validate";
            var response = await ExecuteAsync<Dictionary<string, string>, CaRealNameStatus>(dss, url, appName);
            return response;
        }


        public async Task<FxqResponse<CaCompanyRealName4Response>> CaCompanyAuthApply(string name, string keyword, string legalPerson, string legalPersonCardID, string verifyId,string appName)
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
            dss.Add("certNo", legalPersonCardID);
            dss.Add("keyword", keyword);
            dss.Add("legalPerson", legalPerson);
            dss.Add("name", name);
            dss.Add("verifyId", verifyId);
            string url = $"{_fxqOption.IdentityUrl}/company/v1/newapply/4";
            Common.StaticLog.Debug("【CaCompanyAuthApply】请求参数：" + Newtonsoft.Json.JsonConvert.SerializeObject(dss));
            var response = await ExecuteAsync<Dictionary<string, string>, CaCompanyRealName4Response>(dss, url, appName);
            Common.StaticLog.Debug("【CaCompanyAuthApply】请求响应：" + Newtonsoft.Json.JsonConvert.SerializeObject(response.Data));
            return response;
        }

   
        public async Task<FxqResponse<CaFaceFileResponse>> GetCaFaceFile(string orderNo,string appName)
        {
            var accessToken = await GetFxqAccessToken(appName);
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("token", accessToken);
            var response = await client.GetAsync($"https://identity.fangxinqian.cn/face/v1/h5/result?file=1&orderNo={orderNo}");
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<FxqResponse<CaFaceFileResponse>>();
                return res;
            }
            else
            {
                return new FxqResponse<CaFaceFileResponse>()
                {
                    Code = 500,
                    TradeNo = string.Empty,
                    Msg = $"放心签接口请求异常，发送Http请求异常！HttpStatusCode:{response.StatusCode}"
                };
            }
        }

        #endregion
    }
}
