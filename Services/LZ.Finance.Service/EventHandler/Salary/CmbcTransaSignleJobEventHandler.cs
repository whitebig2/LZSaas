//using LZ.Core.EventData;
//using LZ.Core.EventData.FxqEventData;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Finance.Service.Interface;
//using LZ.Finance.Service.Services;
//using LZ.Model.Exceptions;
//using LZ.Model;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Senparc.CO2NET.Extensions;
//using Microsoft.Extensions.Logging;
//using LZ.Core.Interface;
//using LZ.Core.Services;
//using LZ.Model.Enums;
//using LZ.Core.EventData.Salary;
//using SqlSugar.IOC;
//using NPOI.SS.Formula.Functions;

//namespace LZ.Finance.Service.EventHandler
//{
//    public class CmbcTransaSignleJobEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<CmbcTransaSignleJobEventData>
//    {
//        public async Task HandleAsync(CmbcTransaSignleJobEventData @event)
//        {


//            // 员工工资单笔发放处理
//            //using (var scope = scopeFactory.CreateScope())
//            //{
//            //    var logger = scope.ServiceProvider.GetRequiredService<ILogger<CmbcTransaSignleJobEventHandler>>();
//            //    var singleTransactionService = scope.ServiceProvider.GetRequiredService<ITransSingleRecordService>();
//            //    var otherService = scope.ServiceProvider.GetRequiredService<IFinanceOtherService>();
//            //    var transSingleRecords = await singleTransactionService.List(f => f.Status == CmbcStatusEnumType.待确认);
//            //    foreach (var item in transSingleRecords)
//            //    {
//            //        try
//            //        {
//            //            item.Remark = string.Empty;
//            //            item.ConfirmTimes++;
//            //            var resp = await otherService.SingleTransactionDetail(item.BusinessID);
//            //            if (resp == null)
//            //            {

//            //                item.Remark = "接口调用异常";
//            //            }
//            //            else
//            //            {

//            //                if (resp.Code == 200)
//            //                {
//            //                    item.TransStatus = resp.Result.transStatus;
//            //                    item.TransStatusName = resp.Result.transStatusDesc;
//            //                    if (resp.Result.transStatus == "02" || resp.Result.transStatus == "03")
//            //                    {
//            //                        item.Status = CmbcStatusEnumType.已确认;

//            //                    }
//            //                }
//            //                else
//            //                {
//            //                    item.Remark = resp.Message;
//            //                }
//            //            }
//            //            // 查询次数
//            //            if (item.ConfirmTimes >= 30)
//            //            {
//            //                item.Status = CmbcStatusEnumType.确认异常;
//            //                item.Remark = "交易确认异常，请人工确认";
//            //            }
//            //            await singleTransactionService.Update(item);
//            //        }
//            //        catch (Exception ex)
//            //        {
//            //            item.Remark = $"异常信息：{ex.Message}";
//            //            await singleTransactionService.Update(item);
//            //        }
//            //    }
//            //}



//            // 单笔交易状态查询
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CmbcTransaSignleJobEventHandler>>();
//                var singleTransactionService = scope.ServiceProvider.GetRequiredService<ITransSingleRecordService>();
//                var otherService = scope.ServiceProvider.GetRequiredService<IFinanceOtherService>();
//                var failRecordService = scope.ServiceProvider.GetRequiredService<ISalaryFailRecordService>();
//                var salaryRecordService = scope.ServiceProvider.GetRequiredService<ISalaryRecordService>();
//                var applyRecordService = scope.ServiceProvider.GetRequiredService<IPrepaidApplyRecordService>();
//                var transSingleRecords = await singleTransactionService.List(f => f.Status == CmbcStatusEnumType.待确认);
//                foreach (var item in transSingleRecords)
//                {
//                    try
//                    {
//                        item.Remark = string.Empty;
//                        item.ConfirmTimes++;
//                        var resp = await otherService.SingleTransactionDetail(item.BusinessID);
//                        if (resp == null)
//                        {
//                            item.Remark = "接口调用异常";
//                        }
//                        else
//                        {
//                            if (resp.Code == 200)
//                            {
//                                item.TransStatus = resp.Result.transStatus;
//                                item.TransStatusName = resp.Result.transStatusDesc;
//                                if (resp.Result.transStatus == "02")
//                                {

//                                    if (item.BusinessType == CmbcTransBusinessTypeEnumType.企业工资)
//                                    {
//                                        var salaryRecord = await salaryRecordService.Get(f => f.ID == item.BusinessID);
//                                        salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;

//                                      await  salaryRecordService.Update(salaryRecord);
//                                    }
//                                    else if (item.BusinessType == CmbcTransBusinessTypeEnumType.预发工资)
//                                    {

//                                        var applyRecord = await applyRecordService.Get(f => f.ID == item.BusinessID);

//                                        applyRecord.Status= PrepaidRecordStatusEnumType.已发放;

//                                        await applyRecordService.Update(applyRecord);
//                                    }


//                                    item.Status = CmbcStatusEnumType.已确认;

//                                }
//                                else if (resp.Result.transStatus == "03")
//                                {

//                                    if (item.BusinessType == CmbcTransBusinessTypeEnumType.企业工资)
//                                    {
//                                        var salaryRecord = await salaryRecordService.Get(f => f.ID == item.BusinessID);
//                                        salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;


