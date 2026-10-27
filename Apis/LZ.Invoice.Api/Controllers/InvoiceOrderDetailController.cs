using LZ.Core;
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
    public class InvoiceOrderDetailController : InvoiceControllerBase
    {
        private readonly IInvoiceOrderDetailService _invoiceOrderDetailService;

        public InvoiceOrderDetailController(IInvoiceOrderDetailService invoiceOrderDetailService)
        {
            _invoiceOrderDetailService = invoiceOrderDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string goodsName, string goodsCode, int? type,string ids, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<FIN_InvoiceOrderDetail>();
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin || user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                exp.And(f => f.CompanyID == user.CompanyID);
            }
            else
            {
                exp.AndIF(query.CompanyID.HasValue, f => f.CompanyID == query.CompanyID);
            }
            if (!string.IsNullOrEmpty(ids))
            {
                var arr = ids.Split(',');
                exp.And(f => arr.Contains(f.ID.ToString()));
            }
            exp.AndIF(query.Enabled.HasValue, f => f.IsEnabled == query.Enabled);
            exp.AndIF(!string.IsNullOrWhiteSpace(goodsName), f => f.GoodsName.Contains(goodsName));
            exp.AndIF(!string.IsNullOrWhiteSpace(goodsCode), f => f.GoodsCode.Contains(goodsCode));
            exp.AndIF(type.HasValue, f => f.Type == type.Value);
            var models = await _invoiceOrderDetailService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add(FIN_InvoiceOrderDetail model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            await _invoiceOrderDetailService.Add(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddList(List<FIN_InvoiceOrderDetail> models)
        {
            await _invoiceOrderDetailService.Add(models);
            return Ok();
        }

        [HttpGet]
        public async Task<FIN_InvoiceOrderDetail> Get(int id)
        {
            var model = await _invoiceOrderDetailService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FIN_InvoiceOrderDetail model)
        {
            await _invoiceOrderDetailService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _invoiceOrderDetailService.Delete(id);
            return Ok();
        }
    }
}
