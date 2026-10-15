using LZ.Clients.API;
using LZ.Common;
using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate.profit;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Cache;
using Senparc.NeuChar.Entities;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.API.Controllers
{
    [AllowAnonymous]
    public class ProfitSigningUnitReportController : FinanceControllerBase
    {

        private readonly IFinanceOtherService   _financeOtherService;
        private readonly IUserEntryService  _userEntryService;
        private readonly IProfitManageService _manageService;
        private readonly IProfitConfigService _configService;
        private readonly IProfitManageRecordService _recordService;
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly IProfitSigningUnitReportService  _profitSigningUnitReportService;
        private readonly IInvoiceCompanyService   _invoiceCompanyService;
        private readonly IInvoiceService    _invoiceService;
        private readonly ISettlementService    _settlementService;

        public ProfitSigningUnitReportController(IProfitManageService manageService, IProfitConfigService configService, IProfitManageRecordService recordService, IUserEntryService userEntryService, ISalaryRecordService salaryRecordService, IProfitSigningUnitReportService profitSigningUnitReportService, IInvoiceCompanyService invoiceCompanyService, IFinanceOtherService financeOtherService, IInvoiceService invoiceService, ISettlementService settlementService)
        {
            _manageService = manageService;
            _configService = configService;
            _recordService = recordService;
            _userEntryService = userEntryService;
            _salaryRecordService = salaryRecordService;
            _profitSigningUnitReportService = profitSigningUnitReportService;
            _invoiceCompanyService = invoiceCompanyService;
            _financeOtherService = financeOtherService;
            _invoiceService = invoiceService;
            _settlementService = settlementService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string signingUnitName,DateTime? billMonth,bool? isFinance, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_ProfitSigningUnitReport>();
            exp.AndIF(!string.IsNullOrWhiteSpace(signingUnitName), f => f.SigningUnitName.Contains(signingUnitName));
            exp.AndIF(billMonth != null, f => f.BillMonth== billMonth.Value);
            exp.AndIF(isFinance.HasValue&&isFinance.Value, f =>f.IsPush==true);
            var models = await _profitSigningUnitReportService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_ProfitSigningUnitReport model)
        {
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await _profitSigningUnitReportService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_ProfitSigningUnitReport> Get(Guid id)
        {
            var model = await _profitSigningUnitReportService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpGet]
        public async Task<Live_ProfitSigningUnitReportExtension> GetDomain(Guid id)
        {
            var model = await _profitSigningUnitReportService.GetDomain(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_ProfitSigningUnitReport model)
        {
            try
            {
                var existModel = await _profitSigningUnitReportService.GetDomain(f => f.ID == model.ID);
                DbScoped.SugarScope.BeginTran();
                model.CreateTime= existModel.CreateTime;
                await _profitSigningUnitReportService.Update(model);
                if (model.BusInsuranceSheetID.HasValue)
                {
                    var sheet = await _settlementService.Get(model.BusInsuranceSheetID.Value);
                    if (sheet != null)
                    {
                        sheet.TotalAmount = model.BusInsuranceAmount + model.BusInsuranceBlanceAmount.Value;
                        await _settlementService.Update(sheet);
                    }
                }

                if (model.IsRiskPrint)
                {
                    if (model.RiskInvoiceID.HasValue)
                    {
                        var invoice = await _invoiceService.Get(model.RiskInvoiceID.Value);
                        if (invoice!=null)
                        {
                            invoice.Status = InvoiceStatusEnumType.已开具;
                            await _invoiceService.Update(invoice);
                        }
                    }
                }

                if (model.IsBusinessPrint)
                {
                    if (model.BusinessInvoiceID.HasValue)
                    {
                        var invoice = await _invoiceService.Get(model.BusinessInvoiceID.Value);
                        if (invoice != null)
                        {
                            invoice.Status = InvoiceStatusEnumType.已开具;
                            await _invoiceService.Update(invoice);
                        }
                    }
                }
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message);
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {


            try
            {
                DbScoped.SugarScope.BeginTran();
                var model = await _profitSigningUnitReportService.Get(f => f.ID == id);
                if (model != null)
                {
                 
                    // 派单费发票
                    if (model.BusinessInvoiceID.HasValue)
                    {
                        await _invoiceService.Delete(model.BusinessInvoiceID.Value);
                    }
                    // 风险金发票
                    if (model.RiskInvoiceID.HasValue)
                    {
                        await _invoiceService.Delete(model.RiskInvoiceID.Value);
                    }

                    // 风险金结算单
                    if (model.RiskSheetID.HasValue)
                    {
                        await _settlementService.Delete(model.RiskSheetID.Value);
                    }
                    // 派单费结算单
                    if (model.BusinessSheetID.HasValue)
                    {
                        await _settlementService.Delete(model.BusinessSheetID.Value);
                    }
                    // 保费结算单
                    if (model.BusInsuranceSheetID.HasValue)
                    {
                        await _settlementService.Delete(model.BusInsuranceSheetID.Value);
                    }
                }

                await _profitSigningUnitReportService.Delete(id);

                DbScoped.SugarScope.CommitTran();

            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();
                throw new ErrorCodeException(-1, ex.Message);
            }
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> SetPush([FromForm] Guid id,[FromForm]bool isPush)
        {
            var model = await _profitSigningUnitReportService.Get(f => f.ID == id);
            model.IsPush = isPush;



            if (model.BusInsuranceSheetID.HasValue)
            {
                var sheet = await _settlementService.Get(model.BusInsuranceSheetID.Value);
                if (sheet != null)
                {
                    sheet.TotalAmount = model.BusInsuranceAmount + model.BusInsuranceBlanceAmount.Value;
                    await _settlementService.Update(sheet);
                }
            }

           
                if (model.RiskInvoiceID.HasValue)
                {
                    var invoice = await _invoiceService.Get(model.RiskInvoiceID.Value);
                    if (invoice != null)
                    {
                        invoice.Status = InvoiceStatusEnumType.待开具;
                        await _invoiceService.Update(invoice);
                    }
                }

           
                if (model.BusinessInvoiceID.HasValue)
                {
                    var invoice = await _invoiceService.Get(model.BusinessInvoiceID.Value);
                    if (invoice != null)
                    {
                        invoice.Status = InvoiceStatusEnumType.待开具;
                        await _invoiceService.Update(invoice);
                    }
                }

            await  _profitSigningUnitReportService.Update(model);

            return Ok();
        }





        [HttpPost]
        public async Task<IActionResult> GenerateBill()
        {
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var billMonth = currentMonth.AddMonths(-1);
            var configList = await _configService.ListView(f => f.IsEnabled);
            var signingUnitNameList = configList.Select(f => f.SigningUnitName).Distinct().ToList();


          var existModelList= await _profitSigningUnitReportService.List(f => f.BillMonth == billMonth);

            var reportList = await _profitSigningUnitReportService.GroupBySigningUnit(billMonth);



            var xfzadCompany = await _invoiceCompanyService.Get(f => f.CompanyName == "宁波幸福职安盾雇主企业管理有限公司" && f.IsEnabled);
            if (xfzadCompany == null)
            {
                throw new ErrorCodeException(-1, $"【宁波幸福职安盾雇主企业管理有限公司】 开票单位信息缺失");
            }

            var wlkjCompany = await _invoiceCompanyService.Get(f => f.CompanyName == "宁波莱博网络科技有限公司" && f.IsEnabled);
            if (wlkjCompany==null)
            {
                throw new ErrorCodeException(-1,$"【宁波莱博网络科技有限公司】 开票单位信息缺失");
            }
            var ylrlCompany = await _invoiceCompanyService.Get(f => f.CompanyName == "宁波莱博友联人力资源有限公司" && f.IsEnabled);

            if (ylrlCompany == null)
            {
                throw new ErrorCodeException(-1, $"【宁波莱博友联人力资源有限公司】 开票单位信息缺失");
            }
            var byqyCompany = await _invoiceCompanyService.Get(f => f.CompanyName == "宁波宝誉企业服务有限公司" && f.IsEnabled);


            if (byqyCompany == null)
            {
                throw new ErrorCodeException(-1, $"【宁波宝誉企业服务有限公司】 开票单位信息缺失");
            }
            var cusResp = await _financeOtherService.GetCustomerByNameAsync("平台默认开票单位");

            if (cusResp==null|| cusResp.Code!=200|| cusResp.Data==null)
            {
                throw new ErrorCodeException(-1, "平台默认开票单位 客户单位不存在");
            }
            var cus = cusResp.Data;


            List<Live_ProfitSigningUnitReport> gnerateList = new List<Live_ProfitSigningUnitReport>();
            List<Live_Settlement> settlementList = new List<Live_Settlement>();
            List<Live_Invoice> invoiceList = new List<Live_Invoice>();
            foreach (var item in reportList)
            {
                if (existModelList.Any(f => f.SigningUnitName == item.SigningUnitName))
                {
                    continue;
                }

                var signingUnit = await _invoiceCompanyService.Get(f => f.CompanyName == item.SigningUnitName && f.IsEnabled);

               


                if (signingUnit == null)
                {
                    throw new ErrorCodeException(-1, $"【{item.SigningUnitName}】 开票单位信息缺失");
                }
                #region 业务派单费
                var businessSheet = new Live_Settlement()
                {
                    ID=Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    CustomerGID = cus.CustomerGID.Value,
                    Remark = string.Empty,
                    SalaryCustomerID = null,
                    SalaryMonth = null,
                    SettlementType = (int)SettlementBusinessTypeEnumType.费用审批单,
                    //Title = "新业务开发费",
                    Title = "软件服务费",
                    TotalAmount = item.BusinessAmount,
                    ApplyDate = DateTime.Now,
                    BusinessType = SettlementBusinessTypeEnumType.费用审批单,

                    IssuerUnitID = signingUnit.ID,
                    IssuerName = signingUnit.CompanyName,
                    IssuerCompanyNo = signingUnit.CompanyNo,
                    IssuerPhone = signingUnit.Phone,
                    IssuerBank = signingUnit.Bank,
                    IssuerBankNo = signingUnit.BankNo,
                    IssuerAddress = signingUnit.Address,
                    RecipientUnitID = wlkjCompany.ID,
                    RecipientName = wlkjCompany.CompanyName,
                    RecipientPhone = wlkjCompany.Phone,
                    RecipientAddress = wlkjCompany.Address,
                    RecipientBank = wlkjCompany.Bank,
                    RecipientBankNo = wlkjCompany.BankNo,
                    RecipientCompanyNo = wlkjCompany.CompanyNo,
                    CustomerName = wlkjCompany.CompanyName,

                    CustomerAddress = string.Empty,
                    CustomerCompanyNo = string.Empty,
                    CustomerBank = string.Empty,
                    CustomerBankNo = string.Empty,
                    CustomerPhone = string.Empty,
                    Description = $"支付{wlkjCompany.CompanyName} {billMonth.AddMonths(-1).ToString("yyyy-MM")}月软件服务费。"
                };
                #endregion
                #region 风险金结算单
                var riskSheet = new Live_Settlement()
                {
                    ID=Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    CustomerGID = cus.CustomerGID.Value,
                    Remark = string.Empty,
                    SalaryCustomerID = null,
                    SalaryMonth = null,
                    SettlementType = (int)SettlementBusinessTypeEnumType.费用审批单,
                    //Title = "合伙人保障金",
                    Title = "软件服务费",
                    TotalAmount = item.RiskAmount,
                    ApplyDate = DateTime.Now,
                    BusinessType = SettlementBusinessTypeEnumType.费用审批单,

                    IssuerUnitID = signingUnit.ID,
                    IssuerName = signingUnit.CompanyName,
                    IssuerCompanyNo = signingUnit.CompanyNo,
                    IssuerPhone = signingUnit.Phone,
                    IssuerBank = signingUnit.Bank,
                    IssuerBankNo = signingUnit.BankNo,
                    IssuerAddress = signingUnit.Address,
                    RecipientUnitID = xfzadCompany.ID,
                    RecipientName = xfzadCompany.CompanyName,
                    RecipientPhone = xfzadCompany.Phone,
                    RecipientAddress = xfzadCompany.Address,
                    RecipientBank = xfzadCompany.Bank,
                    RecipientBankNo = xfzadCompany.BankNo,
                    RecipientCompanyNo = xfzadCompany.CompanyNo,
                    CustomerName = xfzadCompany.CompanyName,
                    CustomerAddress = string.Empty,
                    CustomerCompanyNo = string.Empty,
                    CustomerBank = string.Empty,
                    CustomerBankNo = string.Empty,
                    CustomerPhone = string.Empty,
                    Description = $"支付{xfzadCompany.CompanyName} {billMonth.AddMonths(+1).ToString("yyyy-MM")}月软件服务费。"
                };
                #endregion
                #region 保费审批单
                var busInsuranceSheet = new Live_Settlement()
                {
                    ID = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    CustomerGID = cus.CustomerGID.Value,
                    CustomerName = byqyCompany.CompanyName,
                    Remark = string.Empty,
                    SalaryCustomerID = null,
                    SalaryMonth = null,
                    SettlementType = (int)SettlementBusinessTypeEnumType.费用审批单,
                    Title = "商业保险费",
                    TotalAmount = item.BusInsuranceAmount,
                    ApplyDate = DateTime.Now,
                    BusinessType = SettlementBusinessTypeEnumType.费用审批单,

                    IssuerUnitID = signingUnit.ID,
                    IssuerName = signingUnit.CompanyName,
                    IssuerCompanyNo = signingUnit.CompanyNo,
                    IssuerPhone = signingUnit.Phone,
                    IssuerBank = signingUnit.Bank,
                    IssuerBankNo = signingUnit.BankNo,
                    IssuerAddress = signingUnit.Address,


                    RecipientUnitID = byqyCompany.ID,
                    RecipientName = byqyCompany.CompanyName,
                    RecipientPhone = byqyCompany.Phone,
                    RecipientAddress = byqyCompany.Address,
                    RecipientBank = byqyCompany.Bank,
                    RecipientBankNo = byqyCompany.BankNo,
                    RecipientCompanyNo = byqyCompany.CompanyNo,
                   



                    CustomerAddress = string.Empty,
                    CustomerCompanyNo = string.Empty,
                    CustomerBank = string.Empty,
                    CustomerBankNo = string.Empty,
                    CustomerPhone = string.Empty,
                    Description = $"支付{byqyCompany.CompanyName} {billMonth.AddMonths(1).ToString("yyyy-MM")}月商业保险费。"
                };
                #endregion
                #region 风险金发票
                var riskInvoice =  new Live_InvoiceExtension()
                {
                    ID = Guid.NewGuid(),
                    SalaryCustomerID =null,
                    CustomerGID = cus.CustomerGID.Value,
                    Remark = string.Empty,
                    AddedTaxPointAmount = 0,
                    AdditionalTaxPointAmount = 0,
                    InvoiceAmount = item.RiskAmount,
                    InvoiceType =  InvoiceTypeEnumType.SpecialInvoice,
                    Title = "信息技术服务*软件服务",
                    AddedTaxDiscountAmount = 0,
                    AdditionalTaxDiscountAmount = 0,
                    ApplyTime = DateTime.Now,
                    BusinessType =   PrintTemplateEnumType.保障金发票,
                    AddedTaxAmount = 0,
                    AddedTaxSumAmount = 0,
                    AdditionalTaxAmount = 0,
                    AdditionTaxSumAmount = 0,
                    ChangeAmount = 0,
                    CreateTime = DateTime.Now,
                    Description = $"{xfzadCompany.CompanyName}-{signingUnit.CompanyName}合伙人保障金",
                    IsAddedTax = false,
                    IsAdditionalTax = false,
                    IsCancel = false,
                    IsIssued = false,
                    IsLocked = false,
                    IsSf = false,
                    IssuedTime = null,
                    IssuerAddress = xfzadCompany.Address,
                    IssuerBank = xfzadCompany.Bank,
                    IssuerBankNo = xfzadCompany.BankNo,
                    IssuerCompanyNo = xfzadCompany.CompanyNo,
                    IssuerName = xfzadCompany.CompanyName,
                    IssuerPhone = xfzadCompany.Phone,
                    LockedTime = DateTime.Now,
                    RecordCount = 0,
                    SalaryMonth = null,
                    SfAddedTaxAmount = 0,
                    ServiceFeeAmount = 0,
                    SfAddedTaxPointAmount = 0,
                    TotalAmount = item.RiskAmount,
                    Status = InvoiceStatusEnumType.待锁定,
                    SfOtherPaidAmount = 0,
                    SfTaxRefundAmount = 0,
                    SfPersonalTaxPointAmount = 0,
                    SfPersonalTaxAmount = 0,
                    SfSocialAmount = 0,
                    IsSpecial = false,
                    LegalCustomerName = string.Empty,
                    PartnerGID = cus.PartnerGID,
                    PartnerName = cus.PartnerName,
                    CompanyID = cus.CompanyID,
                    CustomerName = string.Empty,
                    IssuerUnitID = xfzadCompany.ID,
                    RecipientBank = signingUnit.Bank,
                    RecipientAddress = signingUnit.Address,
                    RecipientBankNo = signingUnit.BankNo,
                    RecipientPhone = signingUnit.Phone,
                    RecipientCompanyNo = signingUnit.CompanyNo,
                    RecipientName = signingUnit.CompanyName,
                    RecipientUnitID = signingUnit.ID,
                };
                #endregion
                #region 派单费发票
                var businessInvoice = new Live_InvoiceExtension()
                {
                    ID = Guid.NewGuid(),
                    SalaryCustomerID = null,
                    CustomerGID = cus.CustomerGID.Value,
                    Remark = string.Empty,
                    AddedTaxPointAmount = 0,
                    AdditionalTaxPointAmount = 0,
                    InvoiceAmount = item.BusinessAmount,

                    InvoiceType = InvoiceTypeEnumType.SpecialInvoice,
                    Title = "信息技术服务*软件服务",
                    AddedTaxDiscountAmount = 0,
                    AdditionalTaxDiscountAmount = 0,
                    ApplyTime = DateTime.Now,
                    BusinessType = PrintTemplateEnumType.业务派单发票,
                    AddedTaxAmount = 0,
                    AddedTaxSumAmount = 0,
                    AdditionalTaxAmount = 0,
                    AdditionTaxSumAmount = 0,
                    ChangeAmount = 0,
                    CreateTime = DateTime.Now,
                    Description = $"{wlkjCompany.CompanyName}-{signingUnit.CompanyName}-新业务开发费",
                    IsAddedTax = false,
                    IsAdditionalTax = false,
                    IsCancel = false,
                    IsIssued = false,
                    IsLocked = false,
                    IsSf = false,
                    IssuedTime = null,
                    IssuerAddress = wlkjCompany.Address,
                    IssuerBank = wlkjCompany.Bank,
                    IssuerBankNo = wlkjCompany.BankNo,
                    IssuerCompanyNo = wlkjCompany.CompanyNo,
                    IssuerName = wlkjCompany.CompanyName,
                    IssuerPhone = wlkjCompany.Phone,
                    LockedTime = DateTime.Now,
                    RecordCount = 0,
                    SalaryMonth = null,
                    SfAddedTaxAmount = 0,
                    ServiceFeeAmount = 0,
                    SfAddedTaxPointAmount = 0,
                    TotalAmount = item.BusinessAmount,
                    Status = InvoiceStatusEnumType.待锁定,
                    SfOtherPaidAmount = 0,
                    SfTaxRefundAmount = 0,
                    SfPersonalTaxPointAmount = 0,
                    SfPersonalTaxAmount = 0,
                    SfSocialAmount = 0,
                    IsSpecial = false,
                    LegalCustomerName = string.Empty,
                    PartnerGID = cus.PartnerGID,
                    PartnerName = cus.PartnerName,
                    CompanyID = cus.CompanyID,
                    CustomerName = string.Empty,
                    IssuerUnitID = wlkjCompany.ID,
                    RecipientBank = signingUnit.Bank,
                    RecipientAddress = signingUnit.Address,
                    RecipientBankNo = signingUnit.BankNo,
                    RecipientPhone = signingUnit.Phone,
                    RecipientCompanyNo = signingUnit.CompanyNo,
                    RecipientName = signingUnit.CompanyName,
                    RecipientUnitID = signingUnit.ID,
                  
                };
                #endregion
                // 保费
                settlementList.Add(busInsuranceSheet);
                item.BusInsuranceSheetID = busInsuranceSheet.ID;
                //风险金
                settlementList.Add(riskSheet);
                invoiceList.Add(riskInvoice);
                item.RiskInvoiceID = riskInvoice.ID;
                item.RiskSheetID = riskSheet.ID;


                // 业务派单费
                if (wlkjCompany.CompanyName != signingUnit.CompanyName)
                {
                    settlementList.Add(businessSheet);
                    invoiceList.Add(businessInvoice);
                    item.BusinessInvoiceID = businessInvoice.ID;
                    item.BusinessSheetID = businessSheet.ID;
                }


                gnerateList.Add(item);
            }

            try
            {
                DbScoped.SugarScope.BeginTran();

                foreach (var item in invoiceList)
                {
                    await _invoiceService.Add(item);
                }
                foreach (var item in settlementList)
                {
                    await _settlementService.Add(item);
                }
                await _profitSigningUnitReportService.Add(gnerateList);
                DbScoped.SugarScope.CommitTran();
            }
            catch (Exception ex)
            {
                DbScoped.SugarScope.RollbackTran();

                throw new ErrorCodeException(-1,ex.Message);
            }
         
            return Ok(reportList);
        }
        [HttpPost]
        public async Task<IActionResult> ExportRecord([FromForm] Guid id)
        {
            var model = await _profitSigningUnitReportService.Get(f => f.ID == id);
            var recordList = await _recordService.List(f => f.ProfitMonth == model.BillMonth && f.SigningUnitName == model.SigningUnitName);
            var data2 = recordList.Select(f => new ProfitManageRecordExcelDto()
            {

                AgentPaidAmount = f.AgentPaidAmount,
                AgentPaidTaxAmount = f.AgentPaidTaxAmount,
                BusinessAmount = f.BusinessAmount,
                //BusInsuranceBalanceAmount = f.BusInsuranceBalanceAmount,
                //BusInsuranceCurrentAmount = f.BusInsuranceCurrentAmount,
                BusInsuranceNextAmount = f.BusInsuranceNextAmount,
                BusInsuranceSumAmount = f.BusInsuranceSumAmount,
                BusInsuranceChangeAmount = f.BusInsuranceCurrentAmount + f.BusInsuranceBalanceAmount - f.BusInsuranceLastAmount,
                Days = f.Days,
                DisabilityAmount = f.DisabilityAmount,
                EndDate = f.EndDate.Value.ToString("yyyy-MM-dd"),
                EntryDate = f.EntryDate.Value.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveDate?.ToString("yyyy-MM-dd"),
                FromType = f.FromType,
                IdCardNo = f.IdCardNo,
                InJobDays = f.InJobDays,
                IsInJob = f.IsInJob ? "是" : "否",
                CustomerName = f.CustomerName,
                Name = f.Name,
                PartnerNamer = f.PartnerNamer,
                PlanName1 = f.PlanName1,
                ProfitMonth = f.ProfitMonth.Value.ToString("yyyy-MM-dd"),
                JoinManageAmount = f.JoinManageAmount,
                RiskCurrentAmount = f.RiskCurrentAmount,
                ServiceFeeTotalAmount = f.ServiceFeeTotalAmount,
                SigningUnitName = f.SigningUnitName,
                StartDate = f.StartDate.Value.ToString("yyyy-MM-dd"),
                TaxMonth = f.TaxMonth.Value.ToString("yyyy-MM-dd"),
                UnitSalaryTotalAmount = f.UnitSalaryTotalAmount,
                UnitSocialTotalAmount = f.UnitSocialTotalAmount,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                CustomerThingAmount = f.CustomerThingAmount,
                ProfitAmount = f.ServiceFeeTotalAmount - f.JoinManageAmount - f.CustomerThingAmount - f.BusInsuranceSumAmount - f.RiskCurrentAmount - f.BusinessAmount - f.DisabilityAmount - f.UnitSocialTotalAmount - f.AgentPaidTaxAmount
            }).ToList();
            data2.Add(new ProfitManageRecordExcelDto()
            {

                AgentPaidAmount = data2.Sum(f => f.AgentPaidAmount),
                AgentPaidTaxAmount = data2.Sum(f => f.AgentPaidTaxAmount),
                BusinessAmount = data2.Sum(f => f.BusinessAmount),
                //BusInsuranceBalanceAmount = f.BusInsuranceBalanceAmount,
                //BusInsuranceCurrentAmount = f.BusInsuranceCurrentAmount,
                BusInsuranceNextAmount = data2.Sum(f => f.BusInsuranceNextAmount),
                BusInsuranceSumAmount = data2.Sum(f => f.BusInsuranceSumAmount),
                BusInsuranceChangeAmount = data2.Sum(f => f.BusInsuranceChangeAmount),
                Days = data2.Sum(f => f.Days),
                DisabilityAmount = data2.Sum(f => f.DisabilityAmount),
                EndDate = "",
                EntryDate = "",
                LeaveDate = "",
                FromType = "",
                IdCardNo = "",
                InJobDays = data2.Sum(f => f.InJobDays),
                IsInJob = "",
                CustomerName = "合计",
                Name = "",
                PartnerNamer = "",
                PlanName1 = "",
                ProfitMonth = "",
                JoinManageAmount = data2.Sum(f => f.JoinManageAmount),
                RiskCurrentAmount = data2.Sum(f => f.RiskCurrentAmount),
                ServiceFeeTotalAmount = data2.Sum(f => f.ServiceFeeTotalAmount),
                SigningUnitName = "",
                StartDate = "",
                TaxMonth = "",
                UnitSalaryTotalAmount = data2.Sum(f => f.UnitSalaryTotalAmount),
                UnitSocialTotalAmount = data2.Sum(f => f.UnitSocialTotalAmount),
                WorkClass = "",
                WorkName = "",
                CustomerThingAmount = data2.Sum(f => f.CustomerThingAmount),
                ProfitAmount = data2.Sum(f => f.ProfitAmount),
            });
            var fileBytes = NpoiExcelHelper.WriteExcel< ProfitManageRecordExcelDto>( data2, "派单费");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "派单费.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }


    }
}
