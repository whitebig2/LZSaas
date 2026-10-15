


//using LZ.ApiService.Interface;
//using LZ.ApiService.Services;
//using LZ.Core.EventData;
//using LZ.Core.EventData.Contract;
//using LZ.Core.EventData.FxqEventData;
//using LZ.Core.EventData.Insurance;
//using LZ.Core.RabbitMQ;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Customer.Service.Services;
//using LZ.Model;
//using LZ.Model.Dtos;
//using LZ.Model.Enums;
//using LZ.Model.Exceptions;
//using LZ.Model.FxqModel;
//using LZ.Model.InsuranceApi;
//using LZ.Model.PagedModel;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using NPOI.HPSF;
//using Senparc.CO2NET.Extensions;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
//using SqlSugar;
//using SqlSugar.IOC;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography;
//using static LZ.Customer.API.Controllers.AATestController;

//namespace LZ.Customer.API.Controllers
//{

//    [AllowAnonymous]
//    public class BusinessJobController(
//         IServiceScopeFactory scopeFactory,
//        IAliyunOSSService aliyunOSSService,
//        IEventPublisher eventPublisher) : CustomerControllerBase
//    {
//        /// <summary>
//        ///  商保推送定时任务
//        /// </summary>
//        /// <param name="insuranceCompany"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<IActionResult> BusinessInsuranceJobExecute(string insuranceCompany)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var today = DateTime.Today;
//                var nextDay = today.AddDays(1);
//                var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
//                var insuranceOrderService = scope.ServiceProvider.GetRequiredService<IInsuranceOrderService>();
//                var insurancePushService = scope.ServiceProvider.GetRequiredService<IInsurancePushService>();
//                var insuranceApiService = scope.ServiceProvider.GetRequiredService<IInsuranceApiService>();
//                var pushRecordService = scope.ServiceProvider.GetRequiredService<IInsurancePushRecordService>();
//                var businessInsuranceRecordService = scope.ServiceProvider.GetRequiredService<IBusinessInsuranceRecordService>();
//                // 指定保险公司
//                var userEntryInsuranceRecordList = await businessInsuranceRecordService.List(
//                    f => f.InsuranceCompany == insuranceCompany
//                    && (f.Status == Model.Enums.InsuranceSendStatusEnumType.待增员 || f.Status == Model.Enums.InsuranceSendStatusEnumType.待减员));
//                var orderPlans = await insuranceOrderService.GetOrderPlanViewList(f => f.InsuranceCompany == insuranceCompany && f.StartDate <= nextDay && f.EndDate >= nextDay);
//                var dayOrderPlans = orderPlans.Where(f => f.OrderType == (int)InsuranceOrderEnumType.日保单);
//                var monthOrderPlans = orderPlans.Where(f => f.OrderType == (int)InsuranceOrderEnumType.月保单);
//                List<Live_BusinessInsuranceRecord> businessList = new List<Live_BusinessInsuranceRecord>();
//                #region 增减员匹配
//                foreach (var item in monthOrderPlans)
//                {
//                    var addRecords = userEntryInsuranceRecordList.Where(f => f.Status == InsuranceSendStatusEnumType.待增员 && f.OrderNo == item.OrderNo && f.PlanName == item.GnteeName).ToList();
//                    var delRecords = userEntryInsuranceRecordList.Where(f => f.Status == InsuranceSendStatusEnumType.待减员 && f.OrderNo == item.OrderNo && f.PlanName == item.GnteeName).ToList();
//                    var sameIdCardNoList = addRecords.Select(f => f.IdCardNo).Intersect(delRecords.Select(f => f.IdCardNo)).Distinct().ToList();
//                    if (sameIdCardNoList != null && sameIdCardNoList.Count > 0)
//                    {
//                        // 身份证号码相同
//                        foreach (var idcardNo in sameIdCardNoList)
//                        {
//                            var sameAddRecord = addRecords.First(f => f.IdCardNo == idcardNo);
//                            var sameDelRecord = delRecords.First(f => f.IdCardNo == idcardNo);
//                            // 增员需要减员一条
//                            sameAddRecord.AddMatchRecordID = sameDelRecord.ID;
//                            sameDelRecord.DelMatchRecordID = sameAddRecord.ID;
//                        }
//                    }
//                    foreach (var addRecord in addRecords)
//                    {
//                        if (sameIdCardNoList.Contains(addRecord.IdCardNo))
//                        {
//                            continue;
//                        }

