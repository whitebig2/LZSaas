using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.IExcelTemplate.cityPartners;
using LZ.Model.IExcelTemplate.profit;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.Annotations;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using SqlSugar.IOC;
using System.Collections.Generic;
using System.Linq;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class CityPartnerBillController : ApiControllerBase
    {

        private readonly IProfitConfigService  _profitConfigService;
        private readonly IProfitManageService  _profitManageService;
        private readonly IProfitManageRecordService  _profitManageRecordService;
        private readonly ICityPartnerBillService  _cityPartnerBillService;
        private readonly ICityPartnerSalaryRecordService  _cityPartnerSalaryRecordService;
        private readonly ISalaryRecordService  _salaryRecordService;
        private readonly IFinanceOtherService  _otherService;
        private readonly ICityPartnerEmpRecordService  _cityPartnerEmpRecordService;
        private readonly IUserEntryService _userEntryService;
        private readonly ISaleService _saleService;

        public CityPartnerBillController(ICityPartnerEmpRecordService cityPartnerEmpRecordService, IUserEntryService userEntryService, IFinanceOtherService otherService, ISalaryRecordService salaryRecordService, ICityPartnerSalaryRecordService cityPartnerSalaryRecordService, ICityPartnerBillService cityPartnerBillService, IProfitManageService profitManageService, IProfitConfigService profitConfigService, IProfitManageRecordService profitManageRecordService, ISaleService saleService)
        {
            _cityPartnerEmpRecordService = cityPartnerEmpRecordService;
            _userEntryService = userEntryService;
            _otherService = otherService;
            _salaryRecordService = salaryRecordService;
            _cityPartnerSalaryRecordService = cityPartnerSalaryRecordService;
            _cityPartnerBillService = cityPartnerBillService;
            _profitManageService = profitManageService;
            _profitConfigService = profitConfigService;
            _profitManageRecordService = profitManageRecordService;
            _saleService = saleService;


        }



        [HttpGet]
        public async Task<IActionResult> PageList(string title, DateTime? billMonth, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_CityPartnerBill>();
            exp.AndIF(!string.IsNullOrWhiteSpace(title), it => it.CityPartnerTitle.Contains(title));
            exp.AndIF(billMonth.HasValue, it => it.BillMonth == billMonth.Value);
            var models = await _cityPartnerBillService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string title, DateTime? billMonth)
        {
            var exp = Expressionable.Create<Model.Live_CityPartnerBill>();
            exp.AndIF(!string.IsNullOrWhiteSpace(title), it => it.CityPartnerTitle.Contains(title));
            exp.AndIF(billMonth.HasValue, it => it.BillMonth == billMonth.Value);
            var models = await _cityPartnerBillService.List(exp.ToExpression());
            return Ok(new { items = models });
        }

      


        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await _cityPartnerBillService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_CityPartnerBill model)
        {

            await _cityPartnerBillService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var record = await _cityPartnerBillService.Get(f => f.ID == id);
            await _cityPartnerBillService.Delete(id);
            await _cityPartnerEmpRecordService.Delete(f => f.CityPartnerBillID == id);
            await _cityPartnerSalaryRecordService.Delete(f => f.CityPartnerBillID==id);
            return Ok();
        }

        #region 账单计算


        //[HttpPost]
        //public async Task<IActionResult> GenerateBills()
        //{
        //    var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
        //    var billMonth = currentMonth.AddMonths(-1);
        //    var startDate = billMonth.AddDays(3);
        //    var endDate = currentMonth.AddDays(3);

        //    // 获取城市合伙人
        //    List<Live_CityPartner> cityPartners = new List<Live_CityPartner>();
        //    // 获取城市合伙人
        //    var cityPartnerRes = await _otherService.ListCityPartners(string.Empty, true);
        //    if (cityPartnerRes != null && cityPartnerRes.Data != null)
        //    {
        //        cityPartners = cityPartnerRes.Data;
        //    }

        //    cityPartners = cityPartners.Where(f => f.City == "合伙人总部").ToList();
        //    var busInsuranceConfigList=await   _otherService.BusinessInsuranceConfigListAll();

        // var   busInsuranceConfigCustomerGIDList= busInsuranceConfigList.Select(f=>f.CustomerGID).Distinct().ToList();

        //    // 生成月账单数据
        //    var existRecords = await _cityPartnerBillService.List(f => f.BillMonth == billMonth);

        //    List< Live_CityPartnerBill > billRecordList=new List<Live_CityPartnerBill>();

        //    foreach (var cityPartner in cityPartners)
        //    {

        //        if (existRecords.Any(f => f.CityPartnerID == cityPartner.ID))
        //        {
        //            continue;
        //        }
        //        billRecordList.Add(new Live_CityPartnerBill()
        //        {
        //            BillMonth = billMonth,
        //            CityPartnerAgentName = cityPartner.AgentName,
        //            CityPartnerID = cityPartner.ID,
        //            CityPartnerTitle = cityPartner.Title,
        //            CreateTime = DateTime.Now,
        //            OutsourcingRebateAmount = 0,
        //            InsuranceRebateAmount = 0,
        //            LaborRebateAmount = 0,
        //            SalesmanAmount = 0,
        //            RecruitmentProfitAmount = 0,
        //            LaborProfitAmount = 0,
        //            Status = 0,
        //             ProfitPartnerGIDArr= cityPartner.ProfitPartnerGIDArr,
        //        });
        //    }

        //    Dictionary<int, string> dis = new Dictionary<int, string>();
        //    dis.Add(0, "未知");
        //    dis.Add(1, "劳务派遣");
        //    dis.Add(2, "劳务外包");
        //    dis.Add(3, "劳动关系-外包");
        //    dis.Add(4, "代缴商保");
        //    dis.Add(5, "代缴商保风险金");
        //    dis.Add(6, "代缴社保");
        //    dis.Add(7, "内部业务");
        //    dis.Add(8, "代发业务");
        //    dis.Add(9, "劳动关系-派遣");



        //    List<int> businessModes = new List<int>() { 2, 3 };
        //    int milliseconds = 0;

        //    List<Guid> partnerGIDArr= new List<Guid>(); 
        //    foreach (var item in billRecordList)
        //    {


        //        if (!string.IsNullOrWhiteSpace(item.ProfitPartnerGIDArr))
        //        {
        //            partnerGIDArr= item.ProfitPartnerGIDArr.Split(',').Select(x =>Guid.Parse(x.Trim())).ToList();
        //        }

        //        #region 获取工资+服务费总金额大于2000
        //        var customerRes = await _otherService.ListCustomers(string.Empty, item.CityPartnerTitle, string.Empty, true);
        //        List<Live_V_Customer> customers = new List<Live_V_Customer>();
        //        if (customerRes != null && customerRes.Data != null)
        //        {
        //            customers = customerRes.Data;
        //        }
        //        customers.Where(f => f.BusinessModel != null && businessModes.Contains(f.BusinessModel.Value));
        //        milliseconds = 0;
        //        List<Live_CityPartnerSalaryRecord> cityPartnerSalaryRecordList = new List<Live_CityPartnerSalaryRecord>();
        //        foreach (var customerItem in customers)
        //        {

        //            Console.WriteLine(customerItem.CustomerName);
        //            var salaryExp = Expressionable.Create<Model.Live_V_SalaryRecord>();
        //            salaryExp.And(f => f.CustomerGID == customerItem.CustomerGID);
        //            salaryExp.And(f => f.IsCustomerPaid == true && f.CustomerPaidTime != null && f.CustomerPaidTime.Value >= startDate && f.CustomerPaidTime < endDate);
        //            var salaryRecords = await _salaryRecordService.ListView(salaryExp.ToExpression());
        //            // 确保一个身份证号码只收取一次费用
        //            var salaryGroups = salaryRecords.GroupBy(f => new { f.Name, f.IdCardNo }).ToList();
        //            var cityPartnerSalaryRecords = salaryGroups.Select(f => new Live_CityPartnerSalaryRecord()
        //            {
        //                CityPartnerID = customerItem.CityPartnerID ?? Guid.Empty,
        //                CityPartnerTitle = customerItem.CityPartnerTitle ?? string.Empty,
        //                CreateTime = DateTime.Now.AddMilliseconds(milliseconds++),
        //                CustomerGID = f.First().CustomerGID ?? Guid.Empty,
        //                CustomerName = f.First().CustomerName,
        //                EntryDate = f.First().EntryDate.Value.Date,
        //                LeaveDate = f.First().LeaveDate?.Date,
        //                CardID = f.Key.IdCardNo,
        //                Name = f.Key.Name,
        //                PerPartnerGID = customerItem.PartnerGID ?? Guid.Empty,
        //                PerPartnerName = customerItem.PartnerName ?? string.Empty,
        //                ServiceAmount = f.Sum(ff => ff.ServiceFeeAmount),
        //                TaxMonth = f.First().TaxMonth,
        //                UnitSalaryAmount = f.Sum(ff => ff.UnitSalaryAmount),
        //                UserEntryID = 0,
        //                BusinessModel = customerItem.BusinessModel ?? 0,
        //                BusinessModelName = dis[customerItem.BusinessModel??0],
        //                SalesmanPercent = 0,
        //                Salesman = customerItem.SalesMan ?? string.Empty,
        //                RecordMonth = billMonth,
        //                CustomerPaidTime = f.First().CustomerPaidTime,
        //                RebateAmount = 0,//外包反
        //            }).ToList();
        //            if (cityPartnerSalaryRecords.Any())
        //            {
        //                cityPartnerSalaryRecordList.AddRange(cityPartnerSalaryRecords);
        //            }
        //        }
        //            foreach (var salaryRecordItem in cityPartnerSalaryRecordList)
        //            {
        //                if (salaryRecordItem.ServiceAmount + salaryRecordItem.UnitSalaryAmount >= 2000)
        //                {
        //                    if (salaryRecordItem.BusinessModel == 2)
        //                    {
        //                        salaryRecordItem.RebateAmount = 12;
        //                        salaryRecordItem.HQRebateAmount = 3;

        //                    }
        //                    else if (salaryRecordItem.BusinessModel == 3)
        //                    {
        //                        salaryRecordItem.RebateAmount = 5;
        //                    salaryRecordItem.HQRebateAmount = 1;
        //                }
        //                }
        //                else
        //                {
        //                    salaryRecordItem.RebateAmount = 0;
        //                }
        //            }
        //        #endregion

        //        milliseconds = 0;
        //        #region 城市合伙人 员工信息

        //        //_otherService.ExistCustomerInsuranceConfig()

        //        var empExp = Expressionable.Create<Model.Live_V_UserEntryUnit>();
        //        // 配置过商保配置的
        //        empExp.And(f => busInsuranceConfigCustomerGIDList.Contains(f.CustomerGID.Value));
        //        empExp.And(f => f.Status == 2 || f.Status == 4);
        //        empExp.And(f => f.EntryTime < currentMonth);
        //        empExp.And(f => f.LeaveTime == null || f.LeaveTime.Value > currentMonth);
        //        empExp.And(f => f.CityPartnerID == item.CityPartnerID||f.CompanyCityPartnerID== item.CityPartnerID);
        //        var userEntryList = await _userEntryService.UserEntryUnitList(empExp.ToExpression());
        //        var cityPartnerEmpRecordList = userEntryList.Select(f => new Live_CityPartnerEmpRecord()
        //        {
        //            CityPartnerID = f.CityPartnerID ?? Guid.Empty,
        //            CityPartnerTitle = f.CityPartnerTitle ?? string.Empty,
        //            CreateTime = DateTime.Now.AddMilliseconds(milliseconds++),
        //            CustomerGID = f.CustomerGID ?? Guid.Empty,
        //            CustomerName = f.CustomerName,
        //            EntryDate = f.EntryTime.Value.Date,
        //            LeaveDate = f.LeaveTime?.Date,
        //            IdCardNo = f.CardID,
        //            Name = f.Name,
        //            PerPartnerGID = f.PerPartnerGID ?? Guid.Empty,
        //            PerPartnerName = f.PerPartnerName ?? string.Empty,
        //            RebateAmount = 3m,
        //            RecordMonth = billMonth,
        //            UserEntryID = f.ID,
        //            CityPartnerAgentName = f.CityPartnerAgentName ?? string.Empty,
        //        }).ToList();

        //        #endregion

        //        List<int> businessModels=new List<int>() { };

        //        #region 业务员
        //        var salesmanProfitRecords = await _profitManageRecordService.List(f =>f.CustomerBusinessModel!=null&& f.CustomerBusinessModel.Value<=2&&  f.ProfitMonth == billMonth && f.Salesman == item.CityPartnerAgentName);
        //        #endregion
        //        #region 毛利获取数据
        //        List<Live_Partner> partners = new List<Live_Partner>();
        //        // 获取服务商
        //        var partnerRes = await _otherService.ListPartners(item.CityPartnerID);
        //        if (partnerRes != null && partnerRes.Data != null)
        //        {
        //            partners = partnerRes.Data;
        //        }
        //        // 获取此城市合伙人的服务商信息
        //        var laborProfitList = await _profitManageService.List(f => partnerGIDArr.Contains(f.PartnerGID)&& f.ProfitMonth == billMonth && f.CityPartnerID==item.CityPartnerID&& f.PartnerName.StartsWith("劳动关系-"));
        //        var otherProfitList = await _profitManageService.List(f => partnerGIDArr.Contains(f.PartnerGID) && f.ProfitMonth == billMonth && f.CityPartnerID == item.CityPartnerID&&!f.PartnerName.StartsWith("劳动关系-"));
        //        #endregion
        //        var salesmanProfitRecordList = await _profitManageRecordService.List(f => f.ProfitMonth == billMonth &&f.Salesman==item.CityPartnerAgentName);
        //        item.BillMonth = billMonth;
        //        item.InsuranceRebateAmount = cityPartnerEmpRecordList.Sum(f => f.RebateAmount);
        //        // 劳动关系
        //        item.LaborRebateAmount = cityPartnerSalaryRecordList.Where(f => f.BusinessModelName == "劳动关系-外包").Sum(f=>f.RebateAmount) ;
        //        //外包财务奖励
        //        item.OutsourcingRebateAmount = cityPartnerSalaryRecordList.Where(f => f.BusinessModelName == "劳务外包").Sum(f => f.RebateAmount);
        //        item.SalesmanAmount = salesmanProfitRecords.Sum(f => f.BusinessAmount);
        //        item.LaborProfitAmount = laborProfitList.Sum(f => f.ProfitAmount ?? 0);
        //        item.RecruitmentProfitAmount = otherProfitList.Sum(f => f.ProfitAmount ?? 0);
        //        item.CityPartnerAgentName = item.CityPartnerAgentName;
        //        item.CityPartnerID = item.CityPartnerID;
        //        item.CityPartnerTitle = item.CityPartnerTitle;
        //        item.CreateTime = DateTime.Now;
        //        try
        //        {
        //            DbScoped.SugarScope.BeginTran();
        //            if (cityPartnerEmpRecordList.Any())
        //            {
        //                await _cityPartnerEmpRecordService.Add(cityPartnerEmpRecordList);
        //            }
        //            if (cityPartnerSalaryRecordList.Any())
        //            {
        //                await _cityPartnerSalaryRecordService.Add(cityPartnerSalaryRecordList);
        //            }
        //            await _cityPartnerBillService.Add(item);
        //            DbScoped.SugarScope.CommitTran();
        //        }
        //        catch (Exception ex)
        //        {
        //            DbScoped.SugarScope.RollbackTran();
        //            throw new ErrorCodeException(-1,ex.Message);
        //        }
        //    }




        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> Test()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("time", currentMonth.ToString("yyyy-MM-dd"));
            var result = await _saleService.CityPartnerBillList(dic, "/api/web/cityPartner/billList", 1);
            return Ok(new {result});
        }

        [HttpPost]
        public async Task<IActionResult> GenerateBills()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var billMonth = currentMonth.AddMonths(-1);
            var startDate = billMonth.AddDays(3);
            var endDate = currentMonth.AddDays(3);

          


            #region 生成账单

            List<Live_CityPartner> cityPartners = new List<Live_CityPartner>();
            // 获取城市合伙人
            var cityPartnerRes = await _otherService.ListCityPartners(string.Empty, true);
            if (cityPartnerRes != null && cityPartnerRes.Data != null)
            {
                cityPartners = cityPartnerRes.Data;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("time", billMonth.ToString("yyyy-MM-dd"));
            var result = await _saleService.CityPartnerBillList(dic, "/api/web/cityPartner/billList", 1);
            if(result.Code != 0)
            {
                throw new ErrorCodeException(-1, "销售系统报错:"+result.Msg);
            }
            // 生成月账单数据
            var existRecords = await _cityPartnerBillService.List(f => f.BillMonth == billMonth);

            List<Live_CityPartnerBill> billRecordList = new List<Live_CityPartnerBill>();

            foreach (var cityPartner in cityPartners)
            {
                if (existRecords.Any(f => f.CityPartnerID == cityPartner.ID))
                {
                    continue;
                }
                var bill = result.Bills.Where(t => t.CityPartnerName == cityPartner.Title).ToList();
                var money = bill != null ? bill.Sum(t => t.Money) : 0;
                billRecordList.Add(new Live_CityPartnerBill()
                {
                    BillMonth = billMonth,
                    CityPartnerAgentName = cityPartner.AgentName,
                    CityPartnerID = cityPartner.ID,
                    CityPartnerTitle = cityPartner.Title,
                    CreateTime = DateTime.Now,
                    OutsourcingRebateAmount = 0,
                    InsuranceRebateAmount = 0,
                    LaborRebateAmount = 0,
                    SalesmanAmount = 0,
                    RecruitmentProfitAmount = 0,
                    LaborProfitAmount = 0,
                    Status = 0,
                    ProfitPartnerGIDArr = cityPartner.ProfitPartnerGIDArr,
                    CityPartnerCity = cityPartner.City,
                    SaleBill = money
                });
            }
            #endregion

            if (billRecordList.Any())
            {

                foreach (var item in billRecordList)
                {
                  await  _cityPartnerBillService.Add(item);
                }
              
            }



            // 客户单位商保配置
            var busInsuranceConfigList = await _otherService.BusinessInsuranceConfigListAll();
            //配置了商保配置的客户单位标识
            var busInsuranceConfigCustomerGIDList = busInsuranceConfigList.Where(f => f.IsEnabled).Select(f => f.CustomerGID).Distinct().ToList();
            List<int> businessModels = new List<int>() { 2,3};
            Dictionary<int, string> dis = new Dictionary<int, string>();
            dis.Add(0, "未知");
            dis.Add(1, "劳务派遣");
            dis.Add(2, "劳务外包");
            dis.Add(3, "劳动关系-外包");
            dis.Add(4, "代缴商保");
            dis.Add(5, "代缴商保风险金");
            dis.Add(6, "代缴社保");
            dis.Add(7, "内部业务");
            dis.Add(8, "代发业务");
            dis.Add(9, "劳动关系-派遣");
            int milliseconds = 0;
     
            foreach (var item in billRecordList)
            {
                if (item.CityPartnerCity == "合伙人总部")
                {
                    // 幸福职

                    #region 员工已参保商保返利
                    // 获取所有配置了商保
                    List<Live_CityPartnerEmpRecord> allEmpRecords = new List<Live_CityPartnerEmpRecord>();
                    List<Live_CityPartnerSalaryRecord> salaryRecordList = new List<Live_CityPartnerSalaryRecord>();
                    foreach (var configItem in busInsuranceConfigCustomerGIDList)
                    {
                        milliseconds = 0;
                        var empExp = Expressionable.Create<Model.Live_V_UserEntryUnit>();
                        // 配置过商保配置的
                        empExp.And(f => f.CustomerGID == configItem);
                        empExp.And(f => f.Status == 2 || f.Status == 4);
                        empExp.And(f => f.EntryTime < currentMonth);
                        empExp.And(f => f.LeaveTime == null || f.LeaveTime.Value >= currentMonth);
                        var userEntryList = await _userEntryService.UserEntryUnitList(empExp.ToExpression());
                        var cityPartnerEmpRecordList = userEntryList.Select(f => new Live_CityPartnerEmpRecord()
                        {
                            CityPartnerID = f.CityPartnerID ?? Guid.Empty,
                            CityPartnerTitle = f.CityPartnerTitle ?? string.Empty,
                            CreateTime = DateTime.Now.AddMilliseconds(milliseconds++),
                            CustomerGID = f.CustomerGID ?? Guid.Empty,
                            CustomerName = f.CustomerName,
                            EntryDate = f.EntryTime.Value.Date,
                            LeaveDate = f.LeaveTime?.Date,
                            IdCardNo = f.CardID,
                            Name = f.Name,
                            PerPartnerGID = f.PerPartnerGID ?? Guid.Empty,
                            PerPartnerName = f.PerPartnerName ?? string.Empty,
                            RebateAmount = 0.5m,
                            RecordMonth = billMonth,
                            UserEntryID = f.ID,
                            CityPartnerAgentName = f.CityPartnerAgentName ?? string.Empty,
                            CompanyCityPartnerTitle = f.CompanyCityPartnerTitle ?? string.Empty,
                            HQRebateAmount = 0.5m,
                            PlanStatus1 = f.PlanStatus1 ?? 0,
                             CityPartnerBillID=item.ID
                        }).ToList();
                        if (cityPartnerEmpRecordList.Any())
                        {
                            allEmpRecords.AddRange(cityPartnerEmpRecordList);
                        }
                    }
                    #endregion
                    #region 工资+服务费超过3500 提成金额
                    var cusRes = await _otherService.ListCustomersByCompanyID(1);
                    if (cusRes.Data != null && cusRes.Data.Count >= 0)
                    {
                        var companyCustomerList = new List<Live_V_Customer>();

                        companyCustomerList= cusRes.Data.Where(f=>f.Enabled&&f.BusinessModel!=null&& businessModels.Contains(f.BusinessModel.Value)).ToList();

                        foreach (var customerItem in companyCustomerList)
                        {
                            Console.WriteLine(customerItem.CustomerName);
                            var salaryExp = Expressionable.Create<Model.Live_V_SalaryRecord>();
                            salaryExp.And(f => f.CustomerGID == customerItem.CustomerGID);
                            salaryExp.And(f => f.IsCustomerPaid == true && f.CustomerPaidTime != null && f.CustomerPaidTime.Value >= startDate && f.CustomerPaidTime < endDate);
                            var salaryRecords = await _salaryRecordService.ListView(salaryExp.ToExpression());
                            // 确保一个身份证号码只收取一次费用
                            var salaryGroups = salaryRecords.GroupBy(f => new { f.Name, f.IdCardNo }).ToList();
                            var cityPartnerSalaryRecords = salaryGroups.Select(f => new Live_CityPartnerSalaryRecord()
                            {
                                CityPartnerID = customerItem.CityPartnerID ?? Guid.Empty,
                                CityPartnerTitle = customerItem.CityPartnerTitle ?? string.Empty,
                                CreateTime = DateTime.Now.AddMilliseconds(milliseconds++),
                                CustomerGID = f.First().CustomerGID ?? Guid.Empty,
                                CustomerName = f.First().CustomerName,
                                EntryDate = f.First().EntryDate.Value.Date,
                                LeaveDate = f.First().LeaveDate?.Date,
                                CardID = f.Key.IdCardNo,
                                Name = f.Key.Name,
                                PerPartnerGID = customerItem.PartnerGID ?? Guid.Empty,
                                PerPartnerName = customerItem.PartnerName ?? string.Empty,
                                ServiceAmount = f.Sum(ff => ff.ServiceFeeAmount),
                                TaxMonth = f.First().TaxMonth,
                                UnitSalaryAmount = f.Sum(ff => ff.UnitSalaryAmount),
                                UserEntryID = 0,
                                BusinessModel = customerItem.BusinessModel ?? 0,
                                BusinessModelName = dis[customerItem.BusinessModel ?? 0],
                                SalesmanPercent = 0,
                                Salesman = string.Empty,
                                RecordMonth = billMonth,
                                CustomerPaidTime = f.First().CustomerPaidTime,
                                RebateAmount = 0,//外包反
                                CityPartnerBillID = item.ID
                            }).ToList();
                            foreach (var item1 in cityPartnerSalaryRecords)
                            {

                                if (item1.UnitSalaryAmount + item1.ServiceAmount >= 3500)
                                {
                                    if (item1.BusinessModel == 2)
                                    {
                                        item1.RebateAmount = 3;
                                    }
                                    else if (item1.BusinessModel == 3)
                                    {
                                        item1.RebateAmount = 1;
                                    }
                                }
                            }
                            if (cityPartnerSalaryRecords.Any())
                            {
                                salaryRecordList.AddRange(cityPartnerSalaryRecords);
                            }
                        }
                    }
                    #endregion



                    try
                    {
                        DbScoped.SugarScope.BeginTran();
                        if (allEmpRecords.Any())
                        {
                            await _cityPartnerEmpRecordService.Add(allEmpRecords);
                        }
                        if (salaryRecordList.Any())
                        {
                            await _cityPartnerSalaryRecordService.Add(salaryRecordList);
                        }
                        item.InsuranceRebateAmount = allEmpRecords.Sum(f => f.RebateAmount);
                        item.LaborRebateAmount = salaryRecordList.Where(f => f.BusinessModelName == "劳动关系-外包").Sum(f => f.RebateAmount);
                        //外包财务奖励
                        item.OutsourcingRebateAmount = salaryRecordList.Where(f => f.BusinessModelName == "劳务外包").Sum(f => f.RebateAmount);
                        if (item.ID > 0)
                        {
                            await _cityPartnerBillService.Update(item);
                        }
                        else
                        {
                            await _cityPartnerBillService.Add(item);
                        }

                        DbScoped.SugarScope.CommitTran();
                    }
                    catch (Exception ex)
                    {
                        DbScoped.SugarScope.RollbackTran();
                        throw new ErrorCodeException(-1, ex.Message);
                    }

                }
                else {

                    List<Guid> partnerGIDArr = new List<Guid>();

                    if (!string.IsNullOrWhiteSpace(item.ProfitPartnerGIDArr))
                    {
                        partnerGIDArr = item.ProfitPartnerGIDArr.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                    }
                    #region 获取工资+服务费总金额大于3500
                    var customerRes = await _otherService.ListCustomers(string.Empty, item.CityPartnerTitle, string.Empty, true);
                    List<Live_V_Customer> customers = new List<Live_V_Customer>();
                    if (customerRes != null && customerRes.Data != null)
                    {
                        customers = customerRes.Data;
                    }
                    customers= customers.Where(f => f.BusinessModel != null && businessModels.Contains(f.BusinessModel.Value)).ToList();
                    List<Live_CityPartnerSalaryRecord> cityPartnerSalaryRecordList = new List<Live_CityPartnerSalaryRecord>();
                    foreach (var customerItem in customers)
                    {

                        Console.WriteLine(customerItem.CustomerName);

                        
                        var salaryExp = Expressionable.Create<Model.Live_V_SalaryRecord>();
                        salaryExp.And(f => f.CustomerGID == customerItem.CustomerGID);
                        salaryExp.And(f => f.IsCustomerPaid == true && f.CustomerPaidTime != null && f.CustomerPaidTime.Value >= startDate && f.CustomerPaidTime < endDate);
                        var salaryRecords = await _salaryRecordService.ListView(salaryExp.ToExpression());
                        // 确保一个身份证号码只收取一次费用
                        var salaryGroups = salaryRecords.GroupBy(f => new { f.Name, f.IdCardNo }).ToList();
                        var cityPartnerSalaryRecords = salaryGroups.Select(f => new Live_CityPartnerSalaryRecord()
                        {
                            CityPartnerID = customerItem.CityPartnerID ?? Guid.Empty,
                            CityPartnerTitle = customerItem.CityPartnerTitle ?? string.Empty,
                            CreateTime = DateTime.Now.AddMilliseconds(milliseconds++),
                            CustomerGID = f.First().CustomerGID ?? Guid.Empty,
                            CustomerName = f.First().CustomerName,
                            EntryDate = f.First().EntryDate.Value.Date,
                            LeaveDate = f.First().LeaveDate?.Date,
                            CardID = f.Key.IdCardNo,
                            Name = f.Key.Name,
                            PerPartnerGID = customerItem.PartnerGID ?? Guid.Empty,
                            PerPartnerName = customerItem.PartnerName ?? string.Empty,
                            ServiceAmount = f.Sum(ff => ff.ServiceFeeAmount),
                            TaxMonth = f.First().TaxMonth,
                            UnitSalaryAmount = f.Sum(ff => ff.UnitSalaryAmount),
                            UserEntryID = 0,
                            BusinessModel = customerItem.BusinessModel ?? 0,
                            BusinessModelName = dis[customerItem.BusinessModel ?? 0],
                            SalesmanPercent = 0,
                            Salesman = customerItem.SalesMan ?? string.Empty,
                            RecordMonth = billMonth,
                            CustomerPaidTime = f.First().CustomerPaidTime,
                            RebateAmount = 0,//外包反
                            CityPartnerBillID = item.ID,
                            HQRebateAmount = 0
                        }).ToList();
                        if (cityPartnerSalaryRecords.Any())
                        {
                            cityPartnerSalaryRecordList.AddRange(cityPartnerSalaryRecords);
                        }
                    }
                    foreach (var salaryRecordItem in cityPartnerSalaryRecordList)
                    {
                        if (salaryRecordItem.ServiceAmount + salaryRecordItem.UnitSalaryAmount >= 3500)
                        {
                            if (salaryRecordItem.BusinessModel == 2)
                            {
                                salaryRecordItem.RebateAmount = 12;
                            }
                            else if (salaryRecordItem.BusinessModel == 3)
                            {
                                salaryRecordItem.RebateAmount = 5;
                            }
                        }
                        else
                        {
                            salaryRecordItem.RebateAmount = 0;
                        }
                    }
                    #endregion

                    milliseconds = 0;
                    #region 城市合伙人 员工信息

                    //_otherService.ExistCustomerInsuranceConfig()

                    var empExp = Expressionable.Create<Model.Live_V_UserEntryUnit>();
                    // 配置过商保配置的
                    empExp.And(f => busInsuranceConfigCustomerGIDList.Contains(f.CustomerGID.Value));
                    empExp.And(f => f.Status == 2 || f.Status == 4);
                    empExp.And(f => f.EntryTime < currentMonth);
                    empExp.And(f => f.LeaveTime == null || f.LeaveTime.Value > currentMonth);
                    empExp.And(f => f.CityPartnerID == item.CityPartnerID || f.CompanyCityPartnerID == item.CityPartnerID);
                    var userEntryList = await _userEntryService.UserEntryUnitList(empExp.ToExpression());
                    var cityPartnerEmpRecordList = userEntryList.Select(f => new Live_CityPartnerEmpRecord()
                    {
                        CityPartnerID = f.CityPartnerID ?? Guid.Empty,
                        CityPartnerTitle = f.CityPartnerTitle ?? string.Empty,
                        CreateTime = DateTime.Now.AddMilliseconds(milliseconds++),
                        CustomerGID = f.CustomerGID ?? Guid.Empty,
                        CustomerName = f.CustomerName,
                        EntryDate = f.EntryTime.Value.Date,
                        LeaveDate = f.LeaveTime?.Date,
                        IdCardNo = f.CardID,
                        Name = f.Name,
                        PerPartnerGID = f.PerPartnerGID ?? Guid.Empty,
                        PerPartnerName = f.PerPartnerName ?? string.Empty,
                        RebateAmount = 3m,
                        RecordMonth = billMonth,
                        UserEntryID = f.ID,
                        CityPartnerAgentName = f.CityPartnerAgentName ?? string.Empty,
                        CityPartnerBillID = item.ID,
                        CompanyCityPartnerTitle = f.CompanyCityPartnerTitle ?? string.Empty,
                        HQRebateAmount = 0,
                        PlanStatus1 = f.PlanStatus1 ?? 0

                    }).ToList();

                    #endregion
                    #region 业务员
                    var salesmanProfitRecords = await _profitManageRecordService.List(f => f.CustomerBusinessModel != null && f.CustomerBusinessModel.Value <= 2 && f.ProfitMonth == billMonth && f.Salesman == item.CityPartnerAgentName);
                    #endregion
                    #region 毛利获取数据
                    List<Live_Partner> partners = new List<Live_Partner>();
                    // 获取服务商
                    var partnerRes = await _otherService.ListPartners(item.CityPartnerID);
                    if (partnerRes != null && partnerRes.Data != null)
                    {
                        partners = partnerRes.Data;
                    }
                    // 获取此城市合伙人的服务商信息
                    var laborProfitList = await _profitManageService.List(f => partnerGIDArr.Contains(f.PartnerGID) && f.ProfitMonth == billMonth && f.CityPartnerID == item.CityPartnerID && f.PartnerName.StartsWith("劳动关系-"));
                    var otherProfitList = await _profitManageService.List(f => partnerGIDArr.Contains(f.PartnerGID) && f.ProfitMonth == billMonth && f.CityPartnerID == item.CityPartnerID && !f.PartnerName.StartsWith("劳动关系-"));
                    #endregion
                    var salesmanProfitRecordList = await _profitManageRecordService.List(f => f.ProfitMonth == billMonth && f.Salesman == item.CityPartnerAgentName);
                    item.BillMonth = billMonth;
                    item.InsuranceRebateAmount = cityPartnerEmpRecordList.Sum(f => f.RebateAmount);
                    // 劳动关系
                    item.LaborRebateAmount = cityPartnerSalaryRecordList.Where(f => f.BusinessModelName == "劳动关系-外包").Sum(f => f.RebateAmount);
                    //外包财务奖励
                    item.OutsourcingRebateAmount = cityPartnerSalaryRecordList.Where(f => f.BusinessModelName == "劳务外包").Sum(f => f.RebateAmount);
                    item.SalesmanAmount = salesmanProfitRecords.Sum(f => f.BusinessAmount);
                    item.LaborProfitAmount = laborProfitList.Sum(f => f.ProfitAmount ?? 0);
                    item.RecruitmentProfitAmount = otherProfitList.Sum(f => f.ProfitAmount ?? 0);
                    item.CityPartnerAgentName = item.CityPartnerAgentName;
                    item.CityPartnerID = item.CityPartnerID;
                    item.CityPartnerTitle = item.CityPartnerTitle;
                    item.CreateTime = DateTime.Now;
                    try
                    {
                        DbScoped.SugarScope.BeginTran();
                        if (cityPartnerEmpRecordList.Any())
                        {
                            await _cityPartnerEmpRecordService.Add(cityPartnerEmpRecordList);
                        }
                        if (cityPartnerSalaryRecordList.Any())
                        {
                            await _cityPartnerSalaryRecordService.Add(cityPartnerSalaryRecordList);
                        }

                        if (item.ID > 0)
                        {
                            await _cityPartnerBillService.Update(item);
                        }
                        else
                        {
                            await _cityPartnerBillService.Add(item);
                        }
                        DbScoped.SugarScope.CommitTran();
                    }
                    catch (Exception ex)
                    {
                        DbScoped.SugarScope.RollbackTran();
                        throw new ErrorCodeException(-1, ex.Message);
                    }
                }
            }




            return Ok();
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> ExportBillDetail(int id)
        {

            var billRecord = await _cityPartnerBillService.Get(f => f.ID == id);

            List<Guid> partnerGIDArr = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(billRecord.ProfitPartnerGIDArr))
            {
                partnerGIDArr = billRecord.ProfitPartnerGIDArr.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            }
            var empRecordList = await _cityPartnerEmpRecordService.List(f =>f.CityPartnerBillID==id);
            var salaryRecordList = await _cityPartnerSalaryRecordService.List(f => f.CityPartnerBillID==id);

            #region 城市合伙人账单
            CityPartnerBillExcelTemplate cityPartnerBillExcelTemplate = new CityPartnerBillExcelTemplate()
            {

                BillMonth = billRecord.BillMonth,
                CityPartnerAgentName = billRecord.CityPartnerAgentName,
                CityPartnerTitle = billRecord.CityPartnerTitle,
                InsuranceRebateAmount = billRecord.InsuranceRebateAmount,
                LaborProfitAmount = billRecord.LaborProfitAmount,
                LaborRebateAmount = billRecord.LaborRebateAmount,
                OutsourcingRebateAmount = billRecord.OutsourcingRebateAmount,
                RecruitmentProfitAmount = billRecord.RecruitmentProfitAmount,
                SalesmanAmount = billRecord.SalesmanAmount,
            };
            var data1 = new List<CityPartnerBillExcelTemplate>() { cityPartnerBillExcelTemplate };
            #endregion
            #region 商保返利账单
            var data2 = empRecordList.Select(f => new CityPartnerEmpRecordExcelTemplate()
            {
                CityPartnerAgentName = f.CityPartnerAgentName,
                CityPartnerTitle = f.CityPartnerTitle,
                CustomerName = f.CustomerName,
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                IdCardNo = f.IdCardNo,
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                Name = f.Name,
                PerPartnerName = f.PerPartnerName,
                RebateAmount = f.RebateAmount,
                RecordMonth = f.RecordMonth,
            }).ToList();
            #endregion


          
            #region 商保返利账单
            var data3 = salaryRecordList.Select(f => new CityPartnerSalaryRecordExcelTemplate()
            {
                CityPartnerTitle = f.CityPartnerTitle,
                CustomerName = f.CustomerName,
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                Name = f.Name,
                PerPartnerName = f.PerPartnerName,
                RebateAmount = f.RebateAmount,
                RecordMonth = f.RecordMonth,
                BusinessModelName = f.BusinessModelName,
                CardID = f.CardID,
                Salesman = f.Salesman,
                ServiceAmount = f.ServiceAmount,
                TaxMonth = f.TaxMonth,
                UnitSalaryAmount = f.UnitSalaryAmount,
            }).ToList();
            #endregion

            if (billRecord.CityPartnerCity == "合伙人总部")
            {
                var fileBytesHq = NpoiExcelHelper.WriteExcel<CityPartnerBillExcelTemplate, CityPartnerEmpRecordExcelTemplate, CityPartnerSalaryRecordExcelTemplate>(
             data1, "服务中心账单",
             data2, "商保返利",
             data3, "财政奖励"
             );
                string base64StringHq = Convert.ToBase64String(fileBytesHq);
                return Ok(new { base64String = base64StringHq, fileName = "毛利统计.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                //return File(fileBytesHq, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "11.xlsx");

            }



            // 只针对 劳务派遣、劳务外包
            var salesmanProfitRecords = await _profitManageRecordService.List(f => f.CustomerBusinessModel != null && f.CustomerBusinessModel.Value <= 2 && f.ProfitMonth == billRecord.BillMonth && f.Salesman == billRecord.CityPartnerAgentName);

            var data4 =  salesmanProfitRecords.Select(f => new CityPartnerSalesmanProfitExcelTemplate()
            {
                BusinessAmount = f.BusinessAmount,
                CardID = f.IdCardNo,
                CityPartnerTitle = f.CityPartnerTitle,
                CustomerName = f.CustomerName,
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                Name = f.Name,
                PerPartnerName = f.RealPartnerName,
                ProfitMonth = f.ProfitMonth,
                Salesman = f.Salesman,
                ServiceAmount = f.ServiceFeeTotalAmount,
                UnitSalaryAmount = f.UnitSalaryTotalAmount,
                BusinessModelName = f.CustomerBusinessModelName

            }).ToList();


            #region 毛利汇总数据
            // 获取服务商
            var profitManages = await _profitManageService.List(f => partnerGIDArr.Contains(f.PartnerGID)&& f.ProfitMonth == billRecord.BillMonth && f.CityPartnerID == billRecord.CityPartnerID);

            var data6 = profitManages.Select(f => new ProfitManageExcelDto()
            {
                InJobRecordCount = f.InJobRecordCount,
                ProfitMonth = f.ProfitMonth.Value.ToString("yyyy-MM-dd"),
                AgentPaidTaxTotalAmount = f.AgentPaidTaxAmount,
                AgentPaidTotalAmount = f.AgentPaidTotalAmount,
                BusinessTotalAmount = f.BusinessTotalAmount,
                BusInsuranceSumAmount = f.BusInsuranceSumAmount,
                DeductionRemark = string.Empty,
                DeductionTotalAmount = f.DeductionAmount,
                DisabilityTotalAmount = f.DisabilityTotalAmount,
                FromType = string.Empty,
                PartnerName = f.PartnerName,
                ProfitTotalAmount = f.ProfitAmount.Value,
                RiskTotalAmount = f.RiskTotalAmount,
                ServiceFeeRecordCount = f.ServiceFeeRecordCount,
                ServiceFeeTotalAmount = f.ServiceFeeTotalAmount,
                SigningUnitName = f.SigningUnitName,
                SubsidyRemark = string.Empty,
                SubsidyTotalAmount = f.SubsidyAmount,
                TaxMonth = f.TaxMonth.Value.ToString("yyyy-MM-dd"),
                UnitSalaryTotalAmount = f.UnitSalaryTotalAmount,
                UnitSocialTotalAmount = f.UnitSocialTotalAmount,
                CustomerThingTotalAmount = f.CustomerThingTotalAmount.Value,
                JoinManageIncomeAmount = f.JoinManageIncomeAmount,
                JoinManagePaidAmount = f.JoinManagePaidAmount,
                SocialRepayRiskAmount = f.SocialRepayRiskAmount,

            }).ToList();

      

            var recordList = await _profitManageRecordService.List(f => partnerGIDArr.Contains(f.PartnerGID) && f.ProfitMonth == billRecord.BillMonth &&f.CityPartnerID== billRecord.CityPartnerID);


            var data7 = recordList.Select(f => new ProfitManageRecordExcelDto()
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
                PartnerNamer = f.PartnerNamer,
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

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("time", billRecord.BillMonth.Value.ToString("yyyy-MM-dd"));
            dic.Add("name", billRecord.CityPartnerTitle);
            var result = await _saleService.CityPartnerBillList(dic, "/api/web/cityPartner/BillRecordList", 2);
            if (result.Code != 0)
            {
                throw new ErrorCodeException(-1, "销售系统报错:" + result.Msg);
            }
            
            var data8 = result.Records.Select(f => new SaleCityPartnerBillRecordTemplate()
            {

                FullName =f.FullName,
                CityPartnerName = f.CityPartnerName,
                CustomerName = f.CustomerName,
                StatusName = f.StatusName,
                Money = f.Money,
                CreateTime = f.CreateTime.Value.ToString("yyyy-MM-dd")
            }).ToList();


            #endregion

            var fileBytes =  NpoiExcelHelper.WriteExcel<CityPartnerBillExcelTemplate, CityPartnerEmpRecordExcelTemplate, CityPartnerSalaryRecordExcelTemplate, CityPartnerSalesmanProfitExcelTemplate, ProfitManageExcelDto, ProfitManageRecordExcelDto, SaleCityPartnerBillRecordTemplate>(
                data1,"服务中心账单",
                data2, "商保返利", 
                data3, "财政奖励", 
                data4, "新业务开发费佣金",
                data6, "企业毛利总数据",
                data7, "企业毛利详细",
                data8, "业务上传客户详情"
                );


            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "毛利统计.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

            //return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","11.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthBill(Guid cityPartnerID)
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var billMonth = currentMonth.AddMonths(-1);
            var model = await _cityPartnerBillService.Get(f => f.CityPartnerID == cityPartnerID && f.BillMonth != null && f.BillMonth.Value == billMonth);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListYearBills(Guid cityPartnerID)
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var billMonth = currentMonth.AddMonths(-1);
            var yearStartMonth = new DateTime(billMonth.Year, 1,1);
            var model = await _cityPartnerBillService.List(f => f.CityPartnerID == cityPartnerID && f.BillMonth != null && f.BillMonth.Value >= yearStartMonth);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
    }
}
