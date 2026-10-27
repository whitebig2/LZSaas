using AutoMapper;
using Consul;
using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.Dtos.BankDo;
using LZ.Model.Dtos.SalarySlip;
using LZ.Model.Dtos.SpecialDeductionDtos;
using LZ.Model.Enums;
using LZ.Model.Enums.Budget;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.IExcelTemplate.Banks;
using LZ.Model.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.Options;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using OfficeOpenXml;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using SqlSugar.Extensions;
using SqlSugar.IOC;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalaryCustomerController : ApiControllerBase
    {
        private readonly IPlatformNoticeService    _platformNoticeService;
        private readonly IEventPublisher   _eventPublisher;
        private readonly IAuthService  _authService;
        private readonly ISettingsService _settingsService;
        private readonly AesOption _aesOption;
        private ILogger<SalaryCustomerController> _logger;
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly ISalaryCustomerService _salaryCustomerService;
        private readonly IFinanceOtherService _financeOtherService;
        private readonly ISalaryConfigService _salaryConfigService;
        private readonly IPrepaidApplyRecordService _prepaidApplyRecordService;
        private readonly IAliyunOSSService _aliyunOSSService;
        private readonly IInvoiceConfigService _invoiceConfigService;
        private readonly IInvoiceService _invoiceService;
        private readonly ISettlementService _settlementService;
        private readonly IRedisService _redisService;
        private readonly ITransBatchService _transBatchService;
        private readonly ISalaryDelaySendRecordService _delaySendRecordService;
        private readonly ISalaryCashSendRecordService _cashSendRecordService;
        private readonly ISalaryFailRecordService _failRecordService;
        private readonly IPrepaidRepaymentRecordService _repaymentRecordService;
        private readonly ISalarySlipService _salarySlipService;
        private readonly IActionPasswordService _actionPasswordService;
        private readonly IUserEntryService _userEntryService;
        private readonly IBudgetApplyRecordService  _budgetApplyRecordService;
        private readonly IThirdSalarySlipService  _thirdSalarySlipService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISendMQService _sendMQService;
        private readonly ISalarySplitSendService _splitSendService;
        private readonly IHttpService  _httpService;
        private readonly ISalaryTransactionService  _salaryTransactionService;
        private readonly IPaymentOrderService _paymentOrderService;
        private readonly IPaymentAccountService _paymentAccountService;
        public readonly object _locker = new object();

        private readonly IMapper _mapper;

        public SalaryCustomerController(IOptions<AesOption> options, ILogger<SalaryCustomerController> logger, ISalaryCustomerService salaryCustomerService, IMapper mapper, IFinanceOtherService financeOtherService, ISalaryConfigService salaryConfigService, IPrepaidApplyRecordService prepaidApplyRecordService, IAliyunOSSService aliyunOSSService, IInvoiceConfigService invoiceConfigService, IInvoiceService invoiceService, ISettlementService settlementService, IRedisService redisService, ITransBatchService transBatchService, ISalaryRecordService salaryRecordService, ISalaryDelaySendRecordService delaySendRecordService, ISalaryCashSendRecordService cashSendRecordService, ISalaryFailRecordService failRecordService, IPrepaidRepaymentRecordService repaymentRecordService, ISalarySlipService salarySlipService, IActionPasswordService actionPasswordService, IUserEntryService userEntryService, ISettingsService settingsService, IAuthService authService, IEventPublisher eventPublisher, IPlatformNoticeService platformNoticeService, IBudgetApplyRecordService budgetApplyRecordService, IHttpContextAccessor httpContextAccessor, IThirdSalarySlipService thirdSalarySlipService, ISalarySplitSendService splitSendService, ISendMQService sendMQService, IHttpService httpService, ISalaryTransactionService salaryTransactionService, IPaymentOrderService paymentOrderService, IPaymentAccountService paymentAccountService)
        {
            _logger = logger;
            _salaryCustomerService = salaryCustomerService;
            _mapper = mapper;
            _financeOtherService = financeOtherService;
            _salaryConfigService = salaryConfigService;
            _prepaidApplyRecordService = prepaidApplyRecordService;
            _aliyunOSSService = aliyunOSSService;
            _invoiceConfigService = invoiceConfigService;
            _invoiceService = invoiceService;
            _settlementService = settlementService;
            _redisService = redisService;
            _transBatchService = transBatchService;
            _salaryRecordService = salaryRecordService;
            _delaySendRecordService = delaySendRecordService;
            _cashSendRecordService = cashSendRecordService;
            _failRecordService = failRecordService;
            _repaymentRecordService = repaymentRecordService;
            _salarySlipService = salarySlipService;
            _aesOption = options.Value;
            _actionPasswordService = actionPasswordService;
            _userEntryService = userEntryService;
            _settingsService = settingsService;
            _authService = authService;
            _eventPublisher = eventPublisher;
            _platformNoticeService = platformNoticeService;
            _budgetApplyRecordService = budgetApplyRecordService;
            _httpContextAccessor = httpContextAccessor;
            _thirdSalarySlipService = thirdSalarySlipService;
            _splitSendService = splitSendService;
            _sendMQService = sendMQService;
            _httpService = httpService;
            _salaryTransactionService = salaryTransactionService;
            _paymentOrderService = paymentOrderService;
            _paymentAccountService = paymentAccountService;
        }


        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var models = await _salaryCustomerService.GetNav(f => f.ID == id);
            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var models = await _salaryCustomerService.GetNavView(f => f.ID == id);
            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> GetPreTransaction(Guid id)
        {
            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            return Ok(new
            {
                model.ID,
                model.CustomerName,
                model.SigningUnitCompanyName,
                model.CooperationUnitCompanyName,
                model.SalaryBatchType,
                model.SalaryMonth,
                model.TaxMonth,
                model.UnitDeductionSumAmount,
                model.UnitSalaryTotalAmount,
                TransTotalAmount = model.Records.Where(f => f.IsBatchSend && f.ActualSalarySendCardAmount > 0).Sum(f => f.ActualSalarySendCardAmount),
                TransRecordCount = model.Records.Where(f => f.IsBatchSend && f.ActualSalarySendCardAmount > 0).Count(),
                Remark = "批量对私",
                Summary = "工资",
                Purpose = "工资",
            });
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string signingUnitName, string customerName, DateTime? salaryMonth, DateTime? taxMonth, SalaryStatusEnumType? status, bool? isFinance, DateTime? sendTime, [FromQuery] PagedModelBase query)
        {
            //var exp = Expressionable.Create<Model.Live_V_SalaryCustomer>();

            ////exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);

            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            //{
            //    var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
            //    if (info != null && info.UserType == 2)
            //    {
            //        exp.And(t => t.PartnerID == info.PartnerID);
            //    }
            //}

            var user = CurrentUser;
            SYS_CompanyUser info = null;
            var exp = Expressionable.Create<Model.Live_V_SalaryCustomer>();
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin && user.CompanyType != CompanyType.Party.ToString().ToLower())
            {
                info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(it => it.PartnerID == info.PartnerID);
                }
            }
            if (user.CompanyType == CompanyType.Party.ToString().ToLower())//人力公司授权甲方公司
            {
                exp.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {


                if (info==null)
                {
                    info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                }

                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID||it.CustomerName== "宁波君禾清洁科技有限公司");
                }
                else {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }
            exp.AndIF(query.CID.HasValue, it => it.CustomerID == query.CID);




            exp.AndIF(isFinance.HasValue && isFinance.Value, f => f.Status >= SalaryStatusEnumType.PendingReview && f.Status != SalaryStatusEnumType.Cancelled);
            exp.AndIF(!signingUnitName.IsNullOrWhiteSpace(), f => f.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!customerName.IsNullOrWhiteSpace(), f => f.CustomerName.Contains(customerName));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);
            exp.AndIF(taxMonth.HasValue, f => f.TaxMonth == taxMonth.Value);
            exp.AndIF(sendTime.HasValue, f => f.SendTime != null && f.SendTime.Value.Date == sendTime.Value.Date);
            var models = await _salaryCustomerService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_SalaryCustomer>();
            exp.AndIF(!query.Keyword.IsNullOrWhiteSpace(), f => f.CustomerName.Contains(query.Keyword));
            var models = await _salaryCustomerService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        #region 工资计算

        /// <summary>
        /// 上传工资结算单
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ErrorCodeException"></exception>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadSalaryRecord(IFormFile file)
        {


            List<string> cmbcNameList = new List<string>() {
                "宁波莱博友翌供应链管理有限公司",
                "宁波新莱博友盟供应链管理有限公司",
                "宁波新莱博金浩供应链管理有限公司",
                "宁波蓝鑫人力资源有限公司",
                "宁波莱鑫人力资源有限公司"
            };
            // 当月月初
            var taxMonth = DateTime.Now.Date.AddDays(1 - DateTime.Today.Day);
            HttpContext.Request.Headers.TryGetValue("authorization", out var value);
            List<SalaryRecordExcelTemplate> salaryRecordExcelTemplates = new List<SalaryRecordExcelTemplate>();

            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                salaryRecordExcelTemplates = NpoiExcelHelper.ReadExcelFile<SalaryRecordExcelTemplate>(ms, "工资结算单");
            }
            if (salaryRecordExcelTemplates == null || salaryRecordExcelTemplates.Count <= 0)
            {
                throw new Exception("未获取到【客户结算清单】的记录信息，请核对后重新上传");
            }

            salaryRecordExcelTemplates = salaryRecordExcelTemplates.Where(f => !string.IsNullOrWhiteSpace(f.IdCardNo)).ToList();

            var sameIdCardNos = salaryRecordExcelTemplates.GroupBy(f => f.IdCardNo).Where(f => f.Count() > 1).Select(f => f.Key).ToList();
            if (sameIdCardNos.Any())
            {
                throw new Exception($"检测到身份证号码重复记录,重复记录：{string.Join('、', sameIdCardNos)}");
            }


            if (salaryRecordExcelTemplates.Select(f => f.SalaryMonth).Distinct().Count() != 1)
            {
                throw new Exception("无法多个工资所属月的工资同时导入！");
            }
            if (salaryRecordExcelTemplates.Select(f => f.CustomerName).Distinct().Count() != 1)
            {
                throw new Exception("无法多个客户单位的工资同时导入！");
            }
            if (salaryRecordExcelTemplates.Any(f => f.UnitSalaryAmount<0))
            {
                throw new Exception("企业应发工资不能小于0，请仔细核对数据！");
            }


            var salaryMonth = salaryRecordExcelTemplates.Max(f => f.SalaryMonth);

            List<Live_SalaryCustomerExtension> salaryCustomers = new List<Live_SalaryCustomerExtension>();
            Live_SalaryCustomerExtension salaryCustomer = null;
            Live_V_Customer customerInfo = null;

            //客户单位信息


            //var customerRes = await _httpService.GetAsync<CaAuthIdentityDto>(ServiceNames.UsersService, $"/api/Customer/GetByName?name={salaryRecordExcelTemplates.First().CustomerName}&companyID={CurrentUser.CompanyGID}");


            customerInfo = await _httpService.GetAsync<Live_V_Customer>(ServiceNames.CustomerService, $"/api/Customer/GetByName?name={salaryRecordExcelTemplates.First().CustomerName}&companyID={CurrentUser.CompanyID}");
            if (customerInfo == null)
            {
                throw new Exception("客户单位信息获取失败！");
            }


            var salaryConfig = await _salaryConfigService.Get(f => f.CustomerGID == customerInfo.CustomerGID&&f.IsEnabled);

            if (salaryConfig == null)
            {
                throw new Exception("客户单位的工资配置未设置！");
            }

            if (customerInfo.AgentName.IsNullOrWhiteSpace() || customerInfo.AgentIdCardNo.IsNullOrWhiteSpace() || customerInfo.AgentBank.IsNullOrWhiteSpace() || customerInfo.AgentBankNo.IsNullOrWhiteSpace())
            {
                throw new Exception("客户单位现金发放代理信息信息缺失！");
            }
            List<Live_SalaryRecordExtension> salaryRecordExtensions = salaryRecordExcelTemplates.Select(f => new Live_SalaryRecordExtension()
            {

                IsFailSalary=false,
                ID = Guid.NewGuid(),
                //CompanyName = f.CompanyName,
                UnitDeductionAmount = f.UnitDeductionAmount,
                UnitSalaryAmount = f.UnitSalaryAmount,
                CreateTime = DateTime.Now,
                CustomerName = customerInfo.CustomerName,
                DeductionNoTaxAmount = f.DeductionNoTaxAmount,
                SalaryMonth = f.SalaryMonth,
                CompanyName = f.CustomerName,
                IdCardNo = f.IdCardNo,
                SocialGongJiJinAmount = f.SocialGongJiJinAmount,
                IsDelayPayment = f.IsDelayPayment,
                Name = f.Name,
                PlatformCommissionAmount = f.PlatformCommissionAmount,
                PlatformDepartment = f.PlatformDepartment,
                PlatformWelfareAmount = f.PlatformWelfareAmount,
                Remark = f.Remark,
                SendMonth = f.SalaryMonth,
                ServiceFeeAmount = f.ServiceFeeAmount,
                SocialUnitSumAmount = f.SocialUnitSumAmount,
                SocialDaBingAmount = f.SocialDaBingAmount,
                SocialShiYeAmount = f.SocialShiYeAmount,
                SocialYangLaoAmount = f.SocialYangLaoAmount,
                SocialYiLiaoAmount = f.SocialYiLiaoAmount,
                SubsidyAmount = f.SubsidyAmount,
                SubsidyNoTaxAmount = f.SubsidyNoTaxAmount,
                Workshop = string.IsNullOrWhiteSpace(f.Workshop) ? string.Empty : f.Workshop,
                SpJiaoYuAmount = 0,
                SpZiNvAmount = 0,
                SpDaiKuanAmount = 0,
                SpZuJinAmount = 0,
                SpShanYangAmount = 0,
                SpYingErAmount = 0,
                InnerServiceFeeAmount = 0,
                ActualSalaryAmount = 0,
                //SalaryCustomerID = null,
                SocialDaBingTotalAmount = 0,
                SocialShiYeTotalAmount = 0,
                SocialYangLaoTotalAmount = 0,
                SocialYiLiaoTotalAmount = 0,
                TaxSpDaiKuanTotalAmount = 0,
                TaxSpJiaoYuTotalAmount = 0,
                TaxSpShanYangTotalAmount = 0,
                TaxSpTotalAmount = 0,
                TaxSpYingErTotalAmount = 0,
                TaxSpZiNvTotalAmount = 0,
                TaxSpZuJinTotalAmount = 0,
                TaxTimes = 0,
                TaxMonthAmount = 0,
                BeforeTaxSalaryAmount = 0,
                BeforeTaxSalaryTotalAmount = 0,
                TaxSalaryTotalAmount = 0,
                SpBasicTotalAmount = 0,
                //SalaryType =  SalaryRecordTypeEnumType.常规工资,
                IsCashSend = f.IsCashSend,
                AfterTaxAmount = 0,
                Bank = f.Bank,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName??f.Name,
                CompanySalarySumAmount = 0,
                CompanyTotalAmount = 0,
                EntryDate = f.EntryDate,
                LeaveDate = f.LeaveDate,
                PreSendAmount = 0,
                SocialEmpSumAmount = 0,
                UserEntryID = f.UserEntryID,
                TaxTotalAmount = 0,
                LastTaxTotalAmount = 0,
                LastBeforeTaxSalaryTotalAmount = 0,
                LastSocialDaBingTotalAmount = 0,
                LastSocialEmpTotalAmount = 0,
                LastSocialGongJiJinTotalAmount = 0,
                LastSocialShiYeTotalAmount = 0,
                LastSocialYangLaoTotalAmount = 0,
                LastSocialYiLiaoTotalAmount = 0,
                SocialEmpTotalAmount = 0,
                SocialGongJiJinTotalAmount = 0,
                SpBasiclAmount = 0,
                SpSumAmount = 0,
                SpTotalAmount = 0,
                TaxSocialTotalAmount = 0,
                VerifyInfoOk = true,
                ActualSalarySendCardAmount = 0,
                IsCashSignSuc = false,
                IsDelayPaymentCancel = false,
                MonthNetSalaryAmount = 0,
                PrepaidRepaymentAmount = 0,
                //SalaryDelayType = f.IsDelayPayment ? SalaryDelaySendTypeEnumType.客服暂缓 : SalaryDelaySendTypeEnumType.无,
                Status = SalaryRecordSendStatusEnumType.待处理,
                SendType = SalaryRecordSendTypeEnumType.线上发放,
                Phone = string.Empty,
                BankChangeFilePath = string.Empty,
                DelayType = f.IsDelayPayment ? SalaryDelaySendTypeEnumType.客服暂缓 : SalaryDelaySendTypeEnumType.未暂缓,
                IsDelayPaymentSuc = false,
                UnitManageAmount = 0,
                MonthWorkDays = 0,
                //IsPushTrans = false,
                IsTransBatch = false,
                SettlementID = null,
                //TransNonce = Guid.NewGuid(),
                TransStatus = string.Empty,
                TransStatusName = string.Empty,
                SalaryYear = f.SalaryYear,
                SalaryRecordType = SalaryRecordTypeEnumType.常规工资,
                BatchTransStatus = string.Empty,
                BatchTransStatusName = string.Empty,
                DelayPaymentAmount = 0,
                IsBatchPushSuc = false,
                IsBatchSend = false,
                PrepaidTotalAmount = 0,
                IsSinglePushSuc = false,
                IsSingleSend = false,
                UserID = 0,
                IsCombineSalary = false,
                ChangeAmount = 0,
                ChangeHash = string.Empty,
                ChangeRemark = string.Empty,
                ChangeUpdateTime = null,
                SendTime = null,
                TransDescription = string.Empty,
                TransMsg = string.Empty,
                IsAutoLocked=true,
                 

            }).ToList();


            #region 结算费用


            var idCardNoList = salaryRecordExtensions.Select(f => f.IdCardNo).ToList();

            // 签章单位与商务合作单位不一致则收取管理费
            if (customerInfo.SigningUnitName != customerInfo.CooperationUnitName)
            {
                // 工资所属月已收取过公司管费的员工
                var salaryMonthRecordList = await _salaryCustomerService.QuerySalaryRecordViewList(f =>
                 f.SalaryYear == 0
                 && f.IsCombineSalary == false
                && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资
                && f.CustomerName == customerInfo.CustomerName
                && f.SalaryMonth == salaryMonth
                && idCardNoList.Contains(f.IdCardNo)
                && f.UnitManageAmount > 0
                );
                foreach (var item in salaryRecordExtensions)
                {
                    if (item.SalaryRecordType == SalaryRecordTypeEnumType.常规工资)
                    {
                        if (!salaryMonthRecordList.Any(f => f.IdCardNo == item.IdCardNo))
                        {
                            item.UnitManageAmount = 90;
                        }
                    }
                }
            }
            #endregion



            #region 获取民生电子卡
            //if (customerInfo.SigningUnitName== "宁波莱鑫人力资源有限公司"|| customerInfo.SigningUnitName == "宁波蓝鑫人力资源有限公司")
            //if (cmbcNameList.Contains(customerInfo.SigningUnitName))
            //{
            //    var cmbcAccounts = await _financeOtherService.GetCmbcAccounts(idCardNoList);
            //    if (cmbcAccounts.Success && cmbcAccounts.Result != null && cmbcAccounts.Result.Count > 0)
            //    {
            //        foreach (var item in cmbcAccounts.Result)
            //        {
            //            var record = salaryRecordExtensions.First(f => f.IdCardNo == item.IdCardNo);
            //            record.Bank = "民生银行";
            //            record.BankNo = item.CmbcAccountNo;
            //            record.BankUserName = item.Name;
            //        }
            //    }
            //}
            #endregion

            #region 现金发放匹配
            var today = DateTime.Today;
            var whiteListRes = await _financeOtherService.GeCashWhiteAllListAsync();
            List<Live_WhiteList> whiteLists = new List<Live_WhiteList>();
            if (whiteListRes != null && whiteListRes.Code == 200 && whiteListRes.Data != null)
            {
                whiteLists = whiteListRes.Data.Where(f=>f.Enabled&&f.EndTime!=null&&f.EndTime.Value>=today).ToList();
            }
            Live_WhiteList cashWhiteName = null;
            if (whiteLists.Any())
            {
                salaryRecordExtensions.AsParallel().ForAll(f =>
                {
                    if (whiteLists.Any(x => x.IdentityCard == f.IdCardNo))
                    {
                        var whiteRecord = whiteLists.FirstOrDefault(x => x.IdentityCard == f.IdCardNo);

                        f.IsCashSend = true;
                        f.SendType = SalaryRecordSendTypeEnumType.线下发放;
                        f.CashTypeName = whiteRecord.LabelName;
                        //cashWhiteName = whiteLists.First(x => x.IdentityCard == f.IdCardNo);
                        f.Bank = customerInfo.AgentBank;
                        f.BankNo = customerInfo.AgentBankNo;
                        f.BankUserName = customerInfo.AgentName;
                    }
                });
            }
            #endregion


            #region 员工信息匹配
            if (salaryConfig.IsVerifyEmpInfo)
            {
                var entryExp = Expressionable.Create<Live_UserEntry>();
                entryExp.And(f => f.CustomerGID == customerInfo.CustomerGID);
                entryExp.And(f => f.EntryTime != null );
                entryExp.And(f => f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= salaryMonth.AddMonths(-3)));


                var userEntries = await _userEntryService.List(entryExp.ToExpression());

                Live_UserEntry userEntry = null;
                foreach (var record in salaryRecordExtensions)
                {
                    // 优先匹配在职
                    if (userEntries.Any(f => f.Status.Value == 2 && f.CardID == record.IdCardNo&&f.Name==record.Name))
                    {
                        userEntry = userEntries.First(f => f.Status.Value == 2 && f.CardID == record.IdCardNo && f.Name == record.Name);
                    }
                    else if (userEntries.Any(f => f.Status == 4 && f.CardID == record.IdCardNo && f.Name == record.Name))
                    {
                        userEntry = userEntries.First(f => f.Status == 4 && f.CardID == record.IdCardNo && f.Name == record.Name);
                    }
                    else
                    {
                        userEntry = userEntries.FirstOrDefault(f => f.CardID == record.IdCardNo && f.Name == record.Name);
                    }
                    if (userEntry != null)
                    {

                        // 记录中有银行卡信息则不会替换
                        if (string.IsNullOrWhiteSpace(record.Bank) || string.IsNullOrWhiteSpace(record.BankNo) || string.IsNullOrWhiteSpace(record.BankUserName))
                        {
                            record.Bank = userEntry.BankName;
                            record.BankNo = userEntry.BankNumber;
                            record.BankUserName = userEntry.Name;
                        }
                        record.Phone = userEntry.Phone;
                        record.UserEntryID = userEntry.ID;
                        record.EntryDate = userEntry.EntryTime.Value.Date;
                        record.UserID = userEntry.UserID;
                        if (userEntry.LeaveTime.HasValue)
                        {
                            record.LeaveDate = userEntry.LeaveTime.Value.Date;
                        }
                        // 只针对线上员工
                        if (salaryConfig.IsContractDelay&& userEntry.UserID.HasValue&& userEntry.UserID>0)
                        {
                            // 在职且合同未签署
                            if (userEntry.Status == 2 && (userEntry.ContractStatus == null || userEntry.ContractStatus.Value != (int)ContractStatus.EntryComplete))
                            {

                                // 入职超过15天限制
                                if (userEntry.EntryTime.HasValue && userEntry.EntryTime.Value.Date <= DateTime.Now.Date.AddDays(-15))
                                {
                                    record.IsDelayPayment = true;
                                    record.IsDelayPaymentSuc = false;
                                    record.DelayType = SalaryDelaySendTypeEnumType.网签暂缓;
                                }
                            }
                            else if (userEntry.Status == 4 && userEntry.ContractStatus != null && userEntry.ContractStatus.Value != (int)ContractStatus.LeaveComplete&& userEntry.ContractStatus.Value>=(int)ContractStatus.EntryComplete)
                            {
                                // 离职超过3天的不限制
                                if (userEntry.LeaveTime.HasValue && userEntry.LeaveTime.Value.Date > DateTime.Now.Date.AddDays(-3))
                                {
                                    record.IsDelayPayment = true;
                                    record.IsDelayPaymentSuc = false;
                                    record.DelayType = SalaryDelaySendTypeEnumType.网签暂缓;
                                }
                                
                            }
                        }

                      

                    }
                    else
                    {
                        record.Remark = "员工信息匹配失败";
                    }

                    if (string.IsNullOrWhiteSpace(record.Bank) || string.IsNullOrWhiteSpace(record.BankNo) || record.Bank.Length<=2 || record.BankNo.Length<=2)
                    {
                        if (!record.IsDelayPayment)
                        {
                            record.IsDelayPayment = true;
                            record.IsDelayPaymentSuc = false;
                            record.DelayType = SalaryDelaySendTypeEnumType.银行卡暂缓;
                        }
                        else {
                            if (record.DelayType> SalaryDelaySendTypeEnumType.银行卡暂缓)
                            {
                                record.IsDelayPayment = true;
                                record.IsDelayPaymentSuc = false;
                                record.DelayType = SalaryDelaySendTypeEnumType.银行卡暂缓;
                            }
                        
                        }
                    }
                    // 取消银行卡暂缓限制条件
                    //else if (userEntry != null && userEntry.ContractStatus != (int)ContractStatus.EntryComplete && userEntry.ContractStatus != (int)ContractStatus.LeaveComplete)
                    //{

                    //    if (!record.IsDelayPayment)
                    //    {
                    //        record.IsDelayPayment = true;
                    //        record.IsDelayPaymentSuc = false;

                    //        record.DelayType = SalaryDelaySendTypeEnumType.网签暂缓;
                    //    }
                    //}
                }
                if (salaryRecordExtensions.Any(f => f.UserEntryID <= 0))
                {
                    throw new ErrorCodeException(-1, $"员工信息匹配失败，人员名单:{string.Join(',', salaryRecordExtensions.Where(f => f.UserEntryID <= 0).Select(f => f.Name))}");
                }
            }
            #endregion
            //未验证员工信息则提示银行卡信息缺失
            if (!salaryConfig.IsVerifyEmpInfo)
            {
                if (salaryRecordExtensions.Any(f => string.IsNullOrWhiteSpace(f.Bank) || string.IsNullOrWhiteSpace(f.BankNo)))
                {
                    throw new ErrorCodeException(-1, $"银行卡信息缺失，人员名单:{string.Join(',', salaryRecordExtensions.Where(f => string.IsNullOrWhiteSpace(f.Bank) || string.IsNullOrWhiteSpace(f.BankNo)).Select(f => f.Name))}");
                }
            }

            // 应发为0的情况下，不生成暂缓和现金签收单
            //应发为了0，实发不为了0的是个别情况，不考虑在主流程中了 2025-08-20

            foreach (var item in salaryRecordExtensions)
            {
                var beforTax = item.UnitSalaryAmount + item.SubsidyAmount + item.SubsidyNoTaxAmount;
                if (item.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && beforTax <= 0)
                {

                    item.SendType = SalaryRecordSendTypeEnumType.线上发放;
                    item.IsDelayPayment = false;
                    item.IsCashSend = false;

                    item.DelayType = SalaryDelaySendTypeEnumType.未暂缓;

                }

                if (item.IsDelayPayment||item.IsCashSend)
                {
                    item.IsSingleSend = true;
                    item.IsSinglePushSuc = false;
                }
            }



            var companySalaryGroup = salaryRecordExtensions.GroupBy(f => new { f.CustomerName, f.SalaryMonth, f.Workshop, f.SalaryYear }).ToList();
            foreach (var itemGroup in companySalaryGroup)
            {

                salaryCustomer = new Live_SalaryCustomerExtension()
                {

                    ID = Guid.NewGuid(),
                    IsCustomerPaid = false,
                    // 客户信息
                    CustomerGID = customerInfo.CustomerGID.Value,
                    CustomerName = customerInfo.CustomerName,
                    CustomerCompanyAddress = customerInfo.Address,
                    CustomerCompanyBank = customerInfo.Bank,
                    CustomerCompanyBankNo = customerInfo.BankAccount,
                    CustomerCompanyNo = customerInfo.CompanyNo,
                    CustomerCompanyPhone = customerInfo.Phone,
                    CooperationUnitID = customerInfo.CooperationUnitGID.Value,
                    // 签章单位信息
                    SigningUnitID = customerInfo.SigningUnitGID.Value,
                    SigningUnitCompanyName = customerInfo.SigningUnitName,
                    SigningUnitCompanyNo = customerInfo.SigningUnitCompanyNo,
                    SigningUnitCompanyBank = customerInfo.SigningUnitBankName,
                    SigningUnitCompanyBankNo = customerInfo.SigningUnitBankNo,
                    SigningUnitCompanyAddress = customerInfo.signingUnitAddress,
                    SigningUnitCompanyPhone = customerInfo.SigningUnitPhone,

                    CooperationUnitCompanyName = customerInfo.CooperationUnitName,
                    CooperationUnitCompanyAddress = customerInfo.CooperationUnitAddress,
                    CooperationUnitCompanyBank = customerInfo.CooperationUnitBankName,
                    CooperationUnitCompanyBankNo = customerInfo.CooperationUnitBankNo,
                    CooperationUnitCompanyNo = customerInfo.CooperationUnitCompanyNo,
                    CooperationUnitCompanyPhone = customerInfo.CooperationUnitPhone,
                    ActualSalaryAmount = 0,
                    ManagementFeeAmount = 0,
                    SalaryBatchType = 0,
                    SalarySendType = 0,
                    Status = SalaryStatusEnumType.AwaitingCalculation,
                    TaxMonth = taxMonth,
                    //CompanyGID = customerInfo.CompanyGID.Value,
                    CreateTime = DateTime.Now,
                    SealCompanyName = string.Empty,
                    SendMonth = taxMonth,
                    SalaryMonth = itemGroup.Key.SalaryMonth,
                    CompanyDeductionAmount = itemGroup.Sum(f => f.UnitDeductionAmount),
                    CompanySendSalaryAmount = itemGroup.Sum(f => f.UnitSalaryAmount),
                    CompanySocialSecurityAmount = itemGroup.Sum(f => f.SocialUnitSumAmount),
                    DeductionNoTaxAmount = itemGroup.Sum(f => f.DeductionNoTaxAmount),
                    ServiceFeeAmount = itemGroup.Sum(f => f.ServiceFeeAmount),
                    SubsidyAmount = itemGroup.Sum(f => f.SubsidyAmount),
                    SubsidyNoTaxAmount = itemGroup.Sum(f => f.SubsidyNoTaxAmount),
                    WorkShop = itemGroup.Key.Workshop ?? string.Empty,
                    Records = itemGroup.ToList(),
                    AccountsReceivableSumAmount = 0,
                    CompanySendSalarySumAmount = 0,
                    CompanySettlementSumAmount = 0,
                    InnerServiceFeeAmount = 0,
                    ActualSalarySumAmount = 0,

                    ServiceFeeRecordCount = itemGroup.Count(f => f.ServiceFeeAmount > 0),
                    IsCompanyPosted = false,
                    IsMainBatchSalary = false,
                    IsMatchZeroSalary = false,
                    BeforeTaxSalarySumAmount = 0,
                    CustomerBillTotalAmount = 0,
                    MonthNetSalaryAmount = 0,
                    MonthNetSalarySumAmount = 0,
                    ReceivableTotalAmount = 0,
                    ServiceFeeSumAmount = 0,
                    ServiceRecordCount = itemGroup.Count(f => f.ServiceFeeAmount > 0),
                    SocialUnitSumAmount = 0,
                    UnitDeductionSumAmount = 0,
                    UnitManageSumAmount = 0,
                    UnitSalarySumAmount = 0,
                    UnitSalaryTotalAmount = 0,
                    UnitSettlementAmount = 0,
                    RecordCount = 0,
                    ZeroSalaryCount = 0,
                    IsCmbcSend = false,
                    IsPushTrans = false,
                    TransNonce = Guid.NewGuid(),
                    TransStatus = string.Empty,
                    TransStatusName = string.Empty,
                    SalaryYear = itemGroup.Key.SalaryYear,
                    SalaryType = itemGroup.Key.SalaryYear <= 0 ? SalaryTypeEnumType.常规工资 : SalaryTypeEnumType.年终奖工资,
                    IsCanCmbcSend = false,
                    IsPlatformPrepaid = false,
                    SalarySlipID = null,
                    IsSalarySlip = salaryConfig.IsVerifyPaySlip,
                    IsHasOutInvoice = false,
                    IsCombineSalary = false,
                    CombineCompanyGID = salaryConfig.CombineCompanyGID,
                    CombineCustomerSalaryID = string.Empty,
                    CompanyID = customerInfo.CompanyID,
                    WorkShopBatchNo = null

                };
                //if (salaryCustomer.SigningUnitCompanyName == "宁波莱鑫人力资源有限公司" || salaryCustomer.SigningUnitCompanyName == "宁波蓝鑫人力资源有限公司")
                if (cmbcNameList.Contains(salaryCustomer.SigningUnitCompanyName))
                {
                    salaryCustomer.IsCanCmbcSend = true;
                }

                if (salaryCustomers.Count == 0)
                {
                    salaryCustomer.IsMainBatchSalary = true;
                }
                salaryCustomers.Add(salaryCustomer);
            }


            // 补0工资
            if (salaryConfig.IsVerifyEmpInfo)
            {
                #region 补0工资处理
                List<Live_UserEntry> zeroList = new List<Live_UserEntry>();
                var existList = await _salaryCustomerService.List(f => f.TaxMonth == taxMonth && f.Status != SalaryStatusEnumType.Cancelled && f.CustomerGID == customerInfo.CustomerGID);
                if (existList == null || existList.Count <= 0)
                {
                    var lastMonth = taxMonth.AddMonths(-1);

                    var entrZeroExp = Expressionable.Create<Live_UserEntry>();
                    entrZeroExp.And(f => f.CustomerGID == customerInfo.CustomerGID);
                    entrZeroExp.And(f => f.EntryTime != null && f.EntryTime.Value < taxMonth && f.EntryTime.Value >= lastMonth);
                    entrZeroExp.And(f => (f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= taxMonth)));

                    var userEntryZeroList = await _userEntryService.List(entrZeroExp.ToExpression());

                    if (userEntryZeroList != null)
                    {
                        foreach (var item in userEntryZeroList)
                        {
                            if (salaryRecordExtensions.Any(f => f.IdCardNo == item.CardID))
                            {
                                continue;
                            }

                            if (item.Status == 2)//在职
                            {
                                zeroList.Add(item);
                            }
                            else
                            { // 已离职

                                // 同月入离职
                                if (item.EntryTime.Value.ToString("yyyy-MM") == item.LeaveTime.Value.AddMonths(-1).ToString("yyyy-MM"))
                                {
                                    double days = (item.LeaveTime.Value.Date - item.EntryTime.Value.Date).TotalDays;
                                    if (days >= 7)
                                    {
                                        zeroList.Add(item);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }


                            }
                        }
                    }
                }
                var matchZeroSalry = salaryCustomers.FirstOrDefault(f => f.IsMainBatchSalary && f.SalaryType == SalaryTypeEnumType.常规工资);
                if (matchZeroSalry != null)
                {
                    // 补0工资
                    matchZeroSalry.IsMatchZeroSalary = true;
                    if (zeroList != null && zeroList.Count > 0)
                    {
                        var zeroSalaryRecordList = zeroList.Select(f => new Live_SalaryRecordExtension()
                        {
                            SocialGongJiJinAmount = 0,
                            IdCardNo = f.CardID,
                            AfterTaxAmount = 0,
                            Bank = string.Empty,
                            BankNo = string.Empty,
                            BankUserName = string.Empty,
                            BeforeTaxSalaryAmount = 0,
                            BeforeTaxSalaryTotalAmount = 0,
                            UnitDeductionAmount = 0,
                            CompanyName = matchZeroSalry.CustomerName,
                            UnitSalaryAmount = 0,
                            CompanySalarySumAmount = 0,
                            CompanyTotalAmount = 0,
                            CreateTime = DateTime.Now,
                            CustomerName = matchZeroSalry.CustomerName,
                            DeductionNoTaxAmount = 0,
                            EntryDate = f.EntryTime,
                            InnerServiceFeeAmount = 0,
                            IsCashSend = false,
                            IsDelayPayment = false,
                            LeaveDate = f.LeaveTime,
                            ActualSalaryAmount = 0,
                            Name = f.Name,
                            PlatformCommissionAmount = 0,
                            PlatformDepartment = string.Empty,
                            PlatformWelfareAmount = 0,
                            PreSendAmount = 0,
                            Remark = "补0工资",
                            SalaryMonth = matchZeroSalry.SalaryMonth,
                            SalaryCustomerID = matchZeroSalry.ID,
                            SendMonth = matchZeroSalry.SendMonth,
                            ServiceFeeAmount = 0,
                            SocialUnitSumAmount = 0,
                            SocialDaBingAmount = 0,
                            SocialEmpSumAmount = 0,
                            SocialShiYeAmount = 0,
                            SocialYangLaoAmount = 0,
                            SocialYiLiaoAmount = 0,
                            SpDaiKuanAmount = 0,
                            SpJiaoYuAmount = 0,
                            SpShanYangAmount = 0,
                            SpYingErAmount = 0,
                            SpZiNvAmount = 0,
                            SpZuJinAmount = 0,
                            SubsidyAmount = 0,
                            SubsidyNoTaxAmount = 0,
                            TaxMonthAmount = 0,
                            TaxSalaryTotalAmount = 0,
                            SocialDaBingTotalAmount = 0,
                            SocialShiYeTotalAmount = 0,
                            SocialYangLaoTotalAmount = 0,
                            SocialYiLiaoTotalAmount = 0,
                            SpBasicTotalAmount = 0,
                            TaxSpDaiKuanTotalAmount = 0,
                            TaxSpJiaoYuTotalAmount = 0,
                            TaxSpShanYangTotalAmount = 0,
                            TaxSpTotalAmount = 0,
                            TaxSpYingErTotalAmount = 0,
                            TaxSpZiNvTotalAmount = 0,
                            TaxSpZuJinTotalAmount = 0,
                            TaxTimes = 0,
                            UserEntryID = f.ID,
                            Workshop = matchZeroSalry.WorkShop,
                            LastBeforeTaxSalaryTotalAmount = 0,
                            LastTaxTotalAmount = 0,
                            LastSocialDaBingTotalAmount = 0,
                            LastSocialEmpTotalAmount = 0,
                            LastSocialGongJiJinTotalAmount = 0,
                            LastSocialShiYeTotalAmount = 0,
                            LastSocialYangLaoTotalAmount = 0,
                            LastSocialYiLiaoTotalAmount = 0,
                            SocialEmpTotalAmount = 0,
                            SocialGongJiJinTotalAmount = 0,
                            SpBasiclAmount = 0,
                            SpSumAmount = 0,
                            SpTotalAmount = 0,
                            TaxSocialTotalAmount = 0,
                            VerifyInfoOk = true,
                            TaxTotalAmount = 0,
                            ActualSalarySendCardAmount = 0,
                            IsCashSignSuc = false,
                            IsDelayPaymentCancel = false,
                            MonthNetSalaryAmount = 0,
                            PrepaidRepaymentAmount = 0,
                            DelayType = SalaryDelaySendTypeEnumType.未暂缓,
                            SendType = SalaryRecordSendTypeEnumType.其它发放,
                            Status = SalaryRecordSendStatusEnumType.不发放,
                            //SalaryType = SalaryRecordTypeEnumType.补零工资,
                            IsDelayPaymentSuc = false,
                            BankChangeFilePath = string.Empty,
                            Phone = f.Phone,
                            UnitManageAmount = 0,
                            SalaryRecordType = SalaryRecordTypeEnumType.补零工资,
                            IsBatchSend = false,
                            IsBatchPushSuc = false,
                            IsTransBatch = false,
                            PrepaidTotalAmount = 0,
                            BatchTransStatus = string.Empty,
                            BatchTransStatusName = string.Empty,
                            DelayPaymentAmount = 0,
                            MonthWorkDays = 0,
                            SalaryYear = 0,
                            SettlementID = null,
                            TransStatus = string.Empty,
                            TransStatusName = string.Empty,
                            IsCombineSalary = false,
                            IsSinglePushSuc = false,
                            IsSingleSend = false,
                            SalarySlipRecordID = null,
                            TransDescription = string.Empty,
                            UserID = f.UserID,
                            IsFailSalary=false,
                        });
                        matchZeroSalry.Records.AddRange(zeroSalaryRecordList);
                        matchZeroSalry.RecordCount = matchZeroSalry.Records.Count;
                    }
                }
                #endregion
            }

            if (salaryCustomers != null && salaryCustomers.Count > 1)
            {
                Guid workShopBatchNo = Guid.NewGuid();
                foreach (var item in salaryCustomers)
                {
                    item.WorkShopBatchNo = workShopBatchNo;
                }
            }

            await _salaryCustomerService.AddNav(salaryCustomers);




            //Guid KeyID, string CustomerName, int RecordCount, string CalculateStatusName


            var mainSalary = salaryCustomers.FirstOrDefault(f => f.IsMainBatchSalary);

                 var mqMsg = new { Type = "salaryCalculateStatus", KeyID= mainSalary.ID, CustomerName= mainSalary.CustomerName, RecordCount=mainSalary.RecordCount, CalculateStatusName="正在计算" };
            _sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());

            return Ok();
        }

        /// <summary>
        /// 常规工资计算
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> SalaryCalculation(Guid id)
        {
            Live_V_Customer customerInfo = null;
            var model = await _salaryCustomerService.GetNav(f => f.ID == id);

            if (model.WorkShopBatchNo.HasValue&& model.WorkShopBatchNo.Value!=Guid.Empty&& model.IsMainBatchSalary)
            {
                var otherSalary = await _salaryCustomerService.List(f => f.IsMainBatchSalary == false && f.WorkShopBatchNo == model.WorkShopBatchNo && f.Status <= SalaryStatusEnumType.AwaitingCalculation);
                if (otherSalary != null && otherSalary.Count > 0)
                {
                    throw new ErrorCodeException(-1, $"主工资记录需要合并数据，请优先计算子工资记录！");
                }
            }

            if (model.SalaryType == SalaryTypeEnumType.年终奖工资)
            {
                throw new ErrorCodeException(-1, $"年终奖工资无法使用此计算方式！");
            }
            model.Status = SalaryStatusEnumType.AwaitingCalculation;
            var config = await _salaryConfigService.Get(f => f.CustomerGID == model.CustomerGID && f.IsEnabled);
            if (config == null)
            {
                throw new ErrorCodeException(-1, $"客户单位工资计算配置未设置");
            }

            // 获取身份证号码
            List<string> idCardNoList = model.Records.Select(f => f.IdCardNo).Distinct().ToList();
            // 获取 客户单位当前个税累计月所有以导入的企业工资
            if (model.SalaryBatchType != 0)
            {
                throw new ErrorCodeException(-1, $"非初始化批次工资无法计算，请联系管理员！");
            }
            if (model.Status != SalaryStatusEnumType.AwaitingCalculation)
            {
                throw new ErrorCodeException(-1, $"此状态无法计算！");
            }
            var salaryList = await _salaryCustomerService.List(f => f.ID != id && f.CustomerGID == model.CustomerGID && f.SalaryYear == model.SalaryYear && f.SalaryBatchType > 0 && f.TaxMonth == model.TaxMonth && f.SalaryType == model.SalaryType);
            int maxBatchType = 0;
            if (salaryList.Any())
            {
                maxBatchType = salaryList.Max(x => x.SalaryBatchType);
            }
            model.SalaryBatchType = maxBatchType + 1;
            //客户单位信息
            var customerRes = await _financeOtherService.GetCustomerByNameAsync(model.CustomerName);
            if (customerRes != null && customerRes.Code == 200 && customerRes.Data != null)
            {
                customerInfo = customerRes.Data;
            }
            if (customerInfo == null)
            {
                throw new ErrorCodeException(-1, $"客户单位信息获取失败！");
            }
            if (config.IsCalculateTax)
            {
                List<Live_V_SalarySpecialDedcution> specialDedcutionList = null;
                List<Live_V_SalaryRecord> lastMonthRecords = new List<Live_V_SalaryRecord>();
                var recordIdCardNoList = model.Records.Select(f => f.IdCardNo).ToList();
                if (model.TaxMonth.Value.Month > 1)
                {
                    lastMonthRecords = await _salaryCustomerService.QuerySalaryRecordViewList(f => f.IsCombineSalary == false && f.SalaryYear == model.SalaryYear && f.TaxMonth == model.TaxMonth.Value.AddMonths(-1) && recordIdCardNoList.Contains(f.IdCardNo) && f.SalaryBatchType > 0 && f.SigningUnitCompanyName == model.SigningUnitCompanyName, "SalaryBatchType desc");
                }
                // 当前个税累计月
                var currentMonthRecords = await _salaryCustomerService.QuerySalaryRecordViewList(f => f.IsCombineSalary == false && f.SalaryYear == model.SalaryYear && f.SalaryBatchType > 0 && f.TaxMonth == model.TaxMonth && recordIdCardNoList.Contains(f.IdCardNo) && f.SigningUnitCompanyName == model.SigningUnitCompanyName, "SalaryBatchType desc");
                Live_V_SalaryRecord salaryViewRecord = new Live_V_SalaryRecord();
                foreach (var item in model.Records)
                {
                    if (currentMonthRecords.Any(f => f.IdCardNo == item.IdCardNo))
                    {
                        salaryViewRecord = currentMonthRecords.First(f => f.IdCardNo == item.IdCardNo);
                    }
                    else if (lastMonthRecords.Any(f => f.IdCardNo == item.IdCardNo))
                    {
                        salaryViewRecord = lastMonthRecords.First(f => f.IdCardNo == item.IdCardNo);
                    }
                    else
                    {
                        salaryViewRecord = null;
                    }
                    if (salaryViewRecord != null)
                    {
                        if (salaryViewRecord.TaxMonth == model.TaxMonth)
                        {
                            item.TaxTimes = salaryViewRecord.TaxTimes;
                        }
                        else
                        {
                            item.TaxTimes = salaryViewRecord.TaxTimes + 1;
                        }



                        #region 上一笔工资社保合计数据
                        item.LastTaxTotalAmount = salaryViewRecord.TaxTotalAmount;
                        item.LastSocialDaBingTotalAmount = salaryViewRecord.SocialDaBingTotalAmount;
                        item.LastSocialShiYeTotalAmount = salaryViewRecord.SocialShiYeTotalAmount;
                        item.LastSocialYangLaoTotalAmount = salaryViewRecord.SocialYangLaoTotalAmount;
                        item.LastSocialYiLiaoTotalAmount = salaryViewRecord.SocialYiLiaoTotalAmount;
                        item.LastSocialGongJiJinTotalAmount = salaryViewRecord.SocialGongJiJinTotalAmount;
                        item.LastSocialEmpTotalAmount = salaryViewRecord.SocialEmpTotalAmount;
                        #endregion
                        #region 税前工资
                        item.LastBeforeTaxSalaryTotalAmount = salaryViewRecord.BeforeTaxSalaryTotalAmount;
                        #endregion
                    }
                    else
                    {
                        item.TaxTimes = 1;
                    }
                    #region 根据累计次数进行累计
                    // 专项扣除
                    specialDedcutionList = await _salaryCustomerService.QuerySpecialDeductionList(f => f.IdCardNo == item.IdCardNo);
                    if (specialDedcutionList.Any(f =>  f.DeductionType == Model.Enums.DeductionEnumType.子女教育支出))
                    {
                        item.TaxSpZiNvTotalAmount = 0;
                        var spList = specialDedcutionList.Where(f => f.DeductionType == Model.Enums.DeductionEnumType.子女教育支出);
                        for (int i = 0; i < item.TaxTimes; i++)
                        {
                            var dt = model.TaxMonth.Value.AddMonths(-1 * i);
                            if (dt==model.TaxMonth)
                            {
                                item.SpZiNvAmount = spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                            }
                            item.TaxSpZiNvTotalAmount += spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                        }
                    }
                    if (specialDedcutionList.Any(f =>  f.DeductionType == Model.Enums.DeductionEnumType.继续教育支出))
                    {
                        item.SpJiaoYuAmount = 0;
                        var spList = specialDedcutionList.Where(f => f.DeductionType == Model.Enums.DeductionEnumType.继续教育支出);
                        for (int i = 0; i < item.TaxTimes; i++)
                        {
                            var dt = model.TaxMonth.Value.AddMonths(-1 * i);
                            if (dt == model.TaxMonth)
                            {
                                item.SpJiaoYuAmount = spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                            }
                            item.TaxSpJiaoYuTotalAmount += spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                        }
                    }

                    if (specialDedcutionList.Any(f =>f.DeductionType == Model.Enums.DeductionEnumType.住房租金支出))
                    {
                        item.TaxSpZuJinTotalAmount = 0;
                        var spList = specialDedcutionList.Where(f =>  f.DeductionType == Model.Enums.DeductionEnumType.住房租金支出);
                        for (int i = 0; i < item.TaxTimes; i++)
                        {
                            var dt = model.TaxMonth.Value.AddMonths(-1 * i);
                            if (dt == model.TaxMonth)
                            {
                                item.SpZuJinAmount = spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                            }
                            item.TaxSpZuJinTotalAmount += spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                        }
                    }
                    if (specialDedcutionList.Any(f => f.DeductionType == Model.Enums.DeductionEnumType.住房贷款利息支出))
                    {
                        item.TaxSpDaiKuanTotalAmount = 0;
                        var spList = specialDedcutionList.Where(f =>  f.DeductionType == Model.Enums.DeductionEnumType.住房贷款利息支出);
                        for (int i = 0; i < item.TaxTimes; i++)
                        {
                            var dt = model.TaxMonth.Value.AddMonths(-1 * i);
                            if (dt == model.TaxMonth)
                            {
                                item.SpDaiKuanAmount = spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                            }
                            item.TaxSpDaiKuanTotalAmount += spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                        }
                    }
                    if (specialDedcutionList.Any(f =>  f.DeductionType == Model.Enums.DeductionEnumType.赡养老人支出))
                    {
                        item.TaxSpShanYangTotalAmount = 0;
                        var spList = specialDedcutionList.OrderBy(f=>f.StartMonth).Where(f =>  f.DeductionType == Model.Enums.DeductionEnumType.赡养老人支出).OrderByDescending(f=>f.StartMonth).Take(1).ToList();
                        // 多条合并--取开始的最小值，结束的最大值

                        for (int i = 0; i < item.TaxTimes; i++)
                        {
                            var dt = model.TaxMonth.Value.AddMonths(-1 * i);
                            if (dt == model.TaxMonth)
                            {
                                item.SpShanYangAmount = spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                            }
                            item.TaxSpShanYangTotalAmount += spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                        }
                    }
                    if (specialDedcutionList.Any(f =>  f.DeductionType == Model.Enums.DeductionEnumType.婴幼儿照护支出))
                    {
                        item.TaxSpYingErTotalAmount = 0;
                        var spList = specialDedcutionList.Where(f =>f.DeductionType == Model.Enums.DeductionEnumType.婴幼儿照护支出);
                        for (int i = 0; i < item.TaxTimes; i++)
                        {
                            var dt = model.TaxMonth.Value.AddMonths(-1 * i);
                            if (dt == model.TaxMonth)
                            {
                                item.SpYingErAmount = spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                            }
                            item.TaxSpYingErTotalAmount += spList.Where(f => f.StartMonth <= dt && f.EndMonth >= dt).Sum(f => f.MonthAmount.Value);
                        }
                    }
                    #endregion


                    item.CalculationTax();
                }
            }
            else
            {
                foreach (var item in model.Records)
                {
                    item.TaxTimes = 1;
                    item.CalculationNoTax();
                }
            }
            List<Live_PrepaidApplyRecordExtension> updatePrepaidApplyRecordList = new List<Live_PrepaidApplyRecordExtension>();
            #region 预发工资
            // 获取预发工资
            var prepaidApplyRecordList = await _prepaidApplyRecordService.ListNav(f =>
            idCardNoList.Contains(f.IdCardNo)
            && f.CustomerGID == model.CustomerGID
            && f.Status == PrepaidRecordStatusEnumType.已发放
            && (f.RepayStatus ==  PrepaidRepayStatusEnumType.无|| f.RepayStatus == PrepaidRepayStatusEnumType.待还款 || f.RepayStatus == PrepaidRepayStatusEnumType.还款中 || f.RepayStatus == PrepaidRepayStatusEnumType.还款失败));

            if (prepaidApplyRecordList!=null&& prepaidApplyRecordList.Count>0)
            {
                prepaidApplyRecordList = prepaidApplyRecordList.OrderBy(f => f.CreateTime).ToList();
            }
            foreach (var item in prepaidApplyRecordList)
            {
                if (model.Records.Any(f => f.IdCardNo == item.IdCardNo))
                {
                    var salaryRecord = model.Records.First(f => f.IdCardNo == item.IdCardNo);
                    if (salaryRecord.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && salaryRecord.ActualSalaryAmount > 0)
                    {
                        var repaymentAmount = item.ApplyAmount - item.RepayTotalAmount;
                        if (repaymentAmount > 0)
                        {
                            var canRepaymentAmount = salaryRecord.PrepaidRepayment(repaymentAmount);
                            if (canRepaymentAmount > 0)
                            {
                                item.AddRepaymentRecord(salaryRecord.SalaryCustomerID, salaryRecord.ID, salaryRecord.UserEntryID, canRepaymentAmount, "工资发放还款");
                                updatePrepaidApplyRecordList.Add(item);
                            }
                        }
                    }
                }
            }
            #endregion
            #region 生成暂缓发放记录
            List<Live_SalaryDelaySendRecord> delayRecords = null;
            if (model.Records.Any(f => f.IsDelayPayment))
            {
                delayRecords = model.Records.Where(f => f.IsDelayPayment).Select(f => new Live_SalaryDelaySendRecord()
                {
                    CreateTime = DateTime.Now,
                    DelayType = f.DelayType,
                    FilePath = string.Empty,
                    Remark = string.Empty,
                    SalaryRecordID = f.ID,
                    SendAmount = 0,
                    TotalAmount = f.ActualSalarySendCardAmount,
                    Status = f.DelayType == SalaryDelaySendTypeEnumType.客服暂缓 ? SalaryDelaySendStatusEnumType.待客服处理 : SalaryDelaySendStatusEnumType.待系统处理,
                }).ToList();
                model.DelaySendRecords = delayRecords;
            }
            #endregion
            #region  生成现金发放记录

            var cashRecords = new List<Live_SalaryCashSendRecord>();
            if (model.Records.Any(f => f.IsCashSend))
            {
                cashRecords = model.Records.Where(f => f.IsCashSend).Select(f => new Live_SalaryCashSendRecord()
                {
                    AgentBank = f.Bank,
                    AgentBankNo = f.BankNo,
                    AgentIdCardNo = f.BankNo,
                    AgentName = f.BankNo,
                    SalaryRecordID = f.ID,
                    SendAmount = 0,
                    TotalAmount = f.ActualSalarySendCardAmount,
                    Reason = "现金发放",
                    Status =  CashSendStatusEnumType.待签署,
                    Remark = string.Empty,
                    FilePath = string.Empty,
                    CreateTime = DateTime.Now,
                    AuditStatus = SalaryCashAuditStatusEnumType.审核通过,
                    AgentStatus = SalaryCashAgentStatusEnumType.待转账,
                    FromType = "现金白名单",
                    CashType=f.CashTypeName
                }).ToList();

                model.CashSendRecords = cashRecords;
            }
            #endregion
            model.Status = SalaryStatusEnumType.PendingPush;
            DbScoped.SugarScope.BeginTran();
            if (updatePrepaidApplyRecordList.Count > 0)
            {
                await _prepaidApplyRecordService.UpdateNav(updatePrepaidApplyRecordList);
            }
            model.SalaryCount();

            var invoiceConfig = await _invoiceConfigService.List(f => f.PrintTemplate == PrintTemplateEnumType.外部发票 && f.CustomerGID == model.CustomerGID && f.IsEnabled);

            if (invoiceConfig != null && invoiceConfig.Any())
            {
                model.IsHasOutInvoice = true;
            }

            model.SetBatchRecord();
            await _salaryCustomerService.UpdateNav(model);
            DbScoped.SugarScope.CommitTran();

            await GenerateSettlement(model.ID);
            await GenerateInvoice(model.ID);
            if (model.Records.Any(f=>f.IsPrepaidFail))
            {
                var prepaidFailList = model.Records.Where(f => f.IsPrepaidFail).ToList();
                return Ok(new { items = prepaidFailList, count = prepaidFailList.Count,msg=string.Join('、', prepaidFailList.Select(f=>f.Name)) });
            }
            var calculateTimes = _salaryCustomerService.SalaryCustomerCalculateTimes();
            var mqMsg1 = new { Type = "salaryCalculateTimes", calculateTimes.TodayTimes, calculateTimes.MonthTimes,calculateTimes.YearTimes };
            _sendMQService.SendStatisticsMessage(mqMsg1.ToCamelCaseJson());

            var mqMsg2 = new { Type = "salaryCalculateStatus", KeyID = model.ID, CustomerName = model.CustomerName, RecordCount = model.RecordCount, CalculateStatusName = "计算完成" };
            _sendMQService.SendStatisticsMessage(mqMsg2.ToCamelCaseJson());

            if (updatePrepaidApplyRecordList.Count>0)
            {

                var firstRecord = updatePrepaidApplyRecordList.First();

                var mqMsg = new
                {
                    Type = "prepaidSalaryRepayError",
                    Title = "借支还款异常",
                    CustomerName = model.CustomerName,
                    Name = firstRecord.Name,
                    CardID = firstRecord.IdCardNo,
                    ErrorAmount = firstRecord.ApplyAmount- firstRecord.RepayTotalAmount,
                    TotalAmount = updatePrepaidApplyRecordList.Sum(f => f.ApplyAmount-f.RepayTotalAmount)
                };
                _sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
            }

           

            return Ok();
        }
        /// <summary>
        /// 年终奖计算模式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> SalaryYearCalculation(Guid id)
        {
            Live_V_Customer customerInfo = null;
            Live_Enterprise signingUnit = null;
            Live_Enterprise cooperationUnit = null;
            var model = await _salaryCustomerService.GetNav(f => f.ID == id);
            if (model.SalaryType != SalaryTypeEnumType.年终奖工资)
            {
                throw new ErrorCodeException(-1, $"非年终奖无法使用此计算方式");
            }
            model.Status = SalaryStatusEnumType.AwaitingCalculation;
            var config = await _salaryConfigService.Get(f => f.CustomerGID == model.CustomerGID);
            if (config == null)
            {
                throw new ErrorCodeException(-1, $"客户单位工资计算配置未设置");
            }
            // 获取身份证号码
            List<string> idCardNoList = model.Records.Select(f => f.IdCardNo).Distinct().ToList();
            // 获取 客户单位当前个税累计月所有以导入的企业工资
            if (model.SalaryBatchType != 0)
            {
                throw new ErrorCodeException(-1, $"非初始化批次工资无法计算，请联系管理员！");
            }
            if (model.Status != SalaryStatusEnumType.AwaitingCalculation)
            {
                throw new ErrorCodeException(-1, $"此状态无法计算！");
            }
            var salaryList = await _salaryCustomerService.List(f => f.ID != id && f.SalaryBatchType > 0 && f.SalaryYear == model.SalaryYear && f.SalaryType == model.SalaryType);
            int maxBatchType = 0;
            if (salaryList.Any())
            {
                maxBatchType = salaryList.Max(x => x.SalaryBatchType);
            }
            model.SalaryBatchType = maxBatchType + 1;
            //客户单位信息
            var customerRes = await _financeOtherService.GetCustomerByNameAsync(model.CustomerName);
            if (customerRes != null && customerRes.Code == 200 && customerRes.Data != null)
            {
                customerInfo = customerRes.Data;
            }
            if (customerInfo == null)
            {
                throw new ErrorCodeException(-1, $"客户单位信息获取失败！");
            }
            var recordIdCardNoList = model.Records.Select(f => f.IdCardNo).ToList();
            var lastMonthRecords = await _salaryCustomerService.QuerySalaryRecordViewList(f =>
            f.IsCombineSalary == false &&
            f.SalaryYear == model.SalaryYear
            && f.SalaryBatchType > 0
            && f.SigningUnitCompanyName == model.SigningUnitCompanyName, "SalaryBatchType desc");
            // 当前个税累计月
            Live_V_SalaryRecord salaryViewRecord = new Live_V_SalaryRecord();
            foreach (var item in model.Records)
            {
                if (lastMonthRecords.Any(f => f.IdCardNo == item.IdCardNo))
                {
                    salaryViewRecord = lastMonthRecords.First(f => f.IdCardNo == item.IdCardNo);
                }
                else
                {
                    salaryViewRecord = null;
                }
                if (salaryViewRecord != null)
                {
                    item.TaxTimes = salaryViewRecord.TaxTimes + 1;
                    #region 上一笔工资社保合计数据
                    item.LastTaxTotalAmount = salaryViewRecord.TaxTotalAmount;
                    //item.LastSocialDaBingTotalAmount = salaryViewRecord.SocialDaBingTotalAmount;
                    //item.LastSocialShiYeTotalAmount = salaryViewRecord.SocialShiYeTotalAmount;
                    //item.LastSocialYangLaoTotalAmount = salaryViewRecord.LastSocialYangLaoTotalAmount;
                    //item.LastSocialYiLiaoTotalAmount = salaryViewRecord.SocialYiLiaoTotalAmount;
                    //item.LastSocialGongJiJinTotalAmount = salaryViewRecord.SocialGongJiJinTotalAmount;
                    //item.LastSocialEmpTotalAmount = salaryViewRecord.SocialEmpTotalAmount;
                    #endregion
                    #region 税前工资
                    item.LastBeforeTaxSalaryTotalAmount = salaryViewRecord.BeforeTaxSalaryTotalAmount;
                    #endregion
                }
                else
                {
                    item.TaxTimes = 1;
                }
                item.CalculationSalaryYearPersionalTax();
            }
            List<Live_PrepaidApplyRecordExtension> updatePrepaidApplyRecordList = new List<Live_PrepaidApplyRecordExtension>();
            #region 预发工资
            if (config.IsSalaryYearRepayment)
            {
                // 获取预发工资
                var prepaidApplyRecordList = await _prepaidApplyRecordService.ListNav(f =>
                idCardNoList.Contains(f.IdCardNo)
                && f.CustomerGID == model.CustomerGID
                && f.Status == PrepaidRecordStatusEnumType.已发放
                && (f.RepayStatus == PrepaidRepayStatusEnumType.待还款 || f.RepayStatus == PrepaidRepayStatusEnumType.还款中));
                foreach (var item in prepaidApplyRecordList)
                {
                    if (model.Records.Any(f => f.IdCardNo == item.IdCardNo))
                    {
                        var salaryRecord = model.Records.First(f => f.IdCardNo == item.IdCardNo);
                        if (salaryRecord.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && salaryRecord.ActualSalaryAmount > 0)
                        {
                            var repaymentAmount = item.ApplyAmount - item.RepayTotalAmount;
                            if (repaymentAmount > 0)
                            {
                                var canRepaymentAmount = salaryRecord.PrepaidRepayment(repaymentAmount);
                                if (canRepaymentAmount > 0)
                                {
                                    item.AddRepaymentRecord(salaryRecord.SalaryCustomerID, salaryRecord.ID, salaryRecord.UserEntryID, canRepaymentAmount, "工资发放还款");
                                    updatePrepaidApplyRecordList.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 生成暂缓发放记录
            List<Live_SalaryDelaySendRecord> delayRecords = null;
            if (model.Records.Any(f => f.IsDelayPayment))
            {
                delayRecords = model.Records.Where(f => f.IsDelayPayment).Select(f => new Live_SalaryDelaySendRecord()
                {
                    CreateTime = DateTime.Now,
                    DelayType = f.DelayType,
                    FilePath = string.Empty,
                    Remark = string.Empty,
                    SalaryRecordID = f.ID,
                    SendAmount = 0,
                    TotalAmount = f.ActualSalarySendCardAmount,
                    Status = f.DelayType == SalaryDelaySendTypeEnumType.客服暂缓 ? SalaryDelaySendStatusEnumType.待客服处理 : SalaryDelaySendStatusEnumType.待系统处理,
                }).ToList();
                model.DelaySendRecords = delayRecords;
            }
            #endregion
            #region  生成现金发放记录

            var cashRecords = new List<Live_SalaryCashSendRecord>();
            if (model.Records.Any(f => f.IsCashSend && f.ActualSalarySendCardAmount > 0))
            {
                cashRecords = model.Records.Where(f => f.IsCashSend && f.ActualSalarySendCardAmount > 0).Select(f => new Live_SalaryCashSendRecord()
                {
                    AgentBank = f.Bank,
                    AgentBankNo = f.BankNo,
                    AgentIdCardNo = f.BankNo,
                    AgentName = f.BankNo,
                    SalaryRecordID = f.ID,
                    SendAmount = 0,
                    TotalAmount = f.ActualSalarySendCardAmount,
                    Reason = "现金发放",
                    Status = 0,
                    Remark = string.Empty,
                    FilePath = string.Empty,
                    CreateTime = DateTime.Now,
                    FromType = "现金白名单",
                    CashType = f.CashTypeName
                }).ToList();
                model.CashSendRecords = cashRecords;
            }

            #endregion
            model.Status = SalaryStatusEnumType.PendingPush;
            DbScoped.SugarScope.BeginTran();
            if (updatePrepaidApplyRecordList.Count > 0)
            {
                await _prepaidApplyRecordService.UpdateNav(updatePrepaidApplyRecordList);
            }
            model.SalaryCount();

            var invoiceConfig = await _invoiceConfigService.List(f => f.PrintTemplate == PrintTemplateEnumType.外部发票 && f.CustomerGID == model.CustomerGID && f.IsEnabled);

            if (invoiceConfig != null && invoiceConfig.Any())
            {
                model.IsHasOutInvoice = true;
            }
            await _salaryCustomerService.UpdateNav(model);
            DbScoped.SugarScope.CommitTran();

            await GenerateInvoice(model.ID);
            await GenerateSettlement(model.ID);
            return Ok();
        }
        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GenerateSettlement(Guid id)
        {
            var model = await _salaryCustomerService.GetNav(f => f.ID == id);
            var config = await _salaryConfigService.Get(f => f.CustomerGID == model.CustomerGID && f.IsEnabled);
            List<Live_Settlement> settlements = new List<Live_Settlement>();
            Live_Settlement settlement = null;

            #region 生成结算单
            // 费用审批单
            // 签章单位与商户合作单位名称不一致
            // 费用审批单生成一个总的
            if (model.SigningUnitCompanyName!= model.CooperationUnitCompanyName&&model.IsMainBatchSalary)
            {
                List<Live_SalaryCustomer> models = new List<Live_SalaryCustomer>();

                if (model.WorkShopBatchNo != null && model.WorkShopBatchNo.Value != Guid.Empty)
                {
                    models = await _salaryCustomerService.List(f => f.WorkShopBatchNo == model.WorkShopBatchNo);

                }
                else {
                    models.Add(model);
                }
                var modelIdList = models.Select(f => f.ID).ToList();

                settlement = new Live_Settlement()
                {
                    CreateTime = DateTime.Now,
                    CustomerGID = model.CustomerGID,
                    Remark = string.Empty,
                    SalaryCustomerID = model.ID,
                    SalaryMonth = model.SalaryMonth,
                    SettlementType = (int)SettlementBusinessTypeEnumType.费用审批单,
                    Title = "派遣员工工资",
                    TotalAmount = models.Sum(f=>f.UnitSalarySumAmount+ f.SocialUnitSumAmount+ f.UnitManageSumAmount- f.CompanyDeductionAmount),
                    //model.UnitSalarySumAmount  //企业应发工资
                    //+ model.SocialUnitSumAmount // 企业社保
                    //+ model.UnitManageSumAmount // 公司服务费
                    //- model.CompanyDeductionAmount,// 企业扣款

                    ApplyDate = DateTime.Now,
                    BusinessType = SettlementBusinessTypeEnumType.费用审批单,
                    
                    IssuerUnitID = model.CooperationUnitID,
                    IssuerName = model.CooperationUnitCompanyName,
                    IssuerCompanyNo = model.CooperationUnitCompanyNo,
                    IssuerPhone = model.CooperationUnitCompanyPhone,
                    IssuerBank = model.CooperationUnitCompanyBank,
                    IssuerBankNo = model.CooperationUnitCompanyBankNo,
                    IssuerAddress = model.CooperationUnitCompanyAddress,
                    RecipientUnitID = model.SigningUnitID,
                    RecipientName = model.SigningUnitCompanyName,
                    RecipientPhone = model.SigningUnitCompanyPhone,
                    RecipientAddress = model.SigningUnitCompanyAddress,
                    RecipientBank = model.SigningUnitCompanyBank,
                    RecipientBankNo = model.SigningUnitCompanyBankNo,
                    RecipientCompanyNo = model.SigningUnitCompanyBankNo,
                    CustomerAddress = model.CustomerName,
                    CustomerCompanyNo = model.CustomerCompanyNo,
                    CustomerName = model.CustomerName,
                    CustomerBank = model.CustomerCompanyBank,
                    CustomerBankNo = model.CustomerCompanyBankNo,
                    CustomerPhone = model.CustomerCompanyPhone,
                };
                //settlement.Description = $"{settlement.CustomerName}-{settlement.SalaryMonth.Value.ToString("yyyy年MM月")}，企业应发工资{ model.UnitSalarySumAmount - model.UnitDeductionSumAmount}元，公司管理费{model.UnitManageSumAmount}元,结算社保企业部分{model.SocialUnitSumAmount}元";
                settlement.Description = $"{settlement.CustomerName}-{settlement.SalaryMonth.Value.ToString("yyyy年MM月")}，企业应发工资{models.Sum(f=>f.UnitSalarySumAmount-f.UnitDeductionSumAmount)}元，公司管理费{models.Sum(f=> f.UnitManageSumAmount)}元,结算社保企业部分{models.Sum(f=>f.SocialUnitSumAmount)}元";
                settlements.Add(settlement);
            }
            // 工资结算单
            if (config.IsSalarySettlement)
            {
                var salaryRecord = model.Records.Where(f =>f.IsDelayPayment==false&& f.IsCashSend == false && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资);
                settlement = new Live_Settlement()
                {
                    CreateTime = DateTime.Now,
                    CustomerGID = model.CustomerGID,
                    Remark = string.Empty,
                    SalaryCustomerID = model.ID,
                    SalaryMonth = model.SalaryMonth,
                    SettlementType = (int)SettlementBusinessTypeEnumType.工资结算单,
                    Title = "派遣员工工资",
                    TotalAmount = salaryRecord.Sum(f => f.ActualSalarySendCardAmount),
                    ApplyDate = DateTime.Now,
                    BusinessType = SettlementBusinessTypeEnumType.工资结算单,
                    IssuerUnitID = model.SigningUnitID,
                    IssuerName = model.SigningUnitCompanyName,
                    IssuerCompanyNo = model.SigningUnitCompanyNo,
                    IssuerPhone = model.SigningUnitCompanyPhone,
                    IssuerBank = model.SigningUnitCompanyBank,
                    IssuerBankNo = model.SigningUnitCompanyBankNo,
                    IssuerAddress = model.SigningUnitCompanyAddress,
                    RecipientUnitID = model.CooperationUnitID,
                    RecipientName = model.CooperationUnitCompanyName,
                    RecipientPhone = model.CooperationUnitCompanyPhone,
                    RecipientAddress = model.CooperationUnitCompanyAddress,
                    RecipientBank = model.CooperationUnitCompanyBank,
                    RecipientBankNo = model.CooperationUnitCompanyBankNo,
                    RecipientCompanyNo = model.CooperationUnitCompanyNo,
                    CustomerAddress = model.CustomerName,
                    CustomerCompanyNo = model.CustomerCompanyNo,
                    CustomerName = model.CustomerName,
                    CustomerBank = model.CustomerCompanyBank,
                    CustomerBankNo = model.CustomerCompanyBankNo,
                    CustomerPhone = model.CustomerCompanyPhone,
                };
              
                if (model.WorkShop.IsNullOrWhiteSpace())
                {
                    settlement.Description = $"{settlement.CustomerName}{settlement.SalaryMonth.Value.ToString("yyyy年MM月")},共{salaryRecord.Count()}名员工工资，合计{salaryRecord.Sum(f => f.ActualSalarySendCardAmount)}元";
                }
                else
                {
                    settlement.Description = $"{settlement.CustomerName}-{model.WorkShop}{settlement.SalaryMonth.Value.ToString("yyyy年MM月")},共{salaryRecord.Count()}名员工工资，合计{salaryRecord.Sum(f => f.ActualSalarySendCardAmount)}元";
                }
                settlements.Add(settlement);
            }
            #endregion
            if (settlements.Any())
            {
                await _settlementService.Add(settlements);
            }
            return Ok();
        }
        /// <summary>
        /// 生成发票
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateInvoice(Guid id)
        {

            var setting = await _settingsService.GetMasterSettings();
            var model = await _salaryCustomerService.GetView(id);

            List<Live_SalaryRecord> outRecords = new List<Live_SalaryRecord>();
            List<Live_SalaryRecord> insideRecords = new List<Live_SalaryRecord>();
            outRecords = await _salaryRecordService.List(f => f.SalaryCustomerID == id);
            if (model.WorkShopBatchNo != null && model.WorkShopBatchNo != Guid.Empty)
            {
                var models = await _salaryCustomerService.List(f => f.WorkShopBatchNo == model.WorkShopBatchNo);
                var modelIdList = models.Select(f => f.ID).ToList();
                insideRecords = await _salaryRecordService.List(f => modelIdList.Contains(f.SalaryCustomerID));
            }
            else
            {
                insideRecords = outRecords;
            }


            var existInvoiceList = await _invoiceService.List(f => f.SalaryCustomerID != null && f.SalaryCustomerID.Value == id && f.Status <= InvoiceStatusEnumType.已开具);

            List<Live_InvoiceExtension> invoices = new List<Live_InvoiceExtension>();
            string fileOssUrlOut = string.Empty;
            string fileOssUrlInside = string.Empty;

            var configs = await _invoiceConfigService.ListView(f => f.CustomerGID == model.CustomerGID && f.IsEnabled);

            if (configs == null || configs.Count <= 0)
            {
                return Ok();
            }


            if ( configs.Any(f => f.PrintTemplate == PrintTemplateEnumType.外部发票))
            {
                var excelData = outRecords.Select(f => new SalaryInvoiceExcelTemplate()
                {
                    CustomerName = f.CustomerName,
                    CooperationUnitCompanyName = model.CooperationUnitCompanyName,
                    SalaryMonth = model.SalaryMonth.Value,
                    Name = f.Name,
                    IdCardNo = f.IdCardNo,
                    Remark = f.Remark,
                    EntryDate = f.EntryDate.Value,
                    LeaveDate = f.LeaveDate,
                    ServiceFeeAmount = f.ServiceFeeAmount,
                    UnitDeductionAmount = f.UnitDeductionAmount,
                    UnitSalaryAmount = f.UnitSalaryAmount,
                }).ToList();
                excelData.Add(new SalaryInvoiceExcelTemplate()
                {
                    CustomerName = "合计",
                    CooperationUnitCompanyName = string.Empty,
                    SalaryMonth = model.SalaryMonth.Value,
                    Name = string.Empty,
                    IdCardNo = string.Empty,
                    Remark = string.Empty,
                    EntryDate = null,
                    LeaveDate = null,
                    ServiceFeeAmount = excelData.Sum(f => f.ServiceFeeAmount),
                    UnitDeductionAmount = excelData.Sum(f => f.UnitDeductionAmount),
                    UnitSalaryAmount = excelData.Sum(f => f.UnitSalaryAmount),
                });
                using (var ms = new MemoryStream())
                {
                    var fileBytes = NpoiExcelHelper.WriteExcel<SalaryInvoiceExcelTemplate>(excelData, "客户工资附件");
                    ms.Write(fileBytes);
                    var ossResp = await _aliyunOSSService.UploadFile(ms, $"{model.CustomerName}.xlsx", true, false, "invoiceAttachFiles");
                    if (ossResp.Code == 0)
                    {
                        fileOssUrlOut = setting.AliBucketDomain + ossResp.Data;
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "客户工资上传oss失败");
                    }
                }
            }
            //商务合作单位与签章单位不一致的情况下，生成内部发票
            if (model.SigningUnitCompanyName.Trim() != model.CooperationUnitCompanyName.Trim())
            {
                if (model.IsMainBatchSalary)
                {

                    #region 附件上传
                    var excelData = insideRecords.Select(f => new SalaryInvoiceInsideExcelTemplate()
                    {
                        CustomerName = f.CustomerName,
                        CooperationUnitCompanyName = model.CooperationUnitCompanyName,
                        SalaryMonth = model.SalaryMonth.Value,
                        Name = f.Name,
                        IdCardNo = f.IdCardNo,
                        Remark = f.Remark,
                        EntryDate = f.EntryDate.Value,
                        LeaveDate = f.LeaveDate,
                        SocialUnitSumAmount = f.SocialUnitSumAmount,
                        UnitManageAmount = f.UnitManageAmount,
                        UnitDeductionAmount = f.UnitDeductionAmount,
                        UnitSalaryAmount = f.UnitSalaryAmount,
                        ServiceFeeAmount=f.ServiceFeeAmount
                    }).ToList();
                    excelData.Add(new SalaryInvoiceInsideExcelTemplate()
                    {
                        CustomerName = "合计",
                        CooperationUnitCompanyName = string.Empty,
                        SalaryMonth = model.SalaryMonth.Value,
                        Name = string.Empty,
                        IdCardNo = string.Empty,
                        Remark = string.Empty,
                        EntryDate = null,
                        LeaveDate = null,
                        SocialUnitSumAmount = excelData.Sum(f => f.SocialUnitSumAmount),
                        UnitManageAmount = excelData.Sum(f => f.UnitManageAmount),
                        UnitDeductionAmount = excelData.Sum(f => f.UnitDeductionAmount),
                        UnitSalaryAmount = excelData.Sum(f => f.UnitSalaryAmount),
                        ServiceFeeAmount = excelData.Sum(f => f.ServiceFeeAmount),
                    });
                    using (var ms = new MemoryStream())
                    {
                        var fileBytes = NpoiExcelHelper.WriteExcel<SalaryInvoiceInsideExcelTemplate>(excelData, "客户工资附件");
                        ms.Write(fileBytes);
                        var ossResp = await _aliyunOSSService.UploadFile(ms, $"{model.CustomerName}.xlsx", true, false, "invoiceAttachFiles");
                        if (ossResp.Code == 0)
                        {
                            fileOssUrlInside = setting.AliBucketDomain + ossResp.Data;
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "客户工资上传oss失败");
                        }
                    }
                    #endregion

                    #region 生成内部发票

                    var companyInvoiceTotalAmount = insideRecords.Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount + f.UnitManageAmount + f.SocialUnitSumAmount);
                    var companyInvoice = new Live_InvoiceExtension()
                    {

                        IssuerUnitID = model.SigningUnitID,
                        IssuerName = model.SigningUnitCompanyName,
                        IssuerCompanyNo = model.SigningUnitCompanyNo,
                        IssuerPhone = model.SigningUnitCompanyPhone,
                        IssuerBank = model.SigningUnitCompanyBank,
                        IssuerBankNo = model.SigningUnitCompanyBankNo,
                        IssuerAddress = model.SigningUnitCompanyAddress,
                        RecipientUnitID = model.CooperationUnitID,
                        RecipientName = model.CooperationUnitCompanyName,
                        RecipientCompanyNo = model.CooperationUnitCompanyNo,
                        RecipientPhone = model.CooperationUnitCompanyPhone,
                        RecipientBank = model.CooperationUnitCompanyBank,
                        RecipientBankNo = model.CooperationUnitCompanyBankNo,
                        RecipientAddress = model.CooperationUnitCompanyAddress,
                        Description = $"{model.CustomerName}({model.SalaryMonth.Value.Month})月,企业应发工资：{insideRecords.Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount)}元，结算管理费：{insideRecords.Sum(f => f.UnitManageAmount)}元，结算社保企业部分：{insideRecords.Sum(f => f.SocialUnitSumAmount)}元,合计{companyInvoiceTotalAmount}元  ",

                        ID = Guid.NewGuid(),
                        SalaryCustomerID = model.ID,
                        CustomerGID = model.CustomerGID,
                        Remark = string.Empty,
                        AddedTaxPointAmount = 0,
                        AdditionalTaxPointAmount = 0,
                        InvoiceAmount = companyInvoiceTotalAmount,
                        InvoiceType = InvoiceTypeEnumType.ESpecialInvoice,
                        Title = "生产生活服务*劳务派遣服务",
                        AddedTaxDiscountAmount = 0,
                        AdditionalTaxDiscountAmount = 0,
                        ApplyTime = DateTime.Now,
                        BusinessType = PrintTemplateEnumType.公司发票,
                        AddedTaxAmount = 0,
                        AddedTaxSumAmount = 0,
                        AdditionalTaxAmount = 0,
                        AdditionTaxSumAmount = 0,
                        ChangeAmount = 0,
                        CreateTime = DateTime.Now,
                        IsAddedTax = false,
                        IsAdditionalTax = false,
                        IsCancel = false,
                        IsIssued = false,
                        IsLocked = false,
                        IsSf = false,
                        IssuedTime = null,
                        LockedTime = null,
                        RecordCount = 0,
                        SalaryMonth = model.SalaryMonth.Value,
                        SfAddedTaxAmount = 0,
                        ServiceFeeAmount = 0,
                        SfAddedTaxPointAmount = 0,
                        TotalAmount = companyInvoiceTotalAmount,
                        Status = InvoiceStatusEnumType.待锁定,
                        SfOtherPaidAmount = 0,
                        SfTaxRefundAmount = 0,
                        SfPersonalTaxPointAmount = 0,
                        SfPersonalTaxAmount = 0,
                        SfSocialAmount = 0,
                        IsSpecial = false,
                        LegalCustomerName = string.Empty,
                        PartnerGID = model.PartnerGID,
                        PartnerName = model.PartnerName,
                        CompanyID = model.CompanyID,
                        CustomerName = model.CustomerName,
                    };


                    companyInvoice.AddTaxRecord(companyInvoiceTotalAmount, InvoiceTaxDataTypeEnumType.自定义, 0, 0, "企业应发工资-企业扣款+管理费+企业社保");



                    companyInvoice.CalculationTax();
                    companyInvoice.AddAttachFile("发票附件.xlsx", fileOssUrlInside);
                    invoices.Add(companyInvoice);
                    #endregion

                }
            }
            Live_InvoiceExtension invoice = null;

            List<Live_SalaryRecord> records = new List<Live_SalaryRecord>();

            #region 生成外部发票
            //  联营子数据不生成外部发票过滤
            if (model.CombineCompanyGID == null || model.CombineCompanyGID.Value == Guid.Empty)
            {
                foreach (var invoiceConfig in configs)
                {
                    if (invoiceConfig.PrintTemplate == PrintTemplateEnumType.公司发票)
                    {
                        continue;
                    }
                    if (invoiceConfig.PrintTemplate == PrintTemplateEnumType.外部发票)
                    {
                        records = outRecords;
                    }
                    else
                    {
                        records = insideRecords;
                    }
                    // 增值税税点
                    var addedTaxPoint = 0.00m;
                    // 附加税税点
                    var additionanlTaxPoint = 0.00m;
                    var invoiceAmount = 0.00m;
                    invoice = new Live_InvoiceExtension()
                    {
                        ID = Guid.NewGuid(),
                        SalaryCustomerID = model.ID,
                        CustomerGID = model.CustomerGID,
                        Remark = string.Empty,
                        AddedTaxPointAmount = addedTaxPoint,
                        AdditionalTaxPointAmount = additionanlTaxPoint,
                        InvoiceAmount = invoiceAmount,
                        InvoiceType = invoiceConfig.InvoiceType,
                        Title = invoiceConfig.InvoiceTitle,
                        AddedTaxDiscountAmount = 0,
                        AdditionalTaxDiscountAmount = 0,
                        ApplyTime = DateTime.Now,
                        BusinessType = invoiceConfig.PrintTemplate,
                        AddedTaxAmount = 0,
                        AddedTaxSumAmount = 0,
                        AdditionalTaxAmount = 0,
                        AdditionTaxSumAmount = 0,
                        ChangeAmount = 0,
                        CreateTime = DateTime.Now,
                        Description = string.Empty,
                        IsAddedTax = false,
                        IsAdditionalTax = false,
                        IsCancel = false,
                        IsIssued = false,
                        IsLocked = false,
                        IsSf = false,
                        IssuedTime = null,
                        IssuerAddress = string.Empty,
                        IssuerBank = string.Empty,
                        IssuerBankNo = string.Empty,
                        IssuerCompanyNo = string.Empty,
                        IssuerName = string.Empty,
                        IssuerPhone = string.Empty,
                        LockedTime = null,
                        RecipientAddress = string.Empty,
                        RecordCount = 0,
                        SalaryMonth = model.SalaryMonth.Value,
                        SfAddedTaxAmount = 0,
                        ServiceFeeAmount = 0,
                        SfAddedTaxPointAmount = 0,
                        TotalAmount = 0,
                        Status = InvoiceStatusEnumType.待锁定,
                        SfOtherPaidAmount = 0,
                        SfTaxRefundAmount = 0,
                        SfPersonalTaxPointAmount = 0,
                        SfPersonalTaxAmount = 0,
                        SfSocialAmount = 0,
                        IsSpecial = false,
                        LegalCustomerName = invoiceConfig.LegalCustomerName,
                        PartnerGID = invoiceConfig.PartnerGID,
                        PartnerName = invoiceConfig.PartnerName,
                        CompanyID = invoiceConfig.CompanyID,
                        CustomerName = invoiceConfig.CustomerName,

                    };

                    decimal addedTaxTotalAmount = 0;
                    decimal additionanlTotalAmount = 0;


                    string description = string.Empty;

                    if (invoiceConfig.IsIncludeSalary)
                    {

                        var salaryAmount = records.Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount);//企业应发工资-企业扣款
                        invoiceAmount = salaryAmount;

                        description += $"，派遣员工工资：{salaryAmount}元";
                        if (invoiceConfig.TaxPointSalaryValueAdded > 0)
                        {
                            addedTaxTotalAmount = decimal.Round(invoiceConfig.TaxPointSalaryValueAdded * salaryAmount / 100, 2, MidpointRounding.AwayFromZero);

                            description += $"，工资增值税（{invoiceConfig.TaxPointSalaryValueAdded}%）：{addedTaxTotalAmount}元";
                        }

                        if (invoiceConfig.TaxPointSalaryAdditional > 0)
                        {
                            additionanlTotalAmount = decimal.Round(invoiceConfig.TaxPointSalaryAdditional * salaryAmount / 100, 2, MidpointRounding.AwayFromZero);

                            description += $"，工资附加税（{invoiceConfig.TaxPointSalaryAdditional}%）：{additionanlTotalAmount}元";
                        }

                        invoice.AddTaxRecord(salaryAmount, InvoiceTaxDataTypeEnumType.企业工资, invoiceConfig.TaxPointSalaryValueAdded, invoiceConfig.TaxPointSalaryAdditional, "企业应发工资-企业扣款");

                        // 应发工资-企业扣款
                        invoice.SalaryAmount = salaryAmount;
                    }

                    if (invoiceConfig.IsIncludeServiceFee)
                    {
                        var serviceAmount = records.Sum(f => f.ServiceFeeAmount + f.SocialUnitSumAmount);// 服务费+社保企业部分
                        invoiceAmount += serviceAmount;
                        invoice.ServiceFeeAmount = records.Sum(f => f.ServiceFeeAmount);


                        description += $"，服务费：{records.Sum(f => f.ServiceFeeAmount)}元，企业社保金额：{records.Sum(f => f.SocialUnitSumAmount)}元";


                        if (invoiceConfig.TaxPointServiceFeeValueAdded > 0)
                        {
                            var serviceAddedTaxTotalAmount = decimal.Round(invoiceConfig.TaxPointServiceFeeValueAdded * serviceAmount / 100, 2, MidpointRounding.AwayFromZero);
                            addedTaxTotalAmount += serviceAddedTaxTotalAmount;
                            description += $"，服务费增值税（{invoiceConfig.TaxPointServiceFeeValueAdded}%）：{serviceAddedTaxTotalAmount}元";
                        }



                        if (invoiceConfig.TaxPointServiceFeeAdditional > 0)
                        {
                            var serviceAdditionanlTotalAmount = decimal.Round(invoiceConfig.TaxPointServiceFeeAdditional * serviceAmount / 100, 2, MidpointRounding.AwayFromZero);
                            additionanlTotalAmount += serviceAdditionanlTotalAmount;
                            description += $"，服务费附加税（{invoiceConfig.TaxPointServiceFeeAdditional}%）：{serviceAdditionanlTotalAmount}元";
                        }
                        invoice.AddTaxRecord(serviceAmount, InvoiceTaxDataTypeEnumType.服务费, invoiceConfig.TaxPointServiceFeeValueAdded, invoiceConfig.TaxPointServiceFeeAdditional, "服务费+社保企业部分");
                    }
                    invoice.AddedTaxAmount = addedTaxTotalAmount;
                    invoice.AddedTaxSumAmount = addedTaxTotalAmount;
                    invoice.AdditionalTaxAmount = additionanlTotalAmount;
                    invoice.AdditionTaxSumAmount = additionanlTotalAmount;

                    invoice.InvoiceAmount = invoiceAmount;
                    invoice.TotalAmount = invoiceAmount;
                    //if (addedTaxPoint > 0)
                    //{
                    //    invoice.IsAddedTax = true;
                    //    invoice.AddedTaxPointAmount = addedTaxPoint;
                    //}
                    //if (additionanlTaxPoint > 0)
                    //{
                    //    invoice.IsAdditionalTax = true;
                    //    invoice.AdditionalTaxPointAmount = additionanlTaxPoint;
                    //}
                    //invoice.CalculationTax();



                    invoice.AddAttachFile("发票附件.xlsx", fileOssUrlOut);
                    if (invoice.BusinessType == PrintTemplateEnumType.外部发票)
                    {


                        invoice.SalaryRecordCount = records.Count(f => f.ServiceFeeAmount == 0 && f.UnitSalaryAmount > 0);


                        invoice.SerivceFeeRecordCount = records.Count(f => f.ServiceFeeAmount > 0 && f.UnitSalaryAmount == 0);


                        invoice.BothRecordCount = records.Count(f => f.ServiceFeeAmount > 0 && f.UnitSalaryAmount > 0);


                        // 商务合作单位开票给客户单位
                        invoice.IssuerUnitID = model.CooperationUnitID;
                        invoice.IssuerName = model.CooperationUnitCompanyName;
                        invoice.IssuerCompanyNo = model.CooperationUnitCompanyNo;
                        invoice.IssuerPhone = model.CooperationUnitCompanyPhone;
                        invoice.IssuerBank = model.CooperationUnitCompanyBank;
                        invoice.IssuerBankNo = model.CooperationUnitCompanyBankNo;
                        invoice.IssuerAddress = model.CooperationUnitCompanyAddress;

                        invoice.RecipientUnitID = model.CustomerGID;
                        invoice.RecipientName = model.CustomerName;
                        invoice.RecipientCompanyNo = model.CustomerCompanyNo;
                        invoice.RecipientPhone = model.CustomerCompanyPhone;
                        invoice.RecipientBank = model.CustomerCompanyBank;
                        invoice.RecipientBankNo = model.CustomerCompanyBankNo;
                        invoice.RecipientAddress = model.CustomerCompanyAddress;
                        invoice.Description = $"{model.CustomerName}（{model.SalaryMonth.Value.Month}月）";


                        invoice.RecordCount = records.Count;


                        if (!string.IsNullOrWhiteSpace(model.WorkShop))
                        {
                            invoice.Description += $"{model.WorkShop}车间";
                        }
                        invoice.Description += description;


                        //if (invoiceConfig.IsIncludeSalary)
                        //{
                        //    invoice.Description += $"，派遣员工工资：{records.Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount)}元";
                        //}

                        //if (invoiceConfig.IsIncludeServiceFee)
                        //{
                        //    invoice.Description += $"，服务费：{records.Sum(f => f.ServiceFeeAmount)}元，企业社保金额：{records.Sum(f => f.SocialUnitSumAmount)}";
                        //}
                        //// 增值税
                        //if (invoice.AddedTaxPointAmount > 0)
                        //{
                        //    invoice.Description += $"，增值税（{invoice.AddedTaxPointAmount} %）：{invoice.AddedTaxAmount}元（优惠金额{invoice.AddedTaxDiscountAmount}元）";
                        //}

                        //if (invoice.AdditionalTaxPointAmount > 0)
                        //{
                        //    invoice.Description += $"，附加税：{invoice.AdditionalTaxAmount}元（优惠金额：{invoice.AdditionalTaxDiscountAmount}元）";
                        //}

                    }
                    invoice.SalaryCustomerID = model.ID;
                    invoice.AddAttachFile("发票附件.xlsx", fileOssUrlOut);
                    invoices.Add(invoice);
                }
            }
           
            #endregion


            if (invoices.Any())
            {

                try
                {
                    var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                    DbScoped.SugarScope.BeginTran();
                    foreach (var item in invoices)
                    {
                        if (existInvoiceList.Any(f => f.Title == item.Title && f.BusinessType == item.BusinessType))
                        {
                            continue;
                        }
                        if (model.IsCombineSalary)
                        {
                            item.Status = InvoiceStatusEnumType.待审核;
                            item.LockedTime = DateTime.Now;
                        }
                        if (info != null)
                        {
                            item.ApplyUserName = info.UserName;
                            item.ApplyUserID = info.UserID;
                            item.ApplyUserFullName = info.FullName;
                        }
                        item.CalculationTax();
                        await _invoiceService.AddNav(item);
                    }
                    DbScoped.SugarScope.CommitTran();
                }
                catch (Exception ex)
                {
                    DbScoped.SugarScope.RollbackTran();

                    throw new ErrorCodeException(-1, ex.Message);
                }

               
            }
            return Ok();
        }



        [HttpPut]
        public async Task<IActionResult> GenerateCombineInvoice([FromForm]Guid id)
        {

            var setting = await _settingsService.GetMasterSettings();
            var model = await _salaryCustomerService.GetView(id);

            if (model.CombineCompanyGID==null|| model.CombineCompanyGID.Value==Guid.Empty)
            {
                throw new ErrorCodeException(-1, "非联工资无法执行此操作");
            }

            Live_V_Customer customerInfo = null;

            //客户单位信息
            var customerRes = await _financeOtherService.GetCustomerByGIDAsync(model.CombineCompanyGID.Value);
            if (customerRes != null && customerRes.Code == 200 && customerRes.Data != null)
            {
                customerInfo = customerRes.Data;
            }
            var allRecords = await _salaryRecordService.ListView(f =>
             f.IsCombineSalary == false
             && f.CombineCompanyGID == model.CombineCompanyGID
             && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资
             && f.SalaryBatchType == model.SalaryBatchType
             && f.TaxMonth == model.TaxMonth
             && f.SalaryYear == model.SalaryYear
             );
           



            var combineSalaryList = await _salaryCustomerService.List(f => f.CombineCompanyGID == model.CombineCompanyGID && f.TaxMonth == model.TaxMonth && f.SalaryYear == f.SalaryYear && f.SalaryBatchType == model.SalaryBatchType);

            if (combineSalaryList.Any(f=>f.IsCombineSalary))
            {
                throw new ErrorCodeException(-1, "此可单位当前批次已生成过联合票据，无法重复执行");
            }


            //foreach (var item in combineSalaryList)
            //{
            //    item.IsCombineSalary = true;
            //}

            model.IsCombineSalary = true;
            
            var idList = combineSalaryList.Select(f => f.ID).ToList();
            var recordList = await _salaryRecordService.List(f => f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && idList.Contains(f.SalaryCustomerID));
            List<Live_InvoiceExtension> invoices = new List<Live_InvoiceExtension>();
            string fileOssUrlOut = string.Empty;
            var invoiceConfig = await _invoiceConfigService.GetView(f => f.PrintTemplate == PrintTemplateEnumType.外部发票 && f.CustomerGID == model.CombineCompanyGID && f.IsEnabled);
            if (invoiceConfig == null)
            {
                throw new ErrorCodeException(-1, "外部发票配置缺失");
            }
            #region 生成附件
            var excelData = recordList.Select(f => new SalaryInvoiceExcelTemplate()
            {
                CustomerName = f.CustomerName,
                CooperationUnitCompanyName = model.CooperationUnitCompanyName,
                SalaryMonth = model.SalaryMonth.Value,
                Name = f.Name,
                IdCardNo = f.IdCardNo,
                Remark = f.Remark,
                EntryDate = f.EntryDate.Value,
                LeaveDate = f.LeaveDate,
                ServiceFeeAmount = f.ServiceFeeAmount,
                UnitDeductionAmount = f.UnitDeductionAmount,
                UnitSalaryAmount = f.UnitSalaryAmount,
            }).ToList();
            excelData.Add(new SalaryInvoiceExcelTemplate()
            {
                CustomerName = "合计",
                CooperationUnitCompanyName = string.Empty,
                SalaryMonth = model.SalaryMonth.Value,
                Name = string.Empty,
                IdCardNo = string.Empty,
                Remark = string.Empty,
                EntryDate = null,
                LeaveDate = null,
                ServiceFeeAmount = excelData.Sum(f => f.ServiceFeeAmount),
                UnitDeductionAmount = excelData.Sum(f => f.UnitDeductionAmount),
                UnitSalaryAmount = excelData.Sum(f => f.UnitSalaryAmount),
            });
            using (var ms = new MemoryStream())
            {
                var fileBytes = NpoiExcelHelper.WriteExcel<SalaryInvoiceExcelTemplate>(excelData, "客户工资附件");
                ms.Write(fileBytes);
                var ossResp = await _aliyunOSSService.UploadFile(ms, $"{model.CustomerName}.xlsx", true, false, "invoiceAttachFiles");
                if (ossResp.Code == 0)
                {
                    fileOssUrlOut = setting.AliBucketDomain + ossResp.Data;
                }
                else
                {
                    throw new ErrorCodeException(-1, "客户工资上传oss失败");
                }
            }
            #endregion


            Live_InvoiceExtension invoice = null;

            List<Live_SalaryRecord> records = new List<Live_SalaryRecord>();
            records = recordList;

            #region 生成外部发票


            // 增值税税点
            var addedTaxPoint = 0.00m;
            // 附加税税点
            var additionanlTaxPoint = 0.00m;
            var invoiceAmount = 0.00m;
            invoice = new Live_InvoiceExtension()
            {
                ID = Guid.NewGuid(),
                SalaryCustomerID = model.ID,
                CustomerGID = model.CustomerGID,
                Remark = string.Empty,
                AddedTaxPointAmount = addedTaxPoint,
                AdditionalTaxPointAmount = additionanlTaxPoint,
                InvoiceAmount = invoiceAmount,
                InvoiceType = invoiceConfig.InvoiceType,
                Title = invoiceConfig.InvoiceTitle,
                AddedTaxDiscountAmount = 0,
                AdditionalTaxDiscountAmount = 0,
                ApplyTime = DateTime.Now,
                BusinessType = invoiceConfig.PrintTemplate,
                AddedTaxAmount = 0,
                AddedTaxSumAmount = 0,
                AdditionalTaxAmount = 0,
                AdditionTaxSumAmount = 0,
                ChangeAmount = 0,
                CreateTime = DateTime.Now,
                Description = string.Empty,
                IsAddedTax = false,
                IsAdditionalTax = false,
                IsCancel = false,
                IsIssued = false,
                IsLocked = false,
                IsSf = false,
                IssuedTime = null,
                IssuerAddress = string.Empty,
                IssuerBank = string.Empty,
                IssuerBankNo = string.Empty,
                IssuerCompanyNo = string.Empty,
                IssuerName = string.Empty,
                IssuerPhone = string.Empty,
                LockedTime = null,
                RecipientAddress = string.Empty,
                RecordCount = 0,
                SalaryMonth = model.SalaryMonth.Value,
                SfAddedTaxAmount = 0,
                ServiceFeeAmount = 0,
                SfAddedTaxPointAmount = 0,
                TotalAmount = 0,
                Status = InvoiceStatusEnumType.待锁定,
                SfOtherPaidAmount = 0,
                SfTaxRefundAmount = 0,
                SfPersonalTaxPointAmount = 0,
                SfPersonalTaxAmount = 0,
                SfSocialAmount = 0,
                IsSpecial = false,
                LegalCustomerName = customerInfo.LegalCustomerName,
                PartnerGID = customerInfo.PartnerGID,
                PartnerName = customerInfo.PartnerName,
                CompanyID = customerInfo.CompanyID,
                CustomerName = invoiceConfig.CustomerName,
                

            };

            decimal addedTaxTotalAmount = 0;
            decimal additionanlTotalAmount = 0;


            string description = string.Empty;

            if (invoiceConfig.IsIncludeSalary)
            {

                var salaryAmount = records.Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount);//企业应发工资-企业扣款
                invoiceAmount = salaryAmount;

                description += $"，派遣员工工资：{salaryAmount}元";
                if (invoiceConfig.TaxPointSalaryValueAdded > 0)
                {
                    addedTaxTotalAmount = decimal.Round(invoiceConfig.TaxPointSalaryValueAdded * salaryAmount / 100, 2, MidpointRounding.AwayFromZero);

                    description += $"，工资增值税（{invoiceConfig.TaxPointSalaryValueAdded}%）：{addedTaxTotalAmount}元";
                }

                if (invoiceConfig.TaxPointSalaryAdditional > 0)
                {
                    additionanlTotalAmount = decimal.Round(invoiceConfig.TaxPointSalaryAdditional * salaryAmount / 100, 2, MidpointRounding.AwayFromZero);

                    description += $"，工资附加税（{invoiceConfig.TaxPointSalaryAdditional}%）：{additionanlTotalAmount}元";
                }
                invoice.AddTaxRecord(salaryAmount, InvoiceTaxDataTypeEnumType.企业工资, invoiceConfig.TaxPointSalaryValueAdded, invoiceConfig.TaxPointSalaryAdditional, "企业应发工资-企业扣款");
                // 应发工资-企业扣款
                invoice.SalaryAmount = salaryAmount;
            }

            if (invoiceConfig.IsIncludeServiceFee)
            {
                var serviceAmount = records.Sum(f => f.ServiceFeeAmount + f.SocialUnitSumAmount);// 服务费+社保企业部分
                invoiceAmount += serviceAmount;
                invoice.ServiceFeeAmount = records.Sum(f => f.ServiceFeeAmount);


                description += $"，服务费：{records.Sum(f => f.ServiceFeeAmount)}元，企业社保金额：{records.Sum(f => f.SocialUnitSumAmount)}元";


                if (invoiceConfig.TaxPointServiceFeeValueAdded > 0)
                {
                    var serviceAddedTaxTotalAmount = decimal.Round(invoiceConfig.TaxPointServiceFeeValueAdded * serviceAmount / 100, 2, MidpointRounding.AwayFromZero);
                    addedTaxTotalAmount += serviceAddedTaxTotalAmount;
                    description += $"，服务费增值税（{invoiceConfig.TaxPointServiceFeeValueAdded}%）：{serviceAddedTaxTotalAmount}元";
                }



                if (invoiceConfig.TaxPointServiceFeeAdditional > 0)
                {
                    var serviceAdditionanlTotalAmount = decimal.Round(invoiceConfig.TaxPointServiceFeeAdditional * serviceAmount / 100, 2, MidpointRounding.AwayFromZero);
                    additionanlTotalAmount += serviceAdditionanlTotalAmount;
                    description += $"，服务费附加税（{invoiceConfig.TaxPointServiceFeeAdditional}%）：{serviceAdditionanlTotalAmount}元";
                }
                invoice.AddTaxRecord(serviceAmount, InvoiceTaxDataTypeEnumType.服务费, invoiceConfig.TaxPointServiceFeeValueAdded, invoiceConfig.TaxPointServiceFeeAdditional, "服务费+社保企业部分");
            }
            invoice.AddedTaxAmount = addedTaxTotalAmount;
            invoice.AddedTaxSumAmount = addedTaxTotalAmount;
            invoice.AdditionalTaxAmount = additionanlTotalAmount;
            invoice.AdditionTaxSumAmount = additionanlTotalAmount;

            invoice.InvoiceAmount = invoiceAmount;
            invoice.TotalAmount = invoiceAmount;


            invoice.AddAttachFile("发票附件.xlsx", fileOssUrlOut);
            if (invoice.BusinessType == PrintTemplateEnumType.外部发票)
            {


                invoice.SalaryRecordCount = records.Count(f => f.ServiceFeeAmount == 0 && f.UnitSalaryAmount > 0);


                invoice.SerivceFeeRecordCount = records.Count(f => f.ServiceFeeAmount > 0 && f.UnitSalaryAmount == 0);


                invoice.BothRecordCount = records.Count(f => f.ServiceFeeAmount > 0 && f.UnitSalaryAmount > 0);


                // 商务合作单位开票给客户单位
                invoice.IssuerUnitID = customerInfo.CooperationUnitGID.Value;
                invoice.IssuerName = customerInfo.CooperationUnitName;
                invoice.IssuerCompanyNo = customerInfo.CooperationUnitCompanyNo;
                invoice.IssuerPhone = customerInfo.CooperationUnitPhone;
                invoice.IssuerBank = customerInfo.CooperationUnitBankName;
                invoice.IssuerBankNo = customerInfo.CooperationUnitBankNo;
                invoice.IssuerAddress = customerInfo.CooperationUnitAddress;

                invoice.RecipientUnitID = customerInfo.CustomerGID.Value;
                invoice.RecipientName = customerInfo.CustomerName;
                invoice.RecipientCompanyNo = customerInfo.CompanyNo;
                invoice.RecipientPhone = customerInfo.Phone;
                invoice.RecipientBank = customerInfo.Bank;
                invoice.RecipientBankNo = customerInfo.BankAccount;
                invoice.RecipientAddress = customerInfo.Address;
                invoice.Description = $"{customerInfo.CustomerName}（{model.SalaryMonth.Value.Month}月）";


                invoice.RecordCount = records.Count;


                if (!string.IsNullOrWhiteSpace(model.WorkShop))
                {
                    invoice.Description += $"{model.WorkShop}车间";
                }
                invoice.Description += description;


                //if (invoiceConfig.IsIncludeSalary)
                //{
                //    invoice.Description += $"，派遣员工工资：{records.Sum(f => f.UnitSalaryAmount - f.UnitDeductionAmount)}元";
                //}

                //if (invoiceConfig.IsIncludeServiceFee)
                //{
                //    invoice.Description += $"，服务费：{records.Sum(f => f.ServiceFeeAmount)}元，企业社保金额：{records.Sum(f => f.SocialUnitSumAmount)}";
                //}
                //// 增值税
                //if (invoice.AddedTaxPointAmount > 0)
                //{
                //    invoice.Description += $"，增值税（{invoice.AddedTaxPointAmount} %）：{invoice.AddedTaxAmount}元（优惠金额{invoice.AddedTaxDiscountAmount}元）";
                //}

                //if (invoice.AdditionalTaxPointAmount > 0)
                //{
                //    invoice.Description += $"，附加税：{invoice.AdditionalTaxAmount}元（优惠金额：{invoice.AdditionalTaxDiscountAmount}元）";
                //}

            }
            invoice.SalaryCustomerID = model.ID;
            invoice.AddAttachFile("发票附件.xlsx", fileOssUrlOut);
            invoices.Add(invoice);
            #endregion


            if (invoices.Any())
            {

                try
                {
                    var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                    DbScoped.SugarScope.BeginTran();
                    foreach (var item in invoices)
                    {
                        if (model.IsCombineSalary)
                        {
                            item.Status = InvoiceStatusEnumType.待锁定;
                        }
                        if (info != null)
                        {
                            item.ApplyUserName = info.UserName;
                            item.ApplyUserID = info.UserID;
                            item.ApplyUserFullName = info.FullName;
                        }
                        item.CalculationTax();
                        await _invoiceService.AddNav(item);
                    }

                    var currentModel = combineSalaryList.FirstOrDefault(f => f.ID == model.ID);
                    currentModel.IsCombineSalary = true;
                    await _salaryCustomerService.Update(currentModel);

                    DbScoped.SugarScope.CommitTran();
                }
                catch (Exception ex)
                {
                    DbScoped.SugarScope.RollbackTran();

                    throw new ErrorCodeException(-1, ex.Message);
                }

            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> CombineSalary([FromForm] Guid id)
        {
            var model = await _salaryCustomerService.Get(id);
            if (model.CombineCompanyGID == null || model.CombineCompanyGID.Value == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "未指定联合开票单位");
            }
            var salaryList = await _salaryCustomerService.List(f => f.CombineCompanyGID != null && f.CombineCompanyGID.Value == model.CombineCompanyGID.Value && f.TaxMonth == model.TaxMonth && f.SalaryBatchType == model.SalaryBatchType && f.SalaryYear == model.SalaryYear);
            Live_V_Customer customerInfo = null;
            //客户单位信息
            var customerRes = await _financeOtherService.GetCustomerByGIDAsync(model.CombineCompanyGID.Value);
            if (customerRes != null && customerRes.Code == 200 && customerRes.Data != null)
            {
                customerInfo = customerRes.Data;
            }
            var allRecords = await _salaryRecordService.ListView(f =>
             f.IsCombineSalary == false
             && f.CombineCompanyGID == model.CombineCompanyGID
             && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资
             && f.SalaryBatchType == model.SalaryBatchType
             && f.TaxMonth == model.TaxMonth
             && f.SalaryYear == model.SalaryYear
             );
            List<Live_SalaryRecordExtension> salaryRecordExtensions = allRecords.Select(f => new Live_SalaryRecordExtension()
            {

                ID = Guid.NewGuid(),
                //CompanyName = f.CompanyName,
                UnitDeductionAmount = f.UnitDeductionAmount,
                UnitSalaryAmount = f.UnitSalaryAmount,
                CreateTime = DateTime.Now,
                CustomerName = customerInfo.CustomerName,
                DeductionNoTaxAmount = f.DeductionNoTaxAmount,
                SalaryMonth = f.SalaryMonth,
                CompanyName = f.CustomerName,
                IdCardNo = f.IdCardNo,
                SocialGongJiJinAmount = f.SocialGongJiJinAmount,
                IsDelayPayment = f.IsDelayPayment,
                Name = f.Name,
                PlatformCommissionAmount = f.PlatformCommissionAmount,
                PlatformDepartment = f.PlatformDepartment,
                PlatformWelfareAmount = f.PlatformWelfareAmount,
                Remark = f.Remark,
                SendMonth = f.SalaryMonth,
                ServiceFeeAmount = f.ServiceFeeAmount,
                SocialUnitSumAmount = f.SocialUnitSumAmount,
                SocialDaBingAmount = f.SocialDaBingAmount,
                SocialShiYeAmount = f.SocialShiYeAmount,
                SocialYangLaoAmount = f.SocialYangLaoAmount,
                SocialYiLiaoAmount = f.SocialYiLiaoAmount,
                SubsidyAmount = f.SubsidyAmount,
                SubsidyNoTaxAmount = f.SubsidyNoTaxAmount,
                Workshop = string.IsNullOrWhiteSpace(f.Workshop) ? string.Empty : f.Workshop,
                SpJiaoYuAmount = f.SpJiaoYuAmount,
                SpZiNvAmount = f.SpZiNvAmount,
                SpDaiKuanAmount = f.SpDaiKuanAmount,
                SpZuJinAmount = f.SpZuJinAmount,
                SpShanYangAmount = f.SpShanYangAmount,
                SpYingErAmount = f.SpYingErAmount,
                InnerServiceFeeAmount = f.InnerServiceFeeAmount,
                ActualSalaryAmount = f.ActualSalaryAmount,
                //SalaryCustomerID = null,
                SocialDaBingTotalAmount = f.SocialDaBingTotalAmount,
                SocialShiYeTotalAmount = f.SocialShiYeTotalAmount,
                SocialYangLaoTotalAmount = f.SocialYangLaoTotalAmount,
                SocialYiLiaoTotalAmount = f.SocialYiLiaoTotalAmount,
                TaxSpDaiKuanTotalAmount = f.TaxSpDaiKuanTotalAmount,
                TaxSpJiaoYuTotalAmount = f.TaxSpJiaoYuTotalAmount,
                TaxSpShanYangTotalAmount = f.TaxSpShanYangTotalAmount,
                TaxSpTotalAmount = f.TaxSpTotalAmount,
                TaxSpYingErTotalAmount = f.TaxSpYingErTotalAmount,
                TaxSpZiNvTotalAmount = f.TaxSpZiNvTotalAmount,
                TaxSpZuJinTotalAmount = f.TaxSpZuJinTotalAmount,
                TaxTimes = f.TaxTimes,
                TaxMonthAmount =f.TaxMonthAmount,
                BeforeTaxSalaryAmount = f.BeforeTaxSalaryAmount,
                BeforeTaxSalaryTotalAmount = f.BeforeTaxSalaryTotalAmount,
                TaxSalaryTotalAmount = f.TaxSalaryTotalAmount,
                SpBasicTotalAmount = f.SpBasicTotalAmount,
                //SalaryType =  SalaryRecordTypeEnumType.常规工资,
                IsCashSend = f.IsCashSend,
                AfterTaxAmount = f.AfterTaxAmount,
                Bank = f.Bank,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                CompanySalarySumAmount = f.CompanySalarySumAmount,
                CompanyTotalAmount = f.CompanyTotalAmount,
                EntryDate = f.EntryDate,
                LeaveDate = f.LeaveDate,
                PreSendAmount = f.PreSendAmount,
                SocialEmpSumAmount = f.SocialEmpSumAmount,
                UserEntryID = f.UserEntryID,
                TaxTotalAmount = f.TaxTotalAmount,
                LastTaxTotalAmount = f.LastTaxTotalAmount,
                LastBeforeTaxSalaryTotalAmount = f.LastBeforeTaxSalaryTotalAmount,
                LastSocialDaBingTotalAmount = f.LastSocialDaBingTotalAmount,
                LastSocialEmpTotalAmount = f.LastSocialEmpTotalAmount,
                LastSocialGongJiJinTotalAmount = f.LastSocialGongJiJinTotalAmount,
                LastSocialShiYeTotalAmount = f.LastSocialShiYeTotalAmount,
                LastSocialYangLaoTotalAmount = f.LastSocialYangLaoTotalAmount,
                LastSocialYiLiaoTotalAmount = f.LastSocialYiLiaoTotalAmount,
                SocialEmpTotalAmount = f.SocialEmpTotalAmount,
                SocialGongJiJinTotalAmount = f.SocialGongJiJinTotalAmount,
                SpBasiclAmount = f.SpBasiclAmount,
                SpSumAmount = f.SpSumAmount,
                SpTotalAmount = f.SpTotalAmount,
                TaxSocialTotalAmount = f.TaxSocialTotalAmount,
                VerifyInfoOk = true,
                ActualSalarySendCardAmount = f.ActualSalarySendCardAmount,
                IsCashSignSuc = false,
                IsDelayPaymentCancel = false,
                MonthNetSalaryAmount = f.MonthNetSalaryAmount,
                PrepaidRepaymentAmount = f.PrepaidRepaymentAmount,
                //SalaryDelayType = f.IsDelayPayment ? SalaryDelaySendTypeEnumType.客服暂缓 : SalaryDelaySendTypeEnumType.无,
                Status = SalaryRecordSendStatusEnumType.不发放,
                SendType = SalaryRecordSendTypeEnumType.线上发放,
                Phone =f.Phone,
                BankChangeFilePath = string.Empty,
                DelayType = SalaryDelaySendTypeEnumType.未暂缓,
                IsDelayPaymentSuc = false,
                UnitManageAmount = f.UnitManageAmount,
                MonthWorkDays = f.MonthWorkDays,
                //IsPushTrans = false,
                IsTransBatch = false,
                SettlementID = null,
                //TransNonce = Guid.NewGuid(),
                TransStatus = string.Empty,
                TransStatusName = string.Empty,
                SalaryYear = f.SalaryYear,
                SalaryRecordType = SalaryRecordTypeEnumType.合并开票工资,
                BatchTransStatus = string.Empty,
                BatchTransStatusName = string.Empty,
                DelayPaymentAmount = 0,
                IsBatchPushSuc = false,
                IsBatchSend = false,
                PrepaidTotalAmount = 0,
                IsSinglePushSuc = false,
                IsSingleSend = false,
                UserID = 0,
                IsCombineSalary = true

            }).ToList();
            var salaryCustomer = new Live_SalaryCustomerExtension()
            {
                ID = Guid.NewGuid(),
                IsCustomerPaid = false,
                // 客户信息
                CustomerGID = customerInfo.CustomerGID.Value,
                CustomerName = customerInfo.CustomerName,
                CustomerCompanyAddress = customerInfo.Address,
                CustomerCompanyBank = customerInfo.Bank,
                CustomerCompanyBankNo = customerInfo.BankAccount,
                CustomerCompanyNo = customerInfo.CompanyNo,
                CustomerCompanyPhone = customerInfo.Phone,
                CooperationUnitID = customerInfo.CooperationUnitGID.Value,
                // 签章单位信息
                SigningUnitID = customerInfo.SigningUnitGID.Value,
                SigningUnitCompanyName = customerInfo.SigningUnitName,
                SigningUnitCompanyNo = customerInfo.SigningUnitCompanyNo,
                SigningUnitCompanyBank = customerInfo.SigningUnitBankName,
                SigningUnitCompanyBankNo = customerInfo.SigningUnitBankNo,
                SigningUnitCompanyAddress = customerInfo.signingUnitAddress,
                SigningUnitCompanyPhone = customerInfo.SigningUnitPhone,
                CooperationUnitCompanyName = customerInfo.CooperationUnitName,
                CooperationUnitCompanyAddress = customerInfo.CooperationUnitAddress,
                CooperationUnitCompanyBank = customerInfo.CooperationUnitBankName,
                CooperationUnitCompanyBankNo = customerInfo.CooperationUnitBankNo,
                CooperationUnitCompanyNo = customerInfo.CooperationUnitCompanyNo,
                CooperationUnitCompanyPhone = customerInfo.CooperationUnitPhone,
                ActualSalaryAmount = 0,
                ManagementFeeAmount = 0,
                SalaryBatchType = model.SalaryBatchType,
                SalarySendType = 0,
                Status = SalaryStatusEnumType.SucessSend,
                TaxMonth = model.TaxMonth,
                //CompanyGID = customerInfo.CompanyGID.Value,
                CreateTime = DateTime.Now,
                SealCompanyName = string.Empty,
                SendMonth = model.SendMonth,
                SalaryMonth = model.SalaryMonth,
                CompanyDeductionAmount = salaryRecordExtensions.Sum(f => f.UnitDeductionAmount),
                CompanySendSalaryAmount = salaryRecordExtensions.Sum(f => f.UnitSalaryAmount),
                CompanySocialSecurityAmount = salaryRecordExtensions.Sum(f => f.SocialUnitSumAmount),
                DeductionNoTaxAmount = salaryRecordExtensions.Sum(f => f.DeductionNoTaxAmount),
                ServiceFeeAmount = salaryRecordExtensions.Sum(f => f.ServiceFeeAmount),
                SubsidyAmount = salaryRecordExtensions.Sum(f => f.SubsidyAmount),
                SubsidyNoTaxAmount = salaryRecordExtensions.Sum(f => f.SubsidyNoTaxAmount),
                WorkShop = model.WorkShop,
                Records = salaryRecordExtensions.ToList(),
                AccountsReceivableSumAmount = 0,
                CompanySendSalarySumAmount = 0,
                CompanySettlementSumAmount = 0,
                InnerServiceFeeAmount = 0,
                ActualSalarySumAmount = 0,
                ServiceFeeRecordCount = 0,
                IsCompanyPosted = false,
                IsMainBatchSalary = true,
                IsMatchZeroSalary = false,
                BeforeTaxSalarySumAmount = 0,
                CustomerBillTotalAmount = 0,
                MonthNetSalaryAmount = 0,
                MonthNetSalarySumAmount = 0,
                ReceivableTotalAmount = 0,
                ServiceFeeSumAmount = 0,
                ServiceRecordCount = 0,
                SocialUnitSumAmount = 0,
                UnitDeductionSumAmount = 0,
                UnitManageSumAmount = 0,
                UnitSalarySumAmount = 0,
                UnitSalaryTotalAmount = 0,
                UnitSettlementAmount = 0,
                RecordCount = 0,
                ZeroSalaryCount = 0,
                IsCmbcSend = false,
                IsPushTrans = false,
                TransNonce = Guid.NewGuid(),
                TransStatus = string.Empty,
                TransStatusName = string.Empty,
                SalaryYear = model.SalaryYear,
                SalaryType = model.SalaryType,
                IsCanCmbcSend = false,
                IsPlatformPrepaid = false,
                SalarySlipID = null,
                IsSalarySlip = false,
                IsHasOutInvoice = false,
                IsCombineSalary = true,
                CombineCompanyGID = null,
                CombineCustomerSalaryID = string.Empty,
                CompanyID=customerInfo.CompanyID,
                 IsNoticeCompleted=true,
                
            };
            salaryCustomer.SalaryCount();
            try
            {
                DbScoped.SugarScope.BeginTran();
                await _salaryCustomerService.AddNav(salaryCustomer);
                await GenerateInvoice(salaryCustomer.ID);
                await GenerateSettlement(salaryCustomer.ID);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message);
            }
            return Ok();
        }
        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SalaryReset(Guid id)
        {
            try
            {
                var model = await _salaryCustomerService.GetNav(f => f.ID == id);


                if (model.SalaryBatchType>1)
                {
                  var moreBatchList=await  _salaryCustomerService.List(f =>f.CustomerName==model.CustomerName&&f.TaxMonth==model.TaxMonth&& f.SalaryBatchType > model.SalaryBatchType&&f.SalaryYear==model.SalaryYear);

                    if (moreBatchList!=null&& moreBatchList.Count>0)
                    {
                        throw new ErrorCodeException(-1,"多批次工资重置，请先重置大于此批次的客户工资");
                    }
                }


                model.Reset();
                DbScoped.SugarScope.BeginTran();
                var settlementIDs = model.Records.Where(f => f.SettlementID != null).Select(f => f.SettlementID);
                // 删除暂缓
                await _delaySendRecordService.Delete(f => f.SalaryCustomerID == model.ID);
                // 删除现金
                await _cashSendRecordService.Delete(f => f.SalaryCustomerID == model.ID);
                // 删除异常
                await _failRecordService.Delete(f => f.SalaryCustomerID == model.ID);
                //删除 发票
                await _invoiceService.Delete(f => f.SalaryCustomerID == model.ID&&f.Status!= InvoiceStatusEnumType.已开具);
                //删除结算单
                await _settlementService.Delete(f => f.SalaryCustomerID == model.ID);

                if (settlementIDs != null && settlementIDs.Count() > 0)
                {
                    await _settlementService.Delete(f => settlementIDs.Contains(f.ID));
                }

                if (model.BudgetApplyRecordID.HasValue && model.BudgetApplyRecordID.Value > 0)
                {
                    var budgetApplyRecord = await _budgetApplyRecordService.Get(model.BudgetApplyRecordID.Value);

                    if (budgetApplyRecord != null && budgetApplyRecord.ProcessStatus != 5)
                    {
                        await _financeOtherService.BudgetApplyRecordUpdateStatus(model.BudgetApplyRecordID.Value, 5, "客户工资重置");
                    }
                }

                // 删除拆分记录
                if (model.Records.Any(f => f.IsSplitSend))
                {
                    var recordIdList = model.Records.Where(f => f.IsSplitSend).Select(f => f.ID).ToList();
                    await _splitSendService.DeleteNav(f => recordIdList.Contains(f.SalaryRecordID));
                }


                var applyRecordList = await _prepaidApplyRecordService.ListNav(f =>
                  f.CustomerGID == model.CustomerGID
                 && f.Status == PrepaidRecordStatusEnumType.已发放
                 && f.RepayStatus != PrepaidRepayStatusEnumType.还款完成);
                foreach (var item in applyRecordList)
                {
                    if (item.RepaymentRecords != null && item.RepaymentRecords.Count > 0)
                    {
                        item.RepaymentRecords = item.RepaymentRecords.Where(f => f.SalaryCustomerID != model.ID).ToList();
                    }
                    item.Count();
                }

                await _prepaidApplyRecordService.UpdateNav(applyRecordList);
                await _salaryCustomerService.UpdateTransNav(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1,ex.Message,ex);
            }
            return Ok();
        }


        /// <summary>
        /// 设置倒待发放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> FailToPendingSend([FromForm] Guid id)
        {

           var model=await _salaryCustomerService.Get(f => f.ID == id);

            if (model.Status!=  SalaryStatusEnumType.FailSend)
            {
                throw new ErrorCodeException(-1,"此状态无法执行此操作");
            }
            await _salaryCustomerService.RollbackToPendingSend(id);

            return Ok();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            try
            {
                var model = await _salaryCustomerService.GetNav(f => f.ID == id);

                DbScoped.SugarScope.BeginTran();
                var settlementIDs = model.Records.Where(f => f.SettlementID != null).Select(f => f.SettlementID);
                // 删除暂缓
                await _delaySendRecordService.Delete(f => f.SalaryCustomerID == model.ID);
                // 删除现金
                await _cashSendRecordService.Delete(f => f.SalaryCustomerID == model.ID);
                // 删除异常
                await _failRecordService.Delete(f => f.SalaryCustomerID == model.ID);
                //删除 发票 非已开具状态发票
                await _invoiceService.Delete(f => f.SalaryCustomerID == model.ID && f.Status != InvoiceStatusEnumType.已开具);
                //删除结算单
                await _settlementService.Delete(f => f.SalaryCustomerID == model.ID);

                if (settlementIDs != null && settlementIDs.Count() > 0)
                {
                    await _settlementService.Delete(f => settlementIDs.Contains(f.ID));
                }
                var applyRecordList = await _prepaidApplyRecordService.ListNav(f =>
                f.CustomerGID == model.CustomerGID
                 && f.Status == PrepaidRecordStatusEnumType.已发放
                && f.RepayStatus != PrepaidRepayStatusEnumType.还款完成);
                foreach (var item in applyRecordList)
                {
                    if (item.RepaymentRecords != null && item.RepaymentRecords.Count > 0)
                    {
                        item.RepaymentRecords = item.RepaymentRecords.Where(f => f.SalaryCustomerID != model.ID).ToList();
                    }
                    item.Count();
                }

                if (model.BudgetApplyRecordID.HasValue && model.BudgetApplyRecordID.Value > 0)
                {
                    var budgetApplyRecord = await _budgetApplyRecordService.Get(model.BudgetApplyRecordID.Value);

                    if (budgetApplyRecord != null && budgetApplyRecord.ProcessStatus != 5)
                    {
                        await _financeOtherService.BudgetApplyRecordUpdateStatus(model.BudgetApplyRecordID.Value, 5, "客户工资重置");
                    }
                }
                await _prepaidApplyRecordService.UpdateNav(applyRecordList);
                await _salaryCustomerService.DeleteNav(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message, ex);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadSpecialDeduction(IFormFile file)
        {

            var houseRentCityRes = await _financeOtherService.GetDeductionHouseRentCityListAsync("专项扣除住房租金支出");


            List<Live_SalarySpecialDedcution> dedcutions = new List<Live_SalarySpecialDedcution>();
            var year = DateTime.Today.Year;
            year = 2024;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionZNJYDto>(ms, "子女教育支出");

                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 2000.00m,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.子女教育支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    Name = f.Name,
                    RelationIdCardNo = f.RelationIdCardNo,
                    RelationName = f.RelationName,
                    Relationship = f.Relationship,
                    TaxCountMonth = DateTime.Today,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionZYZGDto>(ms, "继续教育支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 3600,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.继续教育支出,
                    StartMonth = f.StartMonth,// 一次性扣费
                    EndMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 3600,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "职业资格继续教育情况"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionJXJYDto>(ms, "继续教育支出");

                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 400m,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.继续教育支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 400m,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "学历（学位）继续教育情况"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);

                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionGJJDKDto>(ms, "住房贷款利息支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 1000m,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.住房贷款利息支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 1000m,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "公积金贷款信息"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionSYDKDto>(ms, "住房贷款利息支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 1000,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.住房贷款利息支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 1000,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "商业贷款信息"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionZFZJDto>(ms, "住房租金支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 0,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.住房租金支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 0,
                    Name = f.Name,
                    WorkCity = f.WorkCity,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionSYLRDto>(ms, "赡养老人支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = f.BaseAmount,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.赡养老人支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = f.BaseAmount,
                    Name = f.Name,
                    RelationIdCardNo = f.RelationIdCardNo,
                    RelationName = f.RelationName,
                    Relationship = f.Relationship,
                    TaxCountMonth = DateTime.Today,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionYYEZHFDto>(ms, "3岁以下婴幼儿照护支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 2000.00m,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.婴幼儿照护支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 1500,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            var houseRentDeductionList = dedcutions.Where(f => f.DeductionType == Model.Enums.DeductionEnumType.住房租金支出).ToList();

            if (houseRentCityRes.Code == 200 && houseRentCityRes.Data != null)
            {
                foreach (var item in houseRentDeductionList)
                {
                    if (houseRentCityRes.Data.Any(f => f.Name == item.WorkCity))
                    {
                        item.BaseAmount = Convert.ToDecimal(houseRentCityRes.Data.First(f => f.Name == item.WorkCity).NO);
                    }
                    else
                    {
                        throw new Exception($"{item.WorkCity}专项扣除住房租金支出未配置");

                    }
                }
            }
            else
            {


                throw new Exception($"专项扣除住房租金支出数据获取失败");
            }
            if (dedcutions.Any())
            {
                dedcutions.AsParallel().ForAll(dedcution =>
                {
                    dedcution.MonthAmount = dedcution.BaseAmount * Convert.ToDecimal(dedcution.DeductionPercentage.TrimEnd('%')) / 100;
                });
            }
            await _salaryCustomerService.AddSpecialDeduction(dedcutions);
            return Ok(dedcutions);


        }

        [HttpPost]
        public IActionResult DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<SalaryRecordExcelTemplate>(stream, "工资结算单");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资结算单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        /// <summary>
        /// 公司已过账
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetBillPosted([FromForm] Guid id)
        {
            var model = await _salaryCustomerService.Get(id);
            model.IsCompanyPosted = true;

            await _salaryCustomerService.Update(model);
            return Ok();
        }
        /// <summary>
        /// 客户已打款
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetCustomerPaid([FromForm] Guid id)
        {
            var model = await _salaryCustomerService.Get(id);
            if (model.IsCustomerPaid)
            {
                throw new ErrorCodeException(-1, "此条记录已经被标记过客户已打款，无法重复操作！");
            }
            var isb = await _salaryCustomerService.SetCustomerPaid(id);
            return Ok();
        }
        #region 状态更新


        /// <summary>
        /// 推送到财务
        /// 待审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateStatusPendingReview([FromForm] Guid id)
        {

            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != SalaryStatusEnumType.PendingPush)
            {
                throw new ErrorCodeException(-1, message: "此状态无法执行此操作");
            }

         var config=await    _salaryConfigService.Get(f => f.IsEnabled&&f.CustomerGID==model.CustomerGID);

            if (config!=null&&config.IsNeedSalarySlip)
            {

              var slipModel=await  _salarySlipService.Get(f => f.SalaryCustomerID == model.ID);
                if (slipModel==null)
                {
                    throw new ErrorCodeException(-1, message: "请上传工资条后再执行此操作");
                }
            }
            // 非现金发放
            // 非暂缓发放
            //foreach (var item in model.Records)
            //{
            //    if (item.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && item.IsCashSend == false && item.IsDelayPayment == false)
            //    {
            //        item.IsBatchSend = true;
            //        item.IsSinglePushSuc = false;
            //        item.IsSingleSend = false;
            //        item.Status = SalaryRecordSendStatusEnumType.发放中;
            //    }

            //}
            //model.Status = SalaryStatusEnumType.PendingReview;
            //var invoiceList = await _invoiceService.List(f => f.SalaryCustomerID!=null&& f.SalaryCustomerID.Value == id);
            //DbScoped.SugarScope.BeginTran();
            //if (invoiceList != null && invoiceList.Any())
            //{
            //    foreach (var item in invoiceList)
            //    {
            //        item.Status = InvoiceStatusEnumType.待审核;
            //        item.LockedTime = DateTime.Now;
            //        await _invoiceService.Update(item);
            //    }
            //    if (invoiceList.Any(f => f.BusinessType == PrintTemplateEnumType.外部发票))
            //    {
            //        model.Status = SalaryStatusEnumType.PendingReview;
            //    }
            //    else
            //    {
            //        model.Status = SalaryStatusEnumType.PendingSend;
            //    }

            //}
            //else {
            //    model.Status = SalaryStatusEnumType.PendingSend;
            //}
            //await _salaryCustomerService.UpdateTransNav(model);
            //DbScoped.SugarScope.CommitTran();


         await   _salaryCustomerService.SetPendingReview(model.ID);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStatusPendingPush([FromForm] Guid id)
        {

            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            if (model.Status != SalaryStatusEnumType.ReviewRejected )
            {
                throw new ErrorCodeException(-1, message: "此状态无法执行此操作");
            }
            await _salaryCustomerService.SetPendingPush(model.ID);
            return Ok();
        }



        /// <summary>
        /// 审核退回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> UpdateStatusReviewRejected([FromForm] Guid id)
        {
            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != SalaryStatusEnumType.PendingSend && model.Status != SalaryStatusEnumType.PendingReview)
            {
                throw new ErrorCodeException(-1, message: "此状态无法执行此操作");
            }
            await _salaryCustomerService.SetReviewRejected(model.ID);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateStatusPendingSend([FromForm] Guid id)
        {

            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != SalaryStatusEnumType.PendingReview )
            {
                throw new ErrorCodeException(-1, message: "此状态无法执行此操作");
            }
            await _salaryCustomerService.SetPendingSend(model.ID);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> SetLocked([FromForm] Guid id, [FromForm] bool isLocked)
        {
            var model = await _salaryCustomerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != SalaryStatusEnumType.PendingSend)
            {
                throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");
            }
            await _salaryCustomerService.SetLocked(model.ID, isLocked);
            return Ok();
        }






        /// <summary>
        /// 工资发放成功
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> UpdateStatusSuccessSend([FromForm] Guid id)
        {

            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != SalaryStatusEnumType.PendingSend&& model.Status != SalaryStatusEnumType.FailSend)
            {
                throw new ErrorCodeException(-1, message: "此状态无法执行此操作");
            }

            await _salaryCustomerService.BatchSendSuccess(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStatusFailSend([FromForm] Guid id)
        {
            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != SalaryStatusEnumType.PendingSend)
            {
                throw new ErrorCodeException(-1, message: "此状态无法执行此操作");
            }
            await _salaryCustomerService.BatchSendFail(model);
            return Ok();
        }

        #endregion
        [HttpPut]
        public async Task<IActionResult> SalaryTransBatch([FromForm] Guid id, [FromForm] string pwd)
        {
            //model.IsPushTrans = true;
            var context = HttpContext;
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"{id}_count";
            var isb = await _redisService.SetAsync(cacheKey, "11", TimeSpan.FromSeconds(5), CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, "请求过快,请5秒后再操作");
            }

            var actionPwd = await _actionPasswordService.Get(f => f.UserID == CurrentUser.UserID);
            if (actionPwd == null)
            {
                throw new ErrorCodeException(-1, "未设置操作密码");
            }

            var realPwd = AesGcmHelper.Decrypt(actionPwd.Pwd, Convert.FromBase64String(_aesOption.Key));
            if (pwd != realPwd)
            {
                throw new ErrorCodeException(-1, message: "操作密码验证失败");
            }

            var model = await _salaryCustomerService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != SalaryStatusEnumType.PendingSend)
            {
                throw new ErrorCodeException(-1, message: "非待发放状态无法执行此操作！");
            }
            if (model.IsCmbcSend)
            {
                throw new ErrorCodeException(-1, message: "已民生代发过的工资记录，无法重复操作，请联系管理员");
            }
            if (!model.IsLocked)
            {
              await  _salaryCustomerService.SetLocked(model.ID,true);
            }

            // 重新获取最新数据
            model = await _salaryCustomerService.GetNav(f => f.ID == id);


            var businessExist = await _transBatchService.List(f => f.BusinessID == id);
            if (businessExist != null && businessExist.Count > 0)
            {
                throw new ErrorCodeException(-1, "此条工资记录已推送过，无法重复操作");
            }


            if (!model.IsCustomerPaid)
            {
                throw new ErrorCodeException(-1, "客户未打款，无法执行此功能！");
            }

            // 暂缓发放由民生代发
            if (model.Records.Any(f => f.IsDelayPayment == true && f.ActualSalarySendCardAmount > 0))
            {
                var delayRecords = model.Records.Where(f => f.IsDelayPayment == true && f.ActualSalarySendCardAmount > 0).ToList();
                foreach (var item in delayRecords)
                {
                    item.IsSingleSend = true;
                }
            }
            // 批量发放且 非单笔发放
            var salaryRecords = model.Records.Where(f => f.IsBatchSend&&f.IsSingleSend==false && f.ActualSalarySendCardAmount > 0).ToList();
            if (salaryRecords == null || salaryRecords.Count <= 0)
            {
                throw new ErrorCodeException(-1, "没有需要发放的记录，操作失败");
            }


            foreach (var record in salaryRecords)
            {
                // 批量代发
                record.IsBatchPushSuc = true;
                record.BatchTransStatus = string.Empty;
                record.BatchTransStatusName = string.Empty;
            }

            var transBatch = new Live_TransBatchExtension()
            {
                ID = Guid.NewGuid(),
                ConfirmTimes = 0,
                CreateTime = DateTime.Now,
                CustomerName = model.CustomerName,
                Remark = string.Empty,
                SigningUnitName = model.SigningUnitCompanyName,
                Status = CmbcStatusEnumType.待确认,
                TotalAmount = salaryRecords.Sum(f => f.ActualSalarySendCardAmount),
                TotalNum = salaryRecords.Count,
                TransNonce = model.ID,
                BusinessID = model.ID,
                TransStatus = string.Empty,
                TransStatusName = string.Empty,
                TransTime = null,
                Records = salaryRecords.Select(f => new Live_TransBatchRecord()
                {
                    ID = Guid.NewGuid(),
                    Bank = f.Bank,
                    BankNo = f.BankNo,
                    BankUserName = f.BankUserName,
                    BusinessID = f.ID,
                    IdCardNo = f.IdCardNo,
                    Name = f.Name,
                    TransAmount = f.ActualSalarySendCardAmount,
                    TranStatus = string.Empty,
                    TranStatusName = string.Empty,
                }).ToList()
            };


            if (transBatch.Records.Any(f=>string.IsNullOrWhiteSpace(f.Bank) || string.IsNullOrWhiteSpace(f.BankNo) || string.IsNullOrWhiteSpace(f.BankUserName)))
            {
                throw new ErrorCodeException(-1, $"部分员工银行卡信息缺失，请人工核对！");
            }
          


            try
            {
                var resp = await _financeOtherService.BatchTransaction(new BatchTransactionRequest()
                {
                    BusinessID = transBatch.BusinessID,
                    CustomerName = transBatch.CustomerName,
                    Postscript = string.Empty,
                    SigningUnitName = transBatch.SigningUnitName,
                    Records = transBatch.Records.Select(f => new BatchTransactionRecord()
                    {
                        Bank = f.Bank,
                        BankNo = f.BankNo,
                        BankUserName = f.BankUserName,
                        BusinessID = f.ID,
                        IdCardNo = f.IdCardNo,
                        Name = f.Name,
                        TransAmt = f.TransAmount,
                    }).ToList()
                });
                if (resp == null)
                {
                    throw new ErrorCodeException(-1, $"幸福职代发数据同步接口调用异常，请人工核对！");
                }
                else
                {
                    if (resp.Result)
                    {
                        transBatch.Status = CmbcStatusEnumType.待确认;
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, $"幸福职代发数据同步失败，响应信息：{resp.Message}！");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, $"幸福职接口调用异常，{ex.Message}");
            }
            DbScoped.SugarScope.BeginTran();
            await _transBatchService.AddNav(transBatch);
            model.IsCmbcSend = true;
            model.IsPushTrans = true;
            model.IsPushToOrder = true;
            model.PushTransTime = DateTime.Now;

            await _salaryCustomerService.UpdateNav(model);
            DbScoped.SugarScope.CommitTran();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> SetPlatformPrepaid([FromForm] Guid id)
        {
            var model = await _salaryCustomerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            model.IsPlatformPrepaid = true;
            await _salaryCustomerService.Update(model);
            return Ok();
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExportSalaryCustomerExcel(string signingUnitName, string customerName, DateTime? salaryMonth, DateTime? taxMonth, SalaryStatusEnumType? status, bool? isFinance, DateTime? sendTime, [FromQuery] PagedModelBase query)
        {
          
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_SalaryCustomer>();
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin && user.CompanyType != CompanyType.Party.ToString().ToLower())
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(it => it.PartnerID == info.PartnerID);
                }
            }
            if (user.CompanyType == CompanyType.Party.ToString().ToLower())//人力公司授权甲方公司
            {
                exp.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {

                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }
            exp.AndIF(query.CID.HasValue, it => it.CustomerID == query.CID);

            exp.AndIF(isFinance.HasValue && isFinance.Value, f => f.Status >= SalaryStatusEnumType.PendingReview && f.Status != SalaryStatusEnumType.Cancelled);
            exp.AndIF(!signingUnitName.IsNullOrWhiteSpace(), f => f.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!customerName.IsNullOrWhiteSpace(), f => f.CustomerName.Contains(customerName));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);
            exp.AndIF(taxMonth.HasValue, f => f.TaxMonth == taxMonth.Value);
            exp.AndIF(sendTime.HasValue, f => f.SendTime!=null&&  f.SendTime.Value.Date == sendTime.Value.Date);
            var models = await _salaryCustomerService.PageList(exp.ToExpression(),new PagedModelBase() { Page=0});
            List<SalaryCustomerExcelTemplate> excelData = new List<SalaryCustomerExcelTemplate>();
            SalaryCustomerExcelTemplate excelItem = null;
            foreach (var item in models)
            {
                excelItem = new SalaryCustomerExcelTemplate()
                {
                    ActualSalarySumAmount = item.ActualSalarySumAmount,
                    CreateTime = item.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    CustomerName = item.CustomerName,
                    IsCanCmbcSend = item.IsCanCmbcSend ? "是" : "否",
                    IsCmbcSend = item.IsCmbcSend ? "是" : "否",
                    IsCmbcSendFail = item.IsCmbcSendFail ? "民生代发失败" : "",
                    IsLocked = item.IsLocked ? "已锁定" : "未锁定",
                    SalaryBatchType = $"批次{item.SalaryBatchType}",
                    SalaryMonth = item.SalaryMonth?.ToString("yyyy-MM-dd"),
                    TaxMonth = item.TaxMonth?.ToString("yyyy-MM-dd"),
                    SendTime = item.SendTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    SalaryType = item.SalaryType.ToString(),
                    SalaryYear = item.SalaryYear,
                    UnitSettlementAmount = item.UnitSettlementAmount
                };

                switch (item.SalaryType)
                {
                    case 0:
                        excelItem.SalaryType = "常规工资";
                        break;
                    case 1:
                        excelItem.SalaryType = "年终奖工资";
                        break;
                    case 2:
                        excelItem.SalaryType = "小时工工资";
                        break;
                    default:
                        break;
                }
                switch (item.Status)
                {
                    case SalaryStatusEnumType.Pending:
                        excelItem.Status = "待处理";
                        break;
                    case SalaryStatusEnumType.AwaitingCalculation:
                        excelItem.Status = "待计算";
                        break;
                    case SalaryStatusEnumType.PendingPush:
                        excelItem.Status = "待推送";
                        break;
                    case SalaryStatusEnumType.PendingReview:
                        excelItem.Status = "待审核";
                        break;
                    case SalaryStatusEnumType.ReviewRejected:
                        excelItem.Status = "审核退回";
                        break;
                    case SalaryStatusEnumType.PendingSend:
                        excelItem.Status = "待发放";
                        break;
                    case SalaryStatusEnumType.SucessSend:
                        excelItem.Status = "已发放";
                        break;
                    case SalaryStatusEnumType.FailSend:
                        excelItem.Status = "发放失败";
                        break;
                    case SalaryStatusEnumType.Cancelled:
                        excelItem.Status = "作废";
                        break;
                    default:
                        break;
                }

                excelData.Add(excelItem);

            }
;
           var fileBytes = NpoiExcelHelper.WriteExcel(excelData, "工资记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "客户工资.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

            //return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "11.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportTaxApplyExcel(string signingUnitName, string customerName, DateTime? salaryMonth, DateTime? taxMonth, SalaryStatusEnumType? status, bool? isFinance, DateTime? sendTime, [FromQuery] PagedModelBase query)
        {

            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_SalaryCustomer>();
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin && user.CompanyType != CompanyType.Party.ToString().ToLower())
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(it => it.PartnerID == info.PartnerID);
                }
            }
            if (user.CompanyType == CompanyType.Party.ToString().ToLower())//人力公司授权甲方公司
            {
                exp.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {

                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }

            exp.And(f=>!f.CustomerName.Contains("顺丰"));
            exp.AndIF(query.CID.HasValue, it => it.CustomerID == query.CID);

            exp.AndIF(isFinance.HasValue && isFinance.Value, f => f.Status >= SalaryStatusEnumType.PendingReview && f.Status != SalaryStatusEnumType.Cancelled);
            exp.AndIF(!signingUnitName.IsNullOrWhiteSpace(), f => f.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!customerName.IsNullOrWhiteSpace(), f => f.CustomerName.Contains(customerName));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);
            exp.AndIF(taxMonth.HasValue, f => f.TaxMonth == taxMonth.Value);
            exp.AndIF(sendTime.HasValue, f => f.SendTime != null && f.SendTime.Value.Date == sendTime.Value.Date);
            var models = await _salaryCustomerService.PageList(exp.ToExpression(), new PagedModelBase() { Page = 0 });

            var ids = models.Select(f => f.ID).ToList();

            var recordList = await _salaryRecordService.ListView(f => ids.Contains(f.SalaryCustomerID)&&f.IdCardNo.Length==18);



            var excelData = recordList.Select(f => new TaxApplySalaryRecordExcelTemplate()
            {
                CardID = f.IdCardNo,
                TaxMonth = f.TaxMonth.Value.ToString("yyyy-MM"),
                Name = f.Name,
                Phone = f.Phone,
                Sex = "",
                CustomerName = f.CustomerName,
                SignUnitName = f.SigningUnitCompanyName,
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                StatusName = f.Status.ToString(),
                UnitSalaryAmount = f.UnitSalaryAmount,
                YangLaoAmount = f.SocialYangLaoAmount,
                YiLiaoAmount = f.SocialYiLiaoAmount,
                ShiYeAmount = f.SocialShiYeAmount,
                DaBingAmount = f.SocialDaBingAmount,
                GongJiJinAmount = f.SocialGongJiJinAmount,
                NoTaxSalaryAmount = 0,
                SalaryBatchType = $"批次{f.SalaryBatchType}",
                BeforeTaxSalaryTotalAmount = f.BeforeTaxSalaryTotalAmount,
                SpBasicTotalAmount = f.SpBasicTotalAmount,
                TaxSpZiNvTotalAmount = f.TaxSpZiNvTotalAmount,
                TaxSpShanYangTotalAmount = f.TaxSpShanYangTotalAmount,
                TaxSpZuJinTotalAmount = f.TaxSpZuJinTotalAmount,
                TaxSpDaiKuanTotalAmount = f.TaxSpDaiKuanTotalAmount,
                TaxSpJiaoYuTotalAmount = f.TaxSpJiaoYuTotalAmount,
                TaxSpYingErTotalAmount = f.TaxSpYingErTotalAmount,
                TaxSalaryTotalAmount = f.TaxSalaryTotalAmount > 0 ? f.TaxSalaryTotalAmount : 0,
                TaxTotalAmount =  f.TaxTotalAmount,
                LastTaxTotalAmount =  f.LastTaxTotalAmount,
                MonthTaxAmount=f.TaxMonthAmount,
                SendDate=f.SendTime?.ToString("yyyy-MM-dd")

            }).ToList();

            foreach (var item in excelData)
            {
                Console.WriteLine(item.CardID);
                item.Sex = IdCardHelper.GetGender(item.CardID);
            }
            var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.MinShengTempUri);

            var fileByteArray = NpoiExcelHelper.WriteExcel(excelData, "Sheet1");

            string base64String = Convert.ToBase64String(fileByteArray);


            //return File(fileByteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "个税申报表.xlsx");
            return Ok(new { base64String, fileName = "个税申报表.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }





        [HttpPost]
        public async Task<IActionResult> ExportSalaryRecordExcel([FromForm] Guid? id, [FromForm] bool isFinance, [FromForm] string signingUnitName, [FromForm] string customerName, [FromForm] DateTime? salaryMonth, [FromForm] DateTime? taxMonth, [FromForm] SalaryStatusEnumType? status)
        {
            var exp = Expressionable.Create<Live_V_SalaryRecord>();
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }
            var user = CurrentUser;

             if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {

                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }


            exp.AndIF(isFinance, f => f.IsBatchSend == true&& f.IsSingleSend == false && f.ActualSalarySendCardAmount > 0);
            exp.AndIF(id.HasValue,f => f.SalaryCustomerID == id);
            exp.AndIF(!signingUnitName.IsNullOrWhiteSpace(), f => f.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!customerName.IsNullOrWhiteSpace(), f => f.CustomerName.Contains(customerName));
            exp.AndIF(status.HasValue, f => f.SalaryStatus == status.Value);
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);
            exp.AndIF(taxMonth.HasValue, f => f.TaxMonth == taxMonth.Value);


            var records = await _salaryCustomerService.QuerySalaryRecordViewList(exp.ToExpression());

            SalaryRecordExportExcelTemplate total = new SalaryRecordExportExcelTemplate();
            total.CustomerName = "";
            total.Bank = "";
            total.BankNo = "";
            total.BankUserName = "";
            total.CooperationUnitCompanyName = "";
            total.EntryChannel = "";
            total.EntryDate = "";
            total.IdCardNo = "";
            total.LeaveDate = "";
            total.Name = "";
            total.Phone = "";
            total.Remark = "";
            total.SigningUnitCompanyName = "";
            total.SalaryBatchType = "合计金额";
            total.SalaryMonth = "";
            foreach (var x in records)
            {
                total.CompanyTotalAmount += x.CompanyTotalAmount;
                total.BeforeTaxSalaryAmount += x.BeforeTaxSalaryAmount;
                total.BeforeTaxSalaryTotalAmount += x.BeforeTaxSalaryTotalAmount;
                total.UnitDeductionAmount += x.UnitDeductionAmount;
                total.UnitSalaryAmount += x.UnitSalaryAmount;
                total.CompanySalarySumAmount += x.CompanySalarySumAmount;
                total.DeductionNoTaxAmount += x.DeductionNoTaxAmount;
                total.DelaySendAmount += x.ActualSalarySendCardAmount;
                total.SocialGongJiJinAmount += x.SocialGongJiJinAmount;
                total.UnitManageAmount += x.UnitManageAmount;
                total.ActualSalarySendCardAmount += x.ActualSalarySendCardAmount;
                total.PlatformWelfareAmount += x.PlatformWelfareAmount;
                total.PlatformCommissionAmount += x.PlatformCommissionAmount;
                total.PrepaidRepaymentAmount += x.PrepaidRepaymentAmount;
                total.SocialUnitSumAmount += x.SocialUnitSumAmount;
                total.SocialDaBingAmount += x.SocialDaBingAmount;
                total.SocialShiYeAmount += x.SocialShiYeAmount;
                total.SocialYangLaoAmount += x.SocialYangLaoAmount;
                total.SocialYiLiaoAmount += x.SocialYiLiaoAmount;
                total.SpDaiKuanAmount += x.SpDaiKuanAmount;
                total.SpJiaoYuAmount += x.SpJiaoYuAmount;
                total.SpShanYangAmount += x.SpShanYangAmount;
                total.SpYingErAmount += x.SpYingErAmount;
                total.SpZiNvAmount += x.SpZiNvAmount;
                total.SpZuJinAmount += x.SpZuJinAmount;
                total.SubsidyAmount += x.SubsidyAmount;
                total.SubsidyNoTaxAmount += x.SubsidyNoTaxAmount;
                total.TaxMonthAmount += x.TaxMonthAmount;
                total.TaxTotalAmount += x.TaxTotalAmount;
                total.TaxSalaryTotalAmount += x.TaxSalaryTotalAmount;
                total.TaxSocialTotalAmount += x.TaxSocialTotalAmount;
                total.TaxSpBasicTotalAmount += x.SpBasicTotalAmount;
                total.TaxSpTotalAmount += x.TaxSpTotalAmount;
                total.SocialEmpSumAmount += x.SocialEmpSumAmount;
                total.TaxTimes += x.TaxTimes;
                total.LastTaxTotalAmount+= x.LastTaxTotalAmount;

            }

            var exportDataList = records.Select(f => new SalaryRecordExportExcelTemplate()
            {
                CompanyTotalAmount = f.CompanyTotalAmount,
                Bank = f.Bank,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                BeforeTaxSalaryAmount = f.BeforeTaxSalaryAmount,
                BeforeTaxSalaryTotalAmount = f.BeforeTaxSalaryTotalAmount,
                UnitDeductionAmount = f.UnitDeductionAmount,
                UnitSalaryAmount = f.UnitSalaryAmount,
                CompanySalarySumAmount = f.CompanySalarySumAmount,
                CooperationUnitCompanyName = f.CooperationUnitCompanyName,
                CustomerName = f.CustomerName,
                DeductionNoTaxAmount = f.DeductionNoTaxAmount,
                DelaySendAmount = f.ActualSalarySendCardAmount,
                EntryChannel = string.Empty,
                EntryDate = f.EntryDate.HasValue?f.EntryDate.Value.ToString("yyyy-MM-dd"):"",
                SocialGongJiJinAmount = f.SocialGongJiJinAmount,
                IdCardNo = f.IdCardNo,
                UnitManageAmount = f.UnitManageAmount,
                IsCashSend = f.IsCashSend?"是":"否",
                IsDelayPayment = f.IsDelayPayment?"是":"否",
                LeaveDate = f.LeaveDate.HasValue?f.LeaveDate.Value.ToString("yyyy-MM-dd"):"",
                ActualSalarySendCardAmount = f.ActualSalarySendCardAmount,
                Name = f.Name,
                Phone = f.Phone,
                 PlatformWelfareAmount=f.PlatformWelfareAmount,
                PlatformCommissionAmount = f.PlatformCommissionAmount,
                PrepaidRepaymentAmount = f.PrepaidRepaymentAmount,
                Remark = f.Remark,
                SalaryBatchType = $"第{f.SalaryBatchType.Value}批次",
                SalaryMonth = f.SalaryMonth.HasValue?f.SalaryMonth.Value.ToString("yyyy-MM-dd") : "",
                SigningUnitCompanyName = f.SigningUnitCompanyName,
                SocialUnitSumAmount = f.SocialUnitSumAmount,
                SocialDaBingAmount = f.SocialDaBingAmount,
                SocialShiYeAmount = f.SocialShiYeAmount,
                SocialYangLaoAmount = f.SocialYangLaoAmount,
                SocialYiLiaoAmount = f.SocialYiLiaoAmount,
                SpDaiKuanAmount = f.SpDaiKuanAmount,
                SpJiaoYuAmount = f.SpJiaoYuAmount,
                SpShanYangAmount = f.SpShanYangAmount,
                SpYingErAmount = f.SpYingErAmount,
                SpZiNvAmount = f.SpZiNvAmount,
                SpZuJinAmount = f.SpZuJinAmount,
                SubsidyAmount = f.SubsidyAmount,
                SubsidyNoTaxAmount = f.SubsidyNoTaxAmount,
                TaxMonthAmount = f.TaxMonthAmount,
                TaxTotalAmount = f.TaxTotalAmount,
                TaxSalaryTotalAmount = f.TaxSalaryTotalAmount,
                TaxSocialTotalAmount = f.TaxSocialTotalAmount,
                TaxSpBasicTotalAmount = f.SpBasicTotalAmount,
                TaxSpTotalAmount = f.TaxSpTotalAmount,
                SocialEmpSumAmount = f.SocialEmpSumAmount,
                TaxTimes = f.TaxTimes,
                ChangeAmount = f.ChangeAmount ?? 0,
                ChangeRemark = f.ChangeRemark,
                LastTaxTotalAmount = f.LastTaxTotalAmount
            }).ToList();
            exportDataList.Add(total);
            byte[] fileBytes;
            fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "工资记录");
            //if(id.HasValue)
            //{
            //    fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "工资记录");
            //}
            //else
            //{
            //    fileBytes = _salaryCustomerService.Export(exportDataList);
            //}
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资结算单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

        }





        [HttpPost]
        public async Task<IActionResult> SalaryRecorePrintList([FromForm] Guid? id, [FromForm] bool isFinance, [FromForm] string signingUnitName, [FromForm] string customerName, [FromForm] DateTime? salaryMonth, [FromForm] DateTime? taxMonth, [FromForm] SalaryStatusEnumType? status)
        {
            var exp = Expressionable.Create<Live_V_SalaryRecord>();
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }
            var user = CurrentUser;

            if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {

                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }


            exp.AndIF(isFinance, f => f.IsBatchSend == true && f.IsSingleSend == false && f.ActualSalarySendCardAmount > 0);
            exp.AndIF(id.HasValue, f => f.SalaryCustomerID == id);
            exp.AndIF(!signingUnitName.IsNullOrWhiteSpace(), f => f.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!customerName.IsNullOrWhiteSpace(), f => f.CustomerName.Contains(customerName));
            exp.AndIF(status.HasValue, f => f.SalaryStatus == status.Value);
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);
            exp.AndIF(taxMonth.HasValue, f => f.TaxMonth == taxMonth.Value);


            var records = await _salaryCustomerService.QuerySalaryRecordViewList(exp.ToExpression());

            //SalaryRecordExportExcelTemplate total = new SalaryRecordExportExcelTemplate();
            //total.CustomerName = "";
            //total.Bank = "";
            //total.BankNo = "";
            //total.BankUserName = "";
            //total.CooperationUnitCompanyName = "";
            //total.EntryChannel = "";
            //total.EntryDate = "";
            //total.IdCardNo = "";
            //total.LeaveDate = "";
            //total.Name = "";
            //total.Phone = "";
            //total.Remark = "";
            //total.SigningUnitCompanyName = "";
            //total.SalaryBatchType = "合计金额";
            //total.SalaryMonth = "";
            //foreach (var x in records)
            //{
            //    total.CompanyTotalAmount += x.CompanyTotalAmount;
            //    total.BeforeTaxSalaryAmount += x.BeforeTaxSalaryAmount;
            //    total.BeforeTaxSalaryTotalAmount += x.BeforeTaxSalaryTotalAmount;
            //    total.UnitDeductionAmount += x.UnitDeductionAmount;
            //    total.UnitSalaryAmount += x.UnitSalaryAmount;
            //    total.CompanySalarySumAmount += x.CompanySalarySumAmount;
            //    total.DeductionNoTaxAmount += x.DeductionNoTaxAmount;
            //    total.DelaySendAmount += x.ActualSalarySendCardAmount;
            //    total.SocialGongJiJinAmount += x.SocialGongJiJinAmount;
            //    total.UnitManageAmount += x.UnitManageAmount;
            //    total.ActualSalarySendCardAmount += x.ActualSalarySendCardAmount;
            //    total.PlatformWelfareAmount += x.PlatformWelfareAmount;
            //    total.PlatformCommissionAmount += x.PlatformCommissionAmount;
            //    total.PrepaidRepaymentAmount += x.PrepaidRepaymentAmount;
            //    total.SocialUnitSumAmount += x.SocialUnitSumAmount;
            //    total.SocialDaBingAmount += x.SocialDaBingAmount;
            //    total.SocialShiYeAmount += x.SocialShiYeAmount;
            //    total.SocialYangLaoAmount += x.SocialYangLaoAmount;
            //    total.SocialYiLiaoAmount += x.SocialYiLiaoAmount;
            //    total.SpDaiKuanAmount += x.SpDaiKuanAmount;
            //    total.SpJiaoYuAmount += x.SpJiaoYuAmount;
            //    total.SpShanYangAmount += x.SpShanYangAmount;
            //    total.SpYingErAmount += x.SpYingErAmount;
            //    total.SpZiNvAmount += x.SpZiNvAmount;
            //    total.SpZuJinAmount += x.SpZuJinAmount;
            //    total.SubsidyAmount += x.SubsidyAmount;
            //    total.SubsidyNoTaxAmount += x.SubsidyNoTaxAmount;
            //    total.TaxMonthAmount += x.TaxMonthAmount;
            //    total.TaxTotalAmount += x.TaxTotalAmount;
            //    total.TaxSalaryTotalAmount += x.TaxSalaryTotalAmount;
            //    total.TaxSocialTotalAmount += x.TaxSocialTotalAmount;
            //    total.TaxSpBasicTotalAmount += x.SpBasicTotalAmount;
            //    total.TaxSpTotalAmount += x.TaxSpTotalAmount;
            //    total.SocialEmpSumAmount += x.SocialEmpSumAmount;
            //    total.TaxTimes += x.TaxTimes;
            //    total.LastTaxTotalAmount += x.LastTaxTotalAmount;

            //}

            var exportDataList = records.Select(f => new SalaryRecordPrintExcelTemplate()
            {
                EntryDate = f.EntryDate?.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                Name = f.Name,
                IdCardNo = f.IdCardNo,
                Phone = f.Phone,
                UnitSalaryAmount = f.UnitSalaryAmount,
                ActualSalarySendCardAmount = f.ActualSalarySendCardAmount,
                PrepaidRepaymentAmount = f.PrepaidRepaymentAmount,
                Bank = f.Bank,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                ChangeAmount = f.ChangeAmount ?? 0,
                ChangeRemark = f.ChangeRemark ?? "",
                CustomerName = f.CustomerName,
                SignUnitName = f.SigningUnitCompanyName,
                DeductionNoTaxAmount = f.DeductionNoTaxAmount,
                SalaryMonth = f.SalaryMonth?.ToString("yyyy-MM-dd"),
                SocialDaBingAmount = f.SocialDaBingAmount,
                SocialGongJiJinAmount = f.SocialGongJiJinAmount,
                SocialShiYeAmount = f.SocialShiYeAmount,
                SocialYangLaoAmount = f.SocialYangLaoAmount,
                SocialYiLiaoAmount = f.SocialYiLiaoAmount,
                SubsidyAmount = f.SubsidyAmount,
                SubsidyNoTaxAmount = f.SubsidyNoTaxAmount,
                TaxMonthAmount = f.TaxMonthAmount,
            }).ToList();
            //exportDataList.Add(total);
            byte[] fileBytes;
            fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "工资记录");
            //if(id.HasValue)
            //{
            //    fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "工资记录");
            //}
            //else
            //{
            //    fileBytes = _salaryCustomerService.Export(exportDataList);
            //}
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(exportDataList);

        }





        #region 工资条

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadSalarySlip(IFormFile file)
        {
            Guid id = Guid.Empty;
             //id = Guid.Parse("9f0ed1da-e5aa-4a26-8583-d120ae73f691");
            if (HttpContext.Request.Form.Any(f => f.Key == "id"))
            {
                id = Guid.Parse(HttpContext.Request.Form["id"]);
            }

            Live_SalaryCustomerExtension model = null;
            if (id != Guid.Empty)
            {
                model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            }
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var kvList = NpoiExcelHelper.ReadExcelKV(ms);
                if (kvList == null || kvList.Count <= 0)
                {
                    //读取失败
                }

                // 或表头
                var header = kvList.First();

                int customerIndex = 0;
                int salaryMonthIndex = 0;
                int nameIndex = 0;
                int idCardNoIndex = 0;
                if (header.Any(f => f.Value == "客户单位"))
                {
                    customerIndex = header.First(f => f.Value == "客户单位").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【客户单位】");
                }
                if (header.Any(f => f.Value == "工资所属月"))
                {
                    salaryMonthIndex = header.First(f => f.Value == "工资所属月").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【工资所属月】");
                }
                if (header.Any(f => f.Value == "姓名"))
                {
                    nameIndex = header.First(f => f.Value == "姓名").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【姓名】");
                }
                if (header.Any(f => f.Value == "身份证号码"))
                {
                    idCardNoIndex = header.First(f => f.Value == "身份证号码").Key;
                }
                else
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【身份证号码】");
                }


                if (!header.Any(f => f.Value == "社保补贴"))
                {
                    throw new ErrorCodeException(-1, "表头信息缺少【社保补贴】");
                }
               

               
                var headerJson = Newtonsoft.Json.JsonConvert.SerializeObject(header);
                var headerBytes = Encoding.UTF8.GetBytes(headerJson);
                var headerBase64 = Convert.ToBase64String(headerBytes);

                // 获取客户单位
                var content = kvList.Last();
                kvList.RemoveAt(0);

                List<Live_SalarySlipRecord> records = new List<Live_SalarySlipRecord>();

                Live_SalarySlipRecord slipRecord;

                foreach (var kv in kvList)
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(kv);
                    var jsonBytes = Encoding.UTF8.GetBytes(json);
                    slipRecord = new Live_SalarySlipRecord()
                    {
                        ID = Guid.NewGuid(),
                        SalaryRecordID = null,
                        CustomerName = kv[customerIndex],
                        SalaryMonth = Convert.ToDateTime(kv[salaryMonthIndex]),
                        IdCardNo = kv[idCardNoIndex],
                        Name = kv[nameIndex],
                        ContentKV = Convert.ToBase64String(jsonBytes),
                        IsNotice = false,
                    };
                    records.Add(slipRecord);
                }

                var groups = records.GroupBy(f => new { f.CustomerName, f.SalaryMonth }).ToList();
                if (groups.Count > 1)
                {
                    throw new ErrorCodeException(-1, "不支持多客户单位或多工资所属月导入");
                }

                var group = groups.First();

                Live_SalarySlipExtension salarySlip = new Live_SalarySlipExtension()
                {
                    CreateTime = DateTime.Now,
                    CustomerName = group.Key.CustomerName,
                    IsNotice = false,
                    SalaryMonth = group.Key.SalaryMonth,
                    TotalNum = kvList.Count - 1,
                    HeaderKV = headerBase64,
                    CustomerGID = model.CustomerGID,
                    Records = records,
                    SalaryCustomerID = model.ID,
                    ID = Guid.NewGuid()
                };
                var salaryRecords = model.Records.Where(f => f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资).ToList();


                List<string> nameList = new List<string>();
                foreach (var salaryRecord in salaryRecords)
                {
                    if (records.Any(f => f.Name == salaryRecord.Name && f.IdCardNo == salaryRecord.IdCardNo))
                    {
                        salaryRecord.SalarySlipRecordID = records.First(f => f.IdCardNo == salaryRecord.IdCardNo).ID;
                    }
                    else
                    {

                        nameList.Add(salaryRecord.Name);
                    }
                }

                //if (nameList.Any())
                //{
                //    throw new ErrorCodeException(-1, $"工资条信息缺失，人员名单：{string.Join('、', nameList)}");
                //}
                DbScoped.SugarScope.BeginTran();
                await _salarySlipService.AddNav(salarySlip);
                model.SalarySlipID = salarySlip.ID;
                await _salaryCustomerService.UpdateTransNav(model);
                DbScoped.SugarScope.CommitTran();
                return Ok();

            }
        }
        [HttpPost]
        public async Task<IActionResult> DownloadSalarySlip([FromForm] Guid id)
        {

            //var salaryCustomer= await _salaryCustomerService.Get(id);

            //if (salaryCustomer.SalarySlipID==null|| salaryCustomer.SalarySlipID.Value==Guid.Empty)
            //{
            //    throw new ErrorCodeException(-1, "你要读取的数据不存在");
            //}

            //var model = await _salarySlipService.GetNav(salaryCustomer.SalarySlipID.Value);
            //if (model == null)
            //{
            //    throw new ErrorCodeException(-1, "你要读取的数据不存在");
            //}
            //Dictionary<int, string> headerDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(model.HeaderKV)));
            //List<Dictionary<int, string>> dss = new List<Dictionary<int, string>>();
            //dss.Add(headerDic);
            //foreach (var item in model.Records)
            //{
            //    Dictionary<int, string> contentDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(item.ContentKV)));
            //    dss.Add(contentDic);
            //}


            var dss = await _salaryCustomerService.ListSalaryRecordSlipRecord(id);
            var fileBytes = NpoiExcelHelper.WriteExcelKV(dss);
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资条.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ListSalarySlipByUserID(int userID)
        {

            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);

            var minMonth = currentMonth.AddMonths(-12);

            var slips = await _salaryCustomerService.ListUserSalaryRecordSlipRecord(f => f.SalaryMonth>=minMonth&& f.SalaryCustomerStatus == (int)SalaryStatusEnumType.SucessSend && f.UserID.HasValue && f.UserID.Value == userID);

            if (slips==null)
            {
                slips = new List<UserSalarySlip>();
            }

            var thirdSlipRecordList = await _thirdSalarySlipService.ListRecords(f => f.UserID == userID && f.SalaryMonth >= minMonth && f.IsNotice);

          
            if (thirdSlipRecordList!=null)
            {
                var groupList = thirdSlipRecordList.GroupBy(f => f.SalaryMonth).ToList();
                int batchNo = 0;
                foreach (var groupItem in groupList)
                {
                    UserSalarySlip thirdSlip = new UserSalarySlip();
                    batchNo =0;

                    var records = groupItem.OrderByDescending(f => f.CreateTime).ToList();

                    foreach (var item in records)
                    {
                        batchNo++;
                        var dss = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(item.SlipContent);
                        thirdSlip.ID = item.ID;
                        thirdSlip.SalaryMonth = item.SalaryMonth.Value;
                        thirdSlip.SalaryBatchType = batchNo;
                        thirdSlip.CustomerName = item.CustomerName;
                        thirdSlip.Records = dss.Select(f => new UserSalarySlipItem() { Title = f.Key, VauleInfo = f.Value }).ToList();
                        slips.Add(thirdSlip);
                    }

                }

            }




            slips= slips.OrderByDescending(f =>f.SalaryMonth).ToList() ;
            // 排除客户单位信息的展示
            foreach (var item in slips)
            {
                if (item.CustomerName == "劳动关系-无锡毅丰物流有限公司" || item.CustomerName == "劳动关系--晨恒供应链（宁波）有限公司")
                {
                    if (item.Records != null && item.Records.Count > 0)
                    {
                        item.Records = item.Records.Where(f => f.Title != "客户单位").ToList();
                    }
                }
            }






            return Ok(slips);
        }
        [HttpGet]
        [OperationLogAction("查看了{date}的工资记录！")]
        public IActionResult ShowSalarySlip(string date)
        {
            return Ok();
        }
        #endregion
        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> ApplyBudget([FromForm]Guid id)
        {
            try
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();

                if (info==null)
                {
                    throw new ErrorCodeException(-1, "授权信息获取失败");
                }

                DbScoped.SugarScope.BeginTran();
                var salaryCustomer = await _salaryCustomerService.Get(id);
                if (salaryCustomer==null)
                {
                    throw new ErrorCodeException(-1, "你要读取的数据不存在");
                }
                if (!salaryCustomer.CustomerName.EndsWith("（内部员工）"))
                {
                    throw new ErrorCodeException(-1, "客户单位要以“内部员工）”结尾，才能使用此功能");
                }

                if (salaryCustomer.Status != SalaryStatusEnumType.PendingPush)
                {
                    throw new ErrorCodeException(-1, "非待推送状态无法使用此功能");
                }
                if (salaryCustomer.BudgetApplyRecordID.HasValue && salaryCustomer.BudgetApplyRecordID.Value > 0)
                {
                    //throw new ErrorCodeException(-1, "预算记录标识已存在");
                    var applyRecord = await _budgetApplyRecordService.Get(salaryCustomer.BudgetApplyRecordID.Value);
                    if (applyRecord == null || applyRecord.ProcessStatus == (int)BudgetApplyRecordProcessStatus.作废 || applyRecord.ProcessStatus == (int)BudgetApplyRecordProcessStatus.审核退回)
                    {
                        salaryCustomer.BudgetApplyRecordID = null;
                    }
                    else {
                        throw new ErrorCodeException(-1, message: "工资预算申请已存在或非作废状态，请手动作废后重试！");
                    }
                }
                await UpdateStatusPendingReview(id);
                var applyRes = await _financeOtherService.SalaryCustomerAutoComplete(salaryCustomer.ID,info.UserGID,info.FullName,info.CompanyID.Value,info.CompanyGID.Value);
                if (applyRes != null && applyRes.Code == 200 && applyRes.Data > 0)
                {
                    salaryCustomer.BudgetApplyRecordID = applyRes.Data;
                }
                else
                {
                    throw new ErrorCodeException(-1, $"工资预算申请失败，{applyRes?.Message}");
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id", id);
                dic.Add("BudgetApplyRecordID", salaryCustomer.BudgetApplyRecordID);
                await _salaryCustomerService.DictionarySet(dic);
                // 工资状态改成待审核
                
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();

                throw new ErrorCodeException(-1, ex.Message);
            }

            return Ok();
        }

        #region 银行制单
        //[HttpPost]
        //public async Task<IActionResult> ExportMinShengExcel([FromForm] Guid id)
        //{
        //    var model = await _salaryCustomerService.Get(id);
        //    if (model == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
        //    if (model.Status != SalaryStatusEnumType.PendingSend)
        //    {
        //        throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

        //    }
        //    if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
        //    {
        //        throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
        //    }
        //    // 未锁定则自动锁定
        //    if (!model.IsLocked)
        //    {
        //        await _salaryCustomerService.SetLocked(model.ID, true);

        //    }


        //    var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id&&f.ActualSalarySendCardAmount>0&&f.IsCashSend==false&&f.IsDelayPayment==false);
        //    var excelData = recordList.Select(f => new MinShengYinHangExcelTemplate()
        //    {
        //        BankNameNo = string.Empty,
        //        BankNo = f.BankNo,
        //        BankUserName = f.BankUserName,
        //        Remark = f.CustomerName,
        //        Summary = $"{18+f.SalaryMonth.Value.Month}-{f.SalaryMonth.Value.Month}月工资",
        //        TransactionAmount = f.ActualSalarySendCardAmount
        //    }).ToList();

        //    var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.MinShengTempUri);
        //    var fileByteArray = NpoiExcelHelper.WriteMinShengYinHang(excelData, tempBase64);
        //    string base64String = Convert.ToBase64String(fileByteArray);
        //    return Ok(new { base64String, fileName = "民生银行-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

        //    //return File(fileByteArray,  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","11.xls" );

        //}
        //[HttpPost]
        //public async Task<IActionResult> ExportJianSheExcel([FromForm] Guid id)
        //{

        //    var model = await _salaryCustomerService.Get(id);
        //    if (model == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
        //    if (model.Status != SalaryStatusEnumType.PendingSend)
        //    {
        //        throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

        //    }
        //    if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
        //    {
        //        throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
        //    }
        //    // 未锁定则自动锁定
        //    if (!model.IsLocked)
        //    {
        //        await _salaryCustomerService.SetLocked(model.ID, true);

        //    }

        //    var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.ActualSalarySendCardAmount > 0 && f.IsCashSend == false && f.IsDelayPayment == false);

        //    var excelData = recordList.Select(f => new JianSheYinHangExcelTemplate()
        //    {
        //        BankNameNo = string.Empty,
        //        BankNo = f.BankNo,
        //        BankName=f.Bank,
        //        BankUserName = f.BankUserName,
        //        Remark = f.CustomerName,
        //        Summary = $"{f.SalaryMonth.Value.ToString("yyyy-MM")}月工资",
        //        TransactionAmount = f.ActualSalarySendCardAmount
        //    }).ToList();
        //    var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.JianSheTempUri);
        //    var fileByteArray = NpoiExcelHelper.WriteJianSheYinHang(excelData, tempBase64);
        //    string base64String = Convert.ToBase64String(fileByteArray);
        //    return Ok(new { base64String, fileName = "建设银行-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

        //}

        //[HttpPost]
        //public async Task<IActionResult> ExportZhaoShangExcel([FromForm] Guid id)
        //{

        //    var model = await _salaryCustomerService.Get(id);
        //    if (model == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
        //    if (model.Status != SalaryStatusEnumType.PendingSend)
        //    {
        //        throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

        //    }

        //    if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
        //    {
        //        throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
        //    }

        //    // 未锁定则自动锁定
        //    if (!model.IsLocked)
        //    {
        //        await _salaryCustomerService.SetLocked(model.ID, true);

        //    }




        //    var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.ActualSalarySendCardAmount > 0 && f.IsCashSend == false && f.IsDelayPayment == false);

        //    var excelData = recordList.Select(f => new ZhaoShangYinHangExcelTemplate()
        //    {
        //        BankNo = f.BankNo,
        //        BankUserName = f.BankUserName,
        //        Remark = $"{f.CustomerName}_{f.SalaryMonth.Value.ToString("yyyy-MM")}",
        //        TransactionAmount = f.ActualSalarySendCardAmount,
        //        BankAddreess = string.Empty,
        //        BankName = f.Bank,

        //    }).ToList();
        //    var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.ZhaoShangTempUri);
        //    var fileByteArray = NpoiExcelHelper.WriteZhaoShangYinHang(excelData, tempBase64);
        //    string base64String = Convert.ToBase64String(fileByteArray);
        //    return Ok(new { base64String, fileName = "招商-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        //}

        //[HttpPost]
        //public async Task<IActionResult> ExportYouZhengExcel([FromForm]Guid id)
        //{
        //    var model = await _salaryCustomerService.Get(id);
        //    if (model == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
        //    if (model.Status != SalaryStatusEnumType.PendingSend)
        //    {
        //        throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

        //    }
        //    if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
        //    {
        //        throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
        //    }
        //    // 未锁定则自动锁定
        //    if (!model.IsLocked)
        //    {
        //        await _salaryCustomerService.SetLocked(model.ID, true);

        //    }


        //    var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.ActualSalarySendCardAmount > 0 && f.IsCashSend == false && f.IsDelayPayment == false);

        //    var excelData = recordList.Select(f => new YouZhengYinHangExcelTemplate()
        //    {
        //        BankNo = f.BankNo,
        //        BankUserName = f.BankUserName,
        //        Remark =string.Empty,
        //        TransactionAmount = f.ActualSalarySendCardAmount,
        //        BankNameNo = string.Empty,
        //        IdCardNo = f.IdCardNo,
        //        IdType = string.Empty,
        //        IsYouZhengBank = string.Empty,
        //        SignNo = string.Empty,

        //    }).ToList();
        //    var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.YouZhengTempUri);
        //    var fileByteArray = NpoiExcelHelper.WriteYouZhengYinHang(excelData, tempBase64);
        //    string base64String = Convert.ToBase64String(fileByteArray);
        //    return Ok(new { base64String, fileName = "邮政-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        //}

        #endregion



        #region 银行制单
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ExportMinShengExcel(List<Guid> ids)
        {
            List<Live_SalaryRecord> salaryRecords = new List<Live_SalaryRecord>();

            try
            {
                DbScoped.SugarScope.BeginTran();
                foreach (var id in ids)
                {
                    var model = await _salaryCustomerService.Get(id);
                    if (model == null)
                    {
                        throw new ErrorCodeException(-1, "你要读取的数据不存在");
                    }
                    if (model.Status != SalaryStatusEnumType.PendingSend)
                    {
                        throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

                    }

                    if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
                    {
                        throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
                    }

                    // 未锁定则自动锁定
                    if (!model.IsLocked)
                    {
                        await _salaryCustomerService.SetLocked(model.ID, true);
                    }

                    var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.ActualSalarySendCardAmount > 0 && f.IsCashSend == false && f.IsDelayPayment == false);

                    if (recordList != null && recordList.Count > 0)
                    {
                        salaryRecords.AddRange(recordList);
                    }

                }
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw ex;
            } 

            var excelData = salaryRecords.Select(f => new MinShengYinHangExcelTemplate()
            {
                BankNameNo = string.Empty,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                Remark = f.CustomerName,
                Summary = $"{18 + f.SalaryMonth.Value.Month}-{f.SalaryMonth.Value.Month}月工资",
                TransactionAmount = f.ActualSalarySendCardAmount
            }).ToList();

            var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.MinShengTempUri);
            var fileByteArray = NpoiExcelHelper.WriteMinShengYinHang(excelData, tempBase64);
            string base64String = Convert.ToBase64String(fileByteArray);
            return Ok(new { base64String, fileName = "民生银行-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

            //return File(fileByteArray,  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","11.xls" );

        }
        [HttpPost]
        public async Task<IActionResult> ExportJianSheExcel(List<Guid> ids)
        {

            List<Live_SalaryRecord> salaryRecords = new List<Live_SalaryRecord>();
            foreach (var id in ids)
            {
                var model = await _salaryCustomerService.Get(id);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "你要读取的数据不存在");
                }
                if (model.Status != SalaryStatusEnumType.PendingSend)
                {
                    throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

                }

                if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
                {
                    throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
                }

                // 未锁定则自动锁定
                if (!model.IsLocked)
                {
                    await _salaryCustomerService.SetLocked(model.ID, true);
                }

                var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.ActualSalarySendCardAmount > 0 && f.IsCashSend == false && f.IsDelayPayment == false);

                if (recordList != null && recordList.Count > 0)
                {
                    salaryRecords.AddRange(recordList);
                }

            }
            var excelData = salaryRecords.Select(f => new JianSheYinHangExcelTemplate()
            {
                BankNameNo = string.Empty,
                BankNo = f.BankNo,
                BankName = f.Bank,
                BankUserName = f.BankUserName,
                Remark = f.CustomerName,
                Summary = $"{f.SalaryMonth.Value.ToString("yyyy-MM")}月工资",
                TransactionAmount = f.ActualSalarySendCardAmount
            }).ToList();
            var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.JianSheTempUri);
            var fileByteArray = NpoiExcelHelper.WriteJianSheYinHang(excelData, tempBase64);
            string base64String = Convert.ToBase64String(fileByteArray);
            return Ok(new { base64String, fileName = "建设银行-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

        }

        [HttpPost]
        public async Task<IActionResult> ExportZhaoShangExcel( List<Guid> ids)
        {


            List<Live_SalaryRecord> salaryRecords = new List<Live_SalaryRecord>();
            foreach (var id in ids)
            {
                var model = await _salaryCustomerService.Get(id);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "你要读取的数据不存在");
                }
                if (model.Status != SalaryStatusEnumType.PendingSend)
                {
                    throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

                }

                if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
                {
                    throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
                }

                // 未锁定则自动锁定
                if (!model.IsLocked)
                {
                    await _salaryCustomerService.SetLocked(model.ID, true);
                }

                var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.ActualSalarySendCardAmount > 0 && f.IsCashSend == false && f.IsDelayPayment == false);

                if (recordList!=null&& recordList.Count>0)
                {
                    salaryRecords.AddRange(recordList);
                }

            }






            var excelData = salaryRecords.Select(f => new ZhaoShangYinHangExcelTemplate()
            {
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                Remark = $"{f.CustomerName}_{f.SalaryMonth.Value.ToString("yyyy-MM")}",
                TransactionAmount = f.ActualSalarySendCardAmount,
                BankAddreess = string.Empty,
                BankName = f.Bank,

            }).ToList();
            var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.ZhaoShangTempUri);
            var fileByteArray = NpoiExcelHelper.WriteZhaoShangYinHang(excelData, tempBase64);
            string base64String = Convert.ToBase64String(fileByteArray);
            return Ok(new { base64String, fileName = "招商-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpPost]
        public async Task<IActionResult> ExportYouZhengExcel(List<Guid> ids)
        {
            List<Live_SalaryRecord> salaryRecords = new List<Live_SalaryRecord>();
            foreach (var id in ids)
            {
                var model = await _salaryCustomerService.Get(id);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "你要读取的数据不存在");
                }
                if (model.Status != SalaryStatusEnumType.PendingSend)
                {
                    throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");

                }

                if (!model.IsCustomerPaid && !model.IsPlatformPrepaid)
                {
                    throw new ErrorCodeException(-1, message: "客户未打款或者平台未垫付 无法执行此操作！");
                }

                // 未锁定则自动锁定
                if (!model.IsLocked)
                {
                    await _salaryCustomerService.SetLocked(model.ID, true);
                }

                var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.ActualSalarySendCardAmount > 0 && f.IsCashSend == false && f.IsDelayPayment == false);

                if (recordList != null && recordList.Count > 0)
                {
                    salaryRecords.AddRange(recordList);
                }

            }
            var excelData = salaryRecords.Select(f => new YouZhengYinHangExcelTemplate()
            {
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                Remark = string.Empty,
                TransactionAmount = f.ActualSalarySendCardAmount,
                BankNameNo = string.Empty,
                IdCardNo = f.IdCardNo,
                IdType = string.Empty,
                IsYouZhengBank = string.Empty,
                SignNo = string.Empty,

            }).ToList();
            var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.YouZhengTempUri);
            var fileByteArray = NpoiExcelHelper.WriteYouZhengYinHang(excelData, tempBase64);
            string base64String = Convert.ToBase64String(fileByteArray);
            return Ok(new { base64String, fileName = "邮政-模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        #endregion

        [HttpPut]
        public async Task<IActionResult> SalarySlipNotice([FromForm]Guid id)
        {

            var model = await _salaryCustomerService.Get(id);

            if (model.SalarySlipID==null||model.SalarySlipID.Value==Guid.Empty)
            {
                throw new ErrorCodeException(-1, message: "工资条未上传，无法执行此操作");
            }


            var recordList = await _salaryRecordService.List(f => f.SalaryCustomerID == id && f.SalarySlipRecordID != null && f.SalarySlipRecordID != Guid.Empty);

            model.IsNoticeCompleted = true;

          await  _salaryCustomerService.Update(model);

            foreach (var item in recordList)
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

                _platformNoticeService.EmpSalarySlipNotice(item.UserID.Value, item.SalaryMonth.Value, item.ActualSalarySendCardAmount, item.CustomerName);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSalarySendTime(Guid customerId)
        {
           var model =  await _salaryCustomerService.Get(it=>it.CustomerGID==customerId && it.SendTime.HasValue);
            return Ok(model.SendTime);
        }


        #region 第三方特殊处理
        [HttpPost]
        public IActionResult ThridDownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<ThridSalaryRecordExcelTemplate>(stream, "第三方工资结算单");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "第三方工资结算单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }


        /// <summary>
        /// 上传工资结算单
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        public async Task<IActionResult> ThridUploadSalaryRecord(IFormFile file)
        {


            List<string> cmbcNameList = new List<string>() {
                "宁波莱博友翌供应链管理有限公司",
                "宁波新莱博友盟供应链管理有限公司",
                "宁波新莱博金浩供应链管理有限公司",
                "宁波蓝鑫人力资源有限公司",
                "宁波莱鑫人力资源有限公司"
            };
            // 当月月初
            var taxMonth = DateTime.Now.Date.AddDays(1 - DateTime.Today.Day);
            HttpContext.Request.Headers.TryGetValue("authorization", out var value);
            List<ThridSalaryRecordExcelTemplate> salaryRecordExcelTemplates = new List<ThridSalaryRecordExcelTemplate>();

            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                salaryRecordExcelTemplates = NpoiExcelHelper.ReadExcelFile<ThridSalaryRecordExcelTemplate>(ms, "第三方工资结算单");
            }
            if (salaryRecordExcelTemplates == null || salaryRecordExcelTemplates.Count <= 0)
            {
                throw new Exception("未获取到【客户结算清单】的记录信息，请核对后重新上传");
            }

            salaryRecordExcelTemplates = salaryRecordExcelTemplates.Where(f => !string.IsNullOrWhiteSpace(f.IdCardNo)).ToList();

            var sameIdCardNos = salaryRecordExcelTemplates.GroupBy(f => f.IdCardNo).Where(f => f.Count() > 1).Select(f => f.Key).ToList();
            if (sameIdCardNos.Any())
            {
                throw new Exception($"检测到身份证号码重复记录,重复记录：{string.Join('、', sameIdCardNos)}");
            }


            if (salaryRecordExcelTemplates.Select(f => f.SalaryMonth).Distinct().Count() != 1)
            {
                throw new Exception("无法多个工资所属月的工资同时导入！");
            }
            if (salaryRecordExcelTemplates.Select(f => f.CustomerName).Distinct().Count() != 1)
            {
                throw new Exception("无法多个客户单位的工资同时导入！");
            }
            if (salaryRecordExcelTemplates.Any(f => f.UnitSalaryAmount < 0))
            {
                throw new Exception("企业应发工资不能小于0，请仔细核对数据！");
            }


            var salaryMonth = salaryRecordExcelTemplates.Max(f => f.SalaryMonth);

            List<Live_SalaryCustomerExtension> salaryCustomers = new List<Live_SalaryCustomerExtension>();
            Live_SalaryCustomerExtension salaryCustomer = null;
            Live_V_Customer customerInfo = null;

            //客户单位信息
            var customerRes = await _financeOtherService.GetCustomerByNameAsync(salaryRecordExcelTemplates.First().CustomerName);
            if (customerRes != null && customerRes.Code == 200 && customerRes.Data != null)
            {
                customerInfo = customerRes.Data;
            }
            var salaryConfig = await _salaryConfigService.Get(f => f.CustomerGID == customerInfo.CustomerGID && f.IsEnabled);

            if (salaryConfig == null)
            {
                throw new Exception("客户单位的工资配置未设置！");
            }

            if (customerInfo.AgentName.IsNullOrWhiteSpace() || customerInfo.AgentIdCardNo.IsNullOrWhiteSpace() || customerInfo.AgentBank.IsNullOrWhiteSpace() || customerInfo.AgentBankNo.IsNullOrWhiteSpace())
            {
                throw new Exception("客户单位现金发放代理信息信息缺失！");
            }
            List<Live_SalaryRecordExtension> salaryRecordExtensions = salaryRecordExcelTemplates.Select(f => new Live_SalaryRecordExtension()
            {

                IsFailSalary = false,
                ID = Guid.NewGuid(),
                //CompanyName = f.CompanyName,
                UnitDeductionAmount = 0,
                UnitSalaryAmount = f.UnitSalaryAmount,
                CreateTime = DateTime.Now,
                CustomerName = customerInfo.CustomerName,
                // 税后扣除
                DeductionNoTaxAmount = f.ZhuSuAmount+f.ShuiDianAmount+f.ShangBaoAmunt+f.TiJianAmunt+f.XianXiaYuZhiAmount+ f.QiTaKouChuAmount ,
                SalaryMonth = f.SalaryMonth,
                CompanyName = f.CustomerName,
                IdCardNo = f.IdCardNo,
                SocialGongJiJinAmount = 0,
                IsDelayPayment = false,
                Name = f.Name,
                PlatformCommissionAmount = 0,
                PlatformDepartment = string.Empty,
                PlatformWelfareAmount = 0,
                Remark = f.Remark,
                SendMonth = f.SalaryMonth,
                ServiceFeeAmount = f.ServiceFeeAmount,
                SocialUnitSumAmount =0,
                SocialDaBingAmount = 0,
                SocialShiYeAmount = 0,
                SocialYangLaoAmount =0 ,
                SocialYiLiaoAmount =0,
                SubsidyAmount = 0,
                SubsidyNoTaxAmount =0,
                Workshop = string.IsNullOrWhiteSpace(f.Workshop) ? string.Empty : f.Workshop,
                SpJiaoYuAmount = 0,
                SpZiNvAmount = 0,
                SpDaiKuanAmount = 0,
                SpZuJinAmount = 0,
                SpShanYangAmount = 0,
                SpYingErAmount = 0,
                InnerServiceFeeAmount = 0,
                ActualSalaryAmount = 0,
                //SalaryCustomerID = null,
                SocialDaBingTotalAmount = 0,
                SocialShiYeTotalAmount = 0,
                SocialYangLaoTotalAmount = 0,
                SocialYiLiaoTotalAmount = 0,
                TaxSpDaiKuanTotalAmount = 0,
                TaxSpJiaoYuTotalAmount = 0,
                TaxSpShanYangTotalAmount = 0,
                TaxSpTotalAmount = 0,
                TaxSpYingErTotalAmount = 0,
                TaxSpZiNvTotalAmount = 0,
                TaxSpZuJinTotalAmount = 0,
                TaxTimes = 0,
                TaxMonthAmount = 0,
                BeforeTaxSalaryAmount = 0,
                BeforeTaxSalaryTotalAmount = 0,
                TaxSalaryTotalAmount = 0,
                SpBasicTotalAmount = 0,
                //SalaryType =  SalaryRecordTypeEnumType.常规工资,
                IsCashSend = false,
                AfterTaxAmount = 0,
                Bank = f.Bank,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName ?? f.Name,
                CompanySalarySumAmount = 0,
                CompanyTotalAmount = 0,
                EntryDate =DateTime.MinValue,
                LeaveDate = null,
                PreSendAmount = 0,
                SocialEmpSumAmount = 0,
                UserEntryID = 0,
                TaxTotalAmount = 0,
                LastTaxTotalAmount = 0,
                LastBeforeTaxSalaryTotalAmount = 0,
                LastSocialDaBingTotalAmount = 0,
                LastSocialEmpTotalAmount = 0,
                LastSocialGongJiJinTotalAmount = 0,
                LastSocialShiYeTotalAmount = 0,
                LastSocialYangLaoTotalAmount = 0,
                LastSocialYiLiaoTotalAmount = 0,
                SocialEmpTotalAmount = 0,
                SocialGongJiJinTotalAmount = 0,
                SpBasiclAmount = 0,
                SpSumAmount = 0,
                SpTotalAmount = 0,
                TaxSocialTotalAmount = 0,
                VerifyInfoOk = true,
                ActualSalarySendCardAmount = 0,
                IsCashSignSuc = false,
                IsDelayPaymentCancel = false,
                MonthNetSalaryAmount = 0,
                PrepaidRepaymentAmount = 0,
                //SalaryDelayType = f.IsDelayPayment ? SalaryDelaySendTypeEnumType.客服暂缓 : SalaryDelaySendTypeEnumType.无,
                Status = SalaryRecordSendStatusEnumType.待处理,
                SendType = SalaryRecordSendTypeEnumType.线上发放,
                Phone = string.Empty,
                BankChangeFilePath = string.Empty,
                DelayType =  SalaryDelaySendTypeEnumType.未暂缓,
                IsDelayPaymentSuc = false,
                UnitManageAmount = 0,
                MonthWorkDays = 0,
                //IsPushTrans = false,
                IsTransBatch = false,
                SettlementID = null,
                //TransNonce = Guid.NewGuid(),
                TransStatus = string.Empty,
                TransStatusName = string.Empty,
                SalaryYear =0,
                SalaryRecordType = SalaryRecordTypeEnumType.常规工资,
                BatchTransStatus = string.Empty,
                BatchTransStatusName = string.Empty,
                DelayPaymentAmount = 0,
                IsBatchPushSuc = false,
                IsBatchSend = false,
                PrepaidTotalAmount = 0,
                IsSinglePushSuc = false,
                IsSingleSend = false,
                UserID = 0,
                IsCombineSalary = false,
                ChangeAmount = 0,
                ChangeHash = string.Empty,
                ChangeRemark = string.Empty,
                ChangeUpdateTime = null,
                SendTime = null,
                TransDescription = string.Empty,
                TransMsg = string.Empty,
                IsAutoLocked = true,
                // 第三方专用
                ZhuSuAmount = f.ZhuSuAmount,
                ShuiDianAmount = f.ShangBaoAmunt,
                ShangBaoAmunt = f.ShangBaoAmunt,
                TiJianAmunt = f.TiJianAmunt,
                XianXiaYuZhiAmount = f.XianXiaYuZhiAmount,
                QiTaKouChuAmount = f.QiTaKouChuAmount,
            }).ToList();


            #region 结算费用


            var idCardNoList = salaryRecordExtensions.Select(f => f.IdCardNo).ToList();

            // 签章单位与商务合作单位不一致则收取管理费
            if (customerInfo.SigningUnitName != customerInfo.CooperationUnitName)
            {
                // 工资所属月已收取过公司管费的员工
                var salaryMonthRecordList = await _salaryCustomerService.QuerySalaryRecordViewList(f =>
                 f.SalaryYear == 0
                 && f.IsCombineSalary == false
                && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资
                && f.CustomerName == customerInfo.CustomerName
                && f.SalaryMonth == salaryMonth
                && idCardNoList.Contains(f.IdCardNo)
                && f.UnitManageAmount > 0
                );
                foreach (var item in salaryRecordExtensions)
                {
                    if (item.SalaryRecordType == SalaryRecordTypeEnumType.常规工资)
                    {
                        if (!salaryMonthRecordList.Any(f => f.IdCardNo == item.IdCardNo))
                        {
                            item.UnitManageAmount = 90;
                        }
                    }
                }
            }
            #endregion



            #region 获取民生电子卡
            //if (customerInfo.SigningUnitName== "宁波莱鑫人力资源有限公司"|| customerInfo.SigningUnitName == "宁波蓝鑫人力资源有限公司")
            //if (cmbcNameList.Contains(customerInfo.SigningUnitName))
            //{
            //    var cmbcAccounts = await _financeOtherService.GetCmbcAccounts(idCardNoList);
            //    if (cmbcAccounts.Success && cmbcAccounts.Result != null && cmbcAccounts.Result.Count > 0)
            //    {
            //        foreach (var item in cmbcAccounts.Result)
            //        {
            //            var record = salaryRecordExtensions.First(f => f.IdCardNo == item.IdCardNo);
            //            record.Bank = "民生银行";
            //            record.BankNo = item.CmbcAccountNo;
            //            record.BankUserName = item.Name;
            //        }
            //    }
            //}
            #endregion

            #region 现金发放匹配
            var today = DateTime.Today;
            var whiteListRes = await _financeOtherService.GeCashWhiteAllListAsync();
            List<Live_WhiteList> whiteLists = new List<Live_WhiteList>();
            if (whiteListRes != null && whiteListRes.Code == 200 && whiteListRes.Data != null)
            {
                whiteLists = whiteListRes.Data.Where(f => f.Enabled && f.EndTime != null && f.EndTime.Value >= today).ToList();
            }
            Live_WhiteList cashWhiteName = null;
            if (whiteLists.Any())
            {
                salaryRecordExtensions.AsParallel().ForAll(f =>
                {
                    if (whiteLists.Any(x => x.IdentityCard == f.IdCardNo))
                    {

                        var whiteRecord = whiteLists.FirstOrDefault(x => x.IdentityCard == f.IdCardNo);
                        f.CashTypeName = whiteRecord.LabelName;
                        f.IsCashSend = true;
                        f.SendType = SalaryRecordSendTypeEnumType.线下发放;
                        //cashWhiteName = whiteLists.First(x => x.IdentityCard == f.IdCardNo);
                        f.Bank = customerInfo.AgentBank;
                        f.BankNo = customerInfo.AgentBankNo;
                        f.BankUserName = customerInfo.AgentName;
                    }
                });
            }
            #endregion


            #region 员工信息匹配
            if (salaryConfig.IsVerifyEmpInfo)
            {
                var entryExp = Expressionable.Create<Live_UserEntry>();
                entryExp.And(f => f.CustomerGID == customerInfo.CustomerGID);
                entryExp.And(f => f.EntryTime != null);
                entryExp.And(f => f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= salaryMonth.AddMonths(-3)));


                var userEntries = await _userEntryService.List(entryExp.ToExpression());

                Live_UserEntry userEntry = null;
                foreach (var record in salaryRecordExtensions)
                {
                    // 优先匹配在职
                    if (userEntries.Any(f => f.Status.Value == 2 && f.CardID == record.IdCardNo && f.Name == record.Name))
                    {
                        userEntry = userEntries.First(f => f.Status.Value == 2 && f.CardID == record.IdCardNo && f.Name == record.Name);
                    }
                    else if (userEntries.Any(f => f.Status == 4 && f.CardID == record.IdCardNo && f.Name == record.Name))
                    {
                        userEntry = userEntries.First(f => f.Status == 4 && f.CardID == record.IdCardNo && f.Name == record.Name);
                    }
                    else
                    {
                        userEntry = userEntries.FirstOrDefault(f => f.CardID == record.IdCardNo && f.Name == record.Name);
                    }
                    if (userEntry != null)
                    {

                        // 记录中有银行卡信息则不会替换
                        if (string.IsNullOrWhiteSpace(record.Bank) || string.IsNullOrWhiteSpace(record.BankNo) || string.IsNullOrWhiteSpace(record.BankUserName))
                        {
                            record.Bank = userEntry.BankName;
                            record.BankNo = userEntry.BankNumber;
                            record.BankUserName = userEntry.Name;
                        }
                        record.Phone = userEntry.Phone;
                        record.UserEntryID = userEntry.ID;
                        record.EntryDate = userEntry.EntryTime.Value.Date;
                        record.UserID = userEntry.UserID;
                        if (userEntry.LeaveTime.HasValue)
                        {
                            record.LeaveDate = userEntry.LeaveTime.Value.Date;
                        }
                        // 只针对线上员工
                        if (salaryConfig.IsContractDelay && userEntry.UserID.HasValue && userEntry.UserID > 0)
                        {
                            // 在职且合同未签署
                            if (userEntry.Status == 2 && (userEntry.ContractStatus == null || userEntry.ContractStatus.Value != (int)ContractStatus.EntryComplete))
                            {

                                // 入职超过15天限制
                                if (userEntry.EntryTime.HasValue && userEntry.EntryTime.Value.Date <= DateTime.Now.Date.AddDays(-15))
                                {
                                    record.IsDelayPayment = true;
                                    record.IsDelayPaymentSuc = false;
                                    record.DelayType = SalaryDelaySendTypeEnumType.网签暂缓;
                                }
                            }
                            else if (userEntry.Status == 4 && userEntry.ContractStatus != null && userEntry.ContractStatus.Value != (int)ContractStatus.LeaveComplete && userEntry.ContractStatus.Value >= (int)ContractStatus.EntryComplete)
                            {
                                // 离职超过3天的不限制
                                if (userEntry.LeaveTime.HasValue && userEntry.LeaveTime.Value.Date > DateTime.Now.Date.AddDays(-3))
                                {
                                    record.IsDelayPayment = true;
                                    record.IsDelayPaymentSuc = false;
                                    record.DelayType = SalaryDelaySendTypeEnumType.网签暂缓;
                                }

                            }
                        }



                    }
                    else
                    {
                        record.Remark = "员工信息匹配失败";
                    }

                    if (string.IsNullOrWhiteSpace(record.Bank) || string.IsNullOrWhiteSpace(record.BankNo) || record.Bank.Length <= 2 || record.BankNo.Length <= 2)
                    {
                        if (!record.IsDelayPayment)
                        {
                            record.IsDelayPayment = true;
                            record.IsDelayPaymentSuc = false;
                            record.DelayType = SalaryDelaySendTypeEnumType.银行卡暂缓;
                        }
                        else
                        {
                            if (record.DelayType > SalaryDelaySendTypeEnumType.银行卡暂缓)
                            {
                                record.IsDelayPayment = true;
                                record.IsDelayPaymentSuc = false;
                                record.DelayType = SalaryDelaySendTypeEnumType.银行卡暂缓;
                            }

                        }
                    }
                    // 取消银行卡暂缓限制条件
                    //else if (userEntry != null && userEntry.ContractStatus != (int)ContractStatus.EntryComplete && userEntry.ContractStatus != (int)ContractStatus.LeaveComplete)
                    //{

                    //    if (!record.IsDelayPayment)
                    //    {
                    //        record.IsDelayPayment = true;
                    //        record.IsDelayPaymentSuc = false;

                    //        record.DelayType = SalaryDelaySendTypeEnumType.网签暂缓;
                    //    }
                    //}
                }
                if (salaryRecordExtensions.Any(f => f.UserEntryID <= 0))
                {
                    throw new ErrorCodeException(-1, $"员工信息匹配失败，人员名单:{string.Join(',', salaryRecordExtensions.Where(f => f.UserEntryID <= 0).Select(f => f.Name))}");
                }
            }
            #endregion
            //未验证员工信息则提示银行卡信息缺失
            if (!salaryConfig.IsVerifyEmpInfo)
            {
                if (salaryRecordExtensions.Any(f => string.IsNullOrWhiteSpace(f.Bank) || string.IsNullOrWhiteSpace(f.BankNo)))
                {
                    throw new ErrorCodeException(-1, $"银行卡信息缺失，人员名单:{string.Join(',', salaryRecordExtensions.Where(f => string.IsNullOrWhiteSpace(f.Bank) || string.IsNullOrWhiteSpace(f.BankNo)).Select(f => f.Name))}");
                }
            }

            // 应发为0的情况下，不生成暂缓和现金签收单
            //应发为了0，实发不为了0的是个别情况，不考虑在主流程中了 2025-08-20

            foreach (var item in salaryRecordExtensions)
            {
                var beforTax = item.UnitSalaryAmount + item.SubsidyAmount + item.SubsidyNoTaxAmount;
                if (item.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && beforTax <= 0)
                {

                    item.SendType = SalaryRecordSendTypeEnumType.线上发放;
                    item.IsDelayPayment = false;
                    item.IsCashSend = false;

                    item.DelayType = SalaryDelaySendTypeEnumType.未暂缓;

                }

                if (item.IsDelayPayment || item.IsCashSend)
                {
                    item.IsSingleSend = true;
                    item.IsSinglePushSuc = false;
                }
            }



            var companySalaryGroup = salaryRecordExtensions.GroupBy(f => new { f.CustomerName, f.SalaryMonth, f.Workshop, f.SalaryYear }).ToList();
            foreach (var itemGroup in companySalaryGroup)
            {

                salaryCustomer = new Live_SalaryCustomerExtension()
                {

                    ID = Guid.NewGuid(),
                    IsCustomerPaid = false,
                    // 客户信息
                    CustomerGID = customerInfo.CustomerGID.Value,
                    CustomerName = customerInfo.CustomerName,
                    CustomerCompanyAddress = customerInfo.Address,
                    CustomerCompanyBank = customerInfo.Bank,
                    CustomerCompanyBankNo = customerInfo.BankAccount,
                    CustomerCompanyNo = customerInfo.CompanyNo,
                    CustomerCompanyPhone = customerInfo.Phone,
                    CooperationUnitID = customerInfo.CooperationUnitGID.Value,
                    // 签章单位信息
                    SigningUnitID = customerInfo.SigningUnitGID.Value,
                    SigningUnitCompanyName = customerInfo.SigningUnitName,
                    SigningUnitCompanyNo = customerInfo.SigningUnitCompanyNo,
                    SigningUnitCompanyBank = customerInfo.SigningUnitBankName,
                    SigningUnitCompanyBankNo = customerInfo.SigningUnitBankNo,
                    SigningUnitCompanyAddress = customerInfo.signingUnitAddress,
                    SigningUnitCompanyPhone = customerInfo.SigningUnitPhone,

                    CooperationUnitCompanyName = customerInfo.CooperationUnitName,
                    CooperationUnitCompanyAddress = customerInfo.CooperationUnitAddress,
                    CooperationUnitCompanyBank = customerInfo.CooperationUnitBankName,
                    CooperationUnitCompanyBankNo = customerInfo.CooperationUnitBankNo,
                    CooperationUnitCompanyNo = customerInfo.CooperationUnitCompanyNo,
                    CooperationUnitCompanyPhone = customerInfo.CooperationUnitPhone,
                    ActualSalaryAmount = 0,
                    ManagementFeeAmount = 0,
                    SalaryBatchType = 0,
                    SalarySendType = 0,
                    Status = SalaryStatusEnumType.AwaitingCalculation,
                    TaxMonth = taxMonth,
                    //CompanyGID = customerInfo.CompanyGID.Value,
                    CreateTime = DateTime.Now,
                    SealCompanyName = string.Empty,
                    SendMonth = taxMonth,
                    SalaryMonth = itemGroup.Key.SalaryMonth,
                    CompanyDeductionAmount = itemGroup.Sum(f => f.UnitDeductionAmount),
                    CompanySendSalaryAmount = itemGroup.Sum(f => f.UnitSalaryAmount),
                    CompanySocialSecurityAmount = itemGroup.Sum(f => f.SocialUnitSumAmount),
                    DeductionNoTaxAmount = itemGroup.Sum(f => f.DeductionNoTaxAmount),
                    ServiceFeeAmount = itemGroup.Sum(f => f.ServiceFeeAmount),
                    SubsidyAmount = itemGroup.Sum(f => f.SubsidyAmount),
                    SubsidyNoTaxAmount = itemGroup.Sum(f => f.SubsidyNoTaxAmount),
                    WorkShop = itemGroup.Key.Workshop ?? string.Empty,
                    Records = itemGroup.ToList(),
                    AccountsReceivableSumAmount = 0,
                    CompanySendSalarySumAmount = 0,
                    CompanySettlementSumAmount = 0,
                    InnerServiceFeeAmount = 0,
                    ActualSalarySumAmount = 0,

                    ServiceFeeRecordCount = itemGroup.Count(f => f.ServiceFeeAmount > 0),
                    IsCompanyPosted = false,
                    IsMainBatchSalary = false,
                    IsMatchZeroSalary = false,
                    BeforeTaxSalarySumAmount = 0,
                    CustomerBillTotalAmount = 0,
                    MonthNetSalaryAmount = 0,
                    MonthNetSalarySumAmount = 0,
                    ReceivableTotalAmount = 0,
                    ServiceFeeSumAmount = 0,
                    ServiceRecordCount = itemGroup.Count(f => f.ServiceFeeAmount > 0),
                    SocialUnitSumAmount = 0,
                    UnitDeductionSumAmount = 0,
                    UnitManageSumAmount = 0,
                    UnitSalarySumAmount = 0,
                    UnitSalaryTotalAmount = 0,
                    UnitSettlementAmount = 0,
                    RecordCount = 0,
                    ZeroSalaryCount = 0,
                    IsCmbcSend = false,
                    IsPushTrans = false,
                    TransNonce = Guid.NewGuid(),
                    TransStatus = string.Empty,
                    TransStatusName = string.Empty,
                    SalaryYear = itemGroup.Key.SalaryYear,
                    SalaryType = itemGroup.Key.SalaryYear <= 0 ? SalaryTypeEnumType.常规工资 : SalaryTypeEnumType.年终奖工资,
                    IsCanCmbcSend = false,
                    IsPlatformPrepaid = false,
                    SalarySlipID = null,
                    IsSalarySlip = salaryConfig.IsVerifyPaySlip,
                    IsHasOutInvoice = false,
                    IsCombineSalary = false,
                    CombineCompanyGID = salaryConfig.CombineCompanyGID,
                    CombineCustomerSalaryID = string.Empty,
                    CompanyID = customerInfo.CompanyID,
                    WorkShopBatchNo = null
                };
                //if (salaryCustomer.SigningUnitCompanyName == "宁波莱鑫人力资源有限公司" || salaryCustomer.SigningUnitCompanyName == "宁波蓝鑫人力资源有限公司")
                if (cmbcNameList.Contains(salaryCustomer.SigningUnitCompanyName))
                {
                    salaryCustomer.IsCanCmbcSend = true;
                }

                if (salaryCustomers.Count == 0)
                {
                    salaryCustomer.IsMainBatchSalary = true;
                }
                salaryCustomers.Add(salaryCustomer);
            }


            // 补0工资
            if (salaryConfig.IsVerifyEmpInfo)
            {
                #region 补0工资处理
                List<Live_UserEntry> zeroList = new List<Live_UserEntry>();
                var existList = await _salaryCustomerService.List(f => f.TaxMonth == taxMonth && f.Status != SalaryStatusEnumType.Cancelled && f.CustomerGID == customerInfo.CustomerGID);
                if (existList == null || existList.Count <= 0)
                {
                    var lastMonth = taxMonth.AddMonths(-1);

                    var entrZeroExp = Expressionable.Create<Live_UserEntry>();
                    entrZeroExp.And(f => f.CustomerGID == customerInfo.CustomerGID);
                    entrZeroExp.And(f => f.EntryTime != null && f.EntryTime.Value < taxMonth && f.EntryTime.Value >= lastMonth);
                    entrZeroExp.And(f => (f.Status == 2 || (f.Status == 4 && f.LeaveTime != null && f.LeaveTime >= taxMonth)));

                    var userEntryZeroList = await _userEntryService.List(entrZeroExp.ToExpression());

                    if (userEntryZeroList != null)
                    {
                        foreach (var item in userEntryZeroList)
                        {
                            if (salaryRecordExtensions.Any(f => f.IdCardNo == item.CardID))
                            {
                                continue;
                            }

                            if (item.Status == 2)//在职
                            {
                                zeroList.Add(item);
                            }
                            else
                            { // 已离职

                                // 同月入离职
                                if (item.EntryTime.Value.ToString("yyyy-MM") == item.LeaveTime.Value.AddMonths(-1).ToString("yyyy-MM"))
                                {
                                    double days = (item.LeaveTime.Value.Date - item.EntryTime.Value.Date).TotalDays;
                                    if (days >= 7)
                                    {
                                        zeroList.Add(item);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }


                            }
                        }
                    }
                }
                var matchZeroSalry = salaryCustomers.FirstOrDefault(f => f.IsMainBatchSalary && f.SalaryType == SalaryTypeEnumType.常规工资);
                if (matchZeroSalry != null)
                {
                    // 补0工资
                    matchZeroSalry.IsMatchZeroSalary = true;
                    if (zeroList != null && zeroList.Count > 0)
                    {
                        var zeroSalaryRecordList = zeroList.Select(f => new Live_SalaryRecordExtension()
                        {
                            SocialGongJiJinAmount = 0,
                            IdCardNo = f.CardID,
                            AfterTaxAmount = 0,
                            Bank = string.Empty,
                            BankNo = string.Empty,
                            BankUserName = string.Empty,
                            BeforeTaxSalaryAmount = 0,
                            BeforeTaxSalaryTotalAmount = 0,
                            UnitDeductionAmount = 0,
                            CompanyName = matchZeroSalry.CustomerName,
                            UnitSalaryAmount = 0,
                            CompanySalarySumAmount = 0,
                            CompanyTotalAmount = 0,
                            CreateTime = DateTime.Now,
                            CustomerName = matchZeroSalry.CustomerName,
                            DeductionNoTaxAmount = 0,
                            EntryDate = f.EntryTime,
                            InnerServiceFeeAmount = 0,
                            IsCashSend = false,
                            IsDelayPayment = false,
                            LeaveDate = f.LeaveTime,
                            ActualSalaryAmount = 0,
                            Name = f.Name,
                            PlatformCommissionAmount = 0,
                            PlatformDepartment = string.Empty,
                            PlatformWelfareAmount = 0,
                            PreSendAmount = 0,
                            Remark = "补0工资",
                            SalaryMonth = matchZeroSalry.SalaryMonth,
                            SalaryCustomerID = matchZeroSalry.ID,
                            SendMonth = matchZeroSalry.SendMonth,
                            ServiceFeeAmount = 0,
                            SocialUnitSumAmount = 0,
                            SocialDaBingAmount = 0,
                            SocialEmpSumAmount = 0,
                            SocialShiYeAmount = 0,
                            SocialYangLaoAmount = 0,
                            SocialYiLiaoAmount = 0,
                            SpDaiKuanAmount = 0,
                            SpJiaoYuAmount = 0,
                            SpShanYangAmount = 0,
                            SpYingErAmount = 0,
                            SpZiNvAmount = 0,
                            SpZuJinAmount = 0,
                            SubsidyAmount = 0,
                            SubsidyNoTaxAmount = 0,
                            TaxMonthAmount = 0,
                            TaxSalaryTotalAmount = 0,
                            SocialDaBingTotalAmount = 0,
                            SocialShiYeTotalAmount = 0,
                            SocialYangLaoTotalAmount = 0,
                            SocialYiLiaoTotalAmount = 0,
                            SpBasicTotalAmount = 0,
                            TaxSpDaiKuanTotalAmount = 0,
                            TaxSpJiaoYuTotalAmount = 0,
                            TaxSpShanYangTotalAmount = 0,
                            TaxSpTotalAmount = 0,
                            TaxSpYingErTotalAmount = 0,
                            TaxSpZiNvTotalAmount = 0,
                            TaxSpZuJinTotalAmount = 0,
                            TaxTimes = 0,
                            UserEntryID = f.ID,
                            Workshop = matchZeroSalry.WorkShop,
                            LastBeforeTaxSalaryTotalAmount = 0,
                            LastTaxTotalAmount = 0,
                            LastSocialDaBingTotalAmount = 0,
                            LastSocialEmpTotalAmount = 0,
                            LastSocialGongJiJinTotalAmount = 0,
                            LastSocialShiYeTotalAmount = 0,
                            LastSocialYangLaoTotalAmount = 0,
                            LastSocialYiLiaoTotalAmount = 0,
                            SocialEmpTotalAmount = 0,
                            SocialGongJiJinTotalAmount = 0,
                            SpBasiclAmount = 0,
                            SpSumAmount = 0,
                            SpTotalAmount = 0,
                            TaxSocialTotalAmount = 0,
                            VerifyInfoOk = true,
                            TaxTotalAmount = 0,
                            ActualSalarySendCardAmount = 0,
                            IsCashSignSuc = false,
                            IsDelayPaymentCancel = false,
                            MonthNetSalaryAmount = 0,
                            PrepaidRepaymentAmount = 0,
                            DelayType = SalaryDelaySendTypeEnumType.未暂缓,
                            SendType = SalaryRecordSendTypeEnumType.其它发放,
                            Status = SalaryRecordSendStatusEnumType.不发放,
                            //SalaryType = SalaryRecordTypeEnumType.补零工资,
                            IsDelayPaymentSuc = false,
                            BankChangeFilePath = string.Empty,
                            Phone = f.Phone,
                            UnitManageAmount = 0,
                            SalaryRecordType = SalaryRecordTypeEnumType.补零工资,
                            IsBatchSend = false,
                            IsBatchPushSuc = false,
                            IsTransBatch = false,
                            PrepaidTotalAmount = 0,
                            BatchTransStatus = string.Empty,
                            BatchTransStatusName = string.Empty,
                            DelayPaymentAmount = 0,
                            MonthWorkDays = 0,
                            SalaryYear = 0,
                            SettlementID = null,
                            TransStatus = string.Empty,
                            TransStatusName = string.Empty,
                            IsCombineSalary = false,
                            IsSinglePushSuc = false,
                            IsSingleSend = false,
                            SalarySlipRecordID = null,
                            TransDescription = string.Empty,
                            UserID = f.UserID,
                            IsFailSalary = false,
                        });
                        matchZeroSalry.Records.AddRange(zeroSalaryRecordList);
                        matchZeroSalry.RecordCount = matchZeroSalry.Records.Count;
                    }
                }
                #endregion
            }

            if (salaryCustomers != null && salaryCustomers.Count > 1)
            {
                Guid workShopBatchNo = Guid.NewGuid();
                foreach (var item in salaryCustomers)
                {
                    item.WorkShopBatchNo = workShopBatchNo;
                }
            }

            await _salaryCustomerService.AddNav(salaryCustomers);




            //Guid KeyID, string CustomerName, int RecordCount, string CalculateStatusName


            var mainSalary = salaryCustomers.FirstOrDefault(f => f.IsMainBatchSalary);

            var mqMsg = new { Type = "salaryCalculateStatus", KeyID = mainSalary.ID, CustomerName = mainSalary.CustomerName, RecordCount = mainSalary.RecordCount, CalculateStatusName = "正在计算" };
            _sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());

            return Ok();
        }
        #endregion

        #region 财资云批量代发推送
        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> PushToTransaction([FromForm]Guid id, [FromForm] string summary, [FromForm] string remark, [FromForm] string purpose)
        //{
        //    var context = HttpContext;
        //    var ip = context.Connection.RemoteIpAddress?.ToString();
        //    var cacheKey = $"PushToTransaction_{id.ToString()}";
        //    var isb = await _redisService.SetAsync(cacheKey, $"{id.ToString()}", TimeSpan.FromSeconds(5), CSRedis.RedisExistence.Nx);
        //    if (!isb)
        //    {
        //        throw new ErrorCodeException(-1, "请求过快,请5秒后再操作");
        //    }


        //    var model = await _salaryCustomerService.GetNav(f => f.ID == id);
        //    if (model == null)
        //    {
        //        throw new ErrorCodeException(-1, "你要读取的数据不存在");
        //    }
        //    if (model.Status != SalaryStatusEnumType.PendingSend)
        //    {
        //        throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");
        //    }
        //    if (model.IsCmbcSend || model.IsPushTrans)
        //    {
        //        throw new ErrorCodeException(-1, message: "检测到已标记为【已推送】记录，无法再次推送");
        //    }
        //    if (!model.IsLocked)
        //    {
        //        throw new ErrorCodeException(-1, "【未锁定】状态无法执行此操作");
        //    }

        //    // 重新获取最新数据
        //    model = await _salaryCustomerService.GetNav(f => f.ID == id);
        //    if (!model.IsCustomerPaid)
        //    {
        //        throw new ErrorCodeException(-1, "客户未打款，无法执行此功能！");
        //    }

        //    // 暂缓发放由民生代发
        //    if (model.Records.Any(f => f.IsDelayPayment == true && f.ActualSalarySendCardAmount > 0))
        //    {
        //        var delayRecords = model.Records.Where(f => f.IsDelayPayment == true && f.ActualSalarySendCardAmount > 0).ToList();
        //        foreach (var item in delayRecords)
        //        {
        //            item.IsSingleSend = true;
        //        }
        //    }
        //    // 批量发放且 非单笔发放
        //    var salaryRecords = model.Records.Where(f => f.IsBatchSend && f.IsSingleSend == false && f.ActualSalarySendCardAmount > 0).ToList();
        //    if (salaryRecords == null || salaryRecords.Count <= 0)
        //    {
        //        throw new ErrorCodeException(-1, "没有需要发放的记录，操作失败");
        //    }


        //    if (salaryRecords.Any(f=>string.IsNullOrWhiteSpace(f.Bank)||string.IsNullOrWhiteSpace(f.BankNo)))
        //    {
        //        throw new ErrorCodeException(-1, "代发记录存在银行卡信息缺失记录，操作失败");
        //    }

        //    foreach (var record in salaryRecords)
        //    {
        //        // 批量代发
        //        record.IsBatchPushSuc = true;
        //        record.BatchTransStatus = string.Empty;
        //        record.BatchTransStatusName = string.Empty;
        //    }
        //    model.IsPushTrans = true;
        //    model.PushTransTime = DateTime.Now;
        //    var existModel = await _salaryTransactionService.Get(f => f.BusinessID == model.ID.ToString() && f.Status != SalaryTransactionStatusEnumType.支付失败 && f.Status != SalaryTransactionStatusEnumType.作废);
        //    if (existModel != null)
        //    {
        //        throw new ErrorCodeException(-1, "存在未完结代发流程或已发放记录，无法再次推送！");
        //    }
        //    #region 生成本地的交易管理记录
        //    Live_SalaryTransactionExtension salaryTransaction = new Live_SalaryTransactionExtension()
        //    {
        //        BusinessID = id.ToString(),
        //        ID = Guid.NewGuid(),
        //        CompanyID = model.CompanyID ?? 0,
        //        CompanyName = string.Empty,
        //        CreateTime = DateTime.Now,
        //        CustomerName = model.CustomerName,
        //        CustomerNo = model.CustomerCompanyNo,
        //        SalaryType = 0,
        //        SignUnitName = model.SigningUnitCompanyName,
        //        SignUnitNo = model.SigningUnitCompanyNo,
        //        Status = SalaryTransactionStatusEnumType.待支付,
        //        Remark = remark,
        //        Summary = summary,
        //        Purpose = purpose,
        //        DataType = TransactionDataTypeEnumType.客户工资,
        //         PayType= TransactionPayTypeEnumType.财资云代发

        //    };
        //    var salaryTransactionRecordList = salaryRecords.Select(f => new Live_SalaryTransactionRecord()
        //    {
        //        Bank = f.Bank,
        //        BankNo = f.BankNo,
        //        CardID = f.IdCardNo,
        //        Name = f.Name,
        //        RecordBusinessID = f.ID.ToString(),
        //        TransMsg = string.Empty,
        //        ID = Guid.NewGuid(),
        //        SalaryTransactionID = salaryTransaction.ID,
        //        TransAmount = f.ActualSalarySendCardAmount,
        //        TransTime = null
        //    }).ToList();
        //    salaryTransaction.Records = salaryTransactionRecordList;
        //    salaryTransaction.TotalAmount = salaryTransactionRecordList.Sum(f => f.TransAmount);
        //    salaryTransaction.TotalCount = salaryTransactionRecordList.Count;
        //    bool insertSuc = await _salaryTransactionService.AddNav(salaryTransaction);
        //    if (!insertSuc)
        //    {
        //        throw new ErrorCodeException(-1, "工资记录发放推送失败！");
        //    }
        //    #endregion
        //    #region 推送财资云等待发放
        //    //BankTransactionApplyDto appDto = new BankTransactionApplyDto() {

        //    //    BusinessID = salaryTransaction.BusinessID,
        //    //    TransactionID = salaryTransaction.ID.ToString(),
        //    //    CustomerName = salaryTransaction.CustomerName,
        //    //    CustomerNo = salaryTransaction.CustomerNo,
        //    //    PayDate = DateTime.Today,
        //    //    Summary = salaryTransaction.Summary??"工资代发",
        //    //    Purpose = salaryTransaction.Purpose,
        //    //    Remark = salaryTransaction.Remark??"无",
        //    //    SignUnitName = salaryTransaction.SignUnitName,
        //    //    SignUnitNo = salaryTransaction.SignUnitNo,
        //    //    TotalAmount = salaryTransaction.TotalAmount,
        //    //    TotalCount = salaryTransaction.TotalCount,
        //    //};
        //    //appDto.Records = salaryTransactionRecordList.Select(f => new BankTransactionRecordApplyDto()
        //    //{
        //    //    Bank = f.Bank,
        //    //    BankNo = f.BankNo,
        //    //    CardID = f.CardID,
        //    //    Name = f.Name,
        //    //    RecordBusinessID = f.RecordBusinessID,
        //    //    RecordTransactionID =f.ID.ToString(),
        //    //    TransAmount = f.TransAmount
                 
        //    //}).ToList();
        //    //string json = JsonConvert.SerializeObject( new List<BankTransactionApplyDto> { appDto });
        //    //var res = await _httpService.PostAsync<string>(ServiceNames.BankService, "/api/BankTransaction/PayrollBatchApply", json);

        //    //if (res != null)
        //    //{
        //    //    if (res.Code == 200)
        //    //    {
        //    //        salaryTransaction.Status = SalaryTransactionStatusEnumType.待发放;
        //    //    }
        //    //    else
        //    //    {
        //    //        salaryTransaction.Status = SalaryTransactionStatusEnumType.推送失败;
        //    //        salaryTransaction.TransMsg = res.Message;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    salaryTransaction.Status = SalaryTransactionStatusEnumType.已推送待确认;
        //    //    salaryTransaction.TransMsg = "未能获取到正确的响应信息！";
        //    //}
        //    //salaryTransaction.PushCount = 1;
        //    //salaryTransaction.PushTime = DateTime.Now;
        //    //await _salaryTransactionService.Update(salaryTransaction);
        //    //if (salaryTransaction.Status != SalaryTransactionStatusEnumType.待发放)
        //    //{
        //    //    throw new ErrorCodeException(-1, salaryTransaction.TransMsg);
        //    //}
        //    #endregion


        //    // 需要更新 企业工资及记录 状态
        //    await _salaryCustomerService.UpdateTransNav(model);

        //    return Ok();
        //}

        /// <summary>
        /// 客户工资推送到支付订单（PaymentOrder）
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PushToPaymentOrder([FromForm] Guid id, [FromForm] string summary, [FromForm] string remark, [FromForm] string purpose)
        {
            var cacheKey = $"PushToPaymentOrder_{id}";
            var isb = await _redisService.SetAsync(cacheKey, id.ToString(), TimeSpan.FromSeconds(5), CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, "请求过快,请5秒后再操作");
            }
            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            var salaryRecords = model.SetPushOrder();
            var addDto = new PaymentOrderAddDto
            {
                CompanyID = CurrentUser.CompanyID,
                SigningUnitName = model.SigningUnitCompanyName,
                CustomerName = model.CustomerName,
                CustomerNo = model.CustomerCompanyNo,
                BusinessID = model.ID.ToString(),
                SalaryType = 0,
                DataType = TransactionDataTypeEnumType.客户工资,
                PayUse = string.IsNullOrWhiteSpace(summary) ? "工资" : summary,
                Purpose = string.IsNullOrWhiteSpace(purpose) ? "工资" : purpose,
                Remark = remark ?? string.Empty,
                PaymentDate = DateTime.Today,
                BillsPayOrderNo = Guid.NewGuid().ToString(),
                Records = salaryRecords.Select(f => new PaymentOrderDetailAddDto
                {
                    BusinessID = f.ID.ToString(),
                    Name = f.Name,
                    CardID = f.IdCardNo,
                    BankName = f.Bank,
                    BankNo = f.BankNo,
                    TransAmount = f.ActualSalarySendCardAmount,
                    BillsPayOrderNo = Guid.NewGuid().ToString(),
                }).ToList(),
            };

            try
            {
                DbScoped.SugarScope.BeginTran();
                var paymentOrder = await _paymentOrderService.Add(addDto);
                var updateSuc = await _salaryCustomerService.UpdateTransNav(model);
                if (!updateSuc)
                {
                    throw new ErrorCodeException(-1, "客户工资记录更新失败");
                }
                DbScoped.SugarScope.CommitTran();
                return Ok(new { paymentOrderID = paymentOrder.ID, paymentOrder.TotalCount, paymentOrder.TotalAmount });
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message, ex);
            }
        }


        /// <summary>
        /// 预先生成支付订单用于前端展示（不落库）
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PrePaymentOrder([FromForm] Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "客户工资记录不能为空");
            }
            if (CurrentUser.CompanyID <= 0)
            {
                throw new ErrorCodeException(-1, "当前用户公司不能为空");
            }

            var model = await _salaryCustomerService.GetTransNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (string.IsNullOrWhiteSpace(model.SigningUnitCompanyName))
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }

            var salaryRecords = model.SetPushOrder();
            var records = salaryRecords.Select(f => new PrePaymentOrderDetailDto
            {
                BusinessID = f.ID.ToString(),
                Name = f.Name,
                CardID = f.IdCardNo,
                BankName = f.Bank,
                BankNo = f.BankNo,
                TransAmount = f.ActualSalarySendCardAmount,
            }).ToList();

            var signUnitName = model.SigningUnitCompanyName.Trim();
            var accounts = await _paymentAccountService.ListView(f =>
                !f.IsDelete
                && f.IsEnable
                && f.UnitName == signUnitName
                && f.CompanyID == CurrentUser.CompanyID);
            var account = accounts?.FirstOrDefault(f => f.IsDefault) ?? accounts?.FirstOrDefault();
            if (account == null)
            {
                throw new ErrorCodeException(-1, $"签约单位【{signUnitName}】没有可用的付款账户");
            }
            if (string.IsNullOrWhiteSpace(account.CompanyName))
            {
                throw new ErrorCodeException(-1, "公司名称不能为空");
            }
            if (string.IsNullOrWhiteSpace(account.UnitNo))
            {
                throw new ErrorCodeException(-1, "签约单位编号不能为空");
            }
            if (account.PaymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联签约单位");
            }
            if (account.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "付款账户未关联支付应用");
            }
            if (string.IsNullOrWhiteSpace(account.AppName) || string.IsNullOrWhiteSpace(account.AppID))
            {
                throw new ErrorCodeException(-1, "付款账户未配置支付应用信息");
            }

            return Ok(new PrePaymentOrderDto
            {
                CompanyName = account.CompanyName,
                CompanyID = CurrentUser.CompanyID,
                SignUnitName = account.UnitName,
                SignUnitNo = account.UnitNo,
                PaymentSignUnitID = account.PaymentSignUnitID,
                CustomerName = model.CustomerName,
                CustomerNo = model.CustomerCompanyNo,
                BusinessID = model.ID.ToString(),
                SalaryType = 0,
                TotalCount = records.Count,
                TotalAmount = records.Sum(f => f.TransAmount),
                Status = (int)PaymentOrderTransStatusEnumType.待支付,
                PayUse = "工资",
                Purpose = "工资",
                Remark = string.Empty,
                DataType = TransactionDataTypeEnumType.客户工资,
                PayType = CzyPayTypeEnumType.批量对私,
                PayChannel = CzyPayChannelEnumType.线下支付,
                WhetherUrgent = false,
                PaymentDate = DateTime.Today,
                PaymentAccountID = account.ID,
                PayAccountNo = account.AccountNo,
                PayOrgName = account.UseOrgName,
                PayOrgCode = account.UseOrgCode ?? string.Empty,
                BusinessTypeName = account.BusinessTypeName,
                AppName = account.AppName,
                AppID = account.AppID,
                PaymentApplicationID = account.PaymentApplicationID,
                Records = records,
            });
        }
        #endregion


    }
}
