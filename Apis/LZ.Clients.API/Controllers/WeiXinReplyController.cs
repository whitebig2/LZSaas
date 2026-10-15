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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using System.Drawing.Drawing2D;

namespace LZ.Clients.API
{
    public class WeiXinReplyController(IWeiXinReplyService weiXinReplyService) : ClientsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_WeiXinReply>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.ClientID == query.CID);
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                expable.And(it => it.ReplyType == Convert.ToInt32(query.Type));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            var exp = expable.ToExpression();
            var models = await weiXinReplyService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_WeiXinReply model)
        {
            var count = await weiXinReplyService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_WeiXinReply> GetType(int clientId,int type)
        {
            return await weiXinReplyService.Get(type,clientId);
        }
        [HttpGet]
        public async Task<Model.Live_WeiXinReply> Get(int id)
        {
            var model = await weiXinReplyService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_WeiXinReply model)
        {
           var has = await weiXinReplyService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await weiXinReplyService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await weiXinReplyService.Delete(id);
            return Ok();
        }
    }
}
