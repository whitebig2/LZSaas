using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoicePartnerRelationController : InvoiceControllerBase
    {
        private readonly IInvoicePartnerRelationService _invoicePartnerRelationService;

        public InvoicePartnerRelationController(IInvoicePartnerRelationService invoicePartnerRelationService)
        {
            _invoicePartnerRelationService = invoicePartnerRelationService;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<FIN_InvoicePartnerRelation>();
            exp.AndIF(query.Enabled.HasValue, f => f.IsEnabled == query.Enabled);
            exp.AndIF(query.ParentID.HasValue, f => f.PartnerID == query.ParentID);
            var models = await _invoicePartnerRelationService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add(FIN_InvoicePartnerRelation model)
        {
            await _invoicePartnerRelationService.Add(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddList(List<FIN_InvoicePartnerRelation> models)
        {
            await _invoicePartnerRelationService.Add(models);
            return Ok();
        }

        [HttpGet]
        public async Task<FIN_InvoicePartnerRelation> Get(int id)
        {
            var model = await _invoicePartnerRelationService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FIN_InvoicePartnerRelation model)
        {
            await _invoicePartnerRelationService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _invoicePartnerRelationService.Delete(id);
            return Ok();
        }
    }
}
