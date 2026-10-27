using LZ.Core;
using LZ.Core.CommonHandle;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SalaryDelaySendRecordController : ApiControllerBase
    {

        private readonly IUserEntryService   _userEntryService;
        private readonly ISalaryCustomerService  _salaryCustomer;
        private readonly ISalaryDelaySendRecordService  _delaySendRecordService;
        private readonly ISalaryRecordService   _salaryRecordService;
        private readonly IAuthService    _authService;

        public SalaryDelaySendRecordController(ISalaryDelaySendRecordService delaySendRecordService, ISalaryRecordService salaryRecordService, ISalaryCustomerService salaryCustomer, IAuthService authService, IUserEntryService userEntryService)
        {
            _delaySendRecordService = delaySendRecordService;
            _salaryRecordService = salaryRecordService;
            _salaryCustomer = salaryCustomer;
            _authService = authService;
            _userEntryService = userEntryService;
        }


        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, string name,string idCardNo, SalaryDelaySendStatusEnumType? status, Guid? salaryCustomerID,[FromQuery] PagedModelBase query)
        {
            Console.WriteLine(Guid.Empty);
            var exp = Expressionable.Create<Model.Live_V_SalaryDelaySendRecord>();



            var user = CurrentUser;
            SYS_CompanyUser info = null;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                 info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                exp.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await _authService.UserInfo<Model.Live_Personnel>();
                exp.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryDelaySendRecord>(umodel));
                exp.And(f => f.Status != SalaryDelaySendStatusEnumType.已处理);
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



            exp.AndIF(salaryCustomerID.HasValue, f => f.SalaryCustomerID == salaryCustomerID.Value);
            exp.AndIF(!name.IsNullOrWhiteSpace(), f => f.Name.Contains(name));
            exp.AndIF(!idCardNo.IsNullOrWhiteSpace(), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!customerName.IsNullOrWhiteSpace(), f => f.CustomerName.Contains(customerName));
            exp.AndIF(status.HasValue, f => f.Status == (SalaryDelaySendStatusEnumType)status.Value);

            var models = await _delaySendRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _delaySendRecordService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _delaySendRecordService.GetView(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_SalaryDelaySendRecord model)
        {
            await _delaySendRecordService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]Guid id)
        {
            await _delaySendRecordService.Delete(f=>f.ID==id);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm] Guid id,[FromForm] SalaryDelaySendStatusEnumType status)
        {
            var model = await _delaySendRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            model.Status = status;
            var salaryRecord = await _salaryRecordService.Get(f => f.ID == model.SalaryRecordID.Value);
            if (salaryRecord == null) {
                throw new ErrorCodeException(-1, "关联的工资记录已被删除，请联系管理员处理");
            }
            if (salaryRecord.UserEntryID > 0)
            {
                var entryEntity = await _userEntryService.GetView(f => f.ID == salaryRecord.UserEntryID);
                if (entryEntity != null)
                {
                    if (!salaryRecord.IsCashSend)
                    {
                        salaryRecord.Bank = entryEntity.BankName;
                        salaryRecord.BankNo = entryEntity.BankNumber;
                        salaryRecord.BankUserName = entryEntity.Name;
                    }
                    else
                    {
                        salaryRecord.Bank = entryEntity.AgentBank;
                        salaryRecord.BankNo = entryEntity.AgentBankNo;
                        salaryRecord.BankUserName = entryEntity.AgentName;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(salaryRecord.Bank) || string.IsNullOrWhiteSpace(salaryRecord.BankNo) || string.IsNullOrWhiteSpace(salaryRecord.BankUserName))
            {
                if (salaryRecord.IsCashSend)
                {
                    throw new ErrorCodeException(-1, "工资记录采用【现金发放】，银行卡代理人信息缺失！");
                }
                else
                {

                    throw new ErrorCodeException(-1, "银行卡信息缺失,请修改员工信息或手动输入！");
                }
            }

            if (salaryRecord.Status== SalaryRecordSendStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "工资记录已发放无法更新！");
            }

            // 暂缓处理完成
            salaryRecord.IsDelayPaymentSuc = true;
            if (salaryRecord.IsCashSend && !salaryRecord.IsCashSignSuc)
            {
                salaryRecord.Status = SalaryRecordSendStatusEnumType.待处理;
            }
            else {
                salaryRecord.Status = SalaryRecordSendStatusEnumType.待发放;
            }

            try
            {
                DbScoped.SugarScope.BeginTran();

                _salaryRecordService.VerifyPendingSend(salaryRecord);
                await _salaryRecordService.Update(salaryRecord);
                await _delaySendRecordService.UpdateRecord(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw  new ErrorCodeException(-1,$"处理异常，异常信息：{ex.Message}");
            }



            return Ok();
        }




    }
}
