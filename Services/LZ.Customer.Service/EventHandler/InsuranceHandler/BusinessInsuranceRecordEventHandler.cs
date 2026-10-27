//using LZ.Core.EventData;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Model.Enums;
//using LZ.Model;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using SqlSugar.IOC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LZ.Common;
//using System.Security.Policy;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

//namespace LZ.Customer.Service.EventHandler.InsuranceHandler
//{


//    public class BusinessInsuranceRecordEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<GenerateUserEntryInsuranceRecordEventData>
//    {
//        public async Task HandleAsync(GenerateUserEntryInsuranceRecordEventData @event)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var nextDay = DateTime.Today.AddDays(1);

//                List<InsuranceSendStatusEnumType> insuranceSendStatusEnumTypes = new List<InsuranceSendStatusEnumType>() {
//                  InsuranceSendStatusEnumType.已增员,
//                   InsuranceSendStatusEnumType.待减员,
//                    InsuranceSendStatusEnumType.待增员
//                };



//                var sp = scope.ServiceProvider;
//                var publisher = scope.ServiceProvider.GetService<IEventPublisher>();
//                var logger = scope.ServiceProvider.GetService<ILogger<BusinessInsuranceRecordEventHandler>>();
//                var otherService = scope.ServiceProvider.GetService<ICustomerOtherService>();
//                var insuranceCustomerConfigService = scope.ServiceProvider.GetService<IInsuranceCustomerConfigService>();
//                var businessInsuranceRecordService = scope.ServiceProvider.GetService<IBusinessInsuranceRecordService>();
//                var orderService = scope.ServiceProvider.GetService<IInsuranceOrderService>();
//                var today = DateTime.Today;
//                var insuranceDate = today.AddDays(1);
//                /// 获取员工信息
//                var userEntryResp = await otherService.GetUserEntryRecord(@event.UserEntryID);
//                var userEntry = userEntryResp.Data;
//                // 获取客户单位商保配置信息
//                var configList = await insuranceCustomerConfigService.ListView(f => f.IsEnabled && f.CustomerGID == userEntry.CustomerGID, new Model.PagedModel.PagedModelBase() { Page = 0 });

//                var ageAndSex = IdCardHelper.GetAgeAndGender(userEntry.CardID);
//                // 获取当前员工的参保记录
//                List<Live_UserEntryInsuracneRecord> updateRecords = new List<Live_UserEntryInsuracneRecord>();

//                List<Live_UserEntryInsuracneRecord> records = new List<Live_UserEntryInsuracneRecord>();
//                Live_UserEntryInsuracneRecord record = null;
//                List<Live_V_InsuranceOrderPlan> plans = null;
//                plans = await orderService.GetOrderPlanViewList(f => f.StartDate<=nextDay && nextDay <= f.EndDate && f.OrderType == (int)InsuranceOrderEnumType.月保单);
//                var endDate = plans.First().EndDate;


//                Live_V_InsuranceOrderPlan orderPlan = null;







//                Live_BusinessInsuranceRecord businessInsuranceRecord = null;
//                List<Live_BusinessInsuranceRecord> updateExistList = new List<Live_BusinessInsuranceRecord>(); 
//                List<Live_BusinessInsuranceRecord> addList = new List<Live_BusinessInsuranceRecord>();

//                UserInsuranceInsuranceStatusUpdateEventData changeStatus =null;
               

//                foreach (var item in configList)
//                {
//                    if (item.IsMain)
//                    {
//                        changeStatus = new UserInsuranceInsuranceStatusUpdateEventData()
//                        {
//                            UserEntryID = @event.UserEntryID,
//                        };
//                    }

//                    businessInsuranceRecord = new Live_BusinessInsuranceRecord()
//                    {
//                        ID = Guid.NewGuid(),
//                        AddMatchRecordID = Guid.Empty,
//                        DelMatchRecordID = Guid.Empty,
//                        UserID = userEntry.UserID.Value,
//                        UserEntryID = userEntry.ID,
//                        DelTime = DateTime.MinValue,
//                        AddTime = DateTime.MinValue,
//                        Age = ageAndSex.Age,
//                        Sex = ageAndSex.Gender,
//                        CreateTime = DateTime.Now,
//                        CustomerGID = userEntry.CustomerGID.Value,
//                        CustomerName = userEntry.CustomerName,
//                        EntryDate = userEntry.EntryTime.Value.Date,
//                        LeaveDate = DateTime.MinValue,
//                        IdCardNo = userEntry.CardID,
//                        InsuranceCompany = item.InsuranceCompany,
//                        InsuranceNo = string.Empty,
//                        InsuranceSigningUnitID = item.InsuranceSigningUnitID.Value,
//                        InsuranceSigningUnitName = item.SigningUnitName,
//                        IsExpired = false,
//                        IsMain = item.IsMain,
//                        Name = userEntry.Name,
//                        JobLevelType = string.Empty,
//                        JobType = string.Empty,
//                        OrderNo = string.Empty,
//                        OrderType = InsuranceOrderEnumType.月保单,
//                        PlanName = string.Empty,
//                        StartDate = nextDay,
//                        EndDate = endDate,
//                        Status = InsuranceSendStatusEnumType.待增员,
//                        AddIndecaseId = string.Empty,
//                        DelIndecaseId = string.Empty,
//                        Remark = string.Empty,
//                        IsCommon = item.IsCommon
//                    };

