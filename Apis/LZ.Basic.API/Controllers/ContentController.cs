using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Runtime.InteropServices;

namespace LZ.Basic.API.Controllers
{
    public class ContentController(IContentService contentService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.SYS_V_Content>();
            expable.And(it => it.CompanyID == user.CompanyID);
            expable.AndIF(query.CID.HasValue, it => it.CID == query.CID);
            expable.AndIF(String.IsNullOrEmpty(query.Name), it => it.Title.Contains(query.Name));
            var exp= expable.ToExpression();
            var models = await contentService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Model.SYS_Content model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            await contentService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.SYS_Content> Get(int id)
        {
            return await contentService.Get(id);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SYS_Content model)
        {
            await contentService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await contentService.Delete(id);
            return Ok();
        }
    }
}
