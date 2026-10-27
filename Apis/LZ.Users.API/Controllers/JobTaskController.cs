using LZ.ApiService.Interface;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using LZ.Model.InsuranceApi;
using LZ.Model;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using LZ.Users.Service.Services;
using System.Collections.Generic;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Chart;
using LZ.Common;
using LZ.Core.EventData.Insurance;
using System.Security.Policy;
using LZ.Core.EventData.FxqEventData;
using LZ.ApiService.Services;
using Senparc.NeuChar.NeuralSystems;
using LZ.Model.Exceptions;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.IExcelTemplate;
using System.Linq;
using Org.BouncyCastle.Pqc.Crypto.Frodo;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using LZ.Model.Dtos;
using LZ.Core.Redis;
using NPOI.SS.Formula.Functions;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.ModelExtensions;
using LZ.Model.ICBC.Plkh;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs.Card;

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class JobTaskController : UsersControllerBase
    {

        private readonly ILogger<JobTaskController> _logger;

        private readonly IBusinessInsuranceInitialRecordService _initialRecordService;
        private readonly IBusinessInsuranceRecordService _businessInsuranceRecordService;
        private readonly IInsuranceApiService _insuranceApiService;
        private readonly IEntryService _entryService;
        private readonly IBusinessInsuranceOrderService _orderService;
        private readonly IBusinessInsuranceRecordService _recordService;
        private readonly IBusinessInsurancePushService _pushService;
        private readonly IBusinessInsurancePushRecordService _pushRecordService;
        private readonly IBusinessInsuranceConfigService  _configService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEventPublisher  _publisher;
        private readonly IAliSendInfo  _aliSendInfo;
        private readonly IAliyunOSSService _aliyunOSSService;
        private readonly IBusinessApiOrderDetailService  _businessApiOrderDetailService;
        private readonly IBusinessGuaranteeService  _businessGuaranteeService;
        private readonly IBusinessInsurancService  _businessInsurancService;
        private readonly IRedisService  _redisService;
        private readonly IBusInsuracneService  _busInsuracneService;
        private readonly IWebHostEnvironment  _env;
        private readonly IMailSenderService   _mailSenderService;
        private readonly IDeathInsuranceRecordService _deathInsuranceRecordService;
        private readonly IBankOpenAccountService _openAccountService;
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpService _httpService;
        private readonly IFxqService _fxqService;
        private readonly IUserService _userService;




        public JobTaskController(ILogger<JobTaskController> logger, IEntryService entryService, IBusinessInsuranceOrderService orderService, IBusinessInsuranceRecordService recordService, IServiceScopeFactory scopeFactory, IBusinessInsurancePushService pushService = null, IInsuranceApiService insuranceApiService = null, IBusinessInsurancePushRecordService pushRecordService = null, IBusinessInsuranceRecordService businessInsuranceRecordService = null, IBusinessInsuranceConfigService configService = null, IEventPublisher publisher = null, IAliSendInfo aliSendInfo = null, IBusinessInsuranceInitialRecordService initialRecordService = null, IAliyunOSSService aliyunOSSService = null, IBusinessApiOrderDetailService businessApiOrderDetailService = null, IBusinessGuaranteeService businessGuaranteeService = null, IBusinessInsurancService businessInsurancService = null, IRedisService redisService = null, IBusInsuracneService busInsuracneService = null, IWebHostEnvironment env = null, IMailSenderService mailSenderService = null, IDeathInsuranceRecordService deathInsuranceRecordService = null, IBankOpenAccountService openAccountService = null, ISettingsService settingsService = null, IHttpClientFactory clientFactory = null, IHttpService httpService = null, IFxqService fxqService = null, IUserService userService = null)
        {
            _logger = logger;
            _entryService = entryService;
            _orderService = orderService;
            _recordService = recordService;
            _scopeFactory = scopeFactory;
            _pushService = pushService;
            _insuranceApiService = insuranceApiService;
            _pushRecordService = pushRecordService;
            _businessInsuranceRecordService = businessInsuranceRecordService;
            _configService = configService;
            _publisher = publisher;
            _aliSendInfo = aliSendInfo;
            _initialRecordService = initialRecordService;
            _aliyunOSSService = aliyunOSSService;
            _businessApiOrderDetailService = businessApiOrderDetailService;
            _businessGuaranteeService = businessGuaranteeService;
            _businessInsurancService = businessInsurancService;
            _redisService = redisService;
            _busInsuracneService = busInsuracneService;
            _env = env;
            _mailSenderService = mailSenderService;
            _deathInsuranceRecordService = deathInsuranceRecordService;
            _openAccountService = openAccountService;
            _settingsService = settingsService;
            _clientFactory = clientFactory;
            _httpService = httpService;
            _fxqService = fxqService;
            _userService = userService;
        }





        [HttpGet]
        public async Task<IActionResult> GetEnv()
        {
            if (_env.IsProduction())
            {
                return Ok("Production");
            }
            else
            {
                return Ok("dev");
            }
        }


      

        [HttpPost]
        public async Task<IActionResult> GenerateTestRecord()
        {
            await _busInsuracneService.GenerateTestRecord();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailTest()
        {
            await _busInsuracneService.SendEmailTest();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateBusinsuranceRecordByApi()
        {
            await _busInsuracneService.GenerateBusinsuranceRecordByApi();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInsuranceStatus(IFormFile file)
        {
            // 当月月初
            var taxMonth = DateTime.Now.Date.AddDays(1 - DateTime.Today.Day);
            HttpContext.Request.Headers.TryGetValue("authorization", out var value);
            List<TestExcelTemplate> excelRecords = new List<TestExcelTemplate>();
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                excelRecords = NpoiExcelHelper.ReadExcelFile<TestExcelTemplate>(ms,"");
            }
            foreach (var item in excelRecords)
            {
               var exists=await _entryService.ListView(f =>(f.Status==2||f.Status==4)&& f.Name == item.Name && f.CardID == item.IdCardNo && f.WorkName == item.WorkName && f.CustomerName == item.CustomerName);
                if (exists.Any(f => f.Status == 2))
                {
                    var exist = exists.First(f => f.Status == 2);
                    var entry = await _entryService.Get(f => f.ID == exist.ID);
                    entry.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
                    await _entryService.UpdateBusInsurance(entry);
                }
                else if (exists.Any(f => f.Status == 4))
                {
                    var exist = exists.First(f => f.Status == 4);
                    var entry = await _entryService.Get(f => f.ID == exist.ID);
                    entry.PlanStatus1 = UserEntryPlanStatusEnumType.已减员;
                    await _entryService.UpdateBusInsurance(entry);
                }

            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> JobTest(string insuranceId)
        {
            
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var orderList = await _orderService.ListView(f => f.InsuranceCompany == "阳光保险公司" && f.StartDate == currentMonth);
            var configList = await _configService.ListView(f =>  f.IsEnabled);

            List<BaoquanItem> empList = new List<BaoquanItem>();
            var cusGidList = configList.Where(f=>f.InsuranceCompany == "阳光保险公司" ).Select(f => f.CustomerGID).ToList();
            var entryList = await _entryService.List(f => !string.IsNullOrWhiteSpace(f.WorkClass)&&!string.IsNullOrWhiteSpace(f.WorkName) && cusGidList.Contains(f.CustomerGID.Value) && f.Status == 2);


            foreach (var item in orderList)
            {
                var data = await _insuranceApiService.GetOrderDetail(item.InsuranceId);

                data.Data.baoquanlist.ForEach(baoquan => baoquan.InsuranceId = item.InsuranceId);
          

                empList.AddRange(data.Data.baoquanlist);
            }


            List<Live_BusinessInsuranceRecord> records = new List<Live_BusinessInsuranceRecord>();
            Live_BusinessInsuranceRecord record = new Live_BusinessInsuranceRecord();


            //empList = empList.Where(f => f.Cname == "智慧宏").ToList();

            foreach (var item in empList)
            {
                if (item.Csjygdw== "宁波兄弟印刷业有限公司")
                {
                    item.Csjygdw = "宁波兄弟印刷有限公司";
                }
                var order = orderList.FirstOrDefault(f => f.InsuranceId == item.InsuranceId);

                var config = configList.FirstOrDefault(f => f.CustomerName == item.Csjygdw);
                if (config == null)
                {
                    Console.WriteLine($"{item.Csjygdw} 配置缺失");

                    continue;
                }
                var entry = await _entryService.View(f => f.CustomerName == item.Csjygdw && f.CardID == item.Cidnum && f.Status == 2);
                if (entry != null)
                {
                    record = new Live_BusinessInsuranceRecord()
                    {

                        PlanNameId = Convert.ToInt32(item.Geentid),
                        PlanName = item.Geentname,
                        Age = Convert.ToInt32(item.Cage),
                        AddMatchRecordID = null,
                        AddPushTime = item.Czbrq,
                        AddSysTime = item.Czbrq,
                        BusinessInsurancePushID = null,
                        CreateTime = DateTime.Today,
                        CustomerGID = entry.CustomerGID.Value,
                        CustomerName = entry.CustomerName,
                        DelMatchRecordID = null,
                        DelPushTime = null,
                        DelSysTime = null,
                        EndDate = item.Czbrq,
                        EntryDate = entry.EntryTime.Value.Date,
                        IdCardNo = item.Cidnum,
                        InsuranceCompany = "阳光保险公司",
                        InsuranceId = item.InsuranceId,
                        InsuranceNo = order.InsuranceNo,
                        InsuranceSigningUnitID = config.InsuranceSigningUnitID,
                        InsuranceSigningUnitName = config.InsuranceSigningUnitName,
                        IsDayOrder = order.OrderType == (int)InsuranceOrderEnumType.日保单,
                        ID = Guid.NewGuid(),
                        IsDefaultPush = order.IsDefaultPush,
                        IsExpired = false,
                        IsMain = true,
                        LeaveDate = null,
                        Name = item.Cname,
                        OrderNo ="API 匹配生成",
                        Sex = entry.Sex ?? "",
                        StartDate = item.Cqbrq,
                        Status = BusinessInsuranceStatusEnumType.已增员,
                        UserEntryID = entry.ID,
                        UserID = entry.UserID ?? 0,
                        WorkClass = item.Czylb + "类",
                        WorkName = item.Cgygz,
                    };
                    records.Add(record);
                }
                else
                {
                    entry = await _entryService.View(f => f.CustomerName == item.Csjygdw && f.CardID == item.Cidnum && f.Status == 4, "entrytime desc");
                    if (entry != null)
                    {
                        record = new Live_BusinessInsuranceRecord()
                        {

                            PlanNameId = Convert.ToInt32(item.Geentid),
                            PlanName = item.Geentname,
                            Age = Convert.ToInt32(item.Cage),
                            AddMatchRecordID = null,
                            AddPushTime = item.Czbrq,
                            AddSysTime = item.Czbrq,
                            BusinessInsurancePushID = null,
                            CreateTime = DateTime.Today,
                            CustomerGID = entry.CustomerGID.Value,
                            CustomerName = entry.CustomerName,
                            DelMatchRecordID = null,
                            DelPushTime = null,
                            DelSysTime = null,
                            EndDate = item.Czbrq,
                            EntryDate = entry.EntryTime.Value.Date,
                            IdCardNo = item.Cidnum,
                            InsuranceCompany = "阳光保险公司",
                            InsuranceId = item.InsuranceId,
                            InsuranceNo = order.InsuranceNo,
                            InsuranceSigningUnitID = config.InsuranceSigningUnitID,
                            InsuranceSigningUnitName = config.InsuranceSigningUnitName,
                            IsDayOrder = order.OrderType == (int)InsuranceOrderEnumType.日保单,
                            ID = Guid.NewGuid(),
                            IsDefaultPush = order.IsDefaultPush,
                            IsExpired = false,
                            IsMain = true,
                            LeaveDate = entry.LeaveTime.Value.Date,
                            Name = item.Cname,
                            OrderNo = "API 匹配生成",
                            Sex = entry.Sex ?? "",
                            StartDate = item.Cqbrq,
                            Status = BusinessInsuranceStatusEnumType.待减员匹配,
                            UserEntryID = entry.ID,
                            UserID = entry.UserID ?? 0,
                            WorkClass = item.Czylb + "类",
                            WorkName = item.Cgygz,

                        };
                        records.Add(record);
                    }
                    else
                    {
                        Console.WriteLine($"{item.Csjygdw}-- {item.Cname}--{item.Cidnum}--- {item.Geentname}--{item.Czylb}--{item.Cgygz}--不匹配");
                    }
                }

            }

            var insuranceIdList = orderList.Select(f => f.InsuranceId).ToList();


            var entryIDList = records.Select(f => f.UserEntryID).ToList();

            await _businessInsuranceRecordService.Delete(f => insuranceIdList.Contains(f.InsuranceId) && entryIDList.Contains(f.UserEntryID));
            await _businessInsuranceRecordService.Add(records);
            return Ok();
        }
        #region 退休时间任务,员工年龄

        /// <summary>
        /// 计算员工当前年龄定时任务
        /// </summary>
        /// <param name="insuranceId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EntryAgeJob()
        {
            var entryList = await _entryService.List(f => f.Status == 2 &&  !string.IsNullOrWhiteSpace(f.CardID) && f.CardID.Length == 18);
            foreach (var item in entryList)
            {
              //var age= IdCardHelper.GetAge(item.CardID);
              //  if (age > 0)
              //  {
              //      item.Age = age;
              //  }
            }
           await _entryService.UpdateAge(entryList);
            return Ok();
        }


        /// <summary>
        /// 退休检测定时任务
        /// </summary>
        /// <param name="insuranceId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RetireJob()
        {
            var entryList = await _entryService.List(f => f.Status == 2 && f.RetireDate == null&&!string.IsNullOrWhiteSpace(f.CardID)&&f.CardID.Length==18);
            foreach (var item in entryList)
            {
                if (item.CardID.Trim().Length != 18)
                {
                    Console.WriteLine(item.Name);
                    continue;
                }
                var entry = await _entryService.Get(item.ID);

                var retireDate = IdCardHelper.GetRetireDate(item.CardID.Trim());
                entry.RetireDate = retireDate;
                if (retireDate <= DateTime.Today)
                {
                    entry.IsRetire = true;
                }
                await _entryService.Update(entry);
            }
            return Ok();
        }

        /// <summary>
        ///  电子合同续签定时任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ContractRenewalJob()
        {
            //昨天
            var lastDay=DateTime.Today.AddDays(-1);

            var exp = Expressionable.Create<Live_UserEntry>();
            //exp.And(f => f.Name == "田敏" || f.Name == "王平");
            exp.And(f => f.Status == 2 &&f.ContractDate!=null&&f.ContractStartDate!=null);
            exp.And(f =>f.ContractEndDate != null && f.ContractEndDate<= lastDay);
            var entryList = await _entryService.List(exp.ToExpression());
            if (entryList.Any())
            {
                foreach (var item in entryList)
                {
                    item.ContractStartDate = null;
                    item.ContractEndDate = null;
                }
                await _entryService.UpdateContract(entryList);
                // 生成续签合同
                foreach (var item in entryList)
                {
                    UserEntryContractRecordEventData eventData=new UserEntryContractRecordEventData (item.ID,  ContractEnumType.入职合同,Guid.NewGuid());
                    _publisher.Publish(eventData);
                }
            }
         
            return Ok();
        }

        #endregion
        #region 商保定时任务
        ///// <summary>
        /////  商保相同保单增减匹配
        ///// </summary>
        ///// <param name="insuranceCompany"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> BusinessInsuranceAddDelMatchJob()
        //{
        //    // 测试单位
        //    var userEntryList = await _entryService.List(f => f.EntryTime.Value < DateTime.Today && !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName) && f.Status == 2 && (f.PlanStatus1 == null || f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保));
        //    foreach (var item in userEntryList)
        //    {
        //        await _businessInsuranceRecordService.ManageUserEntryPlan(item.ID);
        //    }

        //    //var userEntryList = await _entryService.List(f => (f.Status == 4 && f.LeaveTime >= DateTime.Today) && !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName) && (f.PlanStatus1 == null || f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保 || f.PlanName2 == null || f.PlanStatus2 != UserEntryPlanStatusEnumType.未参保));
        //    ////var userEntryList = await _entryService.List(f => (f.Status == 2 || (f.Status == 4 && f.LeaveTime >= DateTime.Today)) && !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName) && (f.PlanStatus1 == null || f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保 || f.PlanName2 == null || f.PlanStatus2 != UserEntryPlanStatusEnumType.未参保));
        //    //foreach (var item in userEntryList)
        //    //{
        //    //    await _businessInsuranceRecordService.ManageUserEntryPlan(item.ID);
        //    //}

        //    //await _businessInsuranceRecordService.ManageUserEntryPlanRecord(id);


        //    return Ok();
        //}



        ///// <summary>
        ///// 商保推送增减方案
        ///// </summary>
        ///// <param name="insuranceCompany"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> BusinessInsuranceMatchJob([FromForm]string insuranceCompany)
        //{
        //    if (string.IsNullOrWhiteSpace(insuranceCompany))
        //    {
        //        insuranceCompany = "阳光保险公司";
        //    }
        //    if (insuranceCompany != "阳光保险公司")
        //    {
        //        bool isWorkDay = WorkDayHelper.IsWorkDay2025(DateTime.Today);
        //        if (!isWorkDay)
        //        {
        //            return Ok();
        //        }
        //    }
        //    else {
        //        if ( DateTime.Now.Hour >= 17)
        //        {
        //            // 需要修改参保的保险公司
        //            var dadiRecordList = await _businessInsuranceRecordService.List(f => (f.Status == BusinessInsuranceStatusEnumType.待增员匹配||f.Status== BusinessInsuranceStatusEnumType.待增员) && f.InsuranceCompany == "大地财险");
        //            if (dadiRecordList.Any())
        //            {
        //                foreach (var item in dadiRecordList)
        //                {
        //                    item.Status = BusinessInsuranceStatusEnumType.待增员匹配;
        //                    item.AddMatchRecordID = null;
        //                    item.InsuranceCompany = "阳光保险公司";
        //                    item.InsuranceId=string.Empty;
        //                    item.IsDayOrder = false;
        //                    item.PlanName = "90万+9万";
        //                }
        //                await _businessInsuranceRecordService.Update(dadiRecordList);
        //            }
        //        }
        //    }



        //    var today = DateTime.Today;
        //    var nextDay = today.AddDays(1);
        //    var orderPlans = await _orderService.ListOrderPlanView(f => f.StartDate <= nextDay && f.EndDate >= nextDay && f.InsuranceCompany == insuranceCompany);

        //    var insuranceIdList = orderPlans.Select(f => f.InsuranceId).ToList();



        //    var dayOrderPlans = orderPlans.Where(f => f.OrderType == (int)InsuranceOrderEnumType.日保单).ToList();
        //    var monthOrderPlans = orderPlans.Where(f => f.OrderType == (int)InsuranceOrderEnumType.月保单).ToList();
        //    List<Live_BusinessInsuranceRecord> records = new List<Live_BusinessInsuranceRecord>();
        //    // 增员待匹配
        //    var addMatchList = await _businessInsuranceRecordService.List(f =>  f.Status == BusinessInsuranceStatusEnumType.待增员匹配 && f.InsuranceCompany == insuranceCompany);
        //    // 月保单减员
        //    var delMatchList = await _businessInsuranceRecordService.List(f => insuranceIdList.Contains(f.InsuranceId) && f.Status == BusinessInsuranceStatusEnumType.待减员匹配 && f.InsuranceCompany == insuranceCompany && f.IsDayOrder == false);
        //    // 增员身份证号码集合
        //    var addIdCardNoList = addMatchList.Select(f => f.IdCardNo).Distinct().ToList();
        //    // 减员身份证号码集合
        //    var delIdCardNoList = delMatchList.Select(f => f.IdCardNo).Distinct().ToList();
        //    // 必须减员的身份证号码集合
        //    var sameIdCardNoList = addIdCardNoList.Intersect(delIdCardNoList).ToList();

        //    foreach (var planItem in monthOrderPlans)
        //    {
        //        // 默认推送
        //        if (planItem.IsDefaultPush)
        //        {
        //            var addList = addMatchList.Where(f => f.PlanName == planItem.GnteeName && f.InsuranceCompany == planItem.InsuranceCompany && f.IsDefaultPush).ToList();
        //            if (addList != null && addList.Any())
        //            {
        //                foreach (var addItem in addList)
        //                {
        //                    addItem.StartDate = planItem.StartDate;
        //                    addItem.EndDate = planItem.EndDate;
        //                    addItem.InsuranceNo = planItem.InsuranceNo;
        //                    addItem.InsuranceId = planItem.InsuranceId;
        //                    addItem.PlanNameId = planItem.GnteeId;
        //                    addItem.IsDayOrder = false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var addList = addMatchList.Where(f => f.PlanName == planItem.GnteeName && f.InsuranceCompany == planItem.InsuranceCompany && f.IsDefaultPush == false && f.InsuranceSigningUnitName == planItem.InsuranceSingningUnitName).ToList();
        //            if (addList != null && addList.Any())
        //            {
        //                foreach (var addItem in addList)
        //                {
        //                    addItem.StartDate = planItem.StartDate;
        //                    addItem.EndDate = planItem.EndDate;
        //                    addItem.InsuranceNo = planItem.InsuranceNo;
        //                    addItem.InsuranceId = planItem.InsuranceId;
        //                    addItem.PlanNameId = planItem.GnteeId;
        //                    addItem.IsDayOrder = false;
        //                }
        //            }
        //        }
        //    }
        //    #region 优先处理身份证号码相同的
        //    if (sameIdCardNoList != null && sameIdCardNoList.Any())
        //    {
        //        var sameIdCardNoAddList = addMatchList.Where(f => sameIdCardNoList.Contains(f.IdCardNo)).ToList();
        //        // 必须减员
        //        var sameIdCardNoDelList = delMatchList.Where(f => sameIdCardNoList.Contains(f.IdCardNo)).ToList();

        //        foreach (var addItem in sameIdCardNoAddList)
        //        {
        //            // 方案相同的
        //            if (delMatchList.Any(f =>
        //            f.Status == BusinessInsuranceStatusEnumType.待减员匹配
        //            && f.InsuranceId == addItem.InsuranceId
        //           && f.IdCardNo == addItem.IdCardNo
        //            && f.CustomerName == addItem.CustomerName
        //            && f.InsuranceSigningUnitName == addItem.InsuranceSigningUnitName
        //            && f.WorkName == addItem.WorkName
        //            && f.WorkClass == addItem.WorkClass))
        //            {
        //                var delItem = delMatchList.First(f =>
        //             f.Status == BusinessInsuranceStatusEnumType.待减员匹配
        //            && f.InsuranceId == addItem.InsuranceId
        //            && f.IdCardNo == addItem.IdCardNo
        //             && f.CustomerName == addItem.CustomerName
        //             && f.InsuranceSigningUnitName == addItem.InsuranceSigningUnitName
        //             && f.WorkName == addItem.WorkName
        //             && f.WorkClass == addItem.WorkClass);

        //                addItem.AddPushTime = delItem.AddPushTime;
        //                addItem.Status = BusinessInsuranceStatusEnumType.已增员;
        //                addItem.AddMatchRecordID = delItem.ID;
        //                delItem.DelMatchRecordID = addItem.ID;
        //                delItem.Status = BusinessInsuranceStatusEnumType.已减员;
        //            }
        //            else if (delMatchList.Any(f =>
        //            f.InsuranceId == addItem.InsuranceId
        //            && f.Status == BusinessInsuranceStatusEnumType.待减员匹配
        //            && f.IdCardNo == addItem.IdCardNo
        //            ))
        //            {
        //                var delItem = delMatchList.First(f =>
        //                 f.Status == BusinessInsuranceStatusEnumType.待减员匹配
        //                 && f.InsuranceId == addItem.InsuranceId
        //                 && f.IdCardNo == addItem.IdCardNo);

        //                addItem.AddPushTime = delItem.AddPushTime;
        //                addItem.Status = BusinessInsuranceStatusEnumType.待增员;
        //                addItem.AddMatchRecordID = delItem.ID;
        //                delItem.DelMatchRecordID = addItem.ID;
        //                delItem.Status = BusinessInsuranceStatusEnumType.待减员;
        //            }

        //        }
        //    }
        //    #endregion
        //    foreach (var addItem in addMatchList)
        //    {
        //        if (addItem.Status!= BusinessInsuranceStatusEnumType.待增员匹配)
        //        {
        //            continue;
        //        }
        //        // 方案相同的
        //        if (delMatchList.Any(f =>
        //        f.InsuranceId == addItem.InsuranceId
        //         && f.Status == BusinessInsuranceStatusEnumType.待减员匹配
        //        ))
        //        {
        //            var delItem = delMatchList.First(f =>
        //                f.InsuranceId == addItem.InsuranceId
        //             && f.Status == BusinessInsuranceStatusEnumType.待减员匹配);

        //            addItem.AddPushTime = delItem.AddPushTime;
        //            addItem.Status = BusinessInsuranceStatusEnumType.待增员;
        //            addItem.AddMatchRecordID = delItem.ID;
        //            delItem.DelMatchRecordID = addItem.ID;
        //            delItem.Status = BusinessInsuranceStatusEnumType.待减员;
        //        }
        //        else
        //        {


        //            if (insuranceCompany != "大地财险")
        //            {
        //                addItem.Status = BusinessInsuranceStatusEnumType.待增员;
        //                if (addItem.IsDefaultPush)
        //                {
        //                    if (dayOrderPlans.Any(f => f.IsDefaultPush == true && f.GnteeName == addItem.PlanName))
        //                    {
        //                        var planItem = dayOrderPlans.First(f => f.IsDefaultPush == true && f.GnteeName == addItem.PlanName);

        //                        addItem.StartDate = planItem.StartDate;
        //                        addItem.EndDate = planItem.EndDate;
        //                        addItem.InsuranceNo = planItem.InsuranceNo;
        //                        addItem.InsuranceId = planItem.InsuranceId;
        //                        addItem.PlanNameId = planItem.GnteeId;
        //                        addItem.IsDayOrder = true;
        //                    }
        //                }
        //                else
        //                {

        //                    if (dayOrderPlans.Any(f => f.IsDefaultPush == false && f.InsuranceSingningUnitName == addItem.InsuranceSigningUnitName && f.GnteeName == addItem.PlanName))
        //                    {
        //                        var planItem = dayOrderPlans.First(f => f.IsDefaultPush == false && f.InsuranceSingningUnitName == addItem.InsuranceSigningUnitName && f.GnteeName == addItem.PlanName);
        //                        addItem.StartDate = planItem.StartDate;
        //                        addItem.EndDate = planItem.EndDate;
        //                        addItem.InsuranceNo = planItem.InsuranceNo;
        //                        addItem.InsuranceId = planItem.InsuranceId;
        //                        addItem.PlanNameId = planItem.GnteeId;
        //                        addItem.IsDayOrder = true;

        //                    }
        //                }
        //            }
        //            else {

        //                //addItem.Status = BusinessInsuranceStatusEnumType.待增员匹配;
        //            }

        //        }
        //    }
        //    // 相同身份证号码 如果减员未匹配
        //    if (sameIdCardNoList != null && sameIdCardNoList.Any())
        //    {
        //        var mastDelList = delMatchList.Where(f => sameIdCardNoList.Contains(f.IdCardNo) && f.Status == BusinessInsuranceStatusEnumType.待减员匹配);
        //        foreach (var item in mastDelList)
        //        {
        //            item.Status = BusinessInsuranceStatusEnumType.待减员;
        //        }
        //    }
        //    // 获取所有日保单减员
        //    var dayRecordList = await _businessInsuranceRecordService.List(f => f.IsDayOrder == true && f.Status == BusinessInsuranceStatusEnumType.待减员匹配 && f.InsuranceCompany == insuranceCompany);
        //    records.AddRange(addMatchList);
        //    if (dayRecordList.Any())
        //    {
        //        foreach (var item in dayRecordList)
        //        {
        //            item.Status = BusinessInsuranceStatusEnumType.待减员;
        //        }
        //        records.AddRange(dayRecordList);
        //    }
        //    if (delMatchList.Any(f => f.Status == BusinessInsuranceStatusEnumType.待减员 || f.Status == BusinessInsuranceStatusEnumType.已减员))
        //    {
        //        records.AddRange(delMatchList.Where(f => f.Status == BusinessInsuranceStatusEnumType.待减员 || f.Status == BusinessInsuranceStatusEnumType.已减员).ToList());
        //    }
        //    // 匹配好的饿数据更新到数据库
        //    //await   _businessInsuranceRecordService.Update(records);
        //    // 将已参保、已减员的记录，更新状态
        //    var groups = records.GroupBy(f => new { f.InsuranceId, f.InsuranceCompany, f.PlanName, f.PlanNameId }).ToList();
        //    List<Live_BusinessInsurancePush> pushList = new List<Live_BusinessInsurancePush>();
        //    foreach (var item in groups)
        //    {
        //        // 增员
        //        var addList = item.Where(f => f.Status == BusinessInsuranceStatusEnumType.待增员 || f.Status == BusinessInsuranceStatusEnumType.已增员 ).ToList();
        //        // 减员
        //        var delList = item.Where(f => f.Status == BusinessInsuranceStatusEnumType.待减员 || f.Status == BusinessInsuranceStatusEnumType.已减员).ToList();
        //        Live_BusinessInsurancePush pushRecord = new Live_BusinessInsurancePush()
        //        {
        //            ID = Guid.NewGuid(),
        //            AddFailCount = 0,
        //            AddIndecaseId = string.Empty,
        //            AddPushStatus = InsurancePushStatusEnumType.待推送,
        //            AddPushTime = DateTime.MinValue,
        //            AddRecordCount = addList.Count,
        //            AddResCode = 0,
        //            AddResMsg = string.Empty,
        //            CreateTime = DateTime.Now,
        //            DelFailCount = 0,
        //            DelIndecaseId = string.Empty,
        //            DelPushStatus = InsurancePushStatusEnumType.待推送,
        //            DelRecordCount = delList.Count,
        //            DelPushTime = DateTime.MinValue,
        //            DelResCode = 0,
        //            DelResMsg = string.Empty,
        //            InsuranceId = item.Key.InsuranceId,
        //            PlanName = item.Key.PlanName,
        //            PlanNameId = item.Key.PlanNameId,
        //            InsuranceCompany = item.Key.InsuranceCompany
        //        };
        //        foreach (var recordItem in item)
        //        {
        //            recordItem.BusinessInsurancePushID = pushRecord.ID;
        //        }
        //        pushList.Add(pushRecord);
        //    }
        //    DbScoped.SugarScope.BeginTran();
        //    await _pushService.Add(pushList);
        //    await _recordService.Update(records);
        //    DbScoped.SugarScope.CommitTran();

        //    return Ok();
        //}
        //[HttpPost]
        //public async Task<IActionResult> BusinessInsurancePushJob([FromForm]string insuranceCompany)
        //{
        //    if (string.IsNullOrWhiteSpace(insuranceCompany))
        //    {
        //        insuranceCompany = "阳光保险公司";
        //    }
        //    if (insuranceCompany!= "阳光保险公司")
        //    {
        //      bool isWorkDay=  WorkDayHelper.IsWorkDay2025(DateTime.Today);
        //        if (!isWorkDay)
        //        {
        //            return Ok();
        //        }
        //    }

        //    if (insuranceCompany != "大地财险")
        //    {
        //        var delPushList = await _pushService.List(f => f.CreateTime >= DateTime.Today && f.InsuranceCompany == insuranceCompany && f.DelPushStatus == InsurancePushStatusEnumType.待推送);

        //        // 减员推送
        //        foreach (var recordItem in delPushList)
        //        {
        //            var pushItem = await _pushService.Get(f => f.ID == recordItem.ID);
        //            pushItem.DelPushTime = DateTime.Now;

        //            var records = await _businessInsuranceRecordService.List(f => f.BusinessInsurancePushID == recordItem.ID && f.Status == BusinessInsuranceStatusEnumType.待减员);
        //            if (records.Any())
        //            {
        //                BatchDelEmployeeRequest request = new BatchDelEmployeeRequest()
        //                {
        //                    InsureId = pushItem.InsuranceId,
        //                    CustomerList = records.Select(f => new DelCustomerinfo()
        //                    {

        //                        Custemername = f.Name,
        //                        Custemernum = f.IdCardNo,
        //                    }).ToList()
        //                };
        //                var resp = await _insuranceApiService.BatchDelEmployee(request);
        //                if (resp != null)
        //                {
        //                    pushItem.DelIndecaseId = resp.IndecaseId;
        //                    pushItem.DelResCode = resp.Code;
        //                    pushItem.DelResMsg = resp.Msg;
        //                    if (resp.Code == 200)
        //                    {
        //                        pushItem.DelPushStatus = InsurancePushStatusEnumType.已推送;
        //                        foreach (var item in records)
        //                        {
        //                            item.Status = BusinessInsuranceStatusEnumType.已减员;
        //                        }
        //                        if (resp.FailList != null && resp.FailList.Count > 0)
        //                        {
        //                            foreach (var item in resp.FailList)
        //                            {
        //                                var failRecord = records.First(f => f.IdCardNo == item.Customerinfo.Custemernum && f.Name == item.Customerinfo.Custemername);
        //                                failRecord.Status = BusinessInsuranceStatusEnumType.减员失败;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        pushItem.DelPushStatus = InsurancePushStatusEnumType.推送失败;
        //                        foreach (var item in records)
        //                        {
        //                            item.Status = BusinessInsuranceStatusEnumType.减员失败;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    pushItem.DelPushStatus = InsurancePushStatusEnumType.推送失败;
        //                    pushItem.DelIndecaseId = string.Empty;
        //                    pushItem.DelResCode = 0;
        //                    pushItem.DelResMsg = "响应信息为：null";
        //                }
        //                DbScoped.SugarScope.BeginTran();
        //                await _businessInsuranceRecordService.Update(records);
        //                await _pushService.Update(pushItem);
        //                DbScoped.SugarScope.CommitTran();
        //            }
        //            else
        //            {
        //                pushItem.DelPushStatus = InsurancePushStatusEnumType.取消推送;
        //                await _pushService.Update(pushItem);
        //            }
        //        }
        //        var addPushList = await _pushService.List(f => f.CreateTime >= DateTime.Today && f.InsuranceCompany == insuranceCompany && f.AddPushStatus == InsurancePushStatusEnumType.待推送);
        //        // 增员推送
        //        foreach (var recordItem in addPushList)
        //        {
        //            var pushItem = await _pushService.Get(f => f.ID == recordItem.ID);
        //            pushItem.AddPushTime = DateTime.Now;
        //            var records = await _businessInsuranceRecordService.List(f => f.BusinessInsurancePushID == recordItem.ID && f.Status == BusinessInsuranceStatusEnumType.待增员);
        //            if (records.Any())
        //            {
        //                BatchAddEmployeeRequest request = new BatchAddEmployeeRequest()
        //                {
        //                    InsureId = recordItem.InsuranceId,
        //                    GnteeId = pushItem.PlanNameId,
        //                    GnteeName = pushItem.PlanName,
        //                    CustomerList = records.Select(f => new AddCustomerinfo()
        //                    {
        //                        Custemername = f.Name,
        //                        Custemernum = f.IdCardNo,
        //                        Custemerage = f.Age.ToString(),
        //                        Custemersex = f.Sex,
        //                        Custemertype = "身份证",
        //                        Guyuangongzhong = f.WorkName,
        //                        Zhiyeleibei = f.WorkClass.Substring(0, 1),
        //                        Ldhtdw = f.InsuranceSigningUnitName,
        //                        Sjygdw = f.CustomerName,
        //                    }).ToList()
        //                };
        //                var resp = await _insuranceApiService.BatchAddEmployee(request);
        //                if (resp != null)
        //                {
        //                    pushItem.AddIndecaseId = resp.IndecaseId;
        //                    pushItem.AddResCode = resp.Code;
        //                    pushItem.AddResMsg = resp.Msg;
        //                    if (resp.Code == 200)
        //                    {
        //                        pushItem.AddPushStatus = InsurancePushStatusEnumType.已推送;
        //                        foreach (var item in records)
        //                        {
        //                            item.Status = BusinessInsuranceStatusEnumType.已增员;
        //                        }
        //                        if (resp.FailList != null && resp.FailList.Count > 0)
        //                        {
        //                            foreach (var item in resp.FailList)
        //                            {
        //                                var failRecord = records.First(f => f.IdCardNo == item.Customerinfo.Custemernum && f.Name == item.Customerinfo.Custemername);
        //                                failRecord.Status = BusinessInsuranceStatusEnumType.增员失败;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        pushItem.AddPushStatus = InsurancePushStatusEnumType.推送失败;
        //                        foreach (var item in records)
        //                        {
        //                            item.Status = BusinessInsuranceStatusEnumType.减员失败;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    pushItem.AddPushStatus = InsurancePushStatusEnumType.推送失败;
        //                    pushItem.AddIndecaseId = string.Empty;
        //                    pushItem.AddResCode = 0;
        //                    pushItem.AddResMsg = "响应信息为：null";
        //                }
        //                DbScoped.SugarScope.BeginTran();
        //                await _businessInsuranceRecordService.Update(records);
        //                await _pushService.Update(pushItem);
        //                DbScoped.SugarScope.CommitTran();
        //            }
        //            else
        //            {
        //                pushItem.DelPushStatus = InsurancePushStatusEnumType.取消推送;
        //                await _pushService.Update(pushItem);
        //            }
        //        }
        //    }
        //    else {


        //        var pushList = await _pushService.List(f => f.CreateTime >= DateTime.Today && f.InsuranceCompany == insuranceCompany && (f.DelPushStatus == InsurancePushStatusEnumType.待推送||f.AddPushStatus== InsurancePushStatusEnumType.待推送));
        //        if (pushList.Any())
        //        {
        //            foreach (var item in pushList)
        //            {
        //                item.AddPushStatus = InsurancePushStatusEnumType.已推送;
        //                item.DelPushStatus = InsurancePushStatusEnumType.已推送;
        //            }
        //            var pushIdList = pushList.Select(f => f.ID).ToList();
        //            var records = await _businessInsuranceRecordService.List(f => pushIdList.Contains(f.BusinessInsurancePushID.Value) && (f.Status == BusinessInsuranceStatusEnumType.待增员 || f.Status == BusinessInsuranceStatusEnumType.待减员));
        //            var pushRecords = records.Select(f => new DDCXPushExcelTemplate()
        //            {
        //                Age = f.Age.ToString(),
        //                ChangeType = f.Status == BusinessInsuranceStatusEnumType.待增员 ? "新增" : "删除",
        //                CustomerName = f.CustomerName,
        //                IdCardNo = f.IdCardNo,
        //                Name = f.Name,
        //                StartDate = DateTime.Today.AddDays(1).ToString("yyyy年MM月dd日0时"),
        //                WorkName = f.WorkName,
        //            }).ToList();

        //            var addCount = records.Count(f => f.Status == BusinessInsuranceStatusEnumType.待增员);
        //            var delCount = records.Count(f => f.Status == BusinessInsuranceStatusEnumType.待减员);

        //            var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "Sheet1");

        //            using (var ms = new MemoryStream(fileBytes))
        //            {

        //                string content = $"批单生效日期：{DateTime.Today.AddDays(1).ToString("yyyy年MM月dd日0时")},新增：{addCount}人，删除：{delCount}人";

        //                var res = await _aliyunOSSService.UploadFile(ms, "11.xlsx", false, true);
        //                //_aliSendInfo.SendAliEmail("xuyuannb@ccic-net.com.cn", $"大地财险（{DateTime.Today.ToString("yyyy-MM-dd")}）", content, "", $"/uploadfiles/{res.Data}");
        //                //_aliSendInfo.SendAliEmail("zhulynb@ccic-net.com.cn", $"大地财险（{DateTime.Today.ToString("yyyy-MM-dd")}）", content, "", $"/uploadfiles/{res.Data}");
        //                _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", $"大地财险（{DateTime.Today.ToString("yyyy-MM-dd")}）", content, "", $"/uploadfiles/{res.Data}");
        //                _aliSendInfo.SendAliEmail("15261300969@163.com", $"大地财险（{DateTime.Today.ToString("yyyy-MM-dd")}）", content, "", $"/uploadfiles/{res.Data}");
        //            }
        //            foreach (var item in records)
        //            {
        //                if (item.Status == BusinessInsuranceStatusEnumType.待增员)
        //                {
        //                    item.Status = BusinessInsuranceStatusEnumType.已增员;
        //                }
        //                else
        //                {
        //                    item.Status = BusinessInsuranceStatusEnumType.已减员;
        //                }
        //            }
        //        }
        //        else {
        //            _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", $"大地财险_无增减（{DateTime.Today.ToString("yyyy-MM-dd")}）", "", "", "");
        //        }




        //    }


        //    return Ok();
        //}
        //[HttpPost]
        //public async Task<IActionResult> BusinessInsurancePlanStatusJob([FromForm]string insuranceCompany)
        //{

        //    if (string.IsNullOrWhiteSpace(insuranceCompany))
        //    {
        //        insuranceCompany = "阳光保险公司";
        //    }

        //    if (insuranceCompany == "中国大地财险公司")
        //    {
        //        bool isWorkDay = WorkDayHelper.IsWorkDay2025(DateTime.Today);


        //        if (!isWorkDay)
        //        {
        //            return Ok();
        //        }
        //    }

        //    var delPushList = await _pushService.List(f => f.DelPushTime >= DateTime.Today && f.InsuranceCompany == insuranceCompany && f.DelPushStatus == InsurancePushStatusEnumType.已推送);
        //    var delPushIdList = delPushList.Select(f => f.ID).ToList();
        //    var delRecordList = await _recordService.List(f => delPushIdList.Contains(f.BusinessInsurancePushID.Value));
        //    var delEntryIdList = delRecordList.Select(f => f.UserEntryID).Distinct();
        //    var delEntryList = await _entryService.List(f => delEntryIdList.Contains(f.ID));
        //    foreach (var item in delRecordList)
        //    {
        //        var entry = delEntryList.FirstOrDefault(f => f.ID == item.UserEntryID);
        //        if (entry != null)
        //        {
        //            if (item.IsMain)
        //            {
        //                if (item.Status == BusinessInsuranceStatusEnumType.已减员)
        //                {
        //                    entry.PlanStatus1 = UserEntryPlanStatusEnumType.已减员;
        //                }
        //                else if (item.Status == BusinessInsuranceStatusEnumType.减员失败)
        //                {
        //                    entry.PlanStatus1 = UserEntryPlanStatusEnumType.减员异常;
        //                }
        //            }
        //            else
        //            {
        //                if (item.Status == BusinessInsuranceStatusEnumType.已减员)
        //                {
        //                    entry.PlanStatus2 = UserEntryPlanStatusEnumType.已减员;
        //                }
        //                else if (item.Status == BusinessInsuranceStatusEnumType.减员失败)
        //                {
        //                    entry.PlanStatus2 = UserEntryPlanStatusEnumType.减员异常;
        //                }
        //            }

        //        }
        //    }
        //    await _entryService.UpdateBusInsurance(delEntryList);
        //    var addPushList = await _pushService.List(f => f.AddPushTime >= DateTime.Today && f.InsuranceCompany == insuranceCompany && f.AddPushStatus == InsurancePushStatusEnumType.已推送);
        //    var addPushIdList = addPushList.Select(f => f.ID).ToList();
        //    var addRecordList = await _recordService.List(f => addPushIdList.Contains(f.BusinessInsurancePushID.Value));
        //    var addEntryIdList = addRecordList.Select(f => f.UserEntryID).Distinct();
        //    var addEntryList = await _entryService.List(f => addEntryIdList.Contains(f.ID));
        //    foreach (var item in addRecordList)
        //    {
        //        var entry = addEntryList.FirstOrDefault(f => f.ID == item.UserEntryID);
        //        if (entry != null)
        //        {
        //            if (item.IsMain)
        //            {
        //                if (item.Status == BusinessInsuranceStatusEnumType.已增员)
        //                {
        //                    entry.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
        //                }
        //                else if (item.Status == BusinessInsuranceStatusEnumType.增员失败)
        //                {
        //                    entry.PlanStatus1 = UserEntryPlanStatusEnumType.增员异常;
        //                }
        //            }
        //            else
        //            {
        //                if (item.Status == BusinessInsuranceStatusEnumType.已增员)
        //                {
        //                    entry.PlanStatus2 = UserEntryPlanStatusEnumType.已增员;
        //                }
        //                else if (item.Status == BusinessInsuranceStatusEnumType.增员失败)
        //                {
        //                    entry.PlanStatus2 = UserEntryPlanStatusEnumType.增员异常;
        //                }
        //            }

        //        }
        //    }
        //    await _entryService.UpdateBusInsurance(addEntryList);




        //    return Ok();
        //}
        //[HttpPost]
        //public async Task<IActionResult> BusinessInsuranceEmailJob([FromForm] string insuranceCompany)
        //{
        //    List<Live_BusinessInsuranceRecord> records = new List<Live_BusinessInsuranceRecord>();
        //    var addRecordList = await _recordService.List(f =>f.InsuranceCompany== insuranceCompany&& f.AddPushTime.HasValue && f.AddPushTime.Value >= DateTime.Today && f.Status == BusinessInsuranceStatusEnumType.增员失败);
        //    var delRecordList = await _recordService.List(f =>f.InsuranceCompany==insuranceCompany&& f.DelPushTime.HasValue && f.DelPushTime.Value >= DateTime.Today && f.Status == BusinessInsuranceStatusEnumType.减员失败);


        //    var noPushRecordList = await _recordService.List(f => f.Status== BusinessInsuranceStatusEnumType.待增员匹配||f.Status== BusinessInsuranceStatusEnumType.待增员);


        //    string msg = string.Empty;

        //    if (addRecordList.Any())
        //    {
        //        msg += $"增员异常名单：{string.Join(',', addRecordList.Select(f => f.Name))}\r\n";
        //    }
        //    if (delRecordList.Any())
        //    {
        //        msg += $"减员异常名单：{string.Join(',', delRecordList.Select(f => f.Name))}\r\n";
        //    }

        //    if (noPushRecordList.Any())
        //    {
        //        msg += $"待增员未推送名单：{string.Join(',', noPushRecordList.Select(f => f.Name))}\r\n";
        //    }


        //    var configList = await _configService.ListView(f => f.IsEnabled&&f.IsMain);
        //    foreach (var item in configList)
        //    {
        //        var entryList = await _entryService.List(f => f.CustomerGID == item.CustomerGID&& f.Status == 2 && f.PlanStatus1 != UserEntryPlanStatusEnumType.已增员);

        //        if (entryList.Any())
        //        {
        //            msg += $"{item.CustomerName}--未增员名单，{string.Join(',', entryList.Select(f=>$"{f.Name}（{f.PlanStatus1}）"))}\r\n";
        //        }
        //    }



        //    if (!string.IsNullOrWhiteSpace(msg))
        //    {
        //        _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "商保增减异常通知", msg, "", "");
        //    }
        //    else {
        //        _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "商保推送完成通知", "无增减推送异常人员", "", "");
        //    }
        //    return Ok();
        //}


        ///// <summary>
        /////  根据保全名单生成系统初始参保记录
        ///// </summary>
        ///// <param name="insuranceCompany"></param>
        ///// <returns></returns>
        ///// <exception cref="ErrorCodeException"></exception>
        //[HttpPost]
        //public async Task<IActionResult> GenerateUserEntryInitialRecordJob([FromForm] string insuranceCompany)
        //{
        //    List<string> insuranceCompanyList = new List<string>();
        //    if (insuranceCompany == "阳光保险公司")
        //    {
        //        insuranceCompanyList.Add(insuranceCompany);
        //        //insuranceCompanyList.Add("大地财险");
        //    }
        //    var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
        //    var insuranceMonth = currentMonth.AddMonths(0);
        //    var currentMonthEndDay = insuranceMonth.AddDays(-1);
        //    var timeNode = new DateTime(currentMonth.Year, currentMonth.Month, 24);
        //    var orderPlans = await _orderService.ListOrderPlanView(f => f.InsuranceCompany == insuranceCompany && f.StartDate >= insuranceMonth);
        //    var configList = await _configService.ListView(f => f.IsEnabled && insuranceCompanyList.Contains(f.InsuranceCompany));
        //    await _initialRecordService.Delete(f => f.DataType == BusInsuranceInitialDataTypeEnumType.系统生成 && f.InsuranceMonth == insuranceMonth && f.InsuranceCompany == insuranceCompany);
        //    var recordList = await _initialRecordService.List(f => f.InsuranceMonth == insuranceMonth && f.InsuranceCompany == insuranceCompany);
        //    List<Live_V_UserEntry> userEntryList = null;
        //    //Live_V_UserEntry userEntry = null;
        //    List<Live_BusinessInsuranceInitialRecord> initialRecordList = new List<Live_BusinessInsuranceInitialRecord>();
        //    Live_BusinessInsuranceInitialRecord initialRecord = null;
        //    foreach (var configItem in configList)
        //    {
        //        // 在职且围在保全名单则增员
        //        var exp = Expressionable.Create<Live_V_UserEntry>();
        //        exp.And(f => f.CustomerGID == configItem.CustomerGID);
        //        exp.And(f => f.PlanStatus1 == null || f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保);
        //        exp.And(f => f.PlanName1 == "90万+9万" || f.PlanName1 == "80万+8万");
        //        exp.And(f => f.EntryTime != null && f.EntryTime.Value.Date < insuranceMonth && f.EntryTime.Value.Date >= timeNode && f.Status == 2);
        //        userEntryList = await _entryService.ListView(exp.ToExpression());
        //        var customerInitialRecord = recordList.Where(f => f.CustomerName == configItem.CustomerName).ToList();
        //        foreach (var entryItem in userEntryList)
        //        {
        //            initialRecord = customerInitialRecord.FirstOrDefault(f => f.IdCardNo == entryItem.CardID);
        //            if (initialRecord == null)
        //            {
        //                initialRecord = new Live_BusinessInsuranceInitialRecord()
        //                {
        //                    ActionType = BusInsuranceInitialActionTypeEnumType.待增员,
        //                    CustomerName = configItem.CustomerName,
        //                    DataType = BusInsuranceInitialDataTypeEnumType.系统生成,
        //                    CreateTime = DateTime.Now,
        //                    EntryDate = entryItem.EntryTime,
        //                    ID = Guid.NewGuid(),
        //                    IdCardNo = entryItem.CardID,
        //                    InsuranceCompany = "阳光保险公司",
        //                    InsuranceId = string.Empty,
        //                    InsuranceMonth = insuranceMonth,
        //                    InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
        //                    Msg = "系统在职，未在保全名单中，系统生成待增员记录",
        //                    Name = entryItem.Name,
        //                    PlanName = "90万+9万",
        //                    Remark = string.Empty,
        //                    Status = BusInsuranceInitialStatusEnumType.待处理,
        //                    UserEntryID = entryItem.ID,
        //                    WorkClass = entryItem.WorkClass,
        //                    WorkName = entryItem.WorkName,
        //                    IsMain = configItem.IsMain,
        //                    IsDefaultPush = configItem.IsDefaultPush,
        //                    IsDayOrder = false,
        //                    EndDate = DateTime.MinValue,
        //                    StartDate = DateTime.MinValue,
        //                };
        //                initialRecordList.Add(initialRecord);
        //            }
        //            else
        //            {
        //                initialRecord.UserEntryID = entryItem.ID;
        //                if (initialRecord.InsuranceSigningUnitName == configItem.InsuranceSigningUnitName && initialRecord.WorkClass == entryItem.WorkClass && initialRecord.WorkName == entryItem.WorkName && initialRecord.PlanName == initialRecord.PlanName)
        //                {
        //                    //  已增员
        //                    initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
        //                    initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.已增员;
        //                }
        //                else
        //                {
        //                    // 先减员后增员
        //                    initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
        //                    initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
        //                    initialRecord.Msg = "参保信息不匹配，先减员后增其中的减员记录";
        //                    var addRecord = new Live_BusinessInsuranceInitialRecord()
        //                    {
        //                        ActionType = BusInsuranceInitialActionTypeEnumType.待增员,
        //                        CustomerName = configItem.CustomerName,
        //                        DataType = BusInsuranceInitialDataTypeEnumType.系统生成,
        //                        CreateTime = DateTime.Now,
        //                        EntryDate = entryItem.EntryTime,
        //                        ID = Guid.NewGuid(),
        //                        IdCardNo = entryItem.CardID,
        //                        InsuranceCompany = "阳光保险公司",
        //                        InsuranceId = string.Empty,
        //                        InsuranceMonth = insuranceMonth,
        //                        InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
        //                        Msg = "参保信息不匹配，先减员后增其中的增员记录",
        //                        Name = entryItem.Name,
        //                        PlanName = "90万+9万",
        //                        Remark = string.Empty,
        //                        Status = BusInsuranceInitialStatusEnumType.待处理,
        //                        UserEntryID = entryItem.ID,
        //                        WorkClass = entryItem.WorkClass,
        //                        WorkName = entryItem.WorkName,
        //                        IsMain = configItem.IsMain,
        //                        IsDefaultPush = configItem.IsDefaultPush,
        //                    };
        //                    initialRecordList.Add(addRecord);
        //                }
        //            }
        //        }




        //        foreach (var item in customerInitialRecord)
        //        {

        //            initialRecord = item;

        //            if (item.Status == BusInsuranceInitialStatusEnumType.待处理)
        //            {
        //                continue;
        //            }
        //            var entry = await _entryService.Get(f => f.ID == item.UserEntryID);

        //            if (entry != null)
        //            {
        //                if (entry.Status == 2)
        //                {

        //                    if (initialRecord.InsuranceSigningUnitName == configItem.InsuranceSigningUnitName && initialRecord.WorkClass == entry.WorkClass && initialRecord.WorkName == entry.WorkName && initialRecord.PlanName == entry.PlanName1)
        //                    {
        //                        //  已增员
        //                        initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
        //                        initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.已增员;
        //                    }
        //                    else
        //                    {
        //                        // 先减员后增员
        //                        initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
        //                        initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
        //                        initialRecord.Msg = "参保信息不匹配，先减员后增其中的减员记录";
        //                        var addRecord = new Live_BusinessInsuranceInitialRecord()
        //                        {
        //                            ActionType = BusInsuranceInitialActionTypeEnumType.待增员,
        //                            CustomerName = configItem.CustomerName,
        //                            DataType = BusInsuranceInitialDataTypeEnumType.系统生成,
        //                            CreateTime = DateTime.Now,
        //                            EntryDate = entry.EntryTime,
        //                            ID = Guid.NewGuid(),
        //                            IdCardNo = entry.CardID,
        //                            InsuranceCompany = "阳光保险公司",
        //                            InsuranceId = string.Empty,
        //                            InsuranceMonth = insuranceMonth,
        //                            InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
        //                            Msg = "参保信息不匹配，先减员后增其中的增员记录",
        //                            Name = entry.Name,
        //                            PlanName = configItem.IsMain ? entry.PlanName1 : entry.PlanName2,
        //                            Remark = string.Empty,
        //                            Status = BusInsuranceInitialStatusEnumType.待处理,
        //                            UserEntryID = entry.ID,
        //                            WorkClass = entry.WorkClass,
        //                            WorkName = entry.WorkName,
        //                            IsMain = configItem.IsMain,
        //                            IsDefaultPush = configItem.IsDefaultPush,
        //                        };
        //                        initialRecordList.Add(addRecord);
        //                    }
        //                }
        //                else
        //                {
        //                    item.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
        //                    item.Status = BusInsuranceInitialStatusEnumType.待处理;
        //                }
        //            }
        //            else
        //            {

        //                item.UserEntryID = 0;
        //            }

        //        }

        //        foreach (var record in customerInitialRecord)
        //        {
        //            record.IsMain = configItem.IsMain;
        //            record.IsDefaultPush = configItem.IsDefaultPush;
        //            if (record.UserEntryID <= 0)
        //            {
        //                record.Status = BusInsuranceInitialStatusEnumType.匹配失败;
        //                record.ActionType = BusInsuranceInitialActionTypeEnumType.人工处理;
        //                record.Msg = "未匹配到入离职记录，请人工核对";
        //            }
        //        }
        //    }
        //    if (initialRecordList.Any())
        //    {
        //        var faillRecordList = initialRecordList.Where(f => string.IsNullOrWhiteSpace(f.PlanName) || string.IsNullOrWhiteSpace(f.WorkName) || string.IsNullOrWhiteSpace(f.WorkClass)).ToList();
        //        if (faillRecordList.Any())
        //        {
        //            foreach (var item in faillRecordList)
        //            {
        //                item.Status = BusInsuranceInitialStatusEnumType.匹配失败;
        //                item.ActionType = BusInsuranceInitialActionTypeEnumType.人工处理;
        //                item.Msg = "参保方案、工种、工作类型 缺失";
        //            }
        //        }
        //        await _initialRecordService.Add(initialRecordList);
        //    }
        //    if (recordList.Any())
        //    {
        //        await _initialRecordService.Update(recordList);
        //    }

        //    return Ok();
        //}


        ///// <summary>
        /////  根据初始参保记录生成参保记录
        ///// </summary>
        ///// <param name="insuranceCompany"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> BusinessInsuranceGenerateByInitialRecordJob([FromForm] string insuranceCompany)
        //{


        //    List<BusInsuranceInitialActionTypeEnumType> actionList = new List<BusInsuranceInitialActionTypeEnumType>() { 
        //     BusInsuranceInitialActionTypeEnumType.待增员,
        //      BusInsuranceInitialActionTypeEnumType.已增员,
        //       BusInsuranceInitialActionTypeEnumType.待减员,

        //    };
        //    var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
        //    var insuranceMonth = currentMonth.AddMonths(1);
        //    List<Live_BusinessInsuranceRecord> records = new List<Live_BusinessInsuranceRecord>();
        //    Live_BusinessInsuranceRecord record = null;
        //    var initialRecordList=await _initialRecordService.List(f => actionList.Contains(f.ActionType)&&  f.InsuranceMonth == insuranceMonth&&f.Status== BusInsuranceInitialStatusEnumType.待处理);
        //   var  configList=await _configService.ListView(f => f.IsEnabled);
        //    var orderPlanList = await _orderService.ListOrderPlanView(f => f.StartDate >= insuranceMonth&&f.OrderType== (int)InsuranceOrderEnumType.月保单 );
        //    foreach (var item in initialRecordList)
        //    {


        //        var ageSex =  IdCardHelper.GetAgeAndGender(item.IdCardNo);
        //        record = new Live_BusinessInsuranceRecord()
        //        {
        //            AddMatchRecordID = null,
        //            AddPushTime = null,
        //            Status = Model.Enums.BusinessInsuranceStatusEnumType.待增员匹配,
        //            AddSysTime = DateTime.Now,
        //            Age = ageSex.Age,
        //            Sex = ageSex.Gender,
        //            CustomerGID =Guid.Empty,
        //            CustomerName = item.CustomerName,
        //            InsuranceCompany = item.InsuranceCompany,
        //            InsuranceSigningUnitName = item.InsuranceSigningUnitName,
        //            WorkClass = item.WorkClass,
        //            WorkName = item.WorkName,
        //            CreateTime =insuranceMonth,
        //            EntryDate = item.EntryDate.Value.Date,
        //            DelPushTime = null,
        //            DelSysTime = null,
        //            IdCardNo = item.IdCardNo,
        //            UserID = 0,
        //            UserEntryID = item.UserEntryID,
        //            InsuranceNo = string.Empty,
        //            DelMatchRecordID = null,
        //            InsuranceSigningUnitID = Guid.Empty,
        //            IsDayOrder = false,
        //            IsExpired = false,
        //            IsMain = item.IsMain,
        //            LeaveDate = null,
        //            Name = item.Name,
        //            OrderNo = "由保全名单初始记录生成",
        //            StartDate = new DateTime(2025,08,01),
        //            EndDate = new DateTime(2025, 08, 31),
        //            ID = Guid.NewGuid(),
        //            IsDefaultPush =item.IsDefaultPush,
        //            InsuranceId = item.InsuranceId,
        //            PlanNameId = item.PlanNameId,
        //            PlanName = item.PlanName,
        //            BusinessInsurancePushID = null,



        //        };
        //        switch (item.ActionType)
        //        {
        //            case BusInsuranceInitialActionTypeEnumType.无:
        //            case BusInsuranceInitialActionTypeEnumType.人工处理:
        //                break;
        //            case BusInsuranceInitialActionTypeEnumType.待增员:
        //                record.Status = BusinessInsuranceStatusEnumType.待增员匹配;
        //                break;
        //            case BusInsuranceInitialActionTypeEnumType.已增员:
        //                record.Status = BusinessInsuranceStatusEnumType.已增员;
        //                break;
        //            case BusInsuranceInitialActionTypeEnumType.待减员:
        //                record.Status = BusinessInsuranceStatusEnumType.待减员匹配;
        //                break;

        //            default:
        //                break;
        //        }
        //        item.Status = BusInsuranceInitialStatusEnumType.已处理;
        //        records.Add(record);
        //    }
        //    if (records.Any())
        //    {
        //      await  _businessInsuranceRecordService.Add(records);


        //        foreach (var item in records)
        //        {
        //          var entry=await  _entryService.Get(item.UserEntryID);
        //            if (entry != null)
        //            {
        //                entry.PlanName1 = item.PlanName;

        //                await _entryService.UpdateBusInsurance(entry);
        //            }
        //        }


        //    }

        //    if (initialRecordList.Any())
        //    {
        //        await _initialRecordService.Update(initialRecordList);
        //    }
        //    return Ok();
        //}

        #endregion
        #region 商保定时任务新
        ///// <summary>
        ///// 01、大地财险导入
        ///// </summary>
        ///// <param name="insuranceId"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> DDCXUpload(IFormFile file)
        //{
        //    // 当月月初
        //    var currentMonth = DateTime.Now.Date.AddDays(1 - DateTime.Today.Day);
        //    HttpContext.Request.Headers.TryGetValue("authorization", out var value);
        //    List<DdcxExcelTemplate> excelRecords = new List<DdcxExcelTemplate>();

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        file.CopyTo(ms);
        //        excelRecords = NpoiExcelHelper.ReadExcelFile<DdcxExcelTemplate>(ms, "清单");
        //    }
        //    excelRecords.AsParallel().ForAll(f => f.Count());
        //    List<Live_BusinessGuarantee> recordList = excelRecords.Select(item => new Live_BusinessGuarantee()
        //    {
        //        PlanName = "80万+8万",
        //        Name = item.Name,
        //        InsuranceCompany = "大地财险",
        //        InsuranceMonth = currentMonth,
        //        CreateTime = DateTime.Now,
        //        CustomerName = item.CustomerName,
        //        EndDate = item.EndDate,
        //        StartDate = item.StartDate,
        //        EntryTime = DateTime.MinValue,
        //        Id = 0,
        //        IdCardNo = item.IdCardNo.Replace("*",""),
        //        WorkName = item.WorkName,
        //        InsuranceId = string.Empty,
        //        PlanNameId = 0,
        //        SignUnitName = "",
        //        Status = 0,
        //        UserEntryId = 0,
        //        WorkClass = item.WorkClass,
        //        UserEntryStatus = ""
        //    }).ToList();
        //    if (recordList.Any())
        //    {
        //      await  _businessGuaranteeService.Add(recordList);
        //    }


        //    return Ok();
        //}

        ///// <summary>
        /////  大地财险匹配员工记录
        /////  员工记录修正保险信息
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> DDCXMatchUserEntry()
        //{

        //    var recordList = await _businessGuaranteeService.List(f=>f.Id>0);

        //    List<Live_UserEntry> entryList = new List<Live_UserEntry>();

        //    foreach (var item in recordList)
        //    {
        //        // 优先匹配在职记录==》 再匹配离职记录
        //        var entry = await _entryService.View(f => f.Status == 2 && f.CustomerName == item.CustomerName && f.Name == item.Name && f.CardID.StartsWith(item.IdCardNo)&&!string.IsNullOrWhiteSpace(f.WorkName) && f.WorkName == item.WorkName);
        //        if (entry == null)
        //        {
        //            entry = await _entryService.View(f => f.Status == 4 && f.CustomerName == item.CustomerName && f.Name == item.Name && f.CardID.StartsWith(item.IdCardNo) && !string.IsNullOrWhiteSpace(f.WorkName) && f.WorkName == item.WorkName);
        //        }
        //        if (entry != null)
        //        {
        //            var entryModel = await _entryService.Get(f => f.ID == entry.ID);
        //            entryModel.InsuranceCompany1 = item.InsuranceCompany;
        //            entryModel.PlanName1 = item.PlanName;
        //            if (entryModel.Status == 2)
        //            {
        //                entryModel.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
        //            }
        //            else
        //            {
        //                entryModel.PlanStatus1 = UserEntryPlanStatusEnumType.待减员;
        //            }
        //            entryList.Add(entryModel);
        //            item.UserEntryId = entry.ID;
        //            item.UserEntryStatus = $"{entry.Status}";
        //            item.Remark = string.Empty;
        //        }
        //        else
        //        {
        //            item.Remark = "未匹配到员工记录，【待减员】";
        //        }

        //    }

        //    if (entryList.Any())
        //    {
        //        foreach (var item in entryList)
        //        {
        //            await _entryService.UpdateBusInsurance(item);
        //        }
        //    }
        //    if (recordList.Any())
        //    {
        //        await _businessGuaranteeService.Update(recordList);
        //    }


        //    return Ok();
        //}
        ///// <summary>
        /////  阳光保险
        /////  从Api 获取在保数据
        ///// </summary>
        ///// <returns></returns>
        //private async Task<IActionResult> BusinessInsuranceGenerateApiRecord(bool isMatch)
        //{
        //    var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
        //    var today = DateTime.Today.AddDays(2) ;
        //    var nextDay = today.AddDays(1);
        //    var nextDayMonth = nextDay.AddDays(1 - nextDay.Day);
        //    var orderList = await _orderService.ListView(f => f.InsuranceCompany != "大地财险" && f.StartDate == nextDayMonth);
        //    var configList = await _configService.ListView(f => f.IsEnabled);
        //    await _businessGuaranteeService.Delete(f => f.GenerateDate == today && f.IsMatchRecord == isMatch);

        //    List<Live_BusinessGuarantee> empList = new List<Live_BusinessGuarantee>();
        //    foreach (var item in orderList)
        //    {
        //        var isMonthInsurance = item.OrderType == (int)InsuranceOrderEnumType.月保单;
        //        var data = await _insuranceApiService.GetOrderDetail(item.InsuranceId);
        //        var recordList = data.Data.baoquanlist.Select(f => new Live_BusinessGuarantee()
        //        {
        //            Id = 0,
        //            IdCardNo = f.Cidnum,
        //            CreateTime = DateTime.Now,
        //            CustomerName = f.Csjygdw,
        //            EndDate = f.Czbrq,
        //            StartDate = f.Cqbrq,
        //            EntryTime = DateTime.Now,
        //            InsuranceMonth = DateTime.MinValue,
        //            Name = f.Cname,
        //            PlanName = f.Geentname,
        //            PlanNameId = int.Parse(f.Geentid),
        //            SignUnitName = f.Cldhtdw,
        //            WorkClass = f.Czylb + "类",
        //            WorkName = f.Cgygz,
        //            InsuranceCompany = item.InsuranceCompany,
        //            InsuranceId = item.InsuranceId,
        //            IsMonthInsurance = isMonthInsurance,
        //            Remark = "",
        //            Status = 0,
        //            UserEntryId = 0,
        //            UserEntryStatus = string.Empty,
        //            GenerateDate = today,
        //            DelRecordID = Guid.Empty,
        //            IsDelTag = false,
        //            IsEnabled = true,
        //            IsMatchRecord = isMatch
        //        }).ToList();
        //        empList.AddRange(recordList);
        //    }
        //    await _businessGuaranteeService.Add(empList);
        //    return Ok();
        //}
        ///// <summary>
        ///// 阳光匹配
        ///// </summary>
        ///// <returns></returns>
        //private async Task<IActionResult> BusinessInsurancePlanName1Match(bool isMatch)
        //{

        //    var today = DateTime.Today.AddDays(2);
        //    var nextDay = DateTime.Today.AddDays(1);
        //    var currentMonth = DateTime.Today.AddDays(1 - today.Day);

        //    var endMonth = currentMonth.AddMonths(1).AddDays(-1);
        //    var configList = await _configService.ListView(f => f.InsuranceCompany == "阳光保险公司"&& f.IsEnabled&&f.IsMain);

        //    var orderMonth = nextDay.AddDays(1 - nextDay.Day);
        //    List<Live_UserEntry> entryModelList = new List<Live_UserEntry>();
        //    List<Live_BusinessGuarantee> recordList = new List<Live_BusinessGuarantee>();

        //    string planName = string.Empty;

        //    foreach (var configItme in configList)
        //    {
        //        entryModelList.Clear();
        //        recordList.Clear();
        //        // 必须要参保的员工
        //        var entryList = await _entryService.ListView(f =>f.IsManuallyInsurance==false&& f.CustomerName == configItme.CustomerName &&!string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName) && f.Status == 2);
        //        var ygRecordList = await _businessGuaranteeService.List(f => f.GenerateDate==today&&f.IsMatchRecord== isMatch && f.InsuranceCompany == "阳光保险公司" && f.CustomerName == configItme.CustomerName && f.SignUnitName == configItme.InsuranceSigningUnitName);
        //        foreach (var item in entryList)
        //        {

        //            if (item.WorkClass == "3类")
        //            {
        //                planName = configItme.Plan3;
        //            }
        //            else if (item.WorkClass == "4类")
        //            {
        //                planName = configItme.Plan4;
        //            }
        //            else
        //            {
        //                planName = configItme.Plan5;
        //            }


        //            var entryModel = await _entryService.Get(f => f.ID == item.ID);
        //            // 工种、类别、身份证号码、方案
        //            if (ygRecordList.Any(f => f.Name == item.Name && f.IdCardNo == item.CardID && f.WorkClass == item.WorkClass && f.WorkName == item.WorkName&&f.PlanName==planName))
        //            {
        //                var record = ygRecordList.First(f => f.Name == item.Name && f.IdCardNo == item.CardID && item.WorkClass == f.WorkClass && f.WorkName == item.WorkName && f.PlanName == planName);
        //                // 状态改成 阳光保险 已增员
        //                entryModel.InsuranceCompany1 = record.InsuranceCompany;
        //                entryModel.PlanName1 = record.PlanName;
        //                entryModel.InsuranceId1 = record.InsuranceId;
        //                entryModel.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
        //                record.UserEntryId = item.ID;
        //                record.UserEntryStatus = "2";
        //                record.Status = UserEntryPlanStatusEnumType.已增员;
        //                recordList.Add(record);
        //            }
        //            else
        //            {
        //                Console.WriteLine("+++++++++++++++++++++主险不匹配+++++++++++++++++++++++");
        //                Console.WriteLine($"{item.Name}");
        //                Console.WriteLine($"{item.CardID}");
        //                Console.WriteLine($"{item.WorkClass}");
        //                Console.WriteLine($"{item.WorkName}");
        //                Console.WriteLine($"{item.CustomerName}");
        //                Console.WriteLine($"{item.EntryTime.Value.ToString("yyyy-MM-dd")}");
        //                Console.WriteLine("--------------------------------------------------");
        //                // 状态改成 待增员
        //                entryModel.PlanStatus1 = UserEntryPlanStatusEnumType.待增员;
        //            }
        //            entryModelList.Add(entryModel);
        //        }

        //        if (entryModelList.Any())
        //        {
        //            var dt = new DateTime(2025,10,31);
        //            var td = DateTime.Today;
        //            if (td >= dt)
        //            {
        //                // 更新员工参保状态
        //                await _entryService.UpdateBusInsurance(entryModelList);
        //            }
        //        }
        //        // 匹配记录
        //        if (isMatch)
        //        {
        //            if (recordList.Any())
        //            {
        //                await _businessGuaranteeService.Update(recordList);
        //            }
        //        }
        //    }
        //    // 匹配记录
        //    if (isMatch)
        //    {
        //        List<string> idCardNoList = new List<string>();
        //        var manuallyInsurance = await _entryService.List(f => f.IsManuallyInsurance == true && f.Status == 2);
        //        if (manuallyInsurance != null && manuallyInsurance.Count > 0)
        //        {
        //            // 客服手动参保员工不做自动减员
        //            idCardNoList = manuallyInsurance.Select(f => f.CardID).ToList();
        //        }
        //        var noUserEntryRecord = await _businessGuaranteeService.List(f => !idCardNoList.Contains(f.IdCardNo) && f.GenerateDate == today && f.IsMatchRecord && f.InsuranceCompany == "阳光保险公司" && f.IsEnabled && f.UserEntryId <= 0);
        //        foreach (var item in noUserEntryRecord)
        //        {
        //            item.Remark = "未匹配到在职人员信息";
        //            item.Status = UserEntryPlanStatusEnumType.待减员;
        //            item.IsDelTag = false;
        //        }
        //        await _businessGuaranteeService.Update(noUserEntryRecord);
        //    }
        //    return Ok();
        //}
        ///// <summary>
        ///// 中国大地财险公司
        ///// </summary>
        ///// <param name="isMatch"></param>
        ///// <returns></returns>

        //private async Task<IActionResult> BusinessInsurancePlanName2Match(bool isMatch)
        //{

        //    var today = DateTime.Today;
        //    var nextDay = DateTime.Today.AddDays(1);
        //    var currentMonth = DateTime.Today.AddDays(1 - today.Day);

        //    var endMonth = currentMonth.AddMonths(1).AddDays(-1);
        //    var configList = await _configService.ListView(f => f.InsuranceCompany == "中国大地财险公司" && f.IsEnabled);

        //    var orderMonth = nextDay.AddDays(1 - nextDay.Day);
        //    List<Live_UserEntry> entryModelList = new List<Live_UserEntry>();
        //    List<Live_BusinessGuarantee> recordList = new List<Live_BusinessGuarantee>();
        //    foreach (var configItme in configList)
        //    {
        //        entryModelList.Clear();
        //        recordList.Clear();
        //        // 必须要参保的员工
        //        var entryList = await _entryService.ListView(f => f.CustomerName == configItme.CustomerName && !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName) && f.Status == 2);
        //        var ygRecordList = await _businessGuaranteeService.List(f => f.GenerateDate == today && f.IsMatchRecord == isMatch && f.InsuranceCompany == "中国大地财险公司" && f.CustomerName == configItme.CustomerName && f.SignUnitName == configItme.InsuranceSigningUnitName);
        //        foreach (var item in entryList)
        //        {
        //            var entryModel = await _entryService.Get(f => f.ID == item.ID);
        //            // 工种、类别、身份证号码、方案
        //            if (ygRecordList.Any(f => f.Name == item.Name && f.IdCardNo == item.CardID && f.WorkClass == item.WorkClass && f.WorkName == item.WorkName))
        //            {
        //                var record = ygRecordList.First(f => f.Name == item.Name && f.IdCardNo == item.CardID && item.WorkClass == f.WorkClass && f.WorkName == item.WorkName);
        //                // 状态改成 阳光保险 已增员
        //                entryModel.InsuranceCompany2 = record.InsuranceCompany;
        //                entryModel.PlanName2 = record.PlanName;
        //                entryModel.InsuranceId2 = record.InsuranceId;
        //                entryModel.PlanStatus2 = UserEntryPlanStatusEnumType.已增员;
        //                record.UserEntryId = item.ID;
        //                record.UserEntryStatus = "2";
        //                record.Status = UserEntryPlanStatusEnumType.已增员;
        //                recordList.Add(record);
        //            }
        //            else
        //            {
        //                Console.WriteLine("+++++++++++++++++++++不匹配+++++++++++++++++++++++");
        //                Console.WriteLine($"{item.Name}");
        //                Console.WriteLine($"{item.CardID}");
        //                Console.WriteLine($"{item.WorkClass}");
        //                Console.WriteLine($"{item.WorkName}");
        //                Console.WriteLine($"{item.CustomerName}");
        //                Console.WriteLine("--------------------------------------------------");
        //                // 状态改成 待增员
        //                entryModel.PlanStatus2 = UserEntryPlanStatusEnumType.待增员;
        //            }
        //            entryModelList.Add(entryModel);
        //        }

        //        if (entryModelList.Any())
        //        {
        //            var dt = new DateTime(2025, 10, 31);
        //            var td = DateTime.Today;
        //            if (td >= dt)
        //            {
        //                // 更新员工参保状态
        //                await _entryService.UpdateBusInsurance(entryModelList);
        //            }
        //        }
        //        // 匹配记录
        //        if (isMatch)
        //        {
        //            if (recordList.Any())
        //            {
        //                await _businessGuaranteeService.Update(recordList);
        //            }
        //        }
        //    }
        //    // 匹配记录
        //    if (isMatch)
        //    {

        //        List<string> idCardNoList = new List<string>();
        //        var manuallyInsurance = await _entryService.List(f => f.IsManuallyInsurance == true && f.Status == 2);
        //        if (manuallyInsurance != null && manuallyInsurance.Count > 0)
        //        {
        //            // 客服手动参保员工不做自动减员
        //            idCardNoList = manuallyInsurance.Select(f => f.CardID).ToList();
        //        }
        //        var noUserEntryRecord = await _businessGuaranteeService.List(f => !idCardNoList.Contains(f.IdCardNo)&& f.GenerateDate == today && f.IsMatchRecord && f.InsuranceCompany == "中国大地财险公司" && f.IsEnabled && f.UserEntryId <= 0);
        //        foreach (var item in noUserEntryRecord)
        //        {
        //            item.Remark = "未匹配到在职人员信息";
        //            item.Status = UserEntryPlanStatusEnumType.待减员;
        //            item.IsDelTag = false;
        //        }
        //    }
        //    return Ok();
        //}

        ///// <summary>
        /////  商保相同保单增减匹配
        ///// </summary>
        ///// <param name="insuranceCompany"></param>
        ///// <returns></returns>
        //private async Task<IActionResult> BusinessInsuranceGeneratePushRecord()
        //{

        //    var dt = new DateTime(2025, 10, 31);
        //    var today = DateTime.Today;
        //    var nextDay = today.AddDays(1);
        //    var nextDayMonth = nextDay.AddDays(1 - nextDay.Day);
        //    Live_V_BusinessInsuranceOrderPlan orderPlan = null;
        //    Live_BusinessInsuranceRecord record = null;

        //    List<BusinessInsuranceStatusEnumType> statusEnumTypes = new List<BusinessInsuranceStatusEnumType>() { 
        //     BusinessInsuranceStatusEnumType.已增员, BusinessInsuranceStatusEnumType.已减员};

        //    // 删除今天需要增减员记录
        //    if (today >= dt)
        //    {
        //        await _businessInsuranceRecordService.Delete(f => f.CreateTime >= today && !statusEnumTypes.Contains(f.Status));
        //    }
        //    var orderPlans = await _orderService.ListOrderPlanView(f => f.StartDate.Value == nextDayMonth);
        //    #region 次险生成增员记录
        //    var otherConfigList = await _configService.ListView(f =>f.InsuranceCompany== "中国大地财险公司" && !f.IsMain && f.IsEnabled);
        //    // 月保单
        //    var otherOrderList = orderPlans.Where(f =>  f.InsuranceCompany == "中国大地财险公司" && f.OrderType == (int)InsuranceOrderEnumType.月保单);
        //    List<Live_BusinessInsuranceRecord> otherRecord = new List<Live_BusinessInsuranceRecord>();
        //    foreach (var configItem in otherConfigList)
        //    {
        //        var entryList = await _entryService.ListView(f => f.CustomerName == configItem.CustomerName && !string.IsNullOrWhiteSpace(f.WorkName) && !string.IsNullOrWhiteSpace(f.WorkClass) && f.PlanStatus2 != null && f.PlanStatus2.Value == UserEntryPlanStatusEnumType.待增员);
        //        if (entryList != null && entryList.Count > 0)
        //        {
        //            string planName = string.Empty;

        //            // 获取订单信息
        //            foreach (var entryItem in entryList)
        //            {
        //                if (entryItem.WorkClass == "3类")
        //                {
        //                    planName = configItem.Plan3;
        //                }
        //                else if (entryItem.WorkClass == "4类")
        //                {
        //                    planName = configItem.Plan4;
        //                }
        //                else
        //                {
        //                    planName = configItem.Plan5;
        //                }

        //                if (configItem.IsDefaultPush)
        //                {
        //                    orderPlan = otherOrderList.FirstOrDefault(f => f.IsDefaultPush && f.GnteeName == planName);
        //                }
        //                else
        //                {
        //                    orderPlan = otherOrderList.FirstOrDefault(f => f.InsuranceSingningUnitName == configItem.InsuranceSigningUnitName && !f.IsDefaultPush && f.GnteeName == planName);
        //                }
        //                var ageGender = IdCardHelper.GetAgeAndGender(entryItem.CardID);
        //                record = new Live_BusinessInsuranceRecord()
        //                {
        //                    AddMatchRecordID = null,
        //                    AddPushTime = null,
        //                    AddSysTime = null,
        //                    Age = ageGender.Age,
        //                    BusinessInsurancePushID = null,
        //                    CreateTime = DateTime.Now,
        //                    CustomerGID = configItem.CustomerGID,
        //                    CustomerName = configItem.CustomerName,
        //                    DelMatchRecordID = null,
        //                    DelPushTime = null,
        //                    DelSysTime = null,
        //                    StartDate = orderPlan.StartDate,
        //                    EndDate = orderPlan.EndDate,
        //                    EntryDate = entryItem.EntryTime.Value.Date,
        //                    ID = Guid.NewGuid(),
        //                    IdCardNo = entryItem.CardID,
        //                    InsuranceCompany = orderPlan.InsuranceCompany,
        //                    InsuranceId = orderPlan.InsuranceId,
        //                    InsuranceNo = orderPlan.InsuranceNo,
        //                    InsuranceSigningUnitID = configItem.InsuranceSigningUnitID,
        //                    InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
        //                    IsDayOrder = false,
        //                    IsDefaultPush = orderPlan.IsDefaultPush,
        //                    IsExpired = false,
        //                    IsMain = configItem.IsMain,
        //                    LeaveDate = entryItem.LeaveTime?.Date,
        //                    Name = entryItem.Name,
        //                    OrderNo = string.Empty,
        //                    PlanName = orderPlan.GnteeName,
        //                    PlanNameId = orderPlan.GnteeId,
        //                    Sex = ageGender.Gender,

        //                    Status = BusinessInsuranceStatusEnumType.待增员匹配,
        //                    UserEntryID = entryItem.ID,
        //                    UserID = entryItem.UserID ?? 0,
        //                    WorkClass = entryItem.WorkClass,
        //                    WorkName = entryItem.WorkName,
        //                };
        //                otherRecord.Add(record);
        //            }
        //        }

        //    }
        //    #endregion
        //    #region 主险生成增员记录
        //    var mainConfigList = await _configService.ListView(f => f.InsuranceCompany == "阳光保险公司" && f.IsMain && f.IsEnabled);
        //    // 月保单
        //    var monthOrderList = orderPlans.Where(f =>  f.InsuranceCompany == "阳光保险公司" && f.OrderType == (int)InsuranceOrderEnumType.月保单);
        //    List<Live_BusinessInsuranceRecord> mainRecord = new List<Live_BusinessInsuranceRecord>();
        //    foreach (var configItem in mainConfigList)
        //    {
        //        var mainEntryList = await _entryService.ListView(f => f.CustomerName == configItem.CustomerName && !string.IsNullOrWhiteSpace(f.WorkName) && !string.IsNullOrWhiteSpace(f.WorkClass) && f.PlanStatus1 != null && f.PlanStatus1.Value == UserEntryPlanStatusEnumType.待增员);
        //        if (mainEntryList != null && mainEntryList.Count > 0)
        //        {
        //            string planName = string.Empty;
        //            // 获取订单信息
        //            foreach (var entryItem in mainEntryList)
        //            {

        //                if (entryItem.WorkClass == "3类")
        //                {
        //                    planName = configItem.Plan3;
        //                }
        //                else if (entryItem.WorkClass == "4类")
        //                {
        //                    planName = configItem.Plan4;
        //                }
        //                else
        //                {
        //                    planName = configItem.Plan5;
        //                }
        //                if (configItem.IsDefaultPush)
        //                {
        //                    orderPlan = monthOrderList.FirstOrDefault(f => f.IsDefaultPush && f.GnteeName == planName);
        //                }
        //                else
        //                {
        //                    orderPlan = monthOrderList.FirstOrDefault(f => f.InsuranceSingningUnitName == configItem.InsuranceSigningUnitName && !f.IsDefaultPush && f.GnteeName == planName);
        //                }
        //                var ageGender = IdCardHelper.GetAgeAndGender(entryItem.CardID);
        //                record = new Live_BusinessInsuranceRecord()
        //                {
        //                    AddMatchRecordID = null,
        //                    AddPushTime = null,
        //                    AddSysTime = null,
        //                    Age = ageGender.Age,
        //                    BusinessInsurancePushID = null,
        //                    CreateTime = DateTime.Now,
        //                    CustomerGID = configItem.CustomerGID,
        //                    CustomerName = configItem.CustomerName,
        //                    DelMatchRecordID = null,
        //                    DelPushTime = null,
        //                    DelSysTime = null,
        //                    StartDate = orderPlan.StartDate,
        //                    EndDate = orderPlan.EndDate,
        //                    EntryDate = entryItem.EntryTime.Value.Date,
        //                    ID = Guid.NewGuid(),
        //                    IdCardNo = entryItem.CardID,
        //                    InsuranceCompany = orderPlan.InsuranceCompany,
        //                    InsuranceId = orderPlan.InsuranceId,
        //                    InsuranceNo = orderPlan.InsuranceNo,
        //                    InsuranceSigningUnitID = configItem.InsuranceSigningUnitID,
        //                    InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
        //                    IsDayOrder = orderPlan.OrderType== (int)InsuranceOrderEnumType.日保单,
        //                    IsDefaultPush = orderPlan.IsDefaultPush,
        //                    IsExpired = false,
        //                    IsMain = configItem.IsMain,
        //                    LeaveDate = entryItem.LeaveTime?.Date,
        //                    Name = entryItem.Name,
        //                    OrderNo = string.Empty,
        //                    PlanName = orderPlan.GnteeName,
        //                    PlanNameId = orderPlan.GnteeId,
        //                    Sex = ageGender.Gender,
        //                    Status = BusinessInsuranceStatusEnumType.待减员,
        //                    UserEntryID = entryItem.ID,
        //                    UserID = entryItem.UserID ?? 0,
        //                    WorkClass = entryItem.WorkClass,
        //                    WorkName = entryItem.WorkName,


        //                };
        //                mainRecord.Add(record);
        //            }
        //        }
        //    }
        //    #endregion
        //    List<Live_BusinessInsuranceRecord> addRecordList = new List<Live_BusinessInsuranceRecord>();
        //    List<Live_BusinessInsuranceRecord> delRecordList = new List<Live_BusinessInsuranceRecord>();
        //    if (otherRecord.Any())
        //    {
        //        addRecordList.AddRange(otherRecord);
        //    }
        //    if (mainRecord.Any())
        //    {
        //        addRecordList.AddRange(mainRecord);
        //    }
        //    #region 增员记录匹配减员记录
        //    Live_BusinessGuarantee guaranteeRecord;
        //    List<Live_BusinessGuarantee> businessGuaranteeList = new List<Live_BusinessGuarantee>();
        //    foreach (var item in addRecordList)
        //    {
        //        // 同保单下如果存在相同身份证号码的记录，则优先减员
        //        guaranteeRecord = await _businessGuaranteeService.Get(f =>f.Status== UserEntryPlanStatusEnumType.待减员 && f.GenerateDate==today&&f.IsMatchRecord&& f.InsuranceId == item.InsuranceId && f.IdCardNo == item.IdCardNo && !f.IsDelTag);
        //        if (guaranteeRecord == null)
        //        {
        //            guaranteeRecord = await _businessGuaranteeService.Get(f =>f.Status== UserEntryPlanStatusEnumType.待减员&& f.GenerateDate == today && f.IsMatchRecord && f.InsuranceId == item.InsuranceId && !f.IsDelTag);
        //        }
        //        if (guaranteeRecord != null)
        //        {
        //            guaranteeRecord.Status = UserEntryPlanStatusEnumType.待减员;
        //            guaranteeRecord.IsDelTag = true;
        //            guaranteeRecord.DelRecordID = item.ID;
        //            businessGuaranteeList.Add(guaranteeRecord);
        //        }
        //        else
        //        {
        //            if (item.IsDefaultPush)
        //            {
        //                orderPlan = orderPlans.FirstOrDefault(f =>f.OrderType==0&& f.InsuranceCompany == item.InsuranceCompany && f.IsDefaultPush && f.GnteeName == item.PlanName);
        //            }
        //            else
        //            {
        //                orderPlan = orderPlans.FirstOrDefault(f => f.OrderType == 0 && f.InsuranceCompany == item.InsuranceCompany && f.InsuranceSingningUnitName == item.InsuranceSigningUnitName && !f.IsDefaultPush && f.GnteeName == item.PlanName);
        //            }
        //            // 日保单增员
        //            if (orderPlan != null)
        //            {
        //                item.InsuranceId = orderPlan.InsuranceId;
        //                item.InsuranceNo = orderPlan.InsuranceNo;
        //                item.PlanName = orderPlan.GnteeName;
        //                item.PlanNameId = orderPlan.GnteeId;
        //                item.IsDayOrder = true;
        //            }
        //        }
        //    }

        //    // 日保单 未匹配到在职记录 则全部减员
        //    // 月保单 标记未待减员且有减员标记 则减员 否则保留下次减员
        //    var delList = await _businessGuaranteeService.List(f => f.GenerateDate.Value.Date == today && f.IsMatchRecord&& f.UserEntryId <= 0 && !f.IsMonthInsurance);


        //    if (delList!=null&&delList.Count>0)
        //    {
        //        businessGuaranteeList.AddRange(delList);
        //    }
        //    if (businessGuaranteeList != null && businessGuaranteeList.Count > 0)
        //    {
        //        Live_BusinessInsuranceRecord delRecord = null;
        //        foreach (var item in businessGuaranteeList)
        //        {
        //            item.Status = UserEntryPlanStatusEnumType.待减员;
        //            orderPlan = orderPlans.FirstOrDefault(f => f.InsuranceId == item.InsuranceId&&f.GnteeName==item.PlanName);
        //            delRecord = new Live_BusinessInsuranceRecord()
        //            {
        //                AddMatchRecordID = null,
        //                DelMatchRecordID = item.DelRecordID,
        //                AddPushTime = null,
        //                AddSysTime = null,
        //                Age = 0,
        //                BusinessInsurancePushID = null,
        //                CreateTime = DateTime.Now,
        //                CustomerGID = Guid.Empty,
        //                CustomerName = item.CustomerName,
        //                DelPushTime = null,
        //                DelSysTime = null,
        //                StartDate = item.StartDate,
        //                EndDate = item.EndDate,
        //                EntryDate = item.EntryTime,
        //                ID = Guid.NewGuid(),
        //                IdCardNo = item.IdCardNo,
        //                InsuranceCompany = item.InsuranceCompany,
        //                InsuranceId = item.InsuranceId,
        //                InsuranceNo = orderPlan.InsuranceNo,
        //                InsuranceSigningUnitID = Guid.Empty,
        //                InsuranceSigningUnitName = item.SignUnitName,
        //                IsDayOrder = !item.IsMonthInsurance,
        //                IsDefaultPush = orderPlan.IsDefaultPush,
        //                IsExpired = false,
        //                IsMain = orderPlan.InsuranceCompany == "阳光保险公司",
        //                LeaveDate = null,
        //                Name = item.Name,
        //                OrderNo = string.Empty,
        //                PlanName = orderPlan.GnteeName,
        //                PlanNameId = orderPlan.GnteeId,
        //                Sex = string.Empty,
        //                Status = BusinessInsuranceStatusEnumType.待减员,
        //                UserEntryID = item.UserEntryId,
        //                UserID = 0,
        //                WorkClass = item.WorkClass,
        //                WorkName = item.WorkName,
        //            };
        //            delRecordList.Add(delRecord);
        //        }
        //    }



        //    List<Live_BusinessInsuranceRecord> recordList = new List<Live_BusinessInsuranceRecord>();
        //    if (addRecordList.Any())
        //    {
        //        recordList.AddRange(addRecordList);
        //    }

        //    if (delRecordList.Any())
        //    {
        //        recordList.AddRange(delRecordList);
        //    }
        //    if (recordList.Any())
        //    {
        //        await _businessInsuranceRecordService.Add(delRecordList);
        //    }

        //    if (businessGuaranteeList!=null&& businessGuaranteeList.Any())
        //    {
        //        await _businessGuaranteeService.Update(businessGuaranteeList);
        //    }
        //    #endregion
        //    return Ok();
        //}
        //private async Task<IActionResult> BusinessInsurancePush()
        //{
        //    var today = DateTime.Today;
        //    var delRecordList = await _businessInsuranceRecordService.List(f => f.CreateTime >= today && f.Status ==  BusinessInsuranceStatusEnumType.待减员);
        //    var addRecordList = await _businessInsuranceRecordService.List(f => f.CreateTime >= today && f.Status == BusinessInsuranceStatusEnumType.待增员&& f.PlanNameId > 0&&!string.IsNullOrWhiteSpace(f.PlanName));
        //    var delGroupList = delRecordList.GroupBy(f => new { f.InsuranceCompany, f.InsuranceId,f.PlanName }).ToList();
        //    var addGroupList = addRecordList.GroupBy(f => new { f.InsuranceCompany, f.InsuranceId,f.PlanName,f.PlanNameId }).ToList();
        //    BatchDelEmployeeRequest request = null;
        //    foreach (var groupItem in delGroupList)
        //    {
        //        var records = groupItem.ToList();
        //        request = new BatchDelEmployeeRequest()
        //        {
        //            InsureId = groupItem.Key.InsuranceId,
        //            CustomerList = records.Select(f => new DelCustomerinfo()
        //            {
        //                Custemername = f.Name,
        //                Custemernum = f.IdCardNo,
        //            }).ToList()
        //        };

        //        var resp = await _insuranceApiService.BatchDelEmployee(request);
        //        if (resp != null)
        //        {
        //            Common.StaticLog.Info($"商保减员推送响应：订单编号：{groupItem.Key.InsuranceId}，响应信息：{Newtonsoft.Json.JsonConvert.SerializeObject(resp)}");
        //            if (resp.Code == 200)
        //            {
        //                foreach (var item in records)
        //                {
        //                    item.Status = BusinessInsuranceStatusEnumType.已减员;
        //                    item.PushTime = DateTime.Now;
        //                    item.IndecaseId = resp.IndecaseId;
        //                    item.PushMsg = resp.Msg;
        //                }
        //                if (resp.FailList != null && resp.FailList.Count > 0)
        //                {
        //                    foreach (var item in resp.FailList)
        //                    {
        //                        var failRecord = records.First(f => f.IdCardNo == item.Customerinfo.Custemernum && f.Name == item.Customerinfo.Custemername);
        //                        failRecord.Status = BusinessInsuranceStatusEnumType.减员失败;
        //                        failRecord.PushMsg = item.Msg;
        //                    }
        //                }
        //            }
        //            else
        //            {

        //                foreach (var item in records)
        //                {
        //                    item.Status = BusinessInsuranceStatusEnumType.减员失败;
        //                    item.PushTime = DateTime.Now;
        //                    item.IndecaseId = resp.IndecaseId;
        //                    item.PushMsg = resp.Msg;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var item in records)
        //            {
        //                item.Status = BusinessInsuranceStatusEnumType.减员失败;
        //            }
        //            Common.StaticLog.Info($"商保减员推送响应：订单编号：{groupItem.Key.InsuranceId}，响应信息：Null,未获取到正确的响应信息！");
        //        }
        //        await _businessInsuranceRecordService.Update(records);
        //    }
        //    BatchAddEmployeeRequest addRequest = null;
        //    foreach (var groupItem in addGroupList)
        //    {
        //        var records = groupItem.ToList();
        //        addRequest = new BatchAddEmployeeRequest()
        //        {
        //            InsureId = groupItem.Key.InsuranceId,
        //            GnteeId = groupItem.Key.PlanNameId,
        //            GnteeName = groupItem.Key.PlanName,
        //            CustomerList = records.Select(f => new AddCustomerinfo()
        //            {
        //                Custemername = f.Name,
        //                Custemernum = f.IdCardNo,
        //                Custemerage = f.Age.ToString(),
        //                Custemersex = f.Sex,
        //                Custemertype = "身份证",
        //                Guyuangongzhong = f.WorkName,
        //                Zhiyeleibei = f.WorkClass.Substring(0, 1),
        //                Ldhtdw = f.InsuranceSigningUnitName,
        //                Sjygdw = f.CustomerName,
        //            }).ToList()
        //        };

        //        var resp = await _insuranceApiService.BatchAddEmployee(addRequest);
        //        if (resp != null)
        //        {
        //            Common.StaticLog.Info($"商保增员推送响应：订单编号：{groupItem.Key.InsuranceId}，响应信息：{Newtonsoft.Json.JsonConvert.SerializeObject(resp)}");


        //            if (resp.Code == 200)
        //            {
        //                foreach (var item in records)
        //                {
        //                    item.Status = BusinessInsuranceStatusEnumType.已增员;
        //                    item.PushTime = DateTime.Now;
        //                    item.IndecaseId = resp.IndecaseId;
        //                    item.PushMsg = resp.Msg;
        //                }
        //                if (resp.FailList != null && resp.FailList.Count > 0)
        //                {
        //                    foreach (var item in resp.FailList)
        //                    {
        //                        var failRecord = records.First(f => f.IdCardNo == item.Customerinfo.Custemernum && f.Name == item.Customerinfo.Custemername);
        //                        failRecord.Status = BusinessInsuranceStatusEnumType.增员失败;
        //                        failRecord.PushMsg = item.Msg;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                foreach (var item in records)
        //                {
        //                    item.Status = BusinessInsuranceStatusEnumType.增员失败;
        //                    item.PushTime = DateTime.Now;
        //                    item.IndecaseId = resp.IndecaseId;
        //                    item.PushMsg = resp.Msg;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var item in records)
        //            {
        //                item.Status = BusinessInsuranceStatusEnumType.增员失败;
        //            }
        //            Common.StaticLog.Info($"商保增员推送响应：订单编号：{groupItem.Key.InsuranceId}，响应信息：Null,未获取到正确的响应信息！");
        //        }
        //        await _businessInsuranceRecordService.Update(records);

        //    }
        //    return Ok();
        //}

        //private async Task<IActionResult> BusinessInsuranceEmail([FromForm] string insuranceCompany)
        //{
        //    string msg = string.Empty;

        //    var configList = await _configService.ListView(f => f.IsEnabled && f.IsMain);
        //    foreach (var item in configList)
        //    {
        //        var entryList = await _entryService.List(f => f.CustomerGID == item.CustomerGID && f.Status == 2 && f.PlanStatus1 != UserEntryPlanStatusEnumType.已增员);

        //        if (entryList.Any())
        //        {
        //            msg += $"{item.CustomerName}--未增员名单，{string.Join(',', entryList.Select(f => $"{f.Name}（{f.PlanStatus1}）"))}\r\n";
        //        }
        //    }

        //    if (!string.IsNullOrWhiteSpace(msg))
        //    {
        //        //_aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "商保增减异常通知", msg, "", "");
        //        _aliSendInfo.SendAliEmail("15261300969@163.com", "商保增减异常通知", msg, "", "");
        //    }
        //    else
        //    {
        //        //_aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "商保推送完成通知", "无增减推送异常人员", "", "");
        //        _aliSendInfo.SendAliEmail("15261300969@163.com", "商保推送完成通知", "无增减推送异常人员", "", "");
        //    }
        //    return Ok();
        //}


        #endregion
        #region 商保推送流程



        //[HttpPost]
        //public async Task BusinessPushJob()
        //{
        //    await _businessInsurancService.PushProcess();

        //    await Task.Delay(1000);

        //    await _businessInsurancService.CheckProcess();
        //}
        //[HttpPost]
        //public async Task BusinessCheckJob()
        //{
        //    await _businessInsurancService.CheckProcess();
        //}


        //[HttpPost]
        //public async Task BusinessNoticeJob()
        //{
        //    await _businessInsurancService.EmailNotice();
        //}

        //[HttpPost]
        //public async Task BusinessNoticeJob11111()
        //{
        //   var orderList=await _orderService.ListView(f => !string.IsNullOrWhiteSpace(f.SigningUnitName));

        //    foreach (var item in orderList)
        //    {
        //       var order=await _orderService.Get(f=>f.ID==item.ID);

        //        order.SigningUnitNames = item.SigningUnitName;

        //     await   _orderService.Update(order);
        //    }
        //}










        ///// <summary>
        ///// 临时解决方案
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> UploadInsurancePlanNameByWorkName(IFormFile file)
        //{
        //    List<InsuranceConfigTempDto> configs = new List<InsuranceConfigTempDto>();

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        file.CopyTo(ms);
        //        configs = NpoiExcelHelper.ReadExcelFile<InsuranceConfigTempDto>(ms, "");
        //    }
        //    string key = "customer_wrokname_insuranceInfo";
        //    await _redisService.DeleteAsync(key);
        //    await _redisService.SetAsync("customer_wrokname_insuranceInfo", configs);
        //    return Ok();
        //}
        ///// <summary>
        ///// 临时解决方案
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> DownInsurancePlanNameByWorkName()
        //{
        //    List<InsuranceConfigTempDto> configs = new List<InsuranceConfigTempDto>();
        //    configs = await _redisService.GetAsync<List<InsuranceConfigTempDto>>("customer_wrokname_insuranceInfo");
        //    var fileBytes = NpoiExcelHelper.WriteExcel(configs, "商保配置");
        //    string base64String = Convert.ToBase64String(fileBytes);
        //    return Ok(new { base64String, fileName = "商保配置.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        //}


        ///// <summary>
        ///// 根据工种匹配保险公司及参保方案
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> HandleInsurancePlanNameByWorkNameJob()
        //{
        //    List<InsuranceConfigTempDto> configs = new List<InsuranceConfigTempDto>();
        //    string key = "customer_wrokname_insuranceInfo";
        //    configs = await _redisService.GetAsync<List<InsuranceConfigTempDto>>(key);

        //    List<Live_UserEntry> updateEntityList = new List<Live_UserEntry>();
        //    if (configs!=null&& configs.Count>0)
        //    {
        //        var customerNameList = configs.Select(f => f.CustomerName).Distinct();
        //        List<Live_V_UserEntry> entryViewList = null;
        //        InsuranceConfigTempDto config = null;
        //        foreach (var customerName in customerNameList)
        //        {

        //            updateEntityList.Clear();
        //          entryViewList = await _entryService.ListView(f => f.CustomerName == customerName && f.Status == 2 && !string.IsNullOrWhiteSpace(f.WorkName));
        //            if (entryViewList.Any())
        //            {

        //                foreach (var itemView in entryViewList)
        //                {
        //                    config = configs.FirstOrDefault(f => f.CustomerName == customerName && f.WorkName == itemView.WorkName);
        //                    if (config != null)
        //                    {
        //                        if (itemView.PlanName1 == config.PlanName && itemView.InsuranceCompany1 == config.InsuranceCompany)
        //                        {
        //                            if (config.InsuranceCompany != "阳光保险公司" && itemView.IsManuallyInsurance == true)
        //                            {
        //                                continue;
        //                            }
        //                            else
        //                            {

        //                                itemView.PlanName1 = config.PlanName;
        //                                itemView.InsuranceCompany1 = config.InsuranceCompany;
        //                                if (config.InsuranceCompany != "阳光保险公司")
        //                                {
        //                                    itemView.IsManuallyInsurance = true;
        //                                }

        //                                var entry =await  _entryService.Get(itemView.ID);
        //                                entry.PlanName1 = itemView.PlanName1;
        //                                entry.InsuranceCompany1 = itemView.InsuranceCompany1;
        //                                entry.IsManuallyInsurance = itemView.IsManuallyInsurance;

        //                                updateEntityList.Add(entry);

        //                            }
        //                        }
        //                        else {

        //                            itemView.PlanName1 = config.PlanName;
        //                            itemView.InsuranceCompany1 = config.InsuranceCompany;
        //                            if (config.InsuranceCompany != "阳光保险公司")
        //                            {
        //                                itemView.IsManuallyInsurance = true;
        //                            }
        //                            var entry = await _entryService.Get(itemView.ID);
        //                            entry.PlanName1 = itemView.PlanName1;
        //                            entry.InsuranceCompany1 = itemView.InsuranceCompany1;
        //                            entry.IsManuallyInsurance = itemView.IsManuallyInsurance;

        //                            updateEntityList.Add(entry);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                }

        //                if (updateEntityList.Any())
        //                {
        //                    await _entryService.UpdateInsurancePlan(updateEntityList);

        //                }
        //            }
        //        }
        //    }
        //    return Ok();
        //}

        #endregion
        #region 商保推送定时任务2026-06-05



          [HttpPost]
        public async Task<IActionResult> WorkPlanMatchJob()
        {
            await _busInsuracneService.WorkPlanMatch();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> PushZhongAnProcessJob()
        {
            var isb = await _redisService.SetAsync("PushZhongAnProcessJob", "PushZhongAnProcessJob", 60, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                await _busInsuracneService.PushZhongAnProcess();
                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "一分钟内只能推送一次" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PushAnZeTProcessJob()
        {
            var isb = await _redisService.SetAsync("PushAnZeTProcessJob", "PushAnZeTProcessJob", 60, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                // 取消节假日的限制 每天都可以推送
                //var dt = DateTime.Today;
                //var isWorkDay = WorkDayHelper.IsWorkDay2026(dt);
                //if (isWorkDay)
                //{
                    await _busInsuracneService.PushAnZeTProcess();
                //}
                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "一分钟内只能推送一次" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> PushDaDiProcessJob()
        {
            var isb = await _redisService.SetAsync("PushDaDiProcessJob", "PushDaDiProcessJob", 60, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                var dt = DateTime.Today;
                var isWorkDay = WorkDayHelper.IsWorkDay2026(dt);
                if (isWorkDay)
                {
                    await _busInsuracneService.PushDaDiProcess();
                }
                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "一分钟内只能推送一次" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> PushYangGuangProcessJob()
        {
            var isb = await _redisService.SetAsync("PushYangGuangProcessJob", "PushYangGuangProcessJob", 60, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                await _busInsuracneService.PushYangGuangProcess();

                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "一分钟内只能推送一次" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> PushDaDi365ProcessJob()
        {
            var isb = await _redisService.SetAsync("PushDaDi365ProcessJob", "PushDaDi365ProcessJob", 60, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                await _busInsuracneService.PushDaDi365Process();

                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "一分钟内只能推送一次" });
            }
        }

     

        [HttpPost]
        public async Task<IActionResult> PushLiBaoProcessJob()
        {

            var isb = await _redisService.SetAsync("PushLiBaoProcessJob", "PushLiBaoProcessJob", 350, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                await _busInsuracneService.PushLiBaoProcess();

                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "5分钟内只能推送一次" });
            }


        }


        [HttpPost]
        public async Task<IActionResult> YangGuangPushResultNoticeByEmail()
        {
            await _busInsuracneService.YangGuangPushResultNoticeByEmail();
            return Ok();
        }


       


        [HttpPost]
        public async Task<IActionResult> PushDeathInsuranceProcessJob()
        {
            await _deathInsuranceRecordService.PushProcess();
            return Ok();
        }

        /// <summary>
        /// 5
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PushFiveNoInsuranceJob()
        {
            await _busInsuracneService.FiveNoInsurancePush();
            return Ok();
        }
        #endregion

        #region 银行卡开卡
        /// <summary>
        /// 银行卡照片打水印
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BankOpenCardWatermarkJob()
        {
            Live_SchedulerLog log = new Live_SchedulerLog();

            Random random = new Random();
            var client = _clientFactory.CreateClient();
            string fontPath = Path.Combine(_env.WebRootPath, "Fonts", "msyh.ttf");
            var config = await _settingsService.GetMasterSettings();
            var ossDomain=config.AliBucketDomain;
            var records = await _openAccountService.List(f => f.Status == OpenAccountStatusEnumType.待处理 &&string.IsNullOrWhiteSpace(f.FailMsg)&&f.Name== "李崇亨");
            if (records.Count > 0)
            {
                foreach(var x in records)
                {
                    x.Content = "水印任务";
                    await _openAccountService.Update(x);
                }
                

            }
            #region 生成同意协议书
            var fileConfig = await _httpService.GetAsync<Live_ContractFileConfigExtension>(ServiceNames.CustomerService, $"/api/ContractFileConfig/Get?id=1F4D5007-841F-477B-B72B-11960EF4E8F3", isItem: false);
            #endregion

            foreach (var item in records)
            {
             var timestamp=  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                try
                {
                    item.EmpSealKey = fileConfig.Signers.FirstOrDefault(f => f.SignerPartyType == SignerPartyEnumType.第二当事人).SignKeyword;
                    // 获取电子签章
                     var signerInfo= await _httpService.GetAsync<Live_ContractSigner>(ServiceNames.CustomerService, $"/api/ContractSigner/GetSigner?name={item.Name}&idno={item.CardID}&isCompany=false", isItem: false);
                    if (signerInfo==null)
                    {
                        item.FailMsg = "员工电子章获取失败！";
                        await _openAccountService.Update(item);
                        continue;
                    }
                    item.EmpSealPath = signerInfo.OssUrl;
                    #region 电子合同
                    if (string.IsNullOrWhiteSpace(item.PdfPath))
                    {
                        Dictionary<string, string> dss = new Dictionary<string, string>();
                        dss.Add("EmpCardNo", item.CardID);
                        dss.Add("EmpName", item.Name);
                        var fileBase64 = await _fxqService.DownloadFile(fileConfig.FilePath);
                        var pdfResp = await _fxqService.CombineDocxKeywordToPdf(fileBase64, dss);
                        if (pdfResp.Code == 200)
                        {
                            using (var ms = new MemoryStream(Convert.FromBase64String(pdfResp.Data)))
                            {
                                var ossResp = await _aliyunOSSService.UploadFile(ms, "开户同意.pdf", true, false, "temp");
                                if (ossResp.Code != 0)
                                {
                                    item.FailMsg = "文件上传Oss异常";
                                    await _openAccountService.Update(item);
                                    continue;
                                }
                                item.PdfPath = ossResp.Data.ToString();
                            }

                        }
                        else
                        {
                            item.FailMsg = pdfResp.Msg;
                        }
                    }
                    #endregion
                    string frontFullPath = string.Empty;
                    string backFullPath = string.Empty;
                    if (item.CardFrontPath.StartsWith("http"))
                    {
                        frontFullPath = item.CardFrontPath;
                    }
                    else
                    {
                        frontFullPath = $"{ossDomain.TrimEnd('/') + "/" + item.CardFrontPath.TrimStart('/')}";
                    }
                    if (item.CardBackPath.StartsWith("http"))
                    {
                        backFullPath = item.CardBackPath;
                    }
                    else
                    {
                        backFullPath = $"{ossDomain.TrimEnd('/') + "/" + item.CardBackPath.TrimStart('/')}";
                    }
                    #region 正面照打水印
                    if (string.IsNullOrWhiteSpace(item.WatermarkFrontPath))
                    {
                        var frontResp = client.GetAsync(frontFullPath, HttpCompletionOption.ResponseHeadersRead).Result;
                        if (!frontResp.IsSuccessStatusCode)
                        {
                            item.FailMsg = $"证件照正面图片下载失败： {frontFullPath}。 Status: {frontResp.StatusCode}";
                            await _openAccountService.Update(item);
                            continue;
                        }
                        else
                        {
                            var extName = System.IO.Path.GetExtension(frontFullPath);
                            var stream = await frontResp.Content.ReadAsStreamAsync();
                            using var outputStream = new MemoryStream();
                            ImageSharp.AddTextWatermark(
                             stream,
                             outputStream,
                             "本人授权单位使用本人信息代申领工商银行借记卡",
                             fontPath,
                             fontSize: 30,
                             colorHex: "#80FFFFFF");

                            var frontObjectKey = $"BankFile/ICBC/Watermark/{item.CardID}/{timestamp}_front{extName}";

                            bool frontExistKey = await _aliyunOSSService.Exist(frontObjectKey);

                            if (frontExistKey)
                            {
                                await _aliyunOSSService.Delete(frontObjectKey);
                            }

                            var frontOssRes = await _aliyunOSSService.UploadOssFile(outputStream, frontObjectKey);
                            if (frontOssRes.Code == 0)
                            {
                                item.WatermarkFrontPath = frontOssRes.Data.ToString();
                            }
                        }
                    }
                    #endregion
                    #region 正面照打水印
                    if (string.IsNullOrWhiteSpace(item.WatermarkBackPath))
                    {
                        var backResp = client.GetAsync(backFullPath, HttpCompletionOption.ResponseHeadersRead).Result;
                        if (!backResp.IsSuccessStatusCode)
                        {
                            item.FailMsg = $"证件照正面图片下载失败： {backFullPath}。 Status: {backResp.StatusCode}";
                            await _openAccountService.Update(item);
                            continue;
                        }
                        else
                        {
                            var extName = System.IO.Path.GetExtension(backFullPath);
                            var stream = await backResp.Content.ReadAsStreamAsync();
                            using var outputStream = new MemoryStream();
                            ImageSharp.AddTextWatermark(
                             stream,
                             outputStream,
                             "本人授权单位使用本人信息代申领工商银行借记卡",
                             fontPath,
                             fontSize: 30,
                             colorHex: "#80FFFFFF");

                            var frontObjectKey = $"BankFile/ICBC/Watermark/{item.CardID}/{timestamp}_back{extName}";

                            bool frontExistKey = await _aliyunOSSService.Exist(frontObjectKey);
                            if (frontExistKey)
                            {
                                await _aliyunOSSService.Delete(frontObjectKey);
                            }


                            var frontOssRes = await _aliyunOSSService.UploadOssFile(outputStream, frontObjectKey);
                            if (frontOssRes.Code == 0)
                            {
                                item.WatermarkBackPath = frontOssRes.Data.ToString();
                            }
                        }
                    }
                    #endregion
                    if (string.IsNullOrWhiteSpace(item.SmsCode))
                    {
                        item.SmsCode = random.Next(1000, 10000).ToString();
                        item.SmsContent = $"您好！您已成功提交工商银行借记卡开户申请，请您及时登录微信小程序进行签署，验证码：{item.SmsCode}。";

                        if (_env.IsProduction())
                        {
                            var smsRes = await _fxqService.SmsSend(item.Phone, item.SmsContent);
                            if (smsRes != null && smsRes.Code == 10000)
                            {
                                item.SmsSendTime = DateTime.Now;
                                item.FailMsg = string.Empty;
                            }
                            else
                            {
                                item.FailMsg = $"短信发送异常{smsRes?.Msg}";
                                await _openAccountService.Update(item);
                                continue;
                            }
                        }
                        else{
                            item.SmsCode = "1234";
                            item.SmsContent = $"测试环境——您好！您已成功提交工商银行借记卡开户申请，请您及时登录微信小程序进行签署，验证码：{item.SmsCode}。";
                        }

                    }
                    item.SignStatus = CommonSignStatusEnumType.待签署;
                    item.FailMsg = string.Empty;

                    item.Status = OpenAccountStatusEnumType.待开户;
                    await _openAccountService.Update(item);
                }
                catch (Exception ex)
                {
                    item.FailMsg = ex.Message;
                    await _openAccountService.Update(item);
                }
            }

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> BankOpenCardPushJob()
        {
            Random random = new Random();
            var client = _clientFactory.CreateClient();
            string fontPath = Path.Combine(_env.WebRootPath, "Fonts", "msyh.ttf");
            var config = await _settingsService.GetMasterSettings();
            var ossDomain = config.AliBucketDomain;
            var records = await _openAccountService.List(f => 
            f.Status == OpenAccountStatusEnumType.待开户
            &&f.SignStatus== CommonSignStatusEnumType.已完成
            && !string.IsNullOrWhiteSpace(f.WatermarkFrontPath)
            &&!string.IsNullOrWhiteSpace(f.WatermarkBackPath)
            &&string.IsNullOrWhiteSpace(f.FailMsg)
            );

            if (records.Any())
            {
                Guid batchNo = Guid.NewGuid();
                var pushRecords = records.Select(f => new OpenAccountDto()
                {
                    Address = f.CompanyAddress,
                    BatchNo = batchNo,
                    BirthDate = f.BirthDate.Value,
                    BusinessID = f.ID,
                    CardEndDate = f.CardEndDate.Value,
                    CardFrontPath = f.WatermarkFrontPath,
                    CardBackPath = f.WatermarkBackPath,
                    CardID = f.CardID,
                    CardStartDate = f.CardStartDate.Value,
                    CardType = f.CardType,
                    CustomerAddress = f.CompanyAddress,
                    CustomerName = f.CompanyName,
                    Name = f.Name,
                    Phone = f.Phone,
                    Sex = f.Sex,
                }).ToList();
                string json = JsonConvert.SerializeObject(pushRecords);
                var res = await _httpService.PostAsync(ServiceNames.BankService, "/api/IcbcOpenAccountRecord/Apply", json);
                if (res.Code == 200)
                {
                    foreach (var item in records)
                    {
                        item.Status = OpenAccountStatusEnumType.待确认;
                    }
                }
                else
                {
                    foreach (var item in records)
                    {
                        item.FailMsg = res.Message ?? "批量开户推送失败";
                    }
                }
                await _openAccountService.Update(records);
            }
            return Ok();
        }
        #endregion

        #region 入职记录获取CA认证状态

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> HandleEntryCaAuthStatusJob()
        {
            int days = 0;
            try
            {
                var caAuthData = await _httpService.GetAsync<List<CaAuthCardIdDto>>(ServiceNames.CustomerService, $"/api/CaAuthIdentity/ListCaAuthStatus?days={days}",null,false);
                if (caAuthData != null && caAuthData.Count > 0)
                {
                    var cardIdList = caAuthData.Select(f => f.CardID).Distinct().ToList();
                    var entryList = await _entryService.List(f => cardIdList.Contains(f.CardID) && f.IsCaAuth == false&&(f.Status==2||f.Status==4));
                    if (entryList.Any())
                    {
                        var entryIdList = entryList.Select(f => f.ID).ToList();
                        await _entryService.ChangeCaAuth(entryIdList, true);
                    }
                    var userList = await _userService.List(f => cardIdList.Contains(f.CardID) && f.IsCaAuth == false, new Model.PagedModel.UserPagedModel() { Page = 0 });
                    if (userList.Any())
                    {
                        var userIdList = userList.Select(f => f.UserID).ToList();
                        await _userService.ChangeCaAuth(userIdList, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Debug($"定时任务获取Ca认证信息失败，Message:{ex.Message}");
            }

            return Ok();

        }


        #endregion

    }
}
