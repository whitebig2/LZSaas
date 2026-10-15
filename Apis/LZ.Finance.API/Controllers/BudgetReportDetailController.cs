using AutoMapper;
using LZ.Clients.API;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Budget;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BudgetReportDetailController(IBudgetReportDetailService budgetReportDetailService,IBudgetApplyRecordService budgetApplyRecordService,IBudgetReportConfigService budgetReportConfigService,IBudgetReportService budgetReportService, IProfitManageService profitManageService,ISalaryRecordService salarydRecordService,IBudgetApplyRecordDetailService budgetApplyRecordDetailService, IProfitManageRecordService profitManageRecordService) : FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query)
        {
            var exp = Expressionable.Create<Model.Live_BudgetReportDetail>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }

            var models = await budgetReportDetailService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_BudgetReportDetail model)
        {
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }
            var expable = Expressionable.Create<Model.Live_BudgetReportDetail>();
            expable.And(t => t.CompanyID == cid);
            if (await budgetReportDetailService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该名称已存在!");
            }
            model.CompanyID = CurrentUser.CompanyID;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            await budgetReportDetailService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await budgetReportDetailService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            return Ok(new { model });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BudgetReportDetail model)
        {
            await budgetReportDetailService.Update(model);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] Model.Budget.RecordMonthModel model)
        {
            FinancePageModel query = new FinancePageModel();
            query.Page = 0;
            int year = model.Year;
            var exp_BudgetApplyRecord = Expressionable.Create<Model.Live_BudgetApplyRecord>();
            if(model.Month==1|| model.Month == 2 || model.Month == 3)
            {
                year = model.Year + 1;
            }
            int quarterType = 0;
            if (model.Month == 4 || model.Month == 5 || model.Month == 6)
            {
                quarterType = (int)BudgetQuarterTypeEnum.第一季度;

            }
            else if (model.Month == 7 || model.Month == 8 || model.Month == 9)
            {
                quarterType = (int)BudgetQuarterTypeEnum.第二季度;

            }
            else if (model.Month == 10 || model.Month == 11 || model.Month == 12)
            {
                quarterType = (int)BudgetQuarterTypeEnum.第三季度;

            }
            else
            {
                quarterType = (int)BudgetQuarterTypeEnum.第四季度;

            }
            exp_BudgetApplyRecord.And(t => t.Year == model.Year);
            exp_BudgetApplyRecord.And(t => t.ProcessStatus==4 && t.PayTime != null && t.PayTime.Value.Year == year && t.PayTime.Value.Month == model.Month);
            exp_BudgetApplyRecord.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_BudgetApplyRecord.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            var models_BudgetApplyRecord = await budgetApplyRecordService.List(exp_BudgetApplyRecord.ToExpression(), query);
            //models_BudgetApplyRecord：当月所有已打款的申请

            var exp_BudgetApplyRecordDetail = Expressionable.Create<Model.Live_V_BudgetApplyRecordDetail>();
            exp_BudgetApplyRecordDetail.And(t => t.ProcessStatus == 4 && t.PayTime != null && t.PayTime.Value.Year == year && t.PayTime.Value.Month == model.Month);
            exp_BudgetApplyRecordDetail.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_BudgetApplyRecordDetail.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            var models_BudgetApplyRecordDetail = await budgetApplyRecordDetailService.ListV(exp_BudgetApplyRecordDetail.ToExpression(), query);






            var exp_BudgetReportConfig = Expressionable.Create<Model.Live_BudgetReportConfig>();
            exp_BudgetReportConfig.And(t => t.CompanyID == CurrentUser.CompanyID);
            var models_BudgetReportConfig = await budgetReportConfigService.List(exp_BudgetReportConfig.ToExpression(), query);

            var exp_BudgetReport = Expressionable.Create<Model.Live_BudgetReport>();
            exp_BudgetReport.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_BudgetReport.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            exp_BudgetReport.And(t => t.Year == model.Year);
            var models_BudgetReport = await budgetReportService.List(exp_BudgetReport.ToExpression(), query);

            var exp_BudgetReportDetail = Expressionable.Create<Model.Live_BudgetReportDetail>();
            exp_BudgetReportDetail.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_BudgetReportDetail.And(t => t.Year == model.Year);
            exp_BudgetReportDetail.And(t => t.QuarterType==quarterType);
            exp_BudgetReportDetail.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            var models_BudgetReportDetail = await budgetReportDetailService.List(exp_BudgetReportDetail.ToExpression(), query);

            var exp_ProfitManageRecord = Expressionable.Create<Model.Live_ProfitManageRecord>();
            exp_ProfitManageRecord.And(it => SqlFunc.Subqueryable<Model.Live_Partner>().Where(s => s.PartnerGID == it.PartnerGID && s.IsBudget).Any());
            exp_ProfitManageRecord.And(it => (it.ProfitMonth.HasValue && it.ProfitMonth.Value.Year == year && it.ProfitMonth.Value.Month == model.Month));
            var models_ProfitManageRecord = await profitManageRecordService.List(exp_ProfitManageRecord.ToExpression());

            var exp_ProfitManage = Expressionable.Create<Model.Live_ProfitManage>();
            exp_ProfitManage.And(it => SqlFunc.Subqueryable<Model.Live_Partner>().Where(s => s.PartnerGID == it.PartnerGID && s.IsBudget).Any());
            exp_ProfitManage.And(it => (it.ProfitMonth.HasValue && it.ProfitMonth.Value.Year == year && it.ProfitMonth.Value.Month == model.Month));
            exp_ProfitManage.AndIF(model.PartnerGID.HasValue, t => t.PartnerGID == model.PartnerGID);
            var models_ProfitManage = await profitManageService.List(exp_ProfitManage.ToExpression());




            var exp_SalaryRecord = Expressionable.Create<Model.Live_V_SalaryRecord>();
            exp_SalaryRecord.And(it => SqlFunc.Subqueryable<Model.Live_BudgetReport>().Where(s => s.PartnerName == it.PlatformDepartment && s.Year == model.Year).Any());
            exp_SalaryRecord.And(it => (it.SalaryMonth.HasValue && it.SalaryMonth.Value.AddMonths(1).Year == year && it.SalaryMonth.Value.AddMonths(1).Month == model.Month));
            //exp_SalaryRecord.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            exp_SalaryRecord.And(t => t.CustomerName.Contains("内部员工"));
            var models_SalaryRecord = await salarydRecordService.ListView(exp_SalaryRecord.ToExpression());


            List<Model.Live_BudgetReportDetail> detailsAdd = new List<Live_BudgetReportDetail>();
            List<Model.Live_BudgetReportDetail> detailsUpdate = new List<Live_BudgetReportDetail>();
            foreach (var partner in models_BudgetReport)
            {
                foreach(var item in models_BudgetReportConfig)
                {
                    var list = models_BudgetReportDetail.Where(t => t.BudgetReportID == partner.ID && t.BudgetReportConfigID == item.ID && t.Month== model.Month).ToList();
                    if (list.Count > 0) continue;
                    Model.Live_BudgetReportDetail detail = new Live_BudgetReportDetail();
                    detail.CreateTime = DateTime.Now;
                    detail.CompanyID = partner.CompanyID;
                    detail.CompanyGID = partner.CompanyGID;
                    detail.BudgetReportID = partner.ID;
                    detail.BudgetReportConfigID = item.ID;
                    detail.PartnerID = partner.PartnerID;
                    detail.PartnerGID = partner.PartnerGID;
                    detail.Title = item.CostName;
                    detail.Money = 0;
                    if (item.ItemType == 0)
                    {
                        var profitrecords = models_ProfitManageRecord.Where(t => t.RealPartnerGID==partner.PartnerGID).ToList();
                        var profits = models_ProfitManage.Where(t => t.PartnerGID == partner.PartnerGID).ToList();
                        switch (item.CostType)
                        {
                            case (int)BudgetGrossProfitEnum.主营业务收入:
                                detail.Money = profitrecords.Sum(t=>t.ServiceFeeTotalAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.其他业务收入_友翌返税共享:
                                detail.Money = 0;
                                break;
                            case (int)BudgetGrossProfitEnum.其他收入_房租收入:
                                detail.Money = 0;
                                break;
                            case (int)BudgetGrossProfitEnum.人数:
                                detail.Money = profitrecords.Count;
                                break;
                            case (int)BudgetGrossProfitEnum.新业务开发费:
                                detail.Money = profitrecords.Sum(t=>t.BusinessAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.派遣员工工伤保险_商业:
                                detail.Money = profitrecords.Sum(t=>t.BusInsuranceSumAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.派遣员工风险金:
                                detail.Money = profitrecords.Sum(t=>t.RiskCurrentAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.残保金:
                                detail.Money = profitrecords.Sum(t=>t.DisabilityAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.驻厂管理费:
                                detail.Money = profitrecords.Sum(t=>t.JoinManageAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.技术金融服务费:
                                detail.Money = profits.Sum(t=>t.InterestTotalAmount??0M);
                                break;
                            case (int)BudgetGrossProfitEnum.税费:
                                detail.Money = profitrecords.Sum(t=>t.AgentPaidTaxAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.社保缴纳风险金:
                                detail.Money = profitrecords.Sum(t => t.SocialRepayRiskAmount);
                                break;
                            case (int)BudgetGrossProfitEnum.返顺丰税款:
                                detail.Money = 0;
                                break;
                        }

                    }
                    else if (item.ItemType == 10)
                    {
                        var salarys = models_SalaryRecord.Where(t => t.PlatformDepartment == partner.PartnerName);
                        foreach(var x in salarys)
                        {
                            switch (item.CostType)
                            {
                                case (int)BudgetMainEmployeesSettlementEnum.内部员工应发工资:
                                    detail.Money += (x.UnitSalaryAmount - x.PlatformCommissionAmount - x.PlatformWelfareAmount);
                                    break;
                                case (int)BudgetMainEmployeesSettlementEnum.提成:
                                    detail.Money += x.PlatformCommissionAmount;
                                    break;
                                case (int)BudgetMainEmployeesSettlementEnum.员工福利:
                                    detail.Money += x.PlatformWelfareAmount;
                                    break;
                                case (int)BudgetMainEmployeesSettlementEnum.社保公积金:
                                    detail.Money += x.SocialGongJiJinAmount + x.SocialUnitSumAmount;
                                    break;
                            }
                        }
                        
                    }
                    else
                    {
                        var applyRecords = models_BudgetApplyRecordDetail.Where(t => t.PartnerID == partner.PartnerID && t.ExpensesTitle == item.CostName && t.CostType==item.ItemType).ToList();
                        if (applyRecords.Count == 0) continue;
                        detail.Money = applyRecords.Sum(t => t.Money);
                    }
                    detail.Year = model.Year;
                    detail.Month = model.Month;
                    detail.MonthType = (detail.Month + 2) % 3;
                    detail.MonthName = ((BudgetMonthTypeEnum)detail.MonthType).ToString();
                    detail.QuarterType = quarterType;
                    detail.QuarterName = ((BudgetQuarterTypeEnum)detail.QuarterType).ToString();

                    //是否已有月度小计，如果存在则累加，没有则创建
                    var xiaoji = models_BudgetReportDetail.Find(t => t.PartnerID == partner.PartnerID && t.BudgetReportConfigID == item.ID && t.MonthType == 3);
                    if(xiaoji!=null)
                    {
                        var arr = models_BudgetReportDetail.Where(t => t.PartnerID == partner.PartnerID && t.BudgetReportConfigID == item.ID && t.MonthType != 3);
                        xiaoji.Money = arr.Sum(t => t.Money);
                        xiaoji.Money += detail.Money;
                        detailsUpdate.Add(xiaoji);
                    }
                    else
                    {
                        Model.Live_BudgetReportDetail detailNew = new Live_BudgetReportDetail();
                        detailNew.CreateTime = DateTime.Now;
                        detailNew.CompanyID = detail.CompanyID;
                        detailNew.CompanyGID = detail.CompanyGID;
                        detailNew.BudgetReportID = detail.BudgetReportID;
                        detailNew.BudgetReportConfigID = detail.BudgetReportConfigID;
                        detailNew.PartnerID = detail.PartnerID;
                        detailNew.PartnerGID = detail.PartnerGID;
                        detailNew.Title = "小计";
                        detailNew.Money = detail.Money;
                        detailNew.Year = detail.Year;
                        detailNew.Month = 0;
                        detailNew.MonthType = 3;
                        detailNew.QuarterType = detail.QuarterType;
                        detailNew.QuarterName = detail.QuarterName;
                        detailNew.MonthName = ((BudgetMonthTypeEnum)detailNew.MonthType).ToString();
                        detailsAdd.Add(detailNew);
                    }
                    detailsAdd.Add(detail);


                }
            }
            if(detailsUpdate.Count>0)
            {
                await budgetReportDetailService.Update(detailsUpdate);
            }
            if(detailsAdd.Count>0)
            {
                await budgetReportDetailService.Add(detailsAdd);
            }
            return Ok(new { detailsUpdate, detailsAdd });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMonthBill([FromBody] Model.Budget.RecordMonthModel model)
        {
            int year = model.Year;
            if (model.Month == 1 || model.Month == 2 || model.Month == 3)
            {
                year = model.Year + 1;
            }
            var exp_BudgetReportDetail = Expressionable.Create<Model.Live_BudgetReportDetail>();
            exp_BudgetReportDetail.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_BudgetReportDetail.And(t => t.Year == model.Year);
            exp_BudgetReportDetail.And(t => t.Month == model.Month);
            exp_BudgetReportDetail.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            var models_BudgetReportDetail = await budgetReportDetailService.Delete(exp_BudgetReportDetail.ToExpression());
            //更新小计的统计
            int quarterType = 0;
            if (model.Month == 4 || model.Month == 5 || model.Month == 6)
            {
                quarterType = (int)BudgetQuarterTypeEnum.第一季度;


            }
            else if (model.Month == 7 || model.Month == 8 || model.Month == 9)
            {
                quarterType = (int)BudgetQuarterTypeEnum.第二季度;

            }
            else if (model.Month == 10 || model.Month == 11 || model.Month == 12)
            {
                quarterType = (int)BudgetQuarterTypeEnum.第三季度;

            }
            else
            {
                quarterType = (int)BudgetQuarterTypeEnum.第四季度;


            }
            FinancePageModel query = new FinancePageModel();
            query.Page = 0;
            var exp = Expressionable.Create<Model.Live_BudgetReportDetail>();
            exp.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp.And(t => t.Year == model.Year);
            exp.And(t => t.QuarterType == quarterType);
            exp.AndIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID);
            var details = await budgetReportDetailService.List(exp.ToExpression(), query);
            var xiaojiList = details.Where(t => t.MonthType == (int)BudgetMonthTypeEnum.季度小计);
            List<Model.Live_BudgetReportDetail> detailsUpdate = new List<Live_BudgetReportDetail>();
            foreach (var x in xiaojiList)
            {
                var arr = details.Where(t => t.PartnerID == x.PartnerID && t.BudgetReportConfigID == x.BudgetReportConfigID && t.MonthType != 3);
                x.Money = arr.Sum(t => t.Money);
                detailsUpdate.Add(x);
            }
            await budgetReportDetailService.Update(detailsUpdate);
            return Ok();
        }

            [HttpPost]
        public async Task<IActionResult> ReportMonthList([FromBody] RecordMonthModel model)
        {
            FinancePageModel query = new FinancePageModel();
            query.Page = 0;
            var exp = Expressionable.Create<Model.Live_BudgetReportDetail>();
            exp.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp.And(t => t.Year == model.Year);
            exp.And(t => t.PartnerID == model.PartnerID);
            var models = await budgetReportDetailService.List(exp.ToExpression(), query);
            var list = budgetReportDetailService.ReportMonthList(models, model);
            return Ok(new { items = list});
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await budgetReportDetailService.Delete(id);
            return Ok();
        }
    }
}