//                    string planName = string.Empty;

//                    if (userEntry.WorkClass == "3类")
//                    {
//                        businessInsuranceRecord.JobLevelType = $"{3}";
//                        businessInsuranceRecord.PlanName = item.LevelThreePlan;

//                    }
//                    else if (userEntry.WorkClass == "4类")
//                    {
//                        businessInsuranceRecord.JobLevelType = $"{4}";
//                        businessInsuranceRecord.PlanName = item.LevelFourPlan;
//                    }
//                    else if (userEntry.WorkClass == "5类")
//                    {
//                        businessInsuranceRecord.JobLevelType = $"{5}";
//                        businessInsuranceRecord.PlanName = item.LevelFivePlan;
//                    }
//                    else
//                    {
//                        businessInsuranceRecord.JobLevelType = $"{5}";
//                        businessInsuranceRecord.PlanName = item.LevelFivePlan;
//                    }
//                    if (item.IsCommon)
//                    {
//                        // 获取当前有效保单
//                        orderPlan = plans.FirstOrDefault(f => f.InsuranceCompany == item.InsuranceCompany && f.IsCommon == true && f.GnteeName == businessInsuranceRecord.PlanName);
//                    }
//                    else
//                    {
//                        orderPlan = plans.FirstOrDefault(f => f.InsuranceCompany == item.InsuranceCompany && f.IsCommon == false && f.SigningUnitName == item.SigningUnitName && f.GnteeName == businessInsuranceRecord.PlanName);
//                    }

//                    if (orderPlan != null)
//                    {
//                        businessInsuranceRecord.InsuranceNo = orderPlan.InsuranceNo;
//                        businessInsuranceRecord.OrderNo = orderPlan.OrderNo;
//                        businessInsuranceRecord.EndDate = orderPlan.EndDate;
//                        businessInsuranceRecord.PlanNameID = orderPlan.GnteeId;
//                    }

//                    if (@event.IsAdd)
//                    {

                       

//                        var existList = await businessInsuranceRecordService.List(f =>
//                         f.Name == businessInsuranceRecord.Name 
//                        && f.IdCardNo == businessInsuranceRecord.IdCardNo 
//                        && f.InsuranceCompany == item.InsuranceCompany 
//                        && insuranceSendStatusEnumTypes.Contains(f.Status) 
//                        && orderPlan.StartDate <= f.StartDate 
//                        && orderPlan.EndDate >= f.EndDate);


//                        businessInsuranceRecord.Status = InsuranceSendStatusEnumType.待增员;
//                        if (existList.Any(f =>
//                       f.InsuranceSigningUnitName == businessInsuranceRecord.InsuranceSigningUnitName
//                       && f.CustomerName == businessInsuranceRecord.CustomerName
//                            && f.JobLevelType == businessInsuranceRecord.JobLevelType
//                                 && f.JobType == businessInsuranceRecord.JobType
//                                 && f.PlanName == businessInsuranceRecord.PlanName
//                       ))
//                        {

//                            var samePlanRecords = existList.Where(f =>
//                       f.InsuranceSigningUnitName == businessInsuranceRecord.InsuranceSigningUnitName
//                       && f.CustomerName == businessInsuranceRecord.CustomerName
//                            && f.JobLevelType == businessInsuranceRecord.JobLevelType
//                                 && f.JobType == businessInsuranceRecord.JobType
//                                 && f.PlanName == businessInsuranceRecord.PlanName

//                       ).ToList();
//                            // 存在待增员记录
//                            if (samePlanRecords.Any(f => f.Status == InsuranceSendStatusEnumType.待增员 ))
//                            {

//                                if (changeStatus!=null)
//                                {
//                                    changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                                }
                               


