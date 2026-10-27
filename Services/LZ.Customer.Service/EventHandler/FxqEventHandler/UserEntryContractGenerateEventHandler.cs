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
//using LZ.Core.RabbitMQ;
//using NPOI.HSSF.Record.Chart;
//using Org.BouncyCastle.Ocsp;
//using System.Security.Cryptography;

//namespace LZ.Customer.Service.EventHandler.FxqEventHandler
//{
//    public class UserEntryContractGenerateEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<UserEntryContractGenerateEventData>
//    {
//        public async Task HandleAsync(UserEntryContractGenerateEventData @event)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var sp = scope.ServiceProvider;
//                var logger = sp.GetRequiredService<ILogger<UserEntryContractGenerateEventHandler>>();
//                var  enterpriseService = sp.GetRequiredService<IEnterpriseService>();
//                var customerService = sp.GetRequiredService<ICustomerService>();
//                var signerService = sp.GetRequiredService<IContractSignerService>();
//                var recordService = sp.GetRequiredService<IContractRecordService>();
//                var fxqService = sp.GetRequiredService<IFxqService>();
//                var aliyunOSSService = sp.GetRequiredService<IAliyunOSSService>();
//                var  eventPublisher = sp.GetRequiredService<IEventPublisher>();

//                List<Live_ContractRecord> records = new List<Live_ContractRecord>();
//                if (@event.ContractType == ContractEnumType.入职合同)
//                {
//                    records = await recordService.List(f =>!string.IsNullOrWhiteSpace(f.EmpName)&& f.BatchNo == @event.BatchNo && (f.ContractType == ContractEnumType.入职声明 || f.ContractType == ContractEnumType.入职合同));
//                }
//                else if (@event.ContractType == ContractEnumType.离职合同)
//                {
//                    records = await recordService.List(f => f.BatchNo == @event.BatchNo && f.ContractType == ContractEnumType.离职合同);
//                }
//                else
//                {
//                    return;
//                }
//                var empIdCardNo = records.First().EmpIdCardNo;
//                var signingUnitCompanyNo = records.First().SigningUnitCompanyNo;
//                var signers =await signerService.List(f => f.IsEnabled && (f.Idno == empIdCardNo || f.Idno == signingUnitCompanyNo));

          
//                foreach (var record in records)
//                {
//                    record.Remark = string.Empty;
//                    // 员工实名信息
//                    if (string.IsNullOrWhiteSpace(record.EmpOssUrl))
//                    {
//                        if (signers.Any(f => f.Idno == record.EmpIdCardNo))
//                        {

//                            record.EmpOssUrl = signers.First(f => f.Idno == record.EmpIdCardNo).OssUrl;
//                        }
//                        else
//                        {
//                            record.Status= SignStatusEnumType.生成异常;
//                            record.IsSuc= false;
//                            record.Remark += $"{record.EmpName}未实名认证！";
//                        }
//                    }
//                    // 签章单位实名信息
//                    if (string.IsNullOrWhiteSpace(record.SigningUnitOssUrl))
//                    {
//                        if (signers.Any(f => f.Idno == record.SigningUnitCompanyNo))
//                        {
//                            record.SigningUnitOssUrl = signers.First(f => f.Idno == record.SigningUnitCompanyNo).OssUrl;
//                        }
//                        else
//                        {
//                            record.Status = SignStatusEnumType.生成异常;
//                            record.IsSuc = false;
//                            record.Remark += $"{record.SigningUnitName}未实名认证！";
//                        }
//                    }
//                    // 模板不为空， word转换为空
//                    if (!string.IsNullOrWhiteSpace(record.TemplateOssUrl) && string.IsNullOrWhiteSpace(record.WordOssUrl))
//                    {

