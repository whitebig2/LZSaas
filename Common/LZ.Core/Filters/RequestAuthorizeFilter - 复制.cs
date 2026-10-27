using LZ.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using LZ.Model.Enums;
using LZ.Core.Attributes;
using Microsoft.IdentityModel.Tokens;
using LZ.Core.Interface;
using System.Net;
using LZ.Common.Extensions;

namespace LZ.Core.Filters
{
    /// <summary>
    /// 请求接口权限过滤器而AuthenticationHandler则是用户认证，token认证
    /// </summary>
    public class RequestAuthorizeFilter(IPermissionService permissionService) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 接口标记了[AllowAnonymous]，则不需要进行权限验证
            if (context.ActionDescriptor.EndpointMetadata.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)))
            {
                return;
            }
            // 其他需要登录验证的，则通过AuthenticationHandler进行用户认证
            if (context.HttpContext.User.Identity==null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status401Unauthorized, "请先登录", null));
                return;
            }
            try
            {
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (actionDescriptor != null)
                {
                    PlatformLimitAttribute? platformAttribute = null;
                    var classAttribute = context.ActionDescriptor.EndpointMetadata.FirstOrDefault(a => a.GetType() == typeof(PlatformLimitAttribute));
                    if (classAttribute != null)
                    {
                        platformAttribute = classAttribute as PlatformLimitAttribute;
                    }
                    if (platformAttribute==null)
                    {
                        platformAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<PlatformLimitAttribute>();
                    }
                    if (platformAttribute != null)//限制平台
                    {
                        var type = context.HttpContext.User.FindFirst(LZClaimTypes.PlatFormType);
                        if (type != null)
                        {
                            var ptype = (PlatformType)Convert.ToInt32(type.Value);
                            if (platformAttribute.platform != ptype)
                            {
                                context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status403Forbidden, "您无权访问", null));
                                return;
                            }
                        }
                       
                    }
                    var permissionAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<ManagePermissionAttribute>();//权限验证
                    if (permissionAttribute != null)
                    {
                        var uid = Convert.ToInt32(context.HttpContext.User.FindFirst(LZClaimTypes.UserId).Value);
                        var permissionOperateAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<PermissionOperateAttribute>();
                        var hasPermission = await permissionService.ManagePermission(permissionAttribute.permission, permissionOperateAttribute != null ? permissionOperateAttribute.operate : "", uid);
                        if (!hasPermission)
                        {
                            context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status403Forbidden, "您无权访问", null));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status403Forbidden, "来源错误，您无权访问", null));
                return;
            }
            
            
            await Task.CompletedTask;
        }
    }
}
