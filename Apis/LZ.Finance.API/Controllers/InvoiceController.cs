using AutoMapper;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Clients.API;
using LZ.Common;
using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Dtos.InvoiceDto;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using SqlSugar;
using SqlSugar.IOC;
using System.Text;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceController : FinanceControllerBase
    {


        private readonly IPlatformNoticeService  _platformNoticeService;
        private readonly IFinanceOtherService    _financeOtherService;
        private readonly IAuthService   _authService;
        private readonly ICurrentUser  _currentUser;
        private readonly ISettingsService     _settingsService;
        private readonly IAccountReceivableService    _accountReceivableService;
        private readonly IFinanceOtherService   _otherService;
        private readonly IAliyunOSSService   _aliyunOSSService;
        private readonly ISalaryCustomerService   _salaryCustomerService;
        private readonly IInvoiceService  _invoiceService;
        private readonly IMapper    _mapper;

        public InvoiceController(IInvoiceService invoiceService, IMapper mapper, IAliyunOSSService aliyunOSSService, ISalaryCustomerService salaryCustomerService, IFinanceOtherService otherService, IAccountReceivableService accountReceivableService, ISettingsService settingsService, ICurrentUser currentUser, IAuthService authService, IFinanceOtherService financeOtherService, IPlatformNoticeService platformNoticeService)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
            _aliyunOSSService = aliyunOSSService;
            _salaryCustomerService = salaryCustomerService;
            _otherService = otherService;
            _accountReceivableService = accountReceivableService;
            _settingsService = settingsService;
            _currentUser = currentUser;
            _authService = authService;
            _financeOtherService = financeOtherService;
            _platformNoticeService = platformNoticeService;
        }
        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string issuerName,string recipientName,decimal? invoiceAmount,decimal? totalAmount,InvoiceStatusEnumType? status, PrintTemplateEnumType? businessType, DateTime? createMonth,string fullName, bool? isFinance, [FromQuery] PagedModelBase query)
        {

            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_Invoice>();
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
               var  info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info!=null&& info.UserType == 2)
                {
                    exp.And(f => f.PartnerID == info.PartnerID);
                }
            }
           else if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var info = await _authService.UserInfo<Model.Live_Personnel>();
                exp.And(f=> f.PartnerID == info.PartnerID);
            }

             if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {

                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
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
            exp.AndIF(!string.IsNullOrWhiteSpace(fullName), f => f.ApplyUserFullName.Contains(fullName));
            exp.AndIF(isFinance.HasValue, f => f.Status >= InvoiceStatusEnumType.待审核);
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(issuerName), f => f.IssuerName.Contains(issuerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(recipientName), f => f.RecipientName.Contains(recipientName));
            exp.AndIF(invoiceAmount.HasValue, f => invoiceAmount==invoiceAmount.Value);
            exp.AndIF(totalAmount.HasValue, f => f.TotalAmount== totalAmount.Value);
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(businessType.HasValue, f => f.BusinessType == (int)businessType.Value);
            exp.AndIF(createMonth.HasValue, f =>f.CreateTime>=createMonth.Value&&f.CreateTime<createMonth.Value.AddMonths(1));


            query.Order = "desc";
            if (isFinance.HasValue && isFinance.Value)
            {
                query.OrderField = "LockedTime";
            }
            else {
                query.OrderField = "CreateTime";
            }

            var models = await _invoiceService.PageViewList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportExcel(string customerName, string issuerName, string recipientName, decimal? invoiceAmount, decimal? totalAmount, InvoiceStatusEnumType? status, PrintTemplateEnumType? businessType, DateTime? createMonth,string fullName, bool? isFinance, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_Invoice>();
            query.Page = 0;
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }
            exp.AndIF(isFinance.HasValue, f => f.Status >= InvoiceStatusEnumType.待审核);
            //exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(fullName), f => f.ApplyUserFullName.Contains(fullName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(issuerName), f => f.IssuerName.Contains(issuerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(recipientName), f => f.RecipientName.Contains(recipientName));
            exp.AndIF(invoiceAmount.HasValue, f => f.InvoiceAmount == invoiceAmount.Value);
            exp.AndIF(totalAmount.HasValue, f => f.TotalAmount == totalAmount.Value);
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(businessType.HasValue, f => f.BusinessType == (int)businessType.Value);
            exp.AndIF(createMonth.HasValue, f =>f.CreateTime>=createMonth.Value&&f.CreateTime<createMonth.Value.AddMonths(1));

            var models = await _invoiceService.PageViewList(exp.ToExpression(), query);
            string[] InvoiceTypeEnumType = {
                "增值税专用发票",
                "增值税普通发票",
                "值税电子普通发票",
                "增值税电子专用发票"
            };


            var excelData = models.Select(f => new InvoiceExcelTemplate()
            {
                ApplyUserFullName = f.ApplyUserFullName,
                BothRecordCount = f.BothRecordCount ?? 0,
                RecipientName = f.RecipientName,
                BusinessTypeName = ((PrintTemplateEnumType)f.BusinessType).ToString(),
                CreateTime = f.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                IssuedTime = f.IssuedTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                LockedTime = f.LockedTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                CustomerName = f.CustomerName,
                Description = f.Description,
                InvoiceTypeName = InvoiceTypeEnumType[f.InvoiceType],
                IssuerName = f.IssuerName,
                RecipientCompanyNo = f.RecipientCompanyNo,
                RecordCount = f.RecordCount,
                SalaryRecordCount = f.SalaryRecordCount ?? 0,
                SerivceFeeRecordCount = f.SerivceFeeRecordCount ?? 0,
                ServiceFeeAmount = f.ServiceFeeAmount,
                StatusName = f.Status.ToString(),
                Title = f.Title,
                SalaryAmount=f.SalaryAmount??0,
                //InvoiceAmount = f.InvoiceAmount,
                TotalAmount = f.TotalAmount,
            }).ToList();

            var fileBytes = NpoiExcelHelper.WriteExcel(excelData, "发票信息");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "发票信息.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_Invoice>();
            exp.AndIF(!string.IsNullOrWhiteSpace(query.Keyword), f => f.CustomerName.Contains(query.Keyword));
            exp.AndIF(!string.IsNullOrWhiteSpace(query.Name), f => f.Title.Contains(query.Name));
            var models = await _invoiceService.PageViewList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(InvoiceManageDto modeldto)
        {
            var model = _mapper.Map<Live_Invoice>(modeldto);

            var info = await _authService.UserInfo<Model.SYS_CompanyUser>();

            if (info != null)
            {
                model.ApplyUserName = info.UserName;
                model.ApplyUserID = info.UserID;
                model.ApplyUserFullName = info.FullName;
            }





            //model.ApplyUserID=info.UserID;
            //model.ApplyUserName=info.UserName;

            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            model.ApplyTime = DateTime.Today;
            model.Status= InvoiceStatusEnumType.待锁定;
            model.CompanyID=CurrentUser.CompanyID;



            var cus = await _financeOtherService.GetCustomerByGIDAsync(model.CustomerGID);
            if (cus.Code==500)
            {
                throw new ErrorCodeException(-1,"客户单位信息获取失败");
            }
            model.PartnerGID = cus.Data.PartnerGID;
            model.PartnerName = cus.Data.PartnerName;
            model.CustomerName = cus.Data.CustomerName;

            DbScoped.SugarScope.BeginTran();

            if (modeldto.AttachFiles != null && modeldto.AttachFiles.Count > 0)
            {
                var attachFiles = await _invoiceService.ListAttachFiles(f => modeldto.AttachFiles.Contains(f.ID));
                if (attachFiles!=null&& attachFiles.Count>0)
                {
                    foreach (var item in attachFiles)
                    {
                        item.InvoiceID = model.ID;
                    }
                   await _invoiceService.UpdateAttachFiles(attachFiles);
                }
            }

            #region 开票金额汇总

            if (modeldto.TaxRecords!=null&&modeldto.TaxRecords.Any())
            {
                foreach (var item in modeldto.TaxRecords)
                {
                    item.CalculateTax();
                }
                model.InvoiceAmount = modeldto.TaxRecords.Sum(f => f.BaseAmount);
                model.AddedTaxSumAmount = modeldto.TaxRecords.Sum(f => f.ValueAddedTaxAmount);
                model.AdditionTaxSumAmount = modeldto.TaxRecords.Sum(f => f.AdditionalTaxAmount);
                model.TotalAmount = modeldto.TaxRecords.Sum(f => f.BaseAmount+f.AdditionalTaxAmount+f.ValueAddedTaxAmount)+model.ChangeAmount;


                var taxRecordList = modeldto.TaxRecords.Select(f => new Live_InvoiceTaxRecord()
                {
                    AdditionalTaxAmount = f.AdditionalTaxAmount,
                    ID = Guid.NewGuid(),
                    AdditionalTaxPointPercent = f.AdditionalTaxPointPercent,
                    BaseAmount = f.BaseAmount,
                    DataType = f.DataType,
                    InvoiceID = model.ID,
                    Remark = "",
                    TaxType = 0,
                    TotalAmount = f.TotalAmount,
                    ValueAddedTaxAmount = f.ValueAddedTaxAmount,
                    ValueAddedTaxPointPercent = f.ValueAddedTaxPointPercent,
                }).ToList();

              await  _invoiceService.AddInvoiceTaxRecords(taxRecordList);
            }


            #endregion
            await _invoiceService.Add(model);
            DbScoped.SugarScope.CommitTran();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> AddList(List<Live_Invoice> list)
        {
            await _invoiceService.Add(list);
            return Ok();
        }

            [HttpGet]
        public async Task<Model.Live_Invoice> Get(Guid id)
        {
            var model = await _invoiceService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetView(Guid id)
        {
            var model = await _invoiceService.GetView(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_Invoice modelDto)
        {

            var model =await _invoiceService.Get(modelDto.ID);
            if (model.Status!= InvoiceStatusEnumType.待锁定&&model.Status!= InvoiceStatusEnumType.审核退回)
            {
                throw new ErrorCodeException(-1, "此状态无法修改发票信息");
            }
            //Live_Enterprise enterprise = null;
            //if (model.RecipientUnitID != modelDto.RecipientUnitID)
            //{
            //    var recipientUnitRes = await _otherService.GetEnterpriseByIdAsync(modelDto.RecipientUnitID);
            //    if (recipientUnitRes == null && recipientUnitRes.Data != null)
            //    {
            //        enterprise=recipientUnitRes.Data;
            //    }
            //}
            model =  _mapper.Map(modelDto, model);
            // 修改收票单位
            //if (enterprise != null)
            //{
            //    model.RecipientName = enterprise.Name;
            //    model.RecipientBank = enterprise.BankName;
            //    model.RecipientBankNo = enterprise.BankNo;
            //    model.RecipientAddress = enterprise.Address;
            //    model.RecipientCompanyNo = enterprise.CompanyNo;
            //    model.RecipientPhone = enterprise.Phone;
            //}




            await _invoiceService.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNav([FromBody] Live_InvoiceExtension model)
        {


            if (model.Status != InvoiceStatusEnumType.待锁定 && model.Status != InvoiceStatusEnumType.审核退回)
            {
                throw new ErrorCodeException(-1, "此状态无法修改发票信息");
            }
            model.CalculationTax();
            await _invoiceService.UpdateNav(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var model = await _invoiceService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status == InvoiceStatusEnumType.冲红 || model.Status == InvoiceStatusEnumType.冲红_红字)
            {
                throw new ErrorCodeException(-1, "此状态无法删除");
            }
            await _invoiceService.Delete(id);
            return Ok(new { id });
        }
        [HttpDelete]
        public async Task<IActionResult> JoinDelete(Guid id)
        {
            //接口删除
            await _invoiceService.Delete(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> JoinUpdate([FromForm] Guid id, [FromForm] InvoiceStatusEnumType? status, [FromForm] string remark, [FromForm] decimal? totalAmount)
        {
            var model = await _invoiceService.Get(id);
            if(status.HasValue)
            {
                if(status == InvoiceStatusEnumType.待锁定)
                {
                    if (model.SalaryCustomerID.HasValue && model.SalaryCustomerID.Value != Guid.Empty)
                    {
                        model.IsReset = true;
                    }

                    model.IsLocked = false;
                    model.IsIssued = false;
                    model.Status = status.Value;
                    model.IsCancel = false;
                    model.IssuedTime = null;
                    model.LockedTime = null;
                }
                if(status == InvoiceStatusEnumType.待开具)
                {
                    model.Status = status.Value;
                    model.IsCancel = false;
                }
            }
            if(!string.IsNullOrEmpty(remark))
            {
                model.Remark = remark;
            }
            if(totalAmount.HasValue)
            {
                model.TotalAmount = totalAmount.Value;
            }
            await _invoiceService.Update(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm] Guid id,[FromForm]InvoiceStatusEnumType status)
        {
            var model = await _invoiceService.GetNav(f=>f.ID==id);

            var modelView = await _invoiceService.GetView(f=>f.ID==id);
            Live_Invoice redModel = null;
            Live_AccountReceivableExtension receivableModel = null;
            Live_AccountReceivableExtension redReceivableModel = null; 
            switch (status)
            {
                case InvoiceStatusEnumType.待锁定:

                    // 企业工资发票 记录重置状态 允许手动锁定
                    if (model.SalaryCustomerID.HasValue&& model.SalaryCustomerID.Value!=Guid.Empty)
                    {
                        model.IsReset=true;
                    }

                    model.IsLocked = false;
                    model.IsIssued = false;
                    model.Status = status;
                    model.IsCancel = false;
                    model.IssuedTime = null;
                    model.LockedTime = null;
                    break;
                case InvoiceStatusEnumType.待审核:
                    model.IsLocked = true;
                    model.LockedTime = DateTime.Now;
                    model.IsIssued = false;
                    model.Status = status;
                    model.IsCancel = false;
                    model.IssuedTime = null;
                    break;
                case InvoiceStatusEnumType.审核退回:
                    model.IsIssued = false;
                    model.Status = status;
                    model.IsCancel = false;
                    model.IssuedTime = null;
                    break;
                case InvoiceStatusEnumType.待开具:
                    model.Status = status;
                    model.IsCancel = false;
                    break;
                case InvoiceStatusEnumType.已开具:
                    model.IsIssued = true;
                    model.IssuedTime = DateTime.Now;
                    model.IsCancel = false;
                    model.Status = status;
                    break;
                case InvoiceStatusEnumType.冲红:
                    model.IsCancel = true;
                    model.Status = status;

                    model.IsCancel = true;
                    model.Status = status;
                    redModel = new Live_InvoiceExtension()
                    {
                        ID = Guid.NewGuid(),
                        Status = InvoiceStatusEnumType.冲红_红字,
                        AddedTaxAmount = model.AddedTaxAmount * -1,
                        AddedTaxDiscountAmount = model.AddedTaxDiscountAmount * -1,
                        AddedTaxSumAmount = model.AddedTaxSumAmount * -1,
                        AdditionalTaxAmount = model.AdditionalTaxAmount * -1,
                        AdditionalTaxDiscountAmount = model.AdditionalTaxDiscountAmount * -1,
                        AdditionTaxSumAmount = model.AdditionTaxSumAmount * -1,
                        ChangeAmount = model.ChangeAmount * -1,
                        InvoiceAmount = model.InvoiceAmount * -1,
                        ServiceFeeAmount = model.ServiceFeeAmount * -1,
                        TotalAmount = model.TotalAmount * -1,
                        AddedTaxPointAmount = model.AddedTaxPointAmount,
                        AdditionalTaxPointAmount = model.AdditionalTaxPointAmount,
                        ApplyTime = model.ApplyTime,
                        BusinessType = model.BusinessType,
                        CreateTime = model.CreateTime,
                        CustomerGID = model.CustomerGID,
                        Description = model.Description,
                        InvoiceType = model.InvoiceType,
                        IsAddedTax = model.IsAddedTax,
                        IsAdditionalTax = model.IsAdditionalTax,
                        IsCancel = model.IsCancel,
                        IsIssued = model.IsIssued,
                        IsLocked = model.IsLocked,
                        IsSf = model.IsSf,
                        IssuedTime = model.IssuedTime,
                        IssuerAddress = model.IssuerAddress,
                        IssuerBank = model.IssuerBank,
                        IssuerBankNo = model.IssuerBankNo,
                        IssuerCompanyNo = model.IssuerCompanyNo,
                        IssuerName = model.IssuerName,
                        IssuerPhone = model.IssuerPhone,
                        IssuerUnitID = model.IssuerUnitID,
                        LockedTime = model.LockedTime,
                        RecipientAddress = model.RecipientAddress,
                        RecipientBank = model.RecipientBank,
                        RecipientBankNo = model.RecipientBankNo,
                        RecipientCompanyNo = model.RecipientCompanyNo,
                        RecipientName = model.RecipientName,
                        RecipientPhone = model.RecipientPhone,
                        RecipientUnitID = model.RecipientUnitID,
                        RecordCount = model.RecordCount,
                        Remark = model.Remark,
                        SalaryMonth = model.SalaryMonth,
                        SfAddedTaxAmount = model.SfAddedTaxAmount,
                        SfAddedTaxPointAmount = model.SfAddedTaxPointAmount,
                        SfOtherPaidAmount = model.SfOtherPaidAmount,
                        SfPersonalTaxAmount = model.SfPersonalTaxAmount,
                        SfPersonalTaxPointAmount = model.SfPersonalTaxPointAmount,
                        SfSocialAmount = model.SfSocialAmount,
                        SfTaxRefundAmount = model.SfTaxRefundAmount,
                        Title = model.Title,
                        ApplyUserFullName = model.ApplyUserFullName,
                        ApplyUserID = model.ApplyUserID,
                        ApplyUserName = model.ApplyUserName,
                        CompanyID = model.CompanyID,
                        CustomerName = model.CustomerName,
                        IsSpecial = model.IsSpecial,
                        LegalCustomerName = model.LegalCustomerName,
                        PartnerGID = model.PartnerGID,
                        PartnerName = model.PartnerName,
                        SalaryCustomerID = model.SalaryCustomerID,
                         BothRecordCount=model.BothRecordCount??0*-1,
                          SalaryAmount=model.SalaryAmount??0*-1,
                           SalaryRecordCount=model.SalaryRecordCount??0*-1,
                            SerivceFeeRecordCount=model.SerivceFeeRecordCount??0*-1,
                        AttachFiles = model.AttachFiles.Select(f => new Live_InvoiceAttachFile()
                        {
                            CreateTime = f.CreateTime,
                            FileName = f.FileName,
                            FileOssUrl = f.FileOssUrl,
                            FilePath = f.FilePath,
                            ID = Guid.NewGuid(),
                            InvoiceID = f.InvoiceID
                        }).ToList(),
                        TaxRecords = model.TaxRecords.Select(f => new Live_InvoiceTaxRecord()
                        {
                            InvoiceID = f.InvoiceID,
                            AdditionalTaxAmount = f.AdditionalTaxAmount,
                            AdditionalTaxPointPercent = f.AdditionalTaxPointPercent,
                            BaseAmount = f.BaseAmount,
                            DataType = f.DataType,
                            ID = Guid.NewGuid(),
                            Remark = f.Remark,
                            TaxType = f.TaxType,
                            TotalAmount = f.TotalAmount,
                            ValueAddedAmount = f.ValueAddedAmount,
                            ValueAddedTaxAmount = f.ValueAddedTaxAmount,
                            ValueAddedTaxPointPercent = f.ValueAddedTaxPointPercent
                        }).ToList()

                    };

                  receivableModel = await   _accountReceivableService.Get(f => f.InvoiceID == model.ID&&f.Status< AccountReceivableStatusEnumType.冲红);
                    if (receivableModel!=null)
                    {
                        receivableModel.Status = AccountReceivableStatusEnumType.冲红;
                        redReceivableModel = new Live_AccountReceivableExtension()
                        {
                            ID = Guid.NewGuid(),
                            AnnexFileName = receivableModel.AnnexFileName,
                            AnnexFilePath = receivableModel.AnnexFilePath,
                            CompanyID = receivableModel.CompanyID,
                            CooperationUnitGID = receivableModel.CooperationUnitGID,
                            CooperationUnitName = receivableModel.CooperationUnitName,
                            CreateTime = receivableModel.CreateTime,
                            CustomerGID = receivableModel.CustomerGID,
                            CustomerName = receivableModel.CustomerName,
                            DataSourceType = receivableModel.DataSourceType,
                            Description = receivableModel.Description,
                            InvoiceID = receivableModel.InvoiceID,
                            InvoiceTotalAmount = receivableModel.InvoiceTotalAmount*-1,
                            IssuerName = receivableModel.IssuerName,
                            LegalCustomerName = receivableModel.LegalCustomerName,
                            PaidAmount = receivableModel.PaidAmount * -1,
                            PartnerGID = receivableModel.PartnerGID,
                            PartnerName = receivableModel.PartnerName,
                            PendingAmount = receivableModel.PendingAmount * -1,
                            RecipientName = receivableModel.RecipientName,
                            SfWithholdAmount = receivableModel.SfWithholdAmount * -1,
                            Status =  AccountReceivableStatusEnumType.冲红_红字,
                            TotalAmount = receivableModel.TotalAmount * -1,
                            YlBillID = receivableModel.YlBillID,
                            Records= receivableModel.Records?.Select(f=>new Live_AccountReceivableRecord() {
                             ActionUserID=f.ActionUserID,
                              //AccountReceivableID=f.AccountReceivableID,
                               ActionUserName=f.ActionUserName,
                               CreateTime=f.CreateTime,
                                ID=Guid.NewGuid(),
                                 PaidAmount=f.PaidAmount*-1,
                                  PaidTime=f.PaidTime,
                                   Remark=f.Remark,
                            }).ToList(),


                        };
                    }



                    break;
                case InvoiceStatusEnumType.冲红_红字:
                    break;
                default:
                    break;
            }
            Live_SalaryCustomerExtension salary = null;
            if (model.Status == InvoiceStatusEnumType.已开具 && model.BusinessType == PrintTemplateEnumType.外部发票 && model.SalaryCustomerID != null && model.SalaryCustomerID != Guid.Empty)
            {
                // 获取企业工资其它的外部发票
                var salaryinvoices = await _invoiceService.List(f => f.ID != model.ID && f.BusinessType == PrintTemplateEnumType.外部发票 && f.SalaryCustomerID != null && f.SalaryCustomerID == model.SalaryCustomerID.Value && f.Status != InvoiceStatusEnumType.冲红 && f.Status != InvoiceStatusEnumType.冲红_红字);
                salary = await _salaryCustomerService.GetNav(f => f.ID == model.SalaryCustomerID.Value);
                if (salary == null)
                {
                    throw new ErrorCodeException(-1, "此发票关联的客户工资已被删除，请联系管理员！");
                }
                if (salaryinvoices != null)
                {
                    if (!salaryinvoices.Any(f => f.Status != InvoiceStatusEnumType.已开具))
                    {
                        if (salary.Status == SalaryStatusEnumType.PendingReview|| salary.Status == SalaryStatusEnumType.ReviewRejected)
                        {
                            salary.Status = SalaryStatusEnumType.PendingSend;
                        }
                    }
                }
                else
                {

                    if (salary.Status == SalaryStatusEnumType.PendingReview || salary.Status == SalaryStatusEnumType.ReviewRejected)
                    {
                        salary.Status = SalaryStatusEnumType.PendingSend;
                    }
                }



            }
            else if (model.Status == InvoiceStatusEnumType.审核退回 && model.BusinessType == PrintTemplateEnumType.外部发票 && model.SalaryCustomerID != null && model.SalaryCustomerID != Guid.Empty)
            {
                salary = await _salaryCustomerService.GetNav(f => f.ID == model.SalaryCustomerID.Value);
                if (salary == null)
                {
                    throw new ErrorCodeException(-1, "此发票关联的客户工资已被删除，请联系管理员！");
                }
                salary.Status = SalaryStatusEnumType.ReviewRejected;
            }
            Live_AccountReceivableExtension receivable = null;
            // 外部发票 客户单位的法人主体单位与收票单位不一致则生成应收账款
            if (model.Status== InvoiceStatusEnumType.已开具&& model.BusinessType== PrintTemplateEnumType.外部发票&& modelView.CustomerName!= modelView.CooperationUnitName)
            {
               

                if (modelView.CooperationUnitID == null)
                {
                    throw new ErrorCodeException(-1, "此客户单位商务合作单位信息缺失，请到客户单位补全信息后重试！");
                }


                var cus = await _financeOtherService.GetCustomerByGIDAsync(model.CustomerGID);
                var existModel = await _accountReceivableService.Get(f => f.InvoiceID == model.ID && f.Status < AccountReceivableStatusEnumType.冲红);
                if (existModel == null) {
                    receivable = new Live_AccountReceivableExtension()
                    {
                        AnnexFileName = string.Empty,
                        AnnexFilePath = string.Empty,
                        RecipientName = model.RecipientName,
                        LegalCustomerName = string.Empty,
                        CreateTime = DateTime.Now,
                        CustomerGID = model.CustomerGID,
                        CooperationUnitGID = modelView.CooperationUnitID.Value,
                        CustomerName = modelView.CustomerName,
                        CooperationUnitName = modelView.CooperationUnitName,
                        DataSourceType = AccountReceivableDataSourceTypeEnumType.发票,
                        Description = string.Empty,
                        InvoiceID = model.ID,
                        InvoiceTotalAmount = model.TotalAmount,
                        PaidAmount = 0,
                        IssuerName = model.IssuerName,
                        PartnerGID = model.PartnerGID ?? cus.Data.CustomerGID.Value,
                        PartnerName = model.PartnerName ?? cus.Data.CustomerName,
                        PendingAmount = model.TotalAmount,
                        CompanyID = model.CompanyID,
                        TotalAmount = model.TotalAmount,
                        YlBillID = Guid.Empty,
                        Status = AccountReceivableStatusEnumType.待收款,
                        SfWithholdAmount = model.SfOtherPaidAmount,
                    };

                    if (model.IsSf)
                    {
                        receivable.TotalAmount = model.TotalAmount - model.SfOtherPaidAmount;
                        receivable.PendingAmount = model.TotalAmount - model.SfOtherPaidAmount;
                        receivable.Description = $"发票金额：{model.TotalAmount}元,顺丰代扣代缴：{model.SfOtherPaidAmount}元";
                        receivable.SfWithholdAmount = model.SfOtherPaidAmount;
                    }
                }
            }


            if (model.Status == InvoiceStatusEnumType.已开具 && model.BusinessType == PrintTemplateEnumType.外部发票)
            {
                var cus = await _financeOtherService.GetCustomerByGIDAsync(model.CustomerGID);
                if (cus!=null&& cus.Code==200&&cus.Data!=null&&cus.Data.PartnerID!=null)
                {
                    _platformNoticeService.ManagerDefaultNotice(cus.Data.PartnerID.Value, "发票已开具通知", $"{model.CustomerName} 发票已开具");
                }
            }





            DbScoped.SugarScope.BeginTran();

            if (receivable!=null)
            {
              await _accountReceivableService .Add(receivable);
            }
            if (redModel != null)
            {
                await _invoiceService.Add(redModel);
            }
            #region 应收账款作废情况

      

            if (receivableModel!=null)
            {
                await _accountReceivableService.UpdateNav(receivableModel);
            }

            if (redReceivableModel != null)
            {
                await _accountReceivableService.AddNav(redReceivableModel);
            }
            #endregion

            if (salary != null && (salary.Status == SalaryStatusEnumType.PendingSend || salary.Status == SalaryStatusEnumType.ReviewRejected))
            {

                if (salary.Status == SalaryStatusEnumType.PendingSend)
                {
                   await _salaryCustomerService.SetPendingSend(salary.ID);
                }
                else if (salary.Status == SalaryStatusEnumType.ReviewRejected)
                {
                    await _salaryCustomerService.SetReviewRejected(salary.ID);
                }
                //var batchRecords = salary.Records.Where(f => f.IsBatchSend).ToList();

                //if (salary.Status == SalaryStatusEnumType.PendingSend)
                //{


               
                //    if (batchRecords != null && batchRecords.Count > 0)
                //    {
                //        foreach (var item in batchRecords)
                //        {
                //            item.Status = SalaryRecordSendStatusEnumType.发放中;
                //        }
                //    }
                //}
                //else if (salary.Status == SalaryStatusEnumType.ReviewRejected)
                //{

                //    if (batchRecords != null && batchRecords.Count > 0)
                //    {
                //        foreach (var item in batchRecords)
                //        {
                //            item.Status = SalaryRecordSendStatusEnumType.审核未通过;
                //        }
                //    }
                //}
                //await _salaryCustomerService.UpdateNav(salary);
            }

            if (model.Status>= InvoiceStatusEnumType.待审核&&model.LockedTime==null)
            {
                model.LockedTime = DateTime.Now;
            }
            await _invoiceService.Update(model);

            DbScoped.SugarScope.CommitTran();
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadAttachFile(IFormFile file)
        {

           var setting=await _settingsService.GetMasterSettings();

            Live_Invoice invoice = null;
            Guid invoiceID = Guid.Empty;
            if (HttpContext.Request.Form.Any(f => f.Key == "invoiceID"))
            {
                invoiceID = Guid.Parse(HttpContext.Request.Form["invoiceID"]);
                invoice =await _invoiceService.Get(invoiceID);
            }

        
            using (var ms = new MemoryStream())
            {
                Live_InvoiceAttachFile attachFile = new Live_InvoiceAttachFile()
                {
                    FileName = file.FileName,
                    CreateTime = DateTime.Now,
                    FilePath = string.Empty,
                    InvoiceID = invoice != null? invoice.ID: Guid.Empty,
                    ID = Guid.NewGuid()
                };
                
                file.CopyTo(ms);

                var ossResp = await _aliyunOSSService.UploadFile(ms, file.FileName, true, false, "invoiceAttachFiles");

                if (ossResp.Code == 0)
                {
                    attachFile.FileOssUrl = setting.AliBucketDomain+"/" + ossResp.Data;
                }

                bool isSuc = await _invoiceService.AddAttachFile(attachFile);

                var obj = new
                {
                    uid = attachFile.ID,
                    name = attachFile.FileName,
                    status = isSuc ? "done" : "error",
                    url = attachFile.FileOssUrl,
                    response = isSuc ? "200" : "500",
                };
                return Ok(obj);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAttachFile([FromForm] Guid attachFileID)
        {
            await _invoiceService.DeleteAttachFile(f => f.ID == attachFileID);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> InjuryCompanyBillList([FromQuery] PagedModelBase query)
        {

            var exp = Expressionable.Create<Model.Live_V_Invoice>();
            
            exp.AndIF(!string.IsNullOrWhiteSpace(query.Name), f => f.CustomerName==query.Name);

            if (query.Year.HasValue)
            {
                exp.And(f => f.CreateTime.HasValue && f.CreateTime.Value.Year == query.Year);
            }
            

            var models = await _invoiceService.PageViewList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpPost]
        public IActionResult CalculateTax(InvoiceTaxCalculateDto dto)
        {
            dto.CalculateTax();
            return Ok(dto);
        }



    }
}
