using Microsoft.AspNetCore.Mvc;
using LZ.Core;
using LZ.Model.Exceptions;
using SqlSugar;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using LZ.Core.Interface;
using LZ.Model.PagedModel;
using LZ.Clients.Service.Interface;
using LZ.Clients.Service;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using System.Drawing.Drawing2D;
using LZ.Clients.Service.Services;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using LZ.Model.Enums;
using LZ.Core.Attributes;

namespace LZ.Clients.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class ClientNoticeTemplateController(IClientNoticeTemplateService clientNoticeTemplateService, IEventPublisher publisher) : ClientsControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] TempPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_ClientNoticeTemplate>();
            expable.AndIF(query.CID.HasValue, it => it.ClientID == query.CID);
            expable.AndIF(query.NoticeID.HasValue, it => it.NoticeID == query.NoticeID);
            if (!string.IsNullOrEmpty(query.NoticeIDS))
            {
                var ids = query.NoticeIDS.Split(',');
                expable.And(it=>ids.Contains(it.NoticeID.ToString()));
            }
            if (user.PlatformType == PlatformType.XFZMini)
            {
                var clientId = HttpContext.Request.Headers["client-id"].ToString();
                if (string.IsNullOrEmpty(clientId))
                {
                    throw new ErrorCodeException(-1, "你无权读取数据");
                }
                expable.And(it => it.ClientID == Convert.ToInt32(clientId));
            }
            var exp = expable.ToExpression();
            var models = await clientNoticeTemplateService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_ClientNoticeTemplate model)
        {
            var count = await clientNoticeTemplateService.Add(model);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendMessage()
        {
            var record = new SendNoticeEventData();
            record.Platform = Model.Enums.PlatformType.XFZMini;
            record.SendType = Model.Enums.NoticeSendEnumType.All;
            record.NoticeType = Model.Enums.NoticeEnumType.UserPayslip;
            record.Keyword1 = "2024-12";
            record.Keyword2 = "12月份工资条已生成";
            record.UserID = 3;
            publisher.Publish(record);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> SynchroTemplate([FromForm]int clientId, [FromForm] string clientType)
        {
            if (await clientNoticeTemplateService.Exist(clientId))
            {
                throw new ErrorCodeException(-1, "该应用已存在订阅消息，不可同步！");
            }
            var record = new NoticeTempEventData();
            record.ClientID = clientId;
            record.Type = clientType;
            publisher.Publish(record);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_ClientNoticeTemplate> Get(int id)
        {
            var model = await clientNoticeTemplateService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_ClientNoticeTemplate model)
        {
           var has = await clientNoticeTemplateService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await clientNoticeTemplateService.Delete(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            return Ok();
        }
    }
}
