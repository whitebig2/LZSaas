using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Customer.Service.EventHandler.FxqEventHandler;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.ModelExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Customer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class JobTaskController : ControllerBase
    {
        
        private readonly IEnterpriseService  _enterpriseService;
        private readonly ICustomerContractConfigService  _customerContractConfigService;
        private readonly IFxqDevService  _fxqDevService;
        private readonly IServiceScopeFactory  _serviceScopeFactory;
        private readonly IContractFileConfigService    _contractFileConfigService;
        private readonly IAdviceNoteService    _adviceNoteService;
        private readonly ISettingsService   _settingsService;
        private readonly IAliyunOSSService  _aliyunOSSService;
        private readonly IContractSignerService  _contractSignerService;
        private readonly IContractRecordService _recordService;
        private readonly IFxqService _fxqService;
        private readonly IEventPublisher _publisher;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICustomerService _customer;
        private readonly ICustomerOtherService _otherService;
        private readonly ISendMQService _sendMQService;
        private readonly IRedisService  _redisService;
        private readonly IContractRecordService  _contractRecordService;
        private readonly ICaAuthIdentityService  _caAuthIdentityService;


        private readonly ICashSignRecordService _cashSignRecordService;
        private readonly IJobRecommendService _jobRecommendService;



        public JobTaskController(IAliyunOSSService aliyunOSSService, IContractSignerService contractSignerService, IContractRecordService recordService, IFxqService fxqService, IEventPublisher publisher, ISettingsService settingsService, IAdviceNoteService adviceNoteService, IContractFileConfigService contractFileConfigService, IServiceScopeFactory scopeFactory, ICustomerService customerService, ICustomerOtherService otherService, IServiceScopeFactory serviceScopeFactory, ISendMQService sendMQService, IFxqDevService fxqDevService, ICustomerContractConfigService customerContractConfigService, IEnterpriseService enterpriseService, ICashSignRecordService cashSignRecordService, IContractRecordService contractRecordService, IRedisService redisService, ICaAuthIdentityService caAuthIdentityService, IJobRecommendService jobRecommendService)
        {
            _aliyunOSSService = aliyunOSSService;
            _contractSignerService = contractSignerService;
            _recordService = recordService;
            _fxqService = fxqService;
            _publisher = publisher;
            _settingsService = settingsService;
            _adviceNoteService = adviceNoteService;
            _contractFileConfigService = contractFileConfigService;
            _scopeFactory = scopeFactory;
            _customer = customerService;
            _otherService = otherService;
            _serviceScopeFactory = serviceScopeFactory;
            _sendMQService = sendMQService;
            _fxqDevService = fxqDevService;
            _customerContractConfigService = customerContractConfigService;
            _enterpriseService = enterpriseService;
            _cashSignRecordService = cashSignRecordService;
            _redisService = redisService;
            _contractRecordService = contractRecordService;
            _caAuthIdentityService = caAuthIdentityService;
            _jobRecommendService = jobRecommendService;
        }

        /// <summary>
        /// 人岗匹配夜间离线预计算：
        /// ① 增量刷新用户画像快照（Live_UserProfile，自上次计算有 Visit/Entry 变化的用户）；
        /// ② TRUNCATE 清场后逐岗位[岗位锁+事务]重建候选人清单（Live_JobCandidatePool，TopN=200/岗位）。
        /// 由调度中心（Live_Scheduler/Quartz）夜间触发，建议 cron 0 0 2 * * ?
        /// skipped=1 表示重复触发被防重入锁拦截；failedJobs>0 表示部分岗位落库失败（详见错误日志）。
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> JobRecommendPrecomputeJob()
        {
            var stats = await _jobRecommendService.PrecomputeNightly();
            return Ok(new
            {
                success = true,
                skipped = stats.GetValueOrDefault("skipped"),
                profilesUpdated = stats.GetValueOrDefault("profilesUpdated"),
                poolJobs = stats.GetValueOrDefault("poolJobs"),
                poolRows = stats.GetValueOrDefault("poolRows"),
                failedJobs = stats.GetValueOrDefault("failedJobs")
            });
        }

        /// <summary>
        /// 单岗位候选人清单实时重算（岗位新增/修改影响匹配字段后触发）：
        /// 岗位锁+事务原地替换该 JobID 的清单行；岗位不存在/已过期则清空并返回 0；
        /// rows=-1 表示夜批正在写该岗位（夜批优先，本次跳过，夜批结果即最新）。
        /// 供岗位发布/修改流程调用（MQ 消费者或直接 HTTP）。
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> JobRecommendPrecomputeJobOne(int jobID)
        {
            var rows = await _jobRecommendService.PrecomputeJob(jobID);
            return Ok(new { success = true, jobID, rows });
        }


        [HttpPut]
        public async Task<IActionResult> DevEmpSign([FromForm] int entryId= 38298)
        {

            var entry = await _contractFileConfigService.GetUserEntry(f => f.ID == entryId);
            if (entry == null)
            {
                throw new ErrorCodeException(-1, "无数据！");
            }
           var  config=await _customerContractConfigService.GetByCustomerGID(entry.CustomerGID.Value);
            if (config == null)
            {
                throw new ErrorCodeException(-1, "合同配置文件未配置！");
            }
            Live_ContractFileConfigExtension tempFile = null;
            if (entry.Status == 2)
            {
                tempFile = await _contractFileConfigService.GetNav(f => f.ID == config.EntryFileID.Value);
                if (tempFile == null)
                {
                    throw new ErrorCodeException(-1, "入职合同模板缺失！");
                }
            }
            else
            {
                tempFile = await _contractFileConfigService.GetNav(f => f.ID == config.LeaveFileID.Value);
                if (tempFile == null)
                {
                    throw new ErrorCodeException(-1, "离职合同模板缺失！");
                }
            }
         


            var docxFileBase64 = await _fxqDevService.DownloadFile(tempFile.FilePath);
            if (string.IsNullOrWhiteSpace(docxFileBase64))
            {
                throw new ErrorCodeException(-1, "合同文件下载失败");
            }

            var cus = await _customer.GetView(f => f.CustomerGID == entry.CustomerGID.Value);

            var signUnit = _enterpriseService.Get(f => f.ID == cus.SigningUnitGID);
            var empRealRecord = await _contractSignerService.Get(f => f.Idno == entry.CardID && !string.IsNullOrWhiteSpace(f.OssUrl));
            var signUnitRealRecord = await _contractSignerService.Get(f => f.Idno == cus.SigningUnitCompanyNo && !string.IsNullOrWhiteSpace(f.OssUrl));

            string empImg = string.Empty;
            string signUnitImg = string.Empty;
            if (empRealRecord!=null)
            {
                empImg = empRealRecord.OssUrl;
            }
            if (signUnitRealRecord != null)
            {
                signUnitImg = signUnitRealRecord.OssUrl;
            }
            var keywords = new Dictionary<string, string>();
            keywords.Add("PartyA", cus.SigningUnitName);
            keywords.Add("LegalPersonName", signUnit.Result.LegalPerson??"未设置");
            keywords.Add("LegalPersonAddress", cus.signingUnitAddress ?? "未设置");
            keywords.Add("PartyB", entry.Name);
            keywords.Add("Sex", entry.Sex ?? "未设置");
            keywords.Add("Phone", entry.Phone ?? "未设置");
            keywords.Add("IdCardNo", entry.CardID);
            keywords.Add("JobCity", cus.JobCity ?? "未设置");
            keywords.Add("JobName", entry.WorkName ?? "未设置");
            keywords.Add("PlaceOfResidence", entry.Address ?? "未设置");
            keywords.Add("ContractStartDate", entry.EntryTime.Value.ToString("yyyy-MM-dd"));
            keywords.Add("ContractEndDate", entry.EntryTime.Value.AddMonths(config.SignMonths.Value).ToString("yyyy-MM-dd"));
            keywords.Add("ProbationPeriod", entry.EntryTime.Value.AddMonths(config.TrialMonths.Value).ToString("yyyy-MM-dd"));
            keywords.Add("RegisteredResidence", entry.IdcardAddress ?? "未设置");
            if (entry.LeaveTime.HasValue)
            {
                keywords.Add("LeaveDate", entry.LeaveTime.Value.ToString("yyyy-MM-dd"));
            }
            keywords.Add("EducationName", entry.Education ?? "未设置");
            keywords.Add("EmergencyContact", entry.EmergencyContact ?? "未设置");
            keywords.Add("EmergencyContactPhone", entry.EmergencyContactPhone ?? "未设置");
            keywords.Add("IdCardAddress", entry.IdcardAddress);

            keywords.Add("TrialStartDate", entry.EntryTime.Value.ToString("yyyy-MM-dd"));

            keywords.Add("TrialEndDate", entry.EntryTime.Value.AddMonths(config.TrialMonths.Value).ToString("yyyy-MM-dd"));

            keywords = keywords.OrderBy(f => f.Key).Where(f => !string.IsNullOrWhiteSpace(f.Value)).ToDictionary(f => f.Key, f => f.Value);

            var contractBase64=await _fxqDevService.DocxKeywordReplace(docxFileBase64, keywords, 0);

          var pdfFile=await  _fxqDevService.DocxToPdf(contractBase64.Data);

            List<KeywordSigner> keywordSigners = new List<KeywordSigner>();

            foreach (var signer in tempFile.Signers)
            {

                if (signer.SignerPartyType== SignerPartyEnumType.第一当事人)
                {
                    keywordSigners.Add(new KeywordSigner(cus.SigningUnitName, cus.SigningUnitCompanyNo, signUnitImg, signer.SignKeyword, 120, null));
                }
                else
                {
                    keywordSigners.Add(new KeywordSigner(entry.Name, entry.CardID, empImg, signer.SignKeyword, 120, null));
                }
            }

            var signRes = await _fxqDevService.KeywordPdfContractSign(pdfFile.Data, keywordSigners.ToArray(), 1, null, null, new SignDate(1, null, null, null));

            if (signRes != null && signRes.Code == 10000)
            {
                return Ok(signRes.Data);
            }
            else {

                throw new ErrorCodeException(-1, signRes.Msg);
            }


           
        }



        /// <summary>
        /// 测试生成合同使用
        /// </summary>
        /// <param name="userEntryID"></param>
        /// <param name="isEntry"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> HandleGenerateEntryContractTest(int userEntryID, bool isEntry)
        {
            UserEntryContractRecordEventData eventData = new UserEntryContractRecordEventData(userEntryID, isEntry ? ContractEnumType.入职合同 : ContractEnumType.离职合同, Guid.NewGuid());
            _publisher.Publish(eventData);
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> HandleRealNameContractTest()
        {
            List<int> ids=new List<int>() {



27019,
38938,
45100,
45101,
26868,
47977,
45308,
49483,
49702,
49704,
49716,
50561,
49092,
51263,
51848,
51849,
52506,
53456,
53471,
53559,
55039,
55868,
51307,
56883,
56811,
58345,
59524,
60370,
60839,
60840,
60841,
61145,
61414,
61927,
61146,
61147,
61931,
61937,
61148,
61151,
61152,
61153,
61154,
63371,
63285,
63597,

            };
            foreach (var userEntryID in ids)
            {
                var exist = await _contractRecordService.Get(f => f.UserEntryID == userEntryID && f.CreateTime >= DateTime.Today);
                if (exist != null)
                {
                    continue;
                }
                UserEntryContractRecordEventData @event = new UserEntryContractRecordEventData(userEntryID, ContractEnumType.入职合同, Guid.NewGuid());
                using (var scope = _scopeFactory.CreateScope())
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
                            smConfig = await contractFileConfigService.GetNav(f => f.ID == Guid.Parse("687A37D3-89DF-43D7-8E58-D5E40E8F239A"));
                            htConfig = await contractFileConfigService.GetNav(f => f.ID == Guid.Parse("DE834B5D-4976-4F3A-81E4-206858ABEC09"));
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
                            smConfig = await contractFileConfigService.GetNav(f => f.ID == Guid.Parse("687A37D3-89DF-43D7-8E58-D5E40E8F239A"));
                            htConfig = await contractFileConfigService.GetNav(f => f.ID == Guid.Parse("61941D0E-E6B9-483E-8A66-0EAA58F1EDED"));
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

                    }
                    if (userEntryRecord.EntryTime.HasValue)
                    {
                        record.EntryDate = userEntryRecord.EntryTime.Value.Date;
                    }
                    if (userEntryRecord.LeaveTime.HasValue)
                    {
                        record.LeaveDate = userEntryRecord.LeaveTime.Value.Date;
                    }

                    record.TrialStartDate = record.ContractStartDate.Value;
                    record.TrialEndDate = record.ContractStartDate.Value.AddMonths(customerContractConfig.TrialMonths.Value);
                    record.TrialDescription = $"，其中试用期为{customerContractConfig.TrialMonths}个月，从{record.TrialStartDate.Value.ToString("yyyy-MM-dd")}起至{record.TrialEndDate.Value.ToString("yyyy-MM-dd")}止；试用期经考核合格后，甲方通知乙方办理转正手续";






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


                    //if (record.IsRetire)
                    //{

                    //}
                    //else
                    //{
                    //    record.ContractStartDate = DateTime.Parse("2026-04-01");
                    //    record.ContractEndDate = DateTime.Parse("2029-03-31");
                    //}

                    record.ReadySigningDate = DateTime.Parse("2026-10-01");
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

            return Ok();
        }


        /// <summary>
        /// 离职通知单
        /// 自动离职
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleAutoLeaveJob()
        {
            var dt1 = DateTime.Today.AddDays(-3);
            var dt2 = DateTime.Today.AddDays(-6);
            var setting = await _settingsService.GetMasterSettings();
            var recordList = await _recordService.List(f =>
            (f.AutoLeaveStatus==null|| f.AutoLeaveStatus == AutoLeaveStatusEnumType.无)
            && string.IsNullOrWhiteSpace(f.AutoLeaveFilePath) 
            && f.ContractType == ContractEnumType.离职合同
            && f.Status == SignStatusEnumType.待签署 
            && f.CreateTime < dt1&&f.CreateTime>=dt2);
            //recordList = await _recordService.List(f => f.ID == Guid.Parse("AA9E8DC9-BE3B-4704-813B-D5E459AA12C0"));
            var file = await _contractFileConfigService.Get(f => f.ContractEnumType == ContractEnumType.离职合同 && f.Name == "解除劳动合同通知书");
            foreach (var item in recordList)
            {

                if (item.CustomerName .Contains("宁波今日食品有限公司"))
                {
                  var reasonResp=await  _otherService.ListLeaveReason(item.UserEntryID.Value);

                    if (reasonResp!=null&&reasonResp.Code==200)
                    {
                        if (reasonResp.Data.Any(f=>!(f.LeaveValue.Contains("自离") || f.LeaveValue.Contains("未办离职手续"))))
                        {
                            item.AutoLeaveStatus = AutoLeaveStatusEnumType.取消通知;
                            await _recordService.Update(item);
                            continue;
                        }
                    }
                }
                try
                {
                    var kv = new Dictionary<string, string>();
                    kv.Add("PartyB", item.EmpName);
                    kv.Add("ContractStartDate", item.ContractStartDate.Value.ToString("yyyy-MM-dd"));
                    kv.Add("JobName", item.JobName);
                    kv.Add("LeaveDate", item.LeaveDate.Value.ToString("yyyy-MM-dd"));
                    kv.Add("CurrentDate", item.CreateTime.Value.ToString("yyyy-MM-dd"));
                    kv = kv.Where(f => !string.IsNullOrWhiteSpace(f.Value)).OrderBy(f => f.Key).ToDictionary();
                    var fxqResponse = await _fxqService.DocxKeywordReplace(file.FilePath, kv, 0);
                    if (fxqResponse != null && fxqResponse.Code == 10000)
                    {
                        // 转换成pdf
                        var pdfRes = await _fxqService.DocxToPdf(fxqResponse.Data);
                        if (pdfRes != null && pdfRes.Code == 1000)
                        {
                            KeywordSigner signer = new KeywordSigner(item.SigningUnitName, item.SigningUnitCompanyNo, item.SigningUnitOssUrl, "特此通知", 120, null);
                            var signResp = await _fxqService.KeywordPdfContractSign(pdfRes.Data, new KeywordSigner[] { signer }, 0, null, null, new SignDate(1, null, null, null));

                            if (signResp.Code == 10000)
                            {
                                var fileBytes = Convert.FromBase64String(signResp.Data);
                                using (var ms = new MemoryStream(fileBytes))
                                {
                                    var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
                                    if (ossResp.Code == 0)
                                    {
                                        item.AutoLeaveFilePath = setting.AliBucketDomain + ossResp.Data;
                                        var czres = await _fxqService.EvidenceFull(item.AutoLeaveFilePath, "2", string.IsNullOrWhiteSpace(item.FxqTransactionId) ? null : item.FxqTransactionId, null);
                                        if (czres != null && czres.Code == 10000)
                                        {
                                            item.FxqTransactionId = czres.Data.TransactionId;
                                            item.AutoLeaveHash = czres.Data.Hash;
                                            item.AutoLeaveStatus = AutoLeaveStatusEnumType.待短信通知;
                                        }
                                        else
                                        {
                                            item.AutoLeaveStatus = AutoLeaveStatusEnumType.生成异常;
                                        }
                                    }
                                    else
                                    {
                                        item.Remark = $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}";
                                        item.AutoLeaveStatus = AutoLeaveStatusEnumType.生成异常;
                                    }
                                }
                            }
                            else
                            {
                                item.AutoLeaveStatus = AutoLeaveStatusEnumType.生成异常;
                            }
                        }
                        else
                        {
                            item.Remark = $"合同文件Word转换成Pdf文件失败,异常信息:{fxqResponse?.Msg}";
                            item.AutoLeaveStatus = AutoLeaveStatusEnumType.生成异常;
                        }
                    }
                    else
                    {
                        item.Remark = $"合同文件关键词替换失败,异常信息:{fxqResponse?.Msg}";
                        item.AutoLeaveStatus = AutoLeaveStatusEnumType.生成异常;
                    }

                    if (item.AutoLeaveStatus == AutoLeaveStatusEnumType.待短信通知)
                    {
                        item.AutoLeaveSmsContent = $"离职合同未签署，离职通知单已生成，请点击查看 ：{item.AutoLeaveFilePath}";
                        var resp = await _fxqService.SmsSend(item.EmpPhone, item.AutoLeaveSmsContent);
                        if (resp.Code == 10000)
                        {
                            item.AutoLeaveSmsSendTime = DateTime.Now;
                            item.AutoLeaveStatus = AutoLeaveStatusEnumType.已通知;
                        }
                        else
                        {
                            item.AutoLeaveStatus = AutoLeaveStatusEnumType.短信通知失败;
                            item.AutoLeaveSmsContent = string.Empty;
                            item.AutoLeaveSmsSendTime = null;
                        }

                        //SendNoticeEventData sendNotice = new SendNoticeEventData()
                        //{
                        //    UserID = item.UserID.Value,
                        //    Keyword1 = item.SalaryMonth.Value.ToString("yyyy-MM-dd"),
                        //    Keyword2 = item.ActualSalarySendCardAmount.ToString() + "元",
                        //    Keyword3 = item.CustomerName,
                        //    NoticeType = NoticeEnumType.UserPayroll,
                        //    Platform = PlatformType.All,
                        //    SendType = NoticeSendEnumType.All
                        //};
                        //_eventPublisher.Publish(sendNotice);
                    }
                    else
                    {
                        item.AutoLeaveStatus = AutoLeaveStatusEnumType.生成异常;
                    }
                    await _recordService.Update(item);






                }
                catch (Exception ex)
                {
                    item.AutoLeaveStatus = AutoLeaveStatusEnumType.生成异常;
                    item.Remark = ex.Message;

                }

            }
            return Ok();
        }




        ///// <summary>
        ///// 离职通知单
        ///// 生成记录
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> HandleAdviceNoteJob()
        //{
        //    var dt=DateTime.Today.AddDays(-3);

        //    var setting = await _settingsService.GetMasterSettings();
        //    var recordList = await _recordService.List(f =>string.IsNullOrWhiteSpace(f.AutoLeaveFilePath)&& f.ContractType == ContractEnumType.离职合同 && f.Status == SignStatusEnumType.待签署&&f.CreateTime< dt);
        //    var list = recordList.Select(f => new Live_AdviceNote()
        //    {
        //        IdCardNo = f.EmpIdCardNo,
        //        ContractEndDate = f.ContractEndDate,
        //        ContractMonth = f.ContractMonth,
        //        ContractStartDate = f.ContractStartDate,
        //        CreateTime = DateTime.Now,
        //        CustomerGID = f.CustomerGID,
        //        CustomerName = f.CustomerName,
        //        ContractRecordID = f.ID,
        //        EntryDate = f.EntryDate,
        //        LeaveDate = f.LeaveDate,
        //        Name = f.EmpName,
        //        SigningUnitName = f.SigningUnitName,
        //        SmsContent = string.Empty,
        //        SmsSendTime = null,
        //        UserEntryID = f.UserEntryID,
        //        UserID = f.UserID,
        //        WorkName = f.JobName,
        //        WorkClass = string.Empty,
        //        Phone = f.EmpPhone,
        //        FilePath = string.Empty,
        //        Msg = string.Empty,
        //        SignedFilePath = string.Empty,
        //        Status = AdviceNoteStatusEnumType.待生成

        //    }).ToList();

        //    try
        //    {
        //        if (recordList!=null&& recordList.Count>0)
        //        {
        //            DbScoped.SugarScope.BeginTran();
        //            foreach (var item in recordList)
        //            {
        //                item.IsAdviceNote = true;
        //            }
        //            await _adviceNoteService.Add(list);
        //            await _recordService.Update(recordList);

        //            DbScoped.SugarScope.CommitTran();
        //        }
              
        //    }
        //    catch (Exception)
        //    {
        //        DbScoped.SugarScope.RollbackTran();
        //    }

        //    return Ok();
        //}

        /// <summary>
        ///  离职通知单
        ///  生成合同
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleGenerateAdviceNoteJob()
        {

            var setting = await _settingsService.GetMasterSettings();
            var exp = Expressionable.Create<Live_AdviceNote>();
            exp.And(f =>string.IsNullOrWhiteSpace(f.FilePath)&&f.Status== AdviceNoteStatusEnumType.待生成);
            var records = await _adviceNoteService.List(exp.ToExpression());
           var file=await _contractFileConfigService.Get(f => f.ContractEnumType == ContractEnumType.离职合同 && f.Name == "解除劳动合同通知书");

            foreach (var item in records)
            {

                try
                {

                    if (file == null)
                    {
                        item.Msg = "模板缺失！";
                        item.Status = AdviceNoteStatusEnumType.生成异常;
                        await _adviceNoteService.Update(item);
                        continue;
                    }
                    var kv = new Dictionary<string, string>();
                    kv.Add("PartyB", item.Name);
                    kv.Add("ContractStartDate", item.ContractStartDate.Value.ToString("yyyy-MM-dd"));
                    kv.Add("JobName", item.WorkName);
                    kv.Add("LeaveDate", item.LeaveDate.Value.ToString("yyyy-MM-dd"));
                    kv.Add("CurrentDate", item.CreateTime.Value.ToString("yyyy-MM-dd"));
                    kv = kv.Where(f => !string.IsNullOrWhiteSpace(f.Value)).OrderBy(f => f.Key).ToDictionary();
                    var fxqResponse = await _fxqService.DocxKeywordReplace(file.FilePath, kv, 0);
                    if (fxqResponse != null && fxqResponse.Code == 10000)
                    {
                        // 转换成pdf
                        var pdfRes = await _fxqService.DocxToPdf(fxqResponse.Data);
                        if (pdfRes != null && pdfRes.Code == 1000)
                        {
                            var fileBytes = Convert.FromBase64String(pdfRes.Data);
                            using (var ms = new MemoryStream(fileBytes))
                            {
                                var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
                                if (ossResp.Code == 0)
                                {
                                    item.Status = AdviceNoteStatusEnumType.待短信通知;
                                    item.FilePath = setting.AliBucketDomain + ossResp.Data;
                                    item.Msg = string.Empty;
                                }
                                else
                                {
                                    item.Msg = $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}";
                                    item.Status = AdviceNoteStatusEnumType.生成异常;
                                }
                            }
                        }
                        else
                        {
                            item.Msg = $"合同文件Word转换成Pdf文件失败,异常信息:{fxqResponse?.Msg}";
                            item.Status = AdviceNoteStatusEnumType.生成异常;
                        }
                    }
                    else
                    {
                        item.Msg = $"合同文件关键词替换失败,异常信息:{fxqResponse?.Msg}";
                        item.Status = AdviceNoteStatusEnumType.生成异常;
                    }
                    if (item.Status == AdviceNoteStatusEnumType.待短信通知)
                    {
                        item.SmsContent = $"离职合同未签署，离职通知单已生成，请点击查看 ：{item.FilePath}";


                        var resp = await _fxqService.SmsSend(item.Phone, item.SmsContent);

                        if (resp.Code == 10000)
                        {
                            item.SmsSendTime = DateTime.Now;
                            item.Status = AdviceNoteStatusEnumType.已通知;
                        }
                        else
                        {
                            item.Status = AdviceNoteStatusEnumType.短信通知失败;

                            item.SmsContent = string.Empty;
                            item.SmsSendTime = null;
                        }
                    }
                    await _adviceNoteService.Update(item);
                }
                catch (Exception ex)
                {
                    item.Status = AdviceNoteStatusEnumType.生成异常;
                    item.Msg = ex.Message;

                }

            }
            return Ok();
        }




        /// <summary>
        /// 电子合同
        /// 生成入职离职合同
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleGenerateEntryContractJob()
        {

           var setting=await _settingsService.GetMasterSettings();
            var recordList = await _recordService.List(f => f.Status == SignStatusEnumType.待生成);
            //var recordList = await _recordService.List(f => f.EmpIdCardNo== "513428200106042514");
            Dictionary<string, string> empDss = new Dictionary<string, string>();
            Dictionary<string, string> companyDss = new Dictionary<string, string>();
            // 员工身份证号
            foreach (var record in recordList)
            {
                if (!string.IsNullOrWhiteSpace(record.EmpSignKeyword) && !string.IsNullOrWhiteSpace(record.EmpIdCardNo))
                {
                    var signer = await _contractSignerService.GetSigner(record.EmpName, record.EmpIdCardNo,false);
                    if (signer != null)
                    {
                        record.EmpOssUrl = signer.OssUrl;
                    }
                }

                if (!string.IsNullOrWhiteSpace(record.SigningUnitSignKeyword) && !string.IsNullOrWhiteSpace(record.SigningUnitCompanyNo))
                {
                    var signer = await _contractSignerService.GetSigner(record.SigningUnitName, record.SigningUnitCompanyNo, true);
                    if (signer != null)
                    {
                        record.SigningUnitOssUrl = signer.OssUrl;
                    }
                }

                var keywords = new Dictionary<string, string>();
                keywords.Add("PartyA", record.SigningUnitName);
                keywords.Add("LegalPersonName", record.SigningUnitLegalPerson);
                keywords.Add("LegalPersonAddress", record.SigningUnitAddress);
                keywords.Add("PartyB", record.EmpName);
                keywords.Add("Sex", record.EmpSex);
                keywords.Add("Phone", record.EmpPhone);
                keywords.Add("IdCardNo", record.EmpIdCardNo);
                keywords.Add("JobCity", record.JobCity);
                keywords.Add("JobName", record.JobName);
                keywords.Add("PlaceOfResidence", record.HomeAddress.Replace("\r\n", "").Replace("\n", ""));
                keywords.Add("ContractStartDate", record.ContractStartDate.Value.ToString("yyyy-MM-dd"));
                keywords.Add("ContractEndDate", record.ContractEndDate.Value.ToString("yyyy-MM-dd"));
                keywords.Add("ProbationPeriod", record.TrialEndDate.Value.ToString("yyyy-MM-dd"));
                keywords.Add("RegisteredResidence", record.NativePlace);
                keywords.Add("College", record.College);
                keywords.Add("CollegeMajor", record.CollegeMajor);
                if (record.LeaveDate.HasValue)
                {
                    keywords.Add("LeaveDate", record.LeaveDate.Value.ToString("yyyy-MM-dd"));
                }
                keywords.Add("EducationName", record.Education);
                keywords.Add("EmergencyContact", record.EmergencyContactName);
                keywords.Add("EmergencyContactPhone", record.EmergencyContactPhone);
                keywords.Add("IdCardAddress", record.NativePlace);
                if (record.TrialStartDate.HasValue)
                {
                    keywords.Add("TrialStartDate", record.TrialStartDate.Value.ToString("yyyy-MM-dd"));
                }
                if (record.TrialEndDate.HasValue)
                {
                    keywords.Add("TrialEndDate", record.TrialEndDate.Value.ToString("yyyy-MM-dd"));
                }
                keywords.Add("TrialDescription", record.TrialDescription);
                if (record.RetireDate.HasValue)
                {
                    keywords.Add("RetireDate", record.RetireDate.Value.ToString("yyyy-MM-dd"));
                }


                keywords = keywords.OrderBy(f => f.Key).Where(f => !string.IsNullOrWhiteSpace(f.Value)).ToDictionary(f => f.Key, f => f.Value);
                if (keywords.Count > 0)
                {
                    // 声明关键词替换
                    if (!string.IsNullOrWhiteSpace(record.SmTemplateFilePath) && string.IsNullOrWhiteSpace(record.SmWordFilePath))
                    {
                        var fxqResponse = await _fxqService.DocxKeywordReplace(record.SmTemplateFilePath, keywords, 1);
                        if (fxqResponse != null && fxqResponse.Code == 10000)
                        {
                            record.SmWordFilePath = fxqResponse.Data;
                        }
                        else
                        {
                            record.Status = SignStatusEnumType.生成异常;
                            record.Remark += $"关键词替换失败,异常信息:{fxqResponse.Msg}";
                            record.SmWordFilePath = string.Empty;
                        }
                    }
                    // 合同关键词替换
                    if (!string.IsNullOrWhiteSpace(record.TemplateFilePath) && string.IsNullOrWhiteSpace(record.WordFilePath))
                    {
                        var fxqResponse = await _fxqService.DocxKeywordReplace(record.TemplateFilePath, keywords, 1);
                        if (fxqResponse != null && fxqResponse.Code == 10000)
                        {
                            record.WordFilePath = fxqResponse.Data;
                        }
                        else
                        {
                            record.Status = SignStatusEnumType.生成异常;
                            record.Remark += $"关键词替换失败,异常信息:{fxqResponse.Msg}";
                            record.WordFilePath = string.Empty;
                        }
                    }

                }
                if (!string.IsNullOrWhiteSpace(record.SmWordFilePath) && string.IsNullOrWhiteSpace(record.SmPdfFilePath))
                {
                    var base64 = await _fxqService.DownloadFile(record.SmWordFilePath);
                    var pdfRes = await _fxqService.DocxToPdf(base64);
                    if (pdfRes != null && pdfRes.Code == 1000)
                    {
                        var fileBytes = Convert.FromBase64String(pdfRes.Data);
                        using (var ms = new MemoryStream(fileBytes))
                        {
                            var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false, "temps");
                            if (ossResp.Code == 0)
                            {
                                record.SmPdfFilePath = setting.AliBucketDomain + ossResp.Data;
                            }
                            else
                            {
                                record.Status = SignStatusEnumType.生成异常;
                                record.IsSuc = false;
                                record.Remark = $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}";
                                record.SmPdfFilePath = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        record.Status = SignStatusEnumType.生成异常;
                        record.SmPdfFilePath = string.Empty;
                        record.Remark = $"";
                    }
                }
                if (!string.IsNullOrWhiteSpace(record.WordFilePath) && string.IsNullOrWhiteSpace(record.PdfFilePath))
                {
                    var base64 = await _fxqService.DownloadFile(record.WordFilePath);
                    var pdfRes = await _fxqService.DocxToPdf(base64);
                    if (pdfRes != null && pdfRes.Code == 1000)
                    {
                        var fileBytes = Convert.FromBase64String(pdfRes.Data);
                        using (var ms = new MemoryStream(fileBytes))
                        {
                            var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false, "temps");
                            if (ossResp.Code == 0)
                            {
                                record.PdfFilePath = setting.AliBucketDomain + ossResp.Data;
                            }
                            else
                            {
                                record.Status = SignStatusEnumType.生成异常;
                                record.IsSuc = false;
                                record.Remark = $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}";
                                record.PdfFilePath = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        record.Status = SignStatusEnumType.生成异常;
                        record.PdfFilePath = string.Empty;
                        record.Remark = $"下载文件异常";
                    }
                }

                UserContractStatusEventData eventData = new UserContractStatusEventData() { EntryID = record.UserEntryID.Value, ContractDate=record.ContractDate, ContractStartDate=record.ContractStartDate.Value,ContractEndDate=record.ContractEndDate.Value };
                if (record.Status != SignStatusEnumType.生成异常 && !string.IsNullOrWhiteSpace(record.PdfFilePath))
                {
                    record.Status = SignStatusEnumType.待发送短信;
                    await _recordService.Update(record);
                }
                else 
                {
                    record.Status = SignStatusEnumType.生成异常;
                    record.Remark = $"合同生成异常请联系管理员";
                    await _recordService.Update(record);
                }


                if (record.ContractType == ContractEnumType.入职合同)
                {
                    if (record.Status == SignStatusEnumType.待发送短信)
                    {
                        eventData.Status = ContractStatus.EntryCreate;
                    }
                    else
                    {
                        eventData.Status = ContractStatus.EntryCreateError;
                    }

                    //修改员工合同状态
                    _publisher.Publish(eventData);
                }
                else if (record.ContractType == ContractEnumType.离职合同)
                {
                    {
                        if (record.Status == SignStatusEnumType.待发送短信)
                        {
                            eventData.Status = ContractStatus.LeaveCreate;
                        }
                        else
                        {
                            eventData.Status = ContractStatus.LeaveCreateError;
                        }
                        //修改员工合同状态
                        _publisher.Publish(eventData);
                    }
                }
                

            }
            return Ok();
        }
        /// <summary>
        /// 电子合同
        /// 发送签署短信
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleEntryContractSendSmsJob()
        {

            string redisKey = "EntryContractSendSms_ShortLink_XFZ_EntryContract_";

            var recordList = await _recordService.List(f => f.Status == SignStatusEnumType.待发送短信 && f.ReadySigningDate <= DateTime.Today);
            //var  recordList = await _recordService.List(f => f.ID==Guid.Parse("0BDD869B-6CAD-427C-A75B-0C35D981F3D3"));
            foreach (var item in recordList)
            {
                if (item.SmsSendFailTimes==null)
                {
                    item.SmsSendFailTimes = 0;
                }

                if (item.ContractType== ContractEnumType.入职合同)
                {
                  var entryResp=await  _otherService.GetUserEntryRecord(item.UserEntryID.Value);
                    if (entryResp == null || entryResp.Code == 500|| entryResp.Data==null)
                    {
                        item.SmsSendFailTimes++;
                        if (item.SmsSendFailTimes >= 5)
                        {
                            item.Status = SignStatusEnumType.短信发送失败;
                            item.Remark = "员工入职信息获取失败";
                        }
                        await _recordService.Update(item);
                        continue;

                    }
                    else {

                        if (entryResp.Data.Status!=2)
                        {
                            item.Status = SignStatusEnumType.短信发送失败;
                            item.Remark = $"员工非在职状态，短信发送取消。状态：{entryResp.Data.Status}当前时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                            await _recordService.Update(item);
                            continue;
                        }
                    }
             
                }

                Random random = new Random();
                int fourDigitNumber = random.Next(1000, 10000); // 包含1000，不包含10000
                item.SmsCode= fourDigitNumber.ToString();
                item.SmsContent = $"请到微信小程序签署{item.CustomerName}的{item.ContractType.Value.ToString()}，签署验证码：{item.SmsCode}";


           


                if (item.SignType != null && item.SignType.Value == SignTypeEnumType.网页签署)
                {
                    item.SmsContent = $"{item.CustomerName}的{item.ContractType.Value.ToString()}，签署验证码：{item.SmsCode},请点击连接进行签署：https://company.cn-labor.cn/web-sign/record-detail/{item.ID}";
                }
                else {
                    //var modelView = await _recordService.GetView(f => f.ID == item.ID);
                    //var shortLink = await _redisService.GetAsync<string>($"{redisKey}_{modelView.CompanyID.Value}");
                    //if (string.IsNullOrWhiteSpace(shortLink))
                    //{
                    //    if (modelView.CompanyID != null && modelView.CompanyID.Value > 0 )
                    //    {
                    //        var res = await _otherService.GetClientShortLink(modelView.CompanyID.Value, "xfz", "pages/entryContract/index", "合同");
                    //        if (res!=null&& res.Code == 200 && res.Data!=null)
                    //        {
                    //            await _redisService.SetAsync($"{redisKey}_{modelView.CompanyID.Value}", res.Data.ToString());
                    //            shortLink = res.Data.ToString();
                    //        }
                    //    }
                    //}
                    //if (!string.IsNullOrWhiteSpace(shortLink))
                    //{
                    //    item.SmsContent = $"请到微信小程序签署{item.CustomerName}的{item.ContractType.Value.ToString()}，签署验证码：{item.SmsCode},请点击链接跳转到相关的小程序进行签署操作：{shortLink}";
                    //}
                }

                //
                var resp = await _fxqService.SmsSend(item.EmpPhone, item.SmsContent);
                if (resp.Code == 10000)
                {
                    item.SendSmsTime = DateTime.Now;
                    item.Status = SignStatusEnumType.待签署;
                    if (item.UserID.HasValue&&item.UserID.Value>0)
                    {
                        SendNoticeEventData sendNotice = new SendNoticeEventData()
                        {
                            UserID = item.UserID.Value,
                            Keyword1 = item.ContractType.ToString(),
                            Keyword2 = item.CustomerName,
                            Keyword3 = $"验证码：{item.SmsCode}",
                            NoticeType = NoticeEnumType.UserContract,
                            Platform = PlatformType.All,
                            SendType = NoticeSendEnumType.All,
                            Page = "pages/user/index"
                        };
                        _publisher.Publish(sendNotice);
                    }
                }
                else
                {
                    item.Status = SignStatusEnumType.短信发送失败;
                    item.SmsCode =string.Empty;
                    item.SmsContent = string.Empty;
                    item.SendSmsTime = null;
                }
               await _recordService.Update(item);
            }
            return Ok();
        }



        /// <summary>
        /// 电子合同
        /// 合同文件存证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleEntryContractEvidenceJob()
        {
            //var recordList = await _recordService.List(f =>string.IsNullOrWhiteSpace(f.HtHash)&&!string.IsNullOrWhiteSpace(f.SignedFilePath) );
            var recordList = await _recordService.List(f => string.IsNullOrWhiteSpace(f.HtHash) && !string.IsNullOrWhiteSpace(f.SignedFilePath)&&f.Status== SignStatusEnumType.待存证);
            foreach (var item in recordList)
            {
                // 离职则寻找入职合同
                if (item.ContractType == ContractEnumType.离职合同)
                {
                    var transRecord = await _recordService.Get(f => f.UserEntryID == item.UserEntryID && f.ContractType == ContractEnumType.入职合同 && f.Status == SignStatusEnumType.存证完成 && string.IsNullOrWhiteSpace(f.FxqTransactionId));
                    if (transRecord!=null)
                    {
                        item.FxqTransactionId = transRecord.FxqTransactionId;
                    }
                }
                // 合同文件
                if (!string.IsNullOrWhiteSpace(item.SignedFilePath)&&string.IsNullOrWhiteSpace(item.HtHash))
                {
                    var res = await _fxqService.EvidenceFull(item.SignedFilePath, "2", string.IsNullOrWhiteSpace(item.FxqTransactionId)?null: item.FxqTransactionId, null);
                    if (res != null && res.Code == 10000)
                    {
                        item.FxqTransactionId = res.Data.TransactionId;
                        item.HtHash = res.Data.Hash;

                        await _recordService.Update(item);
                    }
                }
                // 入职声明文件
                if (!string.IsNullOrWhiteSpace(item.SmSignedFilePath) && string.IsNullOrWhiteSpace(item.SmHash))
                {
                    var res = await _fxqService.EvidenceFull(item.SmSignedFilePath, "2", item.FxqTransactionId, null);
                    if (res != null && res.Code == 10000)
                    {
                        item.FxqTransactionId = res.Data.TransactionId;
                        item.SmHash = res.Data.Hash;
                        await _recordService.Update(item);
                    }
                }

                if (item.ContractType == ContractEnumType.入职合同)
                {
                    if (!string.IsNullOrWhiteSpace(item.HtHash) && !string.IsNullOrWhiteSpace(item.SmHash))
                    {
                        item.Status = SignStatusEnumType.存证完成;
                    }
                    else
                    {
                        item.Status = SignStatusEnumType.存证失败;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item.HtHash))
                    {
                        item.Status = SignStatusEnumType.存证完成;
                    }
                    else
                    {
                        item.Status = SignStatusEnumType.存证失败;
                    }
                }
                await _recordService.Update(item);
            }
            var sucList = recordList.Where(f => f.Status == SignStatusEnumType.存证完成).ToList();
            if (sucList != null && sucList.Count > 0)
            {
                var firstRecord = sucList.First();
                var mqMsg = new { type = "contractEvidenceComplete", Title = "网签存证已完成", Name= firstRecord.EmpName,CardId= firstRecord.EmpIdCardNo, RecordCount = sucList.Count };
                _sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
            }



            return Ok();
        }


        #region 新版现金签收单 

        /// <summary>
        ///  生成现金签收单并发送短息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleGenerateCashSignRecordJob()
        {
            var setting = await _settingsService.GetMasterSettings();
            var recordList = await _cashSignRecordService.List(f => f.Status== CashSignStatusEnumType.待生成||f.Status== CashSignStatusEnumType.待发送短信);

            Dictionary<string, string> dss = new Dictionary<string, string>();
            Random random = new Random();
            foreach (var record in recordList)
            {
                dss.Clear();
                dss.Add("cashType", record.CashType);
                dss.Add("customerName", record.CustomerName);
                dss.Add("employeeCardID", record.CardID);
                dss.Add("employeeName", record.Name);
                dss.Add("employeePhone", record.Phone);
                dss.Add("signUnitName", record.SignUnitName);
                if (record.Status == CashSignStatusEnumType.待生成)
                {
                    var pdfRes = await _fxqService.CombineDocxKeywordToPdf(record.TempPath, dss);
                    if (pdfRes != null && pdfRes.Code == 200 && !string.IsNullOrWhiteSpace(pdfRes.Data))
                    {
                        var fileBytes = Convert.FromBase64String(pdfRes.Data);
                        using (var ms = new MemoryStream(fileBytes))
                        {
                            var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false, "temps");
                            if (ossResp.Code == 0)
                            {
                                record.PdfPath = setting.AliBucketDomain + ossResp.Data;
                                record.Status = CashSignStatusEnumType.待发送短信;
                                record.FailMsg = string.Empty;
                            }
                            else
                            {
                                record.Status = CashSignStatusEnumType.生成异常;
                                record.FailMsg = "Oss 上传失败！";
                            }
                        }
                    }
                    else
                    {
                        record.Status = CashSignStatusEnumType.生成异常;
                        record.FailMsg = pdfRes.Msg ?? string.Empty;
                    }
                }
                if (record.Status == CashSignStatusEnumType.待发送短信 &&string.IsNullOrWhiteSpace(record.Phone))
                {
                    record.Status = CashSignStatusEnumType.短信发送异常;
                    record.FailMsg = "手机号码缺失！";
                }
                if (record.Status == CashSignStatusEnumType.待发送短信)
                {

                    if (string.IsNullOrWhiteSpace(record.SmsCode) && string.IsNullOrWhiteSpace(record.SmsContent))
                    {
                        record.SmsCode = random.Next(1000, 10000).ToString();
                        record.SmsContent = $"请到微信小程序签署现金签收单，签署验证码：{record.SmsCode}";
                    }
                    var smsRes = await _fxqService.SmsSend(record.Phone, record.SmsContent);

                    if (smsRes != null && smsRes.Code == 10000)
                    {
                        record.SmsSendTime = smsRes.Data.Time;
                        record.SmsMsgID = smsRes.Data.MsgId;
                        record.Status = CashSignStatusEnumType.待签署;
                        record.FailMsg = string.Empty;
                    }
                    else
                    {
                        record.Status = CashSignStatusEnumType.短信发送异常;
                        record.FailMsg = smsRes?.Msg ?? string.Empty;
                    }
                }
                // 更新实体信息

               
               var age= IdCardHelper.GetAge(record.CardID);
                if (age<18)
                {
                    record.SignedPath = record.PdfPath;
                    record.Status = CashSignStatusEnumType.已存证;
                    record.Remark = "年龄不满18周岁，默认签署已完成，防止卡流程！";
                    record.SignTime = DateTime.Now;
                }
                await _cashSignRecordService.Update(record);

            }
            return Ok();
        }
        /// <summary>
        /// 现金签署记录 存证任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleCashSignRecordEvidenceJob()
        {
            var recordList = await _cashSignRecordService.List(f => string.IsNullOrWhiteSpace(f.CashHash) && !string.IsNullOrWhiteSpace(f.SignedPath) && f.Status == CashSignStatusEnumType.待存证);
            foreach (var item in recordList)
            {
                var res = await _fxqService.EvidenceFull(item.SignedPath, "2", null, null);
                if (res != null && res.Code == 10000)
                {
                    item.CashHash = res.Data.Hash;
                    item.Status = CashSignStatusEnumType.已存证;
                }
                else
                {
                    item.CashHash = string.Empty;
                    item.Status = CashSignStatusEnumType.存证失败;
                }
                await _cashSignRecordService.Update(item);
            }
                return Ok();
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> HandleCaFaceFileJob()
        {
            var dt = DateTime.Today.AddDays(-3);
            var recordList = await _caAuthIdentityService.List(f =>
            f.Status == CaAuthIdentityStatusEnumType.认证成功
            && (f.AuthTime != null && f.AuthTime.Value > dt)
            && f.AuthType == "Face"
            && !string.IsNullOrWhiteSpace(f.FaceUrl)
            && !string.IsNullOrWhiteSpace(f.OrderNo)
            && string.IsNullOrWhiteSpace(f.FacePhotoPath)
            && string.IsNullOrWhiteSpace(f.FaceVideoPath)
            && string.IsNullOrWhiteSpace(f.Photo)
            && string.IsNullOrWhiteSpace(f.Video)
            && string.IsNullOrWhiteSpace(f.FaceAgeState)// 小于18周岁人员专用
            );
            foreach (var item in recordList)
            {
                var res = await _fxqService.GetCaFaceFile(item.OrderNo,item.AppName);
                if (res.Code == 10000)
                {
                        if (!string.IsNullOrWhiteSpace(res.Data.photo))
                        {
                            using (var ms = new MemoryStream(Convert.FromBase64String(res.Data.photo)))
                            {
                                var photoRes = await _aliyunOSSService.UploadOssFile(ms, $"CaFace/{item.CardID}/{item.OrderNo}.jpg");
                                item.FacePhotoPath = photoRes.Data.ToString();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(res.Data.video))
                        {
                            using (var ms = new MemoryStream(Convert.FromBase64String(res.Data.video)))
                            {
                                var videoRes = await _aliyunOSSService.UploadOssFile(ms, $"CaFace/{item.CardID}/{item.OrderNo}.mp4");
                                item.FaceVideoPath = videoRes.Data.ToString();
                            }
                        }
                        item.OriCode = res.Data.oriCode;
                        item.LiveRate = res.Data.liveRate;
                        item.Similarity = res.Data.similarity;
                        item.OccurredTime = res.Data.occurredTime;
                        item.Success = res.Data.success;
                        item.Photo = "已获取";
                        item.Video = "已获取";
                        await _caAuthIdentityService.Update(item);
                    
                }
                else
                {
                    continue;
                }
            }
            return Ok();
        }
    }
}
