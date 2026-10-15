//using LZ.ApiService.Interface;
//using LZ.ApiService.Services;
//using LZ.Core.EventData;
//using LZ.Core.EventData.Insurance;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Customer.Service.Services;
//using LZ.Model;
//using LZ.Model.Enums;
//using LZ.Model.InsuranceApi;
//using Microsoft.Extensions.DependencyInjection;
//using NPOI.HSSF.Record;
//using SqlSugar.IOC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Customer.Service.EventHandler.InsuranceHandler
//{
//    /// <summary>
//    ///  指定保险公司推送
//    /// </summary>
//    /// <param name="scopeFactory"></param>
//    public class InsurancePushJobEventHandler(IServiceScopeFactory scopeFactory ) : IEventHandler<InsurancePushJobEventData1>
//    {
//        public async Task HandleAsync(InsurancePushJobEventData1 @event)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var today = DateTime.Today;
//                var nextDay = today.AddDays(1);
//                var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
//                var userEntryInsuracneRecordService = scope.ServiceProvider.GetRequiredService<IUserEntryInsuracneRecordService>();
//                var insuranceOrderService = scope.ServiceProvider.GetRequiredService<IInsuranceOrderService>();
//                var insurancePushService = scope.ServiceProvider.GetRequiredService<IInsurancePushService>();
//                var insuranceApiService = scope.ServiceProvider.GetRequiredService<IInsuranceApiService>();
//                var pushRecordService = scope.ServiceProvider.GetRequiredService<IInsurancePushRecordService>();
//                // 指定保险公司
//                var userEntryInsuranceRecordList = await userEntryInsuracneRecordService.List(
//                    f => f.InsuranceCompany == @event.InsuranceCompany 
//                    &&(f.SendStatus == Model.Enums.InsuranceSendStatusEnumType.待增员 || f.SendStatus == Model.Enums.InsuranceSendStatusEnumType.待减员));
//                var orderPlans = await insuranceOrderService.GetOrderPlanViewList(f => f.InsuranceCompany == @event.InsuranceCompany && f.StartDate <= nextDay && f.EndDate >= nextDay);
//                var dayOrderPlans = orderPlans.Where(f => f.OrderType == (int)InsuranceOrderEnumType.日保单);
//                var monthOrderPlans = orderPlans.Where(f => f.OrderType == (int)InsuranceOrderEnumType.月保单);

//                var addRecords = userEntryInsuranceRecordList.Where(f => f.SendStatus == InsuranceSendStatusEnumType.待增员).ToList();
//                var delRecords = userEntryInsuranceRecordList.Where(f => f.SendStatus == InsuranceSendStatusEnumType.待减员).ToList();

//                List<string> sameIdCardNos = new List<string>();
//                Live_UserEntryInsuracneRecord addRecord = null;
//                Live_UserEntryInsuracneRecord delRecord = null;
//                Live_V_InsuranceOrderPlan dayOrderPlan = null;
//                #region 增员处理
//                List<Live_UserEntryInsuracneRecord> addOrDelRecords = new List<Live_UserEntryInsuracneRecord>();
//                // 增员优先匹配月保单减员替换
//                foreach (var monthPlanItem in monthOrderPlans)
//                {
//                    sameIdCardNos.Clear();
//                    if (monthPlanItem.IsCommon)
//                    {
//                        // 方案增员记录
//                        var planMatchAddRecords = addRecords.Where(f =>
//                          f.IsCommon == monthPlanItem.IsCommon// 常规推送
//                          && f.PlanName == monthPlanItem.GnteeName// 方案相同
//                          ).ToList();
//                        if (planMatchAddRecords == null || planMatchAddRecords.Count <= 0)
//                        {
//                            continue;
//                        }

//                        planMatchAddRecords.AsParallel().ForAll(f =>
//                        {
//                            f.InsuranceOrderID = monthPlanItem.InsuranceOrderID;
//                            f.InsuranceStartDate = monthPlanItem.StartDate;
//                            f.InsuranceEndDate = monthPlanItem.EndDate;
//                            f.InsuranceOrderPlanID = monthPlanItem.ID;
//                            f.OrderNo= monthPlanItem.OrderNo;
//                            f.InsuranceNo=monthPlanItem.InsuranceNo;
//                        });

