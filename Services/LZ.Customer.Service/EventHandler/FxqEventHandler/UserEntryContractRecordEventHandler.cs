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
using LZ.Model.ApiModel.FxqModel;
using LZ.Core.Interface;
using LZ.Model.Dtos;

namespace LZ.Customer.Service.EventHandler.FxqEventHandler
{
    public class UserEntryContractRecordEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<UserEntryContractRecordEventData>
    {
        public async Task HandleAsync(UserEntryContractRecordEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var today = DateTime.Today;
                var nextDay = today.AddDays(1);
                var sp = scope.ServiceProvider;
                var logger = sp.GetRequiredService<ILogger<UserEntryContractRecordEventHandler>>();
                var customerService = sp.GetRequiredService<ICustomerService>();
                var contractFileConfigService = sp.GetRequiredService<IContractFileConfigService>();
                var customerContractConfigService = sp.GetRequiredService<ICustomerContractConfigService>();
                var signerService = sp.GetRequiredService<IContractSignerService>();
                var recordService = sp.GetRequiredService<IContractRecordService>();
                var customerOtherService = sp.GetRequiredService<ICustomerOtherService>();
                var sendMQService = scope.ServiceProvider.GetService<ISendMQService>();

                Live_ContractFileConfigExtension smConfig = null;
                Live_ContractFileConfigExtension htConfig = null;
                Live_ContractRecordExtension record = null;
                // 获取员工入职记录
                var userEntryRecordResp = await customerOtherService.GetUserEntryContract(@event.UserEntryID);
                var userEntryRecord = userEntryRecordResp.Data;

                if (userEntryRecord == null)
                {
                    Common.StaticLog.Info($"UserEntryContractRecordEventHandler 生成合同失败，入职员工信息缺失，UserEntryId:{@event.UserEntryID}");
                }
        
             
                // 毕业院校

                // 专业




                var customerContractConfig = await customerContractConfigService.Get(f => f.CustomerGID == userEntryRecord.CustomerGID && f.IsEnabled);


                if (customerContractConfig == null)
                {
                    Common.StaticLog.Info($"UserEntryContractRecordEventHandler 生成合同失败，配置信息缺失，UserEntryId:{@event.UserEntryID}");
                }
                ContractEnumType contractEnumType = ContractEnumType.入职合同;
                var ageAndGender = IdCardHelper.GetAgeAndGender(userEntryRecord.CardID.Trim());
                int age = ageAndGender.Age;
                string sex = ageAndGender.Gender;
                bool isRetire = false;
                // 判断是否是返聘
                if (@event.ContractType == ContractEnumType.入职合同)
                {
                    if (sex == "男" && age >= 60)
                    {
                        isRetire = true;
                    }
                    else if (sex == "女" && age >= 50)
                    {
                        isRetire = true;
                    }
                }
                else if (@event.ContractType == ContractEnumType.离职合同)
                {
                    // 存在返聘合同
                    var retireRecord = await recordService.Get(f => f.UserEntryID == @event.UserEntryID && f.IsRetire == true);
                    if (retireRecord != null)
                    {
                        isRetire = true;
                    }
                }

                if (isRetire)
                {
                    // 返聘
                    if (@event.ContractType == ContractEnumType.入职合同)
                    {
                        smConfig = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.EntryDeclareFileID);
                        htConfig = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.ReEntryFileID);
                    }
                    else
                    {
                        htConfig = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.ReLeaveFileID);
                    }
                }
                else
                {
                    // 常规入职合同
                    if (@event.ContractType == ContractEnumType.入职合同)
                    {
                        smConfig = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.EntryDeclareFileID);
                        htConfig = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.EntryFileID);
                    }
                    else
                    {
                        htConfig = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.LeaveFileID);
                    }
                }

                if (htConfig==null)
                {
                    Common.StaticLog.Info($"UserEntryContractRecordEventHandler 生成合同失败，合同配置文件缺失，UserEntryId:{@event.UserEntryID}");
                    return;
                }

                //userEntryRecord.EntryTime = userEntryRecord.EntryTime.Value.AddMonths(customerContractConfig.SignMonths.Value);
                var retireDate = userEntryRecord.RetireDate;
                if (retireDate == null || retireDate.Value.Year <= 2000)
                {
                    retireDate = IdCardHelper.GetRetireDate(userEntryRecord.CardID.Trim());
                }

                record = new Live_ContractRecordExtension()
                {

                   
                    RetireDate = retireDate,
                    ContractDate = userEntryRecord.ContractDate ?? userEntryRecord.EntryTime.Value.Date,
                    ID = Guid.NewGuid(),
                    RelateGID = null,
                    RelateID = userEntryRecord.ID,
                    ContractType = @event.ContractType,
                    CreateTime = @event.CreateTime,
                    //TemplateOssUrl = contractFileItem.FilePath,
                    Status = SignStatusEnumType.待生成,
                    ContractOssUrl = string.Empty,
                    FxqHash = string.Empty,
                    FxqSignedOssUrl = string.Empty,
                    FxqTransactionId = string.Empty,
                    IsNeedKeywordReplace = false,
                    KeywordReplaceContent = string.Empty,
                    Remark = string.Empty,
                    RStatus = null,
                    SignedOssUrl = string.Empty,
                    SignedTime = null,
                    CustomerGID = userEntryRecord.CustomerGID,
                    BatchNo = @event.BatchNo,
                    ContractStartDate = userEntryRecord.EntryTime.Value.Date,
                    ContractEndDate = userEntryRecord.EntryTime.Value.Date.AddMonths(customerContractConfig.SignMonths.Value),
                    CustomerCompanyNo = userEntryRecord.CustomerCompanyNo,
                    CustomerName = userEntryRecord.CustomerName,
                    SigningUnitCompanyNo = userEntryRecord.SigningUnitCompanyNo,
                    SigningUnitName = userEntryRecord.SigningUnitName,
                    SigningUnitGID = userEntryRecord.SigningUnitID,
                    UserName = userEntryRecord.Name,
                    UserEntryID = userEntryRecord.ID,
                    UserIdCardNo = userEntryRecord.CardID,
                    UserPhone = userEntryRecord.Phone,
                    EndDate = null,
                    SendSmsTime = null,
                    SigningTime = null,
                    SmsCode = string.Empty,
                    SmsContent = string.Empty,
                    StartDate = null,
                    ReadySigningDate = userEntryRecord.EntryTime.Value.Date.AddDays(customerContractConfig.DelayDays.Value),
                    UserID = userEntryRecord.UserID,
                    ContractMonth = customerContractConfig.SignMonths,
                    Education = userEntryRecord.Education,
                    EmergencyContactName = userEntryRecord.EmergencyContact,
                    EmergencyContactPhone = userEntryRecord.EmergencyContactPhone,
                    EmpAge = age,
                    EmpIdCardNo = userEntryRecord.CardID,
                    EmpName = userEntryRecord.Name,
                    EmpNo = string.Empty,
                    EmpSex = sex,
                    EmpSignKeyword = string.Empty,
                    //EntryDate = userEntryRecord.EntryTime.Value.Date,
                    IsNotice = false,
                    IsRetire = isRetire,
                    JobCity = userEntryRecord.CustomerCity,
                    JobContent = string.Empty,
                    JobName = userEntryRecord.WorkName,
                    //LeaveDate = userEntryRecord.LeaveTime.Value.Date,
                    HomeAddress = userEntryRecord.Province + userEntryRecord.City + userEntryRecord.District + userEntryRecord.Address,
                    NativePlace = userEntryRecord.IdcardAddress,
                    SigningUnitAddress = userEntryRecord.SigningUnitAddress,
                    SigningUnitLegalPerson = userEntryRecord.SigningUnitLegalPerson,
                    SigningUnitPhone = userEntryRecord.SigningUnitPhone,
                    SigningUnitSignKeyword = string.Empty,
                    TrialMonth = customerContractConfig.TrialMonths,
                    TrialEndDate = userEntryRecord.EntryTime.Value.Date.AddMonths(customerContractConfig.TrialMonths.Value),
                    EmpPhone = userEntryRecord.Phone,
                    IsSuc = true,
                    SignType= customerContractConfig.SignType,
                    College=string.Empty,
                    CollegeMajor=string.Empty,

                };


                if (!string.IsNullOrWhiteSpace(userEntryRecord.CustomizeField) && (userEntryRecord.CustomizeField.Contains("毕业院校") || userEntryRecord.CustomizeField.Contains("专业")))
                {
                    var fields = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomizeFieldRecord>>(userEntryRecord.CustomizeField);

                    if (fields.Any(f=>f.Title=="毕业院校"))
                    {
                        record.College = fields.First(f => f.Title == "毕业院校")?.Value.ToString();
                    }

                    if (fields.Any(f => f.Title == "专业"))
                    {
                        record.CollegeMajor = fields.First(f => f.Title == "专业")?.Value.ToString();
                    }
                }


                if (smConfig!=null)
                {
                    record.IsSmNeedSigningUnitSignKeyword = smConfig.IsNeedSigningUnitSignKeyword;
                }
                if (@event.ContractType == ContractEnumType.离职合同)
                {
                    record.ReadySigningDate = DateTime.Today; 
                }
                if (record.ContractType == ContractEnumType.入职合同)
                {
                    record.SmTemplateFilePath = smConfig.FilePath;
                    record.SmPdfFilePath = string.Empty;
                    record.SmWordFilePath = string.Empty;
                    record.SmSignedFilePath = string.Empty;
                    record.TemplateFilePath = htConfig.FilePath;
                    record.PdfFilePath = string.Empty;
                    record.WordFilePath = string.Empty;
                    record.SignedFilePath = string.Empty;
                }
                else
                {
                    record.SmTemplateFilePath = string.Empty;
                    record.SmPdfFilePath = string.Empty;
                    record.SmWordFilePath = string.Empty;
                    record.SmSignedFilePath = string.Empty;
                    record.TemplateFilePath = htConfig.FilePath;
                    record.PdfFilePath = string.Empty;
                    record.WordFilePath = string.Empty;
                    record.SignedFilePath = string.Empty;
                }
                // 返聘


                // 入离职时间存在
                //if (userEntryRecord.ContractStartDate.HasValue && userEntryRecord.ContractEndDate.HasValue && userEntryRecord.ContractStartDate.Value.Year > 2000 & userEntryRecord.ContractEndDate.Value.Year > 2000)
                //{
                //    record.ContractStartDate = userEntryRecord.ContractStartDate.Value;
                //    record.ContractEndDate = userEntryRecord.ContractEndDate.Value;
                //}
                //else
                //{

                //    if (record.IsRetire)
                //    {

                //        if (record.ContractDate > record.RetireDate)
                //        {
                //            record.ContractEndDate = record.ContractDate;
                //        }
                //        else {
                //            record.ContractEndDate = record.RetireDate;
                //        }

                //        for (int i = 0; i < 100; i++)
                //        {
                //            if (record.ContractEndDate <= nextDay)
                //            {
                //                record.ContractEndDate = record.ContractEndDate.Value.AddMonths(customerContractConfig.SignMonths.Value);
                //            }
                //            else
                //            {
                //                break;
                //            }

                //        }
                //        record.ContractStartDate = record.ContractEndDate.Value.AddYears(-1);
                //        record.TrialDescription = "0个月";
                //        record.ContractEndDate = record.ContractEndDate.Value.AddDays(-1);
                //    }
                //    else
                //    {
                //        record.ContractEndDate = record.ContractDate;
                //        record.ContractEndDate = record.ContractEndDate.Value.AddMonths(customerContractConfig.SignMonths.Value);
                //        for (int i = 0; i < 100; i++)
                //        {
                //            if (record.ContractEndDate <= nextDay)
                //            {
                //                record.ContractEndDate = record.ContractEndDate.Value.AddMonths(customerContractConfig.SignMonths.Value);
                //            }
                //            else
                //            {
                //                break;
                //            }
                //        }
                //        record.ContractStartDate = record.ContractEndDate.Value.AddMonths(-1 * customerContractConfig.SignMonths.Value);
                //        record.ContractEndDate = record.ContractEndDate.Value.AddDays(-1);
                //    }
                //}




                if (record.IsRetire)
                {

                    if (record.ContractDate > record.RetireDate)
                    {
                        record.ContractEndDate = record.ContractDate;
                    }
                    else
                    {
                        record.ContractEndDate = record.RetireDate;
                    }

                    for (int i = 0; i < 100; i++)
                    {
                        if (record.ContractEndDate <= nextDay)
                        {
                            record.ContractEndDate = record.ContractEndDate.Value.AddYears(1);
                        }
                        else
                        {
                            break;
                        }

                    }
                    record.ContractStartDate = record.ContractEndDate.Value.AddYears(-1);
                    record.TrialDescription = "0个月";
                    record.ContractEndDate = record.ContractEndDate.Value.AddDays(-1);
                }
                else
                {
                    record.ContractEndDate = record.ContractDate;
                    for (int i = 0; i < 100; i++)
                    {
                        if (record.ContractEndDate <= nextDay)
                        {
                            record.ContractEndDate = record.ContractEndDate.Value.AddMonths(customerContractConfig.SignMonths.Value);
                        }
                        else
                        {
                            break;
                        }
                    }
                    record.ContractStartDate = record.ContractEndDate.Value.AddMonths(-1 * customerContractConfig.SignMonths.Value);
                    record.ContractEndDate = record.ContractEndDate.Value.AddDays(-1);
                }


                // 判定为续签
                if (record.ContractStartDate.Value != record.ContractDate)
                {
                    record.IsRenewal = true;
                }
                if (userEntryRecord.EntryTime.HasValue)
                {
                    record.EntryDate = userEntryRecord.EntryTime.Value.Date;
                }
                if (userEntryRecord.LeaveTime.HasValue)
                {
                    record.LeaveDate = userEntryRecord.LeaveTime.Value.Date;
                }
                //非续签非返聘
                if (record.IsRenewal == false && record.IsRetire == false)
                {
                    record.TrialStartDate = record.ContractStartDate.Value;
                    record.TrialEndDate = record.ContractStartDate.Value.AddMonths(customerContractConfig.TrialMonths.Value);
                    record.TrialDescription = $"，其中试用期为{customerContractConfig.TrialMonths}个月，从{record.TrialStartDate.Value.ToString("yyyy-MM-dd")}起至{record.TrialEndDate.Value.ToString("yyyy-MM-dd")}止；试用期经考核合格后，甲方通知乙方办理转正手续";
                    //record.TrialDescription = $"从{record.TrialStartDate.Value.ToString("yyyy-MM-dd")}起至{record.TrialEndDate.Value.ToString("yyyy-MM-dd")}止";
                }
                else
                {
                    record.TrialDescription = "，";
                }

                if (htConfig.Signers != null && htConfig.Signers.Count > 0)
                {
                    if (htConfig.Signers.Any(f => f.SignerPartyType == SignerPartyEnumType.第一当事人))
                    {
                        var signer = htConfig.Signers.First(f => f.SignerPartyType == SignerPartyEnumType.第一当事人);
                        record.SigningUnitSignKeyword = signer.SignKeyword;
                        record.SigningUnitSignDate = signer.IsSignTime;
                        record.SigningUnitSignPageHeight = signer.PageSignHeight;
                    }
                    else
                    {

                        record.SigningUnitSignKeyword = string.Empty; ;
                        record.SigningUnitSignDate = false;
                        record.SigningUnitSignPageHeight = 0;

                    }
                    if (htConfig.Signers.Any(f => f.SignerPartyType == SignerPartyEnumType.第二当事人))
                    {
                        var signer = htConfig.Signers.First(f => f.SignerPartyType == SignerPartyEnumType.第二当事人);
                        record.EmpSignKeyword = signer.SignKeyword;
                        record.EmpSignDate = signer.IsSignTime;
                        record.EmpPageHeight = signer.PageSignHeight;
                    }
                    else
                    {
                        record.EmpSignKeyword = string.Empty; ;
                        record.EmpSignDate = false;
                        record.EmpPageHeight = 0;
                    }
                }

                if (record.CustomerName == "湖州珞狮供应链管理有限公司" || record.CustomerName == "杭州珞狮供应链管理有限公司")
                {
                    record.JobCity = userEntryRecord.City;
                }
                await recordService.Add(record);


                if (record.ContractType == ContractEnumType.入职合同)
                {
                    if (record.IsRetire)
                    {
                        var mqMsg = new { type = "contractPendingRetireSign", Title = "正在签署返聘协议",Name= record.EmpName,CardID= record.EmpIdCardNo };
                        sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                    }
                    else if (record.IsRenewal) {
                        var mqMsg = new { type = "contractPendingRenewalSign", Title = "正在续签合同", Name = record.EmpName, CardID = record.EmpIdCardNo };
                        sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                    }
                    else {
                        var mqMsg = new { type = "contractPendingEntrySign", Title = "正在入职网签", Name = record.EmpName, CardID = record.EmpIdCardNo };
                        sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                    }
                }
                else {

                    var mqMsg = new { type = "contractPendingLeaveSign", Title = "正在离职网签", Name = record.EmpName, CardID = record.EmpIdCardNo };
                    sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());

                }
            }
        }
    }
}
