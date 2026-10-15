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
using LZ.Core.Attributes;

namespace LZ.Clients.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    public class NoticeTemplateController(INoticeTemplateService noticeTemplateService) : ClientsControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_NoticeTemplate>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            var exp = expable.ToExpression();
            var models = await noticeTemplateService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_NoticeTemplate model)
        {
            var count = await noticeTemplateService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_NoticeTemplate> Get(int id)
        {
            var model = await noticeTemplateService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_NoticeTemplate model)
        {
           var has = await noticeTemplateService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await noticeTemplateService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await noticeTemplateService.Delete(id);
            return Ok();
        }
    }
}
