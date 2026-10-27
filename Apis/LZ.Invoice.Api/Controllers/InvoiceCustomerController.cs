using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Invoice.Service.Interface;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceCustomerController(IInvoiceCustomerService invoiceCustomerService,IAuthService authService) : InvoiceControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PageList(string companyName, string companyNo, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.FIN_InvoiceCustomer>();
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin || user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                exp.And(f => f.CompanyID == user.CompanyID);
            }
            else
            {
                exp.AndIF(query.CompanyID.HasValue, f => f.CompanyID == query.CompanyID);
            }
            //小程序
            

            exp.AndIF(query.Enabled.HasValue, f => f.IsEnabled == query.Enabled);
            exp.AndIF(!string.IsNullOrWhiteSpace(companyName), f => f.CustomerName.Contains(companyName));
            exp.AndIF(!string.IsNullOrWhiteSpace(companyNo), f => f.CustomerNo.Contains(companyNo));
            var models = await invoiceCustomerService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string companyName, string companyNo, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.FIN_InvoiceCustomer>();
            if (user.PlatformType != Model.Enums.PlatformType.MiniAdmin || user.PlatformType != Model.Enums.PlatformType.LZMini)
            {
                exp.And(f => f.CompanyID == user.CompanyID);
            }
            else
            {
                exp.AndIF(query.CompanyID.HasValue, f => f.CompanyID == query.CompanyID);
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var info = await authService.UserInfo<Model.Live_Partner>();
                exp.And(f => SqlFunc.Subqueryable<Model.FIN_InvoicePartnerRelation>().Where(s => s.RecipientID == f.ID && s.PartnerID == info.PartnerID).Any());
            }
            exp.AndIF(!string.IsNullOrWhiteSpace(companyName), f => f.CustomerName.Contains(companyName));
            exp.AndIF(!string.IsNullOrWhiteSpace(companyNo), f => f.CustomerNo.Contains(companyNo));
            var models = await invoiceCustomerService.List(exp.ToExpression());
            return Ok(new { items = models, count = models.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add(Model.FIN_InvoiceCustomer model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;

            if (await invoiceCustomerService.Exist(f => f.CustomerName == model.CustomerName && f.CompanyID == user.CompanyID))
            {
                throw new ErrorCodeException(-1, $"{model.CustomerName}信息已存在，无需重复添加");
            }

            await invoiceCustomerService.Add(model);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.FIN_InvoiceCustomer> Get(Guid id)
        {
            var model = await invoiceCustomerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.FIN_InvoiceCustomer model)
        {
            await invoiceCustomerService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await invoiceCustomerService.Delete(id);
            return Ok();
        }
    }
}
