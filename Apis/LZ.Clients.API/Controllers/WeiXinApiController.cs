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

namespace LZ.Clients.API.Controllers
{
    public class WeiXinApiController(IClientsService clientsService,IWeiXinReplyService weiXinReplyService) : ClientsControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string signature, string timestamp, string nonce, string echostr, Guid id)
        {
            try
            {
                var model = await clientsService.Get(id);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "数据不存在");
                }
                if (CheckSignature.Check(signature, timestamp, nonce, model.Token))
                {
                    return Content(echostr);
                }
                else
                {
                    return Content("");
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error("", ex);
                throw new ErrorCodeException(-1, "登录错误");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string signature, string timestamp, string nonce, Guid id)
        {
            try
            {
                var clients = await clientsService.Get(id);
                if (clients == null)
                {
                    throw new ErrorCodeException(-1, "数据不存在");
                }
                if (CheckSignature.Check(signature, timestamp, nonce, clients.Token))
                {
                    var model = new PostModel();
                    model.Token = clients.Token;
                    model.AppId = clients.AppID;
                    model.Signature = signature;
                    model.Timestamp = timestamp;
                    model.Nonce = nonce;
                    var cancellationToken = new CancellationToken();
                    var messageHandler = new Handlers.CustomMessageHandlers(Request.Body, model, 10, clients);
                    await messageHandler.ExecuteAsync(cancellationToken);
                    return new FixWeixinBugWeixinResult(messageHandler);
                }
                else
                {
                    return Content("参数错误！");
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error("", ex);
                throw new ErrorCodeException(-1, "登录错误");
            }
        }
    }
}
