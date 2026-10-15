using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class KnowledgeController(IKnowledgeService knowledgeService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.AI_Knowledge>();
            expable.And(it => it.CompanyID == user.CompanyID);
            var exp= expable.ToExpression();
            var models = await knowledgeService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<RequestResultModel> Index([FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.AI_Knowledge>();
            expable.And(it => it.CompanyID == 1);
            var exp = expable.ToExpression();
            var models = await knowledgeService.List(exp, query);
            var items = new List<Model.AI_Knowledge>();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                foreach (var model in models)
                {
                    if (!string.IsNullOrEmpty(model.Indexs))
                    {
                        var indexs = model.Indexs.Split(',');
                        foreach (var index in indexs)
                        {
                            if (query.Keyword.IndexOf(index) > -1)
                            {
                                items.Add(model);
                            }
                        }
                    }
                }
            }
            else
            {
                items = models;
            }

                var res = new RequestResultModel();
            res.Code = 200;
            res.Message = "查询成功！";
            res.Items= items;
            return res;
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Model.AI_Knowledge model)
        {
            var user = CurrentUser;
            model.CreateTime = DateTime.Now;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            await knowledgeService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.AI_Knowledge> Get(int id)
        {
            return await knowledgeService.Get(id);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.AI_Knowledge model)
        {
            await knowledgeService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await knowledgeService.Delete(id);
            return Ok();
        }
    }
}