//                        if (delRecords.Any(f => f.DelMatchRecordID == Guid.Empty))
//                        {
//                            var delRecord = delRecords.First(f => f.DelMatchRecordID == Guid.Empty);
//                            addRecord.AddMatchRecordID = delRecord.ID;
//                            delRecord.DelMatchRecordID = addRecord.ID;
//                        }
//                        else
//                        {
//                            if (item.IsCommon)
//                            {
//                                var dayPlan = dayOrderPlans.FirstOrDefault(f => f.IsCommon == true && f.GnteeName == addRecord.PlanName);
//                                // 日保单存在则推送日保单，否则推送月保单
//                                if (dayPlan != null)
//                                {
//                                    addRecord.OrderType = InsuranceOrderEnumType.日保单;
//                                    addRecord.OrderNo = dayPlan.OrderNo;
//                                    addRecord.EndDate = dayPlan.EndDate;
//                                    addRecord.PlanNameID = dayPlan.GnteeId;

//                                }
//                            }
//                            else
//                            {
//                                var dayPlan = dayOrderPlans.FirstOrDefault(f => f.IsCommon == false && f.SigningUnitName == addRecord.InsuranceSigningUnitName && f.GnteeName == addRecord.PlanName);
//                                // 日保单存在则推送日保单，否则推送月保单
//                                if (dayPlan != null)
//                                {
//                                    addRecord.OrderType = InsuranceOrderEnumType.日保单;
//                                    addRecord.OrderNo = dayPlan.OrderNo;
//                                    addRecord.EndDate = dayPlan.EndDate;
//                                    addRecord.PlanNameID = dayPlan.GnteeId;

//                                }
//                            }
//                        }

//                    }
//                    businessList.AddRange(addRecords);
//                    businessList.AddRange(delRecords.Where(f => f.DelMatchRecordID != Guid.Empty||f.OrderType== InsuranceOrderEnumType.日保单));
//                }
//                var groups = businessList.GroupBy(f => new { f.OrderNo, f.PlanName, f.PlanNameID, f.OrderType }).ToList();
//                List<Live_InsurancePushExtension> pushList = new List<Live_InsurancePushExtension>();
//                foreach (var groupItem in groups)
//                {
//                    var groupPushRecords = groupItem.Where(f => f.Status == InsuranceSendStatusEnumType.待增员 || f.Status == InsuranceSendStatusEnumType.待减员).ToList();
//                    Live_InsurancePushExtension pushModel = new Live_InsurancePushExtension()
//                    {
//                        ID = Guid.NewGuid(),
//                        AddFailCount = 0,
//                        AddIndecaseId = string.Empty,
//                        AddPushStatus = InsurancePushStatusEnumType.待推送,
//                        AddPushTime = DateTime.Now,
//                        AddRecordCount = groupPushRecords.Count(f => f.Status == InsuranceSendStatusEnumType.待增员),
//                        AddResCode = 0,
//                        AddResMsg = string.Empty,
//                        CreateTime = DateTime.Now,
//                        DelFailCount = 0,
//                        DelIndecaseId = string.Empty,
//                        DelPushStatus = InsurancePushStatusEnumType.待推送,
//                        DelPushTime = DateTime.Now,
//                        DelRecordCount = groupPushRecords.Count(f => f.Status == InsuranceSendStatusEnumType.待减员),
//                        DelResCode = 0,
//                        DelResMsg = string.Empty,
//                        GnteeId = groupItem.Key.PlanNameID,
//                        GnteeName = groupItem.Key.PlanName,
//                        OrderNo = groupItem.Key.OrderNo,
//                        InsuranceOrderPlanID = Guid.Empty,
//                        Status = InsurancePushStatusEnumType.待推送,
//                        PushRecords = groupPushRecords.Select(f => new Live_InsurancePushRecord()
//                        {
//                            Custemerage = f.Age.ToString(),
//                            Custemername = f.Name,
//                            Custemernum = f.IdCardNo,
//                            Custemersex = f.Sex,
//                            Custemertype = "身份证",
//                            Guyuangongzhong = f.JobType,
//                            ID = Guid.NewGuid(),
//                            IsAdd = f.Status == InsuranceSendStatusEnumType.待增员 ? true : false,
//                            IsPushSuc = false,
//                            Ldhtdw = f.InsuranceSigningUnitName,
//                            Remark = string.Empty,
//                            Sjygdw = f.CustomerName,
//                            UserEntryID = f.UserEntryID,
//                            Zhiyeleibei = f.JobLevelType,
//                            BusinessInsuranceRecordID = f.ID,
//                            UserEntryInsuracneRecordID = Guid.Empty,
//                            IsApiPush = false,
//                            IsMain = f.IsMain,
//                            PushTime = DateTime.MinValue,
//                            IsPushing = false,

