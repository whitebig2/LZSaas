using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.NeuChar.App.AppStore;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

namespace LZ.Customer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class ContractUserSignController : ApiControllerBase
    {

        private readonly ICashSignRecordService _cashSignRecordService;
        private readonly ISettingsService _settingsService;
        private readonly IContractSignerService _signerService;
        private readonly IFxqService _fxqService;
        private readonly IAliyunOSSService _oSSService;
        private readonly ICaAuthIdentityService _caAuthIdentityService;

        private record CashRecordDto(Guid Id,string CustomerName, string Name, string IdCardNo,  string FilePath, string SignedFilePath, bool IsNeedSmsCode,string Title,string Description);
        public ContractUserSignController(ICashSignRecordService cashSignRecordService, ISettingsService settingsService, IContractSignerService signerService, IFxqService fxqService, IAliyunOSSService oSSService, ICaAuthIdentityService caAuthIdentityService)
        {
            _cashSignRecordService = cashSignRecordService;
            _settingsService = settingsService;
            _signerService = signerService;
            _fxqService = fxqService;
            _oSSService = oSSService;
            _caAuthIdentityService = caAuthIdentityService;
        }
        [HttpGet]
        public async Task<RequestResultModel> ListCashByUserID(int userID)
        {
            RequestResultModel result = new RequestResultModel(200, "Success", true);

            var records = await _cashSignRecordService.List(f => f.UserID == userID && f.Status >= Model.Enums.CashSignStatusEnumType.待签署);
            foreach (var record in records)
            {
                var caIdentity = await _caAuthIdentityService.Get(f => f.CardID == record.CardID && f.Status == CaAuthIdentityStatusEnumType.认证成功);
                if (caIdentity == null)
                {
                    //throw new ErrorCodeException(200, "请到小程序【我的】-【实名认证】页面完成认证再继续操作！");
                    //return Ok(new { IsAuth=false, Message = "请到小程序【我的】-【实名认证】页面完成认证再继续操作！" });
                    result.Data = false;
                    result.Message = "请到小程序【我的】-【实名认证】页面完成认证再继续操作！";

                    return result;
                }

                if (record.GID == null)
                {
                    record.GID = Guid.NewGuid();
                    await _cashSignRecordService.Update(record);
                }
            }
            var dtos = records.OrderBy(f => f.Status).Select(f => new CashRecordDto(f.GID.Value, f.CustomerName, f.Name, f.CardID, f.PdfPath, f.SignedPath ?? string.Empty, true, "现金承诺函", "备注")).ToList();
            result.Items = dtos;
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> UserSigning([FromForm] Guid id,[FromForm] string smsCode, [FromForm] string empName)
        {
            var setting = await _settingsService.GetMasterSettings();
            Live_CashSignRecord cashSignRecord = null;
            string name = string.Empty;
            string filePath = string.Empty;
            string idCardNo = string.Empty;
            string signedFilePath = string.Empty;
            string failMsg = string.Empty;
            cashSignRecord =await _cashSignRecordService.Get(f => f.GID == id);
            if (cashSignRecord.Name != empName.Trim())
            {
                throw new ErrorCodeException(-1, "姓名信息不匹配");
            }
            if (cashSignRecord.SmsCode != smsCode.Trim())
            {
                throw new ErrorCodeException(-1, "验证码信息不匹配");
            }
            //var caIdentity = await _caAuthIdentityService.Get(f => f.CardID == cashSignRecord.CardID && f.Status == CaAuthIdentityStatusEnumType.认证成功);
            //if (caIdentity == null)
            //{



            //    //throw new ErrorCodeException(-1, "请到小程序【我的】-【实名认证】页面完成认证再继续操作！");
            //    return Ok();
            //}

            name = cashSignRecord.Name;
            filePath = cashSignRecord.PdfPath;
            idCardNo = cashSignRecord.CardID;
            var signerInfo = await _signerService.GetSigner(cashSignRecord.Name, cashSignRecord.CardID,false);
            if (signerInfo == null)
            {
                throw new ErrorCodeException(-1, "电子签章信息缺失");
            }

            KeywordSigner signer = null;
            signer = new KeywordSigner(name, idCardNo, signerInfo.OssUrl, cashSignRecord.SealKey, 120, null);
            var signResp = await _fxqService.KeywordPdfContractSign(filePath, new KeywordSigner[] { signer }, 1, null, null, new SignDate(1, null, null, null));
            if (signResp != null)
            {

                if (signResp.Code == 10000)
                {
                    signedFilePath = signResp.Data;
                }
                else
                {
                    throw new ErrorCodeException(-1, $"签署异常：{signResp.Msg}");
                }
            }
            else
            {
                throw new ErrorCodeException(-1, $"为获取到正确响应，请稍后再试");
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(signedFilePath))
                {
                    var base64 = await _fxqService.DownloadFile(signedFilePath);
                    var fileBase64 = Convert.FromBase64String(base64);
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await _oSSService.UploadFile(ms, "11.pdf", true, false);
                        if (ossResp.Code == 0)
                        {
                            signedFilePath = setting.AliBucketDomain + ossResp.Data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                failMsg = $"上传oss异常，异常信息：{ex.Message}";
            }


            if (!string.IsNullOrWhiteSpace(signedFilePath))
            {
                cashSignRecord.Status = CashSignStatusEnumType.待存证;
                cashSignRecord.SignedPath = signedFilePath;
            }
            else
            {
                cashSignRecord.Status = CashSignStatusEnumType.待签署;
                cashSignRecord.FailMsg = failMsg;
            }
            cashSignRecord.SignTime = DateTime.Now;

          await _cashSignRecordService.Update(cashSignRecord);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> UserDirectSigning([FromForm] Guid id)
        {
            var setting = await _settingsService.GetMasterSettings();
            Live_CashSignRecord cashSignRecord = null;
            string name = string.Empty;
            string filePath = string.Empty;
            string idCardNo = string.Empty;
            string signedFilePath = string.Empty;
            string failMsg = string.Empty;
            cashSignRecord = await _cashSignRecordService.Get(f => f.GID == id);
        

            name = cashSignRecord.Name;
            filePath = cashSignRecord.PdfPath;
            idCardNo = cashSignRecord.CardID;
            var signerInfo = await _signerService.Get(f => f.IsEnabled && f.Idno == cashSignRecord.CardID);
            if (signerInfo == null)
            {
                throw new ErrorCodeException(-1, "电子签章信息缺失");
            }

            KeywordSigner signer = null;
            signer = new KeywordSigner(name, idCardNo, signerInfo.OssUrl, cashSignRecord.SealKey, 120, null);
            var signResp = await _fxqService.KeywordPdfContractSign(filePath, new KeywordSigner[] { signer }, 1, null, null, new SignDate(1, null, null, null));
            if (signResp != null)
            {
                if (signResp.Code == 10000)
                {
                    signedFilePath = signResp.Data;
                }
                else
                {
                    throw new ErrorCodeException(-1, $"签署异常：{signResp.Msg}");
                }
            }
            else
            {
                throw new ErrorCodeException(-1, $"为获取到正确响应，请稍后再试");
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(signedFilePath))
                {
                    var base64 = await _fxqService.DownloadFile(signedFilePath);
                    var fileBase64 = Convert.FromBase64String(base64);
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await _oSSService.UploadFile(ms, "11.pdf", true, false);
                        if (ossResp.Code == 0)
                        {
                            signedFilePath = setting.AliBucketDomain + ossResp.Data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                failMsg = $"上传oss异常，异常信息：{ex.Message}";
            }


            if (!string.IsNullOrWhiteSpace(signedFilePath))
            {
                cashSignRecord.Status = CashSignStatusEnumType.待存证;
                cashSignRecord.SignedPath = signedFilePath;
            }
            else
            {
                cashSignRecord.Status = CashSignStatusEnumType.待签署;
                cashSignRecord.FailMsg = failMsg;
            }
            cashSignRecord.SignTime = DateTime.Now;

            await _cashSignRecordService.Update(cashSignRecord);
            return Ok();
        }
    }
}
