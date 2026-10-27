using LZ.Core;
using LZ.Core.Interface;
using LZ.Invoice.Service.Interface;
using LZ.Invoice.Service.Services;
using LZ.Model;
using LZ.Model.Enums.Invoice;
using LZ.Model.Exceptions;
using LZ.Model.Options;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace LZ.Invoice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceInfoController(IInvoiceInfoService invoiceInfoService, IInvoiceCustomerService invoiceCustomerService, IOptions<BaiWangOption> options,IInvoiceOrderDetailService invoiceOrderDetailService) : InvoiceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List(string customerName, string taxNo, string applyNo, string buyerName, int? status, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<FIN_InvoiceInfo>();
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin || user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                exp.And(f => f.CompanyID == user.CompanyID);
            }
            else
            {
                exp.AndIF(query.CompanyID.HasValue, f => f.CompanyID == query.CompanyID);
            }

            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var per = await authService.UserInfo<Model.Live_Personnel>();
                exp.And(it => it.PartnerID == per.PartnerID);
            }


            exp.AndIF(recipientUnitID.HasValue, f => f.RecipientUnitID == recipientUnitID);
            exp.AndIF(issuerUnitID.HasValue, f => f.IssuerUnitID == issuerUnitID);
            exp.AndIF(!string.IsNullOrWhiteSpace(taxNo), f => f.TaxNo.Contains(taxNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(applyNo), f => f.ApplyNo.Contains(applyNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(buyerName), f => f.BuyerName.Contains(buyerName));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            var models = await invoiceInfoService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FIN_InvoiceInfo model)
        {
            if(!model.RecipientUnitID.HasValue || !model.IssuerUnitID.HasValue)
            {
                throw new ErrorCodeException(-1, "收票方或者开票方未传值");
            }
            if (!model.GoodsID.HasValue)
            {
                throw new ErrorCodeException(-1, "发票项目未传值");
            }
            var per = await authService.UserInfo<Model.Live_Personnel>();
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            model.OrderNo = "LB" + DateTime.Now.ToString("yyyyMMddHHmmss");
            model.OrderDateTime = DateTime.Now;
            model.CreateTime = DateTime.Now;
            model.InvoiceType = 1;
            model.InvoiceTerminalCode = options.Value.InvoiceTerminalCode;//开票终端/数电账号
            var list = await invoiceCustomerService.List(t=>t.CompanyID == user.CompanyID && (t.ID==model.IssuerUnitID || t.ID==model.RecipientUnitID));
            //购方信息
            var buyer = list.Find(t => t.ID == model.IssuerUnitID);
            if (buyer == null)
            {
                throw new ErrorCodeException(-1, "收票方没有信息");
            }
            model.BuyerName = buyer.CustomerName;
            model.BuyerTaxNo = buyer.CustomerNo;
            model.BuyerBankName = buyer.BankName;
            model.BuyerBankNumber = buyer.BankNo;
            if (!string.IsNullOrEmpty(buyer.Address))
            {
                model.BuyerAddress = buyer.Address;
            }
            if (!string.IsNullOrEmpty(buyer.Phone))
            {
                model.BuyerTelephone = buyer.Phone;
            }
            //销方消息
            var seller = list.Find(t => t.ID == model.RecipientUnitID);
            if (seller == null)
            {
                throw new ErrorCodeException(-1, "开票方没有信息");
            }
            model.TaxNo = seller.CustomerNo;
            model.SellerBankName = seller.BankNo;
            model.SellerBankNumber = seller.BankNo;
            model.SellerPhone = seller.Phone;
            if (!string.IsNullOrEmpty(seller.Address))
            {
                model.SellerAddress = seller.Address;
            }
            //项目信息
            var detail = await invoiceOrderDetailService.Get(t => t.ID == model.GoodsID);
            if (detail == null)
            {
                throw new ErrorCodeException(-1, "发票项目没有信息");
            }
            model.GoodsName = detail.GoodsName;
            model.GoodsCode = detail.GoodsCode;
            model.GoodsTaxRate = detail.GoodsTaxRate;
            //if (detail.Type == 1)
            //{
            //    model.goodsTotalPrice
            //}

            // 发票配置
            
            model.TaxNo = "338888888888SMB";//测试用

            //await invoiceInfoService.Add(model);
            return Ok();
        }

        [HttpGet]
        public async Task<FIN_InvoiceInfo> Get(Guid id)
        {
            var model = await invoiceInfoService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FIN_InvoiceInfo model)
        {
            await invoiceInfoService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await invoiceInfoService.Delete(id);
            return Ok();
        }
    }
}
