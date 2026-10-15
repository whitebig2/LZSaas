using LZ.Clients.API;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.IExcelTemplate.profit;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.HPSF;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [AllowAnonymous]
    public class ProfitManageController : FinanceControllerBase
    {
        private readonly IFinanceOtherService    _otherService;
        private readonly IAuthService   _authService;
        private readonly IUserEntryService  _userEntryService;
        private readonly IProfitManageService _manageService;
        private readonly IProfitConfigService _configService;
        private readonly IProfitManageRecordService _recordService;
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly IProfitPartnerConfigService _profitPartnerConfigService;
        private readonly IPrepaidInterestReportService  _prepaidInterestReportService;

        public ProfitManageController(IProfitManageService manageService, IProfitConfigService configService, IProfitManageRecordService recordService, IUserEntryService userEntryService, ISalaryRecordService salaryRecordService, IAuthService authService, IFinanceOtherService otherService, IProfitPartnerConfigService profitPartnerConfigService, IPrepaidInterestReportService prepaidInterestReportService)
        {
            _manageService = manageService;
            _configService = configService;
            _recordService = recordService;
            _userEntryService = userEntryService;
            _salaryRecordService = salaryRecordService;
            _authService = authService;
            _otherService = otherService;
            _profitPartnerConfigService = profitPartnerConfigService;
            _prepaidInterestReportService = prepaidInterestReportService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string partnerName,DateTime? profitMonth, [FromQuery] PagedModelBase query)
        {
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            var exp = Expressionable.Create<Model.Live_V_ProfitManage>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }

            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(profitMonth!=null, f => f.ProfitMonth== profitMonth.Value);
            var models = await _manageService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }



        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string partnerName, DateTime? profitMonth)
        {
            var exp = Expressionable.Create<Model.Live_V_ProfitManage>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }

            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(profitMonth != null, f => f.ProfitMonth == profitMonth.Value);
            var models = await _manageService.ListView(exp.ToExpression());
        
           
            var exportDataList = models.Select(model => new ProfitManageExcelDto()
            {
                InJobRecordCount = model.InJobRecordCount,
                ProfitMonth = model.ProfitMonth.Value.ToString("yyyy-MM-dd"),
                AgentPaidTaxTotalAmount = model.AgentPaidTaxAmount,
                AgentPaidTotalAmount = model.AgentPaidTotalAmount,
                BusinessTotalAmount = model.BusinessTotalAmount,
                BusInsuranceSumAmount = model.BusInsuranceSumAmount,
                DeductionRemark = string.Empty,
                DeductionTotalAmount = model.DeductionAmount,
                DisabilityTotalAmount = model.DisabilityTotalAmount,
                FromType = string.Empty,
                PartnerName = model.PartnerName,
                ProfitTotalAmount = model.ProfitAmount.Value,
                RiskTotalAmount = model.RiskTotalAmount,
                ServiceFeeRecordCount = model.ServiceFeeRecordCount,
                ServiceFeeTotalAmount = model.ServiceFeeTotalAmount,
                SigningUnitName = model.SigningUnitName,
                SubsidyRemark = string.Empty,
                SubsidyTotalAmount = model.SubsidyAmount,
                TaxMonth = model.TaxMonth.Value.ToString("yyyy-MM-dd"),
                UnitSalaryTotalAmount = model.UnitSalaryTotalAmount,
                UnitSocialTotalAmount = model.UnitSocialTotalAmount,
                CustomerThingTotalAmount = model.CustomerThingTotalAmount.Value,
                JoinManageIncomeAmount = model.JoinManageIncomeAmount,
                JoinManagePaidAmount = model.JoinManagePaidAmount,
                SocialRepayRiskAmount = model.SocialRepayRiskAmount,

            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "企业毛利总数据");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "企业毛利总数据.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }




        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_ProfitManage model)
        {
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await _manageService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_ProfitManage> Get(Guid id)
        {
            var model = await _manageService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_ProfitManage model)
        {

            if (model.InterestTotalAmount==null)
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

     

            await _manageService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _manageService.Delete(id);
            return Ok();
        }


        #region 毛利计算
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateEmpty()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            await _manageService.CreateByRecords(grossMonth);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ProfitCalculate([FromForm] Guid id)
        {
            await _manageService.CalculateProfit(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> GenerateProfitRecord()
        {
            await _manageService.GenerateProfitRecord();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> MatchCityPartner()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            var records = await _manageService.List(f => f.ProfitMonth == grossMonth);
            foreach (var record in records)
            {
                //if (record.CityPartnerID == null)
                //{
                    var partnerRes = await _otherService.GetPartnerByName(record.PartnerName);
                    if (partnerRes != null && partnerRes.Data != null)
                    {
                        record.CityPartnerID = partnerRes.Data.CityPartnerID;
                    }
                    await _manageService.Update(record);
                //}
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AgentPaidTest()
        {

            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            var configList = await _configService.ListView(f => f.IsEnabled);

            foreach (var item in configList)
            {
              var records=await  _recordService.List(f => f.ProfitMonth == grossMonth && f.CustomerGID == item.CustomerGID );
                if (records.Any())
                {
                    foreach (var record in records)
                    {
                        record.Salesman = item.SalesMan;
                        record.CityPartnerID = item.CityPartnerID;
                    }
                    await _recordService.Update(records);
                }
               
            }

            return Ok();
        }
        #endregion



        [HttpPut]
        public async Task<IActionResult> ProfitExport([FromForm] Guid id)
        {
            var model = await _manageService.Get(f => f.ID == id);
            var recordList = await _recordService.List(f => f.ProfitMonth == model.ProfitMonth && f.RealPartnerName == model.PartnerName);
            var busInsuranceBalanceList = recordList.Where(f => f.IsDayMatch == false && f.IsRiskAndBusInsurance && f.LeaveDate != f.EntryDate).ToList();
            var startDate = model.ProfitMonth.Value;
            var endDate = model.ProfitMonth.Value.AddMonths(1);
            List<ProfitBusInsuranceChangeExcelDto> data3 = new List<ProfitBusInsuranceChangeExcelDto>();

            while (startDate< endDate)
            {
                ProfitBusInsuranceChangeExcelDto changeExcelDto = new ProfitBusInsuranceChangeExcelDto()
                {
                    ApplyDate = startDate.ToString("yyyy-MM-dd"),
                    AddCount = busInsuranceBalanceList.Count(f => f.EntryDate != null && f.EntryDate.Value == startDate),
                    DelCount = busInsuranceBalanceList.Count(f => f.LeaveDate != null && f.LeaveDate.Value == startDate),
                    ChangeAmount = busInsuranceBalanceList.Where(f => f.EntryDate != null && f.EntryDate.Value == startDate).Sum(f => f.BusInsuranceCurrentAmount + f.BusInsuranceBalanceAmount-f.BusInsuranceLastAmount)
                };
                data3.Add(changeExcelDto);
                startDate = startDate.AddDays(1);
            }
            data3.Add(new ProfitBusInsuranceChangeExcelDto()
            {
                ApplyDate = "",
                AddCount = data3.Sum(f => f.AddCount),
                DelCount = data3.Sum(f => f.DelCount),
                ChangeAmount = data3.Sum(f => f.ChangeAmount)
            });




            var data4 = recordList.Where(f => f.IsDayMatch).Select(f => new ProfitManageDayMathRecordExcelDto()
            {

                BusInsuranceChangeAmount = f.BusInsuranceCurrentAmount + f.BusInsuranceBalanceAmount - f.BusInsuranceLastAmount,
                EndDate = f.EndDate.Value.ToString("yyyy-MM-dd"),
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                FromType = f.FromType,
                IdCardNo = f.IdCardNo,
                InJobDays = f.InJobDays,
                IsInJob = f.IsInJob ? "是" : "否",
                CustomerName = f.CustomerName,
                Name = f.Name,
                PartnerNamer = f.PartnerNamer,
                PlanName1 = f.PlanName1,
                ServiceFeeTotalAmount = f.ServiceFeeTotalAmount,
                SigningUnitName = f.SigningUnitName,
                StartDate = f.StartDate.Value.ToString("yyyy-MM-dd"),
                UnitSalaryTotalAmount = f.UnitSalaryTotalAmount,
                UnitSocialTotalAmount = f.UnitSocialTotalAmount,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                IsDayMatch = f.IsDayMatch ? "是" : "否",
            }).ToList();
            data4.Add(new ProfitManageDayMathRecordExcelDto()
            {

                EndDate = "",
                EntryDate = "",
                LeaveDate = "",
                FromType = "",
                IdCardNo = "",
                InJobDays = data4.Sum(f => f.InJobDays),
                IsInJob = "",
                CustomerName = "合计",
                Name = "",
                PartnerNamer = "",
                PlanName1 = "",
                ServiceFeeTotalAmount = data4.Sum(f => f.ServiceFeeTotalAmount),
                SigningUnitName = "",
                StartDate = "",
                UnitSalaryTotalAmount = data4.Sum(f => f.UnitSalaryTotalAmount),
                UnitSocialTotalAmount = data4.Sum(f => f.UnitSocialTotalAmount),
                WorkClass = "",
                WorkName = "",
                BusInsuranceChangeAmount = data4.Sum(f => f.BusInsuranceChangeAmount),
            });

            DateTime oldMonth = new DateTime(2016, 04,01);

            if (model.ProfitMonth >= oldMonth)
            {
                data3 = new List<ProfitBusInsuranceChangeExcelDto>();
                data4 = new List<ProfitManageDayMathRecordExcelDto>();
            }
            var data2 = recordList.Select(f => new ProfitManageRecordExcelDto()
            {

                AgentPaidAmount = f.AgentPaidAmount,
                AgentPaidTaxAmount = f.AgentPaidTaxAmount,
                BusinessAmount = f.BusinessAmount,
                //BusInsuranceBalanceAmount = f.BusInsuranceBalanceAmount,
                //BusInsuranceCurrentAmount = f.BusInsuranceCurrentAmount,
                BusInsuranceNextAmount = f.BusInsuranceNextAmount,
                BusInsuranceSumAmount = f.BusInsuranceSumAmount,
                BusInsuranceChangeAmount = f.BusInsuranceCurrentAmount + f.BusInsuranceBalanceAmount - f.BusInsuranceLastAmount,
                BusInsuranceCurrentAmount = f.BusInsuranceCurrentAmount,
                BusInsuranceLastAmount = f.BusInsuranceLastAmount,
                Days = f.Days,
                DisabilityAmount = f.DisabilityAmount,
                EndDate = f.EndDate.Value.ToString("yyyy-MM-dd"),
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                FromType = f.FromType,
                IdCardNo = f.IdCardNo,
                InJobDays = f.InJobDays,
                IsInJob = f.IsInJob ? "是" : "否",
                CustomerName = f.CustomerName,
                Name = f.Name,
                PartnerNamer = f.PartnerNamer,
                PlanName1 = f.PlanName1,
                ProfitMonth = f.ProfitMonth.Value.ToString("yyyy-MM-dd"),
                JoinManageAmount = f.JoinManageAmount,
                RiskCurrentAmount = f.RiskCurrentAmount,
                DeathRiskCurrentAmount = f.DeathRiskCurrentAmount,
                ServiceFeeTotalAmount = f.ServiceFeeTotalAmount,
                SigningUnitName = f.SigningUnitName,
                StartDate = f.StartDate.Value.ToString("yyyy-MM-dd"),
                TaxMonth = f.TaxMonth.Value.ToString("yyyy-MM-dd"),
                UnitSalaryTotalAmount = f.UnitSalaryTotalAmount,
                UnitSocialTotalAmount = f.UnitSocialTotalAmount,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                CustomerThingAmount = f.CustomerThingAmount,
                IsDayMatch = f.IsDayMatch ? "是" : "否",
                SocialRepayRiskAmount = f.SocialRepayRiskAmount,
                
                //毛利 =服务费- 联营驻场管理费（支）-客情费- 保费实收- 工伤风险金- 新业务开发费- 残保金 - 代缴税费-疾病身故险
                ProfitAmount = f.ServiceFeeTotalAmount - f.JoinManageAmount - f.CustomerThingAmount - f.BusInsuranceSumAmount - f.RiskCurrentAmount - f.BusinessAmount - f.DisabilityAmount - f.AgentPaidTaxAmount-f.SocialRepayRiskAmount-f.DeathRiskCurrentAmount
            }).ToList();
            data2.Add(new ProfitManageRecordExcelDto()
            {

                AgentPaidAmount = data2.Sum(f => f.AgentPaidAmount),
                AgentPaidTaxAmount = data2.Sum(f => f.AgentPaidTaxAmount),
                BusinessAmount = data2.Sum(f => f.BusinessAmount),
                //BusInsuranceBalanceAmount = f.BusInsuranceBalanceAmount,
                //BusInsuranceCurrentAmount = f.BusInsuranceCurrentAmount,
                BusInsuranceLastAmount = data2.Sum(f => f.BusInsuranceLastAmount),
                BusInsuranceCurrentAmount = data2.Sum(f => f.BusInsuranceCurrentAmount),
                BusInsuranceNextAmount = data2.Sum(f => f.BusInsuranceNextAmount),
                BusInsuranceSumAmount = data2.Sum(f => f.BusInsuranceSumAmount),
                BusInsuranceChangeAmount = data2.Sum(f => f.BusInsuranceChangeAmount),
                SocialRepayRiskAmount = data2.Sum(f => f.SocialRepayRiskAmount),
                DeathRiskCurrentAmount = data2.Sum(f => f.DeathRiskCurrentAmount),
                Days = data2.Sum(f => f.Days),
                DisabilityAmount = data2.Sum(f => f.DisabilityAmount),
                EndDate = "",
                EntryDate = "",
                LeaveDate = "",
                FromType = "",
                IdCardNo = "",
                InJobDays = data2.Sum(f => f.InJobDays),
                IsInJob = "",
                CustomerName = "合计",
                Name = "",
                PartnerNamer = "",
                PlanName1 = "",
                ProfitMonth = "",
                JoinManageAmount = data2.Sum(f => f.JoinManageAmount),
                RiskCurrentAmount = data2.Sum(f => f.RiskCurrentAmount),
                ServiceFeeTotalAmount = data2.Sum(f => f.ServiceFeeTotalAmount),
                SigningUnitName = "",
                StartDate = "",
                TaxMonth = "",
                UnitSalaryTotalAmount = data2.Sum(f => f.UnitSalaryTotalAmount),
                UnitSocialTotalAmount = data2.Sum(f => f.UnitSocialTotalAmount),
                WorkClass = "",
                WorkName = "",
                CustomerThingAmount = data2.Sum(f => f.CustomerThingAmount),
                ProfitAmount = data2.Sum(f => f.ProfitAmount),
                
            });

            ProfitManageExcelDto manageExcelDto = new ProfitManageExcelDto()
            {
                InJobRecordCount = model.InJobRecordCount,
                ProfitMonth = model.ProfitMonth.Value.ToString("yyyy-MM-dd"),
                AgentPaidTaxTotalAmount = model.AgentPaidTaxAmount,
                AgentPaidTotalAmount = model.AgentPaidTotalAmount,
                BusinessTotalAmount = model.BusinessTotalAmount,
                BusInsuranceSumAmount = model.BusInsuranceSumAmount,
                DeductionRemark = string.Empty,
                DeductionTotalAmount = model.DeductionAmount,
                DisabilityTotalAmount = model.DisabilityTotalAmount,
                FromType = string.Empty,
                PartnerName = model.PartnerName,
                ProfitTotalAmount = model.ProfitAmount.Value,
                RiskTotalAmount = model.RiskTotalAmount,
                DeathRiskTotalAmount = model.DeathRiskTotalAmount,
                ServiceFeeRecordCount = model.ServiceFeeRecordCount,
                ServiceFeeTotalAmount = model.ServiceFeeTotalAmount,
                SigningUnitName = model.SigningUnitName,
                SubsidyRemark = string.Empty,
                SubsidyTotalAmount = model.SubsidyAmount,
                TaxMonth = model.TaxMonth.Value.ToString("yyyy-MM-dd"),
                UnitSalaryTotalAmount = model.UnitSalaryTotalAmount,
                UnitSocialTotalAmount = model.UnitSocialTotalAmount,
                CustomerThingTotalAmount = model.CustomerThingTotalAmount.Value,
                JoinManageIncomeAmount = model.JoinManageIncomeAmount,
                JoinManagePaidAmount = model.JoinManagePaidAmount,
                 SocialRepayRiskAmount = model.SocialRepayRiskAmount,
                 

            };
            var data1 = new List<ProfitManageExcelDto>() { manageExcelDto };
            var fileBytes = NpoiExcelHelper.WriteExcel<ProfitManageExcelDto, ProfitManageRecordExcelDto,ProfitBusInsuranceChangeExcelDto , ProfitManageDayMathRecordExcelDto> (data1, "企业毛利总数据", data2, "企业毛利详细", data3, "商保增减",data4, "按天保费");
           
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "毛利统计.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

      


        [HttpPost]
        public async Task<IActionResult> DownloadAllRecord()
        {
            var configList = await _configService.ListView(f => f.IsEnabled);

            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);

            //var existModel = await _recordService.Get(f => f.ProfitMonth == grossMonth);

            //if (existModel != null)
            //{
            //    throw new ErrorCodeException(-1, "当月毛利已生成过，如需重新生成，请删除后再执行此操作");
            //}


            List<Live_ProfitManageRecord> recordList = new List<Live_ProfitManageRecord>();
            Live_ProfitManageRecord record = null;

            List<Live_V_ProfitConfig> configs = new List<Live_V_ProfitConfig>();

            DateTime startDate = grossMonth.AddDays(5);
            DateTime endDate = currentMonth.AddDays(5);

            foreach (var config in configList)
            {

                Console.WriteLine($"-------------------------{config.CustomerName}");
                var busInsuranceConfig = await _manageService.GetBusInsuranceConfig(f => f.IsEnabled && f.CustomerGID == config.CustomerGID && f.IsMain);
                if (busInsuranceConfig == null)
                {
                    throw new ErrorCodeException(-1, $"{config.CustomerName} 商保主险配置缺失");
                }
                var exp = Expressionable.Create<Live_V_UserEntry>();
                exp.And(f => !string.IsNullOrWhiteSpace(f.PlanName1));
                exp.And(f => f.PartnerGID.HasValue);
                exp.And(f => 
                (f.PlanStatus1.HasValue && f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保) 
                || (f.EntryTime.HasValue && f.LeaveTime.HasValue && f.EntryTime.Value.Date == f.LeaveTime.Value.Date && f.Status == 4));
                exp.And(f => !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName));
                exp.And(f => f.EntryTime != null && f.EntryTime.Value < currentMonth);
                exp.And(f => f.CustomerGID != null && f.CustomerGID.Value == config.CustomerGID);
                exp.And(f => f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= grossMonth));

                var entryList = await _userEntryService.ListView(exp.ToExpression());

                //foreach (var entry in entryList)
                //{

                //    if (entry.WorkClass == "3类")
                //    {
                //        entry.PlanName1 = busInsuranceConfig.Plan3;
                //    }
                //    else if (entry.WorkClass == "4类")
                //    {
                //        entry.PlanName1 = busInsuranceConfig.Plan4;
                //    }
                //    else
                //    {
                //        entry.PlanName1 = busInsuranceConfig.Plan5;
                //    }
                //}


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
                    RealPartnerName = f.PartnerName// 员工信息真实的服务商
                }).ToList();
                // 获取工资
                var recordIdCardNoList = new List<string>();





                //var salaryRecordList = await _salaryRecordService.ListView(f =>(f.SalaryStatus == SalaryStatusEnumType.SucessSend || f.IsCustomerPaid == true) && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.CustomerGID == config.CustomerGID && f.TaxMonth == grossMonth);
                // 改成已打款，根据打款时间来筛选
                var salaryRecordList = await _salaryRecordService.ListView(f => f.SalaryStatus == SalaryStatusEnumType.SucessSend && f.SalarySendTime.HasValue && f.SalarySendTime >= startDate && f.SalarySendTime < endDate && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.CustomerGID == config.CustomerGID);



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

                    var entry = await _userEntryService.GetView(f => f.CustomerName == salaryRecord.CustomerName && f.CardID == salaryRecord.IdCardNo);


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
                        SigningUnitID = salaryRecord.SigningUnitID.Value,
                        SigningUnitName = salaryRecord.SigningUnitCompanyName,
                        StartDate = grossMonth,
                        EndDate = currentMonth.AddDays(-1),
                        TaxMonth = grossMonth,
                        IsRiskAndBusInsurance = false,
                        IsContainSalary = true,
                        UnitSalaryTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount),
                        UnitSocialTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.SocialUnitSumAmount),
                        ServiceFeeTotalAmount = salaryRecordList.Where(f => f.IdCardNo == item).Sum(f => f.ServiceFeeAmount),
                        RealPartnerName = config.PartnerName,
                        FromType = string.Empty
                    };

                    if (entry != null)
                    {

                        if (!string.IsNullOrWhiteSpace(entry.PartnerName))
                        {
                            salaryProfitRecord.RealPartnerName = entry.PartnerName;
                        }
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
                int days = grossMonth.AddMonths(+1).AddDays(-1).Day;




                records.AsParallel().ForAll(f => {


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
                        if (!string.IsNullOrWhiteSpace(f.PlanName1))
                        {
                            if (f.PlanName1 == "90万+9万" || f.PlanName1 == "80万+8万")
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
                        // 社保补缴风险金
                        f.SocialRepayRiskAmount = (config.SocialRepayRiskBaseAmount * f.InJobDays / days).ToFixed(2);

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
                    else
                    {
                        f.InJobDays = 0;
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



            // 商保的增减替换
            var inJobList = recordList.OrderBy(f => f.EntryDate).Where(f => f.EntryDate != null && (f.LeaveDate == null || f.LeaveDate != f.EntryDate) && f.EntryDate.Value >= grossMonth && f.IsRiskAndBusInsurance).ToList();
            // 匹配当天离职的
            var leaveList = recordList.OrderBy(f => f.EntryDate).Where(f => !f.IsInJob && f.IsRiskAndBusInsurance && f.LeaveDate != null && f.LeaveDate.Value >= grossMonth && f.EntryDate != f.LeaveDate).ToList();
            Live_ProfitManageRecord leaveRecord = null;
            //foreach (var inJobItem in inJobList)
            //{

            //    //  "90万+9万" 有限同方案同类别匹配，否则 同方案匹配
            //    if (inJobItem.PlanName1 == "90万+9万")
            //    {
            //        // 有限匹配同类别匹配
            //        leaveRecord = leaveList.FirstOrDefault(f => f.RealPartnerName == inJobItem.RealPartnerName && f.LeaveDate == inJobItem.EntryDate && f.IsDelMatchSuc == false && f.PlanName1 == inJobItem.PlanName1 && f.WorkClass == inJobItem.WorkClass);
            //    }
            //    if (leaveRecord == null)
            //    {
            //        leaveRecord = leaveList.FirstOrDefault(f => f.RealPartnerName == inJobItem.RealPartnerName && f.LeaveDate == inJobItem.EntryDate && f.IsDelMatchSuc == false && f.PlanName1 == inJobItem.PlanName1);
            //    }
            //    if (leaveRecord != null)
            //    {
            //        leaveRecord.IsDelMatchSuc = true;
            //        inJobItem.IsAddMatchSuc = true;
            //        inJobItem.BusInsuranceBalanceAmount = leaveRecord.BusInsuranceCurrentAmount * -1;
            //    }
            //    else
            //    {
            //        inJobItem.BusInsuranceBalanceAmount = 0;
            //    }
            //}

            #region 新增减替换规则
            List<string> samePlans = new List<string>() { "90万+9万", "80万+8万" };

            foreach (var inJobItem in inJobList)
            {
                leaveRecord = null;

                //  "90万+9万" 有限同方案同类别匹配，否则 同方案匹配
                if (samePlans.Contains(inJobItem.PlanName1))
                {
                    // 有限匹配同类别匹配
                    leaveRecord = leaveList.FirstOrDefault(f => f.IsDayMatch == false && f.RealPartnerName == inJobItem.RealPartnerName && f.LeaveDate == inJobItem.EntryDate && f.IsDelMatchSuc == false && samePlans.Contains(inJobItem.PlanName1) && f.WorkClass == inJobItem.WorkClass);
                }
                if (leaveRecord == null)
                {
                    leaveRecord = leaveList.FirstOrDefault(f => f.IsDayMatch == false && f.RealPartnerName == inJobItem.RealPartnerName && f.LeaveDate == inJobItem.EntryDate && f.IsDelMatchSuc == false && samePlans.Contains(inJobItem.PlanName1));
                }
                if (leaveRecord != null)
                {
                    leaveRecord.IsDelMatchSuc = true;
                    inJobItem.IsAddMatchSuc = true;
                    inJobItem.BusInsuranceBalanceAmount = leaveRecord.BusInsuranceCurrentAmount * -1;
                    inJobItem.IsDayMatch = false;
                }
                else
                {
                    inJobItem.IsDayMatch = true;
                    inJobItem.IsAddMatchSuc = false;
                    if (inJobItem.WorkClass == "4类" || inJobItem.WorkClass == "3类")
                    {
                        inJobItem.BusInsuranceBalanceAmount = 2.50m * inJobItem.InJobDays;
                    }
                    else if (inJobItem.WorkClass == "5类")
                    {
                        inJobItem.BusInsuranceBalanceAmount = 4.00m * inJobItem.InJobDays;
                    }
                    else
                    {
                        inJobItem.BusInsuranceBalanceAmount = 0;
                    }
                    inJobItem.BusInsuranceCurrentAmount = 0;

                }

            }



            #endregion

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



            if (recordList.Any(f => string.IsNullOrWhiteSpace(f.RealPartnerName)))
            {
                foreach (var item in recordList.Where(f => string.IsNullOrWhiteSpace(f.RealPartnerName)))
                {
                    Console.WriteLine($"{item.CustomerName}--{item.Name}--{item.IdCardNo}");
                }
            }


            if (recordList.Any(f => string.IsNullOrWhiteSpace(f.PartnerNamer)))
            {
                foreach (var item in recordList.Where(f => string.IsNullOrWhiteSpace(f.PartnerNamer)))
                {
                    Console.WriteLine($"{item.CustomerName}--{item.Name}--{item.IdCardNo}");
                }
            }
            //await _recordService.Add(recordList);
            // 下载
            var data2 = recordList.Select(f => new ProfitManageRecordExcelDto()
            {

                AgentPaidAmount = f.AgentPaidAmount,
                AgentPaidTaxAmount = f.AgentPaidTaxAmount,
                BusinessAmount = f.BusinessAmount,
                //BusInsuranceBalanceAmount = f.BusInsuranceBalanceAmount,
                //BusInsuranceCurrentAmount = f.BusInsuranceCurrentAmount,
                BusInsuranceNextAmount = f.BusInsuranceNextAmount,
                BusInsuranceSumAmount = f.BusInsuranceSumAmount,
                BusInsuranceChangeAmount = f.BusInsuranceCurrentAmount + f.BusInsuranceBalanceAmount - f.BusInsuranceLastAmount,
                Days = f.Days,
                DisabilityAmount = f.DisabilityAmount,
                EndDate = f.EndDate.Value.ToString("yyyy-MM-dd"),
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                FromType = f.FromType,
                IdCardNo = f.IdCardNo,
                InJobDays = f.InJobDays,
                IsInJob = f.IsInJob ? "是" : "否",
                CustomerName = f.CustomerName,
                Name = f.Name,
                PartnerNamer = f.RealPartnerName,
                PlanName1 = f.PlanName1,
                ProfitMonth = f.ProfitMonth.Value.ToString("yyyy-MM-dd"),
                JoinManageAmount = f.JoinManageAmount,
                RiskCurrentAmount = f.RiskCurrentAmount,
                ServiceFeeTotalAmount = f.ServiceFeeTotalAmount,
                SigningUnitName = f.SigningUnitName,
                StartDate = f.StartDate.Value.ToString("yyyy-MM-dd"),
                TaxMonth = f.TaxMonth.Value.ToString("yyyy-MM-dd"),
                UnitSalaryTotalAmount = f.UnitSalaryTotalAmount,
                UnitSocialTotalAmount = f.UnitSocialTotalAmount,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                CustomerThingAmount = f.CustomerThingAmount,
                IsDayMatch = f.IsDayMatch ? "是" : "否",
                SocialRepayRiskAmount = f.SocialRepayRiskAmount,
                ProfitAmount = f.ServiceFeeTotalAmount - f.JoinManageAmount - f.CustomerThingAmount - f.BusInsuranceSumAmount - f.RiskCurrentAmount - f.BusinessAmount - f.DisabilityAmount - f.AgentPaidTaxAmount
            }).ToList();
            data2.Add(new ProfitManageRecordExcelDto()
            {

                AgentPaidAmount = data2.Sum(f => f.AgentPaidAmount),
                AgentPaidTaxAmount = data2.Sum(f => f.AgentPaidTaxAmount),
                BusinessAmount = data2.Sum(f => f.BusinessAmount),
                //BusInsuranceBalanceAmount = f.BusInsuranceBalanceAmount,
                //BusInsuranceCurrentAmount = f.BusInsuranceCurrentAmount,
                BusInsuranceNextAmount = data2.Sum(f => f.BusInsuranceNextAmount),
                BusInsuranceSumAmount = data2.Sum(f => f.BusInsuranceSumAmount),
                BusInsuranceChangeAmount = data2.Sum(f => f.BusInsuranceChangeAmount),
                Days = data2.Sum(f => f.Days),
                DisabilityAmount = data2.Sum(f => f.DisabilityAmount),
                EndDate = "",
                EntryDate = "",
                LeaveDate = "",
                FromType = "",
                IdCardNo = "",
                InJobDays = data2.Sum(f => f.InJobDays),
                IsInJob = "",
                CustomerName = "合计",
                Name = "",
                PartnerNamer = "",
                PlanName1 = "",
                ProfitMonth = "",
                JoinManageAmount = data2.Sum(f => f.JoinManageAmount),
                RiskCurrentAmount = data2.Sum(f => f.RiskCurrentAmount),
                ServiceFeeTotalAmount = data2.Sum(f => f.ServiceFeeTotalAmount),
                SigningUnitName = "",
                StartDate = "",
                TaxMonth = "",
                UnitSalaryTotalAmount = data2.Sum(f => f.UnitSalaryTotalAmount),
                UnitSocialTotalAmount = data2.Sum(f => f.UnitSocialTotalAmount),
                WorkClass = "",
                WorkName = "",
                CustomerThingAmount = data2.Sum(f => f.CustomerThingAmount),
                ProfitAmount = data2.Sum(f => f.ProfitAmount),
            });
            var fileBytes = NpoiExcelHelper.WriteExcel< ProfitManageRecordExcelDto>(data2, "月毛利记录");
            string base64String = Convert.ToBase64String(fileBytes);
            //return Ok(new { base64String, fileName = "毛利统计.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });



            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "11.xlsx");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfitRecord()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            await _recordService.Delete(f => f.ProfitMonth == grossMonth);
            return Ok();
        }

  

    }
}
