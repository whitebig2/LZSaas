using LZ.Model.Enums;
using LZ.Model.InsuranceApi;
using LZ.Model;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model.Exceptions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Model.IExcelTemplate;
using LZ.Core;
using Senparc.NeuChar.NeuralSystems;

namespace LZ.Users.Service.Services
{
    public class BusinessInsurancService: ServicesBase<Live_BusinessGuarantee>, IBusinessInsurancService, IScopedDependency
    {

        private readonly IAliyunOSSService   _aliyunOSSService;
        private readonly IRedisService  _redisService;
        private readonly IAliSendInfo  _aliSendInfo;
        private readonly IInsuranceApiService  _insuranceApiService;
        private readonly IBusinessGuaranteeService _businessGuaranteeService;
        private readonly IEntryService _entryService;
        private readonly IBusinessInsuranceConfigService _configService ;
        private readonly IBusinessInsuranceOrderService _orderService ;
        private readonly IBusinessInsuranceRecordService _businessInsuranceRecordService ;

        public BusinessInsurancService(IBusinessGuaranteeService businessGuaranteeService, IEntryService entryService, IBusinessInsuranceConfigService configService, IBusinessInsuranceOrderService orderService, IInsuranceApiService insuranceApiService, IBusinessInsuranceRecordService businessInsuranceRecordService, IAliSendInfo aliSendInfo, IRedisService redisService, IAliyunOSSService aliyunOSSService)
        {
            _businessGuaranteeService = businessGuaranteeService;
            _entryService = entryService;
            _configService = configService;
            _orderService = orderService;
            _insuranceApiService = insuranceApiService;
            _businessInsuranceRecordService = businessInsuranceRecordService;
            _aliSendInfo = aliSendInfo;
            _redisService = redisService;
            _aliyunOSSService = aliyunOSSService;
        }


        /// <summary>
        ///  阳光保险
        ///  从Api 获取在保数据
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GenerateApiRecord(bool isMatch)
        {
            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var nextDayMonth = nextDay.AddDays(1 - nextDay.Day);
            var orderList = await _orderService.ListView(f => f.InsuranceCompany == "阳光保险公司" && f.StartDate == nextDayMonth);
           // 删除今日生成的记录
            await _businessGuaranteeService.Delete(f => f.GenerateDate == today && f.IsMatchRecord == isMatch);
            List<Live_BusinessGuarantee> empList = new List<Live_BusinessGuarantee>();
            int sortNum = 0;
            foreach (var item in orderList)
            {
                var isMonthInsurance = item.OrderType == (int)InsuranceOrderEnumType.月保单;
                var data = await _insuranceApiService.GetOrderDetail(item.InsuranceId);
                var recordList = data.Data.baoquanlist.Select(f => new Live_BusinessGuarantee()
                {
                    Id = 0,
                    IdCardNo = f.Cidnum,
                    CreateTime = DateTime.Now,
                    CustomerName = f.Csjygdw,
                    EndDate = f.Czbrq,
                    StartDate = f.Cqbrq,
                    EntryTime = DateTime.MinValue,
                    InsuranceMonth = nextDayMonth,
                    Name = f.Cname,
                    PlanName = f.Geentname,
                    PlanNameId = int.Parse(f.Geentid),
                    SignUnitName = f.Cldhtdw,
                    WorkClass = f.Czylb + "类",
                    WorkName = f.Cgygz,
                    InsuranceCompany = item.InsuranceCompany,
                    InsuranceId = item.InsuranceId,
                    IsMonthInsurance = isMonthInsurance,
                    Remark = "",
                    Status =  UserEntryPlanStatusEnumType.已增员,
                    UserEntryId = 0,
                    UserEntryStatus = string.Empty,
                    GenerateDate = today,
                    DelRecordID = Guid.Empty,
                    IsDelTag = false,
                    IsEnabled = true,
                    IsMatchRecord = isMatch
                }).ToList();

                sortNum = 1;

                foreach (var recordItem in recordList)
                {
                    recordItem.SortNo = sortNum;
                    sortNum ++;
                }
                var sortNoList = recordList.GroupBy(f => f.IdCardNo).Select(f => f.Max(ff => ff.SortNo)).ToList();
                recordList = recordList.Where(f => f.IdCardNo != "其他" && sortNoList.Contains(f.SortNo)).ToList();
                empList.AddRange(recordList);
            }



            List<string> idCardNoList = new List<string>();
            var manuallyInsurance = await _entryService.List(f => f.IsManuallyInsurance == true && f.Status == 2);
            if (manuallyInsurance != null && manuallyInsurance.Count > 0)
            {
                // 客服手动参保员工不做自动减员
                idCardNoList = manuallyInsurance.Select(f => f.CardID).ToList();
            }
            // 排除手动增减的员工记录
            empList = empList.Where(f => !idCardNoList.Contains(f.IdCardNo)).ToList();
            await _businessGuaranteeService.Add(empList);
            return true;
        }
        /// <summary>
        /// 阳光匹配
        /// </summary>
        /// <returns></returns>
        private async Task<bool>PlanName1Match(bool isMatch)
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var configList = await _configService.ListView(f => f.InsuranceCompany == "阳光保险公司" && f.IsEnabled && f.IsMain);
            List<Live_UserEntry> entryModelList = new List<Live_UserEntry>();
            List<Live_BusinessGuarantee> recordList = new List<Live_BusinessGuarantee>();

