using LZ.Clients.API;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums.Budget;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BudgetReportConfigController(IBudgetReportConfigService budgetReportConfigService) : FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query)
        {
            var exp = Expressionable.Create<Model.Live_BudgetReportConfig>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            exp.AndIF(query.CID.HasValue, t => t.CID == query.CID);
            var models = await budgetReportConfigService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]List<Model.Live_BudgetReportConfig> list)
        {
            if(list.Count==0) throw new ErrorCodeException(-1, "没有数据");
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }
            var expable = Expressionable.Create<Model.Live_BudgetReportConfig>();
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.CID == list[0].CID);
            await budgetReportConfigService.Delete(expable.ToExpression());
            List<Model.Live_BudgetReportConfig> models = new List<Live_BudgetReportConfig>();
            foreach(var x in list)
            {
                if (string.IsNullOrEmpty(x.CostName))
                {
                    continue;
                }
                x.CompanyID = cid;
                x.CompanyGID = cgid;
                models.Add(x);
            }
            if (models.Count == 0) throw new ErrorCodeException(-1, "没有数据");
            await budgetReportConfigService.Add(models);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await budgetReportConfigService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            return Ok(new { model });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BudgetReportConfig model)
        {
            await budgetReportConfigService.Update(model);
            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await budgetReportConfigService.Delete(id);
            return Ok();
        }
    }
}
