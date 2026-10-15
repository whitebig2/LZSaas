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
//using LZ.Model.ModelExtensions;
//using NPOI.HSSF.Record;
//using SqlSugar;

//namespace LZ.Finance.Service.EventHandler
//{


//    /// <summary>
//    /// 客户工资 批量代发结果查询
//    /// </summary>
//    /// <param name="scopeFactory"></param>
//    public class CmbcTransaBatchJobEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<CmbcTransaBatchJobEventData>
//    {
//        public async Task HandleAsync(CmbcTransaBatchJobEventData @event)
//        {
//            // 批量发放状态查询
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CmbcTransaBatchJobEventHandler>>();
//                var salaryCustomerService = scope.ServiceProvider.GetRequiredService<ISalaryCustomerService>();
//                var transBatchService = scope.ServiceProvider.GetRequiredService<ITransBatchService>();
//                var otherService = scope.ServiceProvider.GetRequiredService<IFinanceOtherService>();
//                var  transRecords = await transBatchService.ListNav(f => f.Status == CmbcStatusEnumType.待确认);
//                foreach (var item in transRecords)
//                {
//                    try
//                    {
//                        item.Remark = string.Empty;

//                        item.ConfirmTimes++;
//                        var resp = await otherService.BatchTransactionDetail(item.BusinessID);
//                        if (resp == null)
//                        {
//                            item.Remark = "批量交易结果查询幸福职接口调用失败";
//                        }
//                        else
//                        {
//                            if (resp.Code == 200)
//                            {
//                                item.TransStatus = resp.Result.transStatus;
//                                item.TransStatusName = resp.Result.transStatusDesc;
//                                if (resp.Result.transStatus == "02" || resp.Result.transStatus == "03")
//                                {
//                                    item.Status = CmbcStatusEnumType.已确认;
//                                    foreach (var ritem in resp.Result.resultList)
//                                    {
//                                        if (item.Records.Any(f => f.BusinessID.ToString("N") == ritem.othSerialNo))
//                                        {
//                                            var sendRecord = item.Records.First(f => f.ID.ToString("N") == ritem.othSerialNo);
//                                            sendRecord.TranStatus = ritem.retType;
//                                            sendRecord.TranStatusName = ritem.retTypeDesc;
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                item.Remark = $"批量交易结果查询幸福职接口响应异常：异常信息： {resp.Message}";
//                            }
//                        }
//                        // 查询次数
//                        if (item.ConfirmTimes >= 30 && item.Status != CmbcStatusEnumType.已确认)
//                        {
//                            item.Status = CmbcStatusEnumType.确认异常;
//                            item.Remark = "交易确认异常，请人工确认";
//                        }
//                        if (item.Status == CmbcStatusEnumType.已确认)
//                        {
//                              base.Context.BeginTran();
                
//                            var salary = await salaryCustomerService.GetTransNav(f => f.ID == item.BusinessID);

//                            if (salary!=null)
//                            {
//                                var salaryRecords = salary.Records.Where(f => f.IsBatchPushSuc).ToList();
//                                Live_TransBatchRecord batchRecord = null;
//                                foreach (var salaryRecord in salaryRecords)
//                                {
//                                    if (item.Records.Any(f => f.BusinessID == salaryRecord.ID))
//                                    {
//                                        batchRecord = item.Records.First(f => f.BusinessID == salaryRecord.ID);

//                                        if (!string.IsNullOrWhiteSpace(batchRecord.TranStatus))
//                                        {
//                                            if (batchRecord.TranStatus == "1")
//                                            {
//                                                salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
//                                            }
//                                            else if (batchRecord.TranStatus == "2")
//                                            {
//                                                salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
//                                            }
//                                        }
//                                    }

//                                }
//                                await salaryCustomerService.UpdateTransNav(salary);
//                            }
                        
//                            await transBatchService.UpdateNav(item);

//                              base.Context.CommitTran();
//                        }
//                        else
//                        {
//                            await transBatchService.Update(item);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        item.Remark = $"异常信息：{ex.Message}";
//                        await transBatchService.Update(item);
//                    }
//                }
//            }

          
//        }
//    }
//}
