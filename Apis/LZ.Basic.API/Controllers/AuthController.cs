using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using LZ.Core.Attributes;
using LZ.Basic.Service.Interface;
using LZ.Core.Services;

namespace LZ.Basic.API.Controllers
{
    
    public class AuthController(IAuthService authService,IManageUserService manageUserService, IPermissionService permissionService) : BasicControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [OperationLogAction("登录系统: {userName}")]
        public async Task<IActionResult> Login([FromForm] string userName, [FromForm] string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ErrorCodeException(-1, "用户名和密码不能为空");
            }
           var model =await manageUserService.Login(userName, password);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "用户名或密码不正确");
            }
            if (!model.Enabled)
            {
                throw new ErrorCodeException(-1, "您已被管理员禁止登录");
            }
            await permissionService.DeleteRoles(model.UserID);
            var token = await authService.GenerateUserTokenAsync(model, model.UserID, Model.Enums.PlatformType.Admin,fullName:model.FullName);
            return Ok(token.Token);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var user = CurrentUser;
            await authService.DeleteToken(user.Token);
            return Ok();
        }
    }
}
