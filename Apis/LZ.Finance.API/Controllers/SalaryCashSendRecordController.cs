using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core;
using LZ.Core.CommonHandle;
using LZ.Core.EventData.Cash;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using Org.BouncyCastle.Ocsp;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class SalaryCashSendRecordController : ApiControllerBase
    {

        private readonly IHttpService       _httpService;
        private readonly IAuthService       _authService;
        private readonly ISettingsService      _settingsService;
        private readonly ISalaryRecordService     _salarydRecordService;
        private readonly IFinanceOtherService    _otherService;
        private readonly IAliyunOSSService   _aliyunOSSService;
        private readonly ISalaryCashSendRecordService  _cashSendRecordService;
        private readonly IEventPublisher   _publisher;
        private readonly IFxqService    _fxqService;
        private readonly ISalaryDelaySendRecordService     _delaySendRecordService;
        private readonly ISalaryFailRecordService     _failRecordService;
        private readonly IUserEntryService     _entryService;

        public SalaryCashSendRecordController(ISalaryCashSendRecordService cashSendRecordService, IEventPublisher publisher, IFxqService fxqService, IAliyunOSSService aliyunOSSService, IFinanceOtherService otherService, ISalaryRecordService salarydRecordService, ISettingsService settingsService, IAuthService authService, ISalaryDelaySendRecordService delaySendRecordService, ISalaryFailRecordService failRecordService, IUserEntryService entryService, IHttpService httpService)
        {
            _cashSendRecordService = cashSendRecordService;
            _publisher = publisher;
            _fxqService = fxqService;
            _aliyunOSSService = aliyunOSSService;
            _otherService = otherService;
            _salarydRecordService = salarydRecordService;
            _settingsService = settingsService;
            _authService = authService;
            _delaySendRecordService = delaySendRecordService;
            _failRecordService = failRecordService;
            _entryService = entryService;
            _httpService = httpService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string signingUnitName, string name, string idCardNo, CashSendStatusEnumType? status, Guid? salaryCustomerID, SalaryCashAuditStatusEnumType? auditStatus, SalaryCashAgentStatusEnumType? agentStatus, SalaryRecordSendStatusEnumType? salaryRecordStatus, string? cashType, [FromQuery] PagedModelBase query)
        {

            var user = CurrentUser;
            Console.WriteLine(Guid.Empty);
            var exp = Expressionable.Create<Model.Live_V_SalaryCashSendRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(signingUnitName), it => it.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), it => it.CustomerName.Contains(customerName));
            exp.AndIF(auditStatus.HasValue, it => it.AuditStatus== (int)auditStatus.Value);
            exp.AndIF(agentStatus.HasValue, it => it.AgentStatus == (int)agentStatus.Value);
            exp.AndIF(salaryRecordStatus.HasValue, it => it.SalaryRecordStatus == (int)salaryRecordStatus.Value);
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, it => it.CompanyID == CurrentUser.CompanyID);
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(it => it.PartnerID == info.PartnerID);
                }
            }
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                exp.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await _authService.UserInfo<Model.Live_Personnel>();
                exp.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryCashSendRecord>(umodel));
            }
            exp.AndIF(salaryCustomerID.HasValue, it => it.SalaryCustomerID == salaryCustomerID.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(cashType), it => it.CashType.Contains(cashType));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), it => it.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), it => it.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, it => it.Status == status.Value);

            var models = await _cashSendRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


   

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _cashSendRecordService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }



        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _cashSendRecordService.GetView(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_SalaryCashSendRecord model)
        {
            await _cashSendRecordService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]Guid id)
        {

            var model = await _cashSendRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            var salaryRecord = await _salarydRecordService.Get(f => f.ID == model.SalaryRecordID.Value);
            if (salaryRecord!=null)
            {
                salaryRecord.IsCashSignSuc = false;
               await _salarydRecordService.Update(salaryRecord);
            }
            await _cashSendRecordService.Delete(f=>f.ID==id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> CashContract([FromForm]Guid id)
        {
            var model = await _cashSendRecordService.GetView(f => f.ID == id);
            CashContractEventData cashContractEventData = new CashContractEventData()
            {
                CustomerGID = model.CustomerGID.Value,
                SalaryCashSendRecordID = model.ID,
                 
            };
            _publisher.Publish(cashContractEventData);

            return Ok();
        }

        public record GetSigningRecordByUserIDResponse(Guid id,  string CustomerName, string SalaryMonth,string TotalAmount,string filePath);

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCashContractByUserID(int userID)
        {
            List<GetSigningRecordByUserIDResponse> cashList = new List<GetSigningRecordByUserIDResponse>();

            var records = await _cashSendRecordService.ListView(f => f.Status == CashSendStatusEnumType.待签署 && f.UserID == userID);
            if (records != null)
            {
                cashList= records.Select(f => new GetSigningRecordByUserIDResponse(f.ID, f.CustomerName, f.SalaryMonth.Value.ToString("yyyy-MM"), f.TotalAmount.ToString(),f.FilePath)).ToList();
            }
            return Ok(cashList);
        }
        [HttpPost]
        public async Task<IActionResult> ContractSigning( [FromForm]Guid id)
        {
            var modelView = await _cashSendRecordService.GetView(f => f.ID == id);
            var model = await _cashSendRecordService.Get(f => f.ID == id);

            var setting = await _settingsService.GetMasterSettings();

            if (string.IsNullOrWhiteSpace(model.SignerOssUrl))
            {
                Live_ContractSigner signerInfo = null;
                var signerResp = await _otherService.GetContractSignerByIdno(modelView.IdCardNo);
                if (signerResp != null && signerResp.Code == 200)
                {
                    signerInfo = signerResp.Data;
                }
                if (signerInfo == null)
                {
                    throw new ErrorCodeException(-1, "电子签章信息缺失");
                }
                model.SignerOssUrl = signerInfo.OssUrl;
                model.SignKeyword = "签章";
            }

            var signer = new KeywordSigner(modelView.Name, modelView.IdCardNo, modelView.SignerOssUrl, modelView.SignKeyword, 120, null);
            var signResp = await _fxqService.KeywordPdfContractSign(model.FilePath, new KeywordSigner[] { signer }, 1, null, null, new SignDate(1, null, null, null));
            if (signResp != null && signResp.Code == 10000)
            {
                model.Status = CashSendStatusEnumType.已签署;
                model.SucFilePath = signResp.Data;
                model.SignTime = DateTime.Now;
                model.Remark = string.Empty;

                await _cashSendRecordService.Update(model);
            }
            else
            {
                model.Status = CashSendStatusEnumType.签署失败;
                model.Remark = $"签章失败，异常信息：{signResp.Msg}";
                model.SignTime = null;
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(model.SucFilePath))
                {
                    var base64 = await _fxqService.DownloadFile(model.SucFilePath);
                    var fileBase64 = Convert.FromBase64String(base64);
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
                        if (ossResp.Code == 0)
                        {
                            model.SucFilePath = setting.AliBucketDomain + ossResp.Data;

                            var czres = await _fxqService.EvidenceFull(model.SucFilePath, "2", null, null);
                            if (czres != null && czres.Code == 10000)
                            {
                                model.CashHash = czres.Data.Hash;
                                model.Status = CashSendStatusEnumType.已存证;
                            }
                           

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.Remark = "存储到oss失败";
            }

            

            try
            {
                DbScoped.SugarScope.BeginTran();
                if (model.Status >= CashSendStatusEnumType.已签署)
                {
                    var salaryRecord = await _salarydRecordService.Get(f => f.ID == model.SalaryRecordID);
                    if (salaryRecord != null)
                    {
                        if (salaryRecord.IsCashSend)
                        {
                            salaryRecord.IsCashSignSuc = true;
                        }
                        _salarydRecordService.VerifyPendingSend(salaryRecord);
                        await _salarydRecordService.Update(salaryRecord);
                    }
                }
                await _cashSendRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }


            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadSignedFile(IFormFile file)
        {
            var setting =await _settingsService.GetMasterSettings();
            Guid id = Guid.Empty;
            if (HttpContext.Request.Form.Keys.Any(f => f == "id"))
            {
                var idString = HttpContext.Request.Form["id"].ToString();
                if (!string.IsNullOrWhiteSpace(idString))
                {
                    id = Guid.Parse(idString);
                }
            }

            if (id == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "记录标识获取失败");
            }

            var model = await _cashSendRecordService.Get(f => f.ID == id);


            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }


            var salaryRecord = await _salarydRecordService.Get(f=>f.ID==model.SalaryRecordID);
            if (salaryRecord==null)
            {
                throw new ErrorCodeException(-1, "关联的工资记录已被删除，请联系管理员！");

            }


            model.Status = CashSendStatusEnumType.已签署;
            string ossUrl = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var ossResp = await _aliyunOSSService.UploadFile(ms, file.FileName, true, false, "CashSendRecord");
                if (ossResp.Code == 0)
                {
                    model.SucFilePath = setting.AliBucketDomain + ossResp.Data;
                }
                else
                {
                    throw new ErrorCodeException(-1, "文件上传失败");
                }
            }

            try
            {
                salaryRecord.IsCashSignSuc = true;
                _salarydRecordService.VerifyPendingSend(salaryRecord);
                DbScoped.SugarScope.BeginTran();
                await _salarydRecordService.Update(salaryRecord);
                await _cashSendRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }
            return Ok();
        }



        [HttpPost]
        public async Task<IActionResult> SetDirectSignSuc([FromForm]Guid id)
        {
            var model = await _cashSendRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }


            var salaryRecord = await _salarydRecordService.Get(f => f.ID == model.SalaryRecordID);
            if (salaryRecord == null)
            {
                throw new ErrorCodeException(-1, "关联的工资记录已被删除，请联系管理员！");

            }
            model.Status = CashSendStatusEnumType.已签署;
            try
            {
                salaryRecord.IsCashSignSuc = true;
                _salarydRecordService.VerifyPendingSend(salaryRecord);
                DbScoped.SugarScope.BeginTran();
                await _salarydRecordService.Update(salaryRecord);
                await _cashSendRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm] Guid id, [FromForm] CashSendStatusEnumType status)
        {
            var model = await _cashSendRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            model.Status = status;
            var salaryRecord = await _salarydRecordService.GetView(f => f.ID == model.SalaryRecordID.Value);
            if (salaryRecord == null)
            {
                throw new ErrorCodeException(-1, "关联的工资记录已被删除，请联系管理员处理");
            }
            if (status == CashSendStatusEnumType.已签署)
            {
                if (string.IsNullOrWhiteSpace(model.SucFilePath))
                {
                    throw new ErrorCodeException(-1, "现金签收凭据缺失");
                }
            }
            if (model.Status == CashSendStatusEnumType.待生成签收单)
            {
                Live_CustomerContractConfig config = null;
                Live_ContractFileConfig fileConfig = null;
                Live_ContractSigner signer = null;
                var signerResp=await _otherService.GetContractSignerByIdno(salaryRecord.IdCardNo);
                if (signerResp!=null&& signerResp.Code==200)
                {
                    signer = signerResp.Data;
                }
                if (signer==null)
                {
                    throw new ErrorCodeException(-1, "员工未进行实名认证");
                }

                model.SignerOssUrl = signer.OssUrl;
                model.SignKeyword = "签章";
                var contractConfigResp = await _otherService.GetCustomerContractConfigByIdAsync(salaryRecord.CustomerGID.Value);
                if (contractConfigResp!=null&& contractConfigResp.Code==200)
                {
                    config = contractConfigResp.Data;
                }
                if (config!=null)
                {
                    var contractFileResp = await _otherService.GetContractFileConfigByIdAsync(contractConfigResp.Data.CashFileID.Value);
                    if (contractFileResp != null && contractFileResp.Code == 200)
                    {
                        fileConfig = contractFileResp.Data;
                    }
                }
                if (fileConfig != null)
                {
                    var cashSendRecord = await _cashSendRecordService.GetView(f => f.ID == id);
                    Dictionary<string, string> kv = new Dictionary<string, string>();
                    kv.Add("employeeName", cashSendRecord.Name);
                    kv.Add("employeeIdCardNo", cashSendRecord.IdCardNo);
                    kv.Add("employeePhone", cashSendRecord.Phone);
                    kv.Add("sealEmployeeCompanyName", cashSendRecord.SigningUnitCompanyName);
                    kv.Add("employeeCompanyName", cashSendRecord.CustomerName);
                    kv.Add("salaryMonth", cashSendRecord.SalaryMonth.Value.ToString("yyyy年MM月"));
                    kv.Add("totalAmount", cashSendRecord.TotalAmount.ToString());
                    kv.Add("totalAmountWord", Tool.MoneyToUpper(cashSendRecord.TotalAmount.ToString()));
                    kv = kv.Where(f => !string.IsNullOrWhiteSpace(f.Value)).OrderBy(f => f.Key).ToDictionary();
                    var fxqResponse = await _fxqService.DocxKeywordReplace(fileConfig.FilePath, kv, 0);
                    if (fxqResponse != null && fxqResponse.Code == 10000)
                    {
                        // 转换成pdf
                        var pdfRes = await _fxqService.DocxToPdf(fxqResponse.Data);
                        if (pdfRes != null && pdfRes.Code == 1000)
                        {
                            var fileBytes = Convert.FromBase64String(pdfRes.Data);
                            using (var ms = new MemoryStream(fileBytes))
                            {
                                var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
                                if (ossResp.Code == 0)
                                {

                                    model.Status = CashSendStatusEnumType.待签署;
                                    model.FilePath = "https://oss.cn-labor.cn" + ossResp.Data;
                                    model.Msg = string.Empty;
                                }
                                else
                                {
                                    throw new ErrorCodeException(-1, $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}");
                                }
                            }
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, $"合同文件Word转换成Pdf文件失败,异常信息:{fxqResponse?.Msg}");
                        }
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, $"关键词替换失败,异常信息:{fxqResponse.Msg}");
                    }
                }
            }
            await _cashSendRecordService.Update(model);
            return Ok();
        }

        /// <summary>
        ///  现金发放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> DelayRecordToCashSend([FromForm] Guid salaryRecordID, [FromForm] string cashRemark, [FromForm] Guid delayRecordID,[FromForm] string agentName, [FromForm] string agentBank, [FromForm] string agentBankNo, [FromForm] string agentIdCardNo)
        {

            var model = await _salarydRecordService.Get(f => f.ID == salaryRecordID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.IsLocked)
            {
                throw new ErrorCodeException(-1, "此工资记录已被锁定无法执行任何修改操作");
            }

            if (model.IsCashSend)
            {
                throw new ErrorCodeException(-1, "当前工资记录已使用现金发放，无法重复操作！");
            }

            if (model.Status == SalaryRecordSendStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "已发放工资无法执行此操作");
            }
            #region 暂缓处理
            var delaySendRecord = await _delaySendRecordService.Get(f => f.ID == delayRecordID);

            if (delaySendRecord.DelayType== SalaryDelaySendTypeEnumType.网签暂缓)
            {
                throw new ErrorCodeException(-1, "【网签暂缓】无法执行此操作！");
            }
            delaySendRecord.Status = SalaryDelaySendStatusEnumType.已处理;
            delaySendRecord.IsToCash = true;
            //工资记录已处理
            model.IsDelayPaymentSuc = true;
            #endregion
            Live_SalaryCashSendRecord cashSendRecord = new Live_SalaryCashSendRecord()
            {
                CreateTime = DateTime.Now,
                Reason = "客服生成",
                Remark = "",
                FilePath = string.Empty,
                AgentBank = agentBank,
                AgentBankNo = agentBankNo,
                AgentIdCardNo = agentIdCardNo,
                AgentName = agentName,
                SalaryCustomerID = model.SalaryCustomerID,
                SalaryRecordID = model.ID,
                SendAmount = 0,
                SignerOssUrl = string.Empty,
                SignKeyword = string.Empty,
                SignTime = null,
                Status = CashSendStatusEnumType.待处理,
                SucFilePath = string.Empty,
                TotalAmount = model.ActualSalarySendCardAmount,
                ID = Guid.NewGuid(),
                CashType = "临时白名单",
                AuditStatus = 0,
                CashRemark = cashRemark,
                AgentStatus= SalaryCashAgentStatusEnumType.待转账
            };
            cashSendRecord.DelayRecordID = delayRecordID;
            cashSendRecord.FromType = "暂缓发放";
            try
            {
                DbScoped.SugarScope.BeginTran();
             
                model.Status = SalaryRecordSendStatusEnumType.待处理;
                model.IsCashSend = true;
                model.IsCashSignSuc = false;
                model.SendType = SalaryRecordSendTypeEnumType.线下发放;
                model.IsSingleSend = true;
                _salarydRecordService.VerifyPendingSend(model);
                await _cashSendRecordService.Add(cashSendRecord);
                await _delaySendRecordService.Update(delaySendRecord);
                await _salarydRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }
            return Ok();
        }

        /// <summary>
        ///  现金发放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> FailRecordToCashSend([FromForm] Guid salaryRecordID, [FromForm] string cashRemark, [FromForm] Guid failRecordID, [FromForm] string agentName, [FromForm] string agentBank, [FromForm] string agentBankNo, [FromForm] string agentIdCardNo)
        {

            var model = await _salarydRecordService.Get(f => f.ID == salaryRecordID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            if (model.IsCashSend)
            {
                throw new ErrorCodeException(-1, "当前工资记录已使用现金发放，无法重复操作！");
            }

            if (model.Status == SalaryRecordSendStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "已发放工资无法执行此操作");
            }
            #region 异常工资
            var failSendRecord = await _failRecordService.Get(f => f.ID == failRecordID);
            failSendRecord.Status = SalaryFailRecordStatusEnumType.已处理;
            failSendRecord.IsToCash = true;
            //工资记录已处理



            #endregion
            Live_SalaryCashSendRecord cashSendRecord = new Live_SalaryCashSendRecord()
            {
                CreateTime = DateTime.Now,
                Reason = "客服生成",
                Remark = "",
                FilePath = string.Empty,
                AgentBank = agentBank,
                AgentBankNo = agentBankNo,
                AgentIdCardNo = agentIdCardNo,
                AgentName = agentName,
                SalaryCustomerID = model.SalaryCustomerID,
                SalaryRecordID = model.ID,
                SendAmount = 0,
                SignerOssUrl = string.Empty,
                SignKeyword = string.Empty,
                SignTime = null,
                Status = CashSendStatusEnumType.待处理,
                SucFilePath = string.Empty,
                TotalAmount = model.ActualSalarySendCardAmount,
                ID = Guid.NewGuid(),
                CashType = "临时白名单",
                AuditStatus = 0,
                CashRemark = cashRemark,
                AgentStatus = SalaryCashAgentStatusEnumType.待转账
            };
            cashSendRecord.FailRecordID = failRecordID;
            cashSendRecord.FromType = "异常工资";
            try
            {
                DbScoped.SugarScope.BeginTran();
                model.IsFailSalarySuc = true;
                model.Status = SalaryRecordSendStatusEnumType.待处理;
                model.IsCashSend = true;
                model.IsCashSignSuc = false;
                model.SendType = SalaryRecordSendTypeEnumType.线下发放;
                model.IsSingleSend = true;


                _salarydRecordService.VerifyPendingSend(model);
                await _cashSendRecordService.Add(cashSendRecord);
                await _failRecordService.Update(failSendRecord);
                await _salarydRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }
            return Ok();
        }




        [HttpPut]
        public async Task<IActionResult> UpdateAuditStatus([FromForm] Guid id, [FromForm] SalaryCashAuditStatusEnumType auditStatus)
        {
            var cashSendRecord = await _cashSendRecordService.Get(f => f.ID == id);
            if (cashSendRecord == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            var salaryRecord = await _salarydRecordService.Get(f => f.ID == cashSendRecord.SalaryRecordID);

            Live_SalaryDelaySendRecord delayRecord = null;
            Live_SalaryFailRecord failRecord = null;
            if (auditStatus == SalaryCashAuditStatusEnumType.审核未通过)
            {
                cashSendRecord.AuditStatus = SalaryCashAuditStatusEnumType.审核未通过;
                cashSendRecord.Status =  CashSendStatusEnumType.作废;
                if (cashSendRecord.DelayRecordID.HasValue)
                {
                    delayRecord = await _delaySendRecordService.Get(f => f.ID == cashSendRecord.DelayRecordID);
                    delayRecord.Status = SalaryDelaySendStatusEnumType.待系统处理;
                    delayRecord.IsToCash = false;

                    // 取消现金
                    salaryRecord.IsCashSend = false;
                    salaryRecord.IsCashSignSuc = false;
                    // 重新标记暂缓
                    salaryRecord.IsDelayPayment = true;
                    salaryRecord.IsDelayPaymentSuc = false;
                    salaryRecord.SendType = SalaryRecordSendTypeEnumType.线上发放;


                }
                else
                {
                    failRecord = await _failRecordService.Get(f => f.ID == cashSendRecord.FailRecordID.Value);
                    failRecord.Status = SalaryFailRecordStatusEnumType.待处理;
                    failRecord.IsToCash = false;
                    salaryRecord.IsFailSalarySuc = false;
                    salaryRecord.IsCashSend = false;
                    salaryRecord.IsCashSignSuc = false;

                }

                try
                {
                    DbScoped.SugarScope.BeginTran();
                    if (delayRecord != null)
                    {
                        await _delaySendRecordService.Update(delayRecord);

                    }
                    if (failRecord != null)
                    {
                        await _failRecordService.Update(failRecord);
                    }
                    await _cashSendRecordService.Update(cashSendRecord);
                    _salarydRecordService.VerifyPendingSend(salaryRecord);
                    await _salarydRecordService.Update(salaryRecord);
                    DbScoped.SugarScope.CommitTran();
                }
                catch (Exception ex)
                {
                    DbScoped.SugarScope.RollbackTran();
                    throw new ErrorCodeException(-1, ex.Message);
                }

            }
            else if (auditStatus == SalaryCashAuditStatusEnumType.审核通过)
            {
               
                #region 生成现金签署记录

               var entry=await _entryService.GetView(f => f.ID == salaryRecord.UserEntryID);

                //if (entry.UserID==null||entry.UserID.Value<=0)
                //{
                //    throw new ErrorCodeException(-1, "此员工非线上实名员工，无法签署现金签收单，不符合现金发放条件！");
                //}

                var monthEnd = DateTime.Today.AddDays(1-DateTime.Today.Day).AddMonths(1).AddDays(-1);
                if (entry != null)
                {

                    if (entry.UserID != null && entry.UserID.Value > 0)
                    {
                        CashSignRecordDto record = new CashSignRecordDto()
                        {
                            Name = entry.Name,
                            CardID = entry.CardID,
                            Sex = entry.Sex,
                            Age = entry.Age ?? 0,
                            Phone = entry.Phone,
                            CompanyID = entry.CompanyID ?? 0,
                            CompanyName = entry.CompanyName,
                            CustomerID = entry.CustomerID.Value,
                            CustomerName = entry.CustomerName,
                            PartnerID = entry.PartnerID ?? 0,
                            PartnerName = entry.PartnerName,
                            SignUnitName = entry.SignName,
                            UserEntryID = entry.ID,
                            UserID = entry.UserID ?? 0,
                            FailMsg = string.Empty,
                            CashType = "临时白名单",
                            Status = Model.Enums.CashSignStatusEnumType.待生成,
                            ExpirationDate = monthEnd,
                            SignUnitCompanyNo = entry.SignCompanyNo,
                        };
                        var res = await _httpService.PostAsync<Live_CashSignRecord>(ServiceNames.CustomerService, "/api/CashSignRecord/Add", JsonConvert.SerializeObject(record));
                        if (res != null && res.Code == 200)
                        {

                            cashSendRecord.AuditStatus = SalaryCashAuditStatusEnumType.审核通过;
                            salaryRecord.BankUserName = cashSendRecord.AgentName;
                            salaryRecord.Bank = cashSendRecord.AgentBank;
                            salaryRecord.BankNo = cashSendRecord.AgentBankNo;
                            salaryRecord.CashTypeName = "临时白名单";
                            cashSendRecord.Status = CashSendStatusEnumType.待签署;
                            try
                            {
                                DbScoped.SugarScope.BeginTran();
                                await _cashSendRecordService.Update(cashSendRecord);
                                _salarydRecordService.VerifyPendingSend(salaryRecord);
                                await _salarydRecordService.Update(salaryRecord);
                                DbScoped.SugarScope.CommitTran();
                            }
                            catch (Exception ex)
                            {
                                DbScoped.SugarScope.RollbackTran();
                                throw new ErrorCodeException(-1, ex.Message);
                            }
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, res?.Message);
                        }

                    }
                    else {
                        cashSendRecord.AuditStatus = SalaryCashAuditStatusEnumType.审核通过;
                        salaryRecord.BankUserName = cashSendRecord.AgentName;
                        salaryRecord.Bank = cashSendRecord.AgentBank;
                        salaryRecord.BankNo = cashSendRecord.AgentBankNo;
                        salaryRecord.CashTypeName = "线下-临时白名单";
                        cashSendRecord.Status = CashSendStatusEnumType.待签署;
                        try
                        {
                            DbScoped.SugarScope.BeginTran();
                            await _cashSendRecordService.Update(cashSendRecord);
                            _salarydRecordService.VerifyPendingSend(salaryRecord);
                            await _salarydRecordService.Update(salaryRecord);
                            DbScoped.SugarScope.CommitTran();
                        }
                        catch (Exception ex)
                        {
                            DbScoped.SugarScope.RollbackTran();
                            throw new ErrorCodeException(-1, ex.Message);
                        }
                    }
                   
                }
                else {
                    throw new ErrorCodeException(-1, "员工信息获取失败，无法生成现金签署记录");
                }
                #endregion
            }
            else
            {
                throw new ErrorCodeException(-1, "无法执行当前操作");
            }

            return Ok();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadAgentTransFile(IFormFile file,[FromForm] Guid id)
        {
            var setting = await _settingsService.GetMasterSettings();
            var model=await  _cashSendRecordService.Get(f => f.ID == id);
            if (model.AgentStatus!=  SalaryCashAgentStatusEnumType.待转账)
            {
                throw new ErrorCodeException(-1, "此状态无法执行此操作！");
            }
            var record=await  _salarydRecordService.Get(f => f.ID == model.SalaryRecordID);
            if (record.Status!= SalaryRecordSendStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "此工资记录未发放，无法执行此操作");
            }
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var ossResp = await _aliyunOSSService.UploadFile(ms, file.FileName, true, false, "CashSendAgentTrans");
                if (ossResp.Code == 0)
                {
                    model.AgentTransPath = setting.AliBucketDomain + ossResp.Data;
                    model.AgentStatus = SalaryCashAgentStatusEnumType.已转账;
                    model.AgentTransTime = DateTime.Now;
                    await _cashSendRecordService.Update(model);
                }
                else
                {
                    throw new ErrorCodeException(-1, "文件上传失败");
                }
            }

            return Ok();

        }

        [HttpPut]
        public async Task<IActionResult> UpdateAgentStatus([FromForm] Guid id, [FromForm] SalaryCashAgentStatusEnumType status)
        {
            var model = await _cashSendRecordService.Get(f => f.ID == id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "数据不存在！");
            }
            if (model.AgentStatus != SalaryCashAgentStatusEnumType.待转账)
            {
                throw new ErrorCodeException(-1, "非【待转账】状态无法执行此操作！");
            }
            var record = await _salarydRecordService.Get(f => f.ID == model.SalaryRecordID);
            if (record.Status != SalaryRecordSendStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "此工资记录未发放，无法执行此操作");
            }
            model.AgentStatus = status;
            model.AgentTransTime = DateTime.Now;
            await _cashSendRecordService.Update(model);
            return Ok();

        }
    }
}
