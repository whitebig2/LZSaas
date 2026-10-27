using LZ.Clients.API;
using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    public class CustomerServiceChargeController(ICustomerServiceChargeService customerServiceChargeService): FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] CustomerPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_V_CustomerServiceCharge>();
            SetSearch(expable, query);
            var exp = expable.ToExpression();
            var models = await customerServiceChargeService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        private void SetSearch(Expressionable<Live_V_CustomerServiceCharge> expable, CustomerPagedModel query)
        {
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => it.CustomerName.Contains(query.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.InvoiceName))
            {
                expable.And(it => it.InvoiceCustomerName.Contains(query.InvoiceName));
            }
            if (!string.IsNullOrEmpty(query.ReceiptName))
            {
                expable.And(it => it.ReceiptCustomerName == query.ReceiptName);
            }
            if (query.SalaryMonth.HasValue)
            {
                expable.And(it => it.SalaryMonth.Value.ToString("yyyy-MM-dd") == query.SalaryMonth.Value.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(query.TimeType) && (query.StartTime.HasValue || query.EndTime.HasValue))
            {
                var endTime = query.EndTime.HasValue ? query.EndTime.Value.AddDays(1) : query.EndTime;
                switch (query.TimeType)
                {
                    case "invoice":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.InvoiceDate >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.InvoiceDate < endTime);
                        }
                        break;
                    case "send":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.SendDate >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.SendDate < endTime);
                        }
                        break;
                }
            }
        }
        [HttpGet]
        [OperationLogAction("导出客户服务费")]
        public async Task<IActionResult> ExportExcel([FromQuery] CustomerPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_CustomerServiceCharge>();
            SetSearch(expable, query);
            var exp = expable.ToExpression();
            query.Page = 0;
            var models = await customerServiceChargeService.List(exp, query);
            var excelData = models.Select(it => new CustomerServiceChargeExcelTemplate()
            {
                ReceiptCustomerName = it.ReceiptCustomerName,
                CustomerName = it.CustomerName,
                InvoiceCustomerName = it.InvoiceCustomerName,
                SalaryMonth = it.SalaryMonth.HasValue?it.SalaryMonth.Value.ToString("yyyy-MM"):"",
                InvoiceDate = it.InvoiceDate.HasValue ? it.InvoiceDate.Value.ToString("yyyy-MM-dd") : "",
                SendDate = it.SendDate.HasValue ? it.SendDate.Value.ToString("yyyy-MM-dd") : "",
                InvoiceAmount = it.InvoiceAmount,
                SalaryVAT = it.SalaryVAT,
                SalarySurtax = it.SalarySurtax,
                ServiceCharge = it.ServiceCharge,
                ServiceVAT = it.ServiceVAT,
                ServiceSurtax = it.ServiceSurtax,
                SocialAmount = it.SocialAmount,
                PeopleCount = it.PeopleCount,
                TotalAmount = it.TotalAmount,
                IsPredict = it.IsPredict ?"是": "否"
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(excelData, "客户服务费");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = DateTime.Now.ToString("yyyy-MM-dd") + "客户服务费", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        [HttpGet]
        public async Task<Model.Live_CustomerServiceCharge> Statistics([FromQuery] CustomerPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_V_CustomerServiceCharge>();
            SetSearch(expable, query);
            var exp = expable.ToExpression();
            return await customerServiceChargeService.Statistics(exp);
        }
        [HttpGet]
        public async Task<Model.Live_V_CustomerServiceCharge> Get(int id)
        {
            return await customerServiceChargeService.Get(it => it.ID == id);
        }
        [HttpGet]
        public async Task<IActionResult> BatchTest(DateTime? date=null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            await customerServiceChargeService.Batch(Convert.ToDateTime(date.Value.ToString("yyyy-MM-01")));
            return Ok();
        }
    }
}
