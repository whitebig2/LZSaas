using AutoMapper;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Dtos.SettlementDtos;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.IExcelTemplate.Banks;
using LZ.Model.ModelExtensions;
using LZ.Model.Options;
using LZ.Model.PagedModel;
using MathNet.Numerics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NPOI.HSSF.Record;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalaryRecordController : ApiControllerBase
    {
        private readonly AesOption _aesOption;
        private readonly IActionPasswordService   _actionPasswordService;
        private readonly IRedisService  _redisService;
        private readonly IAuthService  _authService;
        private readonly ISettingsService _settingsService;
        private readonly ISalaryFailRecordService _failRecordService;
        private readonly ISalaryDelaySendRecordService _delaySendRecordService;
        private readonly ISettlementService _settlementService;
        private readonly ISalaryCustomerService _salaryCustomerService;
        private readonly IFinanceOtherService _financeOtherService;
        private readonly ISalaryCashSendRecordService _cashSendRecordService;
        private readonly IAliyunOSSService _aliyunOSSService;
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly ISalarySplitSendService _salarySplitSendService;
        private readonly ITransSingleRecordService  _transSingleRecordService;
        private readonly IUserEntryService  _userEntryService;
        private readonly IEventPublisher  _eventPublisher;
        private readonly IMapper _mapper;
        private readonly IPaymentOrderService _paymentOrderService;
        private readonly IPaymentAccountService _paymentAccountService;

        public SalaryRecordController(IOptions<AesOption> options, ISalaryRecordService salaryRecordService, IMapper mapper, IAliyunOSSService aliyunOSSService, ISalaryCashSendRecordService cashSendRecordService, IFinanceOtherService financeOtherService, ISalaryCustomerService salaryCustomerService, ISettlementService settlementService, ISalaryDelaySendRecordService delaySendRecordService, ISalaryFailRecordService failRecordService, ISettingsService settingsService, IAuthService authService, ISalarySplitSendService salarySplitSendService, IRedisService redisService, IActionPasswordService actionPasswordService, ITransSingleRecordService transSingleRecordService, IUserEntryService userEntryService, IEventPublisher eventPublisher, IPaymentOrderService paymentOrderService, IPaymentAccountService paymentAccountService)
        {
            _salaryRecordService = salaryRecordService;
            _mapper = mapper;
            _aliyunOSSService = aliyunOSSService;
            _cashSendRecordService = cashSendRecordService;
            _financeOtherService = financeOtherService;
            _salaryCustomerService = salaryCustomerService;
            _settlementService = settlementService;
            _delaySendRecordService = delaySendRecordService;
            _failRecordService = failRecordService;
            _settingsService = settingsService;
            _authService = authService;
            _salarySplitSendService = salarySplitSendService;
            _redisService = redisService;
            _actionPasswordService = actionPasswordService;
            _aesOption = options.Value;
            _transSingleRecordService = transSingleRecordService;
            _userEntryService = userEntryService;
            _eventPublisher = eventPublisher;
            _paymentOrderService = paymentOrderService;
            _paymentAccountService = paymentAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string signingUnitName,string customerName, string name,string idCardNo, SalaryRecordSendStatusEnumType? status , Guid? salaryCustomerID,DateTime? salaryMonth, DateTime? taxMonth, bool? isFinance, bool? isDelayPayment, bool? isCashSend, bool? isFailSalary,DateTime? sendTime, string filter, [FromQuery] PagedModelBase query)
        {
            Console.WriteLine(Guid.Empty);
            var user = CurrentUser;
            SYS_CompanyUser info = null;
            var exp = Expressionable.Create<Model.Live_V_SalaryRecord>();
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
            else if(user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {

                if (info == null)
                {
                    info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                }

                if (info != null && info.UserType == 4 && !string.IsNullOrWhiteSpace(info.RoleIDS) && info.RoleIDS.Contains("3AD4CA6E-AA01-4824-A223-8B671C41F557".ToLower()))
                {
                    exp.And(it => it.CompanyID == user.CompanyID || it.CustomerName == "宁波君禾清洁科技有限公司");
                }
                else
                {
                    exp.And(it => it.CompanyID == user.CompanyID);
                }
            }
            exp.And(it => it.SalaryRecordType == SalaryRecordTypeEnumType.常规工资);
            exp.AndIF(salaryCustomerID.HasValue, it => it.SalaryCustomerID == salaryCustomerID.Value);
            exp.AndIF(isFinance.HasValue && isFinance.Value, it => it.IsLocked);
            exp.AndIF(isFinance.HasValue && isFinance.Value, it => (it.Status == SalaryRecordSendStatusEnumType.待发放 && it.SalaryCompanyStatus == (int)SalaryStatusEnumType.SucessSend && it.IsSingleSend) || it.Status == SalaryRecordSendStatusEnumType.代发推送异常 || it.Status == SalaryRecordSendStatusEnumType.已发放 || it.Status == SalaryRecordSendStatusEnumType.发放异常);
            exp.AndIF(!string.IsNullOrWhiteSpace(signingUnitName), it => it.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), it => it.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), it => it.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), it => it.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, it => it.Status== status.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(query.Keyword), it => it.CustomerName.Contains(query.Keyword));
            exp.AndIF(!string.IsNullOrWhiteSpace(filter), it => it.Name.Contains(filter) || it.Phone.Contains(filter));
            exp.AndIF(salaryMonth.HasValue, it => it.SalaryMonth == salaryMonth.Value);

            exp.AndIF(isDelayPayment.HasValue, it => it.IsDelayPayment == isDelayPayment.Value);
            exp.AndIF(isCashSend.HasValue, it => it.IsCashSend == isCashSend.Value);
            exp.AndIF(isFailSalary.HasValue, it => it.IsFailSalary == isFailSalary.Value);

            exp.AndIF(sendTime.HasValue, it => it.SendTime!=null&&it.SendTime.Value==sendTime.Value );
            exp.AndIF(taxMonth.HasValue, it => it.TaxMonth!=null&&it.TaxMonth.Value==taxMonth.Value );

            var models = await _salaryRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string signingUnitName, string customerName, string name, string idCardNo, SalaryRecordSendStatusEnumType? status, Guid? salaryCustomerID, DateTime? salaryMonth, bool? isFinance, bool? isDelayPayment, DateTime? taxMonth, bool? isCashSend, bool? isFailSalary,DateTime? sendTime, string filter)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_SalaryRecord>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin && user.CompanyType != CompanyType.Party.ToString().ToLower())
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
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
            exp.And(f => f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资);
            exp.AndIF(salaryCustomerID.HasValue, f => f.SalaryCustomerID == salaryCustomerID.Value);
            //exp.AndIF(isFinance.HasValue && isFinance.Value, f => f.Status == SalaryRecordSendStatusEnumType.待发放 || f.Status == SalaryRecordSendStatusEnumType.发放中 || f.Status == SalaryRecordSendStatusEnumType.代发推送异常 || f.Status == SalaryRecordSendStatusEnumType.已发放 || f.Status == SalaryRecordSendStatusEnumType.发放异常);
            //exp.AndIF(isFinance.HasValue && isFinance.Value, f => f.SalaryCompanyStatus== (int)SalaryStatusEnumType.SucessSend);
            exp.AndIF(isFinance.HasValue && isFinance.Value, it => it.IsLocked);
            exp.AndIF(isFinance.HasValue && isFinance.Value, f => (f.Status == SalaryRecordSendStatusEnumType.待发放 && f.SalaryCompanyStatus == (int)SalaryStatusEnumType.SucessSend) || f.Status == SalaryRecordSendStatusEnumType.发放中 || f.Status == SalaryRecordSendStatusEnumType.代发推送异常 || f.Status == SalaryRecordSendStatusEnumType.已发放 || f.Status == SalaryRecordSendStatusEnumType.发放异常);
            exp.AndIF(!string.IsNullOrWhiteSpace(signingUnitName), f => f.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(filter), f => f.Name.Contains(filter) || f.Phone.Contains(filter));
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);

            exp.AndIF(isDelayPayment.HasValue, it => it.IsDelayPayment == isDelayPayment.Value);
            exp.AndIF(isCashSend.HasValue, it => it.IsCashSend == isCashSend.Value);
            exp.AndIF(isFailSalary.HasValue, it => it.IsFailSalary == isFailSalary.Value);
            exp.AndIF(sendTime.HasValue, it => it.SendTime != null && it.SendTime.Value == sendTime.Value);
            exp.AndIF(taxMonth.HasValue, it => it.TaxMonth != null && it.TaxMonth.Value == taxMonth.Value);
            var dataList = await _salaryRecordService.ListView(exp.ToExpression());
            //var exportDataList = dataList.Select(f => new SalaryRecordExcelDto()
            //{
            //    IdCardNo = f.IdCardNo,
            //    Bank = f.Bank.ToString(),
            //    BankNo = f.BankNo.ToString(),
            //    BankUserName =  f.BankUserName,
            //    CustomerName = f.CustomerName,
            //    Name = f.Name,
            //    CreateTime = f.CreateTime.Value.ToString("yyyy-MM-dd"),
            //    SendTime = f.CreateTime.HasValue? f.CreateTime.Value.ToString("yyyy-MM-dd"):string.Empty,
            //    StatusName = f.Status.ToString(),
            //    IsCash = f.IsCashSend ? "是" : "否",
            //    IsDelayPayment = f.IsDelayPayment ? "是" : "否",
            //    Remark = f.Remark,
            //    SigningUnitName = f.SigningUnitCompanyName,
            //    TotalAmount = f.ActualSalarySendCardAmount.ToString(),
            //}).ToList();
            var exportDataList = dataList.Select(f => new SalaryRecordExcelDto()
            {
                IdCardNo = f.IdCardNo,
                Bank = f.Bank,
                BankNo = f.BankNo,
                BankUserName = f.BankUserName,
                CustomerName = f.CustomerName,
                Name = f.Name,
                BillMonth = f.SalaryMonth.HasValue?f.SalaryMonth.Value.ToString("yyyy-MM-dd"):"",
                CreateTime = f.CreateTime.HasValue?f.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"):"",
                SendTime = f.SendTime.HasValue ? f.SendTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                StatusName = f.Status.ToString(),
                IsCash = f.IsCashSend ? "是" : "否",
                IsDelayPayment = f.IsDelayPayment ? "是" : "否",
                IsFailSalary = f.IsFailSalary ? "是" : "否",
                Remark = f.Remark,
                SigningUnitName = f.SigningUnitCompanyName,
                UnitSalaryAmount = f.UnitSalaryAmount,
                TotalAmount = f.ActualSalarySendCardAmount,
                 IsSplitSend=f.IsSplitSend?"是":"否",
                  SplitSendSucAmount=f.SplitSendSucAmount,
                ChangeAmount =f.ChangeAmount??0,
                ChangeRemark=f.ChangeRemark

            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "工资记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "工资记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        [HttpGet]
        public async Task<IActionResult> ExportQueryBankSheetList(string exportType,string signingUnitName, string customerName, string name, string idCardNo, SalaryRecordSendStatusEnumType? status, Guid? salaryCustomerID, DateTime? salaryMonth, DateTime? taxMonth, bool? isFinance, bool? isDelayPayment, bool? isCashSend, bool? isFailSalary, string filter)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_SalaryRecord>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin && user.CompanyType != CompanyType.Party.ToString().ToLower())
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
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
            exp.And(f => f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资);
            exp.AndIF(salaryCustomerID.HasValue, f => f.SalaryCustomerID == salaryCustomerID.Value);
            //exp.AndIF(isFinance.HasValue && isFinance.Value, f => f.Status == SalaryRecordSendStatusEnumType.待发放 || f.Status == SalaryRecordSendStatusEnumType.发放中 || f.Status == SalaryRecordSendStatusEnumType.代发推送异常 || f.Status == SalaryRecordSendStatusEnumType.已发放 || f.Status == SalaryRecordSendStatusEnumType.发放异常);
            //exp.AndIF(isFinance.HasValue && isFinance.Value, f => f.SalaryCompanyStatus== (int)SalaryStatusEnumType.SucessSend);
            exp.AndIF(isFinance.HasValue && isFinance.Value, it => it.IsLocked);
            exp.AndIF(isFinance.HasValue && isFinance.Value, f => (f.Status == SalaryRecordSendStatusEnumType.待发放 && f.SalaryCompanyStatus == (int)SalaryStatusEnumType.SucessSend) || f.Status == SalaryRecordSendStatusEnumType.发放中 || f.Status == SalaryRecordSendStatusEnumType.代发推送异常 || f.Status == SalaryRecordSendStatusEnumType.已发放 || f.Status == SalaryRecordSendStatusEnumType.发放异常);
            exp.AndIF(!string.IsNullOrWhiteSpace(signingUnitName), f => f.SigningUnitCompanyName.Contains(signingUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(filter), f => f.Name.Contains(filter) || f.Phone.Contains(filter));
            exp.AndIF(salaryMonth.HasValue, f => f.SalaryMonth == salaryMonth.Value);
            exp.AndIF(taxMonth.HasValue, it => it.TaxMonth != null && it.TaxMonth.Value == taxMonth.Value);

            exp.AndIF(isDelayPayment.HasValue, it => it.IsDelayPayment == isDelayPayment.Value);
            exp.AndIF(isCashSend.HasValue, it => it.IsCashSend == isCashSend.Value);
            exp.AndIF(isFailSalary.HasValue, it => it.IsFailSalary == isFailSalary.Value);
            var recordList = await _salaryRecordService.ListView(exp.ToExpression());


            if (exportType.Contains("民生"))
            {
                var excelData = recordList.Select(f => new MinShengYinHangExcelTemplate()
                {
                    BankNameNo = string.Empty,
                    BankNo = f.BankNo,
                    BankUserName = f.BankUserName,
                    Remark = f.CustomerName,
                    Summary = $"{f.SalaryMonth.Value.ToString("yyyy-MM")}月工资",
                    TransactionAmount = f.ActualSalarySendCardAmount
                }).ToList();
                var tempBase64 = await _aliyunOSSService.DownloadFile(NpoiExcelHelper.MinShengTempUri);
                var fileByteArray = NpoiExcelHelper.WriteMinShengYinHang(excelData, tempBase64);
                string base64String = Convert.ToBase64String(fileByteArray);
                return Ok(new { base64String, fileName = "民生银行-模板.xls", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            }
            else if (exportType.Contains("建设"))
            {


                var excelData = recordList.Select(f => new JianSheYinHangExcelTemplate()
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
                return Ok(new { base64String, fileName = "建设银行-模板.xls", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

            }
            else if (exportType.Contains("招商"))
            {
                var excelData = recordList.Select(f => new ZhaoShangYinHangExcelTemplate()
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
            else if (exportType.Contains("邮政"))
            {


                var excelData = recordList.Select(f => new YouZhengYinHangExcelTemplate()
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
            else
            {

                throw new ErrorCodeException(-1, $"未知的银行制单类型：{exportType}");

            }

 

        }



     

        //[HttpPost]
        //public async Task<IActionResult> Add(SettlementManageDto modeldto)
        //{
        //    var model = _mapper.Map<Live_Settlement>(modeldto);
        //    model.ID = Guid.NewGuid();
        //    model.ApplyDate = DateTime.Today;
        //    model.CreateTime = DateTime.Now;
        //    await _settlementService.Add(model);
        //    return Ok();
        //}
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _salaryRecordService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _salaryRecordService.GetView(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }



        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_SalaryRecord model)
        {
            var existModel = await _salaryRecordService.Get(f => f.ID == model.ID);
            if (existModel == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            var salayCustomer = await _salaryCustomerService.Get(existModel.SalaryCustomerID);

            if (salayCustomer == null)
            {

                throw new ErrorCodeException(-1, "客户工资已被删除，请联系管理员");
            }


            //if (salayCustomer)
            //{

            //}

            //if (salayCustomer.Status == SalaryStatusEnumType.SucessSend || salayCustomer.Status == SalaryStatusEnumType.PendingSend)
            //{
            //    if (salayCustomer.IsCustomerPaid)
            //    {
            //        throw new ErrorCodeException(-1, "客户工资待发放、已发放状态且客户已打款无法执行此操作");
            //    }
            //}


            if (existModel.IsLocked)
            {
                throw new ErrorCodeException(-1, "此工资记录已被锁定无法执行任何修改操作");
            }

            if (model.Status== SalaryRecordSendStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "无法修改已发放记录的信息");
            }

           



            if (model.ChangeAmount.HasValue)
            {
                model.ActualSalarySendCardAmount = model.ActualSalaryAmount - model.PrepaidRepaymentAmount + model.ChangeAmount.Value;
                if (string.IsNullOrWhiteSpace(model.ChangeHash))
                {
                    if (model.ChangeAmount.Value > 0 && string.IsNullOrWhiteSpace(model.ChangeRemark))
                    {
                        throw new ErrorCodeException(-1, "客服扣/补金额金额已发生变化，请填写调整备注");
                    }

                }
                else {
                    var hash = $"{model.ChangeAmount}_{model.ActualSalarySendCardAmount}_{model.ChangeRemark}".GetHashCode().ToString();
                    if (model.ChangeHash== hash)
                    {
                        throw new ErrorCodeException(-1, "客服扣/补金额金额已发生变化，请更改调整备注");
                    }
                }
             
            }

            if (model.Status== SalaryRecordSendStatusEnumType.已发放 && !model.SendTime.HasValue)
            {
                model.SendTime = DateTime.Now;
            }
            if (model.BankUserName.Equals(existModel.BankUserName))
            {

                _salaryRecordService.VerifyPendingSend(model);
                await _salaryRecordService.Update(model);
            }
            else {
                if (string.IsNullOrWhiteSpace(model.BankChangeFilePath))
                {
                    throw new ErrorCodeException(-1, "修改银行卡归属人需要上传凭证");
                }
                else {

                    _salaryRecordService.VerifyPendingSend(model);
                    await _salaryRecordService.Update(model);
                }
            }
        

            return Ok();
        }

        /// <summary>
        ///  现金发放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> UpdateSendType([FromForm] Guid id)
        {

            var model = await _salaryRecordService.Get(f => f.ID == id);

            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.IsLocked)
            {
                throw new ErrorCodeException(-1, "此工资记录已被锁定无法执行任何修改操作");
            }

            if (model.IsCashSend)
            {
                throw new ErrorCodeException(-1, "非线上发放记录，无法执行此操作");
            }

            if (model.Status== SalaryRecordSendStatusEnumType.已发放)
            {
                throw new ErrorCodeException(-1, "已发放工资无法执行此操作");
            }
            var customerResp = await _financeOtherService.GetCustomerByNameAsync(model.CustomerName);

            Live_V_Customer customer = null;
            if (customerResp!=null&& customerResp.Code==200)
            {
                customer = customerResp.Data;
            }

            if (customer==null)
            {
                throw new ErrorCodeException(-1, $"{model.Name}客户单位信息获取失败");
            }

            if (string.IsNullOrWhiteSpace(customer.AgentName)|| string.IsNullOrWhiteSpace(customer.AgentBank)|| string.IsNullOrWhiteSpace(customer.AgentBankNo))
            {
                throw new ErrorCodeException(-1, $"{model.Name}客户单位代理人信息缺失");
            }

            Live_SalaryCashSendRecord cashSendRecord = new Live_SalaryCashSendRecord()
            {
                CreateTime = DateTime.Now,
                Reason = "客服生成",
                Remark = "",
                FilePath = string.Empty,
                AgentBank = customer.AgentBank,
                AgentBankNo = customer.AgentBankNo,
                AgentIdCardNo = customer.AgentIdCardNo,
                AgentName = customer.AgentName,
                SalaryCustomerID = model.SalaryCustomerID,
                SalaryRecordID = model.ID,
                SendAmount = 0,
                SignerOssUrl = string.Empty,
                SignKeyword = string.Empty,
                SignTime = null,
                Status = CashSendStatusEnumType.待生成签收单,
                SucFilePath = string.Empty,
                TotalAmount = model.ActualSalarySendCardAmount,
                ID = Guid.NewGuid()
            };
            model.Bank=customer.AgentBank;
            model.BankNo=customer.AgentBankNo;
            model.BankUserName=customer.AgentName;

            try
            {
                DbScoped.SugarScope.BeginTran();
                await _cashSendRecordService.Add(cashSendRecord);
                model.Status = SalaryRecordSendStatusEnumType.待处理;

                if (model.IsBatchPushSuc==false)
                {
                    model.IsBatchSend = false;
                }
                model.IsCashSend = true;
                model.IsSingleSend = true;
                model.IsCashSignSuc = false;
                model.IsSinglePushSuc = false;
                model.SendType = SalaryRecordSendTypeEnumType.线下发放;
                await _salaryRecordService.Update(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1,$"数据保存失败，异常信息：{ex.Message}");
            }
            return Ok();
        }



   

        [HttpPut]
        public async Task<IActionResult> UpdateDelayType([FromForm] Guid id)
        {

            var model = await _salaryRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }


            var salaryCustomer = await _salaryCustomerService.Get(model.SalaryCustomerID);
            if (salaryCustomer == null)
            {
                throw new ErrorCodeException(-1, "客户工资已删除，请联系管理员");
            }

         

            if (model.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放记录，无法暂缓发放");
            }

            if (model.DelayType != SalaryDelaySendTypeEnumType.未暂缓)
            {
                throw new ErrorCodeException(-1, "已暂缓的记录无需重复操作！");
            }

            //if (model.Status != SalaryRecordSendStatusEnumType.待处理 && model.Status != SalaryRecordSendStatusEnumType.处理中)
            //{
            //    if (salaryCustomer.Status == SalaryStatusEnumType.SucessSend || salaryCustomer.Status == SalaryStatusEnumType.PendingSend)
            //    {
            //        if (salaryCustomer.IsCustomerPaid)
            //        {
            //            throw new ErrorCodeException(-1, "客户工资待发放、已发放状态且客户已打款无法执行此操作");
            //        }
            //    }
            //}

            if (model.Status == SalaryRecordSendStatusEnumType.已发放||model.Status == SalaryRecordSendStatusEnumType.发放异常)
            {
                throw new ErrorCodeException(-1, $"【{model.Status.ToString()}】状态无法执行此操作");
            }

            model.DelayType = SalaryDelaySendTypeEnumType.客服暂缓;
            model.DelayPaymentAmount = model.ActualSalarySendCardAmount;
            //model.ActualSalarySendCardAmount = 0;
            model.IsDelayPayment = true;
            model.IsSingleSend = true;
            model.IsSinglePushSuc = false;
            



            Live_SalaryDelaySendRecord delaySendRecord = new Live_SalaryDelaySendRecord()
            {
                CreateTime = DateTime.Now,
                DelayType = SalaryDelaySendTypeEnumType.客服暂缓,
                SalaryRecordID = model.ID,
                Remark = "点击生成",
                FilePath = string.Empty,
                SalaryCustomerID = model.SalaryCustomerID,
                SendAmount = model.DelayPaymentAmount,
                Status = SalaryDelaySendStatusEnumType.待客服处理,
                TotalAmount = model.DelayPaymentAmount,
            };
            DbScoped.SugarScope.BeginTran();
            await _delaySendRecordService.Add(delaySendRecord);
            model.IsDelayPaymentSuc = false;
            model.Status = SalaryRecordSendStatusEnumType.待处理;
            model.IsBatchSend = false;
            await _salaryRecordService.Update(model);
            DbScoped.SugarScope.CommitTran();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBankChangeFile(IFormFile file)
        {

            var  setting=await _settingsService.GetMasterSettings();
            string ossUrl = string.Empty;
            List<SalaryRecordExcelTemplate> salaryRecordExcelTemplates = new List<SalaryRecordExcelTemplate>();
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var ossResp = await _aliyunOSSService.UploadFile(ms, file.FileName, true, false, "SalaryRecord");
                if (ossResp.Code == 0)
                {
                    ossUrl = setting.AliBucketDomain + ossResp.Data;
                }
                else
                {
                    throw new ErrorCodeException(-1, "文件上传失败");
                }
            }
            return Ok(ossUrl);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm] Guid id, [FromForm] SalaryRecordSendStatusEnumType status)
        {
            var record = await _salaryRecordService.Get(f => f.ID == id);

          

            record.Status = status;
            if (status== SalaryRecordSendStatusEnumType.待发放|| status == SalaryRecordSendStatusEnumType.已发放)
            {
                if (record.SendType == SalaryRecordSendTypeEnumType.线下发放)
                {
                    if (!record.IsCashSignSuc)
                    {
                        throw new ErrorCodeException(-1, "现金发放记录校验失败！");
                    }
                }
                if (record.IsDelayPayment)
                {
                    if (!record.IsDelayPaymentSuc)
                    {
                        throw new ErrorCodeException(-1, "暂缓记录校验失败！");
                    }
                }
            }


            DbScoped.SugarScope.BeginTran();
            if (status == SalaryRecordSendStatusEnumType.已发放)
            {
                var model = await _salaryCustomerService.Get(record.SalaryCustomerID);
                var settlement = new Live_Settlement()
                {
                    ID = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    CustomerGID = model.CustomerGID,
                    Remark = string.Empty,
                    SalaryCustomerID = Guid.Empty,
                    SalaryMonth = model.SalaryMonth,
                    SettlementType = 0,
                    Title = "派遣员工工资",
                    TotalAmount =
                      model.UnitSalarySumAmount  //企业工资
                      + model.SocialUnitSumAmount // 企业社保
                      + model.InnerServiceFeeAmount // 内部服务费
                      - model.CompanyDeductionAmount,// 企业扣款

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
                settlement.Description = $"{settlement.CustomerName}-{settlement.SalaryMonth.Value.ToString("yyyy年MM月")}，{record.Name}，共1名员工工资,合计{record.ActualSalarySendCardAmount}元";
                var isb = await _settlementService.Add(settlement);
                if (isb)
                {
                    record.SettlementID = settlement.ID;
                }
                record.SendTime = DateTime.Now;
                if (record.IsCashSend && record.IsCashSignSuc)
                {
                    var cashRecord = await _cashSendRecordService.Get(f => f.SalaryRecordID == record.ID && f.AuditStatus ==  SalaryCashAuditStatusEnumType.审核通过 && f.Status == CashSendStatusEnumType.已签署);
                    if (cashRecord!=null)
                    {
                        cashRecord.AgentStatus = SalaryCashAgentStatusEnumType.待转账;
                        await _cashSendRecordService.Update(cashRecord);
                    }
                   
                }
            }
            else {
                // 删除结算单

                if (record.SettlementID!=null && record.SettlementID!=Guid.Empty)
                {
                  await  _settlementService.Delete(record.SettlementID.Value);
                }

                record.SettlementID = null;
            }
            await _salaryRecordService.Update(record);
            DbScoped.SugarScope.CommitTran();
            return Ok();
        }

        /// <summary>
        /// 发放完成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> SetSendSuc([FromForm] Guid id)
        {
            var model = await _salaryRecordService.Get(f => f.ID == id);

            if (model.Status != SalaryRecordSendStatusEnumType.待发放)
            {
                throw new ErrorCodeException(-1, "此状态无法执行此操作！");
            }
            if (model.IsCashSend)
            {
                if (!model.IsCashSignSuc)
                {
                    throw new ErrorCodeException(-1, "现金发放未处理完成");
                }
            }
            if (model.IsDelayPayment)
            {
                if (!model.IsDelayPaymentSuc)
                {
                    throw new ErrorCodeException(-1, "暂缓发放未处理完成");
                }
            }
         






           
            await _salaryRecordService.SetSendSuccess(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> SetSendFail([FromForm]Guid id, [FromForm] SalaryFailTypeEnumType failType, [FromForm] string description)
        {
            var model = await _salaryRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放无法标记发放异常");
            }

            await _salaryRecordService.SetSendFail(model, failType, description);
            return Ok();
        }

        /// <summary>
        ///  设置待发放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetSendPending([FromForm] Guid id)
        {
            var model = await _salaryRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "记录不存在");
            }

            if (model.Status!= SalaryRecordSendStatusEnumType.代发推送异常)
            {
                throw new ErrorCodeException(-1, "此状态无法执行此操作");
            }

            model.Status = SalaryRecordSendStatusEnumType.待发放;
            await _salaryRecordService.Update(model);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> DownloadCashPdf([FromForm] Guid id)
        {
            var model = await _cashSendRecordService.Get(f => f.Status == CashSendStatusEnumType.已签署 && f.SalaryRecordID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "现金签发放记录不存在");
            }

            if (string.IsNullOrWhiteSpace(model.SucFilePath))
            {
                throw new ErrorCodeException(-1, "签署文件不存在！");
            }
            var base64String = await _aliyunOSSService.DownloadFile(model.SucFilePath);
            return Ok(new { base64String, fileName = $"现金签收单.pdf", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

        }

        [HttpPost]
        public async Task<IActionResult> SplitSend([FromForm] Guid id)
        {
            var model = await _salaryRecordService.Get(f => f.ID == id);
            if (model.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放无法分批发放！");
            }
            model.IsSplitSend = true;
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> SetUnLocked([FromForm] Guid id, [FromForm] bool isFinance)
        {
            var model = await _salaryRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (!isFinance)
            {
                var salaryCustomerModel = await _salaryCustomerService.Get(model.SalaryCustomerID);
                if (salaryCustomerModel != null)
                {
                    if (salaryCustomerModel.Status==  SalaryStatusEnumType.SucessSend)
                    {
                        throw new ErrorCodeException(-1, "客户工资状态为【已发放】,请让财务人员解除锁定");
                    }
                }
            }

            if (!model.IsLocked)
            {
                throw new ErrorCodeException(-1, "未锁定的记录无法执行此操作");
            }
            model.IsLocked = false;
            model.IsAutoLocked = false;
            model.LockedTime = null;
            await _salaryRecordService.Update(model);

            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> SetLocked([FromForm] Guid id)
        {
            var model = await _salaryRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            //if (model.IsAutoLocked)
            //{
            //    throw new ErrorCodeException(-1, "自动锁定记录无法手动操作");
            //}

            _salaryRecordService.VerifyPendingSend(model);

            if (model.Status != SalaryRecordSendStatusEnumType.待发放)
            {
                throw new ErrorCodeException(-1, "不符合锁定条件，暂缓、异常、现金等未处理完成！");
            }

            if (!model.IsLocked)
            {
                model.IsLocked = true;
                model.LockedTime= DateTime.Now;
                model.IsAutoLocked = true;
            }
            await _salaryRecordService.Update(model);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> SalaryTransSingle([FromForm] Guid id, [FromForm] string pwd)
        {


            List<string> signUnitNameList = new List<string>() {
                "宁波莱博友翌供应链管理有限公司",
                "宁波新莱博友盟供应链管理有限公司",
                "宁波新莱博金浩供应链管理有限公司",
                "宁波蓝鑫人力资源有限公司",
                "宁波莱鑫人力资源有限公司"
            };
            //model.IsPushTrans = true;
            var context = HttpContext;
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"{ip}_count";
            var isb = await _redisService.SetAsync(cacheKey, "11", TimeSpan.FromSeconds(2), CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, "请求过快,请2秒后再操作");
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
           var record=await _salaryRecordService.Get(f => f.ID == id);
       
            if (record == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            //if (record.CmbcSendStatus> CmbcSendStatusEnumType.待推送)
            //{
            //    throw new ErrorCodeException(-1, $"民生代发状态：【{record.CmbcSendStatus.ToString()}】无法执行此操作");
            //}

            if (record.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放无法执行此操作");
            }

            var model = await _salaryCustomerService.Get(record.SalaryCustomerID);

            if (model.Status != SalaryStatusEnumType.SucessSend)
            {
                throw new ErrorCodeException(-1, message: "此批次工资批量工资未发放完成！");
            }


            if (!signUnitNameList.Contains(model.SigningUnitCompanyName))
            {
                throw new ErrorCodeException(-1, $"【{model.SigningUnitCompanyName}】未开启暂缓异常代发账户");
            }



            if (record.Status!= SalaryRecordSendStatusEnumType.待发放)
            {
                throw new ErrorCodeException(-1, "非【待发放】记录无法执行此操作");
            }

            if (!(record.IsDelayPayment || record.IsFailSalary))
            {
                throw new ErrorCodeException(-1, "非暂缓或异常工资无法执行此操作");
            }

            if (!record.IsSingleSend)
            {
                throw new ErrorCodeException(-1, "暂缓或异常工资未标记为【单笔发放】");
            }


            if (string.IsNullOrWhiteSpace(record.Bank) || string.IsNullOrWhiteSpace(record.BankNo) || string.IsNullOrWhiteSpace(record.BankUserName))
            {
                throw new ErrorCodeException(-1, "银行卡信息缺失");
            }


          
            var transRecord=await   _transSingleRecordService.Get(f => f.BusinessID == record.ID);
            if (transRecord != null)
            {
                throw new ErrorCodeException(-1, "此工资记录已代发过，无法重复代发！");
            }



            Live_TransSingleRecord transSingleRecord = new Live_TransSingleRecord()
            {
                Bank = record.Bank,
                BankNo = record.BankNo,
                BankUserName = record.BankUserName,
                BusinessID = record.ID,
                CustomerName = record.CustomerName,
                BusinessType = CmbcTransBusinessTypeEnumType.企业工资,
                ConfirmTimes = 0,
                CreateTime = DateTime.Now,
                IdCardNo = record.IdCardNo,
                IsPrepaid = false,
                Name = record.Name,
                Nonce = record.ID,
                Remark = string.Empty,
                SigningUnitName = model.SigningUnitCompanyName,
                Status = CmbcStatusEnumType.待推送,
                TransAmount = record.ActualSalarySendCardAmount,
                TransStatus = string.Empty,
                TransStatusName = string.Empty,
                TransTime = null,
                RecordID=record.ID.ToString()
            };

          
      
            //model.IsCmbcSend = true;
            #region 推送记录保存到数据库
            try
            {
                DbScoped.SugarScope.BeginTran();

                record.IsSinglePushSuc = true;
                await _transSingleRecordService.Add(transSingleRecord);
                record.CmbcSendStatus = CmbcSendStatusEnumType.推送中;
                record.TransDescription = string.Empty;
                record.TransMsg = string.Empty;

                await _salaryRecordService.Update(record);

                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, $"代发异常：{ex.Message}！");
            }
            #endregion
            return Ok();
        }



        [HttpPut]
        public async Task<IActionResult> UpdateUserEntryBank([FromForm] Guid id)
        {
            var user = CurrentUser;
            var record = await _failRecordService.GetView(f => f.ID == id);

            if (record.UserEntryID == null || record.UserEntryID.Value <= 0)
            {
                throw new ErrorCodeException(-1, "此工资记录未关联入离职记录，无法使用此功能！");
            }

            if (string.IsNullOrWhiteSpace(record.SalaryBank) || string.IsNullOrWhiteSpace(record.SalaryBank))
            {
                throw new ErrorCodeException(-1, "关联的工资记录 银行卡信息缺失");
            }

            if (record.SalaryBankUserName != record.Name)
            {
                throw new ErrorCodeException(-1, "银行卡所属人与员姓名不一致，无法使用此功能！");
            }


            var model = await _userEntryService.Get(f => f.ID == record.UserEntryID.Value);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "未匹配到关联的入职记录信息吗，无法使用此功能！");
            }

            if (model.BankName==record.Bank&&model.BankNumber==record.BankNo)
            {
                throw new ErrorCodeException(-1, "员工信息银行卡信息与当前工资记录信息一致，无法执行此功能！");
            }


            string oldBankName = model.BankName;
            string oldBankNo = model.BankNumber;
            model.BankName = record.SalaryBank;
            model.BankNumber = record.SalaryBankNo;
            await _userEntryService.UpdateBank(model);

            var entryRecord = new EntryRecordEventData();
            entryRecord.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
            entryRecord.EntryID = model.ID;
            entryRecord.PerUserID = user.UserID;
            entryRecord.Status = model.Status.Value;
            entryRecord.About = "工资记录更改银行卡操作！银行卡（" + oldBankNo + "）修改为(" + model.BankNumber + ")，所属行（" + oldBankName + "）修改为(" + model.BankNumber + ")";
            entryRecord.CID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
            entryRecord.OperateFrom = user.PlatformType;
            entryRecord.OperateFullName = user.FullName;
            entryRecord.OperateUserID= user.UserID;
            _eventPublisher.Publish(entryRecord);


            return Ok();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SalaryRecordCoverage(IFormFile file)
        {
            // 当月月初
            List<SalaryRecordExportExcelTemplate> excelRecords = new List<SalaryRecordExportExcelTemplate>();
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                excelRecords = NpoiExcelHelper.ReadExcelFile<SalaryRecordExportExcelTemplate>(ms, "");
            }
            foreach (var item in excelRecords)
            {
                int batch = 0;
                for (int i = 0; i < 100; i++)
                {
                    if (item.SalaryBatchType.Contains(i.ToString()))
                    {
                        batch = i;
                        break;
                    }
                }
                var salaryMonth = DateTime.Parse(item.SalaryMonth);

                var record = await _salaryRecordService.GetView(f => f.SalaryBatchType == batch && f.CustomerName == item.CustomerName && f.SalaryMonth == salaryMonth && f.IdCardNo == item.IdCardNo && f.Name == item.Name && f.UnitSalaryAmount == item.UnitSalaryAmount);

                if (record == null)
                {
                    Console.WriteLine($"{item.Name}获取工资信息失败");
                    continue;
                }
                else
                    Console.WriteLine(item.Name);

                var model = await _salaryRecordService.Get(f => f.ID == record.ID);
                model.TaxTimes = item.TaxTimes;
                model.TaxMonthAmount = item.TaxMonthAmount;
                model.BeforeTaxSalaryTotalAmount = item.BeforeTaxSalaryTotalAmount;
                model.SpBasicTotalAmount = item.TaxSpBasicTotalAmount;
                model.TaxSalaryTotalAmount = item.TaxSalaryTotalAmount;
                model.TaxTotalAmount = item.TaxTotalAmount;
                model.LastTaxTotalAmount = item.LastTaxTotalAmount;

                await _salaryRecordService.Update(model);


            }

            return Ok(excelRecords);
        }

        /// <summary>
        ///  工伤查询员工工资
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<decimal> InjurySalaryList(string idCardNo,int customerID, DateTime salaryMonth)
        {
            if (string.IsNullOrEmpty(idCardNo))
            {
                return 0M;
            }
            var exp = Expressionable.Create<Model.Live_V_SalaryRecord>();
            exp.And(it => it.SalaryRecordType == SalaryRecordTypeEnumType.常规工资);
            exp.And(it => it.IdCardNo == idCardNo);
            exp.And(it => it.CustomerID == customerID);
            exp.And(it=>it.Status == SalaryRecordSendStatusEnumType.已发放);
            exp.And(it => it.SalaryMonth.HasValue && it.SalaryMonth >= salaryMonth);

            var avg = await _salaryRecordService.AvgSalary(exp.ToExpression());
            return avg;
        }

        /// <summary>
        /// 暂缓异常工资推送到支付订单
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PushToPaymentOrder([FromForm] Guid id, [FromForm] string summary, [FromForm] string remark, [FromForm] string purpose)
        {
            var cacheKey = $"SalaryRecord_PushToPaymentOrder_{id}";
            var isb = await _redisService.SetAsync(cacheKey, id.ToString(), TimeSpan.FromSeconds(5), CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, "请求过快,请5秒后再操作");
            }

            var record = await _salaryRecordService.Get(f => f.ID == id);
            if (record == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (record.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放无法生成支付订单");
            }
            if (record.IsBatchSend)
            {
                throw new ErrorCodeException(-1, "批量发放无法生成支付订单");
            }
            if (record.ActualSalarySendCardAmount <= 0)
            {
                throw new ErrorCodeException(-1, "实发金额必须大于0");
            }
            if (record.Status != SalaryRecordSendStatusEnumType.待发放)
            {
                throw new ErrorCodeException(-1, "非【待发放】记录无法执行此操作");
            }
            if (record.IsPushToOrder)
            {
                throw new ErrorCodeException(-1, "该记录已推送到支付订单，无法重复推送");
            }
            if (record.IsSendSuccess)
            {
                throw new ErrorCodeException(-1, "该记录已发放成功，无法重复推送");
            }

            if (record.Name!=record.BankUserName)
            {
                throw new ErrorCodeException(-1, "银行卡所属人与员工姓名不一致");
            }
            if (string.IsNullOrWhiteSpace(record.Name)
                || string.IsNullOrWhiteSpace(record.IdCardNo)
                || string.IsNullOrWhiteSpace(record.Bank)
                || string.IsNullOrWhiteSpace(record.BankNo))
            {
                throw new ErrorCodeException(-1, "银行卡或人员信息不完整");
            }

            var recordView = await _salaryRecordService.GetView(f => f.ID == id);
            if (recordView == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (string.IsNullOrWhiteSpace(recordView.SigningUnitCompanyName))
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }

            var businessId = record.ID.ToString();
            var addDto = new PaymentOrderAddDto
            {
                CompanyID = CurrentUser.CompanyID,
                SigningUnitName = recordView.SigningUnitCompanyName,
                CustomerName = recordView.CustomerName,
                CustomerNo = recordView.CustomerCompanyNo,
                BusinessID = businessId,
                SalaryType = 0,
                DataType = TransactionDataTypeEnumType.暂缓异常工资,
                PayUse = string.IsNullOrWhiteSpace(summary) ? "工资" : summary,
                Purpose = string.IsNullOrWhiteSpace(purpose) ? "工资" : purpose,
                Remark = remark ?? string.Empty,
                PaymentDate = DateTime.Today,
                BillsPayOrderNo = Guid.NewGuid().ToString(),
                Records = new List<PaymentOrderDetailAddDto>
                {
                    new PaymentOrderDetailAddDto
                    {
                        BusinessID = businessId,
                        Name = record.Name,
                        CardID = record.IdCardNo,
                        BankName = record.Bank,
                        BankNo = record.BankNo,
                        TransAmount = record.ActualSalarySendCardAmount,
                        BillsPayOrderNo = Guid.NewGuid().ToString(),
                    }
                }
            };

            try
            {
                DbScoped.SugarScope.BeginTran();
                var paymentOrder = await _paymentOrderService.Add(addDto);
                record.IsPushToOrder = true;
                record.IsSingleSend = true;
                await _salaryRecordService.Update(record);
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
                throw new ErrorCodeException(-1, "工资记录不能为空");
            }
            if (CurrentUser.CompanyID <= 0)
            {
                throw new ErrorCodeException(-1, "当前用户公司不能为空");
            }

            var record = await _salaryRecordService.Get(f => f.ID == id);
            if (record == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (record.IsCashSend)
            {
                throw new ErrorCodeException(-1, "现金发放无法生成支付订单");
            }
            if (!record.IsSingleSend)
            {
                throw new ErrorCodeException(-1, "非单笔发放无法生成支付订单");
            }
            if (record.ActualSalarySendCardAmount <= 0)
            {
                throw new ErrorCodeException(-1, "实发金额必须大于0");
            }
            if (record.Status != SalaryRecordSendStatusEnumType.待发放)
            {
                throw new ErrorCodeException(-1, "非【待发放】记录无法执行此操作");
            }
            if (record.IsPushToOrder)
            {
                throw new ErrorCodeException(-1, "该记录已推送到支付订单，无法重复推送");
            }
            if (record.IsSendSuccess)
            {
                throw new ErrorCodeException(-1, "该记录已发放成功，无法重复推送");
            }
            if (record.Name != record.BankUserName)
            {
                throw new ErrorCodeException(-1, "银行卡所属人与员工姓名不一致");
            }
            if (string.IsNullOrWhiteSpace(record.Name)
                || string.IsNullOrWhiteSpace(record.IdCardNo)
                || string.IsNullOrWhiteSpace(record.Bank)
                || string.IsNullOrWhiteSpace(record.BankNo))
            {
                throw new ErrorCodeException(-1, "银行卡或人员信息不完整");
            }

            var recordView = await _salaryRecordService.GetView(f => f.ID == id);
            if (recordView == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (string.IsNullOrWhiteSpace(recordView.SigningUnitCompanyName))
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }

            var records = new List<PrePaymentOrderDetailDto>
            {
                new PrePaymentOrderDetailDto
                {
                    BusinessID = record.ID.ToString(),
                    Name = record.Name,
                    CardID = record.IdCardNo,
                    BankName = record.Bank,
                    BankNo = record.BankNo,
                    TransAmount = record.ActualSalarySendCardAmount,
                }
            };

            var signUnitName = recordView.SigningUnitCompanyName.Trim();
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
                CustomerName = recordView.CustomerName,
                CustomerNo = recordView.CustomerCompanyNo,
                BusinessID = record.ID.ToString(),
                SalaryType = 0,
                TotalCount = records.Count,
                TotalAmount = records.Sum(f => f.TransAmount),
                Status = (int)PaymentOrderTransStatusEnumType.待支付,
                PayUse = "工资",
                Purpose = "工资",
                Remark = string.Empty,
                DataType = TransactionDataTypeEnumType.暂缓异常工资,
                PayType = CzyPayTypeEnumType.普通付款,
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


    }
}