            string planName = string.Empty;

            foreach (var configItme in configList)
            {
                entryModelList.Clear();
                recordList.Clear();
                // 必须要参保的员工
                var entryList = await _entryService.ListView(f =>!string.IsNullOrWhiteSpace(f.PlanName1)&& f.InsuranceCompany1== "阳光保险公司" && f.IsManuallyInsurance == false && f.CustomerName == configItme.CustomerName && !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName) && f.Status == 2);
                var ygRecordList = await _businessGuaranteeService.List(f => f.GenerateDate == today && f.IsMatchRecord == isMatch && f.InsuranceCompany == "阳光保险公司" && f.PlanName != "10万+1万" && f.CustomerName == configItme.CustomerName && f.SignUnitName == configItme.InsuranceSigningUnitName);
                foreach (var item in entryList)
                {

                    //if (item.WorkClass == "3类")
                    //{
                    //    planName = configItme.Plan3;
                    //}
                    //else if (item.WorkClass == "4类")
                    //{
                    //    planName = configItme.Plan4;
                    //}
                    //else
                    //{
                    //    planName = configItme.Plan5;
                    //}


                    var entryModel = await _entryService.Get(f => f.ID == item.ID);
                    // 工种、类别、身份证号码、方案
                    if (ygRecordList.Any(f => f.Name == item.Name && f.IdCardNo == item.CardID && f.WorkClass == item.WorkClass && f.WorkName == item.WorkName && f.PlanName == planName))
                    {
                        var record = ygRecordList.First(f => f.Name == item.Name && f.IdCardNo == item.CardID && item.WorkClass == f.WorkClass && f.WorkName == item.WorkName && f.PlanName == planName);
                        // 状态改成 阳光保险 已增员
                        entryModel.InsuranceCompany1 = record.InsuranceCompany;
                        entryModel.PlanName1 = record.PlanName;
                        entryModel.InsuranceId1 = record.InsuranceId;
                        entryModel.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
                        record.UserEntryId = item.ID;
                        record.UserEntryStatus = "2";
                        record.Status = UserEntryPlanStatusEnumType.已增员;
                        recordList.Add(record);
                    }
                    else
                    {
                        Console.WriteLine("+++++++++++++++++++++主险不匹配+++++++++++++++++++++++");
                        Console.WriteLine($"{item.Name}");
                        Console.WriteLine($"{item.CardID}");
                        Console.WriteLine($"{item.WorkClass}");
                        Console.WriteLine($"{item.WorkName}");
                        Console.WriteLine($"{item.CustomerName}");
                        Console.WriteLine($"{item.EntryTime.Value.ToString("yyyy-MM-dd")}");
                        Console.WriteLine("--------------------------------------------------");
                        // 状态改成 待增员
                        entryModel.PlanStatus1 = UserEntryPlanStatusEnumType.待增员;
                    }
                    entryModelList.Add(entryModel);
                }

                if (entryModelList.Any())
                {
                    // 更新员工参保状态
                    await _entryService.UpdateBusInsurance(entryModelList);
                }
                // 匹配记录

