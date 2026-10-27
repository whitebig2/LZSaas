using LZ.Users.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.ApiService.Interface;
using LZ.Model.PagedModel;

namespace LZ.Users.API.Controllers
{
    public class LaborController(ILaborService laborService, IPermissionService permissionService,IAuthService authService, IAliyunOSSService aliyunOSSService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_Labor>();
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(it => it.UserName.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.FullName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Phone))
            {
                expable.And(it => it.Phone == query.Phone);
            }
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled == query.Enabled);
            }
            var exp = expable.ToExpression();
            var models = await laborService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var uid = CurrentUser.UserID;
            var roles = await permissionService.Roles(uid);
            var user = await laborService.Info(uid);
            return Ok(new { roles, info = new { name = user.FullName, picture = user.Picture, id = user.UserID } });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_Labor model)
        {
            if (await laborService.ExistName(model.UserName))
            {
                throw new ErrorCodeException(-1, "该用户名已存在!");
            }
            model.UserGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await laborService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_Labor> Get()
        {
            return await authService.UserInfo<Model.Live_Labor>();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_Labor model)
        {
            if (!await laborService.Exist(model.UserID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await laborService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await laborService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await laborService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await laborService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await laborService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await laborService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await laborService.Delete(id);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ChangePicture([FromForm] IFormFile file)
        {
            if (file!=null)
            {
                var res =await aliyunOSSService.UploadImage(file);
                if (res.Code == 0)
                {
                    //return Ok(res.Message);
                    var count = await laborService.ChangePicture(CurrentUser.UserID, res.Data.ToString());
                    if (count == 0)
                    {
                        throw new ErrorCodeException(-1, "修改错误");
                    }
                    return Ok(res.Data.ToString());
                }
                else
                {
                    throw new ErrorCodeException(-1, res.Message);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请选择文件");
            }
        }
    }
}
