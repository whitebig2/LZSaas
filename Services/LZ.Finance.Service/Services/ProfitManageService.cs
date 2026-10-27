using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Finance.Service.Services
{
    public class ProfitManageService : ServicesBase<Live_ProfitManage>, IProfitManageService, IScopedDependency
    {

        private readonly IUserEntryService _userEntryService;

        public ProfitManageService(IUserEntryService userEntryService)
        {
            _userEntryService = userEntryService;
        }

        public async Task<List<Live_V_ProfitManage>> PageList(Expression<Func<Live_V_ProfitManage, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_ProfitManage>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_ProfitManage>().Where(exp).OrderByDescending(it =>new { it.ProfitMonth,it.PartnerName,it.CreateTime}).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_ProfitManage>> List(Expression<Func<Live_ProfitManage, bool>> exp)
        {
           
                return await   base.Context.Queryable<Live_ProfitManage>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_V_ProfitManage>> ListView(Expression<Func<Live_V_ProfitManage, bool>> exp)
        {

            return await   base.Context.Queryable<Live_V_ProfitManage>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_ProfitManage model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_ProfitManage> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_ProfitManage>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_ProfitManage model)
        {
            return await   base.Context.Updateable<Live_ProfitManage>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
       
        public async Task<Live_ProfitManage> Get(Expression<Func<Live_ProfitManage, bool>> exp)
        {
            return await   base.Context.Queryable<Live_ProfitManage>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_ProfitManage>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        public Task<Live_BusinessInsuranceConfig> GetBusInsuranceConfig(Expression<Func<Model.Live_BusinessInsuranceConfig, bool>> exp = null)
        {
            return   base.Context.Queryable<Live_BusinessInsuranceConfig>().Where(exp).FirstAsync();
        }


        #region 毛利计算

        //public async Task<bool> GenerateProfitRecord()
        //{

        //    var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
        //    var grossMonthEnd = currentMonth.AddDays(-1);
        //    var grossMonth = currentMonth.AddMonths(-1);
        //    var grossLastMonth = grossMonth.AddMonths(-1);
        //    DateTime startDate = grossMonth.AddDays(3);
        //    DateTime endDate = currentMonth.AddDays(3);


        //    var configList = await base.Context.Queryable<Live_V_ProfitConfig>().Where(f => f.IsEnabled).ToListAsync();

          

        //    var existModel = await base.Context.Queryable<Live_ProfitManageRecord>().Where(f => f.ProfitMonth == grossMonth).FirstAsync();

        //    if (existModel != null)
        //    {
        //        throw new ErrorCodeException(-1, "当月毛利已生成过，如需重新生成，请删除后再执行此操作");
        //    }


        //    List<Live_ProfitManageRecord> recordList = new List<Live_ProfitManageRecord>();
        //    Live_ProfitManageRecord record = null;

        //    List<Live_V_ProfitConfig> configs = new List<Live_V_ProfitConfig>();

            

        //    foreach (var config in configList)
        //    {

        //        Console.WriteLine($"-------------------------{config.CustomerName}");
        //        var busInsuranceConfig = await GetBusInsuranceConfig(f => f.IsEnabled && f.CustomerGID == config.CustomerGID && f.IsMain);
        //        if (busInsuranceConfig == null)
        //        {
        //            throw new ErrorCodeException(-1, $"{config.CustomerName} 商保主险配置缺失");
        //        }
        //        var exp = Expressionable.Create<Live_V_UserEntry>();
        //        //exp.And(f => !string.IsNullOrWhiteSpace(f.PlanName1));
        //        exp.And(f => f.PartnerGID.HasValue);

        //        exp.And(f =>
        //       (!string.IsNullOrWhiteSpace(f.PlanName1) && f.PlanStatus1.HasValue && f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保)
        //       || (f.EntryTime.HasValue && f.LeaveTime.HasValue && f.EntryTime.Value.Date == f.LeaveTime.Value.Date && f.Status == 4));
        //        //exp.And(f => (f.PlanStatus1.HasValue && f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保) || ((f.PlanStatus1==null || f.PlanStatus1 == UserEntryPlanStatusEnumType.未参保) && f.EntryTime.HasValue && f.LeaveTime.HasValue && f.EntryTime.Value.Date == f.LeaveTime.Value.Date&&f.Status==4));
        //        exp.And(f => !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName));
        //        exp.And(f => f.EntryTime != null && f.EntryTime.Value < currentMonth);
        //        exp.And(f => f.CustomerGID != null && f.CustomerGID.Value == config.CustomerGID);
        //        exp.And(f => f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= grossMonth));

        //        var entryList = await _userEntryService.ListView(exp.ToExpression());

        //        foreach (var entry in entryList)
        //        {


        //            if (string.IsNullOrWhiteSpace(entry.PlanName1))
        //            {
        //                if (entry.WorkClass == "3类")
        //                {
        //                    entry.PlanName1 = busInsuranceConfig.Plan3;
        //                }
        //                else if (entry.WorkClass == "4类")
        //                {
        //                    entry.PlanName1 = busInsuranceConfig.Plan4;
        //                }
        //                else
        //                {
        //                    entry.PlanName1 = busInsuranceConfig.Plan5;
        //                }
        //            }

        //        }


        //        var records = entryList.Select(f => new Live_ProfitManageRecord
        //        {
        //            Name = f.Name,
        //            IdCardNo = f.CardID,
        //            CustomerName = f.CustomerName,
        //            PartnerNamer = config.PartnerName,// 客户单位服务商
        //            JoinPartnerName = f.JointPartnerName,
        //            WorkClass = f.WorkClass,
        //            WorkName = f.WorkName,
        //            EntryDate = f.EntryTime.Value.Date,
        //            LeaveDate = (f.LeaveTime == null || f.LeaveTime.Value >= currentMonth) ? null : f.LeaveTime.Value,
        //            IsInJob = f.LeaveTime == null || f.LeaveTime.Value >= currentMonth,
        //            BusInsuranceBalanceAmount = 0,
        //            CreateTime = DateTime.Now,
        //            CustomerGID = f.CustomerGID.Value,
        //            PartnerGID = f.PartnerGID.Value,
        //            JoinPartnerGID = f.JointPartnerGID,
        //            GrossMangeID = Guid.Empty,
        //            ProfitMonth = grossMonth,
        //            PlanName1 = f.PlanName1 ?? string.Empty,
        //            PlanName2 = f.PlanName2 ?? string.Empty,
        //            SigningUnitID = f.SigningUnitGID.Value,
        //            SigningUnitName = f.SignName,
        //            StartDate = grossMonth,
        //            EndDate = currentMonth.AddDays(-1),
        //            TaxMonth = grossMonth,
        //            IsRiskAndBusInsurance = true,
        //            FromType = f.FromType ?? string.Empty,
        //            RealPartnerName = f.PartnerName,// 员工信息真实的服务商
        //            Salesman = config.SalesMan,
        //            RealPartnerGID = f.PartnerGID,
        //            CityPartnerID = f.PerCityPartnerID,
        //            CityPartnerTitle = string.Empty,
        //            RetireDate = f.RetireDate,



        //        }).ToList();
        //        // 获取工资
        //        var recordIdCardNoList = new List<string>();

        //        //var salaryRecordList = await _salaryRecordService.ListView(f =>(f.SalaryStatus == SalaryStatusEnumType.SucessSend || f.IsCustomerPaid == true) && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.CustomerGID == config.CustomerGID && f.TaxMonth == grossMonth);
        //        // 改成已打款，根据打款时间来筛选
        //        var salaryRecordList = await base.Context.Queryable<Live_V_SalaryRecord>().Where(f => f.IsCustomerPaid && f.CustomerPaidTime.HasValue && f.CustomerPaidTime.Value >= startDate && f.CustomerPaidTime.Value < endDate && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.CustomerGID == config.CustomerGID).ToListAsync();



        //        foreach (var item in records)
        //        {
        //            if (recordIdCardNoList.Contains(item.IdCardNo))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                recordIdCardNoList.Add(item.IdCardNo);
        //            }
        //            if (salaryRecordList.Any(f => f.IdCardNo == item.IdCardNo))
        //            {
        //                item.UnitSalaryTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item.IdCardNo).Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount);
        //                item.UnitSocialTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item.IdCardNo).Sum(f => f.SocialUnitSumAmount);
        //                item.ServiceFeeTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item.IdCardNo).Sum(f => f.ServiceFeeAmount);
        //                item.IsContainSalary = true;
        //            }
        //        }
        //        var salaryIdCardNoList = salaryRecordList.Select(f => f.IdCardNo).Distinct().ToList();
        //        foreach (var item in salaryIdCardNoList)
        //        {
        //            if (recordIdCardNoList.Contains(item))
        //            {
        //                continue;
        //            }
        //            var salaryRecord = salaryRecordList.First(f => f.IdCardNo == item);

        //            var entry = await _userEntryService.GetView(f => f.CustomerName == salaryRecord.CustomerName && f.CardID == salaryRecord.IdCardNo);


        //            var salaryProfitRecord = new Live_ProfitManageRecord
        //            {
        //                Name = salaryRecord.Name,
        //                IdCardNo = salaryRecord.IdCardNo,
        //                CustomerName = salaryRecord.CustomerName,
        //                PartnerNamer = config.PartnerName,
        //                JoinPartnerName = string.Empty,
        //                WorkClass = string.Empty,
        //                WorkName = string.Empty,
        //                EntryDate = salaryRecord.EntryDate,
        //                LeaveDate = salaryRecord.LeaveDate,
        //                IsInJob = !(salaryRecord.LeaveDate.HasValue && salaryRecord.LeaveDate.Value.Year > 2000),
        //                BusInsuranceBalanceAmount = 0,
        //                CreateTime = DateTime.Now,
        //                CustomerGID = config.CustomerGID,
        //                PartnerGID = config.PartnerGID.Value,
        //                JoinPartnerGID = null,
        //                GrossMangeID = Guid.Empty,
        //                ProfitMonth = grossMonth,
        //                PlanName1 = string.Empty,
        //                PlanName2 = string.Empty,
        //                SigningUnitID = salaryRecord.SigningUnitID.Value,
        //                SigningUnitName = salaryRecord.SigningUnitCompanyName,
        //                StartDate = grossMonth,
        //                EndDate = currentMonth.AddDays(-1),
        //                TaxMonth = grossMonth,
        //                IsRiskAndBusInsurance = false,
        //                IsContainSalary = true,
        //                UnitSalaryTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount),
        //                UnitSocialTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.SocialUnitSumAmount),
        //                ServiceFeeTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.ServiceFeeAmount),
        //                RealPartnerName = config.PartnerName,//  默认获取客户单位的信息
        //                RealPartnerGID = config.PartnerGID,
        //                FromType = string.Empty,
        //                Salesman = config.SalesMan,
        //                CityPartnerID = config.CityPartnerID,
        //                CityPartnerTitle = string.Empty
        //            };

        //            if (entry != null)
        //            {

        //                if (!string.IsNullOrWhiteSpace(entry.PartnerName))
        //                {
        //                    salaryProfitRecord.RealPartnerName = entry.PartnerName;
        //                    salaryProfitRecord.RealPartnerGID = entry.PartnerGID;
        //                }

        //                salaryProfitRecord.CityPartnerID = entry.PerCityPartnerID;

        //                salaryProfitRecord.EntryDate = entry.EntryTime?.Date;
        //                salaryProfitRecord.LeaveDate = entry.LeaveTime?.Date;
        //                if (entry.Status == 2)
        //                {
        //                    salaryProfitRecord.IsInJob = true;
        //                }
        //                else
        //                {
        //                    salaryProfitRecord.IsInJob = false;
        //                }
        //            }


        //            records.Add(salaryProfitRecord);
        //        }
        //        int days = grossMonth.AddMonths(+1).AddDays(-1).Day;




        //        records.AsParallel().ForAll(f => {

                  



        //            if (f.EntryDate < grossMonth)
        //            {
        //                f.IsDayMatch = false;
        //            }


        //            if (f.EntryDate.Value > f.StartDate)
        //            {
        //                f.StartDate = f.EntryDate.Value;
        //            }

        //            if (f.IsContainSalary)
        //            {
        //                if (config.AgentCountType == ProfitAgentCountEnumType.应发工资)
        //                {
        //                    f.AgentPaidAmount = f.UnitSalaryTotalAmount;
        //                }
        //                else if (config.AgentCountType == ProfitAgentCountEnumType.服务费)
        //                {
        //                    f.AgentPaidAmount = f.ServiceFeeTotalAmount;
        //                }
        //                else if (config.AgentCountType == ProfitAgentCountEnumType.应发工资与服务费)
        //                {
        //                    f.AgentPaidAmount = f.UnitSalaryTotalAmount + f.ServiceFeeTotalAmount;
        //                }
        //                else if (config.AgentCountType == ProfitAgentCountEnumType.差额计算)
        //                {
        //                    f.AgentPaidAmount = config.DifferenceBaseAmount;
        //                }
        //                else if (config.AgentCountType == ProfitAgentCountEnumType.应发工资与服务费与企业社保)
        //                {
        //                    f.AgentPaidAmount = f.UnitSalaryTotalAmount + f.ServiceFeeTotalAmount + f.UnitSocialTotalAmount;
        //                }
        //                else
        //                {
        //                    f.AgentPaidAmount = 0;
        //                }
        //            }
        //            //保费
        //            if (f.IsRiskAndBusInsurance)
        //            {

        //                if (!f.IsInJob)
        //                {
        //                    if (f.LeaveDate != null && f.LeaveDate.Value.Date < f.EndDate)
        //                    {
        //                        f.EndDate = f.LeaveDate.Value.Date;
        //                    }
        //                }

        //                var timeSapn = f.EndDate.Value.Subtract(f.StartDate.Value);
        //                f.InJobDays = timeSapn.Days + 1;
        //                if (!string.IsNullOrWhiteSpace(f.PlanName1))
        //                {
        //                    if (f.PlanName1 == "90万+9万" || f.PlanName1 == "80万+8万" || f.PlanName1 == "50万+5万")
        //                    {
        //                        if (f.WorkClass.Equals("5类"))
        //                        {
        //                            f.BusInsuranceCurrentAmount = config.BusInsurance5Amount;
        //                        }
        //                        else
        //                        {
        //                            f.BusInsuranceCurrentAmount = config.BusInsuranceAmount;
        //                        }
        //                    }
        //                    else if (f.PlanName1 == "80万+20万")
        //                    {
        //                        f.BusInsuranceCurrentAmount = 45;
        //                    }
        //                    else if (f.PlanName1 == "30万+3万")
        //                    {
        //                        f.BusInsuranceCurrentAmount = 35;
        //                    }
        //                    else if (f.PlanName1 == "10万+1万")
        //                    {
        //                        f.BusInsuranceCurrentAmount = 10;
        //                    }
        //                    else
        //                    {
        //                        f.BusInsuranceCurrentAmount = config.BusInsurance5Amount;
        //                    }
        //                }
        //                //风险金
        //                f.RiskCurrentAmount = config.RiskAmount;
        //                f.RiskCurrentAmount = (config.RiskAmount * f.InJobDays / days).ToFixed(2);

        //                // 社保补缴风险金
        //                f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * f.InJobDays / days).ToFixed(2);

        //                // 入职月与毛利月相等 新入职的员工
        //                // 入职天数不超过15天 不收取 社保补缴风险金

        //                if (f.EntryDate.Value.ToString("yyyy-MM") == grossMonth.ToString("yyyy-MM"))
        //                {
        //                    if (f.InJobDays <= 15)
        //                    {
        //                        f.SocialRepayRiskAmount = 0;
        //                    }
        //                }
                    


        //                // 判断上个月保费风险金是否已预收
        //                if (f.EntryDate.Value < f.ProfitMonth)
        //                {
        //                    f.BusInsuranceLastAmount = f.BusInsuranceCurrentAmount;
        //                }
        //                // 在职则预收下个月的保费
        //                if (f.IsInJob)
        //                {
        //                    f.BusInsuranceNextAmount = f.BusInsuranceCurrentAmount;
        //                }


        //            }
        //            else
        //            {
        //                f.InJobDays = 0;
        //            }


        //            f.AgentPaidTaxAmount = (f.AgentPaidAmount * config.AgentPaidPercent / 100).ToFixed(2);
        //            f.BusinessAmount = (f.ServiceFeeTotalAmount * config.BusinessPercent / 100).ToFixed(2);


        //            if (f.PartnerNamer != f.RealPartnerName)
        //            {
        //                f.JoinManageAmount = (f.ServiceFeeTotalAmount * config.JoinManagePercent / 100).ToFixed(2);
        //            }
        //            else
        //            {
        //                f.JoinManageAmount = 0;
        //            }

        //            f.CustomerThingAmount = (f.ServiceFeeTotalAmount * config.CustomerThingPercent / 100).ToFixed(2);





        //            if (f.LeaveDate != null && f.EntryDate == f.LeaveDate)
        //            {
        //                f.BusInsuranceCurrentAmount = 0;
        //            }
        //        });
        //        recordList.AddRange(records);
        //    }



        //    // 商保的增减替换
        //    var inJobList = recordList.OrderBy(f => f.EntryDate).Where(f => f.EntryDate != null && (f.LeaveDate == null || f.LeaveDate != f.EntryDate) && f.EntryDate.Value >= grossMonth && f.IsRiskAndBusInsurance).ToList();
        //    // 匹配当天离职的
        //    var leaveList = recordList.OrderBy(f => f.EntryDate).Where(f => !f.IsInJob && f.IsRiskAndBusInsurance && f.LeaveDate != null && f.LeaveDate.Value >= grossMonth && f.EntryDate != f.LeaveDate).ToList();
        //    Live_ProfitManageRecord leaveRecord = null;
          

        //    #region 新增减替换规则
        //    List<string> samePlans = new List<string>() { "90万+9万", "80万+8万" };

        //    foreach (var inJobItem in inJobList)
        //    {
        //        leaveRecord = null;

        //        //  "90万+9万" 有限同方案同类别匹配，否则 同方案匹配
        //        if (samePlans.Contains(inJobItem.PlanName1))
        //        {
        //            // 有限匹配同类别匹配
        //            leaveRecord = leaveList.FirstOrDefault(f => f.IsDayMatch == false && f.RealPartnerName == inJobItem.RealPartnerName && f.LeaveDate == inJobItem.EntryDate && f.IsDelMatchSuc == false && samePlans.Contains(inJobItem.PlanName1) && f.WorkClass == inJobItem.WorkClass);
        //        }
        //        if (leaveRecord == null)
        //        {
        //            leaveRecord = leaveList.FirstOrDefault(f => f.IsDayMatch == false && f.RealPartnerName == inJobItem.RealPartnerName && f.LeaveDate == inJobItem.EntryDate && f.IsDelMatchSuc == false && samePlans.Contains(inJobItem.PlanName1));
        //        }
        //        if (leaveRecord != null)
        //        {
        //            leaveRecord.IsDelMatchSuc = true;
        //            inJobItem.IsAddMatchSuc = true;
        //            inJobItem.BusInsuranceBalanceAmount = leaveRecord.BusInsuranceCurrentAmount * -1;
        //            inJobItem.IsDayMatch = false;
        //        }
        //        else
        //        {
        //            inJobItem.IsDayMatch = true;
        //            inJobItem.IsAddMatchSuc = false;
        //            if (inJobItem.WorkClass == "4类" || inJobItem.WorkClass == "3类")
        //            {
        //                inJobItem.BusInsuranceBalanceAmount = 2.50m * inJobItem.InJobDays;
        //            }
        //            else if (inJobItem.WorkClass == "5类")
        //            {
        //                inJobItem.BusInsuranceBalanceAmount = 4.00m * inJobItem.InJobDays;
        //            }
        //            else
        //            {
        //                inJobItem.BusInsuranceBalanceAmount = 0;
        //            }
        //            inJobItem.BusInsuranceCurrentAmount = 0;

        //        }

        //    }



        //    #endregion

        //    List<string> disabilityNameList = new List<string>();

        //    recordList.AsParallel().ForAll(f =>
        //    {


        //        // 同一合伙人 同一签章单位 有工资 则收取一次
        //        if (!disabilityNameList.Contains($"{f.PartnerNamer}-{f.SigningUnitName}-{f.IdCardNo}"))
        //        {
        //            if (f.UnitSalaryTotalAmount > 0)
        //            {
        //                f.DisabilityAmount = 3;
        //                disabilityNameList.Add($"{f.PartnerNamer}-{f.SigningUnitName}-{f.IdCardNo}");
        //            }
        //        }

        //        if (f.IsRiskAndBusInsurance)
        //        {
        //            // 当月保费-上月预收-增减退回+次月预收
        //            f.BusInsuranceSumAmount = f.BusInsuranceCurrentAmount - f.BusInsuranceLastAmount + f.BusInsuranceBalanceAmount + f.BusInsuranceNextAmount;
        //        }

        //    });



  
        // return    await base.Context.Insertable<Live_ProfitManageRecord>(recordList).ExecuteCommandAsync()>0;
        //}


        public async Task<bool> GenerateProfitRecord()
        {

            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            var grossMonthEnd = currentMonth.AddDays(-1);

            var grossLastMonth = grossMonth.AddMonths(-1);
            var grossLastMonthEnd = grossMonth.AddDays(-1);
            DateTime startDate = grossMonth.AddDays(3);
            DateTime endDate = currentMonth.AddDays(3);

            //var gg = Guid.Parse("F96C70A1-8D08-48D9-8C96-756656111042");

            var configList = await base.Context.Queryable<Live_V_ProfitConfig>().Where(f => f.IsEnabled).ToListAsync();


            var existModel = await base.Context.Queryable<Live_ProfitManageRecord>().Where(f => f.ProfitMonth == grossMonth).FirstAsync();

            if (existModel != null)
            {
                throw new ErrorCodeException(-1, "当月毛利已生成过，如需重新生成，请删除后再执行此操作");
            }


            List<Live_ProfitManageRecord> recordList = new List<Live_ProfitManageRecord>();
            Live_ProfitManageRecord record = null;

            List<Live_V_ProfitConfig> configs = new List<Live_V_ProfitConfig>();

            // 当前月天数
            int lastMonthDays = grossMonth.AddDays(-1).Day;
            // 当前月天数
            int monthDays = grossMonth.AddMonths(+1).AddDays(-1).Day;
            // 次月天数
            int nextMonthDays = grossMonth.AddMonths(+2).AddDays(-1).Day;


            foreach (var config in configList)
            {

                Console.WriteLine($"-------------------------{config.CustomerName}");
                var exp = Expressionable.Create<Live_V_UserEntry>();
                exp.And(f => f.PartnerGID.HasValue);
                exp.And(f =>
               //( f.PlanStatus1.HasValue && f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保)
               ( f.InsuranceType==null|| f.InsuranceType!= 0)|| (f.EntryTime.HasValue && f.LeaveTime.HasValue && f.EntryTime.Value.Date == f.LeaveTime.Value.Date && f.Status == 4));
                exp.And(f => !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName));
                exp.And(f => f.EntryTime != null && f.EntryTime.Value < currentMonth);
                exp.And(f => f.CustomerGID != null && f.CustomerGID.Value == config.CustomerGID);
                exp.And(f => f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= grossMonth));

                var entryList = await _userEntryService.ListView(exp.ToExpression());
                var records = entryList.Select(f => new Live_ProfitManageRecord
                {
                    Name = f.Name,
                    IdCardNo = f.CardID,
                    CustomerName = f.CustomerName,
                    PartnerNamer = config.PartnerName,// 客户单位服务商
                    JoinPartnerName = f.JointPartnerName,
                    WorkClass = f.WorkClass,
                    WorkName = f.WorkName,
                    EntryDate = f.EntryTime.Value.Date,
                    LeaveDate = (f.LeaveTime == null || f.LeaveTime.Value >= currentMonth) ? null : f.LeaveTime.Value,
                    IsInJob = f.LeaveTime == null || f.LeaveTime.Value >= currentMonth,
                    BusInsuranceBalanceAmount = 0,
                    CreateTime = DateTime.Now,
                    CustomerGID = f.CustomerGID.Value,
                    PartnerGID = f.PartnerGID.Value,
                    JoinPartnerGID = f.JointPartnerGID,
                    GrossMangeID = Guid.Empty,
                    ProfitMonth = grossMonth,
                    PlanName1 = f.PlanName1 ?? string.Empty,
                    PlanName2 = f.PlanName2 ?? string.Empty,
                    SigningUnitID = f.SigningUnitGID.Value,
                    SigningUnitName = f.SignName,
                    StartDate = grossMonth,
                    EndDate = currentMonth.AddDays(-1),
                    TaxMonth = grossMonth,
                    IsRiskAndBusInsurance = true,
                    FromType = f.FromType ?? string.Empty,
                    RealPartnerName = f.PartnerName,// 员工信息真实的服务商
                    Salesman = config.SalesMan,
                    RealPartnerGID = f.PartnerGID,
                    CityPartnerID = f.PerCityPartnerID,
                    CityPartnerTitle = string.Empty,
                    RetireDate=f.RetireDate,
                }).ToList();
                // 获取工资
                var recordIdCardNoList = new List<string>();

                //var salaryRecordList = await _salaryRecordService.ListView(f =>(f.SalaryStatus == SalaryStatusEnumType.SucessSend || f.IsCustomerPaid == true) && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.CustomerGID == config.CustomerGID && f.TaxMonth == grossMonth);
                // 改成已打款，根据打款时间来筛选
                var salaryRecordList = await base.Context.Queryable<Live_V_SalaryRecord>().Where(f => f.IsCustomerPaid && f.CustomerPaidTime.HasValue && f.CustomerPaidTime.Value >= startDate && f.CustomerPaidTime.Value < endDate && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.CustomerGID == config.CustomerGID).ToListAsync();



                foreach (var item in records)
                {
                    if (recordIdCardNoList.Contains(item.IdCardNo))
                    {
                        continue;
                    }
                    else
                    {
                        recordIdCardNoList.Add(item.IdCardNo);
                    }
                    if (salaryRecordList.Any(f => f.IdCardNo == item.IdCardNo))
                    {
                        item.UnitSalaryTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item.IdCardNo).Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount);
                        item.UnitSocialTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item.IdCardNo).Sum(f => f.SocialUnitSumAmount);
                        item.ServiceFeeTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item.IdCardNo).Sum(f => f.ServiceFeeAmount);
                        item.IsContainSalary = true;
                    }
                }
                var salaryIdCardNoList = salaryRecordList.Select(f => f.IdCardNo).Distinct().ToList();
                foreach (var item in salaryIdCardNoList)
                {
                    if (recordIdCardNoList.Contains(item))
                    {
                        continue;
                    }
                    var salaryRecord = salaryRecordList.First(f => f.IdCardNo == item);

                    var entry = await _userEntryService.GetView(f => f.CustomerName == salaryRecord.CustomerName && f.CardID == salaryRecord.IdCardNo&&f.EntryTime<currentMonth);


                    var salaryProfitRecord = new Live_ProfitManageRecord
                    {
                        Name = salaryRecord.Name,
                        IdCardNo = salaryRecord.IdCardNo,
                        CustomerName = salaryRecord.CustomerName,
                        PartnerNamer = config.PartnerName,
                        JoinPartnerName = string.Empty,
                        WorkClass = string.Empty,
                        WorkName = string.Empty,
                        EntryDate = salaryRecord.EntryDate,
                        LeaveDate = salaryRecord.LeaveDate,
                        IsInJob = !(salaryRecord.LeaveDate.HasValue && salaryRecord.LeaveDate.Value.Year > 2000),
                        BusInsuranceBalanceAmount = 0,
                        CreateTime = DateTime.Now,
                        CustomerGID = config.CustomerGID,
                        PartnerGID = config.PartnerGID.Value,
                        JoinPartnerGID = null,
                        GrossMangeID = Guid.Empty,
                        ProfitMonth = grossMonth,
                        PlanName1 = string.Empty,
                        PlanName2 = string.Empty,
                        SigningUnitID = config.SigningUnitGID.Value,
                        SigningUnitName = config.SigningUnitName,
                        StartDate = grossMonth,
                        EndDate = currentMonth.AddDays(-1),
                        TaxMonth = grossMonth,
                        IsRiskAndBusInsurance = false,
                        IsContainSalary = true,
                        UnitSalaryTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount),
                        UnitSocialTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.SocialUnitSumAmount),
                        ServiceFeeTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.ServiceFeeAmount),
                        RealPartnerName = config.PartnerName,//  默认获取客户单位的信息
                        RealPartnerGID = config.PartnerGID,
                        FromType = string.Empty,
                        Salesman = config.SalesMan,
                        CityPartnerID = config.CityPartnerID,
                        CityPartnerTitle = string.Empty
                    };

                    if (entry != null)
                    {

                        if (!string.IsNullOrWhiteSpace(entry.PartnerName))
                        {
                            salaryProfitRecord.RealPartnerName = entry.PartnerName;
                            salaryProfitRecord.RealPartnerGID = entry.PartnerGID;
                        }
                        // 城市合伙人
                        salaryProfitRecord.CityPartnerID = entry.PerCityPartnerID;

                        salaryProfitRecord.EntryDate = entry.EntryTime?.Date;
                        salaryProfitRecord.LeaveDate = entry.LeaveTime?.Date;
                        if (entry.Status == 2)
                        {
                            salaryProfitRecord.IsInJob = true;
                        }
                        else
                        {
                            salaryProfitRecord.IsInJob = false;
                        }
                    }


                    records.Add(salaryProfitRecord);
                }
             




                records.AsParallel().ForAll(f => {

                 var ageAndGender=   IdCardHelper.GetAgeAndGender(f.IdCardNo, grossMonthEnd);


                    f.Age = ageAndGender.Age;
                    f.Sex = ageAndGender.Gender;
                    if (f.EntryDate < grossMonth)
                    {
                        f.IsDayMatch = false;
                    }


                    if (f.EntryDate.Value > f.StartDate)
                    {
                        f.StartDate = f.EntryDate.Value;
                    }

                    if (f.IsContainSalary)
                    {
                        if (config.AgentCountType == ProfitAgentCountEnumType.应发工资)
                        {
                            f.AgentPaidAmount = f.UnitSalaryTotalAmount;
                        }
                        else if (config.AgentCountType == ProfitAgentCountEnumType.服务费)
                        {
                            f.AgentPaidAmount = f.ServiceFeeTotalAmount;
                        }
                        else if (config.AgentCountType == ProfitAgentCountEnumType.应发工资与服务费)
                        {
                            f.AgentPaidAmount = f.UnitSalaryTotalAmount + f.ServiceFeeTotalAmount;
                        }
                        else if (config.AgentCountType == ProfitAgentCountEnumType.差额计算)
                        {
                            f.AgentPaidAmount = config.DifferenceBaseAmount;
                        }
                        else if (config.AgentCountType == ProfitAgentCountEnumType.应发工资与服务费与企业社保)
                        {
                            f.AgentPaidAmount = f.UnitSalaryTotalAmount + f.ServiceFeeTotalAmount + f.UnitSocialTotalAmount;
                        }
                        else
                        {
                            f.AgentPaidAmount = 0;
                        }
                    }
                    //保费
                    if (f.IsRiskAndBusInsurance)
                    {

                        if (!f.IsInJob)
                        {
                            if (f.LeaveDate != null && f.LeaveDate.Value.Date < f.EndDate)
                            {
                                f.EndDate = f.LeaveDate.Value.Date;
                            }
                        }

                        var timeSapn = f.EndDate.Value.Subtract(f.StartDate.Value);
                        f.InJobDays = timeSapn.Days + 1;

                        #region 商保费用
                        if (f.WorkClass == "5类")
                        {
                            f.BusInsuranceCurrentAmount = config.BusInsurance5Amount * f.InJobDays;
                        }
                        else
                        {
                            f.BusInsuranceCurrentAmount = config.BusInsuranceAmount * f.InJobDays;
                        }

                        // 判断上个月保费风险金是否已预收
                        if (f.EntryDate.Value < f.ProfitMonth)
                        {
                            if (f.WorkClass == "5类")
                            {
                                f.BusInsuranceLastAmount = config.BusInsurance5Amount * lastMonthDays;
                                //f.BusInsuranceLastAmount =85;
                            }
                            else
                            {
                                f.BusInsuranceLastAmount = config.BusInsuranceAmount * lastMonthDays;
                                //f.BusInsuranceLastAmount = 55;
                            }
                        }
                        // 在职则预收下个月的保费
                        if (f.IsInJob)
                        {
                            if (f.WorkClass == "5类")
                            {
                                f.BusInsuranceNextAmount = config.BusInsurance5Amount * nextMonthDays;
                            }
                            else
                            {
                                f.BusInsuranceNextAmount = config.BusInsuranceAmount * nextMonthDays;
                            }
                        }
                        // 预收保费+当月实际保费-上月已预缴保费
                        f.BusInsuranceSumAmount =f.BusInsuranceNextAmount+f.BusInsuranceCurrentAmount-f.BusInsuranceLastAmount;

                        #endregion

                        //风险金
                        f.RiskCurrentAmount = config.RiskAmount;
                        f.RiskCurrentAmount = (config.RiskAmount * f.InJobDays / monthDays).ToFixed(2);


                        //疾病身故险
                        f.DeathRiskCurrentAmount = config.DeathRiskAmount;
                        f.DeathRiskCurrentAmount = (config.DeathRiskAmount * f.InJobDays / monthDays).ToFixed(2);
                        //f.DeathRiskCurrentAmount = 0;

                        #region 社保补缴风险金


                        // 入职月与毛利月相等 新入职的员工
                        // 入职天数不超过15天 不收取 社保补缴风险金
                        // 入职不超过15天不收取
                        if (f.EntryDate.Value.ToString("yyyy-MM") == grossMonth.ToString("yyyy-MM"))
                        {
                            if (f.InJobDays <= 15)
                            {
                                f.SocialRepayRiskAmount = 0;
                            }
                            else
                            {
                                // 入职月+6个月大于毛利月则收取60%
                                if (config.SocialRepayRiskBaseAmount == 192 && f.EntryDate.Value.AddDays(1 - f.EntryDate.Value.Day).AddMonths(6) >= grossMonth)
                                {
                                    f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * 0.6m * f.InJobDays / monthDays).ToFixed(2);
                                }
                                else
                                {
                                    // 社保补缴风险金
                                    f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * f.InJobDays / monthDays).ToFixed(2);
                                }

                            }

                        }
                        else if (f.EntryDate.Value.ToString("yyyy-MM") == grossLastMonth.ToString("yyyy-MM"))
                        {


                            // 获取入职月的在职时间，超过15天 则四月不再收取，否则一起收取
                            var entryMonthDays = Convert.ToInt32(grossLastMonthEnd.Subtract(f.EntryDate.Value).TotalDays + 1);
                            if (entryMonthDays > 15)
                            {
                                // 入职月+6个月大于毛利月则收取60%
                                if (config.SocialRepayRiskBaseAmount == 192 && f.EntryDate.Value.AddDays(1 - f.EntryDate.Value.Day).AddMonths(6) >= grossMonth)
                                {
                                    f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * 0.6m * f.InJobDays / monthDays).ToFixed(2);
                                }
                                else
                                {
                                    // 社保补缴风险金
                                    f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * f.InJobDays / monthDays).ToFixed(2);
                                }
                            }
                            else {

                                // 在职总天数
                                var allDays = Convert.ToInt32(f.EndDate.Value.Subtract(f.EntryDate.Value).TotalDays + 1);
                                if (allDays <= 15)
                                {
                                    f.SocialRepayRiskAmount = 0;
                                }
                                else
                                {
                                    // 入职月+6个月大于毛利月则收取60%
                                    if (config.SocialRepayRiskBaseAmount == 192 && f.EntryDate.Value.AddDays(1 - f.EntryDate.Value.Day).AddMonths(6) >= grossMonth)
                                    {
                                        f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * 0.6m * allDays / monthDays).ToFixed(2);
                                    }
                                    else
                                    {
                                        // 社保补缴风险金
                                        f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * allDays / monthDays).ToFixed(2);
                                    }
                                }


                            }
                        }
                        else
                        {
                            // 入职月+6个月大于毛利月则收取60%
                            if (config.SocialRepayRiskBaseAmount == 192 && f.EntryDate.Value.AddDays(1 - f.EntryDate.Value.Day).AddMonths(6) >= grossMonth)
                            {
                                f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * 0.6m * f.InJobDays / monthDays).ToFixed(2);
                            }
                            else
                            {
                                // 社保补缴风险金
                                f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * f.InJobDays / monthDays).ToFixed(2);
                            }
                        }
                      

                        if (f.Age >= 18)
                        {
                            if ((f.Sex == "男" && f.Age >= 60) || f.Sex == "女" && f.Age >= 50)
                            {
                                f.SocialRepayRiskAmount = 0;
                                // 已退休
                                f.IsRetire=true;
                            }
                        }
                        else {
                            f.SocialRepayRiskAmount = 0;
                        }
                        #endregion
                    }
                    else
                    {
                        f.InJobDays = 0;
                        f.Msg = "";
                    }
                    f.AgentPaidTaxAmount = (f.AgentPaidAmount * config.AgentPaidPercent / 100).ToFixed(2);
                    f.BusinessAmount = (f.ServiceFeeTotalAmount * config.BusinessPercent / 100).ToFixed(2);


                    if (f.PartnerNamer != f.RealPartnerName)
                    {
                        f.JoinManageAmount = (f.ServiceFeeTotalAmount * config.JoinManagePercent / 100).ToFixed(2);
                    }
                    else
                    {
                        f.JoinManageAmount = 0;
                    }

                    f.CustomerThingAmount = (f.ServiceFeeTotalAmount * config.CustomerThingPercent / 100).ToFixed(2);
                    if (f.LeaveDate != null && f.EntryDate == f.LeaveDate)
                    {
                        f.BusInsuranceCurrentAmount = 0;
                    }
                });
                recordList.AddRange(records);
            }
            List<string> disabilityNameList = new List<string>();
            recordList.AsParallel().ForAll(f =>
            {
                // 同一合伙人 同一签章单位 有工资 则收取一次
                if (!disabilityNameList.Contains($"{f.PartnerNamer}-{f.SigningUnitName}-{f.IdCardNo}"))
                {
                    if (f.UnitSalaryTotalAmount > 0)
                    {
                        f.DisabilityAmount = 3;
                        disabilityNameList.Add($"{f.PartnerNamer}-{f.SigningUnitName}-{f.IdCardNo}");
                    }
                }
                if (f.IsRiskAndBusInsurance)
                {
                    // 当月保费-上月预收-增减退回+次月预收
                    f.BusInsuranceSumAmount = f.BusInsuranceCurrentAmount - f.BusInsuranceLastAmount + f.BusInsuranceBalanceAmount + f.BusInsuranceNextAmount;
                }
            });


            var groups = recordList.GroupBy(f => f.CustomerGID).ToList();

            foreach (var group in groups)
            {
                var cus = await _userEntryService.GetCustomer(f => f.CustomerGID == group.Key);
                foreach (var item in group)
                {
                    item.CustomerBusinessModel = cus.BusinessModel;
                }
            }



            return await base.Context.Insertable<Live_ProfitManageRecord>(recordList).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 根据毛利记录生成当月毛利管会送数据
        /// </summary>
        /// <param name="grossMonth"></param>
        /// <returns></returns>
        public async Task<List<Live_ProfitManage>> CreateByRecords(DateTime grossMonth)
        {
            //var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            //var grossMonth = currentMonth.AddMonths(-1);

            grossMonth = grossMonth.AddDays(1 - grossMonth.Day);

            var records = await base.Context.Queryable<Live_ProfitManageRecord>()
            .Where(f => f.RealPartnerGID != null && !string.IsNullOrWhiteSpace(f.RealPartnerName) && f.ProfitMonth == grossMonth).ToListAsync();

            var groups = records.GroupBy(f => new {  f.RealPartnerGID, f.RealPartnerName, f.SigningUnitName }).ToList();

            
            List<Live_ProfitManage> recordList = new List<Live_ProfitManage>();
            foreach (var f in groups)
            {
                var model = new Live_ProfitManage()
                {
                    CreateTime = DateTime.Now,
                    PartnerGID = f.Key.RealPartnerGID.Value,
                    PartnerName = f.Key.RealPartnerName,
                    SigningUnitID = Guid.Empty,
                    SigningUnitName = f.Key.SigningUnitName,
                    SigningUnitBankNo = string.Empty,
                    SigningUnitAddress = string.Empty,
                    SigningUnitNo = string.Empty,
                    SigningUnitBank = string.Empty,
                    SigningUnitPhone = string.Empty,
                    FileName = string.Empty,
                    FilePath = string.Empty,
                    TaxMonth = grossMonth,
                    ProfitMonth = grossMonth,
                    CityPartnerID=Guid.Empty
                };

                recordList.Add(model);
            }
            var existList = await base.Context.Queryable<Live_ProfitManage>().Where(f => f.ProfitMonth == grossMonth).ToListAsync();
            var addList = new List<Live_ProfitManage>();

            foreach (var item in recordList)
            {
                if (existList.Any(f => f.PartnerName == item.PartnerName && f.SigningUnitName == item.SigningUnitName))
                {
                    continue;
                }
                else
                {
                    addList.Add(item);
                }
            }
            int num = 0;
            var dt = DateTime.Now;
            foreach (var item in addList)
            {
                num++;
                item.CreateTime = dt.AddMicroseconds(num++);
            }

            if (addList.Any())
            {
                await base.Context.Insertable<Live_ProfitManage>(addList).ExecuteCommandAsync();
            }

            return addList;

        }

        /// <summary>
        /// 毛利计算
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Live_ProfitManage> CalculateProfit(Guid id)
        {

            var model = await Get(f => f.ID == id);
            var recordList = await base.Context.Queryable<Live_ProfitManageRecord>().Where(f => f.ProfitMonth == model.ProfitMonth && f.SigningUnitName == model.SigningUnitName && f.RealPartnerName == model.PartnerName).ToListAsync();
            model.RecordCount = recordList.Count();
            // 服务费记录条数
            model.ServiceFeeRecordCount = recordList.Where(f => f.ServiceFeeTotalAmount > 0).Count();
            // 在职人数
            model.InJobRecordCount = recordList.Where(f => f.IsInJob).Count();
            // 企业应发工资
            model.UnitSalaryTotalAmount = recordList.Sum(f => f.UnitSalaryTotalAmount);
            // 服务费
            model.ServiceFeeTotalAmount = recordList.Sum(f => f.ServiceFeeTotalAmount);
            //  社保企业部分
            model.UnitSocialTotalAmount = recordList.Sum(f => f.UnitSocialTotalAmount);
            // 工伤风险金
            model.RiskTotalAmount = recordList.Sum(f => f.RiskCurrentAmount);
            //model.BusInsuranceBalanceTotalAmount = recordList.Sum(f => f.BusInsuranceCurrentAmount + f.BusInsuranceNextAmount - f.BusInsuranceLastAmount);
           // 上月已预收保费
            model.BusInsuranceLastTotalAmount = recordList.Sum(f => f.BusInsuranceLastAmount);
            // 预收次月保费
            model.BusInsuranceNextTotalAmount = recordList.Sum(f => f.BusInsuranceNextAmount);
           // 保费实收
            model.BusInsuranceSumAmount = recordList.Sum(f => f.BusInsuranceCurrentAmount - f.BusInsuranceLastAmount + f.BusInsuranceNextAmount);
            // 代缴费合计金额
            model.AgentPaidTotalAmount = recordList.Sum(f => f.AgentPaidAmount);
            //代缴税费
            model.AgentPaidTaxAmount = recordList.Sum(f => f.AgentPaidTaxAmount);
            // 残保金
            model.DisabilityTotalAmount = recordList.Sum(f => f.DisabilityAmount);
            // 新业务开发费
            model.BusinessTotalAmount = recordList.Sum(f => f.BusinessAmount);
            // 客情费
            model.CustomerThingTotalAmount = recordList.Sum(f => f.CustomerThingAmount);
            // 营驻场管理费
            model.JoinManageTotalAmount = recordList.Sum(f => f.JoinManageAmount);
            // 营驻场管理费
            model.JoinManagePaidAmount = recordList.Sum(f => f.JoinManageAmount);
            // 社保补缴风险金
            model.SocialRepayRiskAmount = recordList.Sum(f => f.SocialRepayRiskAmount);
            model.DeathRiskTotalAmount = recordList.Sum(f => f.DeathRiskCurrentAmount);



            // 真实服务商== 客户单位服务商
            var joinManageRecordList = await base.Context.Queryable<Live_ProfitManageRecord>().Where(f => f.ProfitMonth == model.ProfitMonth && f.SigningUnitName == model.SigningUnitName && f.PartnerNamer == model.PartnerName).ToListAsync();
            if (joinManageRecordList.Any())
            {
                model.JoinManageIncomeAmount = joinManageRecordList.Sum(f => f.JoinManageAmount);
            }
            // 获取技术金融服务费
            var interestReport = await base.Context.Queryable<Live_PrepaidInterestReport>().Where(f => f.PartnerName == model.PartnerName && f.BillMonth == model.ProfitMonth).FirstAsync();
            if (interestReport != null)
            {
                var modelExist = await Get(f => f.ProfitMonth == model.ProfitMonth && f.ID != id && f.PartnerName == model.PartnerName && f.InterestTotalAmount > 0);
                if (modelExist == null)
                {
                    model.InterestTotalAmount = interestReport.InterestMonthAmount;
                }
                else
                {
                    model.InterestTotalAmount = 0;
                }
            }
            //获取服务商固定顶费用
            var partnerConfit = await base.Context.Queryable<Live_V_ProfitPartnerConfig>().Where(f => f.PartnerName == model.PartnerName).FirstAsync();
            //var partnerConfit = await _profitPartnerConfigService.GetView(f => f.PartnerName == model.PartnerName);
            if (partnerConfit != null)
            {
                var modelExist = await Get(f => f.ProfitMonth == model.ProfitMonth && f.ID != id && f.PartnerName == model.PartnerName && (f.SubsidyAmount > 0 || f.DeductionAmount > 0));
                if (modelExist == null)
                {
                    model.PartnerGID = partnerConfit.PartnerGID;
                    model.SubsidyAmount = partnerConfit.SubsidyAmount;
                    model.DeductionAmount = partnerConfit.DeductionAmount;
                    model.SubsidyRemark = partnerConfit.SubsidyRemark;
                    model.DeductionRemark = partnerConfit.DeductionRemark;
                }
                else
                {
                    model.PartnerGID = partnerConfit.PartnerGID;
                    model.SubsidyAmount = 0;
                    model.DeductionAmount = 0;
                    model.SubsidyRemark = string.Empty;
                    model.DeductionRemark = string.Empty;
                }

            }


            if (model.InterestTotalAmount == null)
            {
                model.InterestTotalAmount = 0;
            }




            model.ProfitAmount =
                  // 服务费
                 model.ServiceFeeTotalAmount -
                 // 代缴税费
                 model.AgentPaidTaxAmount -
                 // 营驻场管理费
                 model.JoinManagePaidAmount -
                 // 客情费
                 model.CustomerThingTotalAmount -
                 // 保费实收合计
                 model.BusInsuranceSumAmount -
                 // 风险金合计
                 model.RiskTotalAmount -
                 // 新业务开发费
                 model.BusinessTotalAmount -
                 // 残障金
                 model.DisabilityTotalAmount +
                 // 补贴
                 model.SubsidyAmount -
                 // 扣除
                 model.DeductionAmount +
                 //  主场管理收入
                 model.JoinManageIncomeAmount -
                 // 社保补缴风险金
                 model.SocialRepayRiskAmount -
                 // 技术金融服务费
                 model.InterestTotalAmount
                 // 疾病身故险
                 - model.DeathRiskTotalAmount
             ;
            // 驻厂收入
            await Update(model);

            return model;

        }


        #endregion


    }
}
