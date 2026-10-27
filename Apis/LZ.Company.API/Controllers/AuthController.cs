using LZ.Company.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Company.API.Controllers
{
    
    public class AuthController(IAuthService authService,ICompanyUserService companyUserService, IPermissionService permissionService, ICompanyConfigService companyConfigService) : CompanyControllerBase
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
           var model =await companyUserService.Login(userName, password);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "用户名或密码不正确");
            }
            if (!model.Enabled)
            {
                throw new ErrorCodeException(-1, "您已被管理员禁止登录");
            }
            await permissionService.DeleteCompanyRoles(model.UserID);
            var company = await companyConfigService.Get(model.CompanyGID);
            if (company == null || company.Company == null)
            {
                throw new ErrorCodeException(-1, "未找到该人力公司");
            }
            var shopId = model.ShopID.HasValue ? model.ShopID.Value : 0;
            var userType = model.UserType.HasValue ? model.UserType.Value : 0;
            var token = await authService.GenerateUserTokenAsync(model, model.UserID, Model.Enums.PlatformType.MiniAdmin,model.CompanyID.Value,model.CompanyGID,company.Company.CutType, fullName: model.FullName,shopId: shopId, userType: userType);
            return Ok(token.Token);
        }
        [HttpGet]
        public async Task<Model.SYS_CompanyUser> GetInfo()
        {
            var model = await companyUserService.Get(CurrentUser.UserID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            return model;
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
