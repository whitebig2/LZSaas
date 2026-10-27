using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class SalarySplitSendController : ApiControllerBase
    {

        private readonly IFinanceOtherService  _otherService    ;
        private readonly ISalarySplitSendService  _splitSendService;
        private readonly ISalaryDelaySendRecordService  _delaySendRecordService;
        private readonly ISalaryRecordService  _salaryRecordService;
        private readonly IAuthService _authService;

        public SalarySplitSendController(ISalarySplitSendService splitSendService, ISalaryDelaySendRecordService delaySendRecordService, ISalaryRecordService salaryRecordService, IFinanceOtherService otherService, IAuthService authService)
        {
            _splitSendService = splitSendService;
            _delaySendRecordService = delaySendRecordService;
            _salaryRecordService = salaryRecordService;
            _otherService = otherService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string partnerName, string customerName, string name, string idCardNo,SalarySplitSendStatusEnumType? status, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_SalarySplitSendExtension>();

            var user = CurrentUser;

            if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {
               
                  var  info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }
           

            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(status.HasValue, f => f.Status== (int)status.Value);
            var models = await _splitSendService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

     


        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Guid id, [FromForm] string bank, [FromForm] string bankNo, [FromForm] string bankUserName)
        {
            var model= await _splitSendService.Get(f => f.ID == id);

           
            if (model == null)
            {
                throw new ErrorCodeException(-1, "记录获取失败");
            }

            if (model.Status == SalarySplitSendStatusEnumType.已处理)
            {
                throw new ErrorCodeException(-1, "已处理记录禁止修改");
            }

            if (!string.IsNullOrWhiteSpace(bank))
            {
                model.Bank = bank;
            }
            if (!string.IsNullOrWhiteSpace(bankNo))
            {
                model.BankNo = bankNo;
            }
            if (!string.IsNullOrWhiteSpace(bankUserName))
            {
                model.BankUserName = bankUserName;
            }
            await _splitSendService.Update(model);
            return Ok();
        }




        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _splitSendService.GetView(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "记录获取失败");
            }
            return Ok(model);
        }



        /// <summary>
        /// 根据暂缓记录生成多批次发放主记录
        /// </summary>
        /// <param name="delaySendRecordID"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> GenerateByDelayRecord([FromForm] Guid delaySendRecordID)
        {

            var dalayRecord = await _delaySendRecordService.Get(f => f.ID == delaySendRecordID);
            if (dalayRecord == null)
            {
                throw new ErrorCodeException(-1, "暂缓记录获取失败");
            }
            var salaryRecord = await _salaryRecordService.Get(f => f.ID == dalayRecord.SalaryRecordID);
            if (salaryRecord == null)
            {
                throw new ErrorCodeException(-1, "工资记录获取失败");
            }
            if (salaryRecord.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放无法执行此操作");
            }
            if (!salaryRecord.IsDelayPayment)
            {
                throw new ErrorCodeException(-1, "非暂缓工资无法执行此操作");
            }
            if (!salaryRecord.IsDelayPaymentSuc)
            {
                throw new ErrorCodeException(-1, "暂缓记录未处理，无法执行此操作！");
            }
            if (salaryRecord.Status!= SalaryRecordSendStatusEnumType.待发放)
            {
                throw new ErrorCodeException(-1, "工资记录非待发放状态，无法执行此操作");
            }
            if (salaryRecord.IsSplitSend)
            {
                throw new ErrorCodeException(-1, "工资记录已生成分批发放记录，重复操作！");
            }

            if (salaryRecord.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放无法执行此操作");
            }

            var cusResp = await _otherService.GetCustomerByNameAsync(salaryRecord.CustomerName);

            Live_V_Customer customer = null;
            if (cusResp.Data != null)
            {
                customer = cusResp.Data;
            }
            else
            {
                throw new ErrorCodeException(-1, "客户单位信息获取失败");
            }

            if (string.IsNullOrWhiteSpace(salaryRecord.Bank)|| string.IsNullOrWhiteSpace(salaryRecord.BankUserName)|| string.IsNullOrWhiteSpace(salaryRecord.BankNo))
            {
                throw new ErrorCodeException(-1, "工资记录 银行卡信息缺失");
            }
            
            Live_SalarySplitSend model = new Live_SalarySplitSend()
            {
                Name = salaryRecord.Name,
                IdCardNo = salaryRecord.IdCardNo,
                ID = Guid.NewGuid(),
                CustomerName = customer.CustomerName,
                CustomerGID = customer.CustomerGID.Value,
                Bank = salaryRecord.Bank,
                BankNo = salaryRecord.BankNo,
                BankUserName = salaryRecord.BankUserName,
                CreateTime = DateTime.Now,
                PartnerGID = customer.PartnerGID.Value,
                PartnerName = customer.PartnerName,
                SalaryDelaySendRecordID = delaySendRecordID,
                SalaryRecordID = salaryRecord.ID,
                SendTime = null,
                PendingSplitAmount= salaryRecord.ActualSalarySendCardAmount,
                Status = SalarySplitSendStatusEnumType.待处理,
                TotalAmount = salaryRecord.ActualSalarySendCardAmount,
            };
            try
            {
                DbScoped.SugarScope.BeginTran();
                salaryRecord.IsSplitSend = true;
                salaryRecord.IsSinglePushSuc = false;
                salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
                salaryRecord.IsLocked = true;
                salaryRecord.SendTime = DateTime.Now;
                salaryRecord.TransDescription = "使用拆分发放";
                await _salaryRecordService.Update(salaryRecord);
                await _splitSendService.Add(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存异常,异常信息：{ex.Message}");
            }


            return Ok();
        }


        /// <summary>
        ///  生成拆分带发放记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="splitAmount"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> GenerateSplitSendRecord([FromForm] Guid id, [FromForm] decimal splitAmount,[FromForm] string remark)
        {
            var modelNav = await _splitSendService.GetNav(f => f.ID == id);
            if (modelNav == null)
            {
                throw new ErrorCodeException(-1, "记录获取失败");
            }
            if (modelNav.PendingSplitAmount < splitAmount)
            {
                throw new ErrorCodeException(-1, $"拆分金额不能大于{modelNav.PendingSplitAmount}");
            }
            modelNav.Split(splitAmount,remark);
            await _splitSendService.UpdateNav(modelNav);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> SetComplete([FromForm] Guid id, [FromForm] decimal splitAmount)
        {
            var modelNav = await _splitSendService.GetNav(f => f.ID == id);
            if (modelNav == null)
            {
                throw new ErrorCodeException(-1, "记录获取失败");
            }
            try
            {
                DbScoped.SugarScope.BeginTran();
                modelNav.SetComplete();
                var salaryRecord = await _salaryRecordService.Get(f => f.ID == modelNav.SalaryRecordID);
                if (salaryRecord == null)
                {
                    throw new ErrorCodeException(-1, "关联的工资记录已被删除，请联系管理员");
                }
                salaryRecord.IsSplitSendSuc = true;
                salaryRecord.SendTime = DateTime.Now;
                salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;

                salaryRecord.TransMsg = "多批次发放,发放完成,手动点击完成";
                await _splitSendService.UpdateNav(modelNav);
                await _salaryRecordService.Update(salaryRecord);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message);
            }
            return Ok();
        }




        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var modelNav = await _splitSendService.GetNav(f => f.ID == id);
            await _splitSendService.DeleteNav(modelNav);
            return Ok();
        }

        #region 记录操作
        [HttpPut]
        public async Task<IActionResult> UpdateRecordStatus([FromForm] Guid id, [FromForm] Guid recordID, [FromForm] SalarySplitSendRecordStatusEnumType status)
        {

            var modelNav = await _splitSendService.GetNav(f => f.ID == id);

            if (modelNav.Status== SalarySplitSendStatusEnumType.已处理)
            {
                throw new ErrorCodeException(-1, $"已处理纪律禁止操作");
            }
            modelNav.SetRecordStatus(recordID, status);
            await _splitSendService.UpdateNav(modelNav);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> DeleteRecord([FromForm] Guid id, [FromForm] Guid recordID)
        {
            var modelNav = await _splitSendService.GetNav(f => f.ID == id);
            modelNav.DeleteRecord(recordID);
            await _splitSendService.UpdateNav(modelNav);
            return Ok();
        }
        #endregion


    }
}
