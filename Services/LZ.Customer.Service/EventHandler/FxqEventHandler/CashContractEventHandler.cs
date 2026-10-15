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
using LZ.Core.RabbitMQ;
using NPOI.HSSF.Record.Chart;
using LZ.Common;
using LZ.Core.EventData.Cash;
using LZ.Core.Interface;

namespace LZ.Customer.Service.EventHandler.FxqEventHandler
{
    public class CashContractEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<CashContractEventData>
    {
        public async Task HandleAsync(CashContractEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var logger = sp.GetRequiredService<ILogger<CashContractEventHandler>>();
                var settingsService = sp.GetRequiredService<ISettingsService>();
                var signerService = sp.GetRequiredService<IContractSignerService>();
                var customerService = sp.GetRequiredService<ICustomerService>();
                var contractFileConfigService = sp.GetRequiredService<IContractFileConfigService>();
                var customerContractConfigService = sp.GetRequiredService<ICustomerContractConfigService>();
                var fxqService = sp.GetRequiredService<IFxqService>();
                var aliyunOSSService = sp.GetRequiredService<IAliyunOSSService>();
                var otherService = sp.GetRequiredService<ICustomerOtherService>();
                var eventPublisher = sp.GetRequiredService<IEventPublisher>();
                var customer = await customerService.Get(f => f.CustomerGID == @event.CustomerGID);
                var setting= await settingsService.GetMasterSettings();
                Live_V_SalaryCashSendRecord cashSendRecord = null;
                var cashRecordResp = await otherService.GetSalaryCashSendRecordByIdAsync(@event.SalaryCashSendRecordID);

                if (cashRecordResp != null && cashRecordResp.Code == 200)
                {
                    cashSendRecord = cashRecordResp.Data;
                }
                if (cashSendRecord == null)
                {
                    throw new Exception($"现金发放记录不存在");
                }

                var customerContractConfig = await customerContractConfigService.Get(f => f.CustomerGID == customer.CustomerGID && f.IsEnabled);

                Live_ContractFileConfigExtension contractFileInfo = null;
                if (customerContractConfig.CashFileID != null)
                {
                    contractFileInfo = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.CashFileID.Value);
                }

                if (contractFileInfo == null)
                {
                    cashSendRecord.Status = CashSendStatusEnumType.签收单生成异常;
                    cashSendRecord.Msg = "现金签收单未配置";
                }
                else {
                    if (contractFileInfo.Signers != null && contractFileInfo.Signers.Count > 0)
                    {
                        cashSendRecord.SignKeyword = contractFileInfo.Signers.FirstOrDefault().SignKeyword;
                    }
                    else {
                        cashSendRecord.Status = CashSendStatusEnumType.签收单生成异常;
                        cashSendRecord.Msg = "现金签收单签署人未配置";
                    }
                }
                // 获取签章信息

                var signer = await signerService.Get(f => f.Idno == cashSendRecord.IdCardNo && f.IsRealNameSuc && f.IsEnabled);
                if (signer == null)
                {

                    cashSendRecord.Status = CashSendStatusEnumType.签收单生成异常;
                    cashSendRecord.Msg = "签署人未实名或电子签名未生成";
                }
                else {
                    cashSendRecord.SignerOssUrl= signer.OssUrl;
                }


                if (cashSendRecord.Status == CashSendStatusEnumType.待生成签收单)
                {
                    Dictionary<string, string> kv = new Dictionary<string, string>();

                    kv.Add("employeeName", cashSendRecord.Name);
                    kv.Add("employeeIdCardNo", cashSendRecord.IdCardNo);
                    kv.Add("employeePhone", cashSendRecord.Phone);
                    kv.Add("sealEmployeeCompanyName", cashSendRecord.SigningUnitCompanyName);
                    kv.Add("employeeCompanyName", cashSendRecord.CustomerName);
                    kv.Add("salaryMonth", cashSendRecord.SalaryMonth.Value.ToString("yyyy年MM月"));
                    kv.Add("totalAmount", cashSendRecord.TotalAmount.ToString());
                    kv.Add("totalAmountWord", Tool.MoneyToUpper(cashSendRecord.SendAmount.Value.ToString()));

                    kv = kv.OrderBy(f => f.Key).ToDictionary();

                    var fxqResponse = await fxqService.DocxKeywordReplace(contractFileInfo.FilePath, kv, 0);
                    if (fxqResponse != null && fxqResponse.Code == 10000)
                    {
                        // 转换成pdf
                        var pdfRes = await fxqService.DocxToPdf(fxqResponse.Data);
                        if (pdfRes != null && pdfRes.Code == 1000)
                        {
                            var fileBytes = Convert.FromBase64String(pdfRes.Data);
                            using (var ms = new MemoryStream(fileBytes))
                            {
                                var ossResp = await aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
                                if (ossResp.Code == 0)
                                {

                                    cashSendRecord.Status = CashSendStatusEnumType.待签署;
                                    cashSendRecord.FilePath = setting.AliBucketDomain + ossResp.Data;
                                    cashSendRecord.Msg = string.Empty;


                                }
                                else
                                {
                                    cashSendRecord.Status = CashSendStatusEnumType.签收单生成异常;
                                    cashSendRecord.FilePath = string.Empty;
                                    cashSendRecord.Msg = $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}";
                                }
                            }
                        }
                        else
                        {
                            cashSendRecord.Status = CashSendStatusEnumType.签收单生成异常;
                            cashSendRecord.FilePath = string.Empty;
                            cashSendRecord.Msg = $"合同文件Word转换成Pdf文件失败,异常信息:{fxqResponse.Msg}";
                        }
                    }
                    else
                    {
                        cashSendRecord.Status = CashSendStatusEnumType.签收单生成异常;
                        cashSendRecord.FilePath = string.Empty;
                        cashSendRecord.Msg = $"关键词替换失败,异常信息:{fxqResponse.Msg}";
                    }


                }
                CashRecordStatusEventData cashRecordStatusEventData = new CashRecordStatusEventData()
                {
                    FilePath = cashSendRecord.FilePath,
                    Status = cashSendRecord.Status.Value,
                    Msg = cashSendRecord.Msg,
                    SalaryCashSendRecordID = cashSendRecord.ID,
                    SignerOssUrl = cashSendRecord.SignerOssUrl,
                    SignKeyword = cashSendRecord.SignKeyword,
                    
                };

                eventPublisher.Publish(cashRecordStatusEventData);


            }
        }
    }
}
