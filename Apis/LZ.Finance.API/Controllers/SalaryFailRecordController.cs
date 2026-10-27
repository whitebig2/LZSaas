using LZ.Core;
using LZ.Core.CommonHandle;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;
using System.Security.Policy;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SalaryFailRecordController : ApiControllerBase
    {

        private readonly IEventPublisher _eventPublisher;
        private readonly IUserEntryService _userEntryService;
        private readonly IAuthService _authService;
        private readonly ISalaryCashSendRecordService _cashSendRecordService;
        private readonly IFinanceOtherService _financeOtherService;
        private readonly ISalaryFailRecordService _failRecordService;
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly ISalaryConfigService _salaryConfigService;

        public SalaryFailRecordController(ISalaryFailRecordService failRecordService, ISalaryRecordService salaryRecordService, IFinanceOtherService financeOtherService, ISalaryCashSendRecordService cashSendRecordService, IAuthService authService, IUserEntryService userEntryService, ISalaryConfigService salaryConfigService, IEventPublisher eventPublisher)
        {
            _failRecordService = failRecordService;
            _salaryRecordService = salaryRecordService;
            _financeOtherService = financeOtherService;
            _cashSendRecordService = cashSendRecordService;
            _authService = authService;
            _userEntryService = userEntryService;
            _salaryConfigService = salaryConfigService;
            _eventPublisher = eventPublisher;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, string name, string idCardNo, SalaryFailRecordStatusEnumType? status, Guid? salaryCustomerID, [FromQuery] PagedModelBase query)
        {
            Console.WriteLine(Guid.Empty);
            var exp = Expressionable.Create<Model.Live_V_SalaryFailRecord>();
            var user = CurrentUser;
            SYS_CompanyUser info = null;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                 info = await _authService.UserInfo<Model.SYS_CompanyUser>();
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
                exp.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryFailRecord>(umodel));
                exp.And(it => it.Status == SalaryFailRecordStatusEnumType.待处理);
                status = null;
            }
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                if (info == null)
                {
                    info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                }

                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }


            exp.AndIF(salaryCustomerID.HasValue, it => it.SalaryCustomerID == salaryCustomerID.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), it => it.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), it => it.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, it => it.Status == status.Value);
            var models = await _failRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _failRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _failRecordService.GetView(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Guid id, [FromForm] string toBankNo, [FromForm] string toBank, [FromForm] string toBankUserName, [FromForm] bool isManual = false)
        {

            var model = await _failRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status == SalaryFailRecordStatusEnumType.已处理)
            {
                throw new ErrorCodeException(-1, "【已处理】记录无法重复操作！");
            }

            if (string.IsNullOrWhiteSpace(toBank) || string.IsNullOrWhiteSpace(toBankUserName))
            {
                throw new ErrorCodeException(-1, "信息缺失，请填写银行卡信息");
            }

            var record = await _salaryRecordService.Get(f => f.ID == model.SalaryRecordID);
            if (record != null)
            {
                if (record.BankUserName != toBankUserName)
                {
                    throw new ErrorCodeException(-1, "此处无法修改银行卡所属人");
                }
                record.Bank = toBank;
                record.BankNo = toBankNo;
                record.IsFailSalarySuc = true;

            }

            model.ToBank = toBank;
            model.ToBankNo = toBankNo;
            model.ToBankUserName = toBankUserName;

            // 非客服确认
            if (!isManual)
            {
                if (model.BankUserName.Equals(model.ToBankUserName) && model.Bank.Equals(model.ToBank) && model.BankNo.Equals(model.ToBankNo))
                {
                    throw new ErrorCodeException(-1, $"因银行卡信息未发生改变,系统无法验证信息的可用性，如银行卡信息是正确的信息，请手动点击【信息确认】按钮！");
                }
            }
            try
            {
                DbScoped.SugarScope.BeginTran();
                model.Status = SalaryFailRecordStatusEnumType.已处理;

                if (record != null)
                {
                    if (record.Status == SalaryRecordSendStatusEnumType.发放异常)
                    {
                        record.Status = SalaryRecordSendStatusEnumType.待发放;

                        await _salaryRecordService.Update(record);
                    }

                }
                await _failRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
                return Ok();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }

        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _failRecordService.Delete(f => f.ID == id);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm] Guid id)
        {
            var model = await _failRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status == SalaryFailRecordStatusEnumType.已处理)
            {
                throw new ErrorCodeException(-1, "【已处理】记录无法重复操作！");
            }
            if (string.IsNullOrWhiteSpace(model.ToBank) || string.IsNullOrWhiteSpace(model.ToBankUserName) || string.IsNullOrWhiteSpace(model.ToBankUserName))
            {
                throw new ErrorCodeException(-1, "信息缺失，请填写银行卡信息");
            }


            var record = await _salaryRecordService.Get(f => f.ID == model.SalaryRecordID);
            if (record != null)
            {
                if (record.BankUserName != model.ToBankUserName)
                {
                    throw new ErrorCodeException(-1, "此处无法修改银行卡所属人");
                }
                record.Bank = model.ToBank;
                record.BankNo = model.ToBankNo;
            }

            try
            {
                DbScoped.SugarScope.BeginTran();
                if (record != null)
                {
                    await _salaryRecordService.Update(record);
                }
                model.Status = SalaryFailRecordStatusEnumType.已处理;
                await _failRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
                return Ok();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }


    
        }



        [HttpPut]
        public async Task<IActionResult> SetCashSend([FromForm] Guid id)
        {
            var model = await _failRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            if (model.Status != SalaryFailRecordStatusEnumType.待处理)
            {
                throw new ErrorCodeException(-1, "此状态无法操作");
            }
            model.Status = SalaryFailRecordStatusEnumType.已处理;
            var salaryRecord = await _salaryRecordService.Get(f => f.ID == model.SalaryRecordID);
            if (salaryRecord == null)
            {
                throw new ErrorCodeException(-1, "关联的工资记录已被删除，请联系管理员处理");
            }
            if (salaryRecord.IsCashSend)
            {
                throw new ErrorCodeException(-1, "无法处理现金发放记录");
            }
            model.IsToCash = true;

            var customerResp = await _financeOtherService.GetCustomerByNameAsync(salaryRecord.CustomerName);

            Live_V_Customer customer = null;
            if (customerResp != null && customerResp.Code == 200)
            {
                customer = customerResp.Data;
            }
            if (customer == null)
            {
                throw new ErrorCodeException(-1, $"{salaryRecord.CustomerName} 客户单位信息获取失败");
            }
            salaryRecord.IsCashSend = true;
            salaryRecord.IsCashSignSuc = false;
            salaryRecord.Status = SalaryRecordSendStatusEnumType.待处理;
            salaryRecord.IsFailSalarySuc = true;

            salaryRecord.BankUserName = customer.AgentName;
            salaryRecord.BankNo = customer.AgentBankNo;
            salaryRecord.Bank = customer.AgentBank;
            if (string.IsNullOrEmpty(salaryRecord.BankUserName) || string.IsNullOrEmpty(salaryRecord.BankNo) || string.IsNullOrEmpty(salaryRecord.Bank))
            {
                throw new ErrorCodeException(-1, "银行卡代理人信息缺失，操作失败！");
            }

            salaryRecord.SendType = SalaryRecordSendTypeEnumType.线下发放;
            Live_SalaryCashSendRecord cashSendRecord = new Live_SalaryCashSendRecord()
            {
                CreateTime = DateTime.Now,
                Reason = "异常工资生成",
                Remark = "",
                FilePath = string.Empty,
                AgentBank = customer.AgentBank,
                AgentBankNo = customer.AgentBankNo,
                AgentIdCardNo = customer.AgentIdCardNo,
                AgentName = customer.AgentName,
                SalaryCustomerID = salaryRecord.SalaryCustomerID,
                SalaryRecordID = salaryRecord.ID,
                SendAmount = 0,
                SignerOssUrl = string.Empty,
                SignKeyword = string.Empty,
                SignTime = null,
                Status = CashSendStatusEnumType.待处理,
                SucFilePath = string.Empty,
                TotalAmount = salaryRecord.ActualSalarySendCardAmount,
                CashType = "临时白名单",
                ID = Guid.NewGuid(),
                FailRecordID = model.ID
            };

            try
            {
                DbScoped.SugarScope.BeginTran();
                await _cashSendRecordService.Add(cashSendRecord);
                await _salaryRecordService.Update(salaryRecord);
                await _failRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存失败，异常信息：{ex.Message}");
            }



            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListByUserID(int userID)
        {
            var models = await _failRecordService.ListView(f => f.UserID == userID && f.Status == SalaryFailRecordStatusEnumType.待处理);
            return Ok(models);
        }
        public record UpdateBankInfoRequest(List<Guid> RecordIds, string BankUserName, string Bank, string BankNo);

        [HttpPost]
        public async Task<IActionResult> UpdateBankInfo(UpdateBankInfoRequest request)
        {
            var modelViews = await _failRecordService.ListView(f => request.RecordIds.Contains(f.ID) && f.Status == SalaryFailRecordStatusEnumType.待处理);


            if (string.IsNullOrWhiteSpace(request.Bank) || string.IsNullOrWhiteSpace(request.BankNo) || string.IsNullOrWhiteSpace(request.BankUserName))
            {
                throw new ErrorCodeException(-1, "银行卡信息不能为空！");
            }
            foreach (var viewRecord in modelViews)
            {

                var failRecord = await _failRecordService.Get(f => f.ID == viewRecord.ID && f.Status == SalaryFailRecordStatusEnumType.待处理);
                if (failRecord == null)
                {
                    continue;
                }
                if (viewRecord.BankUserName != request.BankUserName)
                {
                    throw new ErrorCodeException(-1, "检测到银行卡所属人已变更，请联系驻场管理！");
                }

                if (failRecord.BankNo == request.BankNo && failRecord.Bank == request.Bank && failRecord.BankUserName == request.BankUserName)
                {
                    throw new ErrorCodeException(-1, "请信息修改后再保存！");
                }

                //failRecord.Bank = request.Bank;
                //failRecord.BankNo = request.BankNo;
                //failRecord.BankUserName = request.BankUserName;
                //failRecord.Remark = "小程序端修改";
                var salaryRecord = await _salaryRecordService.Get(f => f.ID == failRecord.SalaryRecordID && f.Status == SalaryRecordSendStatusEnumType.发放异常);
                if (salaryRecord == null)
                {
                    continue;
                }

                salaryRecord.BankUserName = request.BankUserName;
                salaryRecord.Bank = request.Bank;
                salaryRecord.BankNo = request.BankNo;

                DbScoped.SugarScope.BeginTran();
                failRecord.Status = SalaryFailRecordStatusEnumType.已处理;
                salaryRecord.Status = SalaryRecordSendStatusEnumType.待发放;
                await _failRecordService.Update(failRecord);
                await _salaryRecordService.Update(salaryRecord);
                DbScoped.SugarScope.CommitTran();





            }


            return Ok();
        }



        [HttpPut]
        public async Task<IActionResult> UpdateUserEntryBank([FromForm] Guid id)
        {
            var user = CurrentUser;
            var record = await _failRecordService.GetView(f => f.ID == id);

            if (record.UserEntryID == null || record.UserEntryID.Value <= 0)
            {
                throw new ErrorCodeException(-1, "此工资记录未关联入离职记录，无法使用此功能！");
            }

            if (string.IsNullOrWhiteSpace(record.SalaryBank) || string.IsNullOrWhiteSpace(record.SalaryBank))
            {
                throw new ErrorCodeException(-1, "关联的工资记录 银行卡信息缺失");
            }

            if (record.SalaryBankUserName != record.Name)
            {
                throw new ErrorCodeException(-1, "银行卡所属人与员姓名不一致，无法使用此功能！");
            }


            var model = await _userEntryService.Get(f => f.ID == record.UserEntryID.Value);

            string oldBankName = model.BankName;
            string oldBankNo = model.BankNumber;
            model.BankName = record.SalaryBank;
            model.BankNumber = record.SalaryBankNo;
            await _userEntryService.UpdateBank(model);

            var entryRecord = new EntryRecordEventData();
            entryRecord.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
            entryRecord.EntryID = model.ID;
            entryRecord.PerUserID = user.UserID;
            entryRecord.Status = model.Status.Value;
            entryRecord.About = "异常工资更改银行卡操作！银行卡（" + oldBankNo + "）修改为(" + model.BankNumber + ")，所属行（" + oldBankName + "）修改为(" + model.BankNumber + ")";
            entryRecord.CID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
            entryRecord.OperateFrom = user.PlatformType;
            entryRecord.OperateFullName = user.FullName;
            entryRecord.OperateUserID = user.UserID;
            _eventPublisher.Publish(entryRecord);


            return Ok();
        }

    }
}