//                        var idcardNoList = planMatchAddRecords.Select(f => f.IdCardNo);

//                        var delMatchDelRecords = delRecords.Where(f => f.InsuranceNo== monthPlanItem.InsuranceNo).ToList();
//                        // 增减员中存在相同的身份证号码
//                        if (planMatchAddRecords != null && planMatchAddRecords.Count > 0 && delMatchDelRecords != null && delMatchDelRecords.Count > 0)
//                        {
//                            sameIdCardNos = planMatchAddRecords.Select(f => f.IdCardNo).Intersect(planMatchAddRecords.Select(f => f.IdCardNo)).ToList();
//                        }
//                        // 优先处理身份证号码相同的员工
//                        if (sameIdCardNos != null && sameIdCardNos.Count > 0)
//                        {
//                            foreach (var idCardNoItem in sameIdCardNos)
//                            {
//                                addRecord = planMatchAddRecords.First(f => f.IdCardNo == idCardNoItem);
//                                delRecord = delMatchDelRecords.First(f => f.IdCardNo == idCardNoItem);
//                                addRecord.AddMatchType = InsuranceMatchEnumType.身份证号匹配;
//                                addRecord.AddMatchRecordID = delRecord.ID;
//                                delRecord.DelMatchType = InsuranceMatchEnumType.身份证号匹配;
//                                delRecord.DelMatchRecordID = addRecord.ID;
//                                addOrDelRecords.Add(delRecord);
//                                addOrDelRecords.Add(addRecord);
//                            }
//                        }

//                        foreach (var matchRecordItem in planMatchAddRecords)
//                        {
//                            // 身份证号码相同的会被排除
//                            if (matchRecordItem.AddMatchType != InsuranceMatchEnumType.待匹配)
//                            {
//                                continue;
//                            }

//                            // 优先匹配 类型相同的
//                            if (delMatchDelRecords.Any(f => f.DelMatchType == InsuranceMatchEnumType.待匹配 && f.JobLevelType == matchRecordItem.JobLevelType))
//                            {
//                                delRecord = delMatchDelRecords.First(f => f.JobLevelType == matchRecordItem.JobLevelType);
//                                matchRecordItem.AddMatchType = InsuranceMatchEnumType.同类匹配;
//                                matchRecordItem.AddMatchRecordID = delRecord.ID;
//                                delRecord.DelMatchType = InsuranceMatchEnumType.同类匹配;
//                                delRecord.DelMatchRecordID = matchRecordItem.ID;
//                                addOrDelRecords.Add(delRecord);
//                                addOrDelRecords.Add(matchRecordItem);
//                            }
//                            // 然后进行无差别匹配
//                            else if (delMatchDelRecords.Any(f => f.DelMatchType == InsuranceMatchEnumType.待匹配))
//                            {
//                                delRecord = delMatchDelRecords.FirstOrDefault(f => f.DelMatchType == InsuranceMatchEnumType.待匹配);
//                                matchRecordItem.AddMatchType = InsuranceMatchEnumType.异类匹配;
//                                matchRecordItem.AddMatchRecordID = delRecord.ID;
//                                delRecord.DelMatchType = InsuranceMatchEnumType.异类匹配;
//                                delRecord.DelMatchRecordID = matchRecordItem.ID;
//                                addOrDelRecords.Add(delRecord);
//                                addOrDelRecords.Add(matchRecordItem);
//                            }
//                            else
//                            {
//                                // 未匹配到减员的增员记录 则要判断是否存在日保单
//                                matchRecordItem.AddMatchType = InsuranceMatchEnumType.无匹配项;
//                                if (dayOrderPlans.Any(f => f.IsCommon == true && f.GnteeName == matchRecordItem.PlanName))
//                                {
//                                    dayOrderPlan = dayOrderPlans.First(f => f.IsCommon == true && f.GnteeName == matchRecordItem.PlanName);
//                                    matchRecordItem.InsuranceOrderPlanID = dayOrderPlan.ID;
//                                    matchRecordItem.InsuranceOrderID = dayOrderPlan.InsuranceOrderID;
//                                    matchRecordItem.InsuranceStartDate = dayOrderPlan.StartDate;
//                                    matchRecordItem.InsuranceEndDate = dayOrderPlan.EndDate;
//                                    matchRecordItem.OrderNo = dayOrderPlan.OrderNo;
//                                    matchRecordItem.InsuranceNo = dayOrderPlan.InsuranceNo;
//                                }
//                                // 判断是否存在日保单，存在则推送日保单，不存在则继续推送月保单
//                                addOrDelRecords.Add(matchRecordItem);
//                            }

