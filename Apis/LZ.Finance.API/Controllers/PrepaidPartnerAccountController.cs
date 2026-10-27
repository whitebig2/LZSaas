using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PrepaidPartnerAccountController : ApiControllerBase
    {
        private readonly IHttpClientFactory  _httpClientFactory;
        private readonly IPrepaidPartnerAccountService _prepaidPartnerAccountService;
        private readonly IFinanceOtherService  _financeOtherService;
        public PrepaidPartnerAccountController(IPrepaidPartnerAccountService prepaidPartnerAccountService, IFinanceOtherService financeOtherService, IHttpClientFactory httpClientFactory)
        {
            _prepaidPartnerAccountService = prepaidPartnerAccountService;
            _financeOtherService = financeOtherService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string partnerName, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_PrepaidPartnerAccount>();

            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            var models = await _prepaidPartnerAccountService.PageViewList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var exp = Expressionable.Create<Model.Live_PrepaidPartnerAccount>();
            var models = await _prepaidPartnerAccountService.List(exp.ToExpression(), new PagedModelBase() { Page = 0 });
            return Ok(models);
        }

        public record PrepaidPartnerAccountAddRequest(Guid PartnerGID, PrepaidPaymentEnumType PaymentType,bool IsEnabled,decimal BasePercent,decimal FloatPercent);
        [HttpPost]
        public async Task<IActionResult> Add(PrepaidPartnerAccountAddRequest request)
        {
            Live_PrepaidPartnerAccountExtension model = new Live_PrepaidPartnerAccountExtension()
            {
                ID = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                PartnerGID = request.PartnerGID,
                BadAmount = 0,
                AvailableAmount = 0,
                BasePercent = request.BasePercent,
                FloatPercent = request.FloatPercent,
                CashAmount = 0,
                IsEnabled = request.IsEnabled,
                PaymentType = request.PaymentType,
                TotalAmount = 0,
                UsedAmount = 0,
                CompanyID=CurrentUser.CompanyID,
            };
            await _prepaidPartnerAccountService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _prepaidPartnerAccountService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

       
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_PrepaidPartnerAccountExtension model)
        {
            await _prepaidPartnerAccountService.Update(model);
            return Ok();
        }
        public record CashRechargeRequest(Guid ID,decimal Amount, bool IsRecharge);
        [HttpPut]
        public async Task<IActionResult> CashRechargeOrDeduct(CashRechargeRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new ErrorCodeException(-1, "操作金额必须大于0");
            }
            var model = await _prepaidPartnerAccountService.Get(request.ID);
            if (request.IsRecharge)
            {
                model.CashRecharge(request.Amount);
            }
            else
            {
                model.CashDeduct(request.Amount);
            }
            await _prepaidPartnerAccountService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]Guid id)
        {
            await _prepaidPartnerAccountService.Delete(id);
            return Ok();
        }

        #region 申请预发工资
        /// <summary>
        /// 根据合伙人标识获取预发账户信息
        /// </summary>
        /// <param name="partnerGID"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>

        [HttpGet]
        public async Task<IActionResult> GetByPartnerGID(Guid partnerGID)
        {
            var model = await _prepaidPartnerAccountService.Get(f => f.PartnerGID == partnerGID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }


        //public record PrepaidApplyRequest(int UserEntryID,decimal ApplyAmount, PrepaidApplyEnumType ApplyType);
        ///// <summary>
        ///// 预发申请
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> PrepaidApply(PrepaidApplyRequest request)
        //{
        //    var userEntryResp =await _financeOtherService.GetUserEntryByID(request.UserEntryID);
        //    Live_V_UserEntry userEntry=null ;
        //    if (userEntryResp != null && userEntryResp.Code == 200 && userEntryResp.Data != null)
        //    {
        //        userEntry = userEntryResp.Data;
        //    }
        //    else {

        //        throw new ErrorCodeException(-1, $"员工入职记录信息不存在");
        //    }
        //    // 获取客户单位
        //    Live_Customer customer = null;
        //    var customerResp = await _financeOtherService.GetCustomerByID(userEntry.CustomerID.Value);
        //    if (customerResp != null && customerResp.Code == 200 && customerResp.Data != null)
        //    {
        //        customer = customerResp.Data;
        //    }
        //    else
        //    {

        //        throw new ErrorCodeException(-1, $"员工{userEntry.Name}客户单位信息缺失");
        //    }
        //    // 获取签章单位
        //    Live_Enterprise signingUnit = null;
        //    var signingUnitResp = await _financeOtherService.GetEnterpriseByIdAsync(customer.SigningUnitGID.Value);
        //    if (signingUnitResp != null && signingUnitResp.Code == 200 && signingUnitResp.Data != null)
        //    {
        //        signingUnit = signingUnitResp.Data;
        //    }
        //    else
        //    {

        //        throw new ErrorCodeException(-1, $"员工{userEntry.Name}签章单位信息缺失");
        //    }

        //    Live_Partner partner = null;
        //    if (userEntry.JointPartnerID != null && userEntry.JointPartnerID.Value >= 0)
        //    {
        //        var partnerResp = await _financeOtherService.GetPartnerByID(userEntry.JointPartnerID.Value);
        //        if (partnerResp != null && partnerResp.Code == 200 && partnerResp.Data != null)
        //        {
        //            partner = partnerResp.Data;
        //        }
        //    }
        //    else if (userEntry.PartnerID != null && userEntry.PartnerID.Value >= 0)
        //    {
        //        var partnerResp = await _financeOtherService.GetPartnerByID(userEntry.PartnerID.Value);
        //        if (partnerResp != null && partnerResp.Code == 200 && partnerResp.Data != null)
        //        {
        //            partner = partnerResp.Data;
        //        }
        //    }
        //    else {
        //        throw new ErrorCodeException(-1, $"员工{userEntry.Name} 合伙人信息缺失");
        //    }
        //    var prepaidAccount = await _prepaidPartnerAccountService.Get(f => f.IsEnabled && f.PartnerGID == partner.PartnerGID.Value);
        //    if (prepaidAccount == null)
        //    {
        //        throw new ErrorCodeException(-1, $"合伙人{partner.PartnerName} 预发账户信息获取失败");
        //    }

        //    Live_PrepaidApplyRecord applyRecord = new Live_PrepaidApplyRecord()
        //    {
        //        ID=Guid.NewGuid(),
        //        AppalyAmount = request.ApplyAmount,
        //        Bank = userEntry.BankName,
        //        BankNo = userEntry.BankID,
        //        CreateTime = DateTime.Now,
        //        CustomerGID = customer.CustomerGID.Value,
        //        IdCardNo = userEntry.CardID,
        //        PartnerID = partner.PartnerID,
        //        Name = userEntry.Name,
        //        PrepaidPartnerAccountID = prepaidAccount.ID,
        //        SigningUnitID = signingUnit.ID,
        //        UserEntryID = userEntry.ID,
        //        ApplyType = request.ApplyType,
        //        Status = 0,
        //    };

        //    if (applyRecord.BankNo.IsNullOrWhiteSpace())
        //    {
        //        throw new ErrorCodeException(-1, $"{userEntry.Name} 银行卡信息缺失");
        //    }
        //    if (applyRecord.Bank.IsNullOrWhiteSpace())
        //    {
        //        throw new ErrorCodeException(-1, $"{userEntry.Name} 银行卡信息缺失");
        //    }
            

        //    await _prepaidPartnerAccountService.AddApplyRecord(applyRecord);
        //    return Ok();
        //}

        //public record PrepaidAuditRequest(Guid RecordID,bool IsAudit,string Remark);
        ///// <summary>
        ///// 预发审核
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> PrepaidAudit(PrepaidAuditRequest request)
        //{
        //    var model = await _prepaidPartnerAccountService.GetApplyRecord(request.RecordID);
        //    if (model == null)
        //    {
        //        throw new ErrorCodeException(-1, $"申请记录信息不存在");
        //    }
        //    if (model.Status != Model.Enums.PrepaidRecordStatusEnumType.待审核)
        //    {
        //        throw new ErrorCodeException(-1, $"非待审核状态无法执行此操作");
        //    }
        //    if (request.IsAudit)
        //    {
        //        await _prepaidPartnerAccountService.ApplyRecordAudit(request.RecordID);
        //    }
        //    else
        //    {
        //        model.Status = Model.Enums.PrepaidRecordStatusEnumType.审核不通过;
        //        model.Remark=request.Remark;
        //        await _prepaidPartnerAccountService.UpdateApplyRecord(model);
        //    }
        //    return Ok();
        //}
        #endregion

        
    }
}
