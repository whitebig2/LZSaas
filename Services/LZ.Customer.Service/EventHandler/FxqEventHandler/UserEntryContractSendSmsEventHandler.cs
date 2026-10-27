//using LZ.ApiService.Interface;
//using LZ.ApiService.Services;
//using LZ.Core.EventData;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Model.Enums;
//using LZ.Model.FxqModel;
//using LZ.Model;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LZ.Core.EventData.FxqEventData;
//using LZ.Core.EventData.Contract;
//using LZ.Model.ModelExtensions;
//using SqlSugar;
//using SqlSugar.IOC;
//using NPOI.HSSF.Record;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

//namespace LZ.Customer.Service.EventHandler.FxqEventHandler
//{
//    public class UserEntryContractSendSmsEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<UserEntryContractSendSmsEventData>
//    {

//        public async Task HandleAsync(UserEntryContractSendSmsEventData @event)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var sp = scope.ServiceProvider;
//                var logger = sp.GetRequiredService<ILogger<UserEntryContractSendSmsEventHandler>>();
//                var recordService = sp.GetRequiredService<IContractRecordService>();
//                var  eventPublisher = sp.GetRequiredService<IEventPublisher>();
//                List<Live_ContractRecordSigner> signers = null;
//                var fxqService = sp.GetRequiredService<IFxqService>(); var record = await recordService.Get(f =>f.ID==@event.ContractRecordID);
//                if (record == null)
//                {
//                    throw new Exception($"合同已被删除！");
//                }

//                List<SignStatusEnumType> statusEnum = new List<SignStatusEnumType>() {
//                 SignStatusEnumType.待发送短信,
//                 SignStatusEnumType.短信发送失败,
//                  SignStatusEnumType.待签署,
//                  SignStatusEnumType.签署失败,
//                };
//                record.Remark = string.Empty;
//                if (statusEnum.Contains(record.Status.Value) && record.ContractType != ContractEnumType.入职声明)
//                {

//                    Random random = new Random();
//                    int randomNumber = random.Next(100000, 1000000);
//                    record.SmsCode = randomNumber.ToString();
//                    record.SmsContent = $"电子合同网签验证码为：{record.SmsCode}";
//                    var sendSmsResp = await fxqService.SmsSend(record.EmpPhone, record.SmsContent);
//                    if (sendSmsResp != null && sendSmsResp.Code == 10000)
//                    {
//                        //signer.FxqSmsId = sendSmsResp.Data.MsgId;
//                        //signer.FxqSmsTime = sendSmsResp.Data.Time;
//                        record.IsNotice = true;
//                        record.SendSmsTime = DateTime.Now;
//                        record.Status = SignStatusEnumType.待签署;
//                    }
//                    else
//                    {
//                        //测试环境
//                        if (sendSmsResp.Code == 70001)
//                        {
//                            record.IsNotice = true;
//                            record.Remark = $"测试环境发送失败,短信验证码发送失败，异常信息：{sendSmsResp.Msg}";
//                            record.Status = SignStatusEnumType.待签署;
//                            record.SendSmsTime = DateTime.Now;
//                        }
//                        else
//                        {
//                            record.Status = SignStatusEnumType.短信发送失败;
//                            record.IsSuc = false;
//                            record.Remark = $"测试环境发送失败,短信验证码发送失败，异常信息：{sendSmsResp.Msg}";
//                            record.SmsCode = string.Empty;
//                            record.SmsContent = string.Empty;
//                            record.SendSmsTime = null;
//                        }
//                    }

//                    #region bao

//                    if (record.ContractType == ContractEnumType.入职合同 && record.Status == SignStatusEnumType.待签署)
//                    {
//                        var sm = await recordService.Get(f => f.BatchNo == record.BatchNo && f.ContractType == ContractEnumType.入职声明);
//                        if (sm != null)
//                        {
//                            sm.Status = SignStatusEnumType.待签署;

//                            sm.Status = record.Status;
//                            sm.SmsCode = record.SmsCode;
//                            sm.SmsContent = record.SmsContent;
//                            sm.SendSmsTime = record.SendSmsTime;
//                            sm.IsNotice = record.IsNotice;
//                        }
//                        DbScoped.SugarScope.BeginTran();
//                        await recordService.Update(sm);
//                        await recordService.Update(record);
//                        DbScoped.SugarScope.CommitTran();
//                    }
//                    else
//                    {
//                        await recordService.Update(record);
//                    }
//                    #endregion
//                    #region 修改入职记录合同状态
//                    UserContractStatusEventData userContractStatusEventData = new UserContractStatusEventData()
//                    {
//                        EntryID = record.UserEntryID.Value,
//                    };
//                    if (record.Status == SignStatusEnumType.待签署)
//                    {
//                        if (record.ContractType == ContractEnumType.入职合同)
//                        {
//                            userContractStatusEventData.Status = ContractStatus.EntryWaitSign;
//                        }
//                        else
//                        {
//                            userContractStatusEventData.Status = ContractStatus.LeaveWaitSign;
//                        }
//                        var sendNoticeEventData = new SendNoticeEventData();
//                        sendNoticeEventData.Platform = Model.Enums.PlatformType.XFZMini;
//                        sendNoticeEventData.SendType = Model.Enums.NoticeSendEnumType.All;
//                        sendNoticeEventData.NoticeType = Model.Enums.NoticeEnumType.UserContract;
//                        sendNoticeEventData.Keyword1 = nameof(record.ContractType);
//                        sendNoticeEventData.Keyword2 = record.CustomerName;
//                        sendNoticeEventData.Keyword3 = record.SmsContent;
//                        sendNoticeEventData.UserID = record.UserID.Value;
//                        eventPublisher.Publish(sendNoticeEventData);
//                    }
//                    else
//                    {
//                        if (record.ContractType == ContractEnumType.入职合同)
//                        {
//                            userContractStatusEventData.Status = ContractStatus.EntryCreateError;
//                        }
//                        else
//                        {
//                            userContractStatusEventData.Status = ContractStatus.LeaveCreateError;
//                        }
//                    }
//                    eventPublisher.Publish(userContractStatusEventData);
//                    #endregion
//                }
//                else {
//                    record.Remark = "无法发送短信";
//                    await recordService.Update(record);
//                }
//            }
//        }
//    }
//}