//                        }).ToList()
//                    };
//                    pushList.Add(pushModel);
//                }
//                DbScoped.SugarScope.BeginTran();
//                if (pushList.Any())
//                {
//                    foreach (var item in pushList)
//                    {
//                        await insurancePushService.AddNav(item);
//                    }
//                }
//                if (businessList != null && businessList.Count > 0)
//                {
//                    await businessInsuranceRecordService.Update(businessList);
//                }
//                DbScoped.SugarScope.CommitTran();

//                #endregion


//                #region 推送
//                var apiPushList = await insurancePushService.ListNav(f => f.AddPushStatus == InsurancePushStatusEnumType.待推送 || f.DelPushStatus == InsurancePushStatusEnumType.待推送);
//                foreach (var item in apiPushList)
//                {
//                    #region 减员推送
//                    if (item.DelPushStatus == InsurancePushStatusEnumType.待推送 && item.DelRecordCount > 0)
//                    {
//                        var apiDelRecordList = item.PushRecords.Where(f => f.IsAdd == false).ToList();
//                        BatchDelEmployeeRequest batchDelRequest = new BatchDelEmployeeRequest()
//                        {
//                            InsureId = item.OrderNo,
//                            CustomerList = apiDelRecordList.Select(f => new DelCustomerinfo()
//                            {
//                                Custemername = f.Custemername,
//                                Custemernum = f.Custemernum
//                            }).ToList()
//                        };
//                        var delResp = await insuranceApiService.BatchDelEmployee(batchDelRequest);
//                        if (delResp != null)
//                        {
//                            item.DelFailCount = delResp.FailNum;
//                            item.DelPushTime = DateTime.Now;
//                            item.DelResCode = delResp.Code;
//                            item.DelResMsg = delResp.Msg ?? string.Empty;
//                            item.DelIndecaseId = delResp.IndecaseId ?? string.Empty;
//                            if (delResp.Code == 200)
//                            {
//                                item.DelPushStatus = InsurancePushStatusEnumType.已推送;

//                                foreach (var apiDelRecord in apiDelRecordList)
//                                {
//                                    apiDelRecord.IsPushSuc = true;
//                                    apiDelRecord.IsApiPush = true;
//                                }