//                        }
//                    }
//                    else
//                    {
//                        // 方案增员记录
//                        var planMatchAddRecords = addRecords.Where(f =>
//                          f.IsCommon == false// 指定推送
//                          && f.PlanName == monthPlanItem.GnteeName// 方案相同
//                          && f.InsuranceSigningUnitName == monthPlanItem.SigningUnitName//  劳动合同各单位相同
//                          ).ToList();

//                        if (planMatchAddRecords == null || planMatchAddRecords.Count <= 0)
//                        {
//                            continue;
//                        }

//                        planMatchAddRecords.AsParallel().ForAll(f =>
//                        {
//                            f.InsuranceOrderPlanID = monthPlanItem.ID;
//                            f.InsuranceOrderID = monthPlanItem.InsuranceOrderID;
//                            f.InsuranceStartDate = monthPlanItem.StartDate;
//                            f.InsuranceEndDate = monthPlanItem.EndDate;
//                            f.InsuranceNo= monthPlanItem.InsuranceNo;
//                            f.OrderNo = monthPlanItem.OrderNo;
//                        });
//                        var delMatchDelRecords = delRecords.Where(f => f.InsuranceOrderID == monthPlanItem.InsuranceOrderID).ToList();
//                        // 增减员中存在相同的身份证号码
//                        if (planMatchAddRecords != null && planMatchAddRecords.Count > 0 && delMatchDelRecords != null && delMatchDelRecords.Count > 0)
//                        {
//                            sameIdCardNos = planMatchAddRecords.Select(f => f.IdCardNo).Intersect(planMatchAddRecords.Select(f => f.IdCardNo)).ToList();
//                        }
//                        // 优先处理身份证号码相同的员工
//                        if (sameIdCardNos != null && sameIdCardNos.Count > 0)
//                        {
//                            foreach (var idCardNoItem in sameIdCardNos)
//                            {
//                                addRecord = planMatchAddRecords.FirstOrDefault(f => f.IdCardNo == idCardNoItem);
//                                delRecord = delMatchDelRecords.FirstOrDefault(f => f.IdCardNo == idCardNoItem);
//                                addRecord.AddMatchType = InsuranceMatchEnumType.身份证号匹配;
//                                addRecord.AddMatchRecordID = delRecord.ID;
//                                delRecord.DelMatchType = InsuranceMatchEnumType.身份证号匹配;
//                                delRecord.DelMatchRecordID = addRecord.ID;
//                                addOrDelRecords.Add(delRecord);
//                                addOrDelRecords.Add(addRecord);
//                            }
//                        }
//                        foreach (var matchRecordItem in planMatchAddRecords)
//                        {
//                            if (matchRecordItem.AddMatchType != InsuranceMatchEnumType.待匹配)
//                            {
//                                continue;
//                            }
//                            if (delMatchDelRecords.Any(f => f.DelMatchType == InsuranceMatchEnumType.待匹配 && f.JobLevelType == matchRecordItem.JobLevelType))
//                            {
//                                delRecord = delMatchDelRecords.FirstOrDefault(f => f.JobLevelType == matchRecordItem.JobLevelType);
//                                matchRecordItem.AddMatchType = InsuranceMatchEnumType.同类匹配;
//                                matchRecordItem.AddMatchRecordID = delRecord.ID;
//                                delRecord.DelMatchType = InsuranceMatchEnumType.同类匹配;
//                                delRecord.DelMatchRecordID = matchRecordItem.ID;