//                                // 存在记录保留，无需处理
//                                //新增记录作废
//                                businessInsuranceRecord.Status = InsuranceSendStatusEnumType.作废;
//                                addList.Add(businessInsuranceRecord);
//                            }
//                            if (samePlanRecords.Any(f =>  f.Status == InsuranceSendStatusEnumType.已增员))
//                            {
//                                if (changeStatus != null)
//                                {
//                                    changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                                }
//                                // 存在记录保留，无需处理
//                                //新增记录作废
//                                businessInsuranceRecord.Status = InsuranceSendStatusEnumType.作废;
//                                addList.Add(businessInsuranceRecord);
//                            }
//                            else if (samePlanRecords.Any(f => f.Status == InsuranceSendStatusEnumType.待减员))
//                            {

//                                if (changeStatus != null)
//                                {
//                                    changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                                }
//                                // 存在记录改成已参保
//                                var sameRecord = samePlanRecords.First(f => f.Status == InsuranceSendStatusEnumType.待减员);
//                                sameRecord.Status = InsuranceSendStatusEnumType.已增员;
//                                updateExistList.Add(sameRecord);
//                                //新增记录作废
//                                businessInsuranceRecord.Status = InsuranceSendStatusEnumType.作废;
//                                addList.Add(businessInsuranceRecord);
//                            }

//                        }
//                        else if (existList.Any(f => f.Status == InsuranceSendStatusEnumType.待增员))
//                        {

//                            var existRecord = existList.First(f => f.Status == InsuranceSendStatusEnumType.待增员);
//                            //存在记录作废
//                            existRecord.Status = InsuranceSendStatusEnumType.作废;
//                            updateExistList.Add(existRecord);
//                            // 新纪录新增
//                            if (changeStatus != null)
//                            {
//                                changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                            }
//                            addList.Add(businessInsuranceRecord);
//                        }
//                        else if (existList.Any(f => f.Status == InsuranceSendStatusEnumType.已增员))
//                        {

//                            var existRecord = existList.First(f => f.Status == InsuranceSendStatusEnumType.已增员);
//                            //存在记录作废
//                            existRecord.Status = InsuranceSendStatusEnumType.待减员;
//                            updateExistList.Add(existRecord);
//                            // 新纪录新增
//                            if (changeStatus != null)
//                            {
//                                changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                            }
//                            addList.Add(businessInsuranceRecord);
//                        }
//                        else if (existList.Any(f => f.Status == InsuranceSendStatusEnumType.待减员))
//                        {

//                            //存在记录无需处理
//                            // 新纪录新增
//                            if (changeStatus != null)
//                            {
//                                changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                            }
//                            addList.Add(businessInsuranceRecord);
//                        }
//                        else
//                        {
//                            if (changeStatus != null)
//                            {
//                                changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                            }
//                            addList.Add(businessInsuranceRecord);
//                        }
//                    }
//                    else
//                    {

//                        var existList = await businessInsuranceRecordService.List(f =>f.CustomerName==item.CustomerName&&f.InsuranceSigningUnitName==item.SigningUnitName&& f.Name == businessInsuranceRecord.Name && f.IdCardNo == businessInsuranceRecord.IdCardNo && f.InsuranceCompany == item.InsuranceCompany && insuranceSendStatusEnumTypes.Contains(f.Status) &&f.StartDate<=nextDay&&f.EndDate>=today);


//                        // 减员则找到员工的有效的已增员记录减员处理

//                        if (existList.Any(f => f.Status == InsuranceSendStatusEnumType.已增员))
//                        {
//                            var existRecords = existList.Where(f => f.Status == InsuranceSendStatusEnumType.已增员).ToList();
//                            foreach (var existRecord in existRecords)
//                            {
//                                existRecord.Status = InsuranceSendStatusEnumType.待减员;
//                                updateExistList.Add(existRecord);
//                            }
//                            if (changeStatus != null)
//                            {
//                                changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.待减员;
//                            }
//                        }
//                        else if (existList.Any(f => f.Status == InsuranceSendStatusEnumType.待增员))
//                        {
//                            var existRecords = existList.Where(f => f.Status == InsuranceSendStatusEnumType.待增员).ToList();
//                            foreach (var existRecord in existRecords)
//                            {
//                                existRecord.Status = InsuranceSendStatusEnumType.作废;
//                                updateExistList.Add(existRecord);
//                            }
//                            if (changeStatus != null)
//                            {
//                                changeStatus.InsuranceStatus = UserEntryInsuranceStatusEnumType.未参保;
//                            }
//                        }
//                    }
//                }
//                DbScoped.SugarScope.BeginTran();
//                if (updateExistList.Any())
//                {
//                    await businessInsuranceRecordService.Update(updateExistList);
//                }
//                if (addList.Any())
//                {
//                    await businessInsuranceRecordService.Add(addList);
//                }
//                DbScoped.SugarScope.CommitTran();
//                // 修改员工参保状态
//                if (changeStatus!=null)
//                {
//                    publisher.Publish(changeStatus);
//                }
//            }
//        }
//    }
//}