//                                        Live_SalaryFailRecord failRecord = new Live_SalaryFailRecord()
//                                        {
//                                            Bank = salaryRecord.Bank,
//                                            BankNo = salaryRecord.BankNo,
//                                            BankUserName = salaryRecord.BankUserName,
//                                            CreateTime = DateTime.Now,
//                                            Description = "单笔代发发放失败，请转现金发放",
//                                            Remark = salaryRecord.Remark,
//                                            SalaryCustomerID = salaryRecord.SalaryCustomerID,
//                                            Status = SalaryFailRecordStatusEnumType.待处理,
//                                            SalaryRecordID = salaryRecord.ID,
//                                        };

//                                          base.Context.BeginTran();
//                                        await failRecordService.Add(failRecord);
//                                        await salaryRecordService.Update(salaryRecord);
//                                          base.Context.CommitTran();
//                                    }
//                                    else if (item.BusinessType == CmbcTransBusinessTypeEnumType.预发工资)
//                                    {
//                                        var applyRecord = await applyRecordService.Get(f => f.ID == item.BusinessID);
//                                        applyRecord.Status = PrepaidRecordStatusEnumType.发放失败;
//                                        await applyRecordService.Update(applyRecord);
//                                    }

//                                }
//                            }
//                            else
//                            {
//                                item.Remark = resp.Message;
//                            }
//                        }
//                        // 查询次数
//                        if (item.ConfirmTimes >= 30)
//                        {
//                            item.Status = CmbcStatusEnumType.确认异常;
//                            item.Remark = "交易确认异常，请人工确认";
//                        }
//                        await singleTransactionService.Update(item);
//                    }
//                    catch (Exception ex)
//                    {
//                        item.Remark = $"异常信息：{ex.Message}";
//                        await singleTransactionService.Update(item);
//                    }
//                }
//            }


//            // 工资单笔交易推送
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CmbcTransaBatchJobEventHandler>>();
//                var transSingleRecordService = scope.ServiceProvider.GetRequiredService<ITransSingleRecordService>();
//                var otherService = scope.ServiceProvider.GetRequiredService<IFinanceOtherService>();
//                var salarydRecordService = scope.ServiceProvider.GetRequiredService<ISalarydRecordService>();


//                var salaryRecords = await salarydRecordService.ListView(f =>f.IsCashSend==false&& f.IsSingleSend == true && f.IsSinglePushSuc == false && f.Status == SalaryRecordSendStatusEnumType.待发放);
//                if (salaryRecords != null && salaryRecords.Count > 0)
//                {
//                    foreach (var record in salaryRecords)
//                    {
//                        var salaryRecord = await salarydRecordService.Get(f => f.ID == record.ID);
//                        try
//                        {
//                            // 单笔交易推送到幸福职
//                            var resp = await otherService.SingleTransaction(new SingleTransactionRequest()
//                            {
//                                Bank = record.Bank,
//                                BankNo = record.BankNo,
//                                BankUserName = record.BankUserName,
//                                BusinessID = record.ID,
//                                CustomerName = record.CustomerName,
//                                SigningUnitName = record.SigningUnitCompanyName,
//                                IdCardNo = record.IdCardNo,
//                                IsPrepaidSalary = false,
//                                Name = record.Name,
//                                TransAmount = record.ActualSalarySendCardAmount,
//                            });
//                            if (resp == null)
//                            {
//                                salaryRecord.TransDescription = $"单笔代发幸福职接口调用失败,请人工核对";
//                                salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
//                            }
//                            else
//                            {
//                                if (resp.Result)
//                                {
//                                    salaryRecord.Status = SalaryRecordSendStatusEnumType.发放中;
//                                    salaryRecord.IsSinglePushSuc = true;
//                                    Live_TransSingleRecord transSingleRecord = new Live_TransSingleRecord()
//                                    {
//                                        Bank = salaryRecord.Bank,
//                                        BankNo = salaryRecord.BankNo,
//                                        BankUserName = salaryRecord.BankUserName,
//                                        BusinessID = salaryRecord.ID,
//                                        CustomerName = salaryRecord.CustomerName,
//                                        BusinessType = CmbcTransBusinessTypeEnumType.企业工资,
//                                        ConfirmTimes = 0,
//                                        CreateTime = DateTime.Now,
//                                        IdCardNo = salaryRecord.IdCardNo,
//                                        IsPrepaid = false,
//                                        Name = salaryRecord.Name,
//                                        Nonce = salaryRecord.ID,
//                                        Remark = string.Empty,
//                                        SigningUnitName = record.SigningUnitCompanyName,
//                                        Status = CmbcStatusEnumType.待确认,
//                                        TransAmount = record.ActualSalarySendCardAmount,
//                                        TransStatus = string.Empty,
//                                        TransStatusName = string.Empty,
//                                        TransTime = null
//                                    };

//                                      base.Context.BeginTran();
//                                    await salarydRecordService.Update(salaryRecord);
//                                    await transSingleRecordService.Add(transSingleRecord);
//                                      base.Context.CommitTran();
//                                }
//                                else
//                                {
//                                    salaryRecord.TransDescription = $"单笔代发幸福职接口调用失败,响应信息：{resp.Message}";
//                                    salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
//                                    await salarydRecordService.Update(salaryRecord);
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            salaryRecord.TransDescription = $"单笔代发幸福职接口调用异常,异常信息：{ex.Message}";
//                            salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;

//                            await salarydRecordService.Update(salaryRecord);
//                        }
//                    }
//                }



//            }







//        }
//    }
//}
