//using LZ.Core.EventData;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Model;
//using LZ.Model.Enums;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Senparc.CO2NET.Extensions;
//using SqlSugar.IOC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Customer.Service.EventHandler.InsuranceHandler
//{
//    public class GenerateUserEntryInsuranceRecordEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<GenerateUserEntryInsuranceRecordEventData>
//    {
//        public async Task HandleAsync(GenerateUserEntryInsuranceRecordEventData @event)
//        {

//            using (var scope = scopeFactory.CreateScope())
//            {

//                List<InsuranceSendStatusEnumType> insuranceSendStatusEnumTypes = new List<InsuranceSendStatusEnumType>() {
//                  InsuranceSendStatusEnumType.已增员,
//                   InsuranceSendStatusEnumType.待减员,
//                    InsuranceSendStatusEnumType.待增员
//                };

//                Guid batchNo=Guid.NewGuid();

//                var sp = scope.ServiceProvider;
//                var logger = scope.ServiceProvider.GetService<ILogger<GenerateUserEntryInsuranceRecordEventHandler>>();
//                var otherService = scope.ServiceProvider.GetService<ICustomerOtherService>();
//                var insuranceCustomerConfigService = scope.ServiceProvider.GetService<IInsuranceCustomerConfigService>();
//                var userEntryInsuracneRecordService = scope.ServiceProvider.GetService<IUserEntryInsuracneRecordService>();
//                var orderService = scope.ServiceProvider.GetService<IInsuranceOrderService>();
//                var today = DateTime.Today;
//                var insuranceDate = today.AddDays(1);
//                /// 获取员工信息
//                var userEntryResp = await otherService.GetUserEntryRecord(@event.UserEntryID);
//                var userEntry = userEntryResp.Data;
//                // 获取客户单位商保配置信息
//                var configList = await insuranceCustomerConfigService.ListView(f =>f.IsEnabled&& f.CustomerGID == userEntry.CustomerGID, new Model.PagedModel.PagedModelBase() { Page = 0 });
//                // 获取当前员工的参保记录
//                List<Live_UserEntryInsuracneRecord> updateRecords = new List<Live_UserEntryInsuracneRecord>();
//                // 获取在保记录
//                var recordHistoryList = await userEntryInsuracneRecordService.List(f =>  insuranceSendStatusEnumTypes.Contains(f.SendStatus.Value) && f.UserEntryID == userEntry.ID);

//                List<Live_UserEntryInsuracneRecord> records = new List<Live_UserEntryInsuracneRecord>();

//                Live_UserEntryInsuracneRecord record = null;
//                Live_V_InsuranceOrderPlan orderPlan = null;
//                foreach (var item in configList)
//                {
//                    record = new Live_UserEntryInsuracneRecord()
//                    {
//                        Name = userEntry.Name,
//                        IdCardNo = userEntry.CardID,
//                        CustomerName = userEntry.CustomerName,
//                        Age = userEntry.Age,
//                        StartDate = insuranceDate,
//                        InsuranceCompany = item.InsuranceCompany,
//                        CreateTime = DateTime.Now,
//                        InsuranceCustomerConfigID = item.ID,
//                        UserEntryID = userEntry.ID,
//                        SendStatus = @event.IsAdd ? InsuranceSendStatusEnumType.待增员 : InsuranceSendStatusEnumType.待减员,
//                        InsuranceSigningUnitName = item.SigningUnitName,
//                        InsuranceEndDate = null,
//                        EndDate = null,
//                        InsuranceStartDate = null,
//                        InsuranceOrderID = null,
//                        JobType = userEntry.WorkName,
//                        DelMatchType = InsuranceMatchEnumType.待匹配,
//                        AddMatchType = InsuranceMatchEnumType.待匹配,
//                        AddMatchRecordID = null,
//                        DelMatchRecordID = null,
//                        IsCommon = item.IsCommon,
//                        Sex = userEntry.Sex,
//                        BatchNo = batchNo,
//                        IsMain = item.IsMain,
//                        IsAdd = @event.IsAdd,
//                        IsComplete=false,
//                    };
//                    if (userEntry.WorkClass == "3类")
//                    {
//                        record.JobLevelType = 3;
//                        record.PlanName = item.LevelFourPlan;
//                    }
//                    else if (userEntry.WorkClass == "4类")
//                    {
//                        record.JobLevelType = 4;
//                        record.PlanName = item.LevelFourPlan;
//                    }
//                    else if (userEntry.WorkClass == "5类")
//                    {
//                        record.JobLevelType = 5;
//                        record.PlanName = item.LevelFivePlan;
//                    }
//                    else
//                    {
//                        record.Remark = "未知的工种类别";

