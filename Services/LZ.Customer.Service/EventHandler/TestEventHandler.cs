//using log4net.Core;
//using LZ.ApiService.Interface;
//using LZ.Core.EventData;
//using LZ.Core.EventData.Contract;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Model;
//using LZ.Model.Enums;
//using LZ.Model.FxqModel;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.OpenApi.Writers;
//using Org.BouncyCastle.Ocsp;
//using Senparc.CO2NET.Extensions;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Customer.Service.EventHandler
//{


//    /// <summary>
//    /// 发送短信验证码
//    /// </summary>
//    /// <param name="scopeFactory"></param>
//    public class ContractSigningSendSmsEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<ContractSigningSendSmsEventData>
//    {
//        public async Task HandleAsync(ContractSigningSendSmsEventData @event)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var sp = scope.ServiceProvider;
//                var logger = sp.GetRequiredService<ILogger<ContractSigningSendSmsEventHandler>>();
//                var signRecordService = sp.GetRequiredService<IContractRecordService>();
//                List<Live_ContractRecordSigner> signers = null;
//                var fxqService = sp.GetRequiredService<IFxqService>();
//                var record = await signRecordService.Get(@event.ContractRecordID);
//                if (record == null)
//                {
//                    throw new Exception($"{@event.ContractRecordID}合同已被删除！");
//                }
//                record.Remark = string.Empty;
//                if (record.Status != SignStatusEnumType.待发送短信)
//                {
//                    record.Remark = $"{record.Status}状态无法发送短信验证码；";
//                }
//                if (record.Status == SignStatusEnumType.待发送短信)
//                {
//                    signers = await signRecordService.GetRecordSigners(@event.ContractRecordID);
//                    if (signers == null || signers.Count <= 0)
//                    {
//                        record.Status = SignStatusEnumType.短信发送失败;
//                        record.Remark = "签署人信息不存在，短信发送失败";
//                    }
//                    else
//                    {
//                        if (!signers.Any(f => f.SignerName == @event.SmsName))
//                        {
//                            record.Status = SignStatusEnumType.短信发送失败;
//                            record.Remark = "签署人信息不匹配，短信发送失败";
//                        }
//                        else
//                        {

//                            record.Status = SignStatusEnumType.待签署;
//                            var signer = signers.First(f => f.SignerName == @event.SmsName);
//                            signer.SmsPhone = @event.SmsPhone;
//                            signer.SmsContent = @event.SmsContent;
//                            signer.SmsCode = @event.SmsCode;

//                            var sendSmsResp = await fxqService.SmsSend(signer.SmsPhone, signer.SmsContent);
//                            if (sendSmsResp != null && sendSmsResp.Code == 10000)
//                            {
//                                signer.FxqSmsId = sendSmsResp.Data.MsgId;
//                                signer.FxqSmsTime = sendSmsResp.Data.Time;
//                                signer.SmsSendTime = DateTime.Now;
//                            }
//                            else
//                            {
//                                record.Status = SignStatusEnumType.待签署;
//                                record.Remark = $"测试环境发送失败,短信验证码发送失败，异常信息：{sendSmsResp.Msg}";
//                                signer.Remark = $"测试环境发送失败，短信验证码发送失败，异常信息：{sendSmsResp.Msg}";
//                            }
//                        }
//                    }
//                }

//                await signRecordService.UpdateRecordSigners(record, signers);
//            }
//        }
//    }


//    ///// <summary>
//    ///// 合同签署
//    ///// </summary>
//    ///// <param name="scopeFactory"></param>
//    //public class ContractSigningEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<ContractSigningEventData>
//    //{
//    //    public async Task HandleAsync(ContractSigningEventData @event)
//    //    {
//    //        using (var scope = scopeFactory.CreateScope())
//    //        {
//    //            var sp = scope.ServiceProvider;
//    //            var logger = sp.GetRequiredService<ILogger<GenerateContractSignRecordEventHandler>>();
//    //            var signRecordService = sp.GetRequiredService<IContractSignRecordService>();
//    //            var signerService = sp.GetRequiredService<IContractSignerService>();
//    //            var fxqService = sp.GetRequiredService<IFxqService>();
//    //            var aliyunOSSService = sp.GetRequiredService<IAliyunOSSService>();

//    //            var record = await signRecordService.Get(@event.ContractRecordID);
//    //            var signerList = await signRecordService.GetRecordSigners(@event.ContractRecordID);
//    //            List<KeywordSigner> keywordSigners = new List<KeywordSigner>();
//    //            Live_ContractSigner contractSigner = null;

