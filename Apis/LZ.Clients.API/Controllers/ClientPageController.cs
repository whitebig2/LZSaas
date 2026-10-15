using Microsoft.AspNetCore.Mvc;
using LZ.Model.Exceptions;
using SqlSugar;
using Microsoft.AspNetCore.Authorization;
using LZ.Model.PagedModel;
using LZ.Clients.Service.Interface;
using LZ.Core.Attributes;

namespace LZ.Clients.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class ClientPageController(IClientPageService clientPageService,IClientMyTemplateService clientMyTemplateService,IClientsService clientsService) : ClientsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user= CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_ClientPage>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.ClientID == query.CID);
            }
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await clientPageService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_ClientPage model)
        {
            var user = CurrentUser;
            if (!model.TempID.HasValue)
            {
                var client = await clientsService.Get(user.CompanyID, "xfz", "mini");
                if (client == null)
                {
                    throw new ErrorCodeException(-1, "该对应的客户端不存在");
                }
                var temp = await clientMyTemplateService.GetDefault(client.ID);
                if (temp == null)
                {
                    throw new ErrorCodeException(-1, "该对应的模板不存在");
                }
                model.TempID = temp.ID;
                model.ClientID = client.ID;
            }
            var count = await clientPageService.Add(model);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_ClientPage> GetPage(int id, string type)
        {
            var model = await clientPageService.GetPage(id, type);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该页面数据不存在");
            }
            return model;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_ClientPage> Get(int id)
        {
            var model = await clientPageService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_V_ClientPage> GetView(int id)
        {
            var model = await clientPageService.GetView(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_ClientPage model)
        {
            var m = await clientPageService.Get(model.ID);
            if (m == null)
            {
                throw new ErrorCodeException(-1, "该页面数据不存在");
            }
            if (!string.IsNullOrEmpty(model.Moulds))
            {
                m.Moulds = model.Moulds;
            }
            if (!string.IsNullOrEmpty(model.PageType))
            {
                m.PageType = model.PageType;
            }
            if (!string.IsNullOrEmpty(model.Title))
            {
                m.Title = model.Title;
            }
            if (!string.IsNullOrEmpty(model.Style))
            {
                m.Style = model.Style;
            }
            var has = await clientPageService.Update(m);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await clientPageService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await clientPageService.Delete(id);
            return Ok();
        }
    }
}
