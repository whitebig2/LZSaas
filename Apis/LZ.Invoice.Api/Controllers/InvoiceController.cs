using AutoMapper;
using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.InvoiceDto;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceController : InvoiceControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ISettingsService _settingsService;
        private readonly IAliyunOSSService _aliyunOSSService;
        private readonly IInvoiceService _invoiceService;
        private readonly IHttpService _httpService;
        private readonly IMapper _mapper;

        public InvoiceController(
            IInvoiceService invoiceService,
            IMapper mapper,
            IAliyunOSSService aliyunOSSService,
            ISettingsService settingsService,
            IAuthService authService,
            IHttpService httpService)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
            _aliyunOSSService = aliyunOSSService;
            _settingsService = settingsService;
            _authService = authService;
            _httpService = httpService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, string issuerName, string recipientName, decimal? invoiceAmount, decimal? totalAmount, InvoiceStatusEnumType? status, PrintTemplateEnumType? businessType, DateTime? createMonth, string fullName, bool? isFinance, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_Invoice>();
            if (CurrentUser.PlatformType == PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(f => f.PartnerID == info.PartnerID);
                }
            }
            else if (CurrentUser.PlatformType == PlatformType.LZMini)
            {
                var info = await _authService.UserInfo<Model.Live_Personnel>();
                exp.And(f => f.PartnerID == info.PartnerID);
            }

            if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {
                exp.And(it => it.CompanyID == user.CompanyID);
            }
            exp.AndIF(!string.IsNullOrWhiteSpace(fullName), f => f.ApplyUserFullName.Contains(fullName));
            exp.AndIF(isFinance.HasValue, f => f.Status >= InvoiceStatusEnumType.待审核);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(issuerName), f => f.IssuerName.Contains(issuerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(recipientName), f => f.RecipientName.Contains(recipientName));
            exp.AndIF(invoiceAmount.HasValue, f => f.InvoiceAmount == invoiceAmount.Value);
            exp.AndIF(totalAmount.HasValue, f => f.TotalAmount == totalAmount.Value);
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(businessType.HasValue, f => f.BusinessType == (int)businessType.Value);
            exp.AndIF(createMonth.HasValue, f => f.CreateTime >= createMonth.Value && f.CreateTime < createMonth.Value.AddMonths(1));

            query.Order = "desc";
            query.OrderField = isFinance.HasValue && isFinance.Value ? "LockedTime" : "CreateTime";

            var models = await _invoiceService.PageViewList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(string customerName, string issuerName, string recipientName, decimal? invoiceAmount, decimal? totalAmount, InvoiceStatusEnumType? status, PrintTemplateEnumType? businessType, DateTime? createMonth, string fullName, bool? isFinance, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_Invoice>();
            query.Page = 0;
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            if (CurrentUser.PlatformType == PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                if (info != null && info.UserType == 2)
                {
                    exp.And(t => t.PartnerID == info.PartnerID);
                }
            }
            exp.AndIF(isFinance.HasValue, f => f.Status >= InvoiceStatusEnumType.待审核);
            exp.AndIF(!string.IsNullOrWhiteSpace(fullName), f => f.ApplyUserFullName.Contains(fullName));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(issuerName), f => f.IssuerName.Contains(issuerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(recipientName), f => f.RecipientName.Contains(recipientName));
            exp.AndIF(invoiceAmount.HasValue, f => f.InvoiceAmount == invoiceAmount.Value);
            exp.AndIF(totalAmount.HasValue, f => f.TotalAmount == totalAmount.Value);
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(businessType.HasValue, f => f.BusinessType == (int)businessType.Value);
            exp.AndIF(createMonth.HasValue, f => f.CreateTime >= createMonth.Value && f.CreateTime < createMonth.Value.AddMonths(1));

            var models = await _invoiceService.PageViewList(exp.ToExpression(), query);
            string[] invoiceTypeNames = {
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
                InvoiceTypeName = invoiceTypeNames[f.InvoiceType],
                IssuerName = f.IssuerName,
                RecipientCompanyNo = f.RecipientCompanyNo,
                RecordCount = f.RecordCount,
                SalaryRecordCount = f.SalaryRecordCount ?? 0,
                SerivceFeeRecordCount = f.SerivceFeeRecordCount ?? 0,
                ServiceFeeAmount = f.ServiceFeeAmount,
                StatusName = f.Status.ToString(),
                Title = f.Title,
                SalaryAmount = f.SalaryAmount ?? 0,
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

            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            model.ApplyTime = DateTime.Today;
            model.Status = InvoiceStatusEnumType.待锁定;
            model.CompanyID = CurrentUser.CompanyID;

            var cus = await _httpService.GetAsync<Live_V_Customer>(ServiceNames.CustomerService, $"/api/Customer/GetByGID?customerGID={model.CustomerGID}");
            if (cus == null)
            {
                throw new ErrorCodeException(-1, "客户单位信息获取失败");
            }
            model.PartnerGID = cus.PartnerGID;
            model.PartnerName = cus.PartnerName;
            model.CustomerName = cus.CustomerName;

            DbScoped.SugarScope.BeginTran();
            try
            {
                if (modeldto.AttachFiles != null && modeldto.AttachFiles.Count > 0)
                {
                    var attachFiles = await _invoiceService.ListAttachFiles(f => modeldto.AttachFiles.Contains(f.ID));
                    if (attachFiles != null && attachFiles.Count > 0)
                    {
                        foreach (var item in attachFiles)
                        {
                            item.InvoiceID = model.ID;
                        }
                        await _invoiceService.UpdateAttachFiles(attachFiles);
                    }
                }

                if (modeldto.TaxRecords != null && modeldto.TaxRecords.Any())
                {
                    foreach (var item in modeldto.TaxRecords)
                    {
                        item.CalculateTax();
                    }
                    model.InvoiceAmount = modeldto.TaxRecords.Sum(f => f.BaseAmount);
                    model.AddedTaxSumAmount = modeldto.TaxRecords.Sum(f => f.ValueAddedTaxAmount);
                    model.AdditionTaxSumAmount = modeldto.TaxRecords.Sum(f => f.AdditionalTaxAmount);
                    model.TotalAmount = modeldto.TaxRecords.Sum(f => f.BaseAmount + f.AdditionalTaxAmount + f.ValueAddedTaxAmount) + model.ChangeAmount;

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

                    await _invoiceService.AddInvoiceTaxRecords(taxRecordList);
                }

                await _invoiceService.Add(model);
                DbScoped.SugarScope.CommitTran();
            }
            catch
            {
                DbScoped.SugarScope.RollbackTran();
                throw;
            }
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
            var model = await _invoiceService.GetView(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_Invoice modelDto)
        {
            var model = await _invoiceService.Get(modelDto.ID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (model.Status != InvoiceStatusEnumType.待锁定 && model.Status != InvoiceStatusEnumType.审核退回)
            {
                throw new ErrorCodeException(-1, "此状态无法修改发票信息");
            }
            model = _mapper.Map(modelDto, model);
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
            await _invoiceService.Delete(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> JoinUpdate([FromForm] Guid id, [FromForm] InvoiceStatusEnumType? status, [FromForm] string remark, [FromForm] decimal? totalAmount)
        {
            var model = await _invoiceService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            if (status.HasValue)
            {
                if (status == InvoiceStatusEnumType.待锁定)
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
                if (status == InvoiceStatusEnumType.待开具)
                {
                    model.Status = status.Value;
                    model.IsCancel = false;
                }
            }
            if (!string.IsNullOrEmpty(remark))
            {
                model.Remark = remark;
            }
            if (totalAmount.HasValue)
            {
                model.TotalAmount = totalAmount.Value;
            }
            await _invoiceService.Update(model);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadAttachFile(IFormFile file)
        {
            var setting = await _settingsService.GetMasterSettings();

            Live_Invoice? invoice = null;
            if (HttpContext.Request.Form.Any(f => f.Key == "invoiceID"))
            {
                var invoiceID = Guid.Parse(HttpContext.Request.Form["invoiceID"]);
                invoice = await _invoiceService.Get(invoiceID);
            }

            using (var ms = new MemoryStream())
            {
                Live_InvoiceAttachFile attachFile = new Live_InvoiceAttachFile()
                {
                    FileName = file.FileName,
                    CreateTime = DateTime.Now,
                    FilePath = string.Empty,
                    InvoiceID = invoice != null ? invoice.ID : Guid.Empty,
                    ID = Guid.NewGuid()
                };

                file.CopyTo(ms);
                var ossResp = await _aliyunOSSService.UploadFile(ms, file.FileName, true, false, "invoiceAttachFiles");
                if (ossResp.Code == 0)
                {
                    attachFile.FileOssUrl = setting.AliBucketDomain + "/" + ossResp.Data;
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
            exp.AndIF(!string.IsNullOrWhiteSpace(query.Name), f => f.CustomerName == query.Name);
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