//                                if (delResp.FailList != null && delResp.FailList.Count > 0)
//                                {
//                                    foreach (var failItem in delResp.FailList)
//                                    {
//                                        var delItem = apiDelRecordList.First(f => f.Custemernum == failItem.Customerinfo.Custemernum);
//                                        delItem.Remark = failItem.Msg;
//                                        delItem.IsPushSuc = false;
//                                    }
//                                }
//                            }
//                            else if (delResp.Code == 500)
//                            {
//                                item.DelPushStatus = InsurancePushStatusEnumType.推送失败;
//                                item.DelPushTime = DateTime.Now;
//                                item.DelResCode = delResp.Code;
//                                item.DelResMsg = delResp.Msg;
//                                foreach (var failItem in delResp.FailList)
//                                {
//                                    var delItem = apiDelRecordList.First(f => f.Custemernum == failItem.Customerinfo.Custemernum);
//                                    delItem.Remark = failItem.Msg;
//                                    delItem.IsPushSuc = false;
//                                }
//                            }
//                            else
//                            {
//                                item.DelPushStatus = InsurancePushStatusEnumType.推送异常;
//                                item.DelPushTime = DateTime.Now;
//                                item.DelResCode = 0;
//                                item.DelResMsg = "推送异常请联系管理员";
//                                foreach (var failItem in delResp.FailList)
//                                {
//                                    var delItem = apiDelRecordList.First(f => f.Custemernum == failItem.Customerinfo.Custemernum);
//                                    delItem.Remark = failItem.Msg;
//                                    delItem.IsPushSuc = false;
//                                }

//                            }
//                        }
//                        else
//                        {
//                            item.DelPushStatus = InsurancePushStatusEnumType.推送异常;
//                            item.DelPushTime = DateTime.Now;
//                            item.DelResCode = 0;
//                            item.DelResMsg = "推送异常请联系管理员";
//                        }
//                        await insurancePushService.UpdateNav(item);
//                    }
//                    #endregion
//                    await Task.Delay(1000);
//                    #region 增员土推送
//                    if (item.AddPushStatus == InsurancePushStatusEnumType.待推送 && item.AddRecordCount > 0)
//                    {
//                        var apiAddRecordList = item.PushRecords.Where(f => f.IsAdd == true).ToList();
//                        var addRequest = new BatchAddEmployeeRequest()
//                        {
//                            GnteeId = item.GnteeId,
//                            GnteeName = item.GnteeName,
//                            InsureId = item.OrderNo,
//                            CustomerList = apiAddRecordList.Select(f => new AddCustomerinfo()
//                            {
//                                Custemerage = f.Custemerage,
//                                Custemername = f.Custemername,
//                                Custemernum = f.Custemernum,
//                                Custemersex = f.Custemersex,
//                                Custemertype = f.Custemertype,
//                                Guyuangongzhong = f.Guyuangongzhong,
//                                Ldhtdw = f.Ldhtdw,
//                                Sjygdw = f.Sjygdw,
//                                Zhiyeleibei = f.Zhiyeleibei,
//                            }).ToList(),

//                        };
//                        var addResp = await insuranceApiService.BatchAddEmployee(addRequest);
//                        if (addResp != null)
//                        {
//                            item.AddFailCount = addResp.FailNum;
//                            item.AddPushTime = DateTime.Now;
//                            item.AddResCode = addResp.Code;
//                            item.AddResMsg = addResp.Msg ?? string.Empty;
//                            item.AddIndecaseId = addResp.IndecaseId ?? string.Empty;
//                            if (addResp.Code == 200)
//                            {
//                                item.AddPushStatus = InsurancePushStatusEnumType.已推送;
//                                foreach (var addRecordItem in apiAddRecordList)
//                                {
//                                    addRecordItem.IsPushSuc = true;
//                                    addRecordItem.IsApiPush = true;
//                                }