                if (recordList.Any())
                {
                    await _businessGuaranteeService.Update(recordList);
                }
            }
            // 匹配记录
           
               
                var noUserEntryRecord = await _businessGuaranteeService.List(f => f.GenerateDate == today && f.IsMatchRecord && f.InsuranceCompany == "阳光保险公司" && f.PlanName != "10万+1万" && f.IsEnabled && f.UserEntryId <= 0);
                foreach (var item in noUserEntryRecord)
                {
                    item.Remark = "未匹配到在职人员信息";
                    item.Status = UserEntryPlanStatusEnumType.待减员;
                    item.IsDelTag = false;
                }
                await _businessGuaranteeService.Update(noUserEntryRecord);
            return true;
        }
        /// <summary>
        /// 中国大地财险公司
        /// </summary>
        /// <param name="isMatch"></param>
        /// <returns></returns>

        private async Task<bool> PlanName2Match(bool isMatch)
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var configList = await _configService.ListView(f => f.InsuranceCompany == "阳光保险公司" && f.IsEnabled&&f.IsMain==false);
            List<Live_UserEntry> entryModelList = new List<Live_UserEntry>();
            List<Live_BusinessGuarantee> recordList = new List<Live_BusinessGuarantee>();
            foreach (var configItme in configList)
            {

              
                entryModelList.Clear();
                recordList.Clear();
                // 必须要参保的员工
                var entryList = await _entryService.ListView(f => f.IsManuallyInsurance == false && f.CustomerName == configItme.CustomerName && !string.IsNullOrWhiteSpace(f.WorkClass) && !string.IsNullOrWhiteSpace(f.WorkName) && f.Status == 2);
                var ygRecordList = await _businessGuaranteeService.List(f => f.GenerateDate == today && f.IsMatchRecord == isMatch && f.InsuranceCompany == "阳光保险公司" && f.PlanName == "10万+1万" && f.CustomerName == configItme.CustomerName && f.SignUnitName == configItme.InsuranceSigningUnitName);
                foreach (var item in entryList)
                {
                    var entryModel = await _entryService.Get(f => f.ID == item.ID);
                    // 工种、类别、身份证号码、方案
                    if (ygRecordList.Any(f => f.Name == item.Name && f.IdCardNo == item.CardID && f.WorkClass == item.WorkClass && f.WorkName == item.WorkName))
                    {
                        var record = ygRecordList.First(f => f.Name == item.Name && f.IdCardNo == item.CardID && item.WorkClass == f.WorkClass && f.WorkName == item.WorkName);
                        // 状态改成 阳光保险 已增员
                        entryModel.InsuranceCompany2 = record.InsuranceCompany;
                        entryModel.PlanName2 = record.PlanName;
                        entryModel.InsuranceId2 = record.InsuranceId;
                        entryModel.PlanStatus2 = UserEntryPlanStatusEnumType.已增员;
                        record.UserEntryId = item.ID;
                        record.UserEntryStatus = "2";
                        record.Status = UserEntryPlanStatusEnumType.已增员;
                        recordList.Add(record);
                    }
                    else
                    {
                        Console.WriteLine("+++++++++++++++++++++不匹配+++++++++++++++++++++++");
                        Console.WriteLine($"{item.Name}");
                        Console.WriteLine($"{item.CardID}");
                        Console.WriteLine($"{item.WorkClass}");
                        Console.WriteLine($"{item.WorkName}");
                        Console.WriteLine($"{item.CustomerName}");
                        Console.WriteLine("--------------------------------------------------");
                        // 状态改成 待增员
                        entryModel.PlanStatus2 = UserEntryPlanStatusEnumType.待增员;
                    }
                    entryModelList.Add(entryModel);
                }
                if (entryModelList.Any())
                {
                     // 更新员工参保状态
                        await _entryService.UpdateBusInsurance(entryModelList);
                }
                if (recordList.Any())
                {
                    await _businessGuaranteeService.Update(recordList);
                }
            }
            // 匹配记录
         
            var noUserEntryRecord = await _businessGuaranteeService.List(f =>  f.GenerateDate == today && f.IsMatchRecord && f.InsuranceCompany == "阳光保险公司" && f.PlanName == "10万+1万" && f.IsEnabled && f.UserEntryId <= 0);
            foreach (var item in noUserEntryRecord)
            {
                item.Remark = "未匹配到在职人员信息";
                item.Status = UserEntryPlanStatusEnumType.待减员;
                item.IsDelTag = false;
            }
            return true;
        }

        /// <summary>
        ///  商保相同保单增减匹配
        /// </summary>
        /// <param name="insuranceCompany"></param>
        /// <returns></returns>
        private async Task<bool> GeneratePushRecord()
        {

            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var nextDayMonth = nextDay.AddDays(1 - nextDay.Day);
            Live_V_BusinessInsuranceOrderPlan orderPlan = null;
            Live_BusinessInsuranceRecord record = null;

            List<BusinessInsuranceStatusEnumType> statusEnumTypes = new List<BusinessInsuranceStatusEnumType>() {
             BusinessInsuranceStatusEnumType.已增员, BusinessInsuranceStatusEnumType.已减员};
            // 删除今天需要增减员记录
            await _businessInsuranceRecordService.Delete(f => f.CreateTime >= today && !statusEnumTypes.Contains(f.Status));

            var orderPlans = await _orderService.ListOrderPlanView(f => f.StartDate.Value == nextDayMonth);
            #region 次险生成增员记录
            var otherConfigList = await _configService.ListView(f => f.InsuranceCompany == "阳光保险公司" && !f.IsMain && f.IsEnabled);
            // 月保单
            var otherOrderList = orderPlans.Where(f => f.InsuranceCompany == "阳光保险公司"&&f.GnteeName=="10万+1万" && f.OrderType == (int)InsuranceOrderEnumType.月保单);
            List<Live_BusinessInsuranceRecord> otherRecord = new List<Live_BusinessInsuranceRecord>();
            foreach (var configItem in otherConfigList)
            {
                var entryList = await _entryService.ListView(f =>f.Status==2&& f.IsManuallyInsurance==false&& f.CustomerName == configItem.CustomerName && !string.IsNullOrWhiteSpace(f.WorkName) && !string.IsNullOrWhiteSpace(f.WorkClass) && f.PlanStatus2 != null && f.PlanStatus2.Value == UserEntryPlanStatusEnumType.待增员);
                if (entryList != null && entryList.Count > 0)
                {
                    string planName = string.Empty;

                    // 获取订单信息
                    foreach (var entryItem in entryList)
                    {
                        Console.WriteLine(entryItem.Name);
                        if (entryItem.WorkClass == "3类")
                        {
                            planName = configItem.Plan3;
                        }
                        else if (entryItem.WorkClass == "4类")
                        {
                            planName = configItem.Plan4;
                        }
                        else
                        {
                            planName = configItem.Plan5;
                        }

                        if (configItem.IsDefaultPush)
                        {
                            orderPlan = otherOrderList.FirstOrDefault(f => f.IsDefaultPush && f.GnteeName == planName);
                        }
                        else
                        {
                            orderPlan = otherOrderList.FirstOrDefault(f =>!string.IsNullOrWhiteSpace(f.SigningUnitNames)&& f.SigningUnitNames.Contains(configItem.InsuranceSigningUnitName) && !f.IsDefaultPush && f.GnteeName == planName);
                        }
                        if (orderPlan!=null)
                        {
                            var ageGender = IdCardHelper.GetAgeAndGender(entryItem.CardID);
                            record = new Live_BusinessInsuranceRecord()
                            {
                                AddMatchRecordID = null,
                                AddPushTime = null,
                                AddSysTime = null,
                                Age = ageGender.Age,
                                BusinessInsurancePushID = null,
                                CreateTime = DateTime.Now,
                                CustomerGID = configItem.CustomerGID,
                                CustomerName = configItem.CustomerName,
                                DelMatchRecordID = null,
                                DelPushTime = null,
                                DelSysTime = null,
                                StartDate = orderPlan.StartDate,
                                EndDate = orderPlan.EndDate,
                                EntryDate = entryItem.EntryTime.Value.Date,
                                ID = Guid.NewGuid(),
                                IdCardNo = entryItem.CardID,
                                InsuranceCompany = orderPlan.InsuranceCompany,
                                InsuranceId = orderPlan.InsuranceId,
                                InsuranceNo = orderPlan.InsuranceNo,
                                InsuranceSigningUnitID = configItem.InsuranceSigningUnitID,
                                InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
                                IsDayOrder = false,
                                IsDefaultPush = orderPlan.IsDefaultPush,
                                IsExpired = false,
                                IsMain = configItem.IsMain,
                                LeaveDate = entryItem.LeaveTime?.Date,
                                Name = entryItem.Name,
                                OrderNo = string.Empty,
                                PlanName = orderPlan.GnteeName,
                                PlanNameId = orderPlan.GnteeId,
                                Sex = ageGender.Gender,

                                Status = BusinessInsuranceStatusEnumType.待增员,
                                UserEntryID = entryItem.ID,
                                UserID = entryItem.UserID ?? 0,
                                WorkClass = entryItem.WorkClass,
                                WorkName = entryItem.WorkName,
                            };
                            otherRecord.Add(record);
                        }
                    }
                }

            }
            #endregion
            #region 主险生成增员记录
            var mainConfigList = await _configService.ListView(f => f.InsuranceCompany == "阳光保险公司" && f.IsMain && f.IsEnabled);
            // 月保单
            var monthOrderList = orderPlans.Where(f => f.InsuranceCompany == "阳光保险公司" && f.OrderType == (int)InsuranceOrderEnumType.月保单);
            List<Live_BusinessInsuranceRecord> mainRecord = new List<Live_BusinessInsuranceRecord>();
            foreach (var configItem in mainConfigList)
            {
                var mainEntryList = await _entryService.ListView(f => f.Status == 2&& f.IsManuallyInsurance==false&& f.CustomerName == configItem.CustomerName && !string.IsNullOrWhiteSpace(f.WorkName) && !string.IsNullOrWhiteSpace(f.WorkClass) && f.PlanStatus1 != null && f.PlanStatus1.Value == UserEntryPlanStatusEnumType.待增员);
                if (mainEntryList != null && mainEntryList.Count > 0)
                {
                    string planName = string.Empty;
                    // 获取订单信息
                    foreach (var entryItem in mainEntryList)
                    {

                        Console.WriteLine(entryItem.Name);

                        if (entryItem.WorkClass == "3类")
                        {
                            planName = configItem.Plan3;
                        }
                        else if (entryItem.WorkClass == "4类")
                        {
                            planName = configItem.Plan4;
                        }
                        else
                        {
                            planName = configItem.Plan5;
                        }
                        if (configItem.IsDefaultPush)
                        {
                            orderPlan = monthOrderList.FirstOrDefault(f => f.IsDefaultPush && f.GnteeName == planName);
                        }
                        else
                        {
                            orderPlan = monthOrderList.FirstOrDefault(f => !string.IsNullOrWhiteSpace(f.SigningUnitNames)&& f.SigningUnitNames.Contains(configItem.InsuranceSigningUnitName) && !f.IsDefaultPush && f.GnteeName == planName);
                        }
                        if (orderPlan != null)
                        {
                            var ageGender = IdCardHelper.GetAgeAndGender(entryItem.CardID);
                            record = new Live_BusinessInsuranceRecord()
                            {
                                AddMatchRecordID = null,
                                AddPushTime = null,
                                AddSysTime = null,
                                Age = ageGender.Age,
                                BusinessInsurancePushID = null,
                                CreateTime = DateTime.Now,
                                CustomerGID = configItem.CustomerGID,
                                CustomerName = configItem.CustomerName,
                                DelMatchRecordID = null,
                                DelPushTime = null,
                                DelSysTime = null,
                                StartDate = orderPlan.StartDate,
                                EndDate = orderPlan.EndDate,
                                EntryDate = entryItem.EntryTime.Value.Date,
                                ID = Guid.NewGuid(),
                                IdCardNo = entryItem.CardID,
                                InsuranceCompany = orderPlan.InsuranceCompany,
                                InsuranceId = orderPlan.InsuranceId,
                                InsuranceNo = orderPlan.InsuranceNo,
                                InsuranceSigningUnitID = configItem.InsuranceSigningUnitID,
                                InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
                                IsDayOrder = orderPlan.OrderType == (int)InsuranceOrderEnumType.日保单,
                                IsDefaultPush = orderPlan.IsDefaultPush,
                                IsExpired = false,
                                IsMain = configItem.IsMain,
                                LeaveDate = entryItem.LeaveTime?.Date,
                                Name = entryItem.Name,
                                OrderNo = string.Empty,
                                PlanName = orderPlan.GnteeName,
                                PlanNameId = orderPlan.GnteeId,
                                Sex = ageGender.Gender,
                                Status = BusinessInsuranceStatusEnumType.待增员,
                                UserEntryID = entryItem.ID,
                                UserID = entryItem.UserID ?? 0,
                                WorkClass = entryItem.WorkClass,
                                WorkName = entryItem.WorkName,


                            };
                            mainRecord.Add(record);
                        }
                        else {

                            Console.WriteLine(  $"{entryItem.Name} 不匹配");
                        }
                        
                    }
                }
            }
            #endregion
            List<Live_BusinessInsuranceRecord> addRecordList = new List<Live_BusinessInsuranceRecord>();
            List<Live_BusinessInsuranceRecord> delRecordList = new List<Live_BusinessInsuranceRecord>();
            if (otherRecord.Any())
            {
                addRecordList.AddRange(otherRecord);
            }
            if (mainRecord.Any())
            {
                addRecordList.AddRange(mainRecord);
            }
            #region 增员记录匹配减员记录



             var    guaranteeRecordList = await _businessGuaranteeService.List(f => f.Status == UserEntryPlanStatusEnumType.待减员 && f.GenerateDate == today && f.IsMatchRecord&&f.IsMonthInsurance);


            Live_BusinessGuarantee guaranteeRecord;
            List<Live_BusinessGuarantee> businessGuaranteeList = new List<Live_BusinessGuarantee>();
            foreach (var item in addRecordList)
            {
                // 同保单下如果存在相同身份证号码的记录，则优先减员
                guaranteeRecord =  guaranteeRecordList.FirstOrDefault(f => f.Status == UserEntryPlanStatusEnumType.待减员 && f.GenerateDate == today && f.IsMatchRecord && f.InsuranceId == item.InsuranceId && f.IdCardNo == item.IdCardNo && !f.IsDelTag);
                if (guaranteeRecord == null)
                {
                    guaranteeRecord =  guaranteeRecordList.FirstOrDefault(f => f.Status == UserEntryPlanStatusEnumType.待减员 && f.GenerateDate == today && f.IsMatchRecord && f.InsuranceId == item.InsuranceId && !f.IsDelTag);
                }
                if (guaranteeRecord != null)
                {
                    guaranteeRecord.Status = UserEntryPlanStatusEnumType.待减员;
                    guaranteeRecord.IsDelTag = true;
                    guaranteeRecord.DelRecordID = item.ID;
                    businessGuaranteeList.Add(guaranteeRecord);
                }
                else
                {
                    #region 特殊处理代码
                    List<string> insuranceIds = new List<string>() { "NBXL20260227160844926", "NBXL20260227203819197" };
                    if (insuranceIds.Contains(item.InsuranceId))
                    {
                        guaranteeRecord = guaranteeRecordList.FirstOrDefault(f => f.Status == UserEntryPlanStatusEnumType.待减员 && f.GenerateDate == today && f.IsMatchRecord && insuranceIds.Contains(f.InsuranceId) && f.IdCardNo == item.IdCardNo && !f.IsDelTag);
                        if (guaranteeRecord == null)
                        {
                            guaranteeRecord = guaranteeRecordList.FirstOrDefault(f => f.Status == UserEntryPlanStatusEnumType.待减员 && f.GenerateDate == today && f.IsMatchRecord && insuranceIds.Contains(f.InsuranceId) && !f.IsDelTag);
                        }
                        if (guaranteeRecord != null)
                        {
                            guaranteeRecord.Status = UserEntryPlanStatusEnumType.待减员;
                            guaranteeRecord.IsDelTag = true;
                            guaranteeRecord.DelRecordID = item.ID;
                            businessGuaranteeList.Add(guaranteeRecord);
                            orderPlan = monthOrderList.FirstOrDefault(f => f.InsuranceId == guaranteeRecord.InsuranceId);
                            if (orderPlan != null)
                            {
                                item.InsuranceId = orderPlan.InsuranceId;
                                item.InsuranceNo = orderPlan.InsuranceNo;
                                item.PlanName = orderPlan.GnteeName;
                                item.PlanNameId = orderPlan.GnteeId;
                            }
                        }
                    }
                    #endregion

                    if (guaranteeRecord==null)
                    {
                        if (nextDay.Day >= 8)
                        {
                            if (item.IsDefaultPush)
                            {
                                orderPlan = orderPlans.FirstOrDefault(f => f.OrderType == 0 && f.InsuranceCompany == item.InsuranceCompany && f.IsDefaultPush && f.GnteeName == item.PlanName);
                            }
                            else
                            {
                                orderPlan = orderPlans.FirstOrDefault(f => f.OrderType == 0 && f.InsuranceCompany == item.InsuranceCompany&&!string.IsNullOrWhiteSpace(f.SigningUnitNames) && f.SigningUnitNames.Contains(item.InsuranceSigningUnitName) && !f.IsDefaultPush && f.GnteeName == item.PlanName);
                            }
                            // 日保单增员
                            if (orderPlan != null)
                            {
                                item.InsuranceId = orderPlan.InsuranceId;
                                item.InsuranceNo = orderPlan.InsuranceNo;
                                item.PlanName = orderPlan.GnteeName;
                                item.PlanNameId = orderPlan.GnteeId;
                                item.IsDayOrder = true;
                            }
                        }
                    }
                



                   
                }
            }
            // 日保单 未匹配到在职记录 则全部减员
            var delList = await _businessGuaranteeService.List(f => f.GenerateDate.Value.Date == today && f.IsMatchRecord && f.UserEntryId <= 0 && !f.IsMonthInsurance);
            if (delList != null && delList.Count > 0)
            {
                businessGuaranteeList.AddRange(delList);
            }
            if (businessGuaranteeList != null && businessGuaranteeList.Count > 0)
            {
                Live_BusinessInsuranceRecord delRecord = null;
                foreach (var item in businessGuaranteeList)
                {
                    item.Status = UserEntryPlanStatusEnumType.待减员;
                    orderPlan = orderPlans.FirstOrDefault(f => f.InsuranceId == item.InsuranceId && f.GnteeName == item.PlanName);
                    if (orderPlan!=null)
                    {
                        delRecord = new Live_BusinessInsuranceRecord()
                        {
                            AddMatchRecordID = null,
                            DelMatchRecordID = item.DelRecordID,
                            AddPushTime = null,
                            AddSysTime = null,
                            Age = 0,
                            BusinessInsurancePushID = null,
                            CreateTime = DateTime.Now,
                            CustomerGID = Guid.Empty,
                            CustomerName = item.CustomerName,
                            DelPushTime = null,
                            DelSysTime = null,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            EntryDate = item.EntryTime,
                            ID = Guid.NewGuid(),
                            IdCardNo = item.IdCardNo,
                            InsuranceCompany = item.InsuranceCompany,
                            InsuranceId = item.InsuranceId,
                            InsuranceNo = orderPlan.InsuranceNo,
                            InsuranceSigningUnitID = Guid.Empty,
                            InsuranceSigningUnitName = item.SignUnitName,
                            IsDayOrder = !item.IsMonthInsurance,
                            IsDefaultPush = orderPlan.IsDefaultPush,
                            IsExpired = false,
                            IsMain = orderPlan.InsuranceCompany == "阳光保险公司",
                            LeaveDate = null,
                            Name = item.Name,
                            OrderNo = string.Empty,
                            PlanName = orderPlan.GnteeName,
                            PlanNameId = orderPlan.GnteeId,
                            Sex = string.Empty,
                            Status = BusinessInsuranceStatusEnumType.待减员,
                            UserEntryID = item.UserEntryId,
                            UserID = 0,
                            WorkClass = item.WorkClass,
                            WorkName = item.WorkName,
                        };
                        delRecordList.Add(delRecord);
                    }
                }
            }



            List<Live_BusinessInsuranceRecord> recordList = new List<Live_BusinessInsuranceRecord>();
            if (addRecordList.Any())
            {
                recordList.AddRange(addRecordList);
            }

            if (delRecordList.Any())
            {
                recordList.AddRange(delRecordList);
            }
            if (recordList.Any())
            {
                await _businessInsuranceRecordService.Add(recordList);
            }

            if (businessGuaranteeList != null && businessGuaranteeList.Any())
            {
                await _businessGuaranteeService.Update(businessGuaranteeList);
            }
            #endregion
            return true;
        }
        private async Task<bool> PushRecord()
        {
            var today = DateTime.Today;
            var delRecordList = await _businessInsuranceRecordService.List(f => f.CreateTime >= today && f.Status == BusinessInsuranceStatusEnumType.待减员);
            var addRecordList = await _businessInsuranceRecordService.List(f => f.CreateTime >= today && f.Status == BusinessInsuranceStatusEnumType.待增员 && f.PlanNameId > 0 && !string.IsNullOrWhiteSpace(f.PlanName));
            var delGroupList = delRecordList.GroupBy(f => new {  f.InsuranceId }).ToList();
            var addGroupList = addRecordList.GroupBy(f => new { f.InsuranceId, f.PlanName, f.PlanNameId }).ToList();
            BatchDelEmployeeRequest request = null;
            foreach (var groupItem in delGroupList)
            {
                var records = groupItem.ToList();
                request = new BatchDelEmployeeRequest()
                {
                    InsureId = groupItem.Key.InsuranceId,
                    CustomerList = records.Select(f => new DelCustomerinfo()
                    {
                        Custemername = f.Name,
                        Custemernum = f.IdCardNo,
                    }).ToList()
                };

                var resp = await _insuranceApiService.BatchDelEmployee(request);
                if (resp != null)
                {
                  
                    if (resp.Code == 200)
                    {
                        foreach (var item in records)
                        {
                            item.Status = BusinessInsuranceStatusEnumType.已减员;
                            item.PushTime = DateTime.Now;
                            item.IndecaseId = resp.IndecaseId;
                            item.PushMsg = resp.Msg;
                        }
                        if (resp.FailList != null && resp.FailList.Count > 0)
                        {
                            foreach (var item in resp.FailList)
                            {
                                var failRecord = records.First(f => f.IdCardNo == item.Customerinfo.Custemernum && f.Name == item.Customerinfo.Custemername);
                                failRecord.Status = BusinessInsuranceStatusEnumType.减员失败;
                                failRecord.PushMsg = item.Msg;
                            }
                        }
                    }
                    else
                    {

                        foreach (var item in records)
                        {
                            item.Status = BusinessInsuranceStatusEnumType.减员失败;
                            item.PushTime = DateTime.Now;
                            item.IndecaseId = resp.IndecaseId;
                            item.PushMsg = resp.Msg;
                        }
                    }
                }
                else
                {
                    foreach (var item in records)
                    {
                        item.Status = BusinessInsuranceStatusEnumType.减员失败;
                    }
                    //Common.StaticLog.Info($"商保减员推送响应：订单编号：{groupItem.Key.InsuranceId}，响应信息：Null,未获取到正确的响应信息！");
                }
                await _businessInsuranceRecordService.Update(records);
               await Task.Delay(500);
            }

          await  Task.Delay(2000);
            BatchAddEmployeeRequest addRequest = null;
            foreach (var groupItem in addGroupList)
            {
                var records = groupItem.ToList();
                addRequest = new BatchAddEmployeeRequest()
                {
                    InsureId = groupItem.Key.InsuranceId,
                    GnteeId = groupItem.Key.PlanNameId,
                    GnteeName = groupItem.Key.PlanName,
                    CustomerList = records.Select(f => new AddCustomerinfo()
                    {
                        Custemername = f.Name,
                        Custemernum = f.IdCardNo,
                        Custemerage = f.Age.ToString(),
                        Custemersex = f.Sex,
                        Custemertype = "身份证",
                        Guyuangongzhong = f.WorkName,
                        Zhiyeleibei = f.WorkClass.Substring(0, 1),
                        Ldhtdw = f.InsuranceSigningUnitName,
                        Sjygdw = f.CustomerName,
                    }).ToList()
                };

                var resp = await _insuranceApiService.BatchAddEmployee(addRequest);
                if (resp != null)
                {
                    //Common.StaticLog.Info($"商保增员推送响应：订单编号：{groupItem.Key.InsuranceId}，响应信息：{Newtonsoft.Json.JsonConvert.SerializeObject(resp)}");


                    if (resp.Code == 200)
                    {
                        foreach (var item in records)
                        {
                            item.Status = BusinessInsuranceStatusEnumType.已增员;
                            item.PushTime = DateTime.Now;
                            item.IndecaseId = resp.IndecaseId;
                            item.PushMsg = resp.Msg;
                        }
                        if (resp.FailList != null && resp.FailList.Count > 0)
                        {
                            foreach (var item in resp.FailList)
                            {
                                var failRecord = records.First(f => f.IdCardNo == item.Customerinfo.Custemernum && f.Name == item.Customerinfo.Custemername);
                                failRecord.Status = BusinessInsuranceStatusEnumType.增员失败;
                                failRecord.PushMsg = item.Msg;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in records)
                        {
                            item.Status = BusinessInsuranceStatusEnumType.增员失败;
                            item.PushTime = DateTime.Now;
                            item.IndecaseId = resp.IndecaseId;
                            item.PushMsg = resp.Msg;
                        }
                    }
                }
                else
                {
                    foreach (var item in records)
                    {
                        item.Status = BusinessInsuranceStatusEnumType.增员失败;
                    }
                    //Common.StaticLog.Info($"商保增员推送响应：订单编号：{groupItem.Key.InsuranceId}，响应信息：Null,未获取到正确的响应信息！");
                }
                await _businessInsuranceRecordService.Update(records);

                await Task.Delay(500);

            }
            return true;
        }
        public async Task EmailNotice()
        {

            List< BxNoticeExcelTemplate > noticeExcelTemplates = new List< BxNoticeExcelTemplate >();
            string msg = string.Empty;
            var configList = await _configService.ListView(f => f.IsEnabled && f.IsMain);
            foreach (var item in configList)
            {
                var entryList = await _entryService.List(f => f.CustomerGID == item.CustomerGID && f.Status == 2 && f.PlanStatus1 != UserEntryPlanStatusEnumType.已增员);
                if (entryList.Any())
                {
                    var customerExcelList = entryList.Select(f => new BxNoticeExcelTemplate()
                    {
                        InsuranceCompany = item.InsuranceCompany,
                        SignUnitName = item.InsuranceSigningUnitName,
                        CustomerName = item.CustomerName,
                        IdCardNo = f.CardID,
                        Name = f.Name,
                        PlanStatusName = f.PlanStatus1.HasValue ? f.PlanStatus1.Value.ToString() : "未参保",
                        StatusName = "在职",
                        WorkClass = f.WorkClass,
                        WorkName = f.WorkName,
                         EntryDate=f.EntryTime.Value.Date.ToString("yyyy-MM-dd"),
                        IsManuallyInsurance = f.IsManuallyInsurance?"是":"否"
                    }).ToList();

                    noticeExcelTemplates.AddRange(customerExcelList);

                       //msg += $"{item.CustomerName}--未增员名单，{string.Join(',', entryList.Select(f => $"{f.Name}（{f.PlanStatus1}）"))}\r\n";
                }
            }

            //if (!string.IsNullOrWhiteSpace(msg))
            //{
            //    _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "商保增减异常通知", msg, "", "");
            //    _aliSendInfo.SendAliEmail("15261300969@163.com", "商保增减异常通知", msg, "", "");
            //}
            //else
            //{
            //    _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "商保推送完成通知", "无增减推送异常人员", "", "");
            //    _aliSendInfo.SendAliEmail("15261300969@163.com", "商保推送完成通知", "无增减推送异常人员", "", "");
            //}





            if (noticeExcelTemplates.Any())
            {
                var fileBytes = NpoiExcelHelper.WriteExcel(noticeExcelTemplates, "Sheet1");

                using (var ms = new MemoryStream(fileBytes))
                {
                    string content = $"商保推送完成";
                    var res = await _aliyunOSSService.UploadFile(ms, "11.xlsx", false, true);
                    _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", $"商保推送通知（{DateTime.Today.ToString("yyyy-MM-dd")}）", content, "", $"/uploadfiles/{res.Data}");
                    _aliSendInfo.SendAliEmail("15261300969@163.com", $"商保推送通知（{DateTime.Today.ToString("yyyy-MM-dd")}）", content, "", $"/uploadfiles/{res.Data}");
                }
            }
            else {
                _aliSendInfo.SendAliEmail("yyy@cn-labor.cn", $"商保推送通知（{DateTime.Today.ToString("yyyy-MM-dd")}）", "无增减推送异常人员", "", "");
                _aliSendInfo.SendAliEmail("15261300969@163.com", $"商保推送通知（{DateTime.Today.ToString("yyyy-MM-dd")}）", "无增减推送异常人员", "", "");

            }


           


        }

        public async Task PushProcess()
        {

            await GenerateApiRecord(isMatch: true);
            await Task.Delay(1000);
            await PlanName1Match(isMatch: true);
            await Task.Delay(1000);
            await PlanName2Match(isMatch: true);
            await Task.Delay(1000);
            await GeneratePushRecord();
            await Task.Delay(1000);
            await PushRecord();
        }

        public async Task CheckProcess()
        {
            await GenerateApiRecord(isMatch: false);
            await Task.Delay(1000);
            await PlanName1Match(isMatch: false);
            await Task.Delay(1000);
            await PlanName2Match(isMatch: false);
            await Task.Delay(1000);
            await EmailNotice();
        }
    }
}