//                        record.SendStatus = InsuranceSendStatusEnumType.作废;
//                        record.JobLevelType = 5;
//                        record.PlanName = item.LevelFivePlan;
//                    }

//                    if (record.IsAdd)
//                    {
//                        if (recordHistoryList.Any(f => f.InsuranceCompany == record.InsuranceCompany&& f.SendStatus == InsuranceSendStatusEnumType.待增员))
//                        {
//                            var existRecord = recordHistoryList.First(f => f.InsuranceCompany == record.InsuranceCompany && f.SendStatus == InsuranceSendStatusEnumType.待增员);
//                            existRecord.SendStatus = InsuranceSendStatusEnumType.作废;
//                            updateRecords.Add(existRecord);
//                            records.Add(record);
//                        }
//                        else if (recordHistoryList.Any(f => f.InsuranceCompany == record.InsuranceCompany && f.SendStatus == InsuranceSendStatusEnumType.已增员))
//                        {
//                            var existRecord = recordHistoryList.First(f => f.InsuranceCompany == record.InsuranceCompany && f.SendStatus == InsuranceSendStatusEnumType.已增员);
//                            existRecord.SendStatus = InsuranceSendStatusEnumType.待减员;
//                            updateRecords.Add(existRecord);
//                            records.Add(record);
//                        }
//                        else
//                        {
//                            records.Add(record);
//                        }
//                    }
//                    else {
//                        if (recordHistoryList.Any(f => f.InsuranceCompany == record.InsuranceCompany && f.SendStatus == InsuranceSendStatusEnumType.已增员))
//                        {
//                            var existRecord = recordHistoryList.First(f => f.InsuranceCompany == record.InsuranceCompany && f.SendStatus == InsuranceSendStatusEnumType.已增员);
//                            existRecord.SendStatus = InsuranceSendStatusEnumType.待减员;
//                            updateRecords.Add(existRecord);
//                        }
//                        else if(recordHistoryList.Any(f => f.InsuranceCompany == record.InsuranceCompany && f.SendStatus == InsuranceSendStatusEnumType.待增员))
//                        {
//                            var existRecord = recordHistoryList.First(f => f.InsuranceCompany == record.InsuranceCompany && f.SendStatus == InsuranceSendStatusEnumType.待增员);
//                            existRecord.Remark = "待增员记录存在，收到待减员";
//                            existRecord.SendStatus = InsuranceSendStatusEnumType.取消推送;
//                            updateRecords.Add(existRecord);
//                            record.Remark = "待增员记录存在，收到待减员";
//                            record.SendStatus = InsuranceSendStatusEnumType.取消推送;
//                            records.Add(record);
//                        } else
//                        {
//                            record.Remark = "未找到对应的增员记录，请人工核对";
//                            record.SendStatus = InsuranceSendStatusEnumType.减员异常;
//                            records.Add(record);
//                        }
//                    }
//                }
//                DbScoped.SugarScope.BeginTran();
//                if (updateRecords.Any())
//                {
//                    await userEntryInsuracneRecordService.Update(updateRecords);
//                }
//                if (records.Any())
//                {
//                    await userEntryInsuracneRecordService.Add(records);
//                }
//                DbScoped.SugarScope.CommitTran();
//            }
//        }
//    }
//}