//                                if (addResp.FailList != null && addResp.FailList.Count > 0)
//                                {
//                                    foreach (var failItem in addResp.FailList)
//                                    {
//                                        var addItem = apiAddRecordList.First(f => f.Custemernum == failItem.Customerinfo.Custemernum);
//                                        addItem.IsPushSuc = false;
//                                        addItem.Remark = failItem.Msg;
//                                    }
//                                }
//                            }
//                            else if (addResp.Code == 500)
//                            {
//                                item.AddPushStatus = InsurancePushStatusEnumType.推送失败;
//                                item.AddPushTime = DateTime.Now;
//                                item.AddResCode = addResp.Code;
//                                item.AddResMsg = addResp.Msg;
//                                foreach (var addItem in apiAddRecordList)
//                                {
//                                    addItem.IsPushSuc = false;
//                                    addItem.Remark = "推送失败";
//                                }
//                            }

//                        }
//                        else
//                        {
//                            item.AddPushStatus = InsurancePushStatusEnumType.推送异常;
//                            item.AddResMsg = "推送异常请联系管理员";
//                        }

//                        await insurancePushService.UpdateNav(item);
//                    }
//                    #endregion
//                }
//                #endregion

//                #region 修改员工状态
//                var pushRecordList = await pushRecordService.List(f => f.IsApiPush && f.PushTime >= DateTime.Today);
//                var businessInsuranceRecordIDList = pushRecordList.Select(f => f.BusinessInsuranceRecordID).Distinct().ToList();
//                var busineeRecordList = await businessInsuranceRecordService.List(f => businessInsuranceRecordIDList.Contains(f.ID));



//                foreach (var item in pushRecordList)
//                {
//                    var bussinessRecord = busineeRecordList.First(f => f.ID == item.BusinessInsuranceRecordID.Value);
//                    if (item.IsAdd)
//                    {
//                        if (item.IsPushSuc)
//                        {
//                            bussinessRecord.Status = InsuranceSendStatusEnumType.已增员;
//                        }
//                        else
//                        {
//                            bussinessRecord.Status = InsuranceSendStatusEnumType.增员异常;
//                        }
//                    }
//                    else
//                    {
//                        if (item.IsPushSuc)
//                        {
//                            bussinessRecord.Status = InsuranceSendStatusEnumType.已减员;
//                        }
//                        else
//                        {
//                            bussinessRecord.Status = InsuranceSendStatusEnumType.减员异常;
//                        }
//                    }
//                }
//                await businessInsuranceRecordService.Update(busineeRecordList);


//                var mainAddRecordList = busineeRecordList.Where(f => f.IsMain && (f.Status == InsuranceSendStatusEnumType.已增员 || f.Status == InsuranceSendStatusEnumType.增员异常)).ToList();
//                var mainDelRecordList = busineeRecordList.Where(f => f.IsMain && (f.Status == InsuranceSendStatusEnumType.已减员 || f.Status == InsuranceSendStatusEnumType.减员异常)).ToList();
//                List<int> userEntryIdList = mainAddRecordList.Select(f => f.UserEntryID).ToList();
//                foreach (var item in mainAddRecordList)
//                {
//                    UserInsuranceInsuranceStatusUpdateEventData changeStatus = new UserInsuranceInsuranceStatusUpdateEventData()
//                    {
//                        UserEntryID = item.UserEntryID,
//                        InsuranceStatus = UserEntryInsuranceStatusEnumType.已增员
//                    };
//                    if (item.Status == InsuranceSendStatusEnumType.增员异常)
//                    {
//                        changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.增员异常;
//                    }
//                    publisher.Publish(changeStatus);
//                }

//                foreach (var item in mainDelRecordList)
//                {
//                    if (userEntryIdList.Contains(item.UserEntryID))
//                    {
//                        continue;
//                    }
//                    UserInsuranceInsuranceStatusUpdateEventData changeStatus = new UserInsuranceInsuranceStatusUpdateEventData()
//                    {
//                        UserEntryID = item.UserEntryID,
//                        InsuranceStatus = UserEntryInsuranceStatusEnumType.已减员
//                    };
//                    if (item.Status == InsuranceSendStatusEnumType.减员异常)
//                    {
//                        changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.减员异常;
//                    }
//                    publisher.Publish(changeStatus);
//                }

//                #endregion





//            }
//            return Ok();
//        }
//    }
//}
