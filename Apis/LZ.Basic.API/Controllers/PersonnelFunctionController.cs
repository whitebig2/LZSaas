using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Runtime.InteropServices;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    public class PersonnelFunctionController(IPersonnelFunctionService personnelFunctionService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_PersonnelFunction>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it=>it.Name.Contains(query.Name));
            }
            var exp= expable.ToExpression();
            var models = await personnelFunctionService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> Items([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_PersonnelFunctionItems>();
            expable.AndIF(!string.IsNullOrEmpty(query.Type), it => it.UserType == Convert.ToInt32(query.Type));
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                if (query.CID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CID);
                }
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await personnelFunctionService.Items(exp);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Model.Live_PersonnelFunction model)
        {
            await personnelFunctionService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> UserTypeFunction([FromBody] List<Model.Live_PersonnelFunctionItems> list)
        {
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                foreach(var item in list)
                {
                    item.CompanyID = user.CompanyID;
                }
            }
                await personnelFunctionService.UserTypeFunction(list);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_PersonnelFunction> Get(int id)
        {
            var model = await personnelFunctionService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_PersonnelFunction model)
        {
            if(!await personnelFunctionService.Update(model))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await personnelFunctionService.Set(id, false))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await personnelFunctionService.Set(id, false))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await personnelFunctionService.Delete(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            return Ok();
        }
    }
}