//                        var keywords = new Dictionary<string, string>();
//                        keywords.Add("PartyA", record.SigningUnitName);
//                        keywords.Add("LegalPersonName", record.SigningUnitLegalPerson);
//                        keywords.Add("LegalPersonAddress", record.SigningUnitAddress);
//                        keywords.Add("PartyB", record.SigningUnitAddress);
//                        keywords.Add("Sex", record.EmpSex);
//                        keywords.Add("Phone", record.EmpPhone);
//                        keywords.Add("IdCardNo", record.EmpIdCardNo);
//                        keywords.Add("JobCity", record.JobCity);
//                        keywords.Add("JobName", record.JobName);
//                        keywords.Add("PlaceOfResidence", "居住地址暂时未获取");
//                        keywords.Add("ContractStartDate", record.ContractStartDate.Value.ToString("yyyy-MM-dd"));
//                        keywords.Add("ContractEndDate", record.EndDate.Value.ToString("yyyy-MM-dd"));
//                        keywords.Add("ProbationPeriod", record.TrialEndDate.Value.ToString("yyyy-MM-dd"));
//                        keywords.Add("RegisteredResidence", record.NativePlace);
//                        if (record.LeaveDate.HasValue)
//                        {
//                            keywords.Add("LeaveDate", record.CustomerName);
//                        }

//                        keywords.Add("EducationName", record.Education);
//                        keywords.Add("EmergencyContact", record.EmergencyContactName);
//                        keywords.Add("EmergencyContactPhone", record.EmergencyContactPhone);
//                        keywords.Add("IdCardAddress", record.NativePlace);

//                        keywords = keywords.OrderBy(f => f.Key).Where(f => !string.IsNullOrWhiteSpace(f.Value)).ToDictionary(f => f.Key, f => f.Value);
//                        if (keywords.Count > 0)
//                        {
//                            record.IsNeedKeywordReplace = true;
//                            var fxqResponse = await fxqService.DocxKeywordReplace(record.TemplateOssUrl, keywords, 1);
//                            if (fxqResponse != null && fxqResponse.Code == 10000)
//                            {
//                                record.WordOssUrl = fxqResponse.Data;
//                            }
//                            else
//                            {
//                                record.Status = SignStatusEnumType.生成异常;
//                                record.IsSuc = false;
//                                record.Remark += $"关键词替换失败,异常信息:{fxqResponse.Msg}";
//                            }
//                        }
//                    }
//                    // word 转换成pdf
//                    if (!string.IsNullOrWhiteSpace(record.WordOssUrl) && string.IsNullOrWhiteSpace(record.ContractOssUrl))
//                    {
//                        // 下载word 文件
//                        var base64 = await fxqService.DownloadFile(record.WordOssUrl);
//                        var pdfRes = await fxqService.DocxToPdf(base64);
//                        if (pdfRes != null && pdfRes.Code == 1000)
//                        {
//                            var fileBytes = Convert.FromBase64String(pdfRes.Data);
//                            using (var ms = new MemoryStream(fileBytes))
//                            {
//                                var ossResp = await aliyunOSSService.UploadFile(ms, "11.pdf", true, false, "temps");
//                                if (ossResp.Code == 0)
//                                {
//                                    record.ContractOssUrl = "https://lz-uplod.oss-cn-shanghai.aliyuncs.com" + ossResp.Data;
//                                }
//                                else
//                                {
//                                    record.Status = SignStatusEnumType.生成异常;
//                                    record.IsSuc = false;
//                                    record.Remark = $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}";
//                                }
//                            }
//                        }
//                        else
//                        {
//                            record.Status = SignStatusEnumType.生成异常;
//                            record.IsSuc = false;
//                            record.Remark = $"合同文件Word转换成Pdf文件失败,异常信息:{pdfRes.Msg}";
//                        }
//                    }
//                }
//                if (records.Any(f => f.Status == SignStatusEnumType.生成异常))
//                {
//                    foreach (var record in records)
//                    {
//                        record.Status = SignStatusEnumType.生成异常;
//                    }
//                    await recordService.Update(records);
//                }
//                else {
//                    foreach (var record in records)
//                    {
//                        record.Status = SignStatusEnumType.待发送短信;
//                    }
//                    await recordService.Update(records);
//                }
//            }
//        }
//    }
//}