//                                addOrDelRecords.Add(delRecord);
//                                addOrDelRecords.Add(matchRecordItem);
//                            }
//                            else if (delMatchDelRecords.Any(f => f.DelMatchType == InsuranceMatchEnumType.待匹配))
//                            {
//                                delRecord = delMatchDelRecords.FirstOrDefault(f => f.DelMatchType == InsuranceMatchEnumType.待匹配);
//                                matchRecordItem.AddMatchType = InsuranceMatchEnumType.异类匹配;
//                                matchRecordItem.AddMatchRecordID = delRecord.ID;
//                                delRecord.DelMatchType = InsuranceMatchEnumType.异类匹配;
//                                delRecord.DelMatchRecordID = matchRecordItem.ID;
//                                addOrDelRecords.Add(delRecord);
//                                addOrDelRecords.Add(matchRecordItem);
//                            }
//                            else
//                            {
//                                matchRecordItem.AddMatchType = InsuranceMatchEnumType.无匹配项;
//                                if (dayOrderPlans.Any(f => f.IsCommon == false && f.SigningUnitName == matchRecordItem.InsuranceSigningUnitName  && f.GnteeName == matchRecordItem.PlanName))
//                                {
//                                    dayOrderPlan = dayOrderPlans.First(f => f.IsCommon == false && f.SigningUnitName == matchRecordItem.InsuranceSigningUnitName && f.GnteeName == matchRecordItem.PlanName);
//                                    matchRecordItem.InsuranceOrderPlanID = dayOrderPlan.ID;
//                                    matchRecordItem.InsuranceOrderID = dayOrderPlan.InsuranceOrderID;
//                                    matchRecordItem.InsuranceStartDate = dayOrderPlan.StartDate;
//                                    matchRecordItem.InsuranceEndDate = dayOrderPlan.EndDate;
//                                }
//                                // 判断是否存在日保单，存在则推送日保单，不存在则继续推送月保单
//                                addOrDelRecords.Add(matchRecordItem);
//                            }
//                        }
//                    }
//                }
//                #endregion
//                #region 生成推送记录,处理匹配完成 待增减推送的员工记录

//                if (addOrDelRecords.Any())
//                {
//                    List<Live_InsurancePushExtension> insurancePushes = new List<Live_InsurancePushExtension>();
//                    Live_V_InsuranceOrderPlan orderPlan = null;
//                    var groups = addOrDelRecords.GroupBy(f => new { f.InsuranceOrderPlanID, f.SendStatus }).ToList();
                  
//                    foreach (var group in groups)
//                    {

//                        orderPlan = orderPlans.First(f => f.ID == group.Key.InsuranceOrderPlanID);

//                        List<Live_InsurancePushRecord> pushRecords = group.Select(f => new Live_InsurancePushRecord()
//                        {
//                            Custemersex = f.Sex,
//                            Custemerage = f.Age.ToString(),
//                            Custemername = f.Name,
//                            Custemernum = f.IdCardNo,
//                            Custemertype = "身份证",
//                            Guyuangongzhong = f.JobType,
//                            Ldhtdw = f.InsuranceSigningUnitName,
//                            Sjygdw = f.CustomerName,
//                            UserEntryInsuracneRecordID = f.ID,
//                            Zhiyeleibei = f.JobLevelType.Value.ToString(),
//                            Remark = string.Empty,
//                            IsAdd = f.SendStatus == InsuranceSendStatusEnumType.待增员,
//                             UserEntryID=f.UserEntryID.Value,
                           
//                        }).ToList();
//                        Live_InsurancePushExtension insurancePush = new Live_InsurancePushExtension()
//                        {

                            
//                            AddFailCount = 0,
//                            AddIndecaseId = string.Empty,
//                            AddPushTime = DateTime.Today,
//                            DelPushTime = DateTime.Today,
//                            CreateTime = DateTime.Now,
//                            AddRecordCount = 0,
//                            DelFailCount = 0,
//                            DelIndecaseId = string.Empty,
//                            DelRecordCount = 0,
//                            OrderNo = orderPlan.OrderNo,
//                            GnteeId = orderPlan.GnteeId,
//                            GnteeName = orderPlan.GnteeName,
//                            InsuranceOrderPlanID = orderPlan.ID,
//                            Status = InsurancePushStatusEnumType.待推送,
//                            PushRecords = pushRecords,
//                            AddPushStatus = InsurancePushStatusEnumType.待推送,
//                            DelPushStatus = InsurancePushStatusEnumType.待推送,
//                            AddResCode = 0,
//                            AddResMsg = string.Empty,
//                            DelResCode = 0,
//                            DelResMsg = string.Empty,
//                            ID = Guid.NewGuid()
//                        };
//                        insurancePushes.Add(insurancePush);
//                    }

                   


