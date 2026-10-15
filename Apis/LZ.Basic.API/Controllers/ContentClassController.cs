using LZ.Basic.Service.Interface;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    public class ContentClassController(IContentClassService contentClassService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.SYS_ContentClass>();
            expable.And(it => it.CompanyID == user.CompanyID);
            expable.AndIF(query.ParentID.HasValue, it => it.PID == query.ParentID);
            expable.AndIF(String.IsNullOrEmpty(query.Name), it => it.Name.Contains(query.Name));
            var exp= expable.ToExpression();
            var models = await contentClassService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Model.SYS_ContentClass model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            await contentClassService.Add(model);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.SYS_ContentClass> Get(int id)
        {
            return await contentClassService.Get(id);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SYS_ContentClass model)
        {
            await contentClassService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await contentClassService.Delete(id);
            return Ok();
        }
    }
}
