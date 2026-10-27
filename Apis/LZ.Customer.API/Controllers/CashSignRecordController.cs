using LZ.Core;
using LZ.Core.CommonHandle;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using LZ.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using LZ.Customer.Service.Services;
using LZ.Model.Exceptions;
using LZ.Model.Dtos;
using Org.BouncyCastle.Asn1.Sec;
using Microsoft.AspNetCore.Authorization;
using LZ.ApiService.Services;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ;
using LZ.Model.FxqModel;
using LZ.ApiService.Interface;
using NPOI.HSSF.Record;
using LZ.Model.Response;
using Senparc.Weixin.Annotations;
using LZ.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Customer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CashSignRecordController : ApiControllerBase
    {

        private readonly IContractSignerService  _contractSignerService;
        private readonly IContractFileConfigService  _contractFileConfigService;
        private readonly ICashSignRecordService _cashSignRecordService;
        private readonly IFxqService _fxqService;
        private readonly ISettingsService _settingsService;
        private readonly IAliyunOSSService  _aliyunOSSService;
        private readonly IAuthService  _authService;

        public CashSignRecordController(ICashSignRecordService cashSignRecordService, IContractFileConfigService contractFileConfigService, IContractSignerService contractSignerService, IFxqService fxqService, ISettingsService settingsService, IAliyunOSSService aliyunOSSService, IAuthService authService)
        {
            _cashSignRecordService = cashSignRecordService;
            _contractFileConfigService = contractFileConfigService;
            _contractSignerService = contractSignerService;
            _fxqService = fxqService;
            _settingsService = settingsService;
            _aliyunOSSService = aliyunOSSService;
            _authService = authService;
        }
        [HttpGet]
        public async Task<IActionResult> PageList(string name, string customerName, string idCardNo, string signUnitName, CashSignStatusEnumType? status,  [FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Live_CashSignRecord>();



            var user = CurrentUser;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    expable.And(t => t.PartnerID == info.PartnerID);
                }
            }
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                expable.And(it => it.UserID == user.UserID);
            }

            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }


            expable.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            expable.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            expable.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
            expable.AndIF(status.HasValue, f => f.Status==status.Value);
            var exp = expable.ToExpression();
          

            var models = await _cashSignRecordService.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? id,Guid? gid)
        {
            var exp = Expressionable.Create<Live_CashSignRecord>();
            exp.AndIF(id.HasValue, f => f.ID == id.Value);
            exp.AndIF(gid.HasValue, f => f.GID == gid.Value);
            var model = await _cashSignRecordService.Get(exp.ToExpression());
            return Ok(model);
        }


        [HttpGet]
        public async Task<IActionResult> ListByUserID(int userID)
        {
            var expable = Expressionable.Create<Live_CashSignRecord>();
            expable.And(f => f.UserID == userID);
            expable.And(f => f.Status== CashSignStatusEnumType.待签署 );
            expable.And(f => f.GID!=null );
            var exp = expable.ToExpression();
            var models = await _cashSignRecordService.List(exp);
            return Ok(new { items = models, count = models.Count });
        }


        [HttpGet]
        public async Task<IActionResult> GetSignSucRecord(string name,  string idCardNo, string signUnitName, string customerName,string cashType)
        {
            var td = DateTime.Today;
            var expable = Expressionable.Create<Live_CashSignRecord>();
            expable.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            expable.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.CardID== idCardNo);
            expable.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
            expable.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            expable.AndIF(!string.IsNullOrWhiteSpace(cashType), f => f.CashType==cashType);
            expable.And(f => f.Status != CashSignStatusEnumType.生成取消);
            expable.And(f => f.IsEnable);
            //expable.And(f => f.ExpirationDate != null && f.ExpirationDate.Value >= td);
            var exp = expable.ToExpression();
            var model = await _cashSignRecordService.Get(exp);
            if (model==null)
            {
                return Ok(new Live_CashSignRecord());
            }
            return Ok(model);
        }



        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CashSignRecordDto modelDto)
        {
            var dt=DateTime.Today;

            Live_CashSignRecord model = new Live_CashSignRecord()
            {
                ID = 0,
                Age = modelDto.Age,
                Remark = string.Empty,
                CardID = modelDto.CardID,
                CustomerName = modelDto.CustomerName,
                CustomerID = modelDto.CustomerID,
                Name = modelDto.Name,
                Status = modelDto.Status,
                CashType = modelDto.CashType,
                CompanyID = modelDto.CompanyID,
                CompanyName = modelDto.CompanyName,
                CreateTime = DateTime.Now,
                ExpirationDate =DateTime.MaxValue,// 签署记录不需要验证过期时间
                PartnerID = modelDto.PartnerID,
                PartnerName = modelDto.PartnerName,
                Sex = modelDto.Sex,
                UserEntryID = modelDto.UserEntryID,
                UserID = modelDto.UserID,
                SignUnitName = modelDto.SignUnitName,
                PdfPath = string.Empty,
                SealPath = string.Empty,
                SignedPath = string.Empty,
                SignType = (int)SignTypeEnumType.网页签署,
                TempPath = string.Empty,
                CashHash = string.Empty,
                SmsCode = string.Empty,
                SmsContent = string.Empty,
                SmsSendTime = null,
                Phone = modelDto.Phone,
                SmsMsgID = string.Empty,
                SignUnitCompanyNo = modelDto.SignUnitCompanyNo, 
                IsEnable=true,
                FailMsg= modelDto.FailMsg,
                 GID=Guid.NewGuid(),
                
            };
            var existRecord = await _cashSignRecordService.Get(f =>
               f.UserEntryID == model.UserEntryID
               && f.CustomerName == modelDto.CustomerName
               && f.SignUnitName == modelDto.SignUnitName
               &&f.CashType== model.CashType
               && f.Name == modelDto.Name
               && f.CardID == modelDto.CardID
               && f.IsEnable
               );
            if (existRecord != null)
            {
                return Ok(existRecord);
            }
            // 其它的全部禁用
           //await _cashSignRecordService.SetDisabled(f => f.CustomerName == modelDto.CustomerName && f.SignUnitName == modelDto.SignUnitName && f.Name == modelDto.Name && f.CardID == modelDto.CardID);

            #region 获取配置，合同模板，签署方式
            //var config = await _contractFileConfigService.GetView(f => f.CustomerID == modelDto.CustomerID);
            //if (config != null)
            //{
            //    if (string.IsNullOrWhiteSpace(config.CashFilePath))
            //    {
            //        model.Status = CashSignStatusEnumType.生成取消;
            //        model.FailMsg = "现金签收单未配置，取消生成！";
            //    }
            //    else
            //    {
            //        model.SignType = config.SignType;
            //        model.TempPath = config.CashFilePath;
            //    }
            //}


            Guid cashFileID = Guid.Parse("6622A9CE-B38C-47CB-8CC7-D5332CCFD049");
            var fileConfig = await _contractFileConfigService.GetNav(f => f.ID == cashFileID);
            if (fileConfig == null)
            {
                model.Status = CashSignStatusEnumType.生成取消;
                model.FailMsg = "现金签收单文件信息缺失！";
            }

            model.TempPath = fileConfig.FilePath;
            model.SignType =0 ;


            if (fileConfig.Signers.Any(f => f.SignerPartyType == SignerPartyEnumType.第一当事人))
            {
                model.SignUnitSealKey = fileConfig.Signers.First(f => f.SignerPartyType == SignerPartyEnumType.第一当事人).SignKeyword;

                #region 获取实名认证信息
                var signer = await _contractSignerService.GetSigner(model.SignUnitName,model.SignUnitCompanyNo,true);

                if (signer == null)
                {
                    model.Status = CashSignStatusEnumType.生成取消;
                    model.FailMsg = $"{model.SignUnitName}认证信息获取失败，生成取消！";
                }
                model.SignUnitSealPath = signer.OssUrl;
                #endregion
            }

            if (fileConfig.Signers.Any(f => f.SignerPartyType == SignerPartyEnumType.第二当事人))
            {
                model.SealKey = fileConfig.Signers.First(f => f.SignerPartyType == SignerPartyEnumType.第二当事人).SignKeyword;
                #region 获取实名认证信息
                var signer = await _contractSignerService.GetSigner(model.Name, model.CardID, false);

                if (signer == null)
                {
                    model.Status = CashSignStatusEnumType.生成取消;
                    model.FailMsg = $"{model.Name}实名认证信息获取失败，生成取消！";
                }
                model.SealPath = signer.OssUrl;
                #endregion
            }

            #endregion
            // 

            await _cashSignRecordService.Add(model);
            if (model.Status == CashSignStatusEnumType.生成取消)
            {
                throw new ErrorCodeException(-1, model.FailMsg);
            }
            else
            {
                return Ok(model);
            }

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_CashSignRecord model)
        {
            if (!await _cashSignRecordService.Exist(f => f.ID == model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await _cashSignRecordService.Update(model);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateSignInfo([FromForm] Guid gid, [FromForm] CashSignStatusEnumType status, [FromForm] string signedPath,  [FromForm] string failMsg)
        {
            var record = await _cashSignRecordService.Get(f => f.GID == gid);
            record.Status = status;
            record.FailMsg = failMsg;
            record.SignedPath = signedPath;
            if (status== CashSignStatusEnumType.待存证)
            {
                record.SignTime = DateTime.Now;
            }
            await _cashSignRecordService.Update(record);
            return Ok(record);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await _cashSignRecordService.Exist(f => f.ID == id))
            {
                throw new ErrorCodeException(-1, "你要的数据不存在");
            }
            await _cashSignRecordService.Delete(f=>f.ID==id);
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string name, string customerName, string idCardNo, string signUnitName, CashSignStatusEnumType? status)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Live_CashSignRecord>();

            expable.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            expable.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            expable.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
            expable.AndIF(status.HasValue, f => f.Status == status.Value);
            var exp = expable.ToExpression();
            var dataList = await _cashSignRecordService.List(exp);
            var exportDataList = dataList.Select(f => new CashSignRecordExportDto()
            {
                CardID = f.CardID,
                SignUnitName = f.SignUnitName,
                CustomerName = f.CustomerName,
                Name = f.Name,
                SignTime = f.SignTime.HasValue ? f.SignTime.Value.ToString("yyyy-MM-dd") : "",
                ExpirationDate = f.ExpirationDate.HasValue ? f.ExpirationDate.Value.ToString("yyyy-MM-dd") : "",
                CashType = f.CashType,
                Phone = f.Phone,
                Sex = f.Sex,
                SignedPath = f.SignedPath,
                Status = f.Status,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "现金签署记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "现金签署记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

     
        [HttpPut]
        public async Task<IActionResult> SendSms([FromForm]int id)
        {
            var record = await _cashSignRecordService.Get(f => f.ID == id);

            if (record == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            if (record.Status!= CashSignStatusEnumType.待签署)
            {
                throw new ErrorCodeException(-1, "非【待签署】状态无法发送短信");
            }
            var smsRes = await _fxqService.SmsSend(record.Phone, record.SmsContent);
            if (smsRes != null && smsRes.Code == 10000)
            {
                record.SmsSendTime = smsRes.Data.Time;
                record.SmsMsgID = smsRes.Data.MsgId;
                record.Status = CashSignStatusEnumType.待签署;
                record.FailMsg = string.Empty;
            }
            else
            {
                record.Status = CashSignStatusEnumType.短信发送异常;
                record.FailMsg = smsRes?.Msg ?? string.Empty;
            }

            await _cashSignRecordService.Update(record);
            return Ok();
        }

        #region 网络签署
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> WebSignRecord(int id)
        {
            var record = await _cashSignRecordService.Get(f => f.ID == id);
            CashSignResponse response = new CashSignResponse()
            {
                KeyID = record.ID.ToString(),
                CustomerName = record.CustomerName,
                CardID = record.CardID,
                Name = record.Name,
                Phone = record.Phone,
                CashBase64 = string.Empty,
                CashPath = record.PdfPath,
                CashType = record.CashType,
                Sex = record.Sex,
                SignUnitName = record.SignUnitName,
                StatusName = "待签署"
            };

            if (!string.IsNullOrWhiteSpace(record.SignedPath))
            {
                response.CashPath = record.SignedPath;
                response.StatusName = "已签署";
            }
            if (response.StatusName == "待签署")
            {
                response.CashBase64 = await _fxqService.DownloadFile(response.CashPath);
            }
            else
            {
                response.CashBase64 = string.Empty;
            }
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Signing([FromForm] int id, [FromForm] string smsCode)
        {

            var setting = await _settingsService.GetMasterSettings();
            var record = await _cashSignRecordService.Get(f => f.ID == id);
            if (record == null)
            {
                throw new ErrorCodeException(-1, "合同文件不存在");
            }
            if (record.Status != CashSignStatusEnumType.待签署 && record.Status != CashSignStatusEnumType.签署异常)
            {
                throw new ErrorCodeException(-1, "此状态无法签署！");
            }

            if (string.IsNullOrWhiteSpace(smsCode))
            {
                throw new ErrorCodeException(-1, "验证码不能为空");
            }
            if (record.SmsCode != smsCode.Trim())
            {
                throw new ErrorCodeException(-1, "验证码不匹配");
            }

            record.FailMsg = string.Empty;
            List<KeywordSigner> signers = new List<KeywordSigner>();
            if (!string.IsNullOrWhiteSpace(record.SealKey))
            {
                KeywordSigner keywordSigner = new KeywordSigner(record.Name, record.CardID, record.SealPath, record.SealKey, 120, null);
                signers.Add(keywordSigner);
            }

            if (!string.IsNullOrWhiteSpace(record.SignUnitSealKey))
            {
                KeywordSigner keywordSigner = new KeywordSigner(record.SignUnitName, record.SignUnitCompanyNo, record.SignUnitSealPath, record.SignUnitSealKey, 120, null);
                signers.Add(keywordSigner);
            }

            var smResp = await _fxqService.KeywordPdfContractSign(record.PdfPath, signers.ToArray(), 1, null, null, new SignDate(1, null, null, null));
            if (smResp != null)
            {
                if (smResp.Code == 10000)
                {
                    record.SignedPath = smResp.Data;
                    record.SignTime = DateTime.Now;
                    record.Status = CashSignStatusEnumType.待存储;
                    record.FailMsg = string.Empty;
                }
                else
                {
                    record.Status = CashSignStatusEnumType.签署异常;
                    record.FailMsg = $"签章失败，异常信息：{smResp?.Msg}";
                }
            }
            else
            {
                record.Status = CashSignStatusEnumType.签署异常;
                record.FailMsg = "签署异常，请稍后再试";

            }

            if (record.Status == CashSignStatusEnumType.待存储)
            {
                if (!string.IsNullOrWhiteSpace(record.SignedPath))
                {
                    var pdfFileBase64 = await _fxqService.DownloadFile(record.SignedPath);
                    if (!string.IsNullOrWhiteSpace(pdfFileBase64))
                    {

                        using (var ms = new MemoryStream(Convert.FromBase64String(pdfFileBase64)))
                        {
                            var ossResp = await _aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf", true, false, "CashSignRecord");
                            if (ossResp.Code == 0)
                            {
                                record.SignedPath = setting.AliBucketDomain + ossResp.Data;
                                record.Status = CashSignStatusEnumType.待存证;
                                record.FailMsg = string.Empty;
                            }
                            else
                            {
                                record.Status = CashSignStatusEnumType.存储异常;
                                record.FailMsg = $"上传Oss异常";
                            }
                        }
                    }
                    else
                    {
                        record.Status = CashSignStatusEnumType.存储异常;
                        record.FailMsg = $"下载已签署文件异常！";
                    }
                }
            }
            await _cashSignRecordService.Update(record);
            if (record.Status == CashSignStatusEnumType.签署异常 || record.Status == CashSignStatusEnumType.存储异常)
            {
                throw new ErrorCodeException(-1, record.FailMsg);
            }
            return Ok(record);
        }
        #endregion

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> SendSmsBatch()
        //{
        //    var records = await _cashSignRecordService.List(f =>f.Status== CashSignStatusEnumType.待签署);
        //    foreach (var record in records)
        //    {
        //        var smsRes = await _fxqService.SmsSend(record.Phone, record.SmsContent);
        //        if (smsRes != null && smsRes.Code == 10000)
        //        {
        //            record.SmsSendTime = smsRes.Data.Time;
        //            record.SmsMsgID = smsRes.Data.MsgId;
        //            record.FailMsg = string.Empty;
        //        }
        //        else
        //        {
        //            record.Status = CashSignStatusEnumType.短信发送异常;
        //            record.FailMsg = smsRes?.Msg ?? string.Empty;
        //        }
        //        await _cashSignRecordService.Update(record);
        //    }

        //    return Ok();
        //}


    }
}