//                }
//                #endregion

           

//                #region 进行Api批量增减推送操作
//                var insuranceAPiPushs = await insurancePushService.List(f =>f.Status== InsurancePushStatusEnumType.待推送&& f.CreateTime >= DateTime.Today);

//                foreach (var pushItem in insuranceAPiPushs)
//                {
//                    var pushDomain = await insurancePushService.GetNav(f => f.ID == pushItem.ID);
//                    // 先减员
//                    var delList = pushDomain.PushRecords.Where(f => !f.IsAdd).ToList();
//                    if (delList != null && delList.Count > 0)
//                    {
//                        pushDomain.AddRecordCount = delList.Count();
//                        BatchDelEmployeeRequest batchDelRequest = new BatchDelEmployeeRequest()
//                        {
//                            InsureId = pushItem.OrderNo,
//                            CustomerList = delList.Select(f => new DelCustomerinfo()
//                            {
//                                Custemername = f.Custemername,
//                                Custemernum = f.Custemernum
//                            }).ToList()
//                        };
//                        // 只能模拟推送
//                        var delResp = await insuranceApiService.BatchDelEmployee(batchDelRequest);
//                        if (delResp != null)
//                        {
//                            pushDomain.DelFailCount = delResp.FailNum;
//                            pushDomain.DelPushTime = DateTime.Now;
//                            pushDomain.DelResCode = delResp.Code;
//                            pushDomain.DelResMsg = delResp.Msg;
//                            pushDomain.DelIndecaseId = delResp.IndecaseId;
//                            if (delResp.Code == 200)
//                            {

//                                pushDomain.DelPushStatus = InsurancePushStatusEnumType.已推送;

//                                foreach (var item in delList)
//                                {
//                                    item.IsPushSuc = true;
//                                }

//                                if (delResp.FailList != null && delResp.FailList.Count > 0)
//                                {
//                                    foreach (var failItem in delResp.FailList)
//                                    {
//                                        var delItem = delList.First(f => f.Custemernum == failItem.Customerinfo.Custemernum);
//                                        delItem.Remark = failItem.Msg;
//                                        delItem.IsPushSuc = false;
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                pushDomain.DelPushStatus = InsurancePushStatusEnumType.推送失败;
//                                pushDomain.DelPushTime = DateTime.Now;
//                                pushDomain.DelResCode = delResp.Code;
//                                pushDomain.DelResMsg = delResp.Msg;
//                                foreach (var item in delList)
//                                {
//                                    item.IsPushSuc = false;
//                                    item.Remark = "推送失败";
//                                }
//                            }
//                        }
//                        else
//                        {
//                            pushDomain.DelPushStatus = InsurancePushStatusEnumType.推送异常;
//                            pushDomain.DelResMsg = "推送异常请联系管理员";
//                        }
//                    }
//                    else
//                    {
//                        pushDomain.DelRecordCount = 0;
//                    }


//                  await  Task.Delay(1000);
//                    //再增员
//                    var addList = pushDomain.PushRecords.Where(f => f.IsAdd).ToList();

//                    if (addList != null && addList.Count() > 0)
//                    {
//                        var addRequest = new BatchAddEmployeeRequest()
//                        {
//                            GnteeId = pushDomain.GnteeId,
//                            GnteeName = pushDomain.GnteeName,
//                            InsureId = pushDomain.OrderNo,
//                            CustomerList = addList.Select(f => new AddCustomerinfo()
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
//                            pushDomain.AddFailCount = addResp.FailNum;
//                            pushDomain.AddPushTime = DateTime.Now;
//                            pushDomain.AddResCode = addResp.Code;
//                            pushDomain.AddResMsg = addResp.Msg ?? string.Empty;
//                            pushDomain.AddIndecaseId = addResp.IndecaseId ?? string.Empty;
//                            if (addResp.Code == 200)
//                            {
//                                pushDomain.AddPushStatus = InsurancePushStatusEnumType.已推送;
//                                foreach (var item in addList)
//                                {
//                                    item.IsPushSuc = true;
//                                }

