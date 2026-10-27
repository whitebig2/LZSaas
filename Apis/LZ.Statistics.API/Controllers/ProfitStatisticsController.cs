using LZ.Model;
using LZ.Model.Enums;
using LZ.Statistics.Service.Interface;
using LZ.Statistics.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record;
using NPOI.OpenXmlFormats;
using NPOI.SS.Formula.Functions;
using SqlSugar;

namespace LZ.Statistics.API.Controllers
{
    [AllowAnonymous]
    public class ProfitStatisticsController : StatisticsControllerBase
    {

        private readonly IProfitStatisticService _profitStatisticService;

        public ProfitStatisticsController(IProfitStatisticService profitStatisticService)
        {
            _profitStatisticService = profitStatisticService;
        }

        [HttpGet]
        public async Task<IActionResult> ProfitMange()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            var configList = await _profitStatisticService.ListProfitConfigView(f => f.IsEnabled);
            var configGroup = configList.GroupBy(f => new { f.PartnerGID, f.PartnerName, f.SigningUnitGID, f.SigningUnitName }).ToList();
            var recordList = configGroup.Select(f => new Live_ProfitManage()
            {
                CreateTime = DateTime.Now,
                PartnerGID = f.Key.PartnerGID.Value,
                PartnerName = f.Key.PartnerName,
                SigningUnitID = f.Key.SigningUnitGID.Value,
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
            }).ToList();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> GenerateProfitRecord()
        {
            var  configList=await _profitStatisticService.ListProfitConfigView(f => f.IsEnabled);
            Guid partnerGID = Guid.Empty;
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            List<Live_ProfitManageRecord> recordList = new List<Live_ProfitManageRecord>();
            Live_ProfitManageRecord record = null;

            List<Live_V_ProfitConfig> configs = new List<Live_V_ProfitConfig>();
            foreach (var config in configList)
            {
                var busInsuranceConfig=await _profitStatisticService.GetBusInsuranceConfig(f => f.IsEnabled&&f.CustomerGID==config.CustomerGID&&f.IsMain);

               
                var exp = Expressionable.Create<Live_V_UserEntry>();
                exp.And(f =>f.InsuranceStatus!= UserEntryInsuranceStatusEnumType.未参保); 
                exp.And(f => !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName));
                exp.And(f => f.EntryTime != null && f.EntryTime.Value < currentMonth);
                exp.And(f =>f.CustomerGID!=null&&f.CustomerGID.Value==config.CustomerGID);
                exp.And(f => f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= grossMonth));
                var entryList = await _profitStatisticService.ListEntryView(exp.ToExpression());
                foreach (var entry in entryList) {

                    if (entry.WorkClass == "3类")
                    {
                        entry.PlanName1 = busInsuranceConfig.Plan3;
                    }
                    else if (entry.WorkClass == "4类")
                    {
                        entry.PlanName1 = busInsuranceConfig.Plan4;
                    }
                    else
                    {
                        entry.PlanName1 = busInsuranceConfig.Plan5;
                    }
                }
                var records = entryList.Select(f => new Live_ProfitManageRecord
                {
                    Name = f.Name,
                    IdCardNo = f.CardID,
                    CustomerName = f.CustomerName,
                    PartnerNamer = f.PartnerName,
                    JoinPartnerName = f.JointPartnerName,
                    WorkClass = f.WorkClass,
                    WorkName = f.WorkName,
                    EntryDate = f.EntryTime.Value.Date,
                    LeaveDate = (f.LeaveTime == null || f.LeaveTime.Value >= currentMonth)?null : f.LeaveTime.Value,
                    IsInJob = f.LeaveTime==null||f.LeaveTime.Value>=currentMonth,
                    BusInsuranceBalanceAmount = 0,
                    CreateTime = DateTime.Now,
                    CustomerGID = f.CustomerGID.Value,
                    PartnerGID = f.PartnerGID.Value,
                    JoinPartnerGID = f.JointPartnerGID,
                    GrossMangeID = Guid.Empty,
                    ProfitMonth = grossMonth,
                    PlanName1 =f.PlanName1??string.Empty,
                    PlanName2 = f.PlanName2??string.Empty,
                    SigningUnitID = f.SigningUnitGID.Value,
                    SigningUnitName = f.SignName,
                    StartDate = grossMonth,
                    EndDate = currentMonth.AddDays(-1),
                    TaxMonth = grossMonth,
                    IsRiskAndBusInsurance = true,
                     FromType= f.FromType ?? string.Empty,
                     RealPartnerName=f.JointPartnerName??f.PartnerName
                }).ToList();
                // 获取工资
                var recordIdCardNoList = new List<string>();
                var salaryRecordList = await _profitStatisticService.ListSalaryRecordView(f => f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.UnitSalaryAmount > 0 && f.CustomerGID == config.CustomerGID && f.TaxMonth == grossMonth);
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
                        item.UnitSalaryTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item.IdCardNo).Sum(f => f.UnitSalaryAmount-f.UnitDeductionAmount);
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
                    records.Add(new Live_ProfitManageRecord
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
                        IsInJob = salaryRecord.LeaveDate.HasValue && salaryRecord.LeaveDate.Value.Year > 2000,
                        BusInsuranceBalanceAmount = 0,
                        CreateTime = DateTime.Now,
                        CustomerGID = config.CustomerGID,
                        PartnerGID = config.PartnerGID.Value,
                        JoinPartnerGID = null,
                        GrossMangeID = Guid.Empty,
                        ProfitMonth = grossMonth,
                        PlanName1 = string.Empty,
                        PlanName2 = string.Empty,
                        SigningUnitID = salaryRecord.SigningUnitID.Value,
                        SigningUnitName = salaryRecord.SigningUnitCompanyName,
                        StartDate = grossMonth,
                        EndDate = currentMonth.AddDays(-1),
                        TaxMonth = grossMonth,
                        IsRiskAndBusInsurance = false,
                        IsContainSalary = true,
                        UnitSalaryTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.UnitSalaryAmount),
                        UnitSocialTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.SocialUnitSumAmount),
                        ServiceFeeTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.ServiceFeeAmount),
                        RealPartnerName= config.PartnerName,
                         FromType=string.Empty
                    });
                }
                int days=grossMonth.AddMonths(+1).AddDays(-1).Day;

                records.AsParallel().ForAll(f => {


                    if (f.EntryDate.Value > f.StartDate)
                    {
                        f.StartDate = f.EntryDate.Value;
                    }
                    if (!f.IsInJob)
                    {
                        if (f.LeaveDate != null && f.LeaveDate.Value.Date < f.EndDate)
                        {
                            f.EndDate = f.LeaveDate.Value.Date;
                        }
                    }

                    var timeSapn=f.EndDate.Value.Subtract(f.StartDate.Value);

                

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
                            f.AgentPaidAmount = f.UnitSalaryTotalAmount+ f.ServiceFeeTotalAmount;
                        }
                        else if (config.AgentCountType == ProfitAgentCountEnumType.差额计算)
                        {
                            f.AgentPaidAmount = config.DifferenceBaseAmount;
                        }
                        else
                        {
                            f.AgentPaidAmount = 0;
                        }
                    }
                    //保费
                    if (f.IsRiskAndBusInsurance)
                    {

                        f.InJobDays = timeSapn.Days + 1;
                        if (!string.IsNullOrWhiteSpace(f.PlanName1))
                        {
                            if (f.PlanName1 == "90万+9万")
                            {
                                if (f.WorkClass.Equals("5类"))
                                {
                                    f.BusInsuranceCurrentAmount = config.BusInsurance5Amount;
                                }
                                else
                                {
                                    f.BusInsuranceCurrentAmount = config.BusInsuranceAmount;
                                }
                            }
                            else if (f.PlanName1 == "80万+20万")
                            {
                                f.BusInsuranceCurrentAmount = 45;
                            }
                            else if (f.PlanName1 == "30万+3万")
                            {
                                f.BusInsuranceCurrentAmount = 35;
                            }
                            else if (f.PlanName1 == "10万+1万")
                            {
                                f.BusInsuranceCurrentAmount = 10;
                            }
                            else
                            {
                                f.BusInsuranceCurrentAmount = config.BusInsurance5Amount;
                            }
                        }
                        //风险金
                        f.RiskCurrentAmount = config.RiskAmount;
                        f.RiskCurrentAmount = (config.RiskAmount * f.InJobDays / days).ToFixed(2);
                        // 判断上个月保费风险金是否已预收
                        if (f.EntryDate.Value < f.ProfitMonth)
                        {
                            f.BusInsuranceLastAmount = f.BusInsuranceCurrentAmount;
                        }
                        // 在职则预收下个月的保费
                        if (f.IsInJob)
                        {
                            f.BusInsuranceNextAmount = f.BusInsuranceCurrentAmount;
                        }

                    
                    }
                    else {
                        f.InJobDays = 0;
                    }
                   

                    f.AgentPaidTaxAmount = (f.AgentPaidAmount * config.AgentPaidPercent / 100).ToFixed(2);
                    f.BusinessAmount = (f.ServiceFeeTotalAmount * config.BusinessPercent / 100).ToFixed(2);
                    f.JoinManageAmount = (f.ServiceFeeTotalAmount * config.JoinManagePercent / 100).ToFixed(2);
                    f.CustomerThingAmount = (f.ServiceFeeTotalAmount * config.CustomerThingPercent / 100).ToFixed(2);
                });
                recordList.AddRange(records);
            }

           

            // 商保的增减替换
            var inJobList = recordList.Where(f => f.EntryDate!=null&&f.EntryDate.Value>=grossMonth&&f.IsRiskAndBusInsurance).ToList();
            // 匹配当天离职的
            var leaveList = recordList.Where(f =>!f.IsInJob&&f.IsRiskAndBusInsurance&&f.LeaveDate!=null&&f.LeaveDate.Value>=grossMonth).ToList();
            Live_ProfitManageRecord leaveRecord = null;
            foreach (var inJobItem in inJobList)
            {

                //  "90万+9万" 有限同方案同类别匹配，否则 同方案匹配
                if (inJobItem.PlanName1 == "90万+9万")
                {
                    // 有限匹配同类别匹配
                    leaveRecord = leaveList.FirstOrDefault(f =>f.RealPartnerName==inJobItem.RealPartnerName&& f.LeaveDate == inJobItem.EndDate && f.IsDelMatchSuc == false && f.PlanName1 == inJobItem.PlanName1 && f.WorkClass == inJobItem.WorkClass);
                }
                if (leaveRecord == null)
                {
                    leaveRecord = leaveList.FirstOrDefault(f => f.RealPartnerName == inJobItem.RealPartnerName && f.LeaveDate == inJobItem.EndDate && f.IsDelMatchSuc == false && f.PlanName1 == inJobItem.PlanName1);
                }
                if (leaveRecord != null)
                {
                    leaveRecord.IsDelMatchSuc = true;
                    inJobItem.IsAddMatchSuc = true;
                    inJobItem.BusInsuranceBalanceAmount = leaveRecord.BusInsuranceCurrentAmount * -1;
                }
                else
                {
                    inJobItem.BusInsuranceBalanceAmount = 0;
                }
            }

            List<string> disabilityNameList = new List<string>();

            recordList.AsParallel().ForAll(f =>
            {
                // 同一合伙人 同一签章单位 有工资 则收取一次
                if (!disabilityNameList.Contains($"{f.PartnerNamer}-{f.SigningUnitName}-{f.IdCardNo}"))
                {
                    disabilityNameList.Add($"{f.PartnerNamer}-{f.SigningUnitName}-{f.IdCardNo}");
                    if (f.UnitSalaryTotalAmount > 0)
                    {
                        f.DisabilityAmount = 3;
                    }
                }

                if (f.IsRiskAndBusInsurance)
                {
                    // 当月保费-上月预收-增减退回+次月预收
                    f.BusInsuranceSumAmount = f.BusInsuranceCurrentAmount - f.BusInsuranceLastAmount + f.BusInsuranceBalanceAmount + f.BusInsuranceNextAmount;
                }

            });


                await  _profitStatisticService.AddProfitManageRecord(recordList);
            return Ok();
        }








    }
}
