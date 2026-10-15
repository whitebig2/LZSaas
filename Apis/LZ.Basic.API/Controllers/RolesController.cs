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
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class RolesController(IRolesService rolesService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.SYS_Roles>();
            expable.And(it => it.CompanyID == user.CompanyID);
            var exp= expable.ToExpression();
            var models = await rolesService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<List<SYS_RolesFunciton>> Menu(Guid id)
        {
            return await rolesService.Menu(id);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Model.SYS_Roles model)
        {
            var user = CurrentUser;
            model.RGID=Guid.NewGuid();
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            await rolesService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.SYS_Roles> Get(Guid id)
        {
            if (!await rolesService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return await rolesService.Get(id);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SYS_Roles model)
        {
            if(!await rolesService.Exist(model.RGID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await rolesService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] Guid id)
        {
            if (!await rolesService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await rolesService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] Guid id)
        {
            if (!await rolesService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await rolesService.Set(id, false);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> SetMenu([FromBody] List<Model.SYS_RolesFunciton> models)
        {
            if (models.Count==0)
            {
                throw new ErrorCodeException(-1, "数据不能为空");
            }
            await rolesService.SetMenu(models);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await rolesService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await rolesService.Delete(id);
            return Ok();
        }
    }
}
