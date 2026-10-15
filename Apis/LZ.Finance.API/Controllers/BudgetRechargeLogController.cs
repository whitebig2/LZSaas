using LZ.Clients.API;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;

using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using LZ.Core.Interface;
using System;
using NetTaste;
using LZ.Model.Enums.Budget;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BudgetRechargeLogController(IBudgetRechargeLogService budgetRechargeLogService, IBudgetManageService budgetManageService) : FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query)
        {
            var exp = Expressionable.Create<Model.Live_BudgetRechargeLog>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            if (query.CID.HasValue)
            {
                exp.And(t => t.CID == query.CID);
            }
            var models = await budgetRechargeLogService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_BudgetRechargeLog model)
        {

            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }
            var expable = Expressionable.Create<Model.Live_BudgetRechargeLog>();
            model.CompanyID = CurrentUser.CompanyID;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            
            var manage = await budgetManageService.Get(model.CID.Value);
            if(manage==null)
            {
                throw new ErrorCodeException(-1, "没有匹配的预算数据");
            }
            model.Year = manage.Year;
            if (model.Type==(int)BudgetManageRecordActionEnum.服务费充值 || model.Type == (int)BudgetManageRecordActionEnum.管理员充值)
            {
                manage.TotalAmount += model.Money;
                manage.AvailableAmount += model.Money;
                model.AddAmount = model.Money;
            }
            else
            {
                manage.TotalAmount -= model.Money;
                manage.AvailableAmount -= model.Money;
                model.DecreaseAmount = model.Money;
            }
            model.TotalAmount = manage.TotalAmount;
            model.AvailableAmount = manage.AvailableAmount;
            model.LockAmount = manage.LockAmount;
            model.UsedAmount = manage.UsedAmount;
            model.HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount;
            await budgetManageService.Update(manage);
            await budgetRechargeLogService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_BudgetRechargeLog> Get(int id)
        {
            var model = await budgetRechargeLogService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BudgetRechargeLog model)
        {
            await budgetRechargeLogService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await budgetRechargeLogService.Delete(id);
            return Ok();
        }
    }
}
