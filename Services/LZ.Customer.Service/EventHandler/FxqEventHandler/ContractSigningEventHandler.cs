using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Customer.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.FxqModel;
using LZ.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.EventData.Contract;
using LZ.Model.ModelExtensions;
using SqlSugar;
using SqlSugar.IOC;
using NPOI.HSSF.Record;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.Core.Interface;

namespace LZ.Customer.Service.EventHandler.FxqEventHandler
{
    public class ContractSigningEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<ContractSigningEventData>
    {

        public async Task HandleAsync(ContractSigningEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var logger = sp.GetRequiredService<ILogger<ContractSigningEventHandler>>();
                var signRecordService = sp.GetRequiredService<IContractRecordService>();
                var signerService = sp.GetRequiredService<IContractSignerService>();
                var fxqService = sp.GetRequiredService<IFxqService>();
                var settingsService = sp.GetRequiredService<ISettingsService>();
                var aliyunOSSService = sp.GetRequiredService<IAliyunOSSService>();

                var setting =await settingsService.GetMasterSettings();

                var recordList = await signRecordService.ListWithDetail(f=>f.BatchNo==@event.BatchNo);
                foreach (var record in recordList)
                {
                    // 可以签署
                    if (record.Status == SignStatusEnumType.待签署 || record.Status == SignStatusEnumType.签署失败)
                    {
                        record.Status = SignStatusEnumType.待存储合同;
                        if (record.Signers != null && record.Signers.Count > 0)
                        {
                            var keywordSigners = record.Signers.Select(signerItem => new KeywordSigner(signerItem.SignerName, signerItem.SignerIdno, signerItem.SignOssUrl, signerItem.SignKeyword, 120, (signerItem.PageSignHeight.HasValue && signerItem.PageSignHeight.Value > 0) ? signerItem.PageSignHeight.Value:null));
                            var signResp = await fxqService.KeywordPdfContractSign(record.ContractOssUrl, keywordSigners.ToArray(), 1, null, null, new SignDate(1, null, null, null));
                            if (signResp != null && signResp.Code == 10000)
                            {
                                record.FxqSignedOssUrl = signResp.Data;
                                record.SignedTime = DateTime.Now;
                            }
                            else
                            {
                                record.Status = SignStatusEnumType.签署失败;
                                record.Remark = $"签章失败，异常信息：{signResp.Msg}";
                            }
                        }
                    }
                    if (record.Status == SignStatusEnumType.待存储合同)
                    {
                        record.Status = SignStatusEnumType.待存证;
                        try
                        {
                            var base64 = await fxqService.DownloadFile(record.FxqSignedOssUrl);
                            var fileBase64 = Convert.FromBase64String(base64);
                            using (var ms = new MemoryStream(fileBase64))
                            {
                                var ossResp = await aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
                                if (ossResp.Code == 0)
                                {
                                    record.SignedOssUrl = setting.AliBucketDomain + ossResp.Data;
                                }
                                else
                                {
                                    record.Status = SignStatusEnumType.存储合同失败;
                                    record.Remark = "下载文件上传oss 文件失败";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            record.Status = SignStatusEnumType.存储合同失败;
                            record.Remark = "下载文件上传oss 文件失败";
                        }
                    }
                    if (record.Status == SignStatusEnumType.待存证)
                    {
                        record.Remark = string.Empty;
                        record.Status = SignStatusEnumType.存证完成;
                        var resp = await fxqService.EvidenceFull(record.FxqSignedOssUrl, "2", null, null);
                        if (resp != null && resp.Code == 10000)
                        {
                            record.FxqTransactionId = resp.Data.TransactionId;
                            record.FxqHash = resp.Data.Hash;
                        }
                        else
                        {
                            record.Status = SignStatusEnumType.存证失败;
                            record.Remark = $"存证失败，异常信息：{resp.Msg}";
                        }
                    }
                    await signRecordService.Update(record);
                }
            }

        }
    }
}
