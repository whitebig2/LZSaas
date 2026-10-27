using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Invoice.Service.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using SqlSugar;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceConfigController : InvoiceControllerBase
    {
        private readonly IInvoiceConfigService _invoiceConfigService;

        public InvoiceConfigController(IInvoiceConfigService invoiceConfigService)
        {
            _invoiceConfigService = invoiceConfigService;
            _authService = authService;
            _httpService = httpService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string title,string issuer, string recipient, Guid? recipientUnitID, Guid? issuerUnitID, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<FIN_InvoiceConfig>();
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin || user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                exp.And(f => f.CompanyID == user.CompanyID);
            }
            else
            {
                exp.AndIF(query.CompanyID.HasValue, f => f.CompanyID == query.CompanyID);
            }
            if(user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var per = await _authService.UserInfo<Model.Live_Personnel>();
                exp.And(it => it.PartnerID == per.PartnerID);
            }
            exp.AndIF(recipientUnitID.HasValue, f => f.RecipientUnitID == recipientUnitID);
            exp.AndIF(issuerUnitID.HasValue, f => f.IssuerUnitID == issuerUnitID);
            exp.AndIF(query.Enabled.HasValue, f => f.IsEnabled == query.Enabled);
            exp.AndIF(!string.IsNullOrWhiteSpace(title), f => f.Title.Contains(title));
            exp.AndIF(!string.IsNullOrWhiteSpace(issuer), f => f.IssuerName.Contains(issuer));
            exp.AndIF(!string.IsNullOrWhiteSpace(recipient), f => f.RecipientName.Contains(recipient));
            var models = await _invoiceConfigService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add(FIN_InvoiceConfig model)
        {
            if (await _invoiceConfigService.Exist(t=>t.IssuerUnitID==model.IssuerUnitID && t.RecipientUnitID == model.RecipientUnitID))
            {
                throw new ErrorCodeException(-1, "开票方和收票方相同的配置已存在");
            }
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.CreateTime = DateTime.Now;
            var per = await _authService.UserInfo<Model.Live_Personnel>();
            model.PartnerID = per.PartnerID;
            model.PartnerGID = per.PartnerGID;
            await _invoiceConfigService.Add(model);
            return Ok();
        }


        [HttpGet]
        public async Task<FIN_InvoiceConfig> Get(int id)
        {
            var model = await _invoiceConfigService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FIN_InvoiceConfig model)
        {
            await _invoiceConfigService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _invoiceConfigService.Delete(id);
            return Ok();
        }
    }
}
