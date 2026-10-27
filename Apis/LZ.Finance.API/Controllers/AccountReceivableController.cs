using LZ.Clients.API;
using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Dtos.SpecialDeductionDtos;
using LZ.Model.Enums;
using LZ.Model.Enums.Budget;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountReceivableController : ApiControllerBase
    {

        private readonly ISalaryCustomerService   _salaryCustomerService;
        private readonly IInvoiceService  _invoiceService;
        private readonly IAccountReceivableService _accountReceivableService;
        private readonly IFinanceOtherService _otherService;
        private readonly IAuthService _authService;

        public AccountReceivableController(IAccountReceivableService accountReceivableService, IFinanceOtherService otherService, IAuthService authService, IInvoiceService invoiceService, ISalaryCustomerService salaryCustomerService)
        {
            _accountReceivableService = accountReceivableService;
            _otherService = otherService;
            _authService = authService;
            _invoiceService = invoiceService;
            _salaryCustomerService = salaryCustomerService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string issuerName, string partnerName,string recipientName, AccountReceivableStatusEnumType? status,decimal? invoiceTotalAmount,string createTime, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_AccountReceivableExtension>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(issuerName), f => f.IssuerName.Contains(issuerName));
            exp.AndIF(invoiceTotalAmount.HasValue, f => f.InvoiceTotalAmount == invoiceTotalAmount);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(recipientName), f => f.RecipientName.Contains(recipientName));
            exp.AndIF(status.HasValue, f =>f.Status==status.Value);
            if (!string.IsNullOrEmpty(createTime))
            {
                var date = Convert.ToDateTime(createTime);
                exp.And(f => f.CreateTime.HasValue && f.CreateTime.Value.Year== date.Year && f.CreateTime.Value.Month==date.Month);
            }
            if (query.From == "daywork")
            {
                exp.And(f => SqlFunc.Subqueryable<Model.Live_Customer>().Where(s => s.CustomerGID == f.CustomerGID && s.AttendanceType==2).Any());
            }
            var models = await _accountReceivableService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        ///  收到付款
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="partnerName"></param>
        /// <param name="cooperationUnitName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> ReceivePayment([FromForm] Guid id, [FromForm]decimal paidAmount, [FromForm] string remark, [FromForm] DateTime? paidTime)
        {
            var model = await _accountReceivableService.GetNav(f => f.ID == id);
            Live_SalaryCustomer salaryModel = null;

            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }





            if (model.Status >= Model.Enums.AccountReceivableStatusEnumType.冲红)
            {
                throw new ErrorCodeException(-1, "此状态无法收款");
            }

            #region 获取工资数据

            if (model.InvoiceID!=Guid.Empty)
            {
               var invoice=await _invoiceService.Get(model.InvoiceID);
                if (invoice!=null&& invoice.SalaryCustomerID.HasValue&&invoice.SalaryCustomerID.Value!=Guid.Empty)
                {

                    var salaryCustomerModel = await _salaryCustomerService.Get(invoice.SalaryCustomerID.Value);
                    if (salaryCustomerModel!=null)
                    {
                        if (salaryCustomerModel.CombineCompanyGID != null && salaryCustomerModel.CombineCompanyGID.Value != Guid.Empty)
                        {
                            var combineSalaryList = await _salaryCustomerService.List(f => f.TaxMonth == salaryCustomerModel.TaxMonth && f.CombineCompanyGID != null && f.CombineCompanyGID.Value == salaryCustomerModel.CombineCompanyGID);

                            if (combineSalaryList != null && combineSalaryList.Count > 0)
                            {
                                foreach (var item in combineSalaryList)
                                {
                                    await _salaryCustomerService.SetCustomerPaid(item.ID);
                                }
                            }
                        }
                        else {

                            await _salaryCustomerService.SetCustomerPaid(invoice.SalaryCustomerID.Value);

                        }

                    }



                   //var salaryCustomer=await _salaryCustomerService.Get(invoice.SalaryCustomerID.Value);
                   // if (salaryCustomer!=null&& salaryCustomer.IsCustomerPaid==false)
                   // {
                   //     salaryModel=salaryCustomer;
                   //     //设置为客户已打款
                   //     salaryModel.IsCustomerPaid=true;
                   // }
                }
            }


            #endregion



            if (paidAmount<=0)
            {
                throw new ErrorCodeException(-1, "请输入正确的金额");
            }
            string userName = "";
            if(CurrentUser.PlatformType == PlatformType.MiniAdmin)
            {
                var info = await _authService.UserInfo<Model.SYS_CompanyUser>();
                userName = info.FullName;
            }
            Live_AccountReceivableRecord record = new Live_AccountReceivableRecord()
            {
                AccountReceivableID = id,
                ID = Guid.NewGuid(),
                ActionUserID = CurrentUser.UserID,
                ActionUserName = userName,
                CreateTime = DateTime.Now,
                PaidTime = paidTime.HasValue?paidTime: DateTime.Now,
                PaidAmount = paidAmount,
                Remark = remark??string.Empty,
                 
            };
            model.Records.Add(record);
            model.PaidAmount = model.Records.Sum(x => x.PaidAmount);
            model.PendingAmount = model.TotalAmount - model.PaidAmount;
            if (model.PendingAmount < 0)
            {
                model.PendingAmount = 0;
            }

            if (model.PendingAmount == 0)
            {
                model.Status = AccountReceivableStatusEnumType.收款完成;
            }
            else {
                model.Status = AccountReceivableStatusEnumType.收款中;
            }
            await _accountReceivableService.UpdateNav(model);

            if (salaryModel!=null)
            {
               await _salaryCustomerService.Update(salaryModel);
            }

            return Ok();
        }
        /// <summary>
        ///  删除收款金额
        /// </summary>
        /// <param name="id"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> DeleteRecord([FromForm] Guid id, [FromForm] Guid recordID)
        {
            var model = await _accountReceivableService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var record = model.Records.FirstOrDefault(f => f.ID == recordID);

            if (record != null) {
                model.Records.Remove(record);
            }
            model.PaidAmount = model.Records.Sum(x => x.PaidAmount);
            model.PendingAmount = model.TotalAmount - model.PaidAmount;
            if (model.PendingAmount < 0)
            {
                model.PendingAmount = 0;
            }
            await _accountReceivableService.UpdateNav(model);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Get( Guid id)
        {
            var model = await _accountReceivableService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }





        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var model = await _accountReceivableService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }

            await _accountReceivableService.DeleteNav(model);
            return Ok();
        }



        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm] Guid id, [FromForm] AccountReceivableStatusEnumType status)
        {
            var model = await _accountReceivableService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            model.Status= AccountReceivableStatusEnumType.冲红;



          var  redmodel = new Live_AccountReceivableExtension()
            {
                ID = Guid.NewGuid(),
                AnnexFileName = model.AnnexFileName,
                AnnexFilePath = model.AnnexFilePath,
                CompanyID = model.CompanyID,
                CooperationUnitGID = model.CooperationUnitGID,
                CooperationUnitName = model.CooperationUnitName,
                CreateTime = model.CreateTime,
                CustomerGID = model.CustomerGID,
                CustomerName = model.CustomerName,
                DataSourceType = model.DataSourceType,
                Description = model.Description,
                InvoiceID = model.InvoiceID,
                InvoiceTotalAmount = model.InvoiceTotalAmount * -1,
                IssuerName = model.IssuerName,
                LegalCustomerName = model.LegalCustomerName,
                PaidAmount = model.PaidAmount * -1,
                PartnerGID = model.PartnerGID,
                PartnerName = model.PartnerName,
                PendingAmount = model.PendingAmount * -1,
                RecipientName = model.RecipientName,
                SfWithholdAmount = model.SfWithholdAmount * -1,
                Status = AccountReceivableStatusEnumType.冲红_红字,
                TotalAmount = model.TotalAmount * -1,
                YlBillID = model.YlBillID,
                Records = model.Records?.Select(f => new Live_AccountReceivableRecord()
                {
                    ActionUserID = f.ActionUserID,
                    //AccountReceivableID=f.AccountReceivableID,
                    ActionUserName = f.ActionUserName,
                    CreateTime = f.CreateTime,
                    ID = Guid.NewGuid(),
                    PaidAmount = f.PaidAmount * -1,
                    PaidTime = f.PaidTime,
                    Remark = f.Remark,
                }).ToList(),


            };

            
            await _accountReceivableService.UpdateNav(model);
            await _accountReceivableService.AddNav(redmodel);
            return Ok();
        }


        #region 导入导出


        /// <summary>
        ///  下载模板
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public IActionResult DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<AccountReceivableExcelTemplate>(stream, "应收账款");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "应收账款模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }



        [HttpPost]
        public async Task<IActionResult> UploadAccountReceivable(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                List<Live_AccountReceivableExtension> receivableList = new List<Live_AccountReceivableExtension>();
                Live_AccountReceivableExtension receivable = null;
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<AccountReceivableExcelTemplate>(ms, string.Empty);
                if (excelData != null && excelData.Count > 0)
                {
                    foreach (var excel in excelData)
                    {
                        var res = await _otherService.GetCustomerByNameAsync(excel.CustomerName);
                        if (res == null || res.Code != 200)
                        {
                            throw new ErrorCodeException(-1, $"{excel.CustomerName} 单位信息获取失败");
                        }
                        receivable = new Live_AccountReceivableExtension()
                        {
                            CustomerName = excel.CustomerName,
                            CustomerGID = res.Data.CustomerGID.Value,
                            AnnexFileName = string.Empty,
                            AnnexFilePath = string.Empty,
                            CooperationUnitGID = res.Data.CooperationUnitGID.Value,
                            CooperationUnitName = res.Data.CooperationUnitName,
                            CreateTime = DateTime.Now,
                            DataSourceType = Model.Enums.AccountReceivableDataSourceTypeEnumType.导入,
                            Description = excel.Description,
                            ID = Guid.NewGuid(),
                            InvoiceID = Guid.Empty,
                            InvoiceTotalAmount = 0,
                            PaidAmount = 0,
                            PartnerGID = res.Data.PartnerGID.Value,
                            PartnerName = res.Data.PartnerName,
                            TotalAmount = excel.TotalAmount,
                            PendingAmount = 0,
                            SfWithholdAmount = 0,
                            Status = Model.Enums.AccountReceivableStatusEnumType.待收款,
                            YlBillID = Guid.Empty
                        };
                        receivableList.Add(receivable);
                    }
                }

                if (receivableList.Any())
                {
                    await _accountReceivableService.Add(receivableList);
                }
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(string customerName, string issuerName, string partnerName, string recipientName, AccountReceivableStatusEnumType? status, decimal? invoiceTotalAmount, string createTime, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_AccountReceivableExtension>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(issuerName), f => f.IssuerName.Contains(issuerName));
            exp.AndIF(invoiceTotalAmount.HasValue, f => f.InvoiceTotalAmount == invoiceTotalAmount);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(recipientName), f => f.RecipientName.Contains(recipientName));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            if (!string.IsNullOrEmpty(createTime))
            {
                var date = Convert.ToDateTime(createTime);
                exp.And(t => t.CreateTime.HasValue && t.CreateTime.Value.Year == date.Year && t.CreateTime.Value.Month == date.Month);
            }
            if (query.From == "daywork")
            {
                exp.And(f => SqlFunc.Subqueryable<Model.Live_Customer>().Where(s => s.CustomerGID == f.CustomerGID && s.AttendanceType == 2).Any());
            }
            var models = await _accountReceivableService.List(exp.ToExpression(), query);



            var result = _accountReceivableService.ExportExcel(models);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "应收账款.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        #endregion
    }
}
