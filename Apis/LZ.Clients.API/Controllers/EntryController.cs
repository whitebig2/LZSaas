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
    public class EntryController(IClientsService clientsService,IAuthService authService, IEventPublisher publisher) : Controller
    {
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
