using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LZ.Basic.Service.Services;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Core.Attributes;
using LZ.ApiService.Services;
using LZ.ApiService.Interface;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    public class ManageUserController(IManageUserService manageUserService, IPermissionService permissionService,IAuthService authService, IAliyunOSSService aliyunOSSService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.SYS_ManageUser>();
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
            var models = await manageUserService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var uid = CurrentUser.UserID;
            var roles = await permissionService.Roles(uid);
            var user = await manageUserService.Info(uid);
            return Ok(new { roles, info = new { name = user.FullName, gid=user.UserGID , picture = user.Picture, id = user.UserID } });
        }
        [HttpPost]
        [OperationLogAction("新增总后台账号：{model.FullName}")]
        public async Task<IActionResult> Add([FromBody] Model.SYS_ManageUser model)
        {
            if (await manageUserService.ExistName(model.UserName))
            {
                throw new ErrorCodeException(-1, "该用户名已存在!");
            }
            model.UserPassword = Common.Tool.Md5Psw32("111111");
            model.UserGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await manageUserService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.SYS_ManageUser> Get(int id)
        {
            return await manageUserService.Get(id);
            //return await manageUserService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        [OperationLogAction("修改总后台账号：{model.FullName}")]
        public async Task<IActionResult> Update([FromBody] Model.SYS_ManageUser model)
        {
            if (await manageUserService.ExistName(model.UserName,model.UserID))
            {
                throw new ErrorCodeException(-1, "该用户名已存在");
            }
            await manageUserService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await manageUserService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await manageUserService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await manageUserService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await manageUserService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        [OperationLogAction("删除账号：{id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await manageUserService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await manageUserService.Delete(id);
            return Ok();
        }
        [HttpPut]
        [OperationLogAction("修改密码")]
        public async Task<IActionResult> ChangePassword([FromForm] string ypassword, [FromForm] string npassword)
        {
           var count = await manageUserService.ChangePassword(CurrentUser.UserID, Common.Tool.Md5Psw32(ypassword), Common.Tool.Md5Psw32(npassword));
            if (count == 0)
            {
                throw new ErrorCodeException(-1, "原密码错误");
            }
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
                    var count = await manageUserService.ChangePicture(CurrentUser.UserID, res.Data.ToString());
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
