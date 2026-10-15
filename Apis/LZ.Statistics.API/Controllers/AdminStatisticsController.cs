using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Enums.Budget;
using LZ.Model.Exceptions;
using LZ.Model.Response;
using LZ.Statistics.Service.Interface;
using LZ.Statistics.Service.Services;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Security.Claims;

namespace LZ.Statistics.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class AdminStatisticsController(
        IPrepaidApplyRecordService prepaidApplyRecordService,
        IEntryStatisticsService entryStatisticsService,
        IAuthService authService, IUserStatisticsService userStatisticsService, 
        ICustomerStatisticsService customerStatisticsService, 
        IPartnerStatisticsService partnerStatisticsService,
        IBasicDataService basicDataService,
        ICompanyConfigService companyConfigService, 
        ISalaryStatisticsService salaryStatisticsService, 
        IPrepaidApplyStatisticsService prepaidApplyStatisticsService,
        IContractStatisticsService contractStatisticsService,
        IInjuryStatisticService injuryStatisticService,
        IBudgetService budgetService,
        IInsuranceService insuranceService,
        ICmbcBankCardStatisticsService cmbcBankCardStatisticsService,
        IBusinessInsuranceRecordService businessInsuranceRecordService,
        IInvoiceService invoiceService,
        IGrossManageService  grossManageService,
        IPersonnelStatisticsService personnelStatisticsService


        ) : StatisticsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> HomeStatistics()
        {
            var user = CurrentUser;
            var isParty = user.CompanyType == CompanyType.Party.ToString().ToLower();
            var company = await companyConfigService.Get(user.CompanyGID);
            var companyInfo = company.Company;
            var statistics = new Dictionary<string, object>();
            var counts = new Dictionary<string, object>();
            var partnerID = 0;
            Guid partnerGID = Guid.Empty;
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin && !isParty)
            {
                var userModel = await authService.UserInfo<Model.SYS_CompanyUser>();
                if (userModel.UserType == (int)CompanyUserType.Partner)
                {
                    partnerID = userModel.PartnerID.Value;
                    if (userModel.PartnerGID.HasValue)
                    {
                        partnerGID = userModel.PartnerGID.Value;
                    }
                }
            }
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
            expable.And(it => statuss.Contains(it.Status.Value));
            expable.AndIF(!isParty,it => UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            if (isParty)
            {
                expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            expable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
            var statusCount = await entryStatisticsService.StatusCount(expable.ToExpression());
            if (statusCount.Count > 0)
            {
                var data = statusCount.Find(it => it.Status == (int)UserStatus.Entry);
                if (data != null)
                {
                    statistics.Add("onTheJobCount", data.Count);
                }
                data = statusCount.Find(it => it.Status == (int)UserStatus.WaitEntry);
                if (data != null)
                {
                    statistics.Add("waitEntry", data.Count);
                }
            }
            var dayEntryExpable = Expressionable.Create<Model.Live_V_UserEntry>();
            dayEntryExpable.And(it => SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && (it.Status == (int)UserStatus.Entry || it.Status == (int)UserStatus.Leave) && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            if (isParty)
            {
                dayEntryExpable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else
            {
                dayEntryExpable.And(it => it.CompanyID == user.CompanyID);
            }
            dayEntryExpable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
            var count = await entryStatisticsService.Count(dayEntryExpable.ToExpression());
            statistics.Add("toDayEntryCount", count);

            var dayLeaveExpable = Expressionable.Create<Model.Live_V_UserEntry>();
            dayLeaveExpable.And(it => SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            if (isParty)
            {
                dayLeaveExpable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else
            {
                dayLeaveExpable.And(it => it.CompanyID == user.CompanyID);
            }
            dayLeaveExpable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
            count = await entryStatisticsService.Count(dayLeaveExpable.ToExpression());
            statistics.Add("toDayLeaveCount", count);

            var workClassExpable = Expressionable.Create<Model.Live_V_UserEntry>();
            workClassExpable.And(it => it.Status == (int)UserStatus.Entry && it.PlanStatus1 == UserEntryPlanStatusEnumType.已增员 && !SqlFunc.IsNullOrEmpty(it.WorkClass) && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value) && it.PartnerType < (int)PartnerType.Other);
            if (isParty)
            {
                workClassExpable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else
            {
                workClassExpable.And(it => it.CompanyID == user.CompanyID);
            }
            workClassExpable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
            var workClassCount = await entryStatisticsService.WorkClassList(workClassExpable.ToExpression());
            var allCount = 0;
            var insurance = new Dictionary<string, object>();
            foreach (var workClass in workClassCount)
            {
                if (!string.IsNullOrEmpty(workClass.Name))
                {
                    allCount += workClass.Count;
                    insurance.Add(workClass.Name, workClass.Count);
                }
               
            }

            //insurance.Add("总参保", allCount);
            //var insModel = workClassCount.Find(it => it.Name == "4类");
            //var four = 0M;
            //if (insModel != null)
            //{
            //    insurance.Add("4类", insModel.Count);
            //}
            //insModel = workClassCount.Find(it => it.Name == "5类");
            //var five = 0M;
            //if (insModel != null)
            //{
            //    insurance.Add("5类", insModel.Count);
            //    five = insModel.Count;
            //}
            //if (five > 0 && allCount > 0)
            //{
            //    insurance.Add("5类占比", Math.Round(five / allCount, 2)*100);
            //}
            //else
            //{
            //    insurance.Add("5类占比", 0);
            //}
            //工伤
            var injuryExpable = Expressionable.Create<Model.Live_V_InjuryInfo>();
            injuryExpable.And(it => it.ProcessStatus == 1);
            if (isParty)
            {
                injuryExpable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if (companyInfo.CutType != CompanyType.Self.ToString().ToLower())
            {
                injuryExpable.And(it => it.CompanyID == user.CompanyID);
            }
            injuryExpable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
            var injuryExamine = await injuryStatisticService.InfoCount(injuryExpable.ToExpression());
            counts.Add("injuryWaitAudit", injuryExamine);
            if (companyInfo.CutType == CompanyType.Self.ToString().ToLower())
            {
                if (partnerID == 0)
                {
                    var loanPending = await injuryStatisticService.ApplyRecordCount(t => t.MoneyType == 1 && t.Status == 2);
                    var paymentPending = await injuryStatisticService.PaymentCount(t => t.Status == 1);
                    counts.Add("injuryLoanWaitPay", loanPending);
                    counts.Add("injuryPayWaitPay", paymentPending);
                }
                //预算
                var budgetExpable = Expressionable.Create<Model.Live_BudgetApplyRecord>();
                budgetExpable.And(it => it.CompanyID == user.CompanyID);
                budgetExpable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
                var budgetStatusCount = await budgetService.StatusCount(budgetExpable.ToExpression());
                if (budgetStatusCount.Count > 0)
                {
                    var data = budgetStatusCount.Find(it => it.Status == (int)BudgetApplyRecordProcessStatus.审核退回);
                    if (data != null)
                    {
                        counts.Add("budgetBack", data.Count);
                    }
                    data = budgetStatusCount.Find(it => it.Status == (int)BudgetApplyRecordProcessStatus.待审核);
                    if (data != null)
                    {
                        counts.Add("budgetWaitAudit", data.Count);
                    }
                    data = budgetStatusCount.Find(it => it.Status == (int)BudgetApplyRecordProcessStatus.待打款);
                    if (data != null)
                    {
                        counts.Add("budgetWaitPay", data.Count);
                    }
                }
            }
           
            //社保
            var insuranceExpable = Expressionable.Create<Model.Live_V_InsuranceEmployee>();
            if (isParty)
            {
                insuranceExpable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.EmployeeCompanyId && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else
            {
                insuranceExpable.And(it => it.CompanyID == user.CompanyID);
            }
            insuranceExpable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
            var insuranceStatusCount = await insuranceService.StatusCount(insuranceExpable.ToExpression());
            if (insuranceStatusCount.Count > 0)
            {
                var data = insuranceStatusCount.Find(it => it.Status == 2);
                if (data != null)
                {
                    counts.Add("insuranceAdd", data.Count);
                }
                data = insuranceStatusCount.Find(it => it.Status == 4);
                if (data != null)
                {
                    counts.Add("insuranceMinus", data.Count);
                }
            }

            #region 现金发放临时白名单
            var cashAuditCount = await salaryStatisticsService.SalaryCashCount(f => f.CashType == "临时白名单" && f.Status == CashSendStatusEnumType.待处理 && f.AuditStatus == 0);
            counts.Add("cashAuditCount", cashAuditCount);
            #endregion

            #region 预发余额不足
            var subAccountBalanceCount = await salaryStatisticsService.PrepaidSubAccountCounts(f => f.IsEnable&&f.Balance<=f.MinBalance);
            counts.Add("subAccountBalanceCount", subAccountBalanceCount);
            #endregion

            #region 工资、预发统计

            var dt = DateTime.Today;
            // 自充账户：A03A422F-851D-476D-9115-336084B9AFB1
            //要去掉自充值服务商的数据，还要去掉签章单位是  宁波新莱博金浩供应链管理有限公司 的数据
            // 日结工资申请，当天发放统计
            var prepaidDaySalaryExp = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
            prepaidDaySalaryExp.AndIF(partnerGID != Guid.Empty, f => f.PartnerGID == partnerGID);
            prepaidDaySalaryExp.And(f => f.ApplyType== (int)PrepaidApplyEnumType.日结工资申请&& f.Status == PrepaidRecordStatusEnumType.已发放&&f.SendTime!=null&&f.SendTime.Value>= dt);
            prepaidDaySalaryExp.And(f => f.PrepaidPartnerAccountID != Guid.Parse("A03A422F-851D-476D-9115-336084B9AFB1") && f.SigningUnitName != "宁波新莱博金浩供应链管理有限公司");

            var prepaidDaySalarySumAmountt = await salaryStatisticsService.PrepaidApplyRecordSumAmount(prepaidDaySalaryExp.ToExpression());
            counts.Add("prepaidDaySalarySumAmountt", prepaidDaySalarySumAmountt);
            // 预发工资申请,当天发放统计
            var prepaidApplyExp = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
            prepaidApplyExp.AndIF(partnerGID != Guid.Empty, f => f.PartnerGID == partnerGID);
            prepaidApplyExp.And(f => f.ApplyType == (int)PrepaidApplyEnumType.预发工资申请 && f.Status == PrepaidRecordStatusEnumType.已发放 && f.SendTime != null && f.SendTime.Value >= dt);
            prepaidApplyExp.And(f => f.PrepaidPartnerAccountID != Guid.Parse("A03A422F-851D-476D-9115-336084B9AFB1")&&f.SigningUnitName!= "宁波新莱博金浩供应链管理有限公司");
            var prepaidApplyDaySendSumAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(prepaidApplyExp.ToExpression());
            counts.Add("prepaidApplyDaySendSumAmount", prepaidApplyDaySendSumAmount);
            
            // 预发工资待发放统计
            var prepaidExp = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
            prepaidExp.AndIF(partnerGID != Guid.Empty, f => f.PartnerGID == partnerGID);
            prepaidExp.And(f => f.Status == PrepaidRecordStatusEnumType.待发放);
            var prepaidPendingSendCount = await salaryStatisticsService.PrepaidApplyRecordCount(prepaidExp.ToExpression());
            counts.Add("prepaidPendingSendCount", prepaidPendingSendCount);


            
            // 预发工资待还款金额统计
            var prepaidRepayExp = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
            //prepaidExp.AndIF(partnerGID != Guid.Empty, f => f.PartnerGID == partnerGID);
            prepaidRepayExp.And(f => f.Status == PrepaidRecordStatusEnumType.已发放&&f.RepayStatus!= PrepaidRepayStatusEnumType.还款完成);
            prepaidRepayExp.And(f => f.PrepaidPartnerAccountID != Guid.Parse("A03A422F-851D-476D-9115-336084B9AFB1") && f.SigningUnitName != "宁波新莱博金浩供应链管理有限公司");

            var prepaidPendingRepaySumAmount = await salaryStatisticsService.PrepaidApplyRecordPendingRepaySumAmount(prepaidRepayExp.ToExpression());

            counts.Add("prepaidPendingRepaySumAmount", prepaidPendingRepaySumAmount);


            // 工资记录待发放统计
            var salaryExp = Expressionable.Create<Model.Live_V_SalaryRecord>();
            salaryExp.AndIF(partnerGID != Guid.Empty, f => f.PartnerGID == partnerGID);
            salaryExp.And(f =>  f.CompanyID == user.CompanyID && (f.Status == SalaryRecordSendStatusEnumType.待发放 && f.SalaryCompanyStatus == (int)SalaryStatusEnumType.SucessSend));
            var salaryPendingSendCount = await salaryStatisticsService.SalaryRecordViewCount(salaryExp.ToExpression());
            counts.Add("salaryPendingSendCount", salaryPendingSendCount);




            // 工资记录多批次待发放统计
            var splitExp = Expressionable.Create<Model.live_V_SalarySplitSendRecord>();
            splitExp.AndIF(partnerGID != Guid.Empty, f => f.PartnerGID == partnerGID);
            splitExp.And(f => f.CompanyID == user.CompanyID && (f.Status == SalarySplitSendRecordStatusEnumType.待发放 && f.SalaryCustomerStatus !=null&& f.SalaryCustomerStatus ==SalaryStatusEnumType.SucessSend));
            var salarySplitSendRecordCount = await salaryStatisticsService.SalarySplitSendRecordCount(splitExp.ToExpression());
            counts.Add("salarySplitSendRecordCount", salarySplitSendRecordCount);


            // 发票待发放统计
            var invoiceExp = Expressionable.Create<Model.Live_Invoice>();
            invoiceExp.AndIF(partnerGID != Guid.Empty, f => f.PartnerGID == partnerGID);
            invoiceExp.And(f => f.Status == InvoiceStatusEnumType.待开具);
            var invoicePendingIssueCount = await salaryStatisticsService.InvoiceCount(invoiceExp.ToExpression());
            counts.Add("invoicePendingIssueCount", invoicePendingIssueCount);





          
            #endregion

            return Ok(new { statistics, insurance, counts });
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenStatistics()
        {
            try
            {
                var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                var currentYear = new DateTime(currentMonth.Year,1,1) ;
                var today = DateTime.Today;

                var user = CurrentUser;
                var counts = new Dictionary<string, object>();
                var userCount = await userStatisticsService.UserCount(it => 1 > 0);
                counts.Add("userCount", userCount);
                var customerCount = await customerStatisticsService.Count(it => 1 > 0);
                counts.Add("customerCount", customerCount);
                var partnerCount = await partnerStatisticsService.Count(it => it.CompanyID == user.CompanyID);
                counts.Add("partnerCount", partnerCount);
                var personnelCount = await personnelStatisticsService.Count(it => it.CompanyID == user.CompanyID);
                counts.Add("personnelCount", personnelCount);

                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
                var partnerType = new int[] { (int)PartnerType.Self, (int)PartnerType.Hire, (int)PartnerType.Ordinary };
                expable.And(it => statuss.Contains(it.Status.Value));
                //expable.And(it => it.CompanyID == user.CompanyID && it.BusinessModel == (int)BusinessModel.Labor && it.PartnerType < (int)PartnerType.Other);
                expable.And(it =>(((it.BusinessModel == (int)BusinessModel.Agent && !it.CustomerName.Contains("顺丰")) || (partnerType.Contains(it.PartnerType.Value) && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value))) && it.CompanyID== user.CompanyID) || it.CompanyID!= user.CompanyID);
                var statusCount = await entryStatisticsService.StatusCount(expable.ToExpression());
                if (statusCount.Count > 0)
                {
                    var data = statusCount.Find(it => it.Status == (int)UserStatus.Entry);
                    if (data != null)
                    {
                        counts.Add("onTheJobCount", data.Count+ 1157);
                    }
                    data = statusCount.Find(it => it.Status == (int)UserStatus.WaitEntry);
                    if (data != null)
                    {
                        counts.Add("waitEntry", data.Count);
                    }
                }
                var dayEntryExpable = Expressionable.Create<Model.Live_V_UserEntry>();
                dayEntryExpable.And(it => SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && (it.Status == (int)UserStatus.Entry || it.Status == (int)UserStatus.Leave) && it.CompanyID == user.CompanyID && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                var count = await entryStatisticsService.Count(dayEntryExpable.ToExpression());
                counts.Add("toDayEntryCount", count);

                var dayLeaveExpable = Expressionable.Create<Model.Live_V_UserEntry>();
                dayLeaveExpable.And(it => SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave && it.CompanyID == user.CompanyID && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                count = await entryStatisticsService.Count(dayLeaveExpable.ToExpression());
                counts.Add("toDayLeaveCount", count);

                #region 民生电子卡、代发统计
                // 电子卡开户数
                int dianZiKaNum = await cmbcBankCardStatisticsService.RegisterCount();
                //民生当月代发金额
                var daiFaMonthAmount = await salaryStatisticsService.TransBatchSumAmount(f => f.TransStatus == "02" && f.CreateTime >= currentMonth);
                //民生代发总金额
                var daiFaAllAmount = await salaryStatisticsService.TransBatchSumAmount(f => f.TransStatus == "02");
                //民生代发总条数
                var daiFaAllCount = await salaryStatisticsService.TransBatchRecordCount(f => f.TransAmount > 0);
                //电子卡留存金额
                decimal liuCunAllAmount = await cmbcBankCardStatisticsService.AvailableSumAmount();

                counts.Add("dianZiKaNum", dianZiKaNum);
                counts.Add("daiFaMonthAmount", daiFaMonthAmount);
                counts.Add("daiFaAllAmount", daiFaAllAmount);
                counts.Add("liuCunAllAmount", liuCunAllAmount);

                #endregion
                #region 预发工资统计
                //预发工资笔数汇总
                var yuFaNum = await prepaidApplyStatisticsService.Count(f => f.ApplyType == PrepaidApplyEnumType.预发工资申请 && f.Status == PrepaidRecordStatusEnumType.已发放);
                var yuFaAllAmount = await prepaidApplyStatisticsService.ApplySumAmount(f => f.ApplyType == PrepaidApplyEnumType.预发工资申请 && f.Status == PrepaidRecordStatusEnumType.已发放);
                //今日预发工资笔数
                var yuFaDayNum = await prepaidApplyStatisticsService.Count(f => f.ApplyType == PrepaidApplyEnumType.预发工资申请 && f.Status == PrepaidRecordStatusEnumType.已发放 && f.SendTime.HasValue && f.SendTime.Value >= today);
                //本月预发工资笔数
                var yuFaMonthNum = await prepaidApplyStatisticsService.Count(f => f.ApplyType == PrepaidApplyEnumType.预发工资申请 && f.Status == PrepaidRecordStatusEnumType.已发放 && f.SendTime.HasValue && f.SendTime.Value >= currentMonth);
                // 年度预发工资笔数
                var yuFaYearNum = await prepaidApplyStatisticsService.Count(f => f.ApplyType == PrepaidApplyEnumType.预发工资申请 && f.Status == PrepaidRecordStatusEnumType.已发放 && f.SendTime.HasValue && f.SendTime.Value.Year >= currentMonth.Year);
                var month = DateTime.Today.Date.AddDays(1 - DateTime.Today.Day).AddMonths(-3);
                var startMonth = new DateTime(month.Year, 3, 1);
                var endMonth = month.AddMonths(4);
                // 平台垫付本年度笔数
                var paltformPaymentYearNum = await prepaidApplyStatisticsService.ViewCount(f => f.PaymentType.HasValue && f.PaymentType.Value == (int)PrepaidPaymentEnumType.平台垫付模式 && f.Status == PrepaidRecordStatusEnumType.已发放 && f.SendTime.HasValue && f.SendTime.Value >= startMonth && f.SendTime.Value < endMonth);
                // 平台垫付本年度金额
                var paltformPaymentYearAmount = await prepaidApplyStatisticsService.ViewApplySumAmount(f => f.PaymentType.HasValue && f.PaymentType.Value == (int)PrepaidPaymentEnumType.平台垫付模式 && f.Status == PrepaidRecordStatusEnumType.已发放 && f.SendTime.HasValue && f.SendTime.Value >= startMonth && f.SendTime.Value < endMonth);

                counts.Add("yuFaNum", yuFaNum);
                counts.Add("yuFaAllAmount", yuFaAllAmount);
                counts.Add("yuFaDayNum", yuFaDayNum);
                counts.Add("yuFaMonthNum", yuFaMonthNum);
                counts.Add("yuFaYearNum", yuFaYearNum);
                counts.Add("paltformPaymentYearNum", paltformPaymentYearNum);
                counts.Add("paltformPaymentYearAmount", paltformPaymentYearAmount);
                #endregion

                //保险推送次数
                var baoXianTuiSongNum = await businessInsuranceRecordService.Count(f => f.Status >= BusinessInsuranceStatusEnumType.已增员 && f.Status < BusinessInsuranceStatusEnumType.作废);
                counts.Add("baoXianTuiSongNum", baoXianTuiSongNum);

                //网签合同
                int heTongNum = await contractStatisticsService.EntryContractCount(f => f.ContractType == ContractEnumType.入职声明 || f.ContractType == ContractEnumType.入职合同 || f.ContractType == ContractEnumType.离职合同);
                counts.Add("heTongNum", heTongNum);

                //电子工资条
                int gongZiTiaoNum = await salaryStatisticsService.SalaryRecordSlipCount(f => !string.IsNullOrWhiteSpace(f.Name));
                counts.Add("gongZiTiaoNum", gongZiTiaoNum);

                //个税计算
                int geShuiNum = await salaryStatisticsService.SalaryRecordViewCount(f => !string.IsNullOrWhiteSpace(f.IdCardNo));
                counts.Add("geShuiNum", geShuiNum);

                //发票智能生成(企业工资发票)
                int faPiaoNum = await invoiceService.Count(f => f.SalaryCustomerID.HasValue && f.SalaryCustomerID.Value != Guid.Empty);
                counts.Add("faPiaoNum", faPiaoNum);
                //现金签收单条数
                int xianJinHeTongNum = await salaryStatisticsService.SalaryCashCount(f => f.TotalAmount > 0);
                counts.Add("xianJinHeTongNum", xianJinHeTongNum);

                #region 江北开票
                var jiangBeiDayAmount = await invoiceService.SumAmount(f => f.IsSf == false && f.IssuedTime.HasValue && f.IssuedTime.Value >= today);
                var jiangBeiMonthAmount = await invoiceService.SumAmount(f => f.IsSf == false && f.IssuedTime.HasValue && f.IssuedTime.Value >= currentMonth);
                var jiangBeiYearAmount = await invoiceService.SumAmount(f => f.IsSf == false && f.IssuedTime.HasValue && f.IssuedTime.Value.Year >= currentMonth.Year);

                counts.Add("jiangBeiDayAmount", jiangBeiDayAmount);
                counts.Add("jiangBeiMonthAmount", jiangBeiMonthAmount);
                counts.Add("jiangBeiYearAmount", jiangBeiYearAmount);
                #endregion

                #region 嘉善开票
                var jiaShanDayAmount = await invoiceService.SumAmount(f => f.IsSf == true && f.IssuedTime.HasValue && f.IssuedTime.Value >= today);
                var jiaShanMonthAmount = await invoiceService.SumAmount(f => f.IsSf == true && f.IssuedTime.HasValue && f.IssuedTime.Value >= currentMonth);
                var jiaShanYearAmount = await invoiceService.SumAmount(f => f.IsSf == true && f.IssuedTime.HasValue && f.IssuedTime.Value.Year >= currentMonth.Year);
                counts.Add("jiaShanDayAmount", jiaShanDayAmount);
                counts.Add("jiaShanMonthAmount", jiaShanMonthAmount);
                counts.Add("jiaShanYearAmount", jiaShanYearAmount);
                #endregion


                #region 本年度开票合计
                var invoiceDaySumAmount = await invoiceService.SumAmount(f =>  f.IssuedTime.HasValue && f.IssuedTime.Value>= today);
                var invoiceMonthSumAmount = await invoiceService.SumAmount(f =>  f.IssuedTime.HasValue && f.IssuedTime.Value>= currentMonth);
                var invoiceYearSumAmount = await invoiceService.SumAmount(f =>  f.IssuedTime.HasValue && f.IssuedTime.Value>= currentYear);
               //  今日开票
                counts.Add("invoiceDaySumAmount", invoiceDaySumAmount);
                // 本月累计开票
                counts.Add("invoiceMonthSumAmount", invoiceMonthSumAmount);
                // 年度累计开票
                counts.Add("invoiceYearSumAmount", invoiceYearSumAmount);
                #endregion

                #region 商保统计
                List<string> workClassList = new List<string>() {
            "1-4类","1-3类","3类","4类"
            };
                //1-4类参保人数
                var insuranceFourNum = await entryStatisticsService.Count(f => f.Status == (int)UserStatus.Entry && workClassList.Contains(f.WorkClass) && f.PlanStatus1 == UserEntryPlanStatusEnumType.已增员 && f.BusinessModel == (int)BusinessModel.Labor && f.PartnerType < (int)PartnerType.Other);
                // 5类参保人数
                var insuranceFiveNum = await entryStatisticsService.Count(f => f.Status == (int)UserStatus.Entry && f.WorkClass == "5类" && f.PlanStatus1 == UserEntryPlanStatusEnumType.已增员 && f.BusinessModel == (int)BusinessModel.Labor && f.PartnerType < (int)PartnerType.Other);
                // 总参保人数
                int insuranceAllNum = insuranceFourNum + insuranceFiveNum;
                // 今日增员人数
                int zengYuanDayNum = await entryStatisticsService.Count(f => f.CreateTime >= DateTime.Today && f.PlanStatus1 == UserEntryPlanStatusEnumType.待增员);
                // 今日减员人数
                int jianYuanDayNum = await entryStatisticsService.Count(f =>f.CreateTime>=DateTime.Today&& f.PlanStatus1 == UserEntryPlanStatusEnumType.待减员);
                // 参保失败人数
                int shiBaiDayNum = await businessInsuranceRecordService.Count(f => (f.Status == BusinessInsuranceStatusEnumType.减员失败 || f.Status == BusinessInsuranceStatusEnumType.减员失败) && f.CreateTime >= today);


                counts.Add("insuranceFourNum", insuranceFourNum);
                counts.Add("insuranceFiveNum", insuranceFiveNum);
                counts.Add("insuranceAllNum", insuranceAllNum);
                counts.Add("zengYuanDayNum", zengYuanDayNum);
                counts.Add("jianYuanDayNum", jianYuanDayNum);
                counts.Add("shiBaiDayNum", shiBaiDayNum);



                #endregion
                // 保费实收 年度统计
                var baoFeiYearAmount = await grossManageService.ProfitBusInsuranceSumAmount(f => f.ProfitMonth.HasValue && f.ProfitMonth.Value.Year >= today.Year);
                counts.Add("baoFeiYearAmount", baoFeiYearAmount);

                //年度工伤人员
                var year = DateTime.Now.AddMonths(-3).Year;
                var injurylist = await injuryStatisticService.InfoList(f => (f.ProcessStatus != 0 && f.ProcessStatus != 2 && f.ProcessStatus != 9) && f.ShowTime.HasValue && f.ShowTime.Value.AddMonths(-3).Year == year);
                //医疗期工伤人数
                int medicalPeriodCount = injurylist.Where(t => t.ProcessStatus == 4).Count();
                //理赔期工伤人数
                int claimsPeriodCount = injurylist.Where(t => t.ProcessStatus == 6).Count();
                //今日上传工伤事故报告
                int todayInjuryCount = injurylist.Where(t => t.ShowTime>=today).Count();
                //年度累计理赔金额
                decimal insuranceTotalAmount = injurylist.Sum(t => t.InsuranceTotalAmount);
                //累计风险金垫付金额
                decimal riskTotalAmount = injurylist.Sum(t => t.RiskTotalAmount);

                //工伤垫付笔数汇总
                int loanTotalNum = await injuryStatisticService.ApplyRecordCount(f => f.MoneyType == 1 && f.Status == 3 && f.CreateTime.HasValue && f.CreateTime.Value.AddMonths(-3).Year == year);
                //工伤垫付累计金额汇总
                decimal loanTotalAmount = await injuryStatisticService.ApplyRecorSumAmount(f => f.MoneyType == 1 && f.Status == 3 && f.CreateTime.HasValue && f.CreateTime.Value.AddMonths(-3).Year == year);
                counts.Add("medicalPeriodCount", medicalPeriodCount);
                counts.Add("claimsPeriodCount", claimsPeriodCount);
                counts.Add("todayInjuryCount", todayInjuryCount);
                counts.Add("insuranceTotalAmount", insuranceTotalAmount);
                counts.Add("riskTotalAmount", riskTotalAmount);
                counts.Add("loanTotalNum", loanTotalNum);
                counts.Add("loanTotalAmount", loanTotalAmount);

                //业务信息
                int saleCustomerCount = await cmbcBankCardStatisticsService.SaleCustomerCount();
                counts.Add("saleCustomerCount", saleCustomerCount);
                counts.Add("visitCount", 336500);
                counts.Add("imCount", 27500);
                counts.Add("searchCount", 9526);


                var billMonth = currentMonth.AddMonths(-1);
                var billYear = currentMonth.AddMonths(-4).Year;

                //技术金融付费
                var monthPrepaidInterestAmount = await prepaidApplyRecordService.PrepaidInterestReportSumAmount(f=> f.BillMonth== billMonth);
                var yearPrepaidInterestAmount = await prepaidApplyRecordService.PrepaidInterestReportSumAmount(f => f.BillYear == billYear);
                counts.Add("monthPrepaidInterestAmount", monthPrepaidInterestAmount);
                counts.Add("yearPrepaidInterestAmount", yearPrepaidInterestAmount);

           



                return Ok(counts);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "错误");
            }

           
        }


    }
}