//    //            record.Remark = string.Empty;

//    //            if (record.Status == SignStatusEnumType.待签署 || record.Status == SignStatusEnumType.签署失败)
//    //            {
//    //                record.Status = SignStatusEnumType.待下载合同;
//    //                if (signerList.Any(f => f.SignerName == @event.SignerName))
//    //                {
//    //                    var signer = signerList.First(f => f.SignerName == @event.SignerName);
//    //                    if (signer.SmsCode != @event.SignerSmsCode)
//    //                    {
//    //                        record.Status = SignStatusEnumType.签署失败;
//    //                        record.Remark = "短信验证码不匹配，签署失败";
//    //                    }
//    //                    else
//    //                    {
//    //                        //生成电子签章
//    //                        foreach (var signerItem in signerList)
//    //                        {
//    //                            // 签署人信息
//    //                            contractSigner = await signerService.Get(f => f.Name == signerItem.SignerName && f.Idno == signerItem.SignerIdno && f.IsRealNameSuc);
//    //                            signerItem.SignOssUrl = contractSigner.OssUrl;
//    //                            keywordSigners.Add(new KeywordSigner(signerItem.SignerName, signerItem.SignerIdno, signerItem.SignOssUrl, signerItem.SignKeyword, 120, null));
//    //                        }
//    //                        var signResp = await fxqService.KeywordPdfContractSign(record.ContractOssUrl, keywordSigners.ToArray(), 1, null, null, new SignDate(1, null, null, null));
//    //                        if (signResp != null && signResp.Code == 10000)
//    //                        {
//    //                            record.FxqSignedOssUrl = signResp.Data;
//    //                            record.SignedTime = DateTime.Now;
//    //                        }
//    //                        else
//    //                        {
//    //                            record.Status = SignStatusEnumType.签署失败;
//    //                            record.Remark = $"签章失败，异常信息：{signResp.Msg}";
//    //                        }
//    //                    }

//    //                }
//    //                else
//    //                {
//    //                    record.Status = SignStatusEnumType.签署失败;
//    //                    record.Remark = "签署人不匹配，签署失败";
//    //                }
//    //            }
//    //            if (record.Status == SignStatusEnumType.待下载合同)
//    //            {
//    //                record.Status = SignStatusEnumType.待存证;
//    //                try
//    //                {
//    //                    var base64 = await fxqService.DownloadFile(record.FxqSignedOssUrl);
//    //                    var fileBase64 = Convert.FromBase64String(base64);
//    //                    using (var ms = new MemoryStream(fileBase64))
//    //                    {
//    //                        var ossResp = await aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
//    //                        if (ossResp.Code == 0)
//    //                        {
//    //                            record.SignedOssUrl = "https://lz-uplod.oss-cn-shanghai.aliyuncs.com/" + ossResp.Data;
//    //                        }
//    //                        else
//    //                        {
//    //                            record.Status = SignStatusEnumType.下载合同失败;
//    //                            record.Remark = "下载文件上传oss 文件失败";
//    //                        }
//    //                    }
//    //                }
//    //                catch (Exception ex)
//    //                {
//    //                    record.Status = SignStatusEnumType.下载合同失败;
//    //                    record.Remark = "下载文件上传oss 文件失败";
//    //                }
//    //            }
//    //            if (record.Status == SignStatusEnumType.待存证)
//    //            {
//    //                record.Remark = string.Empty;
//    //                record.Status = SignStatusEnumType.存证完成;
//    //                var resp = await fxqService.EvidenceFull(record.FxqSignedOssUrl, "2", null, null);
//    //                if (resp != null && resp.Code == 10000)
//    //                {
//    //                    record.FxqTransactionId = resp.Data.TransactionId;
//    //                    record.FxqHash = resp.Data.Hash;
//    //                }
//    //                else
//    //                {
//    //                    record.Status = SignStatusEnumType.存证失败;
//    //                    record.Remark = $"存证失败，异常信息：{resp.Msg}";
//    //                }
//    //            }
//    //            await signRecordService.UpdateRecordSigners(record, signerList);
//    //        }

//    //    }
//    //}

//    public class TestEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<TestEventData>
//    {
//        public async Task HandleAsync(TestEventData @event)
//        {
//            Console.WriteLine("8888888888888888888;");

//        }
//    }
//}
