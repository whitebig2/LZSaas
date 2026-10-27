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
    public class InvoiceConfigController : ApiControllerBase
    {

        private readonly IInvoiceConfigService  _invoiceConfigService;

        public InvoiceConfigController(IInvoiceConfigService invoiceConfigService)
        {
            _invoiceConfigService = invoiceConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string title,string customerName,[FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_InvoiceConfig>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(title), f => f.Title.Contains(title));
            var models = await _invoiceConfigService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_InvoiceConfig model)
        {
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await _invoiceConfigService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InvoiceConfig> Get(Guid id)
        {
            var model = await _invoiceConfigService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InvoiceConfig model)
        {
            await _invoiceConfigService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _invoiceConfigService.Delete(id);
            return Ok();
        }
    }
}
