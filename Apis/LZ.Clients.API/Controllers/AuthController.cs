using LZ.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.Helpers;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using LZ.Core.Interface;
using LZ.Clients.Service.Interface;

namespace LZ.Clients.API.Controllers
{
    public class AuthController(IClientsService clientsService,IAuthService authService) : ClientsControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromQuery] WeiXinModel model)
        {
            if (!model.ClientID.HasValue)
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var client = await clientsService.Get(model.ClientID.Value);
            var wx = SnsApi.JsCode2Json(client.AppID, client.AppSecret, model.Code);
            if (wx.errcode == ReturnCode.请求成功)
            {
                model.OpenID = wx.openid;
                model.SessionKey = wx.session_key;
                model.UnionID = wx.unionid;
                await SessionContainer.UpdateSessionAsync(wx.openid, wx.openid, wx.session_key, wx.unionid);
                return Ok(model);
            }
            else
            {
                throw new ErrorCodeException(-1, "验证失败，原因："+ wx.errmsg);
            }
           
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSession([FromBody] WeiXinModel model)
        {
            if (model.ClientID.HasValue)
            {
                var client = await clientsService.Get(model.ClientID.Value);
                var wx = SnsApi.JsCode2Json(client.AppID, client.AppSecret, model.Code);
                if (wx.errcode == ReturnCode.请求成功)
                {
                    model.OpenID = wx.openid;
                    //model.SessionKey = wx.session_key;
                    model.UnionID = wx.unionid;
                    await SessionContainer.UpdateSessionAsync(wx.openid, wx.openid, wx.session_key, wx.unionid);
                    return Ok(model);
                }
                else
                {
                    throw new ErrorCodeException(-1, "验证失败，原因：" + wx.errmsg);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<Model.RequestResultModel> GetPhone([FromBody] WeiXinModel model)
        {
            if (model.ClientID.HasValue)
            {
                try
                {
                    var client = await clientsService.Get(model.ClientID.Value);
                    var phoneNumber = EncryptHelper.DecryptPhoneNumber(model.OpenID, model.Encrypted, model.Iv);
                    var phone = phoneNumber.purePhoneNumber;
                    return new Model.RequestResultModel() { Code = 200, Message = "获取成功", Data = phone };
                }
                catch (Exception ex)
                {
                    Common.StaticLog.Error(ex);
                    return new Model.RequestResultModel() { Code = 3,Message="系统获取手机发生错误" };
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
        }
    }
}
