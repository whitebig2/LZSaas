using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceCompanyController : ApiControllerBase
    {

        private readonly IInvoiceCompanyService  _invoiceCompanyService;
        public InvoiceCompanyController(IInvoiceCompanyService invoiceCompanyService)
        {
            _invoiceCompanyService = invoiceCompanyService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string companyName, string companyNo, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_InvoiceCompany>();
            exp.AndIF(!string.IsNullOrWhiteSpace(companyName), f => f.CompanyName.Contains(companyName));
            exp.AndIF(!string.IsNullOrWhiteSpace(companyNo), f => f.CompanyNo.Contains(companyNo));
            var models = await _invoiceCompanyService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string companyName, string companyNo)
        {
            var exp = Expressionable.Create<Model.Live_InvoiceCompany>();
            exp.AndIF(!string.IsNullOrWhiteSpace(companyName), f => f.CompanyName.Contains(companyName));
            exp.AndIF(!string.IsNullOrWhiteSpace(companyNo), f => f.CompanyNo.Contains(companyNo));
            var models = await _invoiceCompanyService.List(exp.ToExpression());
            return Ok(new { items = models, count = models.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_InvoiceCompany model)
        {
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;

            if (await _invoiceCompanyService.Exist(f=>f.CompanyName==model.CompanyName))
            {
                throw new ErrorCodeException(-1, $"{model.CompanyName}信息已存在，无需重复添加");
            }


            await _invoiceCompanyService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InvoiceCompany> Get(Guid id)
        {
            var model = await _invoiceCompanyService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InvoiceCompany model)
        {
            await _invoiceCompanyService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _invoiceCompanyService.Delete(id);
            return Ok();
        }
    }
}
