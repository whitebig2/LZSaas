using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Order.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senparc.Weixin.TenPay.V3;

namespace LZ.Order.API
{
    public class PayController(IOrderService orderService,ICompanyConfigService companyConfigService) : OrderControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> WeiXin()
        {
            var orderID = "";
            var xml = "";
            try
            {
                var resHandler = new ResponseHandler(null);
                var return_code = resHandler.GetParameter("return_code");
                var return_msg = resHandler.GetParameter("return_msg");
                orderID = resHandler.GetParameter("out_trade_no");
                xml = resHandler.ParseXML();
                var model = await orderService.Get(orderID);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "该订单不存在!");
                }
                if (model.OrderStatus != (int)OrderStatus.WaitPay)
                {
                    throw new ErrorCodeException(-1, "该订单或已付款!");
                }
                var company = await companyConfigService.Get(model.CompanyGID);
                var config = company.Config;
                resHandler.SetKey(config.WeiXinPaySecret);
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
                {
                    model.PayID = resHandler.GetParameter("transaction_id");
                    var has = await orderService.OrderPay(model);
                    if (has)
                    {
                        return Ok();
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "该订单或已付款!");
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, "支付解析错误!");
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error("微信支付回调错误：" + orderID + xml, ex);
                throw new ErrorCodeException(-1, "微支付回调错误!");
            }
        }
    }
}
