using LZ.Clients.API;
using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BudgetConfigController(IBudgetConfigService budgetConfigService): FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query)
        {
            var exp = Expressionable.Create<Model.Live_V_BudgetConfig>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                exp.And(t => t.Title.Contains(query.Name));
            }
            var models = await budgetConfigService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_BudgetConfig model)
        {
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }
            var expable = Expressionable.Create<Model.Live_BudgetConfig>();
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.CostType == model.CostType);
            expable.And(t => t.Title == model.Title);
            if (!model.IsRelate)
            {
                
                expable.And(t => t.IsRelate == false);
                if (await budgetConfigService.Exist(expable.ToExpression()))
                {
                    throw new ErrorCodeException(-1, "该名称已存在!");
                }
            } 
            else
            {
                string msg = "";
                bool isok = true;
                expable.And(t => t.IsRelate == true);
                var ids = model.PartnerIDS.Split(',');
                var names = model.PartnerNames.Split(',');
                Dictionary<string, string> dic = new Dictionary<string, string>();
                for(int i = 0;i<ids.Length;i++)
                {
                    dic.Add(ids[i], names[i]);
                }
                FinancePageModel query = new FinancePageModel();
                query.Page = 0;
                var list = await budgetConfigService.ListModel(expable.ToExpression(), query);
                foreach(var x in list)
                {
                    foreach(var d in dic)
                    {
                        if(x.PartnerIDS.Contains(d.Key))
                        {
                            isok = false;
                            msg += d.Value + ",";
                        }
                    }
                }
                if(!isok)
                {
                    msg += "已添加";
                    throw new ErrorCodeException(-1, msg);
                }
                
            }

            model.GID = Guid.NewGuid();
            model.CompanyID = CurrentUser.CompanyID;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            await budgetConfigService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_BudgetConfig> Get(Guid id)
        {
            var model = await budgetConfigService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BudgetConfig model)
        {
            await budgetConfigService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await budgetConfigService.Delete(id);
            return Ok();
        }
    }
}
