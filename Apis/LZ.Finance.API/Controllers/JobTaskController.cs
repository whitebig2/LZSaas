using LZ.Clients.API;
using LZ.Model.Enums;
using LZ.Model;
using Microsoft.AspNetCore.Mvc;
using SqlSugar.IOC;
using SqlSugar;
using LZ.Finance.Service.Interface;
using NPOI.OpenXmlFormats;
using LZ.Finance.Service.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using LZ.Finance.Service.EventHandler;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Exceptions;
using NPOI.SS.Formula.Functions;
using LZ.Common;
using LZ.ApiService.Interface;
using Org.BouncyCastle.Ocsp;
using LZ.Model.FxqModel;
using System.Security.Policy;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SixLabors.ImageSharp.Drawing;
using SqlSugar.Extensions;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Dtos;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.Bank.Apip;
using LZ.Model.ModelExtensions;
using Newtonsoft.Json;
using static LZ.Finance.Service.Services.FinanceOtherService;
using Senparc.NeuChar.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LZ.Finance.API.Controllers
{
    [AllowAnonymous]
    public class JobTaskController : FinanceControllerBase
    {

        private readonly ILogger<IEventPublisher>    _logger;
        private readonly ISalaryCashSendRecordService    _cashSendRecordService;
        private readonly IEventPublisher   _eventPublisher;
        private readonly ISalarySlipService   _salarySlipService;
        private readonly ISettlementService  _settlementService;
        private readonly ISalaryCustomerService _salaryCustomerService;
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly ISalaryFailRecordService  _salaryFailRecordService;
        private readonly ISalaryDelaySendRecordService _delaySendRecordService;
        private readonly IFinanceOtherService _otherService;
        private readonly ITransBatchService  _batchService;
        private readonly ITransSingleRecordService _singleRecordService;
        private readonly IPrepaidApplyRecordService  _applyRecordService;
        private readonly IUserEntryService   _userEntryService;
        private readonly IFxqService   _fxqService;
        private readonly IAliyunOSSService    _aliyunOSSService;
        private readonly IMapper   _mapper;
        private readonly IPrepaidInterestRecordService    _interestRecordService;
        private readonly IPrepaidPartnerAccountService    _prepaidPartnerAccountService;
        private readonly IPrepaidInterestReportService    _prepaidInterestReportService;
        private readonly IPlatformNoticeService     _platformNoticeService;
        private readonly ISendMQService  _sendMQService;
        private readonly IProfitManageService  _profitManageService;
        private readonly IHttpService  _httpService;
        private readonly IUserEntryService  _entryService;
        private readonly IPrepaidSubAccounttService  _subAccounttService;
        private readonly IPaymentOrderService _paymentOrderService;

        public JobTaskController(ISalaryRecordService salaryRecordService, IFinanceOtherService otherService, ITransSingleRecordService singleRecordService, IPrepaidApplyRecordService applyRecordService, ISalaryDelaySendRecordService delaySendRecordService, IMapper mapper, ISettlementService settlementService, ISalaryFailRecordService salaryFailRecordService, ITransBatchService batchService, ISalaryCustomerService salaryCustomerService, ISalarySlipService salarySlipService, IEventPublisher eventPublisher, ILogger<IEventPublisher> logger, IUserEntryService userEntryService, ISalaryCashSendRecordService cashSendRecordService, IFxqService fxqService, IAliyunOSSService aliyunOSSService, IPrepaidInterestRecordService interestRecordService, IPrepaidPartnerAccountService prepaidPartnerAccountService, IPrepaidInterestReportService prepaidInterestReportService, IPlatformNoticeService platformNoticeService, ISendMQService sendMQService, IProfitManageService profitManageService, IHttpService httpService, IUserEntryService entryService, IPrepaidSubAccounttService subAccounttService, IPaymentOrderService paymentOrderService)
        {
            _salaryRecordService = salaryRecordService;
            _otherService = otherService;
            _singleRecordService = singleRecordService;
            _applyRecordService = applyRecordService;
            _delaySendRecordService = delaySendRecordService;
            _mapper = mapper;
            _settlementService = settlementService;
            _salaryFailRecordService = salaryFailRecordService;
            _batchService = batchService;
            _salaryCustomerService = salaryCustomerService;
            _salarySlipService = salarySlipService;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _userEntryService = userEntryService;
            _cashSendRecordService = cashSendRecordService;
            _fxqService = fxqService;
            _aliyunOSSService = aliyunOSSService;
            _interestRecordService = interestRecordService;
            _prepaidPartnerAccountService = prepaidPartnerAccountService;
            _prepaidInterestReportService = prepaidInterestReportService;
            _platformNoticeService = platformNoticeService;
            _sendMQService = sendMQService;
            _profitManageService = profitManageService;
            _httpService = httpService;
            _entryService = entryService;
            _subAccounttService = subAccounttService;
            _paymentOrderService = paymentOrderService;
        }


        #region 工资处理任务


        /// <summary>
        ///  工资记录-系统暂缓
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleSalaryDelayRecordJob()
        {

            var leaveMinDate = DateTime.Today.AddDays(-3);
            List<Live_SalaryRecord> salaryRecords = new List<Live_SalaryRecord>();
            List<Live_SalaryDelaySendRecord> delaySendRecords = new List<Live_SalaryDelaySendRecord>();
            var delayRecordList = await _delaySendRecordService.List(f => f.Status == SalaryDelaySendStatusEnumType.待系统处理);
            foreach (var item in delayRecordList)
            {
                var salaryRecord = await _salaryRecordService.Get(f => f.ID == item.SalaryRecordID);
                if (salaryRecord != null)
                {
                    if (salaryRecord.DelayType == SalaryDelaySendTypeEnumType.银行卡暂缓)
                    {
                        if (!string.IsNullOrWhiteSpace(salaryRecord.BankUserName) && !string.IsNullOrWhiteSpace(salaryRecord.Bank) && !string.IsNullOrWhiteSpace(salaryRecord.BankNo) && !string.IsNullOrWhiteSpace(salaryRecord.BankUserName))
                        {
                            delaySendRecords.Add(item);
                            salaryRecords.Add(salaryRecord);
                        }
                        else
                        {
                            var userEntry = await _userEntryService.Get(f => f.ID == salaryRecord.UserEntryID);
                            if (userEntry != null && !string.IsNullOrWhiteSpace(userEntry.BankName) && !string.IsNullOrWhiteSpace(userEntry.BankNumber))
                            {
                                item.Status = SalaryDelaySendStatusEnumType.已处理;
                                salaryRecord.Bank = userEntry.BankName;
                                salaryRecord.BankNo = userEntry.BankNumber;
                                delaySendRecords.Add(item);
                                salaryRecords.Add(salaryRecord);
                            }
                        }
                    }
                    else if (salaryRecord.DelayType == SalaryDelaySendTypeEnumType.网签暂缓)
                    {

                        var userEntry = await _userEntryService.Get(f => f.ID == salaryRecord.UserEntryID);
                        if (userEntry != null && userEntry.ContractStatus.HasValue)
                        {
                            if (userEntry.Status == 2 && userEntry.ContractStatus.Value == (int)ContractStatus.EntryComplete)
                            {
                                delaySendRecords.Add(item);
                                salaryRecords.Add(salaryRecord);
                            }
                            else if (userEntry.Status == 4 )
                            {

                                if (userEntry.ContractStatus.Value == (int)ContractStatus.LeaveComplete)
                                {
                                    delaySendRecords.Add(item);
                                    salaryRecords.Add(salaryRecord);
                                }
                                else if (userEntry.LeaveTime != null && userEntry.LeaveTime.Value.Date < leaveMinDate)
                                {
                                    delaySendRecords.Add(item);
                                    salaryRecords.Add(salaryRecord);
                                }
                               
                            }
                            else {
                                continue;
                            }
                          
                        }
                    }




                }
            }
            try
            {
                DbScoped.SugarScope.BeginTran();
                if (salaryRecords.Any())
                {
                    foreach (var item in salaryRecords)
                    {
                        item.IsDelayPaymentSuc = true;
                        _salaryRecordService.VerifyPendingSend(item);

                    }
                    await _salaryRecordService.Update(salaryRecords);
                }
                if (delaySendRecords.Any())
                {
                    foreach (var item in delaySendRecords)
                    {
                        item.Status = SalaryDelaySendStatusEnumType.已处理;
                    }
                    await _delaySendRecordService.Update(delaySendRecords);
                }
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"HandleSalaryDelayRecordJob 定时任务数据保存失败：{ex.Message}");
            }
            return Ok();
        }
        /// <summary>
        ///  工资记录-系统暂缓
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleSalaryFailRecordJob()
        {


            var failRecordList = await _salaryFailRecordService.ListView(f =>f.UserEntryID!=null&& f.UserEntryID.Value>0&& f.IsCashSend==false&& f.Status == SalaryFailRecordStatusEnumType.待处理);
            List<Live_SalaryRecord> salaryRecords = new List<Live_SalaryRecord>();
            List<Live_SalaryFailRecord> failRecords = new List<Live_SalaryFailRecord>();

            foreach (var item in failRecordList)
            {
                var userEntry = await _userEntryService.Get(f => f.ID == item.UserEntryID.Value);
                if (userEntry != null && !string.IsNullOrEmpty(userEntry.BankName) && !string.IsNullOrEmpty(userEntry.BankNumber)) {
                    //发到其他人卡里的不管
                    if (item.BankUserName==userEntry.Name)
                    {
                        if (userEntry.BankName!= item.Bank|| userEntry.BankNumber != item.BankNo)
                        {
                            var record = await _salaryRecordService.Get(f => f.ID == item.SalaryRecordID);
                            if (record != null)
                            {
                                record.IsFailSalarySuc = true;
                                record.Bank = userEntry.BankName;
                                record.BankNo = userEntry.BankNumber;
                                _salaryRecordService.VerifyPendingSend(record);
                                salaryRecords.Add(record);
                            }

                         var failRecord=await   _salaryFailRecordService.Get(f => f.ID == item.ID);
                            failRecord.Status = SalaryFailRecordStatusEnumType.已处理;
                            failRecord.ToBankUserName= item.BankUserName;
                            failRecord.ToBank= userEntry.BankName;
                            failRecord.ToBankNo= userEntry.BankNumber;
                            failRecords.Add(failRecord);
                        }
                    }
                }
            }


            try
            {
                DbScoped.SugarScope.BeginTran();
                if (salaryRecords.Any())
                {
                    await _salaryRecordService.Update(salaryRecords);
                }

                if (failRecords != null && failRecords.Any())
                {
                    await _salaryFailRecordService.Update(failRecords);
                }
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"异常工资处理定时任务异常", ex);
            }

            return Ok();
        }



        /// <summary>
        ///  工资记录-结算单生成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleSalaryRecordSettlementJob()
        {
            var recordViews = await _salaryRecordService.ListView(f => f.Status == SalaryRecordSendStatusEnumType.已发放 && f.SettlementID == null && (f.IsDelayPayment || f.IsCashSend));

            List<Live_Settlement> settlements = new List<Live_Settlement>();
            Live_Settlement settlement = null;
            foreach (var item in recordViews)
            {
                settlement = new Live_Settlement()
                {
                    ID = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    CustomerGID = item.CustomerGID.Value,
                    Remark = string.Empty,
                    SalaryCustomerID = null,
                    SalaryMonth = item.SalaryMonth,
                    SettlementType = 0,
                    Title = "派遣员工工资",
                    TotalAmount = item.ActualSalarySendCardAmount,
                    ApplyDate = DateTime.Now,
                    BusinessType = SettlementBusinessTypeEnumType.工资结算单,
                    IssuerUnitID = item.CooperationUnitID,
                    IssuerName = item.CooperationUnitCompanyName,
                    IssuerCompanyNo = item.CooperationUnitCompanyNo,
                    IssuerPhone = item.CooperationUnitCompanyPhone,
                    IssuerBank = item.CooperationUnitCompanyBank,
                    IssuerBankNo = item.CooperationUnitCompanyBankNo,
                    IssuerAddress = item.CooperationUnitCompanyAddress,
                    RecipientUnitID = item.SigningUnitID,
                    RecipientName = item.SigningUnitCompanyName,
                    RecipientPhone = item.SigningUnitCompanyPhone,
                    RecipientAddress = item.SigningUnitCompanyAddress,
                    RecipientBank = item.SigningUnitCompanyBank,
                    RecipientBankNo = item.SigningUnitCompanyBankNo,
                    RecipientCompanyNo = item.SigningUnitCompanyBankNo,
                    CustomerAddress = item.CustomerName,
                    CustomerCompanyNo = item.CustomerCompanyNo,
                    CustomerName = item.CustomerName,
                    CustomerBank = item.CustomerCompanyBank,
                    CustomerBankNo = item.CustomerCompanyBankNo,
                    CustomerPhone = item.CustomerCompanyPhone,
                };
                settlement.Description = $"{settlement.CustomerName}-{settlement.SalaryMonth.Value.ToString("yyyy年MM月")}，{item.Name}，共1名员工工资,合计{item.ActualSalarySendCardAmount}元";
                item.SettlementID = settlement.ID;
                settlements.Add(settlement);
            }


        
            DbScoped.SugarScope.BeginTran();
            var records = _mapper.Map<List<Live_SalaryRecord>>(recordViews);
            await _salaryRecordService.Update(records);
            await _settlementService.Add(settlements);
            DbScoped.SugarScope.CommitTran();
            return Ok();
        }

        ///// <summary>
        /////  工资记录-工资条通知
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> HandleSalarySlipNoticeJob()
        //{
        //    var salaryRecordSlipList = await _salaryRecordService.ListView(f => f.SalaryStatus == SalaryStatusEnumType.SucessSend && f.IsNoticeCompleted == false && f.SalarySlipRecordID != null && f.SalarySlipRecordID != Guid.Empty);
        //    var groups = salaryRecordSlipList.GroupBy(f => f.SalaryCustomerID).ToList();
        //    foreach (var groupItem in groups)
        //    {
        //        var salaryCustomer = await _salaryCustomerService.Get(groupItem.Key);
        //        if (salaryCustomer != null)
        //        {
        //            salaryCustomer.IsNoticeCompleted = true;

        //            foreach (var item in groupItem.ToList())
        //            {

        //                if (item.UserID == null || item.UserID <= 0)
        //                {
        //                    var userEntry = await _userEntryService.Get(f => f.CardID == item.IdCardNo);
        //                    if (userEntry != null)
        //                    {
        //                        item.UserID = userEntry.UserID;
        //                    }
        //                }
        //                if (item.UserID == null || item.UserID <= 0)
        //                {
        //                    continue;
        //                }
        //                SendNoticeEventData sendNotice = new SendNoticeEventData()
        //                {
        //                    UserID = item.UserID.Value,
        //                    Keyword1 = item.SalaryMonth.Value.ToString("yyyy-MM-dd"),
        //                    Keyword2 = item.ActualSalarySendCardAmount.ToString() + "元",
        //                    Keyword3 = item.CustomerName,
        //                    NoticeType = NoticeEnumType.UserPayslip,
        //                    Platform = PlatformType.All,
        //                    SendType = NoticeSendEnumType.All,
        //                     Page= "pages/user/index"
        //                };
        //                _eventPublisher.Publish(sendNotice);
        //            }
        //            await _salaryCustomerService.Update(salaryCustomer);

        //        }
        //    }
        //    return Ok();
        //}

        /// <summary>
        ///  工资记录-现金签收单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleSalaryCashJob()
        {
            //var exp = Expressionable.Create<Live_V_SalaryCashSendRecord>();
            //exp.And(f => !string.IsNullOrWhiteSpace(f.CusAgentBank) && !string.IsNullOrWhiteSpace(f.CusAgentBankNo) && !string.IsNullOrWhiteSpace(f.CusAgentName) && f.CusAgentBankNo.Length >= 6);
            //exp.And(f => f.IsCustomerPaid&&  f.SalaryCustomerStatus == (int)SalaryStatusEnumType.PendingSend|| f.SalaryCustomerStatus == (int)SalaryStatusEnumType.SucessSend || f.SalaryCustomerStatus == (int)SalaryStatusEnumType.FailSend);
            //exp.And(f => (f.Status == CashSendStatusEnumType.待处理|| f.Status == CashSendStatusEnumType.待生成签收单));
            //var recordList = await _cashSendRecordService.ListView(exp.ToExpression());

            //foreach (var item in recordList)
            //{
            //    var model = await _cashSendRecordService.Get(f => f.ID == item.ID);
            //    var salaryRecord = await _salaryRecordService.GetView(f => f.ID == item.SalaryRecordID);

            //    Live_CustomerContractConfig config = null;
            //    Live_ContractFileConfig fileConfig = null;
            //    var contractConfigResp = await _otherService.GetCustomerContractConfigByIdAsync(salaryRecord.CustomerGID.Value);
            //    if (contractConfigResp != null && contractConfigResp.Code == 200)
            //    {
            //        config = contractConfigResp.Data;
            //    }
            //    if (config != null)
            //    {
            //        var contractFileResp = await _otherService.GetContractFileConfigByIdAsync(contractConfigResp.Data.CashFileID.Value);
            //        if (contractFileResp != null && contractFileResp.Code == 200)
            //        {
            //            fileConfig = contractFileResp.Data;
            //        }
            //    }
            //    if (fileConfig != null)
            //    {
            //        Dictionary<string, string> kv = new Dictionary<string, string>();
            //        kv.Add("employeeName", item.Name);
            //        kv.Add("employeeIdCardNo", item.IdCardNo);
            //        kv.Add("employeePhone", item.Phone);
            //        kv.Add("sealEmployeeCompanyName", item.SigningUnitCompanyName);
            //        kv.Add("employeeCompanyName", item.CustomerName);
            //        kv.Add("salaryMonth", item.SalaryMonth.Value.ToString("yyyy年MM月"));
            //        kv.Add("totalAmount", item.TotalAmount.ToString());
            //        kv.Add("totalAmountWord", Tool.MoneyToUpper(item.TotalAmount.ToString()));
            //        kv = kv.Where(f => !string.IsNullOrWhiteSpace(f.Value)).OrderBy(f => f.Key).ToDictionary();
            //        var fxqResponse = await _fxqService.DocxKeywordReplace(fileConfig.FilePath, kv, 0);
            //        if (fxqResponse != null && fxqResponse.Code == 10000)
            //        {
            //            // 转换成pdf
            //            var pdfRes = await _fxqService.DocxToPdf(fxqResponse.Data);
            //            if (pdfRes != null && pdfRes.Code == 1000)
            //            {
            //                var fileBytes = Convert.FromBase64String(pdfRes.Data);
            //                using (var ms = new MemoryStream(fileBytes))
            //                {
            //                    var ossResp = await _aliyunOSSService.UploadFile(ms, "11.pdf", true, false);
            //                    if (ossResp.Code == 0)
            //                    {
            //                        model.Status = CashSendStatusEnumType.待签署;
            //                        model.FilePath = "https://oss.cn-labor.cn" + ossResp.Data;
            //                        model.Msg = string.Empty;
            //                    }
            //                    else
            //                    {
            //                        model.Remark = $"合同文件Word转换成Pdf文件成功,上传阿里云Oss失败,异常信息:{ossResp.Message}";
            //                        model.Status = CashSendStatusEnumType.签收单生成异常;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                model.Remark = $"合同文件Word转换成Pdf文件失败,异常信息:{fxqResponse?.Msg}";
            //                model.Status = CashSendStatusEnumType.签收单生成异常;
            //            }
            //        }
            //        else
            //        {
            //            model.Remark = $"合同文件Word转换成Pdf文件失败,异常信息:{fxqResponse?.Msg}";
            //            model.Status = CashSendStatusEnumType.签收单生成异常;
            //        }
            //    }
            //    else
            //    {
            //        model.Remark = $"现金签收单文件缺失";
            //        model.Status = CashSendStatusEnumType.签收单生成异常;
            //    }
            //    await _cashSendRecordService.Update(model);

            //    if (model.Status== CashSendStatusEnumType.待签署)
            //    {
            //        if (item.UserID.HasValue && item.UserID.Value > 0)
            //        {
            //            _platformNoticeService.EmpCashNotice(item.UserID.Value);
            //        }
            //        RealName2EventData eventData = new RealName2EventData(salaryRecord.Name, salaryRecord.IdCardNo);
            //        _eventPublisher.Publish(eventData);
            //    }


            
             
            //}
            return Ok();
        }



        [HttpPost]
        public async Task<IActionResult> HandlePrepaidRecordCashJob()
        {
            var exp = Expressionable.Create<Live_V_PrepaidApplyRecord>();
            exp.And(f => f.Status == PrepaidRecordStatusEnumType.待发放 && f.SendType != null && f.SendType == 1 && string.IsNullOrWhiteSpace(f.SignedFilePath) &&(f.CashStatus==null || f.CashStatus == PrepaidCashStatusEnumType.无 || f.CashStatus == PrepaidCashStatusEnumType.待生成 || f.CashStatus == PrepaidCashStatusEnumType.待签署));
            var records = await _applyRecordService.ListView(exp.ToExpression());
            Dictionary<string, string> dss = new Dictionary<string, string>();
            foreach (var item in records)
            {
                dss.Clear();
                dss.Add("name", item.Name);
                dss.Add("customerName", item.CustomerName);
                dss.Add("signUnitName", item.SigningUnitName);
                dss.Add("idCardNo", item.IdCardNo);
                var resp = await _httpService.GetAsync<Live_CashSignRecord>(ServiceNames.CustomerService, $"/api/CashSignRecord/GetSignSucRecord", dss);
                if (resp != null)
                {
                    var record = await _applyRecordService.Get(f => f.ID == item.ID);
                    if (resp.ID > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(resp.SignedPath))
                        {

                            record.CashStatus = PrepaidCashStatusEnumType.已签署;
                            record.SignedFilePath = resp.SignedPath;
                            record.PdfFilePath = resp.PdfPath;
                            await _applyRecordService.Update(record);
                        }
                        else if (!string.IsNullOrWhiteSpace(resp.PdfPath))
                        {

                            record.CashStatus = PrepaidCashStatusEnumType.待签署;
                            record.SignedFilePath = string.Empty;
                            record.PdfFilePath = resp.PdfPath;
                            await _applyRecordService.Update(record);
                        }
                    }
                    else {

                        var entry = await _entryService.GetView(f => f.ID == item.UserEntryID);

                        if (entry.UserID == null || entry.UserID.Value <= 0)
                        {
                            record.CashStatus = PrepaidCashStatusEnumType.生成异常;
                            await _applyRecordService.Update(record);
                            continue;
                        }
                        var monthEnd = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(1).AddDays(-1);
                        if (entry != null)
                        {
                            CashSignRecordDto recordDto = new CashSignRecordDto()
                            {
                                Name = entry.Name,
                                CardID = entry.CardID,
                                Sex = entry.Sex,
                                Age = entry.Age ?? 0,
                                Phone = entry.Phone,
                                CompanyID = entry.CompanyID ?? 0,
                                CompanyName = entry.CompanyName,
                                CustomerID = entry.CustomerID.Value,
                                CustomerName = entry.CustomerName,
                                PartnerID = entry.PartnerID ?? 0,
                                PartnerName = entry.PartnerName,
                                SignUnitName = entry.SignName,
                                UserEntryID = entry.ID,
                                UserID = entry.UserID ?? 0,
                                FailMsg = string.Empty,
                                CashType = "临时白名单",
                                Status = Model.Enums.CashSignStatusEnumType.待生成,
                                ExpirationDate = monthEnd,
                                SignUnitCompanyNo = entry.SignCompanyNo,
                            };

                            var res = await _httpService.PostAsync<Live_CashSignRecord>(ServiceNames.CustomerService, "/api/CashSignRecord/Add", JsonConvert.SerializeObject(recordDto));
                            await Task.Delay(100);

                            record.CashStatus = PrepaidCashStatusEnumType.待生成;

                        }
                        else {
                            record.CashStatus = PrepaidCashStatusEnumType.生成异常;
                        }
                        await _applyRecordService.Update(record);


                    }
                }

            }
            return Ok();
        }


        /// <summary>
        ///  异常工资通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleSalaryNoticeJob()
        {


            #region 员工通知


            #region 异常工资通知
            var failExp = Expressionable.Create<Live_V_SalaryFailRecord>();
            failExp.And(f => f.Status == SalaryFailRecordStatusEnumType.待处理 && f.IsNotice == false && f.UserID != null && f.UserID.Value > 0);
            var records = await _salaryFailRecordService.ListView(failExp.ToExpression());
            foreach (var item in records)
            {

                var model = await _salaryFailRecordService.Get(f => f.ID == item.ID);


                if (item.UserID.HasValue && item.UserID.Value > 0)
                {
                    _platformNoticeService.EmpSalaryFailNotice(item.UserID.Value);
                }

                model.IsNotice = true;
                await _salaryFailRecordService.Update(model);

            }
            #endregion


            #region 工资已发放通知
            var salaryRecordSendCompletedList = await _salaryRecordService.ListView(f =>  f.SalaryStatus == SalaryStatusEnumType.SucessSend&&f.ActualSalarySendCardAmount>0 && f.IsNoticeSendCompleted == false && !f.CustomerName.Contains("联营主体") );
            var sendCompletedGroups = salaryRecordSendCompletedList.GroupBy(f => f.SalaryCustomerID).ToList();
            foreach (var groupItem in sendCompletedGroups)
            {
                var salaryCustomer = await _salaryCustomerService.Get(groupItem.Key);
                if (salaryCustomer != null)
                {
                    salaryCustomer.IsNoticeSendCompleted = true;

                    foreach (var item in groupItem.ToList())
                    {
                        if (item.UserID == null || item.UserID <= 0)
                        {
                            var userEntry = await _userEntryService.Get(f => f.CardID == item.IdCardNo);
                            if (userEntry != null)
                            {
                                item.UserID = userEntry.UserID;
                            }
                        }
                        if (item.UserID == null || item.UserID <= 0)
                        {
                            continue;
                        }
                        _platformNoticeService.EmpSalaryRecordSendSucNotice(item.UserID.Value, item.CustomerName);
                        #endregion
                    }
                    await _salaryCustomerService.Update(salaryCustomer);
                    #region  通知服务商和驻场

                    var modelView = await _salaryCustomerService.GetView(salaryCustomer.ID);

                    if (modelView.PartnerID != null && modelView.PartnerID.Value > 0)
                    {
                        _platformNoticeService.ManagerSalaryCustomerSendSucNotice(modelView.PartnerID.Value, $"{modelView.CustomerName} 工资已发放");
                    }


                    #endregion




                }
            }


            #region 工资条通知
            var salaryRecordSlipList = await _salaryRecordService.ListView(f => f.ActualSalarySendCardAmount>0&& f.SalaryStatus == SalaryStatusEnumType.SucessSend && f.IsNoticeCompleted == false && f.SalarySlipRecordID != null && f.SalarySlipRecordID != Guid.Empty);
            var groups = salaryRecordSlipList.GroupBy(f => f.SalaryCustomerID).ToList();
            foreach (var groupItem in groups)
            {
                var salaryCustomer = await _salaryCustomerService.Get(groupItem.Key);
                if (salaryCustomer != null)
                {
                    salaryCustomer.IsNoticeCompleted = true;

                    foreach (var item in groupItem.ToList())
                    {

                        if (item.UserID == null || item.UserID <= 0)
                        {
                            var userEntry = await _userEntryService.Get(f => f.CardID == item.IdCardNo);
                            if (userEntry != null)
                            {
                                item.UserID = userEntry.UserID;
                            }
                        }
                        if (item.UserID == null || item.UserID <= 0)
                        {
                            continue;
                        }

                        _platformNoticeService.EmpSalarySlipNotice(item.UserID.Value,item.SalaryMonth.Value, item.ActualSalarySendCardAmount, item.CustomerName);
                    }
                    await _salaryCustomerService.Update(salaryCustomer);

                }
            }
            #endregion

            #region 预发工资已发放通知
            var prepaidExp = Expressionable.Create<Live_V_PrepaidApplyRecord>();
            prepaidExp.And(f => (f.Status == PrepaidRecordStatusEnumType.已发放||f.Status== PrepaidRecordStatusEnumType.发放失败) && f.SendTime != null && f.IsNotice == false && f.UserID != null && f.UserID.Value > 0);
            var applyRecords = await _applyRecordService.ListView(prepaidExp.ToExpression());
            foreach (var item in applyRecords)
            {

                var model = await _applyRecordService.Get(f => f.ID == item.ID);
               
                model.IsNotice = true;
                await _applyRecordService.Update(model);

                if (model.UserID!=null&&model.UserID.Value>0)
                {
                   var modelView=await _applyRecordService.GetView(f => f.ID == item.ID);

                    string msg = "预发工资发放成功";
                    if (item.Status== PrepaidRecordStatusEnumType.发放失败)
                    {
                        msg = "预发工资发放失败";
                        if (!string.IsNullOrWhiteSpace(model.Msg))
                        {
                            msg = model.Msg;
                        }
                    }
                    _platformNoticeService.EmpPrepaidSalarySendResultNotice(model.UserID.Value, modelView.Name,modelView.ApplyAmount, modelView.CreateTime.Value, msg);
                }

            }
            #endregion

            #region 工资暂缓通知
            var dalayExp = Expressionable.Create<Live_V_SalaryDelaySendRecord>();
            dalayExp.And(f => f.Status == SalaryDelaySendStatusEnumType.待系统处理 && f.IsNotice == false && f.UserID != null && f.UserID.Value > 0);
            var delaySendRecords = await _delaySendRecordService.ListView(dalayExp.ToExpression());
            foreach (var item in delaySendRecords)
            {
                var model = await _delaySendRecordService.Get(f => f.ID == item.ID);
                model.IsNotice = true;
                await _delaySendRecordService.Update(model);
                if (item.UserID != null && item.UserID.Value > 0)
                {
                    _platformNoticeService.EmpSalaryDelayNotice(item.UserID.Value, item.DelayType.ToString());
                }

            }
            #endregion


            #endregion

            #region 服务商通知

        


            #endregion

            return Ok();
        }

      


        /// <summary>
        ///  暂缓工资与异常工资 银行卡更新
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleChangeSalaryBankJob()
        {
            await HandleSalaryDelayRecordJob();
            await Task.Delay(1000);
            await HandleSalaryFailRecordJob();
            return Ok();
        }


            #endregion




            #region 民生代发
            /// <summary>
            ///  单笔代发-结果查询
            /// </summary>
            /// <returns></returns>
            [HttpPost]
        public async Task<IActionResult> HandleSingleTransDetailJob()
        {
            var transSingleRecords = await _singleRecordService.List(f => f.Status == CmbcStatusEnumType.待确认);
            foreach (var item in transSingleRecords)
            {
                try
                {
                    DbScoped.SugarScope.BeginTran();

                    item.Remark = string.Empty;
                    item.ConfirmTimes++;
                    var resp = await _otherService.SingleTransactionDetail(item.BusinessID);
                    if (resp == null)
                    {
                        item.Remark = "接口调用异常";
                    }
                    else
                    {
                        if (resp.Code == 200)
                        {
                            item.TransStatus = resp.Result.transStatus;
                            item.TransStatusName = resp.Result.transStatusDesc;
                            if (resp.Result.transStatus == "02")//发放成功
                            {
                                if (item.BusinessType == CmbcTransBusinessTypeEnumType.企业工资)
                                {
                                    var salaryRecord = await _salaryRecordService.Get(f => f.ID == item.BusinessID);
                                    salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
                                    salaryRecord.SendTime= DateTime.Now;
                                    salaryRecord.CmbcSendStatus = CmbcSendStatusEnumType.已发放;
                                    await _salaryRecordService.Update(salaryRecord);
                                }
                                else if (item.BusinessType == CmbcTransBusinessTypeEnumType.预发工资)
                                {

                                    var applyRecord = await _applyRecordService.Get(f => f.ID == item.BusinessID);
                                    applyRecord.Status = PrepaidRecordStatusEnumType.已发放;
                                    applyRecord.SendTime= item.TransTime;
                                    applyRecord.RepayStatus = PrepaidRepayStatusEnumType.待还款;
                                    if (item.TransTime==null)
                                    {
                                        applyRecord.SendTime = DateTime.Now;
                                    }
                                    await _applyRecordService.Update(applyRecord);

                                    var applyViewRecord = await _applyRecordService.GetView(f => f.ID == applyRecord.ID);

                                    if (applyRecord.ApplyType == PrepaidApplyEnumType.日结工资申请)
                                    {
                                        _sendMQService.SendStatisticsMessage("{\"type\":\"dailySalary\",\"accountPartnerName\":\"" + applyViewRecord .AccountPartnerName+ "\",\"transactionAmount\":\"" + applyViewRecord.ApplyAmount.ToString() + "\"}");
                                    }
                                    else
                                    {
                                        _sendMQService.SendStatisticsMessage("{\"type\":\"prepaidSalary\",\"accountPartnerName\":\"" + applyViewRecord.AccountPartnerName + "\",\"transactionAmount\":\"" + applyViewRecord.ApplyAmount.ToString() + "\"}");
                                    }

                                }
                                item.Status = CmbcStatusEnumType.已确认;
                            }
                            else if (resp.Result.transStatus == "03")//发放失败
                            {
                                item.Status = CmbcStatusEnumType.已确认;
                                item.TransStatusName = "交易失败";
                                // 异常原因
                                if (!string.IsNullOrWhiteSpace(resp.Result.retMsg))
                                {
                                    item.TransRemark = resp.Result.retMsg;
                                }
                                else {
                                    item.TransStatusName = string.Empty;
                                }
                             

                                if (item.BusinessType == CmbcTransBusinessTypeEnumType.企业工资)
                                {
                                    var salaryRecord = await _salaryRecordService.Get(f => f.ID == item.BusinessID);
                                    salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
                                    salaryRecord.CmbcSendStatus =  CmbcSendStatusEnumType.发放失败;
                                    salaryRecord.IsFailSalary = true;
                                    salaryRecord.IsFailSalarySuc=false;
                                    salaryRecord.TransMsg = resp.Result.retMsg;

                                    Live_SalaryFailRecord failRecord = new Live_SalaryFailRecord()
                                    {
                                        Bank = salaryRecord.Bank,
                                        BankNo = salaryRecord.BankNo,
                                        BankUserName = salaryRecord.BankUserName,
                                        CreateTime = DateTime.Now,
                                         FailType= SalaryFailTypeEnumType.其他,
                                        Description = $"单笔代发发放失败，请转现金发放:原因{item.TransRemark}",
                                        Remark = string.Empty,
                                        SalaryCustomerID = salaryRecord.SalaryCustomerID,
                                        Status = SalaryFailRecordStatusEnumType.待处理,
                                        SalaryRecordID = salaryRecord.ID,
                                    };
                                    await _salaryFailRecordService.Add(failRecord);
                                    await _salaryRecordService.Update(salaryRecord);
                                }
                                else if (item.BusinessType == CmbcTransBusinessTypeEnumType.预发工资)
                                {
                                    var applyRecord = await _applyRecordService.Get(f => f.ID == item.BusinessID);
                                    applyRecord.Status = PrepaidRecordStatusEnumType.发放失败;
                                    applyRecord.RepayStatus = PrepaidRepayStatusEnumType.无;
                                    applyRecord.Msg = resp.Result.retMsg;
                                    if (applyRecord.Msg== "本次转账金额大于虚账号可转账余额")
                                    {
                                        applyRecord.Msg = "预发额度不足，已通知财务充值。";
                                    }



                                    if (item.TransTime == null)
                                    {
                                        applyRecord.SendTime = DateTime.Now;
                                    }
                                    else {
                                        applyRecord.SendTime = item.TransTime;
                                    }
                             
                                    //applyRecord.Msg= resp.Result.
                                    await _applyRecordService.Update(applyRecord);
                                }

                            }
                            await _singleRecordService.Update(item);
                        }
                        else
                        {
                            item.Remark = resp.Message;
                        }
                    }
                    // 查询次数
                    if (item.ConfirmTimes >= 30)
                    {
                        item.Status = CmbcStatusEnumType.确认异常;
                        item.Remark = "交易确认异常，请人工确认";
                        await _singleRecordService.Update(item);
                    }
                    DbScoped.SugarScope.CommitTran();

                }
                catch (Exception ex)
                {
                    DbScoped.SugarScope.RollbackTran();
                    item.Remark = $"异常信息：{ex.Message}";
                    await _singleRecordService.Update(item);
                }
            }

            var prepaidList = transSingleRecords.Where(f => f.BusinessType == CmbcTransBusinessTypeEnumType.预发工资).ToList();
            if (prepaidList!=null&& prepaidList.Count>0)
            {
               var prepaidIdList= prepaidList.Select(f => f.BusinessID);
               var prepaidRecordList=await _applyRecordService.ListView(f => f.Status == PrepaidRecordStatusEnumType.已发放 && prepaidIdList.Contains(f.ID));

                var prepaidRecord = prepaidRecordList.FirstOrDefault();
                if (prepaidRecord!=null)
                {
                    var mqMsg = new
                    {
                        Type = "prepaidSalarySendSuccess",
                        Title = "已发放借支",
                        CustomerName=  prepaidRecord.CustomerName,
                        Name=  prepaidRecord.Name,
                       CardID= prepaidRecord.IdCardNo,
                       ApplyAmount= prepaidRecord.ApplyAmount,
                       TotalAmount= prepaidRecordList.Sum(f=>f.ApplyAmount)
                    };
                    _sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());

                    var mqMsg1 = new
                    {
                        Type = "prepaidSalaryPendingRepay",
                        Title = "借支待还款",
                        CustomerName = prepaidRecord.CustomerName,
                        Name = prepaidRecord.Name,
                        CardID = prepaidRecord.IdCardNo,
                        ApplyAmount = prepaidRecord.ApplyAmount,
                        TotalAmount = prepaidRecordList.Sum(f => f.ApplyAmount)
                    };
                    _sendMQService.SendStatisticsMessage(mqMsg1.ToCamelCaseJson());
                }


            }
            return Ok();
        }
        /// <summary>
        /// 批量代发-结果查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleBatchTransDetailJob()
        {
            var transRecords = await _batchService.ListNav(f => f.Status == CmbcStatusEnumType.待确认);
            foreach (var item in transRecords)
            {

                try
                {
                    DbScoped.SugarScope.BeginTran();
                    item.Remark = string.Empty;
                    item.ConfirmTimes++;
                    var resp = await _otherService.BatchTransactionDetail(item.BusinessID);
                    if (resp == null)
                    {
                        item.Remark = "批量交易结果查询幸福职接口调用失败";
                    }
                    else
                    {
                        if (resp.Code == 200)
                        {
                            item.TransStatus = resp.Result.transStatus;
                            item.TransStatusName = resp.Result.transStatusDesc;
                            if (resp.Result.transStatus == "02" || resp.Result.transStatus == "03")
                            {
                                item.Status = CmbcStatusEnumType.已确认;
                                foreach (var ritem in resp.Result.resultList)
                                {
                                    if (item.Records.Any(f => f.ID.ToString("N") == ritem.othSerialNo))
                                    {
                                        var sendRecord = item.Records.First(f => f.ID.ToString("N") == ritem.othSerialNo);
                                        sendRecord.TranStatus = ritem.retType;
                                        sendRecord.TranStatusName = ritem.retTypeDesc;
                                        sendRecord.TransMsg = ritem.retTypeDesc + "--" + ritem.retMsg;
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.Remark = $"批量交易结果查询幸福职接口响应异常：异常信息： {resp.Message}";
                        }
                    }
                    // 查询次数
                    if (item.ConfirmTimes >= 30 && item.Status != CmbcStatusEnumType.已确认)
                    {
                        item.Status = CmbcStatusEnumType.确认异常;
                        item.Remark = "交易确认异常，请人工确认";
                    }
                    if (item.Status == CmbcStatusEnumType.已确认)
                    {

                        var salary = await _salaryCustomerService.GetTransNav(f => f.ID == item.BusinessID);
                        if (salary != null)
                        {
                            var salaryRecords = salary.Records.Where(f => f.IsBatchSend&&!f.IsSingleSend).ToList();
                            Live_TransBatchRecord batchRecord = null;
                            foreach (var salaryRecord in salaryRecords)
                            {
                                if (item.Records.Any(f => f.BusinessID == salaryRecord.ID))
                                {
                                    batchRecord = item.Records.First(f => f.BusinessID == salaryRecord.ID);

                                    if (!string.IsNullOrWhiteSpace(batchRecord.TranStatus))
                                    {
                                        if (batchRecord.TranStatus == "1")
                                        {
                                            salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
                                            salaryRecord.SendTime = DateTime.Now;
                                        }
                                        else if (batchRecord.TranStatus == "2")
                                        {
                                            salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
                                            salaryRecord.IsFailSalary = true;
                                            salaryRecord.SendTime = DateTime.Now;
                                            salaryRecord.TransMsg = item.TransMsg;
                                            //salaryRecord.TransDescription = ; 
                                        }
                                    }
                                }

                            }


                            if (salaryRecords.Any(f => f.Status == SalaryRecordSendStatusEnumType.已发放))
                            {
                                salary.IsCmbcSendFail = false;
                                // 发放完成
                                // 同时还款记录标记完成
                                await _salaryCustomerService.BatchSendSuccess(salary);
                            }
                            else
                            {
                                foreach (var salaryRecord in salaryRecords)
                                {
                                    if (item.Records.Any(f => f.BusinessID == salaryRecord.ID))
                                    {
                                        if (salaryRecord.Status== SalaryRecordSendStatusEnumType.发放异常)
                                        {
                                            salaryRecord.Status = SalaryRecordSendStatusEnumType.待发放;
                                            salaryRecord.IsFailSalary = false;
                                            salaryRecord.TransMsg = string.Empty;
                                        }
                                    }

                                }
                                salary.IsCmbcSendFail = true;
                                await _salaryCustomerService.BatchSendFail(salary);
                            }

                        }
                        await _batchService.UpdateNav(item);

                    }
                    else
                    {
                        await _batchService.Update(item);
                    }
                    DbScoped.SugarScope.CommitTran();
                }
                catch (Exception ex)
                {
                    DbScoped.SugarScope.RollbackTran();
                    item.Remark = $"异常信息：{ex.Message}";
                    await _batchService.Update(item);
                }

            }
            return Ok();

        }
  
        /// <summary>
        /// 单笔代发-暂缓推送
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TransSingleSalaryRecordPushJob()
        {

            List<string> signUnitNameList = new List<string>() {
                "宁波莱博友翌供应链管理有限公司",
                "宁波新莱博友盟供应链管理有限公司",
                "宁波新莱博金浩供应链管理有限公司",
                "宁波蓝鑫人力资源有限公司",
                "宁波莱鑫人力资源有限公司"
            };

            var exp = Expressionable.Create<Live_TransSingleRecord>();
            exp.And(f => f.BusinessType == CmbcTransBusinessTypeEnumType.企业工资 && f.IsPrepaid == false && f.Status == CmbcStatusEnumType.待推送);
            exp.And(f => !string.IsNullOrWhiteSpace(f.Bank) && !string.IsNullOrWhiteSpace(f.BankNo) && !string.IsNullOrWhiteSpace(f.BankUserName));
            exp.And(f => signUnitNameList.Contains(f.SigningUnitName));
            var singleRecordList = await _singleRecordService.List(exp.ToExpression());



            if (singleRecordList != null && singleRecordList.Count > 0)
            {
                foreach (var record in singleRecordList)
                {
                    var salaryRecord = await _salaryRecordService.Get(f => f.ID == record.BusinessID);
                    try
                    {
                        #region 单笔交易推送到幸福职
                        // 单笔交易推送到幸福职
                        var resp = await _otherService.SingleTransaction(new SingleTransactionRequest()
                        {
                            Bank = record.Bank,
                            BankNo = record.BankNo,
                            BankUserName = record.BankUserName,
                            BusinessID = record.BusinessID,
                            CustomerName = record.CustomerName,
                            SigningUnitName = record.SigningUnitName,
                            IdCardNo = record.IdCardNo,
                            IsPrepaidSalary = record.IsPrepaid,
                            Name = record.Name,
                            TransAmount = record.TransAmount,
                        });
                        if (resp == null)
                        {
                            throw new ErrorCodeException(-1, $"单笔代发幸福职接口调用失败,响应值为NULL,请人工核对");
                        }
                        else
                        {
                            if (!resp.Result)
                            {

                                throw new ErrorCodeException(-1, $"单笔代发幸福职接口调用响应异常，异常信息：{resp.Message}");
                            }


                            DbScoped.SugarScope.BeginTran();

                            if (salaryRecord != null)
                            {
                                record.Status = CmbcStatusEnumType.待确认;
                                salaryRecord.CmbcSendStatus = CmbcSendStatusEnumType.已发放;
                                salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
                                salaryRecord.TransMsg = $"推送到幸福职系统成功！";
                                // 修改单笔发放工资状态
                                await _singleRecordService.Update(record);
                                // 修改工资记录的状态
                                await _salaryRecordService.Update(salaryRecord);
                            }
                            DbScoped.SugarScope.CommitTran();


                            //推送成功
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {

                        DbScoped.SugarScope.BeginTran();

                        if (salaryRecord != null)
                        {
                            salaryRecord.CmbcSendStatus = CmbcSendStatusEnumType.发放失败;
                            salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
                            record.Status = CmbcStatusEnumType.已确认;
                            record.TransRemark = $"推送到幸福职系统失败:{ex.Message}";
                            salaryRecord.TransMsg = $"推送到幸福职系统失败:{ex.Message}";
                            salaryRecord.IsFailSalary = true;
                            salaryRecord.IsFailSalarySuc = false;
                            Live_SalaryFailRecord failRecord = new Live_SalaryFailRecord()
                            {
                                Bank = salaryRecord.Bank,
                                BankNo = salaryRecord.BankNo,
                                BankUserName = salaryRecord.BankUserName,
                                CreateTime = DateTime.Now,
                                Description = $"单笔代发发放失败，请转现金发放:原因{record.TransRemark}",
                                Remark = string.Empty,
                                SalaryCustomerID = salaryRecord.SalaryCustomerID,
                                 FailType= SalaryFailTypeEnumType.其他,
                                Status = SalaryFailRecordStatusEnumType.待处理,
                                SalaryRecordID = salaryRecord.ID,
                            };
                            // 添加异常工资记录
                            await _salaryFailRecordService.Add(failRecord);
                            // 修改单笔发放工资状态
                            await _singleRecordService.Update(record);
                            // 修改工资记录的状态
                            await _salaryRecordService.Update(salaryRecord);
                        }
                        DbScoped.SugarScope.CommitTran();
                    }
                }
            }
            return Ok();
        }

        /// <summary>
        /// 单笔代发-预发推送
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TransSinglePrepaidRecordPushJob()
        {
            var dt = DateTime.Now.AddMinutes(-1);

            List<string> cmbcNameList = new List<string>() {
                "宁波莱博友翌供应链管理有限公司",
                "宁波新莱博友盟供应链管理有限公司",
                "宁波新莱博金浩供应链管理有限公司",
                "宁波蓝鑫人力资源有限公司",
                "宁波莱鑫人力资源有限公司"
            };

            var exp = Expressionable.Create<Live_V_PrepaidApplyRecord>();
            //exp.And(f => f.Status == PrepaidRecordStatusEnumType.待发放&& f.SendType!=null&& f.SendType==1)
            exp.And(f => f.Status == PrepaidRecordStatusEnumType.待发放);
            exp.And(f => f.SendType != null && f.SendType == 0);
            exp.And(f => !string.IsNullOrWhiteSpace(f.BankNo) && !string.IsNullOrWhiteSpace(f.Bank))
             .And(f => f.IsSinglePushSuc == false)
             .And(f => f.AuditTime != null && f.AuditTime.Value < dt)
             //.And(f => f.SigningUnitName == "宁波莱鑫人力资源有限公司" || f.SigningUnitName == "宁波蓝鑫人力资源有限公司");
             .And(f => cmbcNameList.Contains(f.SigningUnitName));

            Live_PrepaidSubAccount subAccount =null;
            SingleTransactionRequest request =null;

            var records = await _applyRecordService.ListView(exp.ToExpression());
            foreach (var record in records)
            {
                subAccount = await _subAccounttService.Get(f => f.IsEnable && f.SignUnitName == record.SigningUnitName && f.Status == PrepaidSubAccountStatusEnumType.余额充足);
                if (subAccount == null)
                {
                    continue;
                }
                if (subAccount.Balance <= subAccount.MinBalance)
                {
                    subAccount.Status = PrepaidSubAccountStatusEnumType.余额不足;
                    subAccount.LastSendTime = DateTime.Now;
                    await _subAccounttService.Update(subAccount);
                    continue;
                }

                request = null;

                var applyRecord = await _applyRecordService.Get(f => f.ID == record.ID);
                var existRecord = await _singleRecordService.Get(f => f.BusinessID == record.ID);
                if (existRecord != null)
                {
                    applyRecord.Remark = $"重复提交:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                    await _applyRecordService.Update(applyRecord);
                    continue;
                }


                if (subAccount.Balance < applyRecord.ApplyAmount)
                {
                    //subAccount.Status = PrepaidSubAccountStatusEnumType.余额不足;
                    //subAccount.LastSendTime = DateTime.Now;
                    //await _subAccounttService.Update(subAccount);
                    continue;
                }

                try
                {
                    DbScoped.SugarScope.BeginTran();
                    Live_TransSingleRecord transSingleRecord = new Live_TransSingleRecord()
                    {
                        Bank = record.Bank,
                        BankNo = record.BankNo,
                        BankUserName = record.Name,
                        BusinessID = record.ID,
                        CustomerName = record.CustomerName,
                        BusinessType = CmbcTransBusinessTypeEnumType.预发工资,
                        ConfirmTimes = 0,
                        CreateTime = DateTime.Now,
                        IdCardNo = record.IdCardNo,
                        IsPrepaid = false,
                        Name = record.Name,
                        Nonce = record.ID,
                        Remark = string.Empty,
                        SigningUnitName = record.SigningUnitName,
                        Status = CmbcStatusEnumType.待确认,
                        TransAmount = record.ApplyAmount,
                        TransStatus = string.Empty,
                        TransStatusName = string.Empty,
                        TransTime = null
                    };
                    applyRecord.IsSinglePushSuc = true;
                    applyRecord.RepayStatus = PrepaidRepayStatusEnumType.待还款;
                    subAccount.Balance -= applyRecord.ApplyAmount;
                    if (subAccount.Balance<= subAccount.MinBalance)
                    {
                        subAccount.Status = PrepaidSubAccountStatusEnumType.余额不足;
                        subAccount.LastSendTime = DateTime.Now;
                    }
                    await _applyRecordService.Update(applyRecord);
                    await _singleRecordService.Add(transSingleRecord);
                    await _subAccounttService.Update(subAccount);
                    DbScoped.SugarScope.CommitTran();
                }
                catch (Exception ex)
                {
                    DbScoped.SugarScope.RollbackTran();

                    applyRecord.Status = PrepaidRecordStatusEnumType.发放失败;
                    applyRecord.RepayStatus = PrepaidRepayStatusEnumType.无;

                    applyRecord.Msg = "生成单笔推送记录保存数据库失败";
                    await _applyRecordService.Update(applyRecord);

                    continue;
                }

                try
                {
                    #region 将发放数据推送到幸福职系统

                    request = new SingleTransactionRequest()
                    {
                        Bank = record.Bank,
                        BankNo = record.BankNo,
                        BankUserName = record.Name,
                        BusinessID = record.ID,
                        CustomerName = record.CustomerName,
                        SigningUnitName = record.SigningUnitName,
                        IdCardNo = record.IdCardNo,
                        IsPrepaidSalary = true,
                        Name = record.Name,
                        TransAmount = record.ApplyAmount,
                    };
                    var resp = await _otherService.SingleTransaction(request);
                    if (resp == null)
                    {
                        throw new ErrorCodeException(-1, "单笔代发幸福职接口调用失败，响应信息为null");
                    }
                    else
                    {
                        if (!resp.Result)
                        {
                            throw new ErrorCodeException(-1, $"单笔代发幸福职接口调用成功，响应异常,响应信息：{resp.Message}");
                        }
                    }
                    #endregion

                }
                catch (Exception ex)
                {

                    _logger.LogError($"预发工资单笔代发异常，异常信息：{ex.Message}", ex);
                    DbScoped.SugarScope.BeginTran();

                    if (ex is ErrorCodeException)
                    {
                        applyRecord.Msg = ex.Message;
                    }
                    else
                    {
                        applyRecord.Msg = "代发异常，请联系管理员查看";
                    }
                    var transSingleRecord = await _singleRecordService.Get(f => f.BusinessID == record.ID);
                    if (transSingleRecord != null)
                    {
                        await _singleRecordService.Delete(transSingleRecord.ID);
                    }
                    applyRecord.Status = PrepaidRecordStatusEnumType.发放失败;
                    applyRecord.RepayStatus = PrepaidRepayStatusEnumType.无;
                    await _applyRecordService.Update(applyRecord);
                    DbScoped.SugarScope.CommitTran();
                }
            }
            return Ok();
        }


        #endregion


        #region 技术金融服务费
        [HttpPost]
        public async Task<IActionResult> HandleInterestRecordJob()
        {
            var accountList = await _prepaidPartnerAccountService.ListView(f => !string.IsNullOrWhiteSpace(f.PartnerName)&& f.IsEnabled && f.PaymentType == (int)PrepaidPaymentEnumType.平台垫付模式);
            List<PrepaidRepayStatusEnumType> repayStatusEnumTypes = new List<PrepaidRepayStatusEnumType>() {  PrepaidRepayStatusEnumType.待还款, PrepaidRepayStatusEnumType.还款中,  };

            var currentMonth = DateTime.Today.AddDays(1- DateTime.Today.Day);
            var billMonth = currentMonth.AddMonths(-1);
            var startDate = billMonth;
            var endDate = currentMonth.AddDays(-1);
            List<Live_PrepaidInterestReport> reports = new List<Live_PrepaidInterestReport>();
            List<Live_PrepaidInterestRecord> records = new List<Live_PrepaidInterestRecord>();
            Live_PrepaidInterestRecord record = null;
            Live_PrepaidInterestReport report = null;

            foreach (var account in accountList)
            {
                // 当月还款完成记录
                var exp = Expressionable.Create<Live_V_PrepaidRepaymentRecord>();
                exp.And(f => f.SendTime <currentMonth);
                exp.And(f => f.PrepaidPartnerAccountID == account.ID);
                exp.And(f => f.RepayStatus == PrepaidRepayStatusEnumType.还款完成);
                exp.And(f => f.RepaymentTime!=null&&f.RepaymentTime.Value>=startDate);
                var repayList = await _applyRecordService.ListRepaymentListView(exp.ToExpression());

                var dt = new DateTime(2025,4,27);
                // 收取利息申请记录信息
                var applyIdList= repayList.Select(f=>f.PrepaidApplyRecordID).Distinct().ToList();
                var applyExp = Expressionable.Create<Live_V_PrepaidApplyRecord>();

                applyExp.And(f => f.SigningUnitName != "宁波新莱博金浩供应链管理有限公司");
                applyExp.And(f => f.PrepaidPartnerAccountID == account.ID);
                applyExp.And(f =>f.Status== PrepaidRecordStatusEnumType.已发放);
                applyExp.And(f => (f.SendTime == null&&f.CreateTime<currentMonth)||f.SendTime.Value<currentMonth);
                applyExp.And(f => applyIdList.Contains(f.ID)|| f.RepayStatus != PrepaidRepayStatusEnumType.还款完成);
                applyExp.AndIF(account.PartnerName == "金浩人力-俞浩飞", f => f.CreateTime >= dt);
                applyExp.AndIF(account.PartnerName == "深蓝人力-应重谊", f => f.CreateTime >= dt);
                // 获取符合条件的申请记录
                var applyList = await _applyRecordService.ListView(applyExp.ToExpression());
                if (applyList!=null&& applyList.Count>0)
                {
                    // 执行利率
                    decimal actualPercent = account.BasePercent + account.BasePercent * account.FloatPercent/100;
                    foreach (var applyRecord in applyList)
                    {

                        // 获取当前申请记录的还款记录
                        var applyRepaymentList = await _applyRecordService.ListRepaymentListView(f=>f.IsSuc&&f.RepaymentTime.Value<currentMonth&& f.PrepaidApplyRecordID==applyRecord.ID);
                        // 上月已还款的金额
                        var repaymentTotalAmount = applyRepaymentList.Where(f => f.RepaymentTime.Value.Date < startDate).Sum(f => f.TotalAmount);
                        foreach (var repaymentRecord in applyRepaymentList)
                        {
                            if (repaymentRecord.RepaymentTime.Value.Date < startDate || repaymentRecord.RepaymentTime.Value.Date > endDate)
                            {
                                continue;
                            }
                            repaymentTotalAmount += repaymentRecord.TotalAmount;
                            record = new Live_PrepaidInterestRecord()
                            {
                                SendDate = applyRecord.SendTime.HasValue? applyRecord.SendTime:applyRecord.CreateTime,
                                BillMonth = billMonth,
                                CreateTime = DateTime.Now,
                                CustomerGID = applyRecord.CustomerGID,
                                CustomerName = applyRecord.CustomerName,
                                StartDate = startDate,
                                BasePercent = account.BasePercent,
                                FloatPercent = account.FloatPercent,
                                ActualPercent = actualPercent,
                                ApplyAmount = applyRecord.ApplyAmount,
                                RepaymentAmount = repaymentRecord.TotalAmount,
                                PartnerGID = account.PartnerGID,
                                PartnerName = account.PartnerName,
                                IdCardNo = applyRecord.IdCardNo,
                                Name = applyRecord.Name,
                                EndDate = repaymentRecord.RepaymentTime.Value.Date,
                                PrepaidApplyRecordID = applyRecord.ID,
                                PrepaidPartnerAccountID = account.ID,
                                UserEntryID = applyRecord.UserEntryID,
                                UserID = applyRecord.UserID.Value,
                                PendingAmount = applyRecord.ApplyAmount - repaymentTotalAmount,
                                MainAmount = repaymentRecord.TotalAmount,
                                RepaymentDate = repaymentRecord.RepaymentTime.Value.Date,
                            };
                            records.Add(record);
                        }
                        // 账单月未还款完成
                        if (repaymentTotalAmount < applyRecord.ApplyAmount)
                        {

                           

                            record = new Live_PrepaidInterestRecord()
                            {
                                SendDate = applyRecord.SendTime?? applyRecord.AuditTime?? applyRecord.CreateTime,
                                BillMonth = billMonth,
                                CreateTime = DateTime.Now,
                                CustomerGID = applyRecord.CustomerGID,
                                CustomerName = applyRecord.CustomerName,
                                StartDate = startDate,
                                BasePercent = account.BasePercent,
                                FloatPercent = account.FloatPercent,
                                ActualPercent = actualPercent,
                                ApplyAmount = applyRecord.ApplyAmount,
                                RepaymentAmount = repaymentTotalAmount,
                                PartnerGID = account.PartnerGID,
                                PartnerName = account.PartnerName,
                                IdCardNo = applyRecord.IdCardNo,
                                Name = applyRecord.Name,
                                EndDate = endDate,
                                PrepaidApplyRecordID = applyRecord.ID,
                                PrepaidPartnerAccountID = account.ID,
                                UserEntryID = applyRecord.UserEntryID,
                                UserID = applyRecord.UserID.Value,
                                PendingAmount = applyRecord.ApplyAmount - repaymentTotalAmount,
                                MainAmount = applyRecord.ApplyAmount - repaymentTotalAmount,
                                RepaymentDate = null,
                                Remark = "当月还款未完成"

                            };



                            records.Add(record);
                        }
                    }
                }
            }

            foreach (var item in records)
            {


                if (item.SendDate != null && item.SendDate > item.StartDate)
                {
                    item.StartDate = item.SendDate;
                }



                if (item.RepaymentDate == null)
                {
                    if (item.SendDate != null && item.SendDate > item.StartDate)
                    {
                        item.StartDate = item.SendDate;
                    }

                    item.EndDate = endDate;
                }
                else
                {
                    item.EndDate = item.RepaymentDate;
                }


                item.Days = item.EndDate.Value.Day - item.StartDate.Value.Day + 1;
                item.MainInterestAmount = decimal.Round((item.MainAmount * item.Days * item.ActualPercent / (100 * 365)), 2, MidpointRounding.AwayFromZero);
            }
            var lastBillMonth = billMonth.AddMonths(-1);
            foreach (var account in accountList)
            {
                // 月统计
                report = new Live_PrepaidInterestReport()
                {
                    PartnerGID = account.PartnerGID,
                    PartnerName = account.PartnerName,
                    BasePercent = account.BasePercent,
                    FloatPercent = account.FloatPercent,
                    BillMonth = billMonth,
                    BillYear = billMonth.AddMonths(-3).Year,
                    CreateTime = DateTime.Now,
                    ID = Guid.NewGuid(),
                    InterestMonthAmount = 0,
                    InterestYearAmount = 0,
                    PendingMonthAmount = 0,
                    PendingYearAmount = 0,
                    ApplyMonthAmount = 0,
                    ApplyYearAmount = 0,
                    PrepaidPartnerAccountID = account.ID
                };
                // 月申请金额
                report.ApplyMonthAmount = records.Where(f => f.SendDate >= billMonth && f.PrepaidPartnerAccountID == account.ID).GroupBy(f => new { f.PrepaidApplyRecordID, f.ApplyAmount }).Sum(f => f.Key.ApplyAmount);
                // 月服务费
                report.InterestMonthAmount = records.Where(f => f.PrepaidPartnerAccountID == account.ID).Sum(f => f.MainInterestAmount);

                var lastBill = await _prepaidInterestReportService.Get(f => f.PrepaidPartnerAccountID == account.ID && f.BillMonth == lastBillMonth);

                if (lastBill != null)
                {
                    report.ApplyYearAmount = lastBill.ApplyYearAmount + report.ApplyMonthAmount;
                    report.InterestYearAmount = lastBill.InterestYearAmount + report.InterestMonthAmount;
                }
                else
                {
                    report.ApplyYearAmount = report.ApplyMonthAmount.Value;
                    report.InterestYearAmount = report.InterestMonthAmount;
                }
                reports.Add(report);
            }
            await _prepaidInterestReportService.Add(reports);
            await _interestRecordService.Add(records);


            return Ok();
        }

        #endregion



        #region  按月根据公司应发工资按比例转到预算金额
        [HttpPost]
        public async Task<IActionResult> HandleSalaryCustomerToBudget()
        {
            var date = DateTime.Now;
            var exp = Expressionable.Create<Model.Live_V_SalaryCustomer>();
            //exp.And(t=>t.SendTime.HasValue)
            //var model = await _salaryCustomerService.List()
            return Ok();
        }

        #endregion

        #region 清除日访问量
        [HttpPost]
        public IActionResult SalaryCalculateTimesClearJob()
        {
            _salaryCustomerService.ResetSalaryCustomerCalculateTimes();
            return Ok();
        }


        #endregion

        #region 毛利计算

        [HttpPost]
        public async Task<IActionResult> ProfitCalculateJob()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var grossMonth = currentMonth.AddMonths(-1);
            // 计算当前月毛利记录
            await _profitManageService.GenerateProfitRecord();
            await Task.Delay(1000);
            // 生成毛利管理记录
            await _profitManageService.CreateByRecords(grossMonth);
            await Task.Delay(1000);
            var manageRecords = await _profitManageService.List(f => f.ProfitMonth == grossMonth);
            foreach (var record in manageRecords)
            {
                await _profitManageService.CalculateProfit(record.ID);
            }
            return Ok();
        }


        #endregion


        #region 生成现金签署记录

        [HttpPost]
        public async Task<IActionResult> CashSignRecordJob()
        {


            //var exp = Expressionable.Create<Live_V_SalaryCashSendRecord>();
            //exp.And(f => !string.IsNullOrWhiteSpace(f.AgentBank) && !string.IsNullOrWhiteSpace(f.AgentBankNo) && !string.IsNullOrWhiteSpace(f.AgentName) && f.AgentBankNo.Length >= 6);
            //exp.And(f => f.IsCustomerPaid && (f.SalaryCustomerStatus == (int)SalaryStatusEnumType.PendingSend || f.SalaryCustomerStatus == (int)SalaryStatusEnumType.SucessSend) );
            //exp.And(f => (f.Status == CashSendStatusEnumType.待处理 || f.Status == CashSendStatusEnumType.待生成签收单));
            //exp.And(f =>f.AuditStatus== (int)SalaryCashAuditStatusEnumType.审核通过 && f.UserEntryID != null && f.UserEntryID.Value > 0 && f.UserID != null && f.UserID.Value > 0);

            //var cashRecordList = await _cashSendRecordService.ListView(exp.ToExpression());
            //foreach (var item in cashRecordList)
            //{

            //    var entry = await _userEntryService.GetView(f => f.ID == item.UserEntryID);
            //    CashSignRecordDto record = new CashSignRecordDto()
            //    {
            //        Name = item.Name,
            //        CardID = item.IdCardNo,
            //        Sex = entry.Sex,
            //        Age = entry.Age ?? 0,
            //        Phone = entry.Phone,
            //        CompanyID = entry.CompanyID ?? 0,
            //        CompanyName = entry.CompanyName,
            //        CustomerID = 2594,
            //        CustomerName = entry.CustomerName,
            //        PartnerID = entry.PartnerID ?? 0,
            //        PartnerName = entry.PartnerName,
            //        SignUnitName = entry.SignName,
            //        UserEntryID = entry.ID,
            //        UserID = entry.UserID ?? 0,
            //        FailMsg = string.Empty,
            //        CashType = "临时白名单",
            //        Status = Model.Enums.CashSignStatusEnumType.待生成,
            //        ExpirationDate = DateTime.MaxValue,
            //        SignUnitCompanyNo = entry.SignCompanyNo,
            //    };

            //    var res = await _httpService.PostAsync<Live_CashSignRecord>(ServiceNames.CustomerService, "/api/CashSignRecord/Add", JsonConvert.SerializeObject(record));
            //    if (res != null && res.Code == 200&& res.Data!=null)
            //    {
            //        if (res.Data.Status >= CashSignStatusEnumType.待存储)
            //        {
            //            item.Status = CashSendStatusEnumType.已签署;
            //            item.SucFilePath = res.Data.SignedPath;
            //            item.SignTime = res.Data.SignTime;
            //            item.Msg = string.Empty;

            //            var salaryRecord = await _salaryRecordService.Get(f => f.ID == item.SalaryRecordID);
            //            salaryRecord.IsCashSignSuc = true;
            //            _salaryRecordService.VerifyPendingSend(salaryRecord);
            //            await _cashSendRecordService.UpdateCashSignStatus(item);
            //        }
            //        else
            //        {
            //            item.Status = CashSendStatusEnumType.待签署;
            //            await _cashSendRecordService.UpdateCashSignStatus(item);
            //        }
            //    }
            //    else
            //    {
            //        item.Msg= res?.Message ?? "生成异常";
            //        item.Status = CashSendStatusEnumType.签收单生成异常;
            //        await _cashSendRecordService.UpdateCashSignStatus(item);
            //    }

            //}
            return Ok();
        }

        /// <summary>
        /// 现金签署之后，更改工资状态（新）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleSalaryCashStatusJob()
        {
            var exp = Expressionable.Create<Live_V_SalaryCashSendRecord>();
            //exp.And(f => !string.IsNullOrWhiteSpace(f.AgentBank) && !string.IsNullOrWhiteSpace(f.AgentBankNo) && !string.IsNullOrWhiteSpace(f.AgentName) && f.AgentBankNo.Length >= 6);
            exp.And(f =>f.SalaryCustomerStatus==6&& f.Status == CashSendStatusEnumType.待签署 && f.AuditStatus == 2);
            var recordList = await _cashSendRecordService.ListView(exp.ToExpression());
            var dss = new Dictionary<string, string>();

            foreach (var item in recordList)
            {
                dss.Clear();
                dss.Add("name", item.Name);
                dss.Add("cashType", item.CashType);
                dss.Add("customerName", item.CustomerName);
                dss.Add("signUnitName", item.SigningUnitCompanyName);
                dss.Add("idCardNo", item.IdCardNo);
                var resp = await _httpService.GetAsync<Live_CashSignRecord>(ServiceNames.CustomerService, $"/api/CashSignRecord/GetSignSucRecord", dss);
                if (resp != null)
                {
                    if (resp.ID > 0)
                    {
                        if (resp.Status >= CashSignStatusEnumType.待存储)
                        {
                            var model = await _cashSendRecordService.Get(f => f.ID == item.ID);
                            var salaryRecord = await _salaryRecordService.Get(f => f.ID == item.SalaryRecordID);
                            salaryRecord.IsCashSignSuc = true;
                            _salaryRecordService.VerifyPendingSend(salaryRecord);
                            model.Status = CashSendStatusEnumType.已签署;
                            model.SucFilePath = resp.SignedPath;
                            model.SignTime = resp.SignTime;
                            await _salaryRecordService.Update(salaryRecord);
                            await _cashSendRecordService.Update(model);
                        }
                    }
                    else {
                        if (item.UserEntryID != null && item.UserEntryID.Value > 0)
                        {
                            var entry = await _entryService.GetView(f => f.ID == item.UserEntryID);

                            if (entry.UserID == null || entry.UserID.Value <= 0)
                            {
                                continue;
                            }
                            var monthEnd = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(1).AddDays(-1);
                            if (entry != null)
                            {
                                CashSignRecordDto record = new CashSignRecordDto()
                                {
                                    Name = entry.Name,
                                    CardID = entry.CardID,
                                    Sex = entry.Sex,
                                    Age = entry.Age ?? 0,
                                    Phone = entry.Phone,
                                    CompanyID = entry.CompanyID ?? 0,
                                    CompanyName = entry.CompanyName,
                                    CustomerID = entry.CustomerID.Value,
                                    CustomerName = entry.CustomerName,
                                    PartnerID = entry.PartnerID ?? 0,
                                    PartnerName = entry.PartnerName,
                                    SignUnitName = entry.SignName,
                                    UserEntryID = entry.ID,
                                    UserID = entry.UserID ?? 0,
                                    FailMsg = string.Empty,
                                    CashType = item.CashType??"临时白名单",
                                    Status = Model.Enums.CashSignStatusEnumType.待生成,
                                    ExpirationDate = monthEnd,
                                    SignUnitCompanyNo = entry.SignCompanyNo,
                                };
                                var res = await _httpService.PostAsync<Live_CashSignRecord>(ServiceNames.CustomerService, "/api/CashSignRecord/Add", JsonConvert.SerializeObject(record));
                                await Task.Delay(100);
                            }
                            else
                            {
                                throw new ErrorCodeException(-1, "员工信息获取失败，无法生成现金签署记录");
                            }
                        }
                    }
                }
             

            }
            return Ok();
        }
        #endregion

        #region 财资云相关定时任务

        [HttpPost]
        public async Task<IActionResult> HandlePaymentOrderResultJob()
        {
            var list = await _paymentOrderService.ListNav(f => f.Status == PaymentOrderTransStatusEnumType.待核对支付结果);
            var orders = list.Where(f => !string.IsNullOrWhiteSpace(f.BillsPayOrderNo)).ToList();
            if (orders.Count == 0)
            {
                return Ok();
            }

            var billsPayOrderNos = orders.Select(f => f.BillsPayOrderNo).Distinct().ToList();
            QueryPaymentStatusResponseV1 data;
            try
            {
                var res = await _httpService.PostAsync<QueryPaymentStatusResponseV1>(
                    ServiceNames.BankService,
                    "/api/CzyApip/QueryPaymentStatusBatch",
                    JsonConvert.SerializeObject(billsPayOrderNos));
                if (res == null || res.Code != 200 || res.Data == null || !res.Data.IsSuccess)
                {
                    return Ok();
                }
                data = res.Data;
            }
            catch
            {
                return Ok();
            }

            foreach (var order in orders)
            {
                try
                {
                    var result = data.ResultSet?.FirstOrDefault(f => f.BillsPayOrderNo == order.BillsPayOrderNo);
                    if (result == null)
                    {
                        continue;
                    }
                    if (result.PayStatus == ((int)CzyPayStatusEnumType.支付中).ToString() || result.PayStatus == ((int)CzyPayStatusEnumType.未知).ToString())
                    {
                        continue;
                    }
                    order.PayStatus = result.PayStatus;
                    order.PayStatusName = result.PayStatusName;
                    order.RelPaymentDate = result.RelPaymentDate;
                    order.FailReasons = result.FailReasons;
                    order.PayFailReason = result.PayFailReason;
                    order.RefundAmount = result.RefundAmount;
                    order.RefundDate = result.RefundDate;

                    if (result.PayStatus == ((int)CzyPayStatusEnumType.支付成功).ToString() || result.PayStatus == ((int)CzyPayStatusEnumType.部分支付成功).ToString())
                    {
                        order.TransTime = DateTime.Now;
                        order.Status = PaymentOrderTransStatusEnumType.支付成功;
                    }
                    else if (result.PayStatus == ((int)CzyPayStatusEnumType.支付失败).ToString())
                    {
                        order.Status = PaymentOrderTransStatusEnumType.支付失败;
                        order.SetTransFail();
                    }
                    else if (result.PayStatus == ((int)CzyPayStatusEnumType.已退款).ToString() || result.PayStatus == ((int)CzyPayStatusEnumType.部分退款).ToString())
                    {
                        order.Status = PaymentOrderTransStatusEnumType.待财务确认支付结果;
                    }

                    if (order.Records != null && result.BatchList != null && result.BatchList.Count>0)
                    {
                        foreach (var detail in order.Records)
                        {
                            var batch = result.BatchList.FirstOrDefault(f => f.BillsPayOrderNo == detail.BillsPayOrderNo);
                            if (batch == null)
                            {
                                continue;
                            }
                            detail.PayStatus = batch.PayStatus;
                            detail.PayStatusName = batch.PayStatusName;
                            detail.FailReasons = batch.FailReasons;
                            detail.RelPaymentDate = batch.RelPaymentDate;
                            detail.PayFailReason = batch.PayFailReason;
                            detail.RefundAmount = batch.RefundAmount;
                            detail.RefundDate = batch.RefundDate;

                            if (detail.PayStatus == ((int)CzyPayStatusEnumType.支付成功).ToString() || detail.PayStatus == ((int)CzyPayStatusEnumType.部分支付成功).ToString())
                            {
                                detail.TransTime = DateTime.Now;
                                detail.Status = PaymentOrderTransStatusEnumType.支付成功;
                            }
                            else if (detail.PayStatus == ((int)CzyPayStatusEnumType.支付失败).ToString())
                            {
                                detail.Status = PaymentOrderTransStatusEnumType.支付失败;
                            }
                            else if (detail.PayStatus == ((int)CzyPayStatusEnumType.已退款).ToString() || detail.PayStatus == ((int)CzyPayStatusEnumType.部分退款).ToString())
                            {
                                detail.Status = PaymentOrderTransStatusEnumType.待财务确认支付结果;
                            }
                        }
                        // 根据记录状态更新订单状态
                        order.HandlePayment();
                    }
                    order.UpdateTime = DateTime.Now;
                    await _paymentOrderService.UpdateNav(order);
                }
                catch
                {
                    continue;
                }
            }


            // 将发放结果回传到对应业务
            foreach (var order in orders)
            {
                if (order.Status != PaymentOrderTransStatusEnumType.支付成功&& order.Status != PaymentOrderTransStatusEnumType.支付失败)
                {
                    continue;
                }
                try
                {
                    switch (order.DataType)
                    {
                        case TransactionDataTypeEnumType.客户工资:
                            await SyncPaymentResultToSalaryCustomer(order);
                            break;
                        case TransactionDataTypeEnumType.预发工资:
                            await SyncPaymentResultToPrepaid(order);
                            break;
                        case TransactionDataTypeEnumType.暂缓异常工资:
                            await SyncPaymentResultToFailSalary(order);
                            break;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> HandlePrepaidPaymentOrderJob()
        {

            throw new ErrorCodeException(-1,"预发财资云渠道未启用");

            var records = await _applyRecordService.ListView(f =>
                f.Name == "李崇亨"&&
                f.Status == PrepaidRecordStatusEnumType.待发放
                && !f.IsSinglePushSuc
                && f.ApplyAmount > 0
                && !string.IsNullOrWhiteSpace(f.Name)
                && !string.IsNullOrWhiteSpace(f.IdCardNo)
                && !string.IsNullOrWhiteSpace(f.Bank)
                && !string.IsNullOrWhiteSpace(f.BankNo)
                && !string.IsNullOrWhiteSpace(f.SigningUnitName)
                && !string.IsNullOrWhiteSpace(f.CustomerName));
            foreach (var record in records)
            {
                var businessId = record.ID.ToString();
                if (await _paymentOrderService.Exist(f => f.BusinessID == businessId
                    && f.Status != PaymentOrderTransStatusEnumType.支付失败
                    && f.Status != PaymentOrderTransStatusEnumType.取消支付))
                {
                    continue;
                }

                var addDto = new PaymentOrderAddDto
                {
                    CompanyID = record.CompanyID.HasValue && record.CompanyID.Value > 0
                        ? record.CompanyID.Value
                        : CurrentUser.CompanyID,
                    SigningUnitName = "宁波璟霖供应链管理有限公司",
                    CustomerName = record.CustomerName,
                    CustomerNo = record.CustomerGID.ToString(),
                    BusinessID = businessId,
                    SalaryType = 0,
                    DataType = TransactionDataTypeEnumType.预发工资,
                    PayUse = "预发工资",
                    Purpose = "预发工资",
                    Remark = record.Remark ?? string.Empty,
                    PaymentDate = DateTime.Today,
                    Records = new List<PaymentOrderDetailAddDto>
                    {
                        new PaymentOrderDetailAddDto
                        {
                            BusinessID = businessId,
                            Name = record.Name,
                            CardID = record.IdCardNo,
                            BankName = record.Bank,
                            BankNo = record.BankNo,
                            TransAmount = record.ApplyAmount,
                        }
                    }
                };

                try
                {
                    DbScoped.SugarScope.BeginTran();
                    await _paymentOrderService.Add(addDto);
                    DbScoped.SugarScope.CommitTran();
                }
                catch
                {
                    DbScoped.SugarScope.RollbackTran();
                }
            }
            return Ok();
        }

        private async Task SyncPaymentResultToSalaryCustomer(Live_PaymentOrderExtension order)
        {

            if (order.Status != PaymentOrderTransStatusEnumType.支付成功 && order.Status != PaymentOrderTransStatusEnumType.支付失败)
            {
                return;
            }

            if (!Guid.TryParse(order.BusinessID, out var salaryCustomerId) || salaryCustomerId == Guid.Empty)
            {
                return;
            }

            var salaryNav = await _salaryCustomerService.GetTransNav(f => f.ID == salaryCustomerId);
            if (salaryNav == null || salaryNav.Status != SalaryStatusEnumType.PendingSend)
            {
                return;
            }

            salaryNav.SendTime = order.TransTime ?? DateTime.Now;
            salaryNav.TransStatus = order.Status.ToString();
            salaryNav.TransStatusName = order.Status.ToString();
            salaryNav.TransMsg = string.IsNullOrWhiteSpace(order.PayFailReason) ? order.FailReasons : order.PayFailReason;
            salaryNav.PaymentStatus = order.Status == PaymentOrderTransStatusEnumType.支付成功
                ? SalaryPaymentStatusEnumType.支付成功
                : SalaryPaymentStatusEnumType.支付失败;

            if (order.Records != null && salaryNav.Records != null)
            {
                foreach (var detail in order.Records)
                {
                    if (!Guid.TryParse(detail.BusinessID, out var salaryRecordId) || salaryRecordId == Guid.Empty)
                    {
                        continue;
                    }
                    var salaryRecord = salaryNav.Records.FirstOrDefault(f => f.ID == salaryRecordId);
                    if (salaryRecord == null)
                    {
                        continue;
                    }

                    salaryRecord.SendTime = detail.TransTime ?? order.TransTime ?? DateTime.Now;
                    salaryRecord.TransStatus = detail.Status.ToString();
                    salaryRecord.TransStatusName = detail.Status.ToString();
                    salaryRecord.TransMsg = string.IsNullOrWhiteSpace(detail.PayFailReason) ? detail.FailReasons : detail.PayFailReason;

                    if (detail.Status == PaymentOrderTransStatusEnumType.支付成功)
                    {
                        salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
                        salaryRecord.IsSendSuccess = true;
                        salaryRecord.IsFailSalary = false;
                    }
                    else if (detail.Status == PaymentOrderTransStatusEnumType.支付失败)
                    {
                        salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
                        salaryRecord.IsFailSalary = true;
                    }
                }
            }

            if (order.Status == PaymentOrderTransStatusEnumType.支付成功)
            {
                await _salaryCustomerService.BatchSendSuccess(salaryNav);
            }
            else
            {
                await _salaryCustomerService.BatchSendFail(salaryNav);
            }
        }

        private async Task SyncPaymentResultToPrepaid(Live_PaymentOrderExtension order)
        {

            if (order.Status != PaymentOrderTransStatusEnumType.支付成功 && order.Status != PaymentOrderTransStatusEnumType.支付失败)
            {
                return;
            }

            if (!Guid.TryParse(order.BusinessID, out var prepaidId) || prepaidId == Guid.Empty)
            {
                return;
            }

            var applyRecord = await _applyRecordService.Get(f => f.ID == prepaidId);
            if (applyRecord == null || applyRecord.Status != PrepaidRecordStatusEnumType.待发放)
            {
                return;
            }

            applyRecord.SendTime = order.TransTime ?? DateTime.Now;
            if (order.Status == PaymentOrderTransStatusEnumType.支付成功)
            {
                applyRecord.Status = PrepaidRecordStatusEnumType.已发放;
                applyRecord.RepayStatus = PrepaidRepayStatusEnumType.待还款;
                applyRecord.Msg = string.Empty;
            }
            else
            {
                applyRecord.Status = PrepaidRecordStatusEnumType.发放失败;
                applyRecord.RepayStatus = PrepaidRepayStatusEnumType.无;
                applyRecord.Msg = string.IsNullOrWhiteSpace(order.PayFailReason) ? order.FailReasons : order.PayFailReason;
            }
            await _applyRecordService.Update(applyRecord);

            if (order.Status != PaymentOrderTransStatusEnumType.支付成功)
            {
                return;
            }

            var applyViewRecord = await _applyRecordService.GetView(f => f.ID == applyRecord.ID);
            if (applyViewRecord == null)
            {
                return;
            }
            if (applyRecord.ApplyType == PrepaidApplyEnumType.日结工资申请)
            {
                _sendMQService.SendStatisticsMessage("{\"type\":\"dailySalary\",\"accountPartnerName\":\"" + applyViewRecord.AccountPartnerName + "\",\"transactionAmount\":\"" + applyViewRecord.ApplyAmount.ToString() + "\"}");
            }
            else
            {
                _sendMQService.SendStatisticsMessage("{\"type\":\"prepaidSalary\",\"accountPartnerName\":\"" + applyViewRecord.AccountPartnerName + "\",\"transactionAmount\":\"" + applyViewRecord.ApplyAmount.ToString() + "\"}");
            }

            var mqMsg = new
            {
                Type = "prepaidSalarySendSuccess",
                Title = "已发放借支",
                CustomerName = applyViewRecord.CustomerName,
                Name = applyViewRecord.Name,
                CardID = applyViewRecord.IdCardNo,
                ApplyAmount = applyViewRecord.ApplyAmount,
                TotalAmount = applyViewRecord.ApplyAmount
            };
            _sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());

            var mqPendingRepay = new
            {
                Type = "prepaidSalaryPendingRepay",
                Title = "借支待还款",
                CustomerName = applyViewRecord.CustomerName,
                Name = applyViewRecord.Name,
                CardID = applyViewRecord.IdCardNo,
                ApplyAmount = applyViewRecord.ApplyAmount,
                TotalAmount = applyViewRecord.ApplyAmount
            };
            _sendMQService.SendStatisticsMessage(mqPendingRepay.ToCamelCaseJson());
        }

        private async Task SyncPaymentResultToFailSalary(Live_PaymentOrderExtension order)
        {

            if (order.Status != PaymentOrderTransStatusEnumType.支付成功 && order.Status != PaymentOrderTransStatusEnumType.支付失败)
            {
                return;
            }

            if (order.Records == null || order.Records.Count == 0)
            {
                return;
            }

            foreach (var detail in order.Records)
            {
                if (detail.Status != PaymentOrderTransStatusEnumType.支付成功&& detail.Status != PaymentOrderTransStatusEnumType.支付失败)
                {
                    continue;
                }
                if (!Guid.TryParse(detail.BusinessID, out var salaryRecordId) || salaryRecordId == Guid.Empty)
                {
                    continue;
                }

                var salaryRecord = await _salaryRecordService.Get(f => f.ID == salaryRecordId);
                if (salaryRecord == null|| salaryRecord.Status != SalaryRecordSendStatusEnumType.待发放)
                {
                    continue;
                }

                salaryRecord.SendTime = detail.TransTime ?? order.TransTime ?? DateTime.Now;
                salaryRecord.TransStatus = detail.Status.ToString();
                salaryRecord.TransStatusName = detail.Status.ToString();
                salaryRecord.TransMsg = string.IsNullOrWhiteSpace(detail.PayFailReason) ? detail.FailReasons : detail.PayFailReason;
                if (detail.Status == PaymentOrderTransStatusEnumType.支付成功)
                {
                    salaryRecord.Status = SalaryRecordSendStatusEnumType.已发放;
                    salaryRecord.IsSendSuccess = true;
                    //salaryRecord.IsFailSalary = false;
                    //salaryRecord.IsFailSalarySuc = false;
                }
                else
                {
                    salaryRecord.Status = SalaryRecordSendStatusEnumType.发放异常;
                    salaryRecord.IsSendSuccess = false;
                    salaryRecord.IsFailSalary = true;
                    salaryRecord.IsFailSalarySuc = false;
                }

                if (salaryRecord.Status == SalaryRecordSendStatusEnumType.发放异常)
                {
                    Live_SalaryFailRecord failRecord = new Live_SalaryFailRecord()
                    {
                        Bank = salaryRecord.Bank,
                        BankNo = salaryRecord.BankNo,
                        BankUserName = salaryRecord.BankUserName,
                        CreateTime = DateTime.Now,
                        FailType = SalaryFailTypeEnumType.其他,
                        Description = $"单笔代发发放失败，原因{salaryRecord.TransMsg}",
                        Remark = string.Empty,
                        SalaryCustomerID = salaryRecord.SalaryCustomerID,
                        Status = SalaryFailRecordStatusEnumType.待处理,
                        SalaryRecordID = salaryRecord.ID,
                    };
                    await _salaryFailRecordService.Add(failRecord);
                }
                await _salaryRecordService.Update(salaryRecord);
            }
        }


  
        #endregion




    }
}
