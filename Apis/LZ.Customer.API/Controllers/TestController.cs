using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core.EventData;
using LZ.Core.EventData.Contract;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Customer.Service.EventHandler.FxqEventHandler;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.IExcelTemplate;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.Extensions;
using SqlSugar.IOC;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LZ.Customer.API.Controllers
{
    [AllowAnonymous]
    public class TestController : CustomerControllerBase
    {


        private readonly IAliyunOSSService _oSSService;
        private readonly IFxqDevService _fxqDevService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IContractRecordService _contractRecordService;
        private readonly IContractSignerService _contractSignerService;
        private readonly IContractFileConfigService _contractFileConfigService;
        private readonly IFxqService _fxqService;
        private readonly ICustomerContractConfigService _config;
        private readonly IUserEntryInsuracneRecordService _userEntry;
        private readonly IEnterpriseService _enterpriseService;
        private readonly ICaAuthIdentityService _caAuthIdentityService;



        public TestController(IFxqService fxqService, IUserEntryInsuracneRecordService userEntry, ICustomerContractConfigService config, IContractFileConfigService contractFileConfigService, IContractSignerService contractSignerService, IContractRecordService contractRecordService, IServiceScopeFactory serviceScopeFactory, IFxqDevService fxqDevService, IAliyunOSSService oSSService, IEnterpriseService enterpriseService, ICaAuthIdentityService caAuthIdentityService)
        {
            _fxqService = fxqService;
            _userEntry = userEntry;
            _config = config;
            _contractFileConfigService = contractFileConfigService;
            _contractSignerService = contractSignerService;
            _contractRecordService = contractRecordService;
            _serviceScopeFactory = serviceScopeFactory;
            _fxqDevService = fxqDevService;
            _oSSService = oSSService;
            _enterpriseService = enterpriseService;
            _caAuthIdentityService = caAuthIdentityService;
        }





        [HttpGet]
        public async Task<IActionResult> AgreeTest111()
        {

          var ss=await  _fxqService.DownloadFile("https://oss.cn-labor.cn/ICBC/Plkh/330282199309180015/front.jpg");




            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> AgreeTest()
        {

            var downRes = await _fxqService.CaAgreementApply("https://company.cn-labor.cn/web-sign/auth-identity","默认应用");
            //var downRes = await _fxqService.CaFaceApply("郑康","320722199203033956","1", "https://company.cn-labor.cn/web-sign/auth-identity", "1", "U_20260707201125670843260604416");
            var signer = await _contractSignerService.GetSigner("郑康", "320722199203033956", false);
          //var res=  await _fxqService.CaAgreementAndFaceApply("郑康", "320722199203033956", "a1", "switchTab:/pages/user/index", "a1");
            return Ok(signer);
        }


        //[HttpGet]
        //public async Task<IActionResult> SyncContraceDate()
        //{


        //    var ddt = new DateTime(2025, 09, 15);
        //    var ddtt = new DateTime(2025, 08, 01);

        //    var uList = await _contractFileConfigService.ListUserEntryView(f => f.Status == 2 && f.EntryTime.Value >=ddtt && f.ContractDate !=f.EntryTime.Value.Date);

        //    foreach (var item in uList)
        //    {
        //        var record = await _contractRecordService.Get(f => f.ContractType == ContractEnumType.入职合同 && f.UserEntryID == item.ID);
        //        var userEntry = await _contractFileConfigService.GetUserEntry1(f => f.ID == item.ID);

        //        if (record != null)
        //        {
        //            userEntry.ContractDate = record.ContractDate.Value;
        //            userEntry.ContractStartDate = record.ContractStartDate.Value;
        //            userEntry.ContractEndDate = record.ContractEndDate.Value;
        //            userEntry.RetireDate = record.RetireDate.Value;


        //            if (record.Status >= SignStatusEnumType.待存储合同)
        //            {
        //                userEntry.ContractStatus = 4;
        //            }
        //            else {
        //                userEntry.ContractStatus = 3;
        //            }
        //            await _contractFileConfigService.UpdateContract(userEntry);
        //        }
        //        else
        //        {

        //            userEntry.ContractDate = null;
        //            userEntry.ContractStartDate =null;
        //            userEntry.ContractEndDate = null;
        //            userEntry.RetireDate = null;
        //            userEntry.ContractStatus = 1;
        //            await _contractFileConfigService.UpdateContract(userEntry);

        //        }


        //    }
        //    //    List<int> entryIds = new List<int>();


        //    //var   ss= await _contractRecordService.List(f => f.ContractType== ContractEnumType.入职合同 && f.Status >  SignStatusEnumType.签署失败);

        //    //    ss= ss.OrderByDescending(x => x.CreateTime).ToList();    

        //    //    foreach (var item in ss)
        //    //    {
        //    //        if (entryIds.Contains(item.UserEntryID.Value))
        //    //        {
        //    //            continue;
        //    //        }



        //    //        entryIds.Add(item.UserEntryID.Value);

        //    //      var userEntry=await  _contractFileConfigService.GetUserEntry1(f=>f.ID==item.UserEntryID);



        //    //        if (userEntry==null)
        //    //        {
        //    //            continue;
        //    //        }

        //    //        if (item.ContractDate==null)
        //    //        {
        //    //            continue;   
        //    //        }
        //    //        Console.WriteLine(userEntry.Name);

        //    //        userEntry.ContractDate = item.ContractDate.Value;
        //    //        userEntry.ContractStartDate = item.ContractStartDate.Value;
        //    //        userEntry.ContractEndDate = item.ContractEndDate.Value;
        //    //        userEntry.RetireDate = item.RetireDate.Value;

        //    //        if (userEntry.Status == 2)
        //    //        {
        //    //            userEntry.ContractStatus = 4;
        //    //        }
        //    //        await _contractFileConfigService.UpdateContract(userEntry);
        //    //    }





        //    using (var scope = _serviceScopeFactory.CreateScope())
        //    {


        //        var sp = scope.ServiceProvider;
        //        var logger = sp.GetRequiredService<ILogger<UserEntryContractRecordEventHandler>>();
        //        var customerContractConfigService = sp.GetRequiredService<ICustomerContractConfigService>();

        //        var ddt1 = new DateTime(2025, 09, 15);
        //        var ddtt1 = new DateTime(2025, 09, 01);

        //        var entryList = await _contractFileConfigService.ListUserEntryView(f => f.ContractDate == ddt1 && f.EntryTime < ddtt1 && f.Status == 2);

        //        foreach (var entry in entryList)
        //        {


        //            var userEntryRecord = await _contractFileConfigService.GetUserEntry1(f=>f.ID==entry.ID);

        //            var customerContractConfig = await customerContractConfigService.Get(f => f.CustomerGID == userEntryRecord.CustomerGID);

        //            if (customerContractConfig==null)
        //            {
        //                continue;
        //            }


        //            ContractEnumType contractEnumType = ContractEnumType.入职合同;
        //            var ageAndGender = IdCardHelper.GetAgeAndGender(userEntryRecord.CardID.Trim());
        //            int age = ageAndGender.Age;
        //            string sex = ageAndGender.Gender;
        //            bool isRetire = false;

        //            if (sex == "男" && age >= 60)
        //            {
        //                isRetire = true;
        //            }
        //            else if (sex == "女" && age >= 50)
        //            {
        //                isRetire = true;
        //            }
        //            var retireDate = userEntryRecord.RetireDate;
        //            if (retireDate == null || retireDate.Value.Year <= 2000)
        //            {
        //                retireDate = IdCardHelper.GetRetireDate(userEntryRecord.CardID.Trim());
        //            }

        //            var record = new Live_ContractRecordExtension()
        //            {


        //                RetireDate = retireDate,
        //                ContractDate = entry.EntryTime.Value.Date,
        //                ID = Guid.NewGuid(),
        //                RelateGID = null,
        //                RelateID = userEntryRecord.ID,
        //                ContractType = ContractEnumType.入职合同,
        //                CreateTime = DateTime.Today,
        //                //TemplateOssUrl = contractFileItem.FilePath,
        //                Status = SignStatusEnumType.待生成,
        //                ContractOssUrl = string.Empty,
        //                FxqHash = string.Empty,
        //                FxqSignedOssUrl = string.Empty,
        //                FxqTransactionId = string.Empty,
        //                IsNeedKeywordReplace = false,
        //                KeywordReplaceContent = string.Empty,
        //                Remark = string.Empty,
        //                RStatus = null,
        //                SignedOssUrl = string.Empty,
        //                SignedTime = null,
        //                CustomerGID = userEntryRecord.CustomerGID,
        //                BatchNo = Guid.Empty,
        //                ContractStartDate = userEntryRecord.EntryTime.Value.Date,
        //                ContractEndDate = userEntryRecord.EntryTime.Value.Date.AddMonths(customerContractConfig.SignMonths.Value),
        //                UserName = userEntryRecord.Name,
        //                UserEntryID = userEntryRecord.ID,
        //                UserIdCardNo = userEntryRecord.CardID,
        //                UserPhone = userEntryRecord.Phone,
        //                EndDate = null,
        //                SendSmsTime = null,
        //                SigningTime = null,
        //                SmsCode = string.Empty,
        //                SmsContent = string.Empty,
        //                StartDate = null,
        //                ReadySigningDate = userEntryRecord.EntryTime.Value.Date.AddDays(customerContractConfig.DelayDays.Value),
        //                UserID = userEntryRecord.UserID,
        //                ContractMonth = customerContractConfig.SignMonths,
        //                EmpAge = age,
        //                EmpIdCardNo = userEntryRecord.CardID,
        //                EmpName = userEntryRecord.Name,
        //                EmpNo = string.Empty,
        //                EmpSex = sex,
        //                EmpSignKeyword = string.Empty,
        //                //EntryDate = userEntryRecord.EntryTime.Value.Date,
        //                IsNotice = false,
        //                IsRetire = isRetire,
        //                JobContent = string.Empty,
        //                JobName = userEntryRecord.WorkName,
        //                //LeaveDate = userEntryRecord.LeaveTime.Value.Date,
        //                TrialMonth = customerContractConfig.TrialMonths,
        //                TrialEndDate = userEntryRecord.EntryTime.Value.Date.AddMonths(customerContractConfig.TrialMonths.Value),
        //                EmpPhone = userEntryRecord.Phone,
        //                IsSuc = true,

        //            };



        //                if (record.IsRetire)
        //                {

        //                    if (record.ContractDate > record.RetireDate)
        //                    {
        //                        record.ContractEndDate = record.ContractDate;
        //                    }
        //                    else
        //                    {
        //                        record.ContractEndDate = record.RetireDate;
        //                    }

        //                    while (record.ContractEndDate <= DateTime.Today)
        //                    {
        //                        record.ContractEndDate = record.ContractEndDate.Value.AddYears(1);
        //                    }
        //                    record.ContractStartDate = record.ContractEndDate.Value.AddYears(-1);
        //                    record.TrialDescription = "0个月";
        //                    record.ContractEndDate = record.ContractEndDate.Value.AddDays(-1);
        //                }
        //                else
        //                {

        //                    record.ContractEndDate = record.ContractDate;

        //                    int times = 0;

        //                    var dt = DateTime.Now;
        //                    while (record.ContractEndDate <= dt)
        //                    {
        //                        record.ContractEndDate = record.ContractEndDate.Value.AddMonths(customerContractConfig.SignMonths.Value);
        //                    }
        //                    record.ContractStartDate = record.ContractEndDate.Value.AddMonths(-1 * customerContractConfig.SignMonths.Value);

        //                    record.ContractEndDate = record.ContractEndDate.Value.AddDays(-1);
        //                }

        //            // 判定为续签
        //            if (record.ContractStartDate.Value != record.ContractDate)
        //            {
        //                record.IsRenewal = true;
        //            }
        //            if (userEntryRecord.EntryTime.HasValue)
        //            {
        //                record.EntryDate = userEntryRecord.EntryTime.Value.Date;
        //            }
        //            if (userEntryRecord.LeaveTime.HasValue)
        //            {
        //                record.LeaveDate = userEntryRecord.LeaveTime.Value.Date;
        //            }
        //            //非续签非返聘
        //            if (record.IsRenewal == false && record.IsRetire == false)
        //            {
        //                record.TrialStartDate = record.ContractStartDate.Value;
        //                record.TrialEndDate = record.ContractStartDate.Value.AddMonths(customerContractConfig.TrialMonths.Value);
        //                record.TrialDescription = $"，其中试用期为{customerContractConfig.TrialMonths}个月，从{record.TrialStartDate.Value.ToString("yyyy-MM-dd")}起至{record.TrialEndDate.Value.ToString("yyyy-MM-dd")}止；试用期经考核合格后，甲方通知乙方办理转正手续";
        //                //record.TrialDescription = $"从{record.TrialStartDate.Value.ToString("yyyy-MM-dd")}起至{record.TrialEndDate.Value.ToString("yyyy-MM-dd")}止";
        //            }
        //            else
        //            {
        //                record.TrialDescription = "，";
        //            }

        //            userEntryRecord.ContractDate = record.ContractDate.Value;
        //            userEntryRecord.ContractStartDate = record.ContractStartDate.Value;
        //            userEntryRecord.ContractEndDate = record.ContractEndDate.Value;
        //            userEntryRecord.RetireDate = record.RetireDate.Value;
        //            userEntryRecord.ContractStatus = 4;

        //            await _contractFileConfigService.UpdateContract(userEntryRecord);



        //        }









        //    }

        //    return Ok();

        //}

        /// <summary>
        /// 批量生成合同
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DefaultContractDate()
        {

        

            List<int> userEntryIds= new List<int>() {
182  ,
192  ,
518  ,
535  ,
543  ,
549  ,
215  ,
218  ,
585  ,
623  ,
631  ,
633  ,
637  ,
645  ,
240  ,
245  ,
658  ,
688  ,
254  ,
272  ,
281  ,
283  ,
298  ,
300  ,
365  ,
387  ,
402  ,
430  ,
451  ,
33020,
703  ,
34735,
42250,
43045,
42685,
52071,
48074,
51817,
55201,
60788,
66504,
68279,
70828,
61332,
            };

            foreach (int userId in userEntryIds)
            {
                {

                    try
                    {
                        var @event = new UserEntryContractRecordEventData(userId, ContractEnumType.入职合同, Guid.NewGuid());


                        using (var scope = _serviceScopeFactory.CreateScope())
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
                                    htConfig = await contractFileConfigService.GetNav(f => f.ID ==Guid.Parse("24b31386-508c-4cbf-a204-88f5c757867f"));
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
                                    htConfig = await contractFileConfigService.GetNav(f => f.ID == Guid.Parse("23d2f0e4-5d5a-43b2-84a4-f4a5bb8c254e"));
                                }
                                else
                                {
                                    htConfig = await contractFileConfigService.GetNav(f => f.ID == customerContractConfig.LeaveFileID);
                                }
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
                                SignType = customerContractConfig.SignType,

                            };

                            if (smConfig != null)
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

                            record.ReadySigningDate = new DateTime(2026,10,1);
                            await recordService.Add(record);


                            if (record.ContractType == ContractEnumType.入职合同)
                            {
                                if (record.IsRetire)
                                {
                                    var mqMsg = new { type = "contractPendingRetireSign", Title = "正在签署返聘协议", Name = record.EmpName, CardID = record.EmpIdCardNo };
                                    sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                                }
                                else if (record.IsRenewal)
                                {
                                    var mqMsg = new { type = "contractPendingRenewalSign", Title = "正在续签合同", Name = record.EmpName, CardID = record.EmpIdCardNo };
                                    sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                                }
                                else
                                {
                                    var mqMsg = new { type = "contractPendingEntrySign", Title = "正在入职网签", Name = record.EmpName, CardID = record.EmpIdCardNo };
                                    sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                                }
                            }
                            else
                            {

                                var mqMsg = new { type = "contractPendingLeaveSign", Title = "正在离职网签", Name = record.EmpName, CardID = record.EmpIdCardNo };
                                sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("异常：" + userId);
                        throw;
                    }

                }



            }

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFile()
        {
            var ss = await _fxqService.DownloadFile("https://oss.cn-labor.cn/seals/%E8%92%8B%E6%BA%90.png");
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> CombineToPdf()
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
            dss.Add("partyA", "张三");
            dss.Add("contractStartDate", "2026-01-01");
            dss = dss.OrderBy(f => f.Key).ToDictionary();
            var downRes = await _fxqDevService.DownloadFile("https://oss.cn-labor.cn//files/20251107/5b87ffc149584e8595903b06a5cc7f83.docx");
            var res = await _fxqDevService.CombineDocxKeywordToPdf(downRes, dss);
            return Ok(res);
        }


        [HttpGet]
        public async Task<IActionResult> CombineSigning()
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
            dss.Add("partyA", "张三");
            dss.Add("contractStartDate", "2026-01-01");
            dss = dss.OrderBy(f => f.Key).ToDictionary();
            var downRes = await _fxqDevService.DownloadFile("https://lz-uplod.oss-cn-shanghai.aliyuncs.com/files/20250429/2a10317ac9f4497093b11e6ff1395cf7.docx");
            var res = await _fxqDevService.CombineDocxKeywordToPdf(downRes, dss);

           List<KeywordSigner> signers = new List<KeywordSigner>();
            var partA = new KeywordSigner("杨远伦", "530623200204012319", "https://entry.cn-labor.cn/seals/杨远伦.png", "甲方：（盖 章）",null,null);
            var partB = new KeywordSigner("嘉兴莱博友翌供应链管理有限公司余杭分公司", "91330110MA2KD3W79M", "https://entry.cn-labor.cn/seals/嘉兴莱博友翌供应链管理有限公司余杭分公司.png", "乙方：（签 章）", null,null);

            signers.Add(partA);
            signers.Add(partB);

           var ress=await _fxqDevService.CombinePdfSigning(res.Data, signers.ToArray());

            using (var ms=new MemoryStream(Convert.FromBase64String(ress.Data.PdfBase64)))
            {
               
              var ressd=      await _oSSService.UploadFile(ms, "1.pdf", true, false, "temp");

                return Ok(ressd);
            }

           

            
            return Ok(ress);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> BatchRealName()
        {

            List<string> ls = new List<string>() {

                    "贵州盛轩人力资源有限公司",
        "嘉兴鸿博供应链管理有限公司",
        "嘉兴莱博友翌供应链管理有限公司",
        "嘉兴莱博友翌供应链管理有限公司余杭分公司",
        "荆州伯乐遇马科技产业有限公司",
        "宁波观序科技有限公司",
        "宁波合流科技有限公司",
        "宁波集聘人力资源有限公司",
        "宁波璟霖供应链管理有限公司",
        "宁波莱博网络科技有限公司",
        "宁波莱博友联人力资源有限公司",
        "宁波莱博友翌供应链管理有限公司",
        "宁波莱鑫人力资源有限公司",
        "宁波蓝鑫人力资源有限公司",
        "宁波蓝招大数据科技有限公司",
        "宁波启博商务信息咨询有限公司",
        "宁波人服企业管理有限公司",
        "宁波市超越人力资源有限公司",
        "宁波新莱博金浩供应链管理有限公司",
        "宁波新莱博深蓝供应链管理有限公司",
        "宁波新莱博顺合供应链管理有限公司",
        "宁波新莱博友盟供应链管理有限公司",
        "宁波幸福职邦互联网销售有限公司",
        "宁波职联供应链管理有限公司",
        "宁波职南针人力资源有限公司",
        "宁波职引力企业服务有限公司",
        "上海虹林劳务派遣有限公司宁波分公司",
        "舜宇集团有限公司",
        "台州莱博友翌供应链管理有限公司 ",
        "蚁力（上海）人力资源有限公司",


                    };

            var recordList=await _enterpriseService.List(f => ls.Contains(f.Name));


            foreach (var item in recordList)
            {
               await _contractSignerService.GetSigner(item.Name, item.CompanyNo, true);
            }




            return Ok();
        }


    }
}
