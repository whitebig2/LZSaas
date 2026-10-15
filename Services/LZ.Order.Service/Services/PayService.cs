using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Order.Service.Interface;
using Senparc.Weixin.TenPay;
using Senparc.Weixin.TenPay.V3;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sec;


namespace LZ.Order.Service.Services
{
    public class PayService(ICompanyConfigService companyConfigService,ISettingsService settingsService) : ServicesBase<Model.Shop_Order>, IPayService, IScopedDependency
    {
        public async Task<Model.RequestResultModel> WeiXinPay(Shop_Order model, string appId="")
        {
            try
            {
                var timeStamp = TenPayV3Util.GetTimestamp();
                var nonceStr = TenPayV3Util.GetNoncestr();
                var pmoney = Convert.ToInt32(model.Cash * 100);
                var company = await companyConfigService.Get(model.CompanyGID);
                var config = company.Config;
                var settings = await settingsService.GetMasterSettings();
                var WXAppId =string.IsNullOrEmpty(appId)? config.WeiXinOpenAppId: appId;
                var WXPayMchId = config.WeiXinMerchantID;
                var WXPayKey = config.WeiXinPaySecret;
                var xmlDatainfo = new TenPayV3UnifiedorderRequestData(WXAppId, WXPayMchId, model.OrderID, model.OrderID, pmoney, "127.0.0.1", settings.ApiServicesUrl + "/api/order/pay/weixin", TenPayV3Type.JSAPI, model.OpenID, WXPayKey, nonceStr);
                var payResult = TenPayV3.Unifiedorder(xmlDatainfo);
                var package = string.Format("prepay_id={0}", payResult.prepay_id);
                var paySign = TenPayV3.GetJsPaySign(WXAppId, timeStamp, nonceStr, package, WXPayKey);
                return new Model.RequestResultModel() { Code = 0, Message = "生成成功！", Data = new { appId = WXAppId, timeStamp = timeStamp, nonceStr = nonceStr, package = package, signType = "MD5", paySign = paySign } };
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new Model.RequestResultModel() { Code = -1, Message = "微信支付错误" };
            }
        }
        public async Task<bool> WeiXinRefund(Shop_Order model)
        {
            try
            {
                var timeStamp = TenPayV3Util.GetTimestamp();
                var nonceStr = TenPayV3Util.GetNoncestr();
                var remoney = Convert.ToInt32(model.RefundMoney * 100);
                var ordermoney = Convert.ToInt32(model.Cash * 100);
                var company = await companyConfigService.Get(model.CompanyGID);
                var config = company.Config;
                var settings = await settingsService.GetMasterSettings();
                var WXAppId = config.WeiXinOpenAppId;
                var WXPayMchId = config.WeiXinMerchantID;
                var WXPayKey = config.WeiXinPaySecret;
                TenPayV3RefundRequestData data = new TenPayV3RefundRequestData(WXAppId, WXPayMchId, WXPayKey, "", nonceStr, model.PayID, model.OrderID, model.RefundID, ordermoney, remoney, WXPayMchId, "");
                RefundResult payresult = await TenPayV3.RefundAsync(Senparc.Weixin.CommonDI.CommonSP, data);
                if (payresult.return_code == "SUCCESS")
                {
                    if (!string.IsNullOrEmpty(payresult.err_code))
                    {
                        Common.StaticLog.Error("订单号：" + model.OrderID + "退单失败，错误：" + payresult.err_code_des);
                        throw new ErrorCodeException(-1, "退单失败！");
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Common.StaticLog.Error("订单号：" + model.OrderID + "退单失败，错误：" + payresult.return_msg);
                    throw new ErrorCodeException(-1, "微信退单失败！");
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "微信退单错误！");
            }
        }
    }
}
