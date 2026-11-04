using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Enums.Injury;
using LZ.Statistics.Service.Interface;
using LZ.Statistics.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using SqlSugar;
using System.Globalization;
namespace LZ.Statistics.API.Controllers
{
    [AllowAnonymous]
    public class ExhibitionStatisticsController(IRedisService redisService,ISalaryStatisticsService salaryStatisticsService,IInvoiceService invoiceService,IEntryStatisticsService entryStatisticsService,IContractStatisticsService contractStatisticsService, IInjuryStatisticService injuryStatisticService, IPartnerStatisticsService partnerStatisticsService,IEntryRecordService entryRecordService, IPrepaidApplyStatisticsService prepaidApplyStatisticsService) : StatisticsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> TotalVisits()
        {
            var dayTotalVisits = await redisService.GetAsync<int>(CoreRedisConstants.DayTotalVisits);
            var totalVisits = await redisService.GetAsync<int>(CoreRedisConstants.TotalVisits);
            return Ok(new { totalVisits = totalVisits + dayTotalVisits, dayTotalVisits = dayTotalVisits });
        }
        [HttpGet]
        public async Task<IActionResult> ChangeWorkCompany()
        {
            var workModel = await entryRecordService.Model(it=>it.About.Contains("更改工种操作"));
            var companyModel = await entryRecordService.Model(it => it.About.Contains("该员工已在其他地方入职"));
            var dayTotalChangeWork = await redisService.GetAsync<int>(CoreRedisConstants.DayTotalChangeWork);
            var totalChangeWork = await redisService.GetAsync<int>(CoreRedisConstants.TotalChangeWork);
            var dayTotalChangeCompany = await redisService.GetAsync<int>(CoreRedisConstants.DayTotalChangeCompany);
            var totalChangeCompany = await redisService.GetAsync<int>(CoreRedisConstants.TotalChangeCompany);
            var workData=new Dictionary<string, object>();
            var companyData = new Dictionary<string, object>();
            if (workModel != null)
            {
                workData.Add("name", workModel.Name);
                workData.Add("cardid", workModel.CardID);
            }
            if (companyModel != null)
            {
                companyData.Add("name", companyModel.FullName);
                companyData.Add("cardid", companyModel.CardID);
            }
            return Ok(new { totalChangeWork = totalChangeWork + dayTotalChangeWork, dayTotalChangeWork = dayTotalChangeWork, totalChangeCompany = totalChangeCompany + dayTotalChangeCompany, dayTotalChangeCompany = dayTotalChangeCompany,workData, companyData });
        }
        static string LowerFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToLower(input[0], CultureInfo.InvariantCulture) + input.Substring(1);
        }
        [HttpGet]
        public async Task<IActionResult> IndexStatistics()
        {
            var counts = new Dictionary<string, object>();

            var count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.PartnerType == (int)PartnerType.Ordinary && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            counts.Add(LowerFirstLetter(PersonnelFunction.StarServiceUserCount.ToString()), count);

            count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.Status == (int)UserStatus.Entry && it.BusinessModel == (int)BusinessModel.Agent && !it.CustomerName.Contains("顺丰"));
            counts.Add(LowerFirstLetter(PersonnelFunction.LaborrelationUserCount.ToString()), count + 1157);

            count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.PartnerType == (int)PartnerType.Self && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            counts.Add(LowerFirstLetter(PersonnelFunction.SelfUserCount.ToString()), count);

            count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.PartnerType == (int)PartnerType.Hire && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            counts.Add(LowerFirstLetter(PersonnelFunction.HirePartnerUserCount.ToString()), count);

            count = await entryStatisticsService.Count(it => it.CompanyID != 1 && it.Status == (int)UserStatus.Entry);
            counts.Add(LowerFirstLetter(PersonnelFunction.UnionUserCount.ToString()), count);

            count = 0;
            var auths = new string[] { "starServiceUserCount", "selfUserCount", "hirePartnerUserCount", "laborrelationUserCount", "unionUserCount" };
            foreach (var auth in auths)
            {
                if (counts.ContainsKey(auth))
                {
                    count += (int)counts[auth];
                }
            }
            counts.Add(LowerFirstLetter(PersonnelFunction.AllServiceUserCount.ToString()), count);
            counts.Add("companyUserCount", 0);

            var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
            var statusCount = await entryStatisticsService.StatusCount(it => statuss.Contains(it.Status.Value));
            if (statusCount.Count > 0)
            {
                var data = statusCount.Find(it => it.Status == (int)UserStatus.Entry);
                if (data != null)
                {
                    counts.Add(LowerFirstLetter(PersonnelFunction.OnTheJobCount.ToString()), data.Count);
                }
                data = statusCount.Find(it => it.Status == (int)UserStatus.WaitEntry);
                if (data != null)
                {
                    counts.Add(LowerFirstLetter(PersonnelFunction.WaitEntryCount.ToString()), data.Count);
                }
            }

            count = await entryStatisticsService.Count(it => SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && (it.Status == (int)UserStatus.Entry || it.Status == (int)UserStatus.Leave));
            counts.Add(LowerFirstLetter(PersonnelFunction.ToDayEntryCount.ToString()), count);

            count = await entryStatisticsService.Count(it => SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave);
            counts.Add(LowerFirstLetter(PersonnelFunction.ToDayLeaveCount.ToString()), count);


            count = await partnerStatisticsService.Count(it => it.CompanyID == 1 && it.PartnerType == (int)PartnerType.Ordinary);
            counts.Add(LowerFirstLetter(PersonnelFunction.StarServiceCount.ToString()), count);
            counts.Add("businessPartner", 100);
            counts.Add("franchiseCompany", 100);
            counts.Add("batchCustomers", 100);
            

            return Ok(counts);
        }

        #region  大屏初始化
        public record PrepaidDailySalaryInitRecord(
            string Title,
            decimal prepaidTodayAmount,
            decimal prepaidRepayTodayAmount,
            decimal prepaidMonthAmount,
            decimal prepaidRepayMonthAmount,
            decimal prepaidAmount,
            decimal prepaidRepayAmount,
            decimal dailyTodayAmount,
            decimal dailyRepayTodayAmount,
            decimal dailyMonthAmount,
            decimal dailyMonthRepayAmount,
            decimal dailyAmount,
            decimal dailyRepayAmount
            );
        /// <summary>
        /// 安途商管资金封闭运作
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenPrepaidDailySalaryInit()
        {
            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);

            #region 借支统计
            // 今日借支
            var prepaidTodayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status != PrepaidRecordStatusEnumType.审核退回 && f.CreateTime >= today && f.ApplyType == 0);
            // 今日还款
            var prepaidRepayTodayTotalAmount = await salaryStatisticsService.PrepaidRepaymentRecordRepaySumAmount(f => f.CreateTime >= today && f.ApplyType == 0);
            // 本月借支
            var prepaidMonthTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status != PrepaidRecordStatusEnumType.审核退回 && f.CreateTime >= currentMonth && f.ApplyType == 0);
            // 本月还款
            var prepaidRepayMonthTotalAmount = await salaryStatisticsService.PrepaidRepaymentRecordRepaySumAmount(f => f.CreateTime >= currentMonth && f.ApplyType == 0);
            // 累计借支
            var prepaidTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status != PrepaidRecordStatusEnumType.审核退回 && f.ApplyType == 0);
            // 累计还款
            var prepaidRepayTotalAmount = await salaryStatisticsService.PrepaidRepaymentRecordRepaySumAmount(f => f.ApplyType == 0);

            #endregion
            #region 日结统计
            // 今日借支
            var dailyTodayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status != PrepaidRecordStatusEnumType.审核退回 && f.CreateTime >= today && f.ApplyType == 1);
            // 今日还款
            var dailyRepayTodayTotalAmount = await salaryStatisticsService.PrepaidRepaymentRecordRepaySumAmount(f => f.CreateTime >= today && f.ApplyType == 1);
            // 本月借支
            var dailyMonthTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status != PrepaidRecordStatusEnumType.审核退回 && f.CreateTime >= currentMonth && f.ApplyType == 1);
            // 本月还款
            var dailyRepayMonthTotalAmount = await salaryStatisticsService.PrepaidRepaymentRecordRepaySumAmount(f => f.CreateTime >= currentMonth && f.ApplyType == 1);
            // 累计借支
            var dailyTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status != PrepaidRecordStatusEnumType.审核退回 && f.ApplyType == 1);
            // 累计还款
            var dailyRepayTotalAmount = await salaryStatisticsService.PrepaidRepaymentRecordRepaySumAmount(f => f.ApplyType == 1);

            #endregion
            PrepaidDailySalaryInitRecord record = new PrepaidDailySalaryInitRecord(
                "安途商管资金封闭运作", prepaidTodayTotalAmount, prepaidRepayTodayTotalAmount, prepaidMonthTotalAmount, prepaidRepayMonthTotalAmount, prepaidTotalAmount, prepaidRepayTotalAmount,
                dailyTodayTotalAmount, dailyRepayTodayTotalAmount, dailyMonthTotalAmount, dailyRepayMonthTotalAmount, dailyTotalAmount, dailyRepayTotalAmount
                );
            return Ok(record);
        }
        public record CooperativeParkInitRecord(int Type, string Title, decimal MonthTotalAmount, decimal TotalAmount);
        /// <summary>
        ///  合作产业园
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenCooperativeParkInit()
        {

            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);
            var currentYear = new DateTime(currentMonth.Year, 1, 1);
            List<CooperativeParkInitRecord> records = new List<CooperativeParkInitRecord>();
            #region 江北开票
            var jiangBeiMonthAmount = await invoiceService.SumAmount(f => f.IsSf == false && f.IssuedTime.HasValue && f.IssuedTime.Value >= currentMonth);
            var jiangBeiTotalAmount = await invoiceService.SumAmount(f => f.IsSf == false && f.IssuedTime.HasValue && f.IssuedTime.Value >= currentYear);
            CooperativeParkInitRecord jiangBeiRecord = new CooperativeParkInitRecord(0, "江北产业园", jiangBeiMonthAmount, jiangBeiTotalAmount);
            records.Add(jiangBeiRecord);
            #endregion
            #region 嘉善开票
            //var jiaShanDayAmount = await invoiceService.SumAmount(f => f.IsSf == true && f.IssuedTime.HasValue && f.IssuedTime.Value >= today);
            var jiaShanMonthAmount = await invoiceService.SumAmount(f => f.IsSf == true && f.IssuedTime.HasValue && f.IssuedTime.Value >= currentMonth);
            var jiaShanTotalAmount = await invoiceService.SumAmount(f => f.IsSf == true && f.IssuedTime.HasValue && f.IssuedTime.Value >= currentYear);
            CooperativeParkInitRecord jiaShanRecord = new CooperativeParkInitRecord(1, "嘉善产业园", jiaShanMonthAmount, jiaShanTotalAmount);
            records.Add(jiaShanRecord);
            #endregion
            return Ok(records);
        }



        public record CooperativeBankInitRecord(int Type, string Title, int TodayCount, int MonthCount, int TotalCount);
        /// <summary>
        /// 合作银行
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenCooperativeBankInit()
        {
            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);
            var minShengTodayEmployeeCount = await salaryStatisticsService.TransBatchRecordEmployeeCount(f => f.CreateTime >= today);
            var minShengMonthEmployeeCount = await salaryStatisticsService.TransBatchRecordEmployeeCount(f => f.CreateTime >= currentMonth);
            var minShengEmployeeTotalCount = await salaryStatisticsService.TransBatchRecordEmployeeCount(f => f.TotalAmount > 0);
            List<CooperativeBankInitRecord> records = new List<CooperativeBankInitRecord>();
            CooperativeBankInitRecord minShengRecord = new CooperativeBankInitRecord(0, "民生银行", minShengTodayEmployeeCount, minShengMonthEmployeeCount, minShengEmployeeTotalCount);
            records.Add(minShengRecord);
            return Ok(records);
        }


        public record CooperativeInsuranceCompanyInitRecord(int Type, string Title, int TodayCount, int TotalCount,int TodayDelCount);
        /// <summary>
        /// 合作保司
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenCooperativeInsuranceCompanyInit()
        {

            var today = DateTime.Today;
            // 阳光保险公司
            var ygbxTodayCount = await entryStatisticsService.Count(f => (f.PlanStatus1 == UserEntryPlanStatusEnumType.待增员 || f.PlanStatus1 == null) && f.Status == 2 && f.CreateTime >= today);
            var ygbxTodayDelCount = await entryStatisticsService.Count(f => (f.PlanStatus1 != null && f.PlanStatus1 == UserEntryPlanStatusEnumType.待减员) && f.Status == 4 );
            var ygbxTotalCount = await entryStatisticsService.Count(f => f.Status == 2 && f.PlanStatus1 == UserEntryPlanStatusEnumType.已增员);
            CooperativeInsuranceCompanyInitRecord yangGuangRecord = new CooperativeInsuranceCompanyInitRecord(0, "阳光保险公司", ygbxTodayCount, ygbxTotalCount, ygbxTodayDelCount);
            List<CooperativeInsuranceCompanyInitRecord> records = new List<CooperativeInsuranceCompanyInitRecord>();
            records.Add(yangGuangRecord);
            return Ok(records);
        }


        public record SalaryCalculateInitRecord(Guid KeyID, string CustomerName, int RecordCount, string CalculateStatusName);
        /// <summary>
        ///  实时动态流水
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenSalaryCalculateInit()
        {
            var salaryCustomerTop10 = await salaryStatisticsService.ListSalaryCustomer(f => f.Status >= SalaryStatusEnumType.AwaitingCalculation);
            List<SalaryCalculateInitRecord> records = new List<SalaryCalculateInitRecord>();
            if (salaryCustomerTop10 != null && salaryCustomerTop10.Count > 0)
            {
                records = salaryCustomerTop10.Select(f => new SalaryCalculateInitRecord(f.ID, f.CustomerName, f.RecordCount, $"{(f.Status == SalaryStatusEnumType.AwaitingCalculation ? "正在计算" : "完成计算")}")).ToList();
            }
            return Ok(records);
        }


        public record LargeScreenSalaryCalculateTimesInitRecord(string Title, long TodayTimes, long MonthTimes, long YearTimes);
        /// <summary>
        /// 实时计算请求
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenSalaryCalculateTimesInit()
        {
            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);
            //今天已计算
            var calculateTimes = salaryStatisticsService.SalaryCustomerCalculateTimes();
            LargeScreenSalaryCalculateTimesInitRecord record = new LargeScreenSalaryCalculateTimesInitRecord("实时计算请求", calculateTimes.TodayTimes, calculateTimes.MonthTimes, calculateTimes.YearTimes);
           

            return Ok(record);




        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenSalaryRangeInit()
        {
            var salaryRecordRangeReport = salaryStatisticsService.SalaryRangeReport();
            return Ok(salaryRecordRangeReport);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenSpecialDeductionInit()
        {
            var specialDeductionReport = salaryStatisticsService.SpecialDeductionReport();
            return Ok(specialDeductionReport);
        }



        public record PrepaidSalaryInitRecord(Guid KeyID, string PartnerName, decimal MonthTotalAmount, decimal YearTotalAmount);
        /// <summary>
        ///  借支垫付
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenPrepaidSalaryInit()
        {

            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);
            // 垫付
            var prepaidGroupItmes = await salaryStatisticsService.PrepaidPartnerGroup();
            var prepaidSalaryList = prepaidGroupItmes.Where(f => f.ApplyType == 0).ToList();
            List<PrepaidSalaryInitRecord> records = new List<PrepaidSalaryInitRecord>();
            if (prepaidSalaryList != null && prepaidSalaryList.Count > 0)
            {
                records = prepaidSalaryList.Select(f => new PrepaidSalaryInitRecord(f.PartnerGID, f.PartnerName, f.MonthTotalAmount, f.YearTotalAmount)).ToList();
            }
            return Ok(records);
        }
        public record DailySalaryInitRecord(Guid KeyID, string CustomerName, decimal MonthTotalAmount, decimal YearTotalAmount);
        /// <summary>
        /// 日结垫付
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenDailySalaryInit()
        {

            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);
            // 垫付
            var prepaidGroupItmes = await salaryStatisticsService.PrepaidCustomerGroup();
            var dailySalaryList = prepaidGroupItmes.Where(f => f.ApplyType == 1).ToList();
            List<DailySalaryInitRecord> records = new List<DailySalaryInitRecord>();
            if (dailySalaryList != null && dailySalaryList.Count > 0)
            {
                records = dailySalaryList.Select(f => new DailySalaryInitRecord(f.CustomerGID, f.CustomerName, f.MonthTotalAmount, f.YearTotalAmount)).ToList();
            }
            return Ok(records);
        }


        public record InsuranceStatusInitRecord(int Type, string Title, string Name, string CardID, int TodayCount);
        /// <summary>
        /// 智慧保险
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenInsuranceStatusInit()
        {

            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);

            var sbTodayAddCount = await entryStatisticsService.Count(f => f.Status == 2 && f.PlanStatus1 == UserEntryPlanStatusEnumType.待增员);
            var sbTodayAddEmp = await entryStatisticsService.GetView(f => f.Status == 2 &&  f.PlanStatus1 == UserEntryPlanStatusEnumType.待增员);
            InsuranceStatusInitRecord addRecord = new InsuranceStatusInitRecord(0, "已投保待生效", sbTodayAddEmp != null ? sbTodayAddEmp.Name : string.Empty, sbTodayAddEmp != null ? sbTodayAddEmp.CardID : string.Empty, sbTodayAddCount);
            var sbTodaydelCount = await entryStatisticsService.Count(f => f.Status == 4 && f.PlanStatus1 == UserEntryPlanStatusEnumType.待减员);
            var sbTodaydelEmp = await entryStatisticsService.GetView(f => f.Status == 4 && f.PlanStatus1 == UserEntryPlanStatusEnumType.待减员);
            InsuranceStatusInitRecord delRecord = new InsuranceStatusInitRecord(1, "已减员待生效", sbTodaydelEmp != null ? sbTodaydelEmp.Name : string.Empty, sbTodaydelEmp != null ? sbTodaydelEmp.CardID : string.Empty, sbTodaydelCount);
            List<InsuranceStatusInitRecord> records = new List<InsuranceStatusInitRecord>();
            records.Add(addRecord);
            records.Add(delRecord);
            return Ok(records);



        }

        public record PrepaidSalaryStatusInit(int Type, string Title, string CustomerName, string Name, string CardID, decimal ApplyAmount,string transTime, decimal TodayTotalAmount, decimal TotalAmount);
        /// <summary>
        /// 智能借支
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenPrepaidSalaryStatusInit()
        {

            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);

            Live_V_PrepaidApplyRecord defaultRecord = new Live_V_PrepaidApplyRecord()
            {
                PartnerName = string.Empty,
                Name = string.Empty,
                IdCardNo = string.Empty,
                ApplyAmount = 0,
                CreateTime=DateTime.Now,
                AuditTime = DateTime.Now,   
                SendTime = DateTime.Now,
                
            };

            List<PrepaidSalaryStatusInit> records = new List<PrepaidSalaryStatusInit>();
            #region 待审核
            var prepaidPendingAuditTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status == PrepaidRecordStatusEnumType.待审核);
            var prepaidPendingAuditTodayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status == PrepaidRecordStatusEnumType.待审核 && f.CreateTime >= today);
            var prepaidPendingAuditRecord = await salaryStatisticsService.GetPrepaidApplyRecord(f => f.Status == PrepaidRecordStatusEnumType.待审核 && f.CreateTime >= today);
            if (prepaidPendingAuditRecord == null)
            {
                prepaidPendingAuditRecord = defaultRecord;
            }
            var pendingRecord = new PrepaidSalaryStatusInit(0, "待审核借支", prepaidPendingAuditRecord.PartnerName, prepaidPendingAuditRecord.Name, prepaidPendingAuditRecord.IdCardNo, prepaidPendingAuditRecord.ApplyAmount, prepaidPendingAuditRecord.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), prepaidPendingAuditTodayTotalAmount, prepaidPendingAuditTotalAmount);
            records.Add(pendingRecord);
            #endregion


            #region 已发放
            var prepaidSendTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放);
            var prepaidSendTodayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.CreateTime >= today);
            var prepaidSendRecord = await salaryStatisticsService.GetPrepaidApplyRecord(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.CreateTime >= today&&f.SendTime!=null);
            if (prepaidSendRecord == null)
            {
                prepaidSendRecord = defaultRecord;
            }
            var sendSucRecord = new PrepaidSalaryStatusInit(1, "已发放借支", prepaidSendRecord.PartnerName, prepaidSendRecord.Name, prepaidSendRecord.IdCardNo, prepaidSendRecord.ApplyAmount, prepaidSendRecord.SendTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), prepaidSendTodayTotalAmount, prepaidSendTotalAmount);
            records.Add(sendSucRecord);
            #endregion

            #region 待还款
            var prepaidPendingRepayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordPendingRepaySumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus != PrepaidRepayStatusEnumType.无 && f.RepayStatus != PrepaidRepayStatusEnumType.还款完成);
            var prepaidPendingRepayTodayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordPendingRepaySumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus != PrepaidRepayStatusEnumType.无 && f.RepayStatus != PrepaidRepayStatusEnumType.还款完成 && f.CreateTime >= today);
            var prepaidPendingRepayRecord = await salaryStatisticsService.GetPrepaidApplyRecord(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus != PrepaidRepayStatusEnumType.无 && f.RepayStatus != PrepaidRepayStatusEnumType.还款完成 && f.CreateTime >= today);
            if (prepaidPendingRepayRecord == null)
            {
                prepaidPendingRepayRecord = defaultRecord;
            }
            var pendingRepayRecord = new PrepaidSalaryStatusInit(2, "借支待还款", prepaidPendingRepayRecord.PartnerName, prepaidPendingRepayRecord.Name, prepaidPendingRepayRecord.IdCardNo, prepaidPendingRepayRecord.ApplyAmount, prepaidPendingRepayRecord.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), prepaidPendingRepayTodayTotalAmount, prepaidPendingRepayTotalAmount);
            records.Add(pendingRepayRecord);
            #endregion
            #region 还款完成
            var prepaidSucRepayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus == PrepaidRepayStatusEnumType.还款完成);
            var prepaidSucRepayTodayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordPendingRepaySumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus == PrepaidRepayStatusEnumType.还款完成 && f.RepayCompleteTime != null && f.RepayCompleteTime.Value >= today);
            var prepaidSucRepayRecord = await salaryStatisticsService.GetPrepaidApplyRecord(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus == PrepaidRepayStatusEnumType.还款完成 && f.RepayCompleteTime != null && f.RepayCompleteTime.Value >= today);

            if (prepaidSucRepayRecord == null)
            {
                prepaidSucRepayRecord = defaultRecord;
            }
            var sucRepayRecord = new PrepaidSalaryStatusInit(3, "借支还款完成", prepaidSucRepayRecord.PartnerName, prepaidSucRepayRecord.Name, prepaidSucRepayRecord.IdCardNo, prepaidSucRepayRecord.ApplyAmount, prepaidSucRepayRecord.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), prepaidSucRepayTodayTotalAmount, prepaidSucRepayTotalAmount);
            records.Add(sucRepayRecord);
            #endregion
            #region 还款异常
            var prepaidFailRepayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordSumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus == PrepaidRepayStatusEnumType.还款失败);
            var prepaidFailRepayTodayTotalAmount = await salaryStatisticsService.PrepaidApplyRecordPendingRepaySumAmount(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus == PrepaidRepayStatusEnumType.还款失败 && f.RepayCompleteTime != null && f.RepayCompleteTime.Value >= today);
            var prepaidFailRepayRecord = await salaryStatisticsService.GetPrepaidApplyRecord(f => f.Status == PrepaidRecordStatusEnumType.已发放 && f.RepayStatus == PrepaidRepayStatusEnumType.还款失败 && f.RepayCompleteTime != null && f.RepayCompleteTime.Value >= today);

            if (prepaidFailRepayRecord == null)
            {
                prepaidFailRepayRecord = defaultRecord;
            }
            var failRepayRecord = new PrepaidSalaryStatusInit(4, "借支还款异常", prepaidFailRepayRecord.PartnerName, prepaidFailRepayRecord.Name, prepaidFailRepayRecord.IdCardNo, prepaidFailRepayRecord.ApplyAmount, prepaidFailRepayRecord.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"), prepaidFailRepayTodayTotalAmount, prepaidFailRepayTotalAmount);
            records.Add(failRepayRecord);


            #endregion
            return Ok(records);

        }


        /// <summary>
        /// 一道信任的流水线
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenTrustInit()
        {

            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);
            var todayEntryCount = await entryStatisticsService.Count(f => f.UserID != null && f.UserID.Value > 0 && f.Status == 2 && f.EntryTime != null && f.EntryTime.Value >= today);
            var todayContractCount = await contractStatisticsService.EntryContractCount(f => (f.ContractType == ContractEnumType.入职合同 || f.ContractType == ContractEnumType.离职合同) && f.CreateTime >= today);
            var todaySalaryRecordCount = await salaryStatisticsService.SalaryRecordViewCount(f => f.CreateTime >= today);
            var todaySalarySettlementCount = todaySalaryRecordCount;
            // 工资条暂时没有创建时间 临时用
            var todaySalarySlipRecordCount = await salaryStatisticsService.SalaryRecordViewCount(f => f.SalarySlipRecordID != null && f.CreateTime >= today);
            var todaySalarySendCount = await salaryStatisticsService.SalaryRecordViewCount(f => f.SendTime != null && f.SendTime >= today);
            var todayCustomerPaidCount = await salaryStatisticsService.SalaryCustomerCount(f => f.IsCustomerPaid);
            var todayInvoiceCount = await salaryStatisticsService.InvoiceCount(f => f.CreateTime >= today);
            Dictionary<string,int>dsi=new Dictionary<string, int>();
            dsi.Add("todayEntryCount", todayEntryCount);// 实名入职
            dsi.Add("todayContractCount", todayContractCount);//员工网签合同
            dsi.Add("todayAttendanceRecord", 0);//考勤打卡
            dsi.Add("todaySalaryRecordCount", todaySalaryRecordCount);//原始工资单数据
            dsi.Add("todaySalarySettlementCount", todaySalarySettlementCount);//工资计算单数据
            //dsi.Add("todaySalarySettlementCount", todaySalarySettlementCount);//工资结算单
            dsi.Add("ToadyMiniProgram", 0);//客户查询验证
            dsi.Add("todaySalarySlipRecordCount", todaySalarySlipRecordCount);// 员工工资条
            dsi.Add("todaySalarySendCount", todaySalarySendCount);// 工资已发放
            dsi.Add("todayCustomerPaidCount", todayCustomerPaidCount);// 今日客户已打款
            dsi.Add("todayInvoiceCount", todayInvoiceCount);// 今日客户发票
            return Ok(dsi);

        }


        public record ContractRecordStatusInitRecord(int Type, string Title, string Name, string CardID, int TodayCount, int TotalCount);
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenContractRecordStatusInit()
        {
            List<ContractRecordStatusInitRecord> records = new List<ContractRecordStatusInitRecord>();
            var today = DateTime.Today;
            var currentMonth = today.AddDays(1 - today.Day);
            var realNameTodayCount = await contractStatisticsService.ContractSignerCount(f => (f.RealNameType == RealNameEnumType.公安二要素 || f.RealNameType == RealNameEnumType.全网三要素 || f.RealNameType == RealNameEnumType.银行卡四要素) && f.CreateTime >= today);
            var realNameTotalCount = await contractStatisticsService.ContractSignerCount(f => f.Name != "");
            var realNameRecord = await contractStatisticsService.GetContractSigner(f => f.RealNameType == RealNameEnumType.公安二要素 || f.RealNameType == RealNameEnumType.全网三要素 || f.RealNameType == RealNameEnumType.银行卡四要素);
            ContractRecordStatusInitRecord realName3Record = new ContractRecordStatusInitRecord(0, "二/三/四要素认证", realNameRecord?.Name, realNameRecord?.Idno, realNameTodayCount, realNameTotalCount);

            var caFaceTodayCount = await contractStatisticsService.GetCaAuthCount(f => f.AuthType == "Face" && f.Status == CaAuthIdentityStatusEnumType.认证成功 && f.CreateTime >= today);
            var caFaceTotalCount = await contractStatisticsService.GetCaAuthCount(f => f.AuthType == "Face" && f.Status == CaAuthIdentityStatusEnumType.认证成功);
            var caFaceRecord = await contractStatisticsService.GetCaAuthIdentity(f => f.AuthType == "Face" && f.Status == CaAuthIdentityStatusEnumType.认证成功);

            ContractRecordStatusInitRecord realName4Record = new ContractRecordStatusInitRecord(1, "二要素+人脸认证", caFaceRecord?.Name, caFaceRecord?.CardID, caFaceTodayCount, caFaceTotalCount);
            records.Add(realName3Record);
            records.Add(realName4Record);
            //正在入职网签
            var entryContractRecord = await contractStatisticsService.GetEntryContractRecord(f => f.Status == SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同);
            var entryContractRecordTodayCount = await contractStatisticsService.EntryContractCount(f => f.CreateTime >= today && f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同);
            var entryContractRecordTotalCount = await contractStatisticsService.EntryContractCount(f => f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同);
            ContractRecordStatusInitRecord entryPendingRecord = new ContractRecordStatusInitRecord(2, "正在入职网签", entryContractRecord?.EmpName, entryContractRecord?.EmpIdCardNo, entryContractRecordTodayCount, entryContractRecordTotalCount);
            records.Add(entryPendingRecord);

            var leaveContractRecord = await contractStatisticsService.GetEntryContractRecord(f => f.Status == SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.离职合同);
            var leaveContractRecordTodayCount = await contractStatisticsService.EntryContractCount(f => f.CreateTime >= today && f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.离职合同);
            var leaveContractRecordTotalCount = await contractStatisticsService.EntryContractCount(f => f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.离职合同);
            ContractRecordStatusInitRecord leavePendingRecord = new ContractRecordStatusInitRecord(3, "正在离职网签", leaveContractRecord?.EmpName, leaveContractRecord?.EmpIdCardNo, leaveContractRecordTodayCount, leaveContractRecordTotalCount);
            records.Add(leavePendingRecord);

            var renewalContractRecord = await contractStatisticsService.GetEntryContractRecord(f => f.Status == SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同 && f.IsRenewal);
            var renewalContractRecordTodayCount = await contractStatisticsService.EntryContractCount(f => f.CreateTime >= today && f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同 && f.IsRenewal);
            var renewalContractRecordTotalCount = await contractStatisticsService.EntryContractCount(f => f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同 && f.IsRenewal);
            ContractRecordStatusInitRecord renewalPendingRecord = new ContractRecordStatusInitRecord(4, "正在续签合同", renewalContractRecord?.EmpName, renewalContractRecord?.EmpIdCardNo, renewalContractRecordTodayCount, renewalContractRecordTotalCount);
            records.Add(renewalPendingRecord);

            var retireContractRecord = await contractStatisticsService.GetEntryContractRecord(f => f.Status == SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同 && f.IsRetire);
            var retireContractRecordTodayCount = await contractStatisticsService.EntryContractCount(f => f.CreateTime >= today && f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同 && f.IsRetire);
            var retireContractRecordTotalCount = await contractStatisticsService.EntryContractCount(f => f.Status == (int)SignStatusEnumType.待签署 && f.ContractType == ContractEnumType.入职合同 && f.IsRetire);
            ContractRecordStatusInitRecord retirePendingRecord = new ContractRecordStatusInitRecord(5, "正在签署返聘协议", retireContractRecord?.EmpName, retireContractRecord?.EmpIdCardNo, retireContractRecordTodayCount, retireContractRecordTotalCount);
            records.Add(retirePendingRecord);

            var completeContractRecord = await contractStatisticsService.GetEntryContractRecord(f => f.Status == SignStatusEnumType.存证完成);
            var completeContractRecordTodayCount = await contractStatisticsService.EntryContractCount(f => f.CreateTime >= today && f.Status == (int)SignStatusEnumType.存证完成);
            var completeContractRecordTotalCount = await contractStatisticsService.EntryContractCount(f => f.Status == (int)SignStatusEnumType.存证完成);
            ContractRecordStatusInitRecord completeRecord = new ContractRecordStatusInitRecord(6, "网签存证已完成", completeContractRecord?.EmpName, completeContractRecord?.EmpIdCardNo, completeContractRecordTodayCount, completeContractRecordTotalCount);
            records.Add(completeRecord);
            return Ok(records);
        }

        #endregion
        #region 工伤和日结统计
        [HttpGet]
        public async Task<IActionResult> InjuryDemonstration()
        {
            var now = DateTime.Now;
            //工伤待审核最新以及今日数量
            //sendMQService.SendStatisticsMessage("{\"type\":\"scanCode\",\"city\":\"" + city + "\"}");
            var year = now.AddMonths(-3).Year;
            var start = Convert.ToDateTime(year + "-04-01");
            var end = Convert.ToDateTime((year + 1) + "-04-01");
            var waitAuditModel = await injuryStatisticService.GetInfo(t => t.ProcessStatus == (int)InjuryProcessStatus.待审核, "showTime desc");
            var waitAuditCount = await injuryStatisticService.InfoCount(t => t.ProcessStatus == (int)InjuryProcessStatus.待审核 && t.ShowTime.HasValue && t.ShowTime>=start&&t.ShowTime<end);
            var lipeiModel = await injuryStatisticService.GetInfo(t => t.ProcessStatus == (int)InjuryProcessStatus.工伤理赔期, "showTime desc");
            var lipeiCount = await injuryStatisticService.InfoCount(t => t.ProcessStatus == (int)InjuryProcessStatus.工伤理赔期 && t.ShowTime.HasValue && t.ShowTime >= start && t.ShowTime < end);
            var loanModel = await injuryStatisticService.GetApplyRecord(t => t.MoneyType == (int)InjuryApplyType.借款单, "createTime desc");

           
            var loanYearList = await injuryStatisticService.ApplyRecordList(t => t.CreateTime >= start && t.CreateTime < end && t.MoneyType == (int)InjuryApplyType.借款单);
            var paymentList = await injuryStatisticService.PaymentList(t => t.ApplicationTime >= start && t.ApplicationTime < end);

            decimal loanTodayAmount = loanYearList.Where(t => t.CreateTime.HasValue && t.CreateTime.Value.ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd")).Sum(t => t.Money);
            decimal paymentTodayAmount = paymentList.Where(t => t.CreateTime.HasValue && t.CreateTime.Value.ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd")).Sum(t => t.PaymentAmount);
            decimal todayPayAmount = loanTodayAmount + paymentTodayAmount;
            decimal loanYearAmount = loanYearList.Sum(t => t.Money);
            decimal paymentYearAmount = paymentList.Sum(t => t.PaymentAmount);
            decimal yearPayAmount = loanYearAmount + paymentYearAmount;

            //var completeModel = await injuryStatisticService.GetInfo(t => t.ProcessStatus == (int)InjuryProcessStatus.工伤完成, "showTime desc");
            var completeList = await injuryStatisticService.InfoList(t => t.ShowTime >= start && t.ShowTime < end && t.ProcessStatus == (int)InjuryProcessStatus.工伤完成);
            var completeModel = completeList.OrderByDescending(t => t.ShowTime).FirstOrDefault();
            decimal todayLipeiAmount = completeList.Where(t => t.ShowTime.HasValue && t.ShowTime.Value.ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd")).Sum(t => t.InsuranceTotalAmount);
            decimal yearLipeiAmount = completeList.Sum(t => t.InsuranceTotalAmount);

            

            return Ok(new { waitAuditModel, waitAuditCount, lipeiModel, lipeiCount, loanModel, todayPayAmount, yearPayAmount, completeModel, todayLipeiAmount, yearLipeiAmount });
        }

        [HttpGet]
        public async Task<IActionResult> DayWorkDemonstration()
        {
            //var now = DateTime.Now;
            //var attendanceRecord = await attendanceStatistics.GetAttendanceRecord(t => t.AttendanceType == 2, "AttendanceDate desc");
            //var waitAuditModel = await attendanceStatistics.GetPrepaidApplyRecord(t => t.ApplyType == 1 && t.Status==PrepaidRecordStatusEnumType.待审核, "createTime desc");
            //var sendModel = await attendanceStatistics.GetPrepaidApplyRecord(t => t.ApplyType == 1 && t.Status == PrepaidRecordStatusEnumType.已发放, "createTime desc");
            //var completeModel = await attendanceStatistics.GetPrepaidApplyCustomer(t => t.Status==2, "createTime desc");
            var attendanceRecord = new
            {
                CustomerName = "宁波莱博网络科技有限公司",
                UserName = "李策时",
                CardID = "330327199511170812"
            };
            var waitAuditModel = new
            {
                CustomerName = "嘉兴鸿博供应链管理有限公司",
                UserName = "陈科成",
                CardID = "330327199005170812"
            };
            var sendModel = new
            {
                CustomerName = "嘉兴鸿博供应链管理有限公司",
                UserName = "陈科成",
                CardID = "330327199005170812"
            };
            var completeModel = new
            {
                CustomerName = "嘉兴鸿博供应链管理有限公司",
                UserName = "陈科成",
                CardID = "330327199005170812"
            };
            var taxModel = new
            {
                CustomerName = "嘉兴鸿博供应链管理有限公司",
                UserName = "陈科成",
                CardID = "330327199005170812"
            };
            return Ok(new { attendanceRecord, waitAuditModel, sendModel, completeModel, taxModel });
        }

        #endregion

        #region 



        #endregion


        /// <summary>
        ///  年度税费缴纳金额
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenInvoiceTaxTotalAmount()
        {
            var currentYear = new DateTime(DateTime.Today.Year, 1, 1);
            var totalAmount = await salaryStatisticsService.InvoiceSumTax(f => f.CreateTime.Value >= currentYear);
            return Ok(new { title = "年度税费缴纳金额", totalAmount = totalAmount });
        }
        /// <summary>
        /// 年度社保公积金缴纳金
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> LargeScreenSocialEmpTotalAmount()
        {
            var currentYear = new DateTime(DateTime.Today.Year, 1, 1);
            var totalAmount = await salaryStatisticsService.ListEmpSocialTotalAmount(f => f.BillMonth!=null&&  f.BillMonth.Value >= currentYear);
            return Ok(new { title = "年度社保公积金缴纳金", totalAmount = totalAmount });
        }


        #region 技术金融服务费

        public record InterestMonthReportRecord(DateTime month, List<Live_PrepaidInterestReport> records);
        [HttpGet]
        public async Task<IActionResult> GetInterestMonthReport(DateTime startMonth,DateTime endMonth)
        {

            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            endMonth = endMonth.AddDays(1 - endMonth.Date.Day);
            startMonth = startMonth.AddDays(1 - startMonth.Date.Day);
            if (endMonth>= currentMonth)
            {
                endMonth = currentMonth;
            }

            List<InterestMonthReportRecord> dataList = new List<InterestMonthReportRecord>();

            for (DateTime i = startMonth; i <= endMonth; i=i.AddMonths(1))
            {
                if (i == currentMonth)
                {
                    var accountList = await prepaidApplyStatisticsService.PrepaidPartnerAccountListView(f => !string.IsNullOrWhiteSpace(f.PartnerName) && f.IsEnabled && f.PaymentType == (int)PrepaidPaymentEnumType.平台垫付模式);
                    List<PrepaidRepayStatusEnumType> repayStatusEnumTypes = new List<PrepaidRepayStatusEnumType>() { PrepaidRepayStatusEnumType.待还款, PrepaidRepayStatusEnumType.还款中, };

                    var billMonth = currentMonth.AddMonths(-1);
                    var startDate = billMonth;
                    var endDate = currentMonth.AddDays(-1);
                    List<Live_PrepaidInterestReport> reports = new List<Live_PrepaidInterestReport>();
                    List<Live_PrepaidInterestRecord> records = new List<Live_PrepaidInterestRecord>();
                    Live_PrepaidInterestRecord record = null;
                    Live_PrepaidInterestReport report = null;

                    foreach (var account in accountList)
                    {
                        // 当月还款完成记录
                        var exp = Expressionable.Create<Live_V_PrepaidRepaymentRecord>();
                        exp.And(f => f.SendTime < currentMonth);
                        exp.And(f => f.PrepaidPartnerAccountID == account.ID);
                        exp.And(f => f.RepayStatus == PrepaidRepayStatusEnumType.还款完成);
                        exp.And(f => f.RepaymentTime != null && f.RepaymentTime.Value >= startDate);
                        var repayList = await prepaidApplyStatisticsService.PrepaidRepaymentRecorListView(exp.ToExpression());

                        var dt = new DateTime(2025, 4, 27);
                        // 收取利息申请记录信息
                        var applyIdList = repayList.Select(f => f.PrepaidApplyRecordID).Distinct().ToList();
                        var applyExp = Expressionable.Create<Live_V_PrepaidApplyRecord>();

                        applyExp.And(f => f.SigningUnitName != "宁波新莱博金浩供应链管理有限公司");
                        applyExp.And(f => f.PrepaidPartnerAccountID == account.ID);
                        applyExp.And(f => f.Status == PrepaidRecordStatusEnumType.已发放);
                        applyExp.And(f => (f.SendTime == null && f.CreateTime < currentMonth) || f.SendTime.Value < currentMonth);
                        applyExp.And(f => applyIdList.Contains(f.ID) || f.RepayStatus != PrepaidRepayStatusEnumType.还款完成);
                        applyExp.AndIF(account.PartnerName == "金浩人力-俞浩飞", f => f.CreateTime >= dt);
                        applyExp.AndIF(account.PartnerName == "深蓝人力-应重谊", f => f.CreateTime >= dt);
                        // 获取符合条件的申请记录
                        var applyList = await prepaidApplyStatisticsService.PrepaidApplyRecordListView(applyExp.ToExpression());
                        if (applyList != null && applyList.Count > 0)
                        {
                            // 执行利率
                            decimal actualPercent = account.BasePercent + account.BasePercent * account.FloatPercent / 100;
                            foreach (var applyRecord in applyList)
                            {

                                // 获取当前申请记录的还款记录
                                var applyRepaymentList = await prepaidApplyStatisticsService.PrepaidRepaymentRecorListView(f => f.IsSuc && f.RepaymentTime.Value < currentMonth && f.PrepaidApplyRecordID == applyRecord.ID);
                                // 上月已还款的金额
                                var repaymentTotalAmount = applyRepaymentList.Where(f => f.RepaymentTime.Value.Date < startDate).Sum(f => f.TotalAmount);
                                foreach (var repaymentRecord in applyRepaymentList)
                                {
                                    if (repaymentRecord.RepaymentTime.Value.Date < startDate || repaymentRecord.RepaymentTime.Value.Date > endDate)
                                    {
                                        continue;
                                    }
                                    repaymentTotalAmount += repaymentRecord.TotalAmount;
                                    record = new Live_PrepaidInterestRecord()
                                    {
                                        SendDate = applyRecord.SendTime.HasValue ? applyRecord.SendTime : applyRecord.CreateTime,
                                        BillMonth = billMonth,
                                        CreateTime = DateTime.Now,
                                        CustomerGID = applyRecord.CustomerGID,
                                        CustomerName = applyRecord.CustomerName,
                                        StartDate = startDate,
                                        BasePercent = account.BasePercent,
                                        FloatPercent = account.FloatPercent,
                                        ActualPercent = actualPercent,
                                        ApplyAmount = applyRecord.ApplyAmount,
                                        RepaymentAmount = repaymentRecord.TotalAmount,
                                        PartnerGID = account.PartnerGID,
                                        PartnerName = account.PartnerName,
                                        IdCardNo = applyRecord.IdCardNo,
                                        Name = applyRecord.Name,
                                        EndDate = repaymentRecord.RepaymentTime.Value.Date,
                                        PrepaidApplyRecordID = applyRecord.ID,
                                        PrepaidPartnerAccountID = account.ID,
                                        UserEntryID = applyRecord.UserEntryID,
                                        UserID = applyRecord.UserID.Value,
                                        PendingAmount = applyRecord.ApplyAmount - repaymentTotalAmount,
                                        MainAmount = repaymentRecord.TotalAmount,
                                        RepaymentDate = repaymentRecord.RepaymentTime.Value.Date,
                                    };
                                    records.Add(record);
                                }
                                // 账单月未还款完成
                                if (repaymentTotalAmount < applyRecord.ApplyAmount)
                                {



                                    record = new Live_PrepaidInterestRecord()
                                    {
                                        SendDate = applyRecord.SendTime ?? applyRecord.AuditTime ?? applyRecord.CreateTime,
                                        BillMonth = billMonth,
                                        CreateTime = DateTime.Now,
                                        CustomerGID = applyRecord.CustomerGID,
                                        CustomerName = applyRecord.CustomerName,
                                        StartDate = startDate,
                                        BasePercent = account.BasePercent,
                                        FloatPercent = account.FloatPercent,
                                        ActualPercent = actualPercent,
                                        ApplyAmount = applyRecord.ApplyAmount,
                                        RepaymentAmount = repaymentTotalAmount,
                                        PartnerGID = account.PartnerGID,
                                        PartnerName = account.PartnerName,
                                        IdCardNo = applyRecord.IdCardNo,
                                        Name = applyRecord.Name,
                                        EndDate = endDate,
                                        PrepaidApplyRecordID = applyRecord.ID,
                                        PrepaidPartnerAccountID = account.ID,
                                        UserEntryID = applyRecord.UserEntryID,
                                        UserID = applyRecord.UserID.Value,
                                        PendingAmount = applyRecord.ApplyAmount - repaymentTotalAmount,
                                        MainAmount = applyRecord.ApplyAmount - repaymentTotalAmount,
                                        RepaymentDate = null,
                                        Remark = "当月还款未完成"

                                    };



                                    records.Add(record);
                                }
                            }
                        }
                    }

                    foreach (var item in records)
                    {


                        if (item.SendDate != null && item.SendDate > item.StartDate)
                        {
                            item.StartDate = item.SendDate;
                        }



                        if (item.RepaymentDate == null)
                        {
                            if (item.SendDate != null && item.SendDate > item.StartDate)
                            {
                                item.StartDate = item.SendDate;
                            }

                            item.EndDate = endDate;
                        }
                        else
                        {
                            item.EndDate = item.RepaymentDate;
                        }


                        item.Days = item.EndDate.Value.Day - item.StartDate.Value.Day + 1;
                        item.MainInterestAmount = decimal.Round((item.MainAmount * item.Days * item.ActualPercent / (100 * 365)), 2, MidpointRounding.AwayFromZero);
                    }
                    var lastBillMonth = billMonth.AddMonths(-1);
                    foreach (var account in accountList)
                    {
                        // 月统计
                        report = new Live_PrepaidInterestReport()
                        {
                            PartnerGID = account.PartnerGID,
                            PartnerName = account.PartnerName,
                            BasePercent = account.BasePercent,
                            FloatPercent = account.FloatPercent,
                            BillMonth = billMonth,
                            BillYear = billMonth.AddMonths(-3).Year,
                            CreateTime = DateTime.Now,
                            ID = Guid.NewGuid(),
                            InterestMonthAmount = 0,
                            InterestYearAmount = 0,
                            PendingMonthAmount = 0,
                            PendingYearAmount = 0,
                            ApplyMonthAmount = 0,
                            ApplyYearAmount = 0,
                            PrepaidPartnerAccountID = account.ID
                        };
                        // 月申请金额
                        report.ApplyMonthAmount = records.Where(f => f.SendDate >= billMonth && f.PrepaidPartnerAccountID == account.ID).GroupBy(f => new { f.PrepaidApplyRecordID, f.ApplyAmount }).Sum(f => f.Key.ApplyAmount);
                        // 月服务费
                        report.InterestMonthAmount = records.Where(f => f.PrepaidPartnerAccountID == account.ID).Sum(f => f.MainInterestAmount);

                        var lastBill = await prepaidApplyStatisticsService.PrepaidInterestReportGet(f => f.PrepaidPartnerAccountID == account.ID && f.BillMonth == lastBillMonth);

                        if (lastBill != null)
                        {
                            report.ApplyYearAmount = lastBill.ApplyYearAmount + report.ApplyMonthAmount;
                            report.InterestYearAmount = lastBill.InterestYearAmount + report.InterestMonthAmount;
                        }
                        else
                        {
                            report.ApplyYearAmount = report.ApplyMonthAmount.Value;
                            report.InterestYearAmount = report.InterestMonthAmount;
                        }
                        reports.Add(report);
                    }


                    dataList.Add(new InterestMonthReportRecord(i, reports));

                }
                else
                {
                  var monthReportRecords= await prepaidApplyStatisticsService.PrepaidInterestReportList(f => f.BillMonth == i);

                    dataList.Add(new InterestMonthReportRecord(i, monthReportRecords));
                }
            }




        


            return Ok(dataList);
        }

        #endregion





    }
}
