using LZ.ApiService.Helper;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model.FxqModel;
using LZ.Model.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Services
{
    public class FxqDevService : IFxqDevService, IScopedDependency
    {

        private IRedisService _redisService;
        private IHttpClientFactory _httpClientFactory;


        private readonly ILogger<FxqService> _logger;

        const string _identityUrl = "https://identitytest.fangxinqian.cn";
        const string _restapiUrl = "https://restapitest.fangxinqian.cn";



        public FxqDevService(IHttpClientFactory httpClientFactory,IAliyunOSSService aliyunOSSService, IRedisService redisService,ILogger<FxqService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<string> GetFxqAccessToken()
        {
            string key = "fxqDevAccessToken";

            string accessToken = string.Empty;

            accessToken = await _redisService.GetAsync<string>(key);

            TokenRequest requestToken = new TokenRequest("GoexwR3Toe", "72e9ff5d6a49491690a4cd22eb9402b1");


            // 不存在授权码
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                var client = _httpClientFactory.CreateClient();
                var json = FxqHelper.ToJson(requestToken);
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_identityUrl}/auth/v1/token", stringContent);
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

            string url = $"{_identityUrl}/identity/v1/2";
            RealName2Request request = new RealName2Request(realName, idCardNo);
            var response = await ExecuteAsync<RealName2Request, RealNameStatus>(request, url);
            return response;
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

            string url = $"{_identityUrl}/identity/v1/3";
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

            string url = $"{_identityUrl}/bank_card/v1/3";
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

            string url = $"{_identityUrl}/bank_card/v1/4";
            RealNameBank4Request request = new RealNameBank4Request(realName, idCardNo, bankCardNo, mobile);
            var response = await ExecuteAsync<RealNameBank4Request, RealNameStatus>(request, url);
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
            string url = $"{_identityUrl}/company/v1/3";
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

            string url = $"{_identityUrl}/company/v1/4";
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
            string url = $"{_restapiUrl}/company/v1/4";
            CompanyNameRequest request = new CompanyNameRequest(name);
            var response = await ExecuteAsync<CompanyNameRequest, CompanyNameResponse>(request, url);
            return response;
        }


        /// <summary>
        /// 对公打款验证
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<FxqResponse<CompanyBankPaymentResponse>> CompanyBankPayment(string accountName,string bankName,string bankNo)
        {
            string url = $"{_identityUrl}/pub_bank/v1/payment";
            CompanyBankPaymentRequest request = new CompanyBankPaymentRequest(accountName,bankName, bankNo);
            var response = await ExecuteAsync<CompanyBankPaymentRequest, CompanyBankPaymentResponse>(request, url);
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
            string url = $"{_restapiUrl}/sms/v1/send";
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
            string url = $"{_restapiUrl}/contract/v1/port/sign";
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
            string url = $"{_restapiUrl}/contract/v1/key/sign";
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
            string url = $"{_restapiUrl}/evidence/v1/full";
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
            string url = $"{_restapiUrl}/evidence/v1/letter";
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
            string url = $"{_restapiUrl}/evidence/v1/letter";
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
            string url = $"{_restapiUrl}/api/base64";
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
            string url = $"{_restapiUrl}/seal/v1/personal";
            ElectronicSealRequest request = new ElectronicSealRequest(name, rtype, color, font, type);
            var response = await ExecuteAsync<ElectronicSealRequest, string>(request, url);
            return response;
        }


        public async Task<FxqResponse<string>> CompanyElectronicSeal(string name, string? title, string? seqNo, int? type, int? color, int? font, int? rtype, int? isRound)
        {
            string url = $"{_restapiUrl}/seal/v1/company";
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
            string url = $"{_restapiUrl}/template/v1/docx";
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
            string url = $"{_restapiUrl}/template/v1/pdf2Img/asyn";
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
            string url = $"{_restapiUrl}/contract/v1/file/remove";
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
            string url = $"{_restapiUrl}/bank_card/v1/info";
            BankInfoRequest request = new BankInfoRequest(bankCardNo);
            var response = await ExecuteAsync<BankInfoRequest, BankInfoResponse>(request, url);
            return response;
        }
        private async Task<FxqResponse<TResponse>> ExecuteAsync<TRequest, TResponse>(TRequest request, string url)
        {
            string jsonRequest = FxqHelper.ToJson(request);
            string nonce = Guid.NewGuid().ToString("N");
            var accessToken = await GetFxqAccessToken();
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


       
        public async Task<FxqResponse<CaAgreeApplyResponse>> CaAgreementApply(string redictUrl)
        {
            string url = $"{_identityUrl}/face/v1/agreement/apply";
            CaAgreeApplyRequest request = new CaAgreeApplyRequest() {  redictUrl= redictUrl };
            var response = await ExecuteAsync<CaAgreeApplyRequest, CaAgreeApplyResponse>(request, url);
            return response;
        }

        public async Task<FxqResponse<CaAgreeApplyResponse>> CaIdentity3(string redictUrl)
        {
            string url = $"{_identityUrl}/identity/v1/sendsms";
            CaAgreeApplyRequest request = new CaAgreeApplyRequest() { redictUrl = "www.fangxinqian.cn" };
            var response = await ExecuteAsync<CaAgreeApplyRequest, CaAgreeApplyResponse>(request, url);
            return response;
        }
        #endregion
    }
}
