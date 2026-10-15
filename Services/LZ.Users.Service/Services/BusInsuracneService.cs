using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.IExcelTemplate.busInsurance;
using LZ.Model.InsuranceApi;
using LZ.Model.InsuranceApi.ThirtySix;
using LZ.Model.ModelExtensions;
using LZ.Model.Options;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Services
{


    public class BusInsuracneService : ServicesBase<Live_BusInsuracneOrder>, IBusInsuracneService, IScopedDependency
    {
        private readonly IWebHostEnvironment _env;
        private readonly IEntryService _entryService;
        private readonly IBusInsuranceDayMatchRecordService _dayMatchRecordService;
        private readonly IBusInsuranceWorkPlanService _workPlanService;
        private readonly IBusInsuranceRecordService _recordService;
        private readonly IBusInsuracneOrderService _orderService;
        private readonly IInsuranceApiService _insuranceApiService;
        private readonly IAliyunOSSService _aliyunOSSService;
        private readonly IAliSendInfo _aliSendInfo;
        private readonly IMailSenderService _mailSenderService;
        private readonly IBusInsuranceApiPushService _apiPushService;

        public BusInsuracneService(IEntryService entryService, IBusInsuranceDayMatchRecordService dayMatchRecordService, IBusInsuranceWorkPlanService workPlanService, IBusInsuranceRecordService recordService, IBusInsuracneOrderService orderService, IInsuranceApiService insuranceApiService, IAliyunOSSService aliyunOSSService, IMailSenderService mailSenderService, IWebHostEnvironment env, IAliSendInfo aliSendInfo, IBusInsuranceApiPushService apiPushService)
        {
            _entryService = entryService;
            _dayMatchRecordService = dayMatchRecordService;
            _workPlanService = workPlanService;
            _recordService = recordService;
            _orderService = orderService;
            _insuranceApiService = insuranceApiService;
            _aliyunOSSService = aliyunOSSService;
            _mailSenderService = mailSenderService;
            _env = env;
            _aliSendInfo = aliSendInfo;
            _apiPushService = apiPushService;
        }

        /// <summary>
        /// 待匹配方案 根据配置匹配方案
        /// </summary>
        /// <returns></returns>
        public async Task WorkPlanMatch()
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);

            var orderPlanList = await _orderService.ListOrderPlan(f => f.OrderMonth == insuranceMonth && f.IsEnable);
            var entryInsuranceList = await _entryService.ListInsurance(f =>
            f.CusEnable
            &&f.IsUseBusInsurance
            && f.Status == 2// 在职
            && f.InsuranceType == InsuranceTypeEnumType.系统参保
            //&& f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.待匹配// 待匹配方案
            && !f.IsManuallyInsurance// 非人工参保
            );
            // 获取客户单位
            var cusNameList = entryInsuranceList.Select(f => f.CustomerName).Distinct().ToList();
            //获取工种方案配置
            var workPlanList = await _workPlanService.List(f => cusNameList.Contains(f.CustomerName));

            Live_BusInsuranceWorkPlan workPlan = null;
            Live_V_BusInsuracneOrderPlan orderPlan = null;

            foreach (var item in entryInsuranceList)
            {
                workPlan = workPlanList.FirstOrDefault(f => f.CustomerName == item.CustomerName && f.WorkName == item.WorkName);
                if (workPlan != null)
                {
                
                    if (workPlan.InsuranceType == Model.Enums.InsuranceTypeEnumType.系统参保)
                    {
                        item.InsuranceType = workPlan.InsuranceType;
                        item.IsManuallyInsurance = false;
                        //item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.待推送;
                        item.InsuranceCompany1 = workPlan.InsuranceCompany;
                        item.PlanName1 = workPlan.PlanName;
                        //item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                        orderPlan = orderPlanList.FirstOrDefault(f => f.InsuranceCompany == item.InsuranceCompany1 && f.PlanName == item.PlanName1 && !string.IsNullOrWhiteSpace(f.SignUnitNames) && f.SignUnitNames.Contains(item.SignUnitName));
                        if (orderPlan == null)
                        {
                            orderPlan = orderPlanList.FirstOrDefault(f => f.InsuranceCompany == item.InsuranceCompany1 && f.PlanName == item.PlanName1 && string.IsNullOrWhiteSpace(f.SignUnitNames));
                        }
                        if (orderPlan != null)
                        {
                            if (item.InsuranceCompany1 != orderPlan.InsuranceCompany|| item.InsuranceId1 != orderPlan.OrderNo)
                            {
                                item.InsuranceId1 = orderPlan.OrderNo;
                                item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.待推送;
                            }

                        }
                        else
                        {
                            item.InsuranceId1 = string.Empty;
                            item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.匹配失败订单;
                        }
                    }
                    else if (workPlan.InsuranceType == Model.Enums.InsuranceTypeEnumType.手动参保)
                    {
                        item.InsuranceType = workPlan.InsuranceType;
                        item.IsManuallyInsurance = true;
                        item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.非系统参保;
                        item.InsuranceCompany1 = workPlan.InsuranceCompany;
                        item.PlanName1 = workPlan.PlanName;
                        if (item.PlanStatus1==null|| item.PlanStatus1.Value== Model.Enums.UserEntryPlanStatusEnumType.未参保)
                        {
                            item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                        }
                    }
                    else if (workPlan.InsuranceType == Model.Enums.InsuranceTypeEnumType.不参保)
                    {
                        item.InsuranceType = workPlan.InsuranceType;
                        item.IsManuallyInsurance = false;
                        item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.非系统参保;
                        item.InsuranceCompany1 = string.Empty;
                        item.InsuranceId1 = string.Empty;
                        item.PlanName1 = string.Empty;
                        item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
                    }
                    else
                    {
                        item.InsuranceType = InsuranceTypeEnumType.不参保;
                        item.IsManuallyInsurance = false;
                        item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.非系统参保;
                        item.InsuranceCompany1 = string.Empty;
                        item.InsuranceId1 = string.Empty;
                        item.PlanName1 = string.Empty;
                        item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
                    }
                }
                else
                {
                    item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.匹配失败工种;
                    item.IsManuallyInsurance = false;
                    item.InsuranceCompany1 = string.Empty;
                    item.InsuranceId1 = string.Empty;
                    item.PlanName1 = string.Empty;
                    //item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
                };
            }

            #region 测试流程
            //var failRecords = entryInsuranceList.Where(f => f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.匹配失败工种 || f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.匹配失败订单).Select(f => new BusInsuranceRecordTemplate()
            //{
            //    CustomerName = f.CustomerName,
            //    WorkName = f.WorkName,
            //    Name = f.Name,
            //    InsuranceCompany = f.InsuranceCompany1,
            //    PlanName = f.PlanName1,
            //    WorkClass = f.WorkClass,
            //    OrderNo = f.InsuranceProcessStatus.ToString(),
            //    SignUnitName=f.SignUnitName,
            //    CardID=f.CardID
            //}).Distinct().ToList();
            //var fileBytes = NpoiExcelHelper.WriteExcel(failRecords, "Sheet1");
            //using (var ms = new MemoryStream(fileBytes))
            //{
            //    var res = await _aliyunOSSService.UploadFile(ms, "11.xlsx", false, true);
            //}
            #endregion
            // 更新商保主险信息
            var updateEntryRecord = entryInsuranceList.Select(f => new Live_UserEntry()
            {
                ID = f.ID,
                PlanStatus1 = f.PlanStatus1,
                PlanName1 = f.PlanName1,
                InsuranceCompany1 = f.InsuranceCompany1,
                InsuranceId1 = f.InsuranceId1,
                InsuranceProcessStatus = f.InsuranceProcessStatus,
                IsManuallyInsurance = f.IsManuallyInsurance,
                InsuranceType = f.InsuranceType,
            }).ToList();
            // 更新员工商保信息
            await _entryService.UpdateInsuranceInfo(updateEntryRecord);
        }

        /// <summary>
        /// 再次匹配系统参保数据
        /// </summary>
        /// <returns></returns>
        public async Task WorkPlanMatchByInsuranceCompany(string insuranceCompany)
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            var orderPlanList = await _orderService.ListOrderPlan(f => f.OrderMonth == insuranceMonth&&f.InsuranceCompany==insuranceCompany && f.IsEnable&&!f.IsDayOrder);
            // 获取工种方案配置
            var workPlanList = await _workPlanService.List(f => f.IsEnable && f.InsuranceCompany == insuranceCompany && !string.IsNullOrWhiteSpace(f.InsuranceCompany) && !string.IsNullOrWhiteSpace(f.PlanName) && f.InsuranceType == Model.Enums.InsuranceTypeEnumType.系统参保);
            var customerList = workPlanList.Select(f => f.CustomerName).Distinct().ToList();
            foreach (var customerNameItem in customerList)
            {
                var cusWorkPlans = workPlanList.Where(f => f.CustomerName == customerNameItem).ToList();
                var workNames = cusWorkPlans.Select(f => f.WorkName).Distinct().ToList();
                var entryInsuranceList = await _entryService.ListInsurance(f =>
                f.CustomerName == customerNameItem
                && f.CusEnable
                && f.IsUseBusInsurance
                && f.Status == 2// 在职
                && workNames.Contains(f.WorkName)
                 && f.InsuranceType == InsuranceTypeEnumType.系统参保
                && !f.IsManuallyInsurance// 非人工参保
                );
                if (entryInsuranceList == null || entryInsuranceList.Count <= 0)
                {
                    continue;
                }

                foreach (var item in entryInsuranceList)
                {
                    var workPlan = cusWorkPlans.FirstOrDefault(f => f.WorkName == item.WorkName);
                    // 根据结果搜索必定不为空
                    if (workPlan != null)
                    {
                        item.InsuranceType = workPlan.InsuranceType;
                        item.IsManuallyInsurance = false;
                        item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.待推送;
                        item.InsuranceCompany1 = workPlan.InsuranceCompany;
                        item.PlanName1 = workPlan.PlanName;
                        item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                        item.InsuranceWorkName = workPlan.InsuranceWorkName;
                        var orderPlan = orderPlanList.FirstOrDefault(f => f.InsuranceCompany == item.InsuranceCompany1 && f.PlanName == item.PlanName1 && !string.IsNullOrWhiteSpace(f.SignUnitNames) && f.SignUnitNames.Contains(item.SignUnitName));
                        if (orderPlan == null)
                        {
                            orderPlan = orderPlanList.FirstOrDefault(f => f.InsuranceCompany == item.InsuranceCompany1 && f.PlanName == item.PlanName1 && string.IsNullOrWhiteSpace(f.SignUnitNames));
                        }
                        if (orderPlan != null)
                        {
                            if (item.InsuranceId1 != orderPlan.OrderNo)
                            {
                                item.InsuranceId1 = orderPlan.OrderNo;
                                item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.待推送;
                            }
                        }
                        else
                        {
                            item.InsuranceId1 = string.Empty;
                            item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.匹配失败订单;
                        }
                    }
                    else {

                        item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.匹配失败工种;
                        item.IsManuallyInsurance = false;
                        item.InsuranceCompany1 = string.Empty;
                        item.InsuranceId1 = string.Empty;
                        item.PlanName1 = string.Empty;
                        item.InsuranceWorkName = string.Empty;
                        item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
                    }
                }

                // 更新商保主险信息
                var updateEntryRecord = entryInsuranceList.Select(f => new Live_UserEntry()
                {
                    ID = f.ID,
                    PlanStatus1 = f.PlanStatus1,
                    PlanName1 = f.PlanName1,
                    InsuranceCompany1 = f.InsuranceCompany1,
                    InsuranceId1 = f.InsuranceId1,
                    InsuranceProcessStatus = f.InsuranceProcessStatus,
                    IsManuallyInsurance=f.IsManuallyInsurance,
                    InsuranceType = f.InsuranceType,
                    InsuranceWorkName = f.InsuranceWorkName,
                }).ToList();
                // 更新员工商保信息
                await _entryService.UpdateInsuranceInfo(updateEntryRecord);





            }
        }
        public async Task MatchByOrderNo(string insuranceCompany,string orderNo,bool isGenerateDayMatchRecord=true)
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);
            var order = await _orderService.ListOrderPlan(f =>f.IsEnable&&!f.IsDayOrder&&f.InsuranceCompany == insuranceCompany && f.OrderNo == orderNo && f.OrderMonth == insuranceMonth);

            #region 清理今日已匹配记录

            // 删除今日 未推送的记录  （允许多次匹配，删除上一次处理记录）
            await _dayMatchRecordService.Delete(f => f.InsuranceCompany == insuranceCompany && f.OrderNo == orderNo && f.Status != Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送);
            await Task.Delay(100);

            #endregion
            // 该订单号的保全
            var recordList = await _recordService.List(f => f.InsuranceCompany == insuranceCompany && f.InsuranceMonth == insuranceMonth && f.OrderNo == orderNo && f.Status != Model.Enums.BusInsuranceRecordStatusEnumType.已减员);
            // 获取此订单号的在职信息:非人工参保
            var entryList = await _entryService.ListInsurance(f => 
            f.Status == 2 
            && f.InsuranceCompany1 == insuranceCompany
             && f.InsuranceType == InsuranceTypeEnumType.系统参保
            && f.InsuranceId1 == orderNo
            && f.IsManuallyInsurance == false 
            && !string.IsNullOrWhiteSpace(f.WorkName) 
            && !string.IsNullOrWhiteSpace(f.WorkClass) 
            && (f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.已推送 || f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.待推送));
            List<int> recordIds = new List<int>();
            List<Live_UserEntry> updateEntryList = new List<Live_UserEntry>();
            Live_BusInsuranceRecord record = null;
            Live_UserEntry entry = null;
            foreach (var item in entryList)
            {
                if (insuranceCompany == "安责险 T" || insuranceCompany == "利宝保险")
                {
                    record = recordList.FirstOrDefault(f =>
                  f.Name.Trim() == item.Name.Trim()
               && f.CardID.Equals(item.CardID, StringComparison.OrdinalIgnoreCase)
               && item.CustomerName.Contains(f.CustomerName, StringComparison.OrdinalIgnoreCase)
               && f.SignUnitName.Equals(item.SignUnitName, StringComparison.OrdinalIgnoreCase)
               );
                }
                else if (insuranceCompany == "大地财险")
                {
                    record = recordList.FirstOrDefault(f =>
                 f.Name.Trim() == item.Name.Trim()
                 && f.CardID.Equals(item.CardID, StringComparison.OrdinalIgnoreCase)
                 && f.CustomerName.Equals(item.CustomerName, StringComparison.OrdinalIgnoreCase)
                 && f.SignUnitName.Equals(item.SignUnitName, StringComparison.OrdinalIgnoreCase)
                 && f.WorkClass == item.WorkClass
                 );
                }
                else if (insuranceCompany == "众安保险")
                {
                    record = recordList.FirstOrDefault(f =>
                   f.Name.Trim() == item.Name.Trim()
                 && f.CardID.Equals(item.CardID, StringComparison.OrdinalIgnoreCase)
                 && item.CustomerName.Contains(f.CustomerName)
                 && f.SignUnitName.Equals(item.SignUnitName, StringComparison.OrdinalIgnoreCase)
                 && f.WorkName == item.WorkName
                 && f.WorkClass == item.WorkClass
                 );
                }
                else if (insuranceCompany == "大地365")
                {
                    record = recordList.FirstOrDefault(f =>
                   f.Name.Trim() == item.Name.Trim()
                 && f.CardID.Equals(item.CardID, StringComparison.OrdinalIgnoreCase)
                 && item.CustomerName.Contains(f.CustomerName)
                 && f.SignUnitName.Equals(item.SignUnitName, StringComparison.OrdinalIgnoreCase)
                 && f.WorkName == item.WorkName
                 && f.PlanName == item.PlanName1
                 );
                }
                else
                {
                    record = recordList.FirstOrDefault(f =>
                  f.Name.Trim() == item.Name.Trim()
                 && f.CardID.Equals(item.CardID, StringComparison.OrdinalIgnoreCase)
                 && f.CustomerName.Equals(item.CustomerName, StringComparison.OrdinalIgnoreCase)
                && f.SignUnitName.Equals(item.SignUnitName, StringComparison.OrdinalIgnoreCase)
                 && f.WorkName == item.WorkName
                 && f.WorkClass == item.WorkClass
                 );
                }
                if (record != null)
                {
                    recordIds.Add(record.ID);
                    if (item.PlanStatus1 != Model.Enums.UserEntryPlanStatusEnumType.已增员 || item.InsuranceProcessStatus != Model.Enums.InsuranceProcessEnumType.已推送)
                    {
                        entry = new Live_UserEntry()
                        {
                            ID = item.ID,
                            PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.已增员,
                            InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.已推送,
                        };
                        item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.已增员;
                        item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.已推送;
                        updateEntryList.Add(entry);
                    }
                }
                else
                {
                    if (item.PlanStatus1 != Model.Enums.UserEntryPlanStatusEnumType.待增员 || item.InsuranceProcessStatus != Model.Enums.InsuranceProcessEnumType.待推送)
                    {
                        entry = new Live_UserEntry()
                        {
                            ID = item.ID,
                            PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员,
                            InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.待推送,
                        };
                        item.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                        item.InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.待推送;
                        updateEntryList.Add(entry);
                    }
                }
            }
            foreach (var item in recordList)
            {
                if (recordIds.Contains(item.ID))
                {
                    item.MatchDate = today;
                    item.Status = Model.Enums.BusInsuranceRecordStatusEnumType.已增员;
                }
                else
                {
                    item.MatchDate = today;
                    item.Status = Model.Enums.BusInsuranceRecordStatusEnumType.待减员;
                }
            }




            List<Live_BusInsuranceDayMatchRecord> allMatchRecordList = new List<Live_BusInsuranceDayMatchRecord>();

            var addMatchRecords = entryList.Where(f => f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.待推送 && f.PlanStatus1 == Model.Enums.UserEntryPlanStatusEnumType.待增员).Select(f => new Live_BusInsuranceDayMatchRecord()
            {
                ID = 0,
                Age = 0,
                CardID = f.CardID,
                CreateTime = DateTime.Now,
                CustomerName = f.CustomerName,
                DelMatchRecordID = 0,
                EndDate = insuranceMonthEnd,
                IndecaseId = f.InsuranceId1,
                OrderNo = f.InsuranceId1,
                InsuranceCompany = f.InsuranceCompany1,
                InsuranceWorkClass = f.WorkClass,
                WorkName = f.WorkName,
                InsuranceWorkName = f.InsuranceWorkName??f.WorkName,
                WorkClass = f.WorkClass,
                IsAddRecord = true,
                Msg = string.Empty,
                Name = f.Name,
                PlanName = f.PlanName1,
                PlanNameID = order.First(ff=>ff.PlanName==f.PlanName1).PlanNameID,
                PushResult = string.Empty,
                RecordDate = today,
                Remark = string.Empty,
                Sex = string.Empty,
                SignUnitName = f.SignUnitName,
                StartDate = nextDay,
                Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                UserEntryID = f.ID,
            }).ToList();
            allMatchRecordList.AddRange(addMatchRecords);

            #region 增减匹配 生成减员匹配记录
            if (insuranceCompany != "众安保险")
            {

                if (insuranceCompany == "安责险 T")
                {
                    var cardIDList = addMatchRecords.Select(f => f.CardID).ToList();

                    var mustDelRecords = recordList.Where(f => f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && cardIDList.Contains(f.CardID)).ToList();

                    List<int> delRecordIds = new List<int>();
                    // 必须减员的

                    foreach (var item in addMatchRecords)
                    {
                        var ageAndGender = IdCardHelper.GetAgeAndGender(item.CardID);
                        item.Age = ageAndGender.Age;
                        item.Sex = ageAndGender.Gender;
                        // 优先匹配必减记录
                        record = mustDelRecords.FirstOrDefault(f => f.CardID == item.CardID && f.SignUnitName == item.SignUnitName && f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && !delRecordIds.Contains(f.ID));
                        if (record == null)
                        {
                            record = mustDelRecords.FirstOrDefault(f => f.SignUnitName == item.SignUnitName && f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && !delRecordIds.Contains(f.ID));
                        }

                        if (record == null)
                        {
                            record = recordList.FirstOrDefault(f => f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && f.SignUnitName == item.SignUnitName && !delRecordIds.Contains(f.ID));
                        }
                        if (record != null)
                        {
                            delRecordIds.Add(record.ID);
                            item.DelMatchRecordID = record.ID;
                        }
                    }

                    foreach (var item in mustDelRecords)
                    {
                        if (!delRecordIds.Contains(item.ID))
                        {
                            delRecordIds.Add(item.ID);
                        }
                    }
                    var delMatchRecords = recordList.Where(f => delRecordIds.Contains(f.ID)).Select(f => new Live_BusInsuranceDayMatchRecord()
                    {
                        ID = 0,
                        Age = f.Age,
                        CardID = f.CardID,
                        CreateTime = DateTime.Now,
                        CustomerName = f.CustomerName,
                        DelMatchRecordID = f.ID,
                        EndDate = insuranceMonthEnd,
                        IndecaseId = string.Empty,
                        OrderNo = f.OrderNo,
                        InsuranceCompany = f.InsuranceCompany,
                        InsuranceWorkClass = f.WorkClass,
                        WorkClass = f.WorkClass,
                        InsuranceWorkName = f.WorkName,
                        IsAddRecord = false,
                        Msg = string.Empty,
                        Name = f.Name,
                        PlanName = f.PlanName,
                        PlanNameID = f.PlanNameID ?? 0,
                        PushResult = string.Empty,
                        RecordDate = today,
                        Remark = string.Empty,
                        Sex = f.Sex,
                        SignUnitName = f.SignUnitName,
                        StartDate = f.StartDate,
                        Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                        WorkName = f.WorkName,
                        UserEntryID = 0
                    }).ToList();
                    allMatchRecordList.AddRange(delMatchRecords);


                }
                else if (insuranceCompany == "利宝保险")
                {

                    // 待减员则全部减员
                    var delMatchRecords = recordList.Where(f => f.Status == BusInsuranceRecordStatusEnumType.待减员).Select(f => new Live_BusInsuranceDayMatchRecord()
                    {
                        ID = 0,
                        Age = f.Age,
                        CardID = f.CardID,
                        CreateTime = DateTime.Now,
                        CustomerName = f.CustomerName,
                        DelMatchRecordID = f.ID,
                        EndDate = insuranceMonthEnd,
                        IndecaseId = string.Empty,
                        OrderNo = f.OrderNo,
                        InsuranceCompany = f.InsuranceCompany,
                        InsuranceWorkClass = f.WorkClass,
                        WorkClass = f.WorkClass,
                        InsuranceWorkName = f.WorkName,
                        IsAddRecord = false,
                        Msg = string.Empty,
                        Name = f.Name,
                        PlanName = f.PlanName,
                        PlanNameID = f.PlanNameID ?? 0,
                        PushResult = string.Empty,
                        RecordDate = today,
                        Remark = string.Empty,
                        Sex = f.Sex,
                        SignUnitName = f.SignUnitName,
                        StartDate = f.StartDate,
                        Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                        WorkName = f.WorkName,
                        UserEntryID = 0
                    }).ToList();
                    allMatchRecordList.AddRange(delMatchRecords);


                }
                else if (insuranceCompany == "大地365")
                {
                    allMatchRecordList.Clear();

                    // 减员记录必须匹配增员记录
                    List<Live_BusInsuranceDayMatchRecord> mustDayDelRecord = new List<Live_BusInsuranceDayMatchRecord>();
                    // 获取减员身份证号码
                    List<string> delCardIDs = recordList.Select(f => f.CardID).ToList();
                    // 增员身份证号码
                    List<string> addCardIDs = addMatchRecords.Select(f => f.CardID).ToList();
                    //  今日 即要增员又要减员
                    var addDelCardIDs = addCardIDs.Intersect(delCardIDs).ToList();
                    // 添加必须减员名单
                    List<int> delRecordIds = new List<int>();
                    List<Live_BusInsuranceDayMatchRecord> matchFailRecords = new List<Live_BusInsuranceDayMatchRecord>();
                    foreach (var item in recordList)
                    {
                        if (addDelCardIDs.Contains(item.CardID))
                        {
                            item.Weight = 100;
                        }
                        else
                        {
                            item.Weight = 0;
                        }
                    }
                    recordList = recordList.OrderByDescending(f => f.Weight ?? 0).ToList();
                    foreach (var item in addMatchRecords)
                    {
                        if (addDelCardIDs.Contains(item.CardID))
                        {
                            item.Weight = 100;
                        }
                        else
                        {
                            item.Weight = 0;
                        }
                    }
                    addMatchRecords = addMatchRecords.OrderByDescending(f => f.Weight ?? 0).ToList();
                    List<Live_BusInsuranceDayMatchRecord> delMatchRecords = new List<Live_BusInsuranceDayMatchRecord>();

                    // 优先匹配自身
                    foreach (var item in addDelCardIDs)
                    {
                        var sameAddRecord = addMatchRecords.First(f => f.CardID == item);
                        var sameDelRecord = recordList.First(f => f.CardID == item && sameAddRecord.PlanName == f.PlanName);
                        if (sameDelRecord != null)
                        {


                            var ageAndGender = IdCardHelper.GetAgeAndGender(item);
                            sameAddRecord.Age = ageAndGender.Age;
                            sameAddRecord.Sex = ageAndGender.Gender;
                            sameAddRecord.DelMatchRecordID = sameDelRecord.ID;
                            sameAddRecord.DelMatchRecordName = sameDelRecord.Name;
                            sameAddRecord.DelMatchRecordCardID = sameDelRecord.CardID;
                            sameAddRecord.DelMatchRecordPlanName = sameDelRecord.PlanName;
                            delMatchRecords.Add(new Live_BusInsuranceDayMatchRecord()
                            {
                                ID = 0,
                                Age = sameDelRecord.Age,
                                CardID = sameDelRecord.CardID,
                                CreateTime = DateTime.Now,
                                CustomerName = sameDelRecord.CustomerName,
                                DelMatchRecordID = sameDelRecord.ID,
                                EndDate = insuranceMonthEnd,
                                IndecaseId = string.Empty,
                                OrderNo = sameDelRecord.OrderNo,
                                InsuranceCompany = sameDelRecord.InsuranceCompany,
                                InsuranceWorkClass = sameDelRecord.WorkClass,
                                WorkClass = sameDelRecord.WorkClass,
                                InsuranceWorkName = sameDelRecord.InsuranceWorkName,
                                IsAddRecord = false,
                                Msg = string.Empty,
                                Name = sameDelRecord.Name,
                                PlanName = sameDelRecord.PlanName,
                                PlanNameID = sameDelRecord.PlanNameID ?? 0,
                                PushResult = string.Empty,
                                RecordDate = today,
                                Remark = string.Empty,
                                Sex = sameDelRecord.Sex,
                                SignUnitName = sameDelRecord.SignUnitName,
                                StartDate = sameDelRecord.StartDate,
                                Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                                WorkName = sameDelRecord.WorkName,
                                UserEntryID = 0
                            });
                            allMatchRecordList.Add(sameAddRecord);
                            delRecordIds.Add(sameDelRecord.ID);
                        }
                    }
                    foreach (var item in addMatchRecords)
                    {
                        if (item.DelMatchRecordID>0)
                        {
                            // 已匹配过 则跳过再次匹配
                            continue;
                        }
                        var ageAndGender = IdCardHelper.GetAgeAndGender(item.CardID);
                        item.Age = ageAndGender.Age;
                        item.Sex = ageAndGender.Gender;
                        // 优先选择同一身份证号码
                        record = recordList.FirstOrDefault(f => f.CardID == item.CardID &&f.PlanName==item.PlanName && f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && !delRecordIds.Contains(f.ID));
                        if (record == null)
                        {
                            record = recordList.FirstOrDefault(f => f.PlanName == item.PlanName && f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && !delRecordIds.Contains(f.ID));
                        }
                        if (record != null)
                        {
                            delRecordIds.Add(record.ID);
                            item.DelMatchRecordID = record.ID;
                            item.DelMatchRecordName = record.Name;
                            item.DelMatchRecordCardID = record.CardID;
                            item.DelMatchRecordPlanName = record.PlanName;
                            delMatchRecords.Add(new Live_BusInsuranceDayMatchRecord()
                            {
                                ID = 0,
                                Age = record.Age,
                                CardID = record.CardID,
                                CreateTime = DateTime.Now,
                                CustomerName = record.CustomerName,
                                DelMatchRecordID = record.ID,
                                EndDate = insuranceMonthEnd,
                                IndecaseId = string.Empty,
                                OrderNo = record.OrderNo,
                                InsuranceCompany = record.InsuranceCompany,
                                InsuranceWorkClass = record.WorkClass,
                                WorkClass = record.WorkClass,
                                InsuranceWorkName = record.WorkName,
                                IsAddRecord = false,
                                Msg = string.Empty,
                                Name = record.Name,
                                PlanName = record.PlanName,
                                PlanNameID = record.PlanNameID ?? 0,
                                PushResult = string.Empty,
                                RecordDate = today,
                                Remark = string.Empty,
                                Sex = record.Sex,
                                SignUnitName = record.SignUnitName,
                                StartDate = record.StartDate,
                                Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                                WorkName = record.WorkName,
                                UserEntryID = 0
                            });
                            allMatchRecordList.Add(item);
                        }
                        else
                        {
                            if (!addDelCardIDs.Contains(item.CardID))
                            {
                                allMatchRecordList.Add(item);
                            }
                            else
                            {
                                allMatchRecordList.Add(item);
                                // 增减替换但是无人替换 
                                //matchFailRecords.Add(item);
                            }
                        }
                    }
                    // 发送匹配失败电子邮件
                    //if (matchFailRecords.Any())
                    //{
                    //    foreach (var item in matchFailRecords)
                    //    {
                    //        item.Status = BusInsuranceDayMatchRecordStatusEnumType.待推送;
                    //        item.Msg = "已在保，无法减员，请人工处理！";
                    //    }
                    //}

                    allMatchRecordList.AddRange(delMatchRecords);
                }
                else
                {
                    List<int> delRecordIds = new List<int>();
                    foreach (var item in addMatchRecords)
                    {
                        var ageAndGender = IdCardHelper.GetAgeAndGender(item.CardID);
                        item.Age = ageAndGender.Age;
                        item.Sex = ageAndGender.Gender;
                        // 优先选择同一身份证号码
                        record = recordList.FirstOrDefault(f => f.CardID == item.CardID && f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && !delRecordIds.Contains(f.ID));
                        if (record == null)
                        {
                            record = recordList.FirstOrDefault(f => f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员 && !delRecordIds.Contains(f.ID));
                        }
                        if (record != null)
                        {
                            delRecordIds.Add(record.ID);
                            item.DelMatchRecordID = record.ID;
                        }
                    }
                    var delMatchRecords = recordList.Where(f => delRecordIds.Contains(f.ID)).Select(f => new Live_BusInsuranceDayMatchRecord()
                    {
                        ID = 0,
                        Age = f.Age,
                        CardID = f.CardID,
                        CreateTime = DateTime.Now,
                        CustomerName = f.CustomerName,
                        DelMatchRecordID = f.ID,
                        EndDate = insuranceMonthEnd,
                        IndecaseId = string.Empty,
                        OrderNo = f.OrderNo,
                        InsuranceCompany = f.InsuranceCompany,
                        InsuranceWorkClass = f.WorkClass,
                        WorkClass = f.WorkClass,
                        InsuranceWorkName = f.WorkName,
                        IsAddRecord = false,
                        Msg = string.Empty,
                        Name = f.Name,
                        PlanName = f.PlanName,
                        PlanNameID = f.PlanNameID ?? 0,
                        PushResult = string.Empty,
                        RecordDate = today,
                        Remark = string.Empty,
                        Sex = f.Sex,
                        SignUnitName = f.SignUnitName,
                        StartDate = f.StartDate,
                        Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                        WorkName = f.WorkName,
                        UserEntryID = 0
                    }).ToList();
                    allMatchRecordList.AddRange(delMatchRecords);
                }
            }

            #endregion

            // 跟新员工 参保状态、流程状态
            await _entryService.UpdateInsuranceProcessStatus(updateEntryList);
            // 更新 状态 及 匹配日期
            await _recordService.UpdateRecordMatchStatus(recordList);
            if (isGenerateDayMatchRecord)
            {
                // 生成今日匹配记录
                await _dayMatchRecordService.Add(allMatchRecordList);
            }
        }

        /// <summary>
        /// 阳光保险推送
        /// api 推送
        /// </summary>
        /// <returns></returns>

        public async Task PushYangGuang()
        {
            var matchRecords = await _dayMatchRecordService.List(f => f.InsuranceCompany == "阳光保险公司" && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送);
            var addMatchRecords = matchRecords.Where(f => f.IsAddRecord).ToList();
            var delMatchRecords = matchRecords.Where(f => !f.IsAddRecord).ToList();
            var delRecordGroups = delMatchRecords.GroupBy(f => new { f.OrderNo }).ToList();
            var addRecordGroups = addMatchRecords.GroupBy(f => new { f.OrderNo }).ToList();
            #region 减员
            foreach (var groupItem in delRecordGroups)
            {
                var order = await _orderService.Get(f => f.OrderNo == groupItem.Key.OrderNo);
                var delRequest = new BatchDelEmployeeRequest()
                {
                    InsureId = order.OrderNo,
                    CustomerList = groupItem.Select(f => new DelCustomerinfo()
                    {
                        Custemername = f.Name,
                        Custemernum = f.CardID,
                    }).ToList(),
                };
                var resp = await _insuranceApiService.BatchDelEmployee(delRequest);
                if (resp.Code == 200)
                {
                    foreach (var item in groupItem)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送;
                        item.Msg = string.Empty;
                        item.IndecaseId = resp.IndecaseId;
                    }
                }
                else
                {
                    foreach (var item in groupItem)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.推送异常;
                        item.Msg = resp.Msg;
                    }
                }
                await _dayMatchRecordService.Update(groupItem.ToList());
            }
            #endregion
            // 给保险公司处理减员记录时间
            await Task.Delay(3000);
            #region 增员
            foreach (var groupItem in addRecordGroups)
            {

                var order = await _orderService.Get(f => f.OrderNo == groupItem.Key.OrderNo);
                var addRequest = new BatchAddEmployeeRequest()
                {
                    GnteeId = order.PlanNameID,
                    InsureId = order.OrderNo,
                    GnteeName = order.PlanName,
                    CustomerList = groupItem.Select(f => new AddCustomerinfo()
                    {
                        Custemerage = f.Age.ToString(),
                        Custemername = f.Name,
                        Custemernum = f.CardID,
                        Custemersex = f.Sex,
                        Custemertype = "身份证",
                        Guyuangongzhong = f.WorkName,
                        Zhiyeleibei = f.WorkClass.Substring(0, 1),
                        Ldhtdw = f.SignUnitName,
                        Sjygdw = f.CustomerName,
                    }).ToList(),
                };

                foreach (var item in addRequest.CustomerList)
                {
                    if (item.Guyuangongzhong== "食品加工员")
                    {
                        item.Zhiyeleibei = "3";
                    }
                }


                var resp = await _insuranceApiService.BatchAddEmployee(addRequest);
                if (resp.Code == 200)
                {
                    foreach (var item in groupItem)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送;
                        item.Msg = string.Empty;
                        item.IndecaseId = resp.IndecaseId;
                    }
                }
                else
                {
                    foreach (var item in groupItem)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.推送异常;
                        item.Msg = resp.Msg;
                    }
                }
                await _dayMatchRecordService.Update(groupItem.ToList());
            }
            #endregion
        }
        /// <summary>
        /// 大地财险推送
        /// 邮件推送
        /// </summary>
        /// <returns></returns>
        public async Task PushDaDi()
        {
             var today=DateTime.Today;
            var nextDay= today.AddDays(1);
            var matchRecords = await _dayMatchRecordService.List(f => f.InsuranceCompany == "大地财险" && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送);

            var pushRecords = matchRecords.Select(f => new DaDiInsuranceExcelTemp()
            {
                Age = f.Age,
                CardID = f.CardID,
                CustomerName = f.CustomerName,
                Name = f.Name,
                PushType = f.IsAddRecord ? "新增" : "删除",
                SignUnitName = f.SignUnitName,
                StartDate = nextDay.ToString("yyyy-MM-dd"),
                WorkName = f.WorkName,
            }).OrderBy(f => f.PushType).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "Sheet1");
            if (_env.IsProduction())
            {
                var ccAddressList = new List<MimeKit.MailboxAddress>() { new MimeKit.MailboxAddress("抄送人", "xuyuannb@ccic-net.com.cn") };
                var isSuc = await _mailSenderService.SendUseForInsuranceAsync("zhulynb@ccic-net.com.cn", $"大地商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"大地商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", ccAddressList, "商保推送.xlsx", fileBytes);
                //await _mailSenderService.SendUseForInsuranceAsync("xuyuannb@ccic-net.com.cn", $"大地商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"大地商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", "商保推送.xlsx", fileBytes, false);
                if (isSuc)
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送;
                        item.Msg = string.Empty;
                    }
                }
                else
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.推送异常;
                        item.Msg = "邮件推送失败";
                    }
                }
                await _dayMatchRecordService.Update(matchRecords);
            }
            else
            {
                await _mailSenderService.SendUseForInsuranceAsync("zk@cn-labor.cn", $"大地商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"众安商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）",null, "商保推送.xlsx", fileBytes);
            }
            await _dayMatchRecordService.Update(matchRecords);
        }

        /// <summary>
        /// 众安保险推送
        /// 邮件推送
        /// 无需执行推送处理
        /// </summary>
        /// <returns></returns>
        public async Task PushZhongAn()
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);

            var matchRecords = await _dayMatchRecordService.List(f => f.InsuranceCompany == "众安保险" && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送&&f.RecordDate==today);
            var dayPushRecords = matchRecords.Select(f => new ZhongAnInsuranceExcelTemp()
            {
                CardID = f.CardID,
                CustomerName = f.CustomerName,
                Name = f.Name,
                PlanName = f.PlanName,
                SignUnitName = f.SignUnitName,
                StartDate = f.StartDate?.ToString("yyyy-MM-dd"),
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
            }).ToList();
            var addRecords = await _recordService.List(f =>f.InsuranceCompany=="众安保险"&& f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.已增员&&f.InsuranceMonth== insuranceMonth);
            var pushRecords = addRecords.Select(f => new ZhongAnInsuranceExcelTemp()
            {
                CardID = f.CardID,
                CustomerName = f.CustomerName,
                Name = f.Name,
                PlanName = f.PlanName,
                SignUnitName = f.SignUnitName,
                StartDate = today.ToString("yyyy-MM-dd"),
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
            }).ToList();
            pushRecords.AddRange(dayPushRecords);
            foreach (var item in pushRecords)
            {
                if (item.WorkName== "车床工（关门）")
                {
                    item.WorkName = "数控机床操作工";
                }

                if (item.CustomerName.Contains("劳动关系-"))
                {
                    item.CustomerName = item.CustomerName.Replace("劳动关系-", "");
                }

                if (!item.CustomerName.EndsWith("公司"))
                {
                    item.CustomerName = item.CustomerName.Split("公司")[0]+"公司";
                }

                if (item.WorkClass != "5类")
                {
                    item.WorkClass = "1-4类";
                }
            }
            var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "Sheet1");
            if (_env.IsProduction())
            {
                var ccAddressList = new List<MimeKit.MailboxAddress>() { new MimeKit.MailboxAddress("抄送人", "jiangbw@yanshiaq-nb.com") };

                var isSuc = await _mailSenderService.SendUseForInsuranceAsync("shihj@qidianbx.com", $"众安商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"众安商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", ccAddressList,$"商保推送_{DateTime.Today.ToString("yyyy-MM-dd")}.xlsx", fileBytes);
                //await _mailSenderService.SendUseForInsuranceAsync("jiangbw@yanshiaq-nb.com", $"众安商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"众安商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"商保推送_{DateTime.Today.ToString("yyyy-MM-dd")}.xlsx", fileBytes, false);
                if (isSuc)
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送;
                        item.Msg = string.Empty;
                    }
                }
                else
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.推送异常;
                        item.Msg = "邮件推送失败";
                    }
                }
                await _dayMatchRecordService.Update(matchRecords);
            }
            else
            { 
                await _mailSenderService.SendUseForInsuranceAsync("zk@cn-labor.cn", $"众安商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"众安商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）",null, $"商保推送_ceshi _{DateTime.Today.ToString("yyyy-MM-dd")}.xlsx", fileBytes);
            }

        }
        /// <summary>
        /// 安责 T保险推送
        /// 邮件推送
        /// </summary>
        /// <returns></returns>
        public async Task PushAnZeT()
        {
            var matchRecords = await _dayMatchRecordService.List(f => f.InsuranceCompany == "安责险 T" && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送);
            var pushRecords = matchRecords.Select(f => new AnZeInsuranceExcelTemp()
            {
                CardID = f.CardID,
                CustomerName = f.CustomerName,
                Name = f.Name,
                PushType = f.IsAddRecord ? "加" : "减",
                SignUnitName = f.SignUnitName,
                WorkName = f.WorkName,
            }).OrderBy(f => f.PushType).ToList();
            var addPushRecords = pushRecords.Where(f => f.PushType == "加").ToList();
            var delPushRecords = pushRecords.Where(f => f.PushType == "减").ToList();

            var addFileBytes = NpoiExcelHelper.WriteExcel(addPushRecords, "Sheet1");
            var delFileBytes = NpoiExcelHelper.WriteExcel(delPushRecords, "Sheet1");
            Dictionary<string, byte[]> dss = new Dictionary<string, byte[]>();
            dss.Add("增员.xlsx", addFileBytes);
            dss.Add("减员.xlsx", delFileBytes);
            if (_env.IsProduction())
            {
                var isSuc = await _mailSenderService.SendUseForInsuranceAsync("liusiyao_rb@qq.com", $"安责 T商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"安责 T商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", null,dss);
                if (isSuc)
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送;
                        item.Msg = string.Empty;
                    }
                }
                else
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.推送异常;
                        item.Msg = "邮件推送失败";
                    }
                }
                await _dayMatchRecordService.Update(matchRecords);
            }
            else
            {
                var isSuc = await _mailSenderService.SendUseForInsuranceAsync("zk@cn-labor.cn", $"安责 T商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"安责 T商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", null,dss);
            }
        }
        /// <summary>
        /// 利宝保险
        /// 邮件推送
        /// </summary>
        /// <returns></returns>
        public async Task PushLiBao()
        {
            var matchRecords = await _dayMatchRecordService.List(f => f.InsuranceCompany == "利宝保险" && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送);
            var pushRecords = matchRecords.Select(f => new AnZeInsuranceExcelTemp()
            {
                CardID = f.CardID,
                CustomerName = f.CustomerName,
                Name = f.Name,
                PushType = f.IsAddRecord ? "加" : "减",
                SignUnitName = f.SignUnitName,
                WorkName = f.WorkName,
            }).OrderBy(f => f.PushType).ToList();
            var addPushRecords = pushRecords.Where(f => f.PushType == "加").ToList();
            var delPushRecords = pushRecords.Where(f => f.PushType == "减").ToList();
            var addFileBytes = NpoiExcelHelper.WriteExcel(addPushRecords, "Sheet1");
            var delFileBytes = NpoiExcelHelper.WriteExcel(delPushRecords, "Sheet1");
            Dictionary<string, byte[]> dss = new Dictionary<string, byte[]>();
            dss.Add("减员.xlsx", delFileBytes);
            dss.Add("增员.xlsx", addFileBytes);
            if (_env.IsProduction())
            {
                var isSuc = await _mailSenderService.SendUseForInsuranceAsync("liusiyao_lb@qq.com", $"利宝商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"利宝商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", null,dss);

                if (isSuc)
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送;
                        item.Msg = string.Empty;
                    }
                }
                else
                {
                    foreach (var item in matchRecords)
                    {
                        item.Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.推送异常;
                        item.Msg = "邮件推送失败";
                    }
                }
                await _dayMatchRecordService.Update(matchRecords);
            }
            else
            {
                var isSuc = await _mailSenderService.SendUseForInsuranceAsync("zk@cn-labor.cn", $"利宝商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"利宝商保推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", null,dss);
            }

            await _dayMatchRecordService.Update(matchRecords);


        }
        public async Task PushDaDi365()
        {
            var today = DateTime.Today;
            var effectiveDate = today.AddDays(1).ToString("yyyy-MM-dd");
            var matchRecords = await _dayMatchRecordService.List(f =>f.RecordDate==today&& f.InsuranceCompany == "大地365" && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送);
            var addMatchRecords = matchRecords.Where(f => f.IsAddRecord).ToList();

            var replaceList = addMatchRecords.Where(f => f.DelMatchRecordID > 0).ToList();
            var addList = addMatchRecords.Where(f => f.DelMatchRecordID <= 0).ToList();

            var pushList = new List<Live_BusInsuranceApiPushExtension>();
            var now = DateTime.Now;

            if (replaceList.Any())
            {
                var replacePushId = Guid.NewGuid();
                pushList.Add(new Live_BusInsuranceApiPushExtension
                {
                    ID = replacePushId,
                    InsuranceCompany = "大地365",
                    RecordDate = today,
                    PushType = "替换",
                    Status = BusInsuranceApiPushStatusEnumType.待推送,
                    AddCount = replaceList.Count,
                    DelCount = replaceList.Count,
                    ConfirmUrl = string.Empty,
                    CreateTime = now,
                    Details = replaceList.Select(f => new Live_BusInsuranceApiPushDetail
                    {
                        ID = Guid.NewGuid(),
                        BusInsuranceApiPushID = replacePushId,
                        Name = f.Name,
                        CardID = f.CardID,
                        Age = f.Age,
                        Sex = f.Sex ?? string.Empty,
                        Phone = string.Empty,
                        CustomerName = f.CustomerName,
                        OrderNo = f.OrderNo,
                        PlanName = f.PlanName,
                        WorkName = f.WorkName,
                        WorkClass = f.WorkClass ?? string.Empty,
                        StartDate = f.StartDate?.ToString("yyyy-MM-dd") ?? effectiveDate,
                        IsSocial = false,
                        IsWorkHeight = false,
                        Remark = f.Remark ?? string.Empty,
                        FailMsg = string.Empty,
                        InsuranceCompanyName = f.InsuranceCompany,
                        InsuranceWorkName = f.InsuranceWorkName ?? f.WorkName,
                        InsuranceWorkClass = f.InsuranceWorkClass ?? f.WorkClass ?? string.Empty,
                        CreateTime = now,
                        Status = BusInsuranceApiPushDetailStatusEnumType.待核对,
                        AddBusinessID = f.ID,
                        DelBusinessID = f.DelMatchRecordID,
                        DelName = f.DelMatchRecordName ?? string.Empty,
                        DelCardID = f.DelMatchRecordCardID ?? string.Empty,
                    }).ToList()
                });
            }

            if (addList.Any())
            {
                var addPushId = Guid.NewGuid();
                pushList.Add(new Live_BusInsuranceApiPushExtension
                {
                    ID = addPushId,
                    InsuranceCompany = "大地365",
                    RecordDate = today,
                    PushType = "新增",
                    Status = BusInsuranceApiPushStatusEnumType.待推送,
                    AddCount = addList.Count,
                    DelCount = 0,
                    ConfirmUrl = string.Empty,
                    CreateTime = now,
                    Details = addList.Select(f => new Live_BusInsuranceApiPushDetail
                    {
                        ID = Guid.NewGuid(),
                        BusInsuranceApiPushID = addPushId,
                        Name = f.Name,
                        CardID = f.CardID,
                        Age = f.Age,
                        Sex = f.Sex ?? string.Empty,
                        Phone = string.Empty,
                        CustomerName = f.CustomerName,
                        OrderNo = f.OrderNo,
                        PlanName = f.PlanName,
                        WorkName = f.WorkName,
                        WorkClass = f.WorkClass ?? string.Empty,
                        StartDate = f.StartDate?.ToString("yyyy-MM-dd") ?? effectiveDate,
                        IsSocial = false,
                        IsWorkHeight = false,
                        Remark = f.Remark ?? string.Empty,
                        FailMsg = string.Empty,
                        InsuranceCompanyName = f.InsuranceCompany,
                        InsuranceWorkName = f.InsuranceWorkName ?? f.WorkName,
                        InsuranceWorkClass = f.InsuranceWorkClass ?? f.WorkClass ?? string.Empty,
                        CreateTime = now,
                        Status = BusInsuranceApiPushDetailStatusEnumType.待核对,
                        AddBusinessID = f.ID,
                        DelBusinessID = 0,
                        DelName = string.Empty,
                        DelCardID = string.Empty,
                    }).ToList()
                });
            }

            if (pushList.Any())
            {
                await _apiPushService.AddNav(pushList);
            }
        }

        /// <summary>
        /// 通过接口获取保险公司处理结果
        /// </summary>
        /// <returns></returns>
        public async Task YangGuangPushResultNoticeByEmail()
        {
            var today = DateTime.Today;
           
            var dayMatchRecords = await _dayMatchRecordService.List(f => f.InsuranceCompany == "阳光保险公司" && !string.IsNullOrWhiteSpace(f.IndecaseId) && f.RecordDate == today);
            List<BusInsurancePushFailExcelTemplate> failRecords = new List<BusInsurancePushFailExcelTemplate>();
            var groups = dayMatchRecords.GroupBy(f => new { f.OrderNo, f.PlanName, f.InsuranceCompany, f.IsAddRecord, f.IndecaseId }).ToList();
            foreach (var groupItem in groups)
            {
                var res = await _insuranceApiService.BatchSearchStatus(new BatchSearchStatusRequest()
                {
                    Indecaseid = groupItem.Key.IndecaseId,
                    InsureId = groupItem.Key.OrderNo,
                    Type = groupItem.Key.IsAddRecord ? "addemp" : "delemp"
                });
                if (res != null && res.Data != null && res.Data.FailList != null && res.Data.FailList.Count > 0)
                {
                    var records = res.Data.FailList.Select(f => new BusInsurancePushFailExcelTemplate()
                    {
                        CardID = f.Customerinfo.Custemernum,
                        CustomerName = f.Customerinfo.Sjygdw,
                        SignUnitName = f.Customerinfo.Ldhtdw,
                        FailMsg = f.Msg,
                        InsuranceCompany = groupItem.Key.InsuranceCompany,
                        Name = f.Customerinfo.Custemername,
                        OrderNo = groupItem.Key.OrderNo,
                        PlanName = groupItem.Key.PlanName,
                        WorkClass = f.Customerinfo.Zhiyeleibei + "类",
                        WorkName = f.Customerinfo.Guyuangongzhong
                    }).ToList();
                    failRecords.AddRange(records);
                }
            }




            if (failRecords.Any())
            {
                var fileBytes = NpoiExcelHelper.WriteExcel(failRecords, "Sheet1");
                using (var ms = new MemoryStream(fileBytes))
                {
                    string content = $"";
                    var res = await _aliyunOSSService.UploadFile(ms, "11.xlsx", false, true);
                    _aliSendInfo.SendAliEmail("18668271258@163.com", $"阳光保险公司推送反馈通知--异常（{DateTime.Today.ToString("yyyy-MM-dd")}）", "商保推送完成-存在异常", "", $"/uploadfiles/{res.Data}");
                    //_aliSendInfo.SendAliEmail("15261300969@163.com", $"阳光保险公司推送反馈通知--异常（{DateTime.Today.ToString("yyyy-MM-dd")}）", "商保推送完成-存在异常", "", $"/uploadfiles/{res.Data}");
                }
            }
            else
            {
                _aliSendInfo.SendAliEmail("18668271258@163.com", $"阳光保险公司推送反馈通知--成功（{DateTime.Today.ToString("yyyy-MM-dd")}）", "商保推送完成-全部成功", "",string.Empty);
            }




        }


        public async Task PushResultNoticeByEmail(string insuranceCompany)
        {
            var today = DateTime.Today;
            var dayMatchRecords = await _dayMatchRecordService.List(f => f.InsuranceCompany == insuranceCompany && f.RecordDate == today && f.Status == BusInsuranceDayMatchRecordStatusEnumType.推送异常);
            var failRecords = dayMatchRecords.Select(f => new BusInsurancePushFailExcelTemplate()
            {
                CardID = f.CardID,
                CustomerName = f.CustomerName,
                SignUnitName = f.SignUnitName,
                FailMsg = f.Msg,
                InsuranceCompany = f.InsuranceCompany,
                Name = f.Name,
                OrderNo = f.OrderNo,
                PlanName = f.PlanName,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                InsuranceWorkName = f.InsuranceWorkName,
                PushActionName = f.IsAddRecord ? "增员" : "鉴于那"
            }).ToList();
            if (failRecords.Any())
            {
                var fileBytes = NpoiExcelHelper.WriteExcel(failRecords, "Sheet1");
                using (var ms = new MemoryStream(fileBytes))
                {
                    string content = $"";
                    var res = await _aliyunOSSService.UploadFile(ms, "11.xlsx", false, true);
                    _aliSendInfo.SendAliEmail("18668271258@163.com", $"【{insuranceCompany}】推送反馈通知--异常（{DateTime.Today.ToString("yyyy-MM-dd")}）", "商保推送完成-存在异常", "", $"/uploadfiles/{res.Data}");
                }
            }
            else
            {
                _aliSendInfo.SendAliEmail("18668271258@163.com", $"【{insuranceCompany}】推送反馈通知--成功（{DateTime.Today.ToString("yyyy-MM-dd")}）", "商保推送完成-全部成功", "", string.Empty);
            }




        }
        /// <summary>
        ///  推送完成统一处理
        /// </summary>
        /// <returns></returns>
        public async Task PushCompleted(string insuranceCompany)
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = today.AddDays(1 - today.Day);
            if (today.Month != nextDay.Month)
            {
                insuranceMonth = insuranceMonth.AddMonths(1);
            }
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);

            var beginTime = DateTime.Now.AddMinutes(-10);
            // 增员
            var matchRecordList = await _dayMatchRecordService.List(f => f.InsuranceCompany == insuranceCompany && f.IsAddRecord && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送 && f.RecordDate == today && f.CreateTime >= beginTime);
            var entryIds = matchRecordList.Select(f => f.UserEntryID).ToList();
            var entryList = entryIds.Select(f => new Live_UserEntry()
            {
                ID = f,
                PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.已增员,
                InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.已推送,
            }).ToList();
            await _entryService.UpdateInsuranceProcessStatus(entryList);
            // 生成已参保记录
            var records = matchRecordList.Select(f => new Live_BusInsuranceRecord()
            {
                Status = Model.Enums.BusInsuranceRecordStatusEnumType.已增员,
                AddPushID = string.Empty,
                Age = f.Age,
                CardID = f.CardID,
                CreateTime = f.CreateTime,
                CustomerName = f.CustomerName,
                DelPushID = string.Empty,
                EndDate = insuranceMonthEnd,
                InsuranceCompany = f.InsuranceCompany,
                ID = 0,
                InsuranceMonth = insuranceMonth,
                IsDelMatch = false,
                MatchDate = today,
                Name = f.Name,
                OrderNo = f.OrderNo,
                PlanName = f.PlanName,
                PlanNameID = f.PlanNameID,
                PolicyNum = string.Empty,
                Sex = f.Sex,
                SignUnitName = f.SignUnitName,
                StartDate = f.StartDate,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                InsuranceWorkName= f.InsuranceWorkName,
            }).ToList();
            await _recordService.Add(records);
            //减员
            if (insuranceCompany == "众安保险")
            {
                var delRecords = await _recordService.List(f => f.InsuranceCompany==insuranceCompany&& f.Status == Model.Enums.BusInsuranceRecordStatusEnumType.待减员);
                foreach (var record in delRecords)
                {
                    record.EndDate = today;
                    record.Status = Model.Enums.BusInsuranceRecordStatusEnumType.已减员;
                }
                await _recordService.Update(delRecords);
            }
            else
            {
                var delMatchRecordList = await _dayMatchRecordService.List(f => f.DelMatchRecordID > 0 && f.InsuranceCompany == insuranceCompany && f.IsAddRecord == false && f.Status == Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.已推送 && f.RecordDate == today && f.CreateTime >= beginTime);
                var delRecordIds = delMatchRecordList.Select(f => f.DelMatchRecordID).ToList();
                var delRecords = await _recordService.List(f => delRecordIds.Contains(f.ID));
                foreach (var record in delRecords)
                {
                    record.EndDate = today;
                    record.Status = Model.Enums.BusInsuranceRecordStatusEnumType.已减员;
                }
                await _recordService.Update(delRecords);
            }
        }


        public async Task PushCompleted36k(List<int> dayMatchRecordIds, List<int> delRecordIds)
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = today.AddDays(1 - today.Day);
            if (today.Month != nextDay.Month)
            {
                insuranceMonth = insuranceMonth.AddMonths(1);
            }
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);

            if (dayMatchRecordIds.Any())
            {
                // 增员
                var matchRecordList = await _dayMatchRecordService.List(f => dayMatchRecordIds.Contains(f.ID) && f.IsAddRecord && f.RecordDate == today);
                var entryIds = matchRecordList.Select(f => f.UserEntryID).ToList();
                var entryList = entryIds.Select(f => new Live_UserEntry()
                {
                    ID = f,
                    PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.已增员,
                    InsuranceProcessStatus = Model.Enums.InsuranceProcessEnumType.已推送,
                }).ToList();
                await _entryService.UpdateInsuranceProcessStatus(entryList);
                // 生成已参保记录
                var records = matchRecordList.Select(f => new Live_BusInsuranceRecord()
                {
                    Status = Model.Enums.BusInsuranceRecordStatusEnumType.已增员,
                    AddPushID = string.Empty,
                    Age = f.Age,
                    CardID = f.CardID,
                    CreateTime = f.CreateTime,
                    CustomerName = f.CustomerName,
                    DelPushID = string.Empty,
                    EndDate = insuranceMonthEnd,
                    InsuranceCompany = f.InsuranceCompany,
                    ID = 0,
                    InsuranceMonth = insuranceMonth,
                    IsDelMatch = false,
                    MatchDate = today,
                    Name = f.Name,
                    OrderNo = f.OrderNo,
                    PlanName = f.PlanName,
                    PlanNameID = f.PlanNameID,
                    PolicyNum = string.Empty,
                    Sex = f.Sex,
                    SignUnitName = f.SignUnitName,
                    StartDate = f.StartDate,
                    WorkClass = f.WorkClass,
                    WorkName = f.WorkName,
                    InsuranceWorkName = f.InsuranceWorkName,
                }).ToList();
                await _recordService.Add(records);
            }


            if (delRecordIds.Any())
            {
                // 记录标记为已减员
                var delRecords = await _recordService.List(f => delRecordIds.Contains(f.ID));
                foreach (var record in delRecords)
                {
                    record.EndDate = today;
                    record.Status = Model.Enums.BusInsuranceRecordStatusEnumType.已减员;
                }
                await _recordService.Update(delRecords);
                var delDayRecordList = await _dayMatchRecordService.List(f => delRecordIds.Contains(f.DelMatchRecordID) && !f.IsAddRecord && f.RecordDate == today);
                if (delDayRecordList.Any())
                {
                    foreach (var item in delDayRecordList)
                    {
                        item.Status = BusInsuranceDayMatchRecordStatusEnumType.已推送;
                    }
                    await _dayMatchRecordService.Update(delDayRecordList);
                }
            }
          





        }

        public async Task GenerateBusinsuranceRecordByApi()
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);
            string[] insuranceCompanys = new string[] { "阳光保险公司"};
            var orders = await _orderService.List(f => f.OrderNo== "NBLB20260528203632133" && f.OrderMonth == insuranceMonth && f.IsEnable);
            foreach (var order in orders)
            {
                var resp = await _insuranceApiService.GetOrderDetail(order.OrderNo);
                if (resp.Code == 200 && resp.Data != null)
                {
                    if (resp.Data.baoquanlist != null)
                    {
                        var records = resp.Data.baoquanlist.Select(f => new Live_BusInsuranceRecord()
                        {
                            AddPushID = string.Empty,
                            Age = int.Parse(f.Cage),
                            CardID = f.Cidnum,
                            CreateTime = DateTime.Now,
                            CustomerName = f.Csjygdw,
                            SignUnitName = f.Cldhtdw,
                            DelPushID = string.Empty,
                            EndDate = order.EndDate,
                            InsuranceCompany = order.InsuranceCompany,
                            InsuranceMonth = order.OrderMonth,
                            IsDelMatch = false,
                            ID = 0,
                            MatchDate = DateTime.MinValue,
                            Name = f.Cname,
                            OrderNo = order.OrderNo,
                            PlanName = order.PlanName,
                            PlanNameID = order.PlanNameID,
                            PolicyNum = order.PolicyNum,
                            Sex = string.Empty,
                            StartDate = f.Cqbrq,
                            Status = Model.Enums.BusInsuranceRecordStatusEnumType.已增员,
                            WorkClass = f.Czylb + "类",
                            WorkName = f.Cgygz,
                        }).ToList();

                        // 删除当前已在保记录
                        await _recordService.Delete(f => f.InsuranceCompany=="阳光保险公司"&& f.OrderNo == order.OrderNo && f.Status != Model.Enums.BusInsuranceRecordStatusEnumType.已减员);
                        await _recordService.Add(records);
                    }
                }
            }
        }
        public async Task PushZhongAnProcess()
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            // 根据工种匹配方案
            await WorkPlanMatchByInsuranceCompany("众安保险");
            await Task.Delay(200);
            // 匹配保全名单并生成增减记录
            var orders = await _orderService.List(f => f.IsEnable && !f.IsDayOrder && f.OrderMonth == insuranceMonth  && f.InsuranceCompany == "众安保险");
            foreach (var item in orders)
            {
                await MatchByOrderNo(item.InsuranceCompany, item.OrderNo);
            }
            await Task.Delay(200);
            // 邮件推送众安
            await PushZhongAn();
            await Task.Delay(200);
            await PushCompleted("众安保险");
        }

        public async Task PushAnZeTProcess()
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            // 根据工种匹配方案
            await WorkPlanMatchByInsuranceCompany("安责险 T");
            await Task.Delay(1000);
            // 匹配保全名单并生成增减记录
            var orders = await _orderService.List(f => f.IsEnable && !f.IsDayOrder && f.OrderMonth == insuranceMonth  && f.InsuranceCompany == "安责险 T");
            foreach (var item in orders)
            {
                await MatchByOrderNo(item.InsuranceCompany, item.OrderNo);
            }
            await PushAnZeT();
            await Task.Delay(1000);
            await PushCompleted("安责险 T");
        }

        public async Task PushDaDiProcess()
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            // 根据工种匹配方案
            await WorkPlanMatchByInsuranceCompany("大地财险");
            await Task.Delay(1000);
            // 匹配保全名单并生成增减记录
            var orders = await _orderService.List(f => f.IsEnable && !f.IsDayOrder && f.OrderMonth == insuranceMonth && f.IsEnable && f.InsuranceCompany == "大地财险");
            foreach (var item in orders)
            {
                await MatchByOrderNo(item.InsuranceCompany, item.OrderNo);
            }
            await PushDaDi();
            await Task.Delay(1000);
            await PushCompleted("大地财险");
        }



        public async Task PushLiBaoProcess()
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            //// 根据工种匹配方案
            await WorkPlanMatchByInsuranceCompany("利宝保险");
            await Task.Delay(1000);
            // 匹配保全名单并生成增减记录
            var orders = await _orderService.List(f => f.IsEnable && !f.IsDayOrder  && f.OrderMonth == insuranceMonth && f.IsEnable && f.InsuranceCompany == "利宝保险");
            foreach (var item in orders)
            {
                await MatchByOrderNo(item.InsuranceCompany, item.OrderNo);
            }
            await PushLiBao();
            await Task.Delay(1000);
            await PushCompleted("利宝保险");
        }

        public async Task PushYangGuangProcess()
        {

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            await WorkPlanMatchByInsuranceCompany("阳光保险公司");
            await Task.Delay(1000);
            await MatchByInsuranceCompany("阳光保险公司");
            await PushYangGuang();
            await Task.Delay(1000);
            await PushCompleted("阳光保险公司");
            await Task.Delay(3000);
            await YangGuangPushResultNoticeByEmail();
        }

       

      


        public async Task PushDaDi365Process()
        {
            string insuranceCompany = "大地365";

            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            //// 根据工种匹配方案
            await WorkPlanMatchByInsuranceCompany(insuranceCompany);
            await Task.Delay(1000);
            // 匹配保全名单并生成增减记录
            var orders = await _orderService.List(f => f.IsEnable && !f.IsDayOrder && f.OrderMonth == insuranceMonth && f.IsEnable && f.InsuranceCompany == insuranceCompany);
            foreach (var item in orders)
            {
                await MatchByOrderNo(item.InsuranceCompany, item.OrderNo);
            }

            await PushDaDi365();
            //await Task.Delay(1000);
            //await PushCompleted(insuranceCompany);
            //await Task.Delay(3000);
            //await PushResultNoticeByEmail(insuranceCompany);
        }

        #region 生成测试数据
        public async Task GenerateTestRecord()
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = DateTime.Today.AddDays(1 - today.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);

            var  entryList=await _entryService.ListInsurance(f => f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.待推送);

            var records = entryList.Select(f => new Live_BusInsuranceRecord()
            {
                CreateTime = DateTime.Now,
                EndDate = insuranceMonthEnd,
                InsuranceCompany = f.InsuranceCompany1,
                InsuranceMonth = insuranceMonth,
                OrderNo = f.InsuranceId1,
                PlanName = f.PlanName1,
                PlanNameID = 0,
                PolicyNum = string.Empty,
                StartDate = insuranceMonth,
                CardID = f.CardID,
                AddPushID = string.Empty,
                Age = 0,
                CustomerName = f.CustomerName,
                DelPushID = string.Empty,
                IsDelMatch = false,
                MatchDate = DateTime.MinValue,
                Name = f.Name,
                Sex = string.Empty,
                SignUnitName = f.SignUnitName,
                Status = 0,
                WorkClass = f.InsuranceWorkClass,
                WorkName = f.InsuranceWorkName
            }).ToList();

            await  _recordService.Add(records);
        }  
        

        public async Task SendEmailTest()
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = DateTime.Today.AddDays(1 - today.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);
            List<BusInsuranceRecordExcelTemplate> pushRecords = new List<BusInsuranceRecordExcelTemplate>();
            pushRecords.Add(new BusInsuranceRecordExcelTemplate()
            {
                Age = 0,
                CreateTime = DateTime.Today.ToString("yyyy-MM-dd"),
                CustomerName = "CustomerName",
                EndDate = DateTime.Today.ToString("yyyy-MM-dd"),
                IdCardNo = "IdCardNo",
                InsuranceCompany = "InsuranceCompany",
                InsuranceId = "InsuranceId",
                InsuranceSigningUnitName = "InsuranceSigningUnitName",
                Name = "Name",
                PlanName = "PlanName",
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                StatusName = "StatusName",
                WorkClass = "WorkClass",
                WorkName = "WorkName"
            });
            var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "Sheet1");

            var ccAddressList = new List<MimeKit.MailboxAddress>() { new MimeKit.MailboxAddress("抄送人", "15261300969@163.com") };


            await _mailSenderService.SendUseForInsuranceAsync("zk@cn-labor.cn", $"抄送测试（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"抄送测试（{DateTime.Today.ToString("yyyy-MM-dd")}）", ccAddressList, "商保推送.xlsx", fileBytes);
        }

        public async Task<bool> JobAddOrUpdate(int customerId,string workName,string workClass)
        {
            var jobInfo= await base.Context.Queryable<Live_EntryJob>().Where(f =>f.Name== workName).FirstAsync() ;
            var dt = DateTime.Now;
           



            if (jobInfo==null)
            {
                jobInfo = new Live_EntryJob()
                {

                    Name = workName,
                    Type = workClass,
                    CompanyID = 0,
                    CreateTime = dt,
                    Enabled = true,
                    ID = 0,
                };
                await base.Context.Insertable<Live_EntryJob>(jobInfo).ExecuteCommandIdentityIntoEntityAsync() ; 
            }

            var cusjob = await base.Context.Queryable<Live_CustomerJobs>().Where(f => f.CustomerID == customerId && f.EntryJobID == jobInfo.ID).FirstAsync();
            if (cusjob==null)
            {
                cusjob = new Live_CustomerJobs()
                {
                    CustomerID = customerId,
                    EntryJobID = jobInfo.ID,
                    IsDefault = false
                };

                await base.Context.Insertable<Live_CustomerJobs>(cusjob).ExecuteCommandIdentityIntoEntityAsync();
            }

            return true;

        }


        public async Task<List<Live_EntryJob>> ListEntryJob()
        {
            var jobInfo = await base.Context.Queryable<Live_EntryJob>().Where(f => f.ID>0).ToListAsync();

            return jobInfo;
        }

        #endregion




        /// <summary>
        /// 针对阳光保险公司
        /// 日保单月保单匹配
        /// </summary>
        /// <param name="insuranceCompany"></param>
        /// <param name="isGenerateDayMatchRecord"></param>
        /// <returns></returns>

        public async Task MatchByInsuranceCompany(string insuranceCompany)
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);
            var orders = await _orderService.ListOrderPlan(f => f.InsuranceCompany == insuranceCompany && f.OrderMonth == insuranceMonth && f.IsEnable);

            var orderNos = orders.Select(f => f.OrderNo).ToList();

            await _dayMatchRecordService.Delete(f => f.Status == BusInsuranceDayMatchRecordStatusEnumType.待推送 && f.InsuranceCompany == insuranceCompany);

            var recordList = await _recordService.List(f => orderNos.Contains(f.OrderNo) && (f.Status == BusInsuranceRecordStatusEnumType.已增员 || f.Status == BusInsuranceRecordStatusEnumType.待减员));
            // 获取此保险公司的全部保全记录
            var entryList = await _entryService.ListInsurance(f =>
       
            f.Status == 2
            &&f.CusEnable
            &&f.IsUseBusInsurance
            && f.InsuranceCompany1 == insuranceCompany
            && f.IsManuallyInsurance == false
            && !string.IsNullOrWhiteSpace(f.WorkName)
            && !string.IsNullOrWhiteSpace(f.WorkClass)
            && !string.IsNullOrWhiteSpace(f.PlanName1)
            && (f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.已推送 || f.InsuranceProcessStatus == Model.Enums.InsuranceProcessEnumType.待推送));

            Live_BusInsuranceRecord record;
            List<int> recordIds = new List<int>();

            foreach (var item in entryList)
            {

                record = recordList.FirstOrDefault(f =>
             f.Name.Trim() == item.Name.Trim()
             && f.CardID.Equals(item.CardID, StringComparison.OrdinalIgnoreCase)
             && item.CustomerName.Contains(f.CustomerName)
             && f.SignUnitName.Equals(item.SignUnitName, StringComparison.OrdinalIgnoreCase)
             && f.WorkName == item.WorkName
             && f.WorkClass == item.WorkClass
           && f.PlanName == item.PlanName1
             );

            

                if (record != null)
                {
                    recordIds.Add(record.ID);
                    item.InsuranceId1 = record.OrderNo;
                    item.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
                    item.InsuranceProcessStatus = InsuranceProcessEnumType.已推送;
                }
                else
                {
                    item.InsuranceId1 = string.Empty;
                    item.PlanStatus1 = UserEntryPlanStatusEnumType.待增员;
                    item.InsuranceProcessStatus = InsuranceProcessEnumType.待匹配;
                }


            }
            // 已增员
            var addList = recordList.Where(f => recordIds.Contains(f.ID)).ToList();
            var delList = recordList.Where(f => !recordIds.Contains(f.ID)).ToList();


            foreach (var item in addList)
            {
                item.Status = BusInsuranceRecordStatusEnumType.已增员;
            }

            foreach (var item in delList)
            {
                item.Status = BusInsuranceRecordStatusEnumType.待减员;
            }
            var pendingEntryList = entryList.Where(f => f.InsuranceProcessStatus == InsuranceProcessEnumType.待匹配 && string.IsNullOrWhiteSpace(f.InsuranceId1)).ToList();
            List<int> matchRecordIds = new List<int>();

            List<Live_BusInsuranceDayMatchRecord> dayMatchRecords = new List<Live_BusInsuranceDayMatchRecord>();

            foreach (var item in pendingEntryList)
            {
                // 优先匹配月保单
                var orderPlan = orders.FirstOrDefault(f => !f.IsDayOrder && f.PlanName == item.PlanName1 && !string.IsNullOrWhiteSpace(f.SignUnitNames) && f.SignUnitNames.Contains(item.SignUnitName));
                if (orderPlan == null)
                {
                    orderPlan = orders.FirstOrDefault(f => !f.IsDayOrder && f.PlanName == item.PlanName1 && string.IsNullOrWhiteSpace(f.SignUnitNames));
                }

                if (orderPlan == null)
                {
                    item.InsuranceProcessStatus = InsuranceProcessEnumType.匹配失败订单;
                    item.InsuranceId1 = string.Empty;
                }
                else
                {
                    item.InsuranceProcessStatus = InsuranceProcessEnumType.待推送;
                    item.InsuranceId1 = orderPlan.OrderNo;
                    // 增减替换
                    // 优先匹配相同身份证号码的减员记录
                    record = delList.FirstOrDefault(f => f.OrderNo == item.InsuranceId1 && f.CardID == item.CardID && !matchRecordIds.Contains(f.ID));
                    if (record == null)
                    {
                        record = delList.FirstOrDefault(f => f.OrderNo == item.InsuranceId1 && !matchRecordIds.Contains(f.ID));
                    }
                    // 先减员再增员
                    if (record != null)
                    {
                        matchRecordIds.Add(record.ID);
                        // 增员
                        dayMatchRecords.Add(new Live_BusInsuranceDayMatchRecord()
                        {
                            ID = 0,
                            Age = 0,
                            CardID = item.CardID,
                            CreateTime = DateTime.Now,
                            CustomerName = item.CustomerName,
                            DelMatchRecordID = record.ID,
                            EndDate = insuranceMonthEnd,
                            IndecaseId = string.Empty,
                            OrderNo = item.InsuranceId1,
                            InsuranceCompany = item.InsuranceCompany1,
                            InsuranceWorkClass = item.WorkClass,
                            WorkName = item.WorkName,
                            InsuranceWorkName = item.WorkName,
                            WorkClass = item.WorkClass,
                            IsAddRecord = true,
                            Msg = string.Empty,
                            Name = item.Name,
                            PlanName = orderPlan.PlanName,
                            PlanNameID = orderPlan.PlanNameID,
                            PushResult = string.Empty,
                            RecordDate = today,
                            Remark = string.Empty,
                            Sex = string.Empty,
                            SignUnitName = item.SignUnitName,
                            StartDate = nextDay,
                            Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                            UserEntryID = item.ID
                        });

                        // 减员
                        dayMatchRecords.Add(new Live_BusInsuranceDayMatchRecord()
                        {
                            ID = 0,
                            Age = record.Age,
                            CardID = record.CardID,
                            CreateTime = DateTime.Now,
                            CustomerName = record.CustomerName,
                            DelMatchRecordID = record.ID,
                            EndDate = insuranceMonthEnd,
                            IndecaseId = string.Empty,
                            OrderNo = record.OrderNo,
                            InsuranceCompany = record.InsuranceCompany,
                            InsuranceWorkClass = record.WorkClass,
                            WorkClass = record.WorkClass,
                            InsuranceWorkName = record.WorkName,
                            IsAddRecord = false,
                            Msg = "增减替换",
                            Name = record.Name,
                            PlanName = record.PlanName,
                            PlanNameID = record.PlanNameID ?? 0,
                            PushResult = string.Empty,
                            RecordDate = today,
                            Remark = string.Empty,
                            Sex = record.Sex,
                            SignUnitName = record.SignUnitName,
                            StartDate = record.StartDate,
                            Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                            WorkName = record.WorkName,
                            UserEntryID = 0
                        });
                    }
                    else
                    {
                        //if (insuranceCompany == "阳光保险公司" && today.Day > 8)
                        //{
                        //    orderPlan = orders.FirstOrDefault(f => f.IsDayOrder && f.PlanName == item.PlanName1 && string.IsNullOrWhiteSpace(f.SignUnitNames));
                        //    if (orderPlan != null)
                        //    {
                        //        item.InsuranceId1 = orderPlan.OrderNo;
                        //        // 日保单增员
                        //        dayMatchRecords.Add(new Live_BusInsuranceDayMatchRecord()
                        //        {
                        //            ID = 0,
                        //            Age = 0,
                        //            CardID = item.CardID,
                        //            CreateTime = DateTime.Now,
                        //            CustomerName = item.CustomerName,
                        //            DelMatchRecordID = 0,
                        //            EndDate = insuranceMonthEnd,
                        //            IndecaseId = string.Empty,
                        //            OrderNo = item.InsuranceId1,
                        //            InsuranceCompany = item.InsuranceCompany1,
                        //            InsuranceWorkClass = item.WorkClass,
                        //            WorkName = item.WorkName,
                        //            InsuranceWorkName = item.WorkName,
                        //            WorkClass = item.WorkClass,
                        //            IsAddRecord = true,
                        //            Msg = string.Empty,
                        //            Name = item.Name,
                        //            PlanName = orderPlan.PlanName,
                        //            PlanNameID = orderPlan.PlanNameID,
                        //            PushResult = string.Empty,
                        //            RecordDate = today,
                        //            Remark = string.Empty,
                        //            Sex = string.Empty,
                        //            SignUnitName = item.SignUnitName,
                        //            StartDate = nextDay,
                        //            Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                        //            UserEntryID = item.ID
                        //        });
                        //    }
                        //    else
                        //    {// 未匹配到日保单 则继续推送月保单 已取消日保单匹配 2026-09-12
                        //     //item.InsuranceProcessStatus = InsuranceProcessEnumType.匹配失败订单;
                        //     //item.InsuranceId1 = string.Empty;

                        //        var addRecord = new Live_BusInsuranceDayMatchRecord()
                        //        {
                        //            ID = 0,
                        //            Age = 0,
                        //            CardID = item.CardID,
                        //            CreateTime = DateTime.Now,
                        //            CustomerName = item.CustomerName,
                        //            DelMatchRecordID = 0,
                        //            EndDate = insuranceMonthEnd,
                        //            IndecaseId = string.Empty,
                        //            OrderNo = item.InsuranceId1,
                        //            InsuranceCompany = item.InsuranceCompany1,
                        //            InsuranceWorkClass = item.WorkClass,
                        //            WorkName = item.WorkName,
                        //            InsuranceWorkName = item.WorkName,
                        //            WorkClass = item.WorkClass,
                        //            IsAddRecord = true,
                        //            Msg = string.Empty,
                        //            Name = item.Name,
                        //            PlanName = orderPlan.PlanName,
                        //            PlanNameID = orderPlan.PlanNameID,
                        //            PushResult = string.Empty,
                        //            RecordDate = today,
                        //            Remark = string.Empty,
                        //            Sex = string.Empty,
                        //            SignUnitName = item.SignUnitName,
                        //            StartDate = nextDay,
                        //            Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                        //            UserEntryID = item.ID
                        //        };

                        //        dayMatchRecords.Add(addRecord);
                        //    }
                        //}
                        //else
                        //{
                        //    // 在月保单中直接 增员
                            
                        //}

                        dayMatchRecords.Add(new Live_BusInsuranceDayMatchRecord()
                        {
                            ID = 0,
                            Age = 0,
                            CardID = item.CardID,
                            CreateTime = DateTime.Now,
                            CustomerName = item.CustomerName,
                            DelMatchRecordID = 0,
                            EndDate = insuranceMonthEnd,
                            IndecaseId = string.Empty,
                            OrderNo = item.InsuranceId1,
                            InsuranceCompany = item.InsuranceCompany1,
                            InsuranceWorkClass = item.WorkClass,
                            WorkName = item.WorkName,
                            InsuranceWorkName = item.WorkName,
                            WorkClass = item.WorkClass,
                            IsAddRecord = true,
                            Msg = string.Empty,
                            Name = item.Name,
                            PlanName = orderPlan.PlanName,
                            PlanNameID = orderPlan.PlanNameID,
                            PushResult = string.Empty,
                            RecordDate = today,
                            Remark = string.Empty,
                            Sex = string.Empty,
                            SignUnitName = item.SignUnitName,
                            StartDate = nextDay,
                            Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                            UserEntryID = item.ID
                        });
                    }
                }
            }



            // 日保单待减员 则全部减员
            var dayOrderNos = orders.Where(f => f.IsDayOrder).Select(f => f.OrderNo);

            var delPushRecords = delList.Where(f => dayOrderNos.Contains(f.OrderNo)).Select(f => new Live_BusInsuranceDayMatchRecord()
            {
                ID = 0,
                Age = f.Age,
                CardID = f.CardID,
                CreateTime = DateTime.Now,
                CustomerName = f.CustomerName,
                DelMatchRecordID = f.ID,
                EndDate = insuranceMonthEnd,
                IndecaseId = string.Empty,
                OrderNo = f.OrderNo,
                InsuranceCompany = f.InsuranceCompany,
                InsuranceWorkClass = f.WorkClass,
                WorkClass = f.WorkClass,
                InsuranceWorkName = f.WorkName,
                IsAddRecord = false,
                Msg = "增减替换",
                Name = f.Name,
                PlanName = f.PlanName,
                PlanNameID = f.PlanNameID ?? 0,
                PushResult = string.Empty,
                RecordDate = today,
                Remark = string.Empty,
                Sex = f.Sex,
                SignUnitName = f.SignUnitName,
                StartDate = f.StartDate,
                Status = Model.Enums.BusInsuranceDayMatchRecordStatusEnumType.待推送,
                WorkName = f.WorkName,
                UserEntryID = 0
            }).ToList();

            if (delPushRecords.Any())
            {
                dayMatchRecords.AddRange(delPushRecords);
            }

            var updateEntityList = entryList.Select(f => new Live_UserEntry()
            {
                ID = f.ID,
                PlanStatus1 = f.PlanStatus1,
                PlanName1 = f.PlanName1,
                InsuranceCompany1 = f.InsuranceCompany1,
                InsuranceId1 = f.InsuranceId1,
                InsuranceProcessStatus = f.InsuranceProcessStatus,
                IsManuallyInsurance = f.IsManuallyInsurance,
                InsuranceType = f.InsuranceType,

            }).ToList();

            await _recordService.UpdateRecordMatchStatus(recordList);
            await _entryService.UpdateInsuranceInfo(updateEntityList);

            await _dayMatchRecordService.Add(dayMatchRecords);



        }

        public async Task FiveNoInsurancePush()
        {
            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var insuranceMonth = nextDay.AddDays(1 - nextDay.Day);
            await WorkPlanMatchByInsuranceCompany("阳光保险公司");

            // 今日入职的5类员工
            var entryFiveList = await _entryService.ListInsurance(f => f.Status == 2 && f.WorkClass == "5类" && f.EntryTime >= today&&f.InsuranceCompany1!= "阳光保险公司");
            var cardIDList = entryFiveList.Select(f => f.CardID).ToList();
            // 已在保的5类保全记录
            var sucRecordList = await _recordService.List(f => f.InsuranceMonth == insuranceMonth && cardIDList.Contains(f.CardID) && f.Status == BusInsuranceRecordStatusEnumType.已增员);
            var sucCardIDList = sucRecordList.Select(f => f.CardID).ToList();
            var pushData = entryFiveList.Where(f => !sucCardIDList.Contains(f.CardID)).ToList();
            List<FiveOtherExcelTemplate> pushRecords = pushData.Select(f => new FiveOtherExcelTemplate()
            {
                CustomerName = f.CustomerName,
                CardID = f.CardID,
                SignUnitName = f.SignUnitName,
                Name = f.Name,
                StartDate = nextDay.ToString("yyyy-MM-dd"),
                WorkName = f.WorkName,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "新增");
            var ccAddressList = new List<MimeKit.MailboxAddress>() {  };
            //await _mailSenderService.SendUseForInsuranceAsync("zk@cn-labor.cn", $"抄送测试（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"抄送测试（{DateTime.Today.ToString("yyyy-MM-dd")}）", ccAddressList, "商保推送.xlsx", fileBytes);

            var isSuc = await _mailSenderService.SendUseForInsuranceAsync("liusiyao_rb@qq.com", $"5类意外险渠道推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", $"5类意外险渠道推送（{DateTime.Today.ToString("yyyy-MM-dd")}）", ccAddressList, $"商保推送_{DateTime.Today.ToString("yyyy-MM-dd")}.xlsx", fileBytes);

        }

    }
}
