using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    public class MenuController(IFunctionMenuService functionMenuService) : BasicControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]MenuPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.SYS_FunctionMenu>();
            expable.AndIF(query.PID.HasValue, it => it.PID == query.PID);

            expable.AndIF(query.IsPublic.HasValue, it => it.IsPublic == query.IsPublic);

            expable.AndIF(query.Enabled.HasValue, it => it.Enabled == query.Enabled);

            if (user.PlatformType!= Model.Enums.PlatformType.Admin)
            {
                expable.And(it=> SqlFunc.Subqueryable<Model.SYS_CompanyFunction>().Where(s => s.CompanyID==user.CompanyID && s.FGID==it.FGID).Any());
                expable.And(it=>it.Enabled);
            }
           
            var exp= expable.ToExpression();
            var models = await functionMenuService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<List<Model.SYS_FunctionMenu>> UserMenu()
        {
            var user = CurrentUser;

            return await functionMenuService.UserMenu(user.UserID);
        }
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<List<Model.SYS_FunctionMenu>> CompanyUserMenu()
        {
            var user = CurrentUser;

            return await functionMenuService.CompanyUserMenu(user.UserID, user.CompanyID);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Model.SYS_FunctionMenu model)
        {
            model.FGID=Guid.NewGuid();
            await functionMenuService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.SYS_FunctionMenu> Get(int id)
        {
            if (!await functionMenuService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return await functionMenuService.Get(id);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SYS_FunctionMenu model)
        {
            if(!await functionMenuService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await functionMenuService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm]int id)
        {
            if (!await functionMenuService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await functionMenuService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm]int id)
        {
            if (!await functionMenuService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await functionMenuService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]int id)
        {
            if (!await functionMenuService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await functionMenuService.Delete(id);
            return Ok();
        }
    }
}
