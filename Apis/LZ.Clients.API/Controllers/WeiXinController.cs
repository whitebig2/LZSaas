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
using Senparc.Weixin.AspNet.MvcExtension;
using Senparc.Weixin.MP;
using System.Drawing.Drawing2D;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.AdvancedAPIs;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using LZ.Model.Enums;

namespace LZ.Clients.API.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    public class WeiXinController(IClientsService clientsService,IAuthService authService, IEventPublisher publisher) : Controller
    {
        [AllowAnonymous]
        public async Task<IActionResult> BindMP(string id, string code)
        {
            return await BindWeiXinMP(id, code);
        }
        public async Task<IActionResult> BindUserPageMP(string id, string code)
        {
            return await BindWeiXinMP(id, code, "/pages/user/index");
        }
        private async Task<IActionResult> BindWeiXinMP(string id, string code,string toPage="")
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(code))
            {
                return NoContent();
            }
            var token = await authService.ValidateTokenAsync(id);
            if (token == null)
            {
                return NoContent();
            }
            var userId = 0;
            Model.Live_Clients client = new Model.Live_Clients();
            switch (token.PlatformType)
            {
                case (int)Model.Enums.PlatformType.XFZMini:
                    {
                        var user = await authService.UserInfo<Model.Live_User>(id);
                        if (user == null)
                        {
                            return NoContent();
                        }
                        userId = user.UserID;
                        client = await clientsService.Get(token.CompanyID.Value, "xfz", "mp");
                        if (client == null)
                        {
                            return NoContent();
                        }
                    }
                    break;
                case (int)Model.Enums.PlatformType.LZMini:
                    {
                        toPage = "/pages/user/index";
                        var user = await authService.UserInfo<Model.Live_Personnel>(id);
                        if (user == null)
                        {
                            return NoContent();
                        }
                        userId = user.UserID;
                        client = await clientsService.Get(0, "lz", "mp");
                        if (client == null)
                        {
                            return NoContent();
                        }
                    }
                    break;
                default:
                    return NoContent();
            }
            var result = OAuthApi.GetAccessToken(client.AppID, client.AppSecret, code, "authorization_code");
            var record = new UserOpenIDEventData();
            record.UserID = userId;
            record.ClientID = client.ID;
            record.OpenID = result.openid;
            record.OpenIdType = "mp";
            record.Platform = (Model.Enums.PlatformType)token.PlatformType;
            publisher.Publish(record);
            ViewBag.OpenID = result.openid;
            ViewBag.ToPage = toPage;
            return View();
        }
    }
}
