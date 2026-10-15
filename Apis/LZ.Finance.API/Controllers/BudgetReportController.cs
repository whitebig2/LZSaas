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
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BudgetReportController(IBudgetReportDetailService budgetReportDetailService, IBudgetReportConfigService budgetReportConfigService, IBudgetReportService budgetReportService, IHttpService httpService, IProfitManageService profitManageService) : FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query)
        {
            var exp = Expressionable.Create<Model.Live_BudgetReport>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }

            var models = await budgetReportService.List(exp.ToExpression(), query, "Year desc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_BudgetReport model)
        {
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }
            var expable = Expressionable.Create<Model.Live_BudgetReport>();
            expable.And(t => t.CompanyID == cid);
            if (await budgetReportService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该名称已存在!");
            }
            model.CompanyID = CurrentUser.CompanyID;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            await budgetReportService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await budgetReportService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            
            return Ok(new {model});
        }
       
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BudgetReport model)
        {
            await budgetReportService.Update(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateExcel([FromBody]Model.Live_BudgetReport model)
        {
            var date = DateTime.Now.AddMonths(-1);
            string filename = date.ToString("yyyyMM") + "_" + model.PartnerName + "_经营报表";
            FinancePageModel query = new FinancePageModel();
            query.Page = 0;
            var exp_BudgetReportConfig = Expressionable.Create<Model.Live_BudgetReportConfig>();
            exp_BudgetReportConfig.And(t => t.CompanyID == CurrentUser.CompanyID);
            var models_BudgetReportConfig = await budgetReportConfigService.List(exp_BudgetReportConfig.ToExpression(), query);

            
            var exp_BudgetReportDetail = Expressionable.Create<Model.Live_BudgetReportDetail>();
            exp_BudgetReportDetail.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_BudgetReportDetail.And(t => t.Year == model.Year);
            exp_BudgetReportDetail.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            var models_BudgetReportDetail = await budgetReportDetailService.List(exp_BudgetReportDetail.ToExpression(), query);
            int? month;
            if(models_BudgetReportDetail.Where(t=>t.Month==1).ToList().Count>0)
            {
                month = models_BudgetReportDetail.Where(t=>t.Month<4).Max(t => t.Month);
            }
            else
            {
                month = models_BudgetReportDetail.Max(t => t.Month);
            }

            if (month.HasValue)
            {
                int? y = month > 3 ? model.Year : model.Year + 1;
                string MM = month < 10 ? "0" + month : month.Value.ToString();
                filename = y + MM + "_" + model.PartnerName + "_经营报表";
            }
           

            var exp_ProfitManage = Expressionable.Create<Model.Live_ProfitManage>();
            exp_ProfitManage.And(it => SqlFunc.Subqueryable<Model.Live_Partner>().Where(s => s.PartnerGID == it.PartnerGID && s.IsBudget ).Any());
            exp_ProfitManage.And(it => (it.ProfitMonth.Value.Year == model.Year && it.ProfitMonth.Value.Month > 3) || (it.ProfitMonth.Value.Year == model.Year + 1 && it.ProfitMonth.Value.Month < 4));
            var models_ProfitManage = await profitManageService.List(exp_ProfitManage.ToExpression());
            
           

            var result = await budgetReportService.ReportExcel(models_BudgetReportConfig, models_BudgetReportDetail, models_ProfitManage);
            string base64String = Convert.ToBase64String(result);
            
            return Ok(new { base64String, filename, mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpPut]
        public async Task<IActionResult> CreatePartner()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("IsBudget", "true");
            dic.Add("page", "0");
            dic.Add("CompanyID", CurrentUser.CompanyID.ToString());
            var insuracne = await httpService.GetAsync(ServiceNames.BasicService, "/api/Partner/list", dic);
            List<Model.Live_Partner> Partners = new List<Live_Partner>();
            if (!string.IsNullOrEmpty(insuracne.Data.ToString()))
            {
                var ii = JsonConvert.DeserializeObject(insuracne.Data.ToString()) as JObject;
                Partners = JsonConvert.DeserializeObject<List<Model.Live_Partner>>(ii["items"].ToString());
            }
            else
            {
                throw new ErrorCodeException(-1, "读取数据错误");
            }
            List<Model.Live_BudgetReport> list = new List<Live_BudgetReport>();
            var year = DateTime.Now.AddMonths(-3).Year;
            FinancePageModel query = new FinancePageModel();
            query.Page = 0;
            var exp = Expressionable.Create<Model.Live_BudgetReport>();
            exp.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp.And(t => t.Year == year);
            var models = await budgetReportService.List(exp.ToExpression(), query);
            
            foreach (var x in Partners)
            {
                var info = models.Find(t => t.PartnerID == x.PartnerID);
                if(info!=null)
                {
                    continue;
                }
                Model.Live_BudgetReport model = new Live_BudgetReport();
                model.Year = year;
                model.PartnerID = x.PartnerID;
                model.PartnerGID = x.PartnerGID;
                model.PartnerName = x.PartnerName;
                model.CompanyID = x.CompanyID;
                model.CompanyGID = x.CompanyGID;
                model.CreateTime = DateTime.Now;
                list.Add(model);
            }
            if(list.Count>0)
            {
                await budgetReportService.Add(list);
            }
            return Ok(new {items = Partners });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await budgetReportService.Delete(id);
            return Ok();
        }
    }
}