//                                if (addResp.FailList != null && addResp.FailList.Count > 0)
//                                {
//                                    foreach (var item in addResp.FailList)
//                                    {
//                                        var addItem = addList.First(f => f.Custemernum == item.Customerinfo.Custemernum);
//                                        addItem.IsPushSuc = false;
//                                        addItem.Remark = item.Msg;
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                pushDomain.AddPushStatus = InsurancePushStatusEnumType.推送失败;
//                                pushDomain.AddPushTime = DateTime.Now;
//                                pushDomain.AddResCode = addResp.Code;
//                                pushDomain.AddResMsg = addResp.Msg;
//                                foreach (var item in addList)
//                                {
//                                    item.IsPushSuc = false;
//                                    item.Remark = "推送失败";
//                                }

//                            }

//                        }
//                        else
//                        {
//                            pushDomain.AddPushStatus = InsurancePushStatusEnumType.推送异常;
//                            pushDomain.AddResMsg = "推送异常请联系管理员";
//                        }
//                    }
//                    pushDomain.Status = InsurancePushStatusEnumType.已推送;
//                    await insurancePushService.UpdateNav(pushDomain);
//                }
//                #endregion

//                #region 修改入职记录参保状态

//                var pushDomains = await insurancePushService.ListNav(f => f.Status == InsurancePushStatusEnumType.已推送 && f.CreateTime >= DateTime.Today);
//                List<Guid> batchNoList = new List<Guid>();
//                foreach (var pushDomain in pushDomains)
//                {
//                    if (pushDomain.PushRecords == null || pushDomain.PushRecords.Count <= 0)
//                    {
//                        continue;
//                    }
//                    var userEntryInsuracneRecords = await userEntryInsuracneRecordService.List(f => pushDomain.PushRecords.Select(f => f.UserEntryInsuracneRecordID).Contains(f.ID));
//                    batchNoList.AddRange(userEntryInsuracneRecords.Select(f => f.BatchNo.Value).ToList());
//                    foreach (var item in pushDomain.PushRecords)
//                    {
//                        if (userEntryInsuracneRecords.Any(f => f.ID == item.UserEntryInsuracneRecordID))
//                        {
//                            var userEntryInsuracneRecord = userEntryInsuracneRecords.First(f => f.ID == item.UserEntryInsuracneRecordID);
//                            if (item.IsPushSuc)
//                            {
//                                if (userEntryInsuracneRecord.SendStatus == InsuranceSendStatusEnumType.待增员 )
//                                {
//                                    userEntryInsuracneRecord.SendStatus = InsuranceSendStatusEnumType.已增员;
//                                    userEntryInsuracneRecord.StartDate = DateTime.Today.AddDays(1);
//                                }
//                                else if (userEntryInsuracneRecord.SendStatus == InsuranceSendStatusEnumType.待减员 )
//                                {
//                                    userEntryInsuracneRecord.SendStatus = InsuranceSendStatusEnumType.已减员;
//                                    userEntryInsuracneRecord.EndDate = DateTime.Today;
//                                }
                                
//                            }
//                            else
//                            {
//                                if (userEntryInsuracneRecord.SendStatus == InsuranceSendStatusEnumType.待增员)
//                                {
//                                    userEntryInsuracneRecord.SendStatus = InsuranceSendStatusEnumType.增员异常;
//                                }
//                                else if (userEntryInsuracneRecord.SendStatus == InsuranceSendStatusEnumType.待减员)
//                                {
//                                    userEntryInsuracneRecord.SendStatus = InsuranceSendStatusEnumType.减员异常;
//                                }
//                            }
//                        }

//                    }
//                    // 更新参保状态
//                    await userEntryInsuracneRecordService.Update(userEntryInsuracneRecords);
//                }
//                #endregion

//                #region 根据批次号获取所有的关联记录
//                var records = await pushRecordService.ListView(f => f.CreateTime >= today);
//                var userEntryIds = records.Select(f => f.UserEntryID).Distinct().ToList();
//                foreach (var userEntryID in userEntryIds)
//                {
//                    InsuranceStatusEventData eventData = new InsuranceStatusEventData(userEntryID.Value);
//                    publisher.Publish(eventData);
//                }
//                #endregion
//            }
//        }
//    }
//}
