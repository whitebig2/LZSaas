using LZ.ApiService.Services;
using LZ.Model.FxqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Interface
{
    public interface IFxqService
    {
        Task<FxqResponse<BankInfoResponse>> BankInfo(string bankCardNo);
        Task<FxqResponse<CompanyRealName3Response>> CompanyAuthentication3(string name, string keyword, string legalPerson);
        Task<FxqResponse<CompanyRealName4Response>> CompanyAuthentication4(string name, string keyword, string legalPerson, string certNo);

        Task<FxqResponse<RealNameApply>> CompanyIdentityMessageAuthentication(string legalName, string legalIdNo, string companyName, string creditNo, string redirectUrl);
        Task<FxqResponse<string>> CompanyElectronicSeal(string name, string? title, string? seqNo, int? type, int? color, int? font, int? rtype, int? isRound);
        Task<FxqResponse<CompanyNameResponse>> CompanyInfo(string name);
        Task<FxqResponse<string>> ContractRemove(string contractUrl);
        Task<FxqResponse<string>> DocxKeywordReplace<T>(string content, T replaceData, int rtype);
        Task<FxqResponse<string>> ElectronicSeal(string name, int rtype = 0, int color = 0, int font = 0, int type = 0);
        Task<FxqResponse<RealNameStatus>> IdentityAuthentication2(string realName, string idCardNo);
        Task<FxqResponse<RealNameStatus>> IdentityAuthentication3(string realName, string idCardNo, string mobile);
        Task<FxqResponse<RealNameStatus>> IdentityBankAuthentication3(string realName, string idCardNo, string bankCardNo);
        Task<FxqResponse<RealNameStatus>> IdentityBankAuthentication4(string realName, string idCardNo, string bankCardNo, string mobile);
        Task<FxqResponse<RealNameApply>> PersonIdentityMessageAuthentication(string personName, string idNo, string phone, string idType, string redirectUrl);
        Task<FxqResponse<string>> KeywordPdfContractSign(string contract, KeywordSigner[] signers, int type, int? move, int? size, SignDate? signDate);
        Task<FxqResponse<PdfToPngResponse>> PdfToPng(string fileUrl);
        Task<FxqResponse<SmsSendResponse>> SmsSend(string phone, string content);
        Task<FxqResponse<string>> DocxToPdf(string fileBase64);
        Task<string> DownloadFile(string url);
        Task<FxqResponse<EvidenceFullResponse>> EvidenceFull(string content, string evidenceType, string transactionId, string phase = null);
        Task<FxqResponse<string>> CombineDocxKeywordToPdf(string docxBase64, Dictionary<string, string> keywords);
        Task<FxqResponse<FxqService.SignAndEvidenceFullResponse>> CombinePdfSigning(string PdfBase64, KeywordSigner[] signers);
        Task<FxqResponse<CompanyBankPaymentResponse>> CompanyBankPayment(string accountName, string bankName, string bankNo);
        Task<FxqResponse<CaAgreeApplyResponse>> CaAgreementApply(string redictUrl,string appName);
        Task<FxqResponse<string>> CaFaceApply(string name, string cardID, string businessID, string returnUrl, string orderId, string verifyId, string appName);
        Task<FxqResponse<CaAgreeApplyResponse>> CaAgreementAndFaceApply(string name, string cardID, string businessID, string returnUrl, string orderId,string appName);
        Task<FxqResponse<SmsSendResponse>> CaSendSmsApply(string mobile, string? content, string? signature, string appName);
        Task<FxqResponse<CaRealNameStatus>> CaValidateSmsCodeApply(string realName, string idCardNo, string mobile, string code, string verifyId, string appName);
        Task<FxqResponse<CaCompanyRealName4Response>> CaCompanyAuthApply(string name, string keyword, string legalPerson, string legalPersonCardID, string verifyId, string appName);
        Task<FxqResponse<CaFaceFileResponse>> GetCaFaceFile(string orderNo, string appName);
        Task<FxqResponse<CaAge18ApplyResponse>> CaAge18Apply(string name, string cardID, string redictUrl, string appName);
    }
}
