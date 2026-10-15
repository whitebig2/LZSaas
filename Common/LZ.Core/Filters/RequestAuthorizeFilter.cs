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
                var userType = context.HttpContext.User.FindFirst(LZClaimTypes.PlatFormType);
                if (userType != null)
                {
                    var ptype = (PlatformType)Convert.ToInt32(userType.Value);
                    if (ptype == PlatformType.Api)
                    {
                        await Task.CompletedTask;
                        return;
                    }
                }
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (actionDescriptor != null)
                {
                    var classAttributes = context.ActionDescriptor.EndpointMetadata.Where(a => a.GetType() == typeof(PlatformLimitAttribute));
                    if (classAttributes.Count()>0)//类的平台限制
                    {
                        List<PlatformLimitAttribute> platformAttributes = new List<PlatformLimitAttribute>();
                        foreach (var a in classAttributes)
                        {
                            var ap = (PlatformLimitAttribute)a;
                            platformAttributes.Add(ap);
                        }
                        if (!platformAttributes.Exists(it => it.platform == PlatformType.All))
                        {
                            if (platformAttributes != null && platformAttributes.Count > 0)//限制平台
                            {
                                var type = context.HttpContext.User.FindFirst(LZClaimTypes.PlatFormType);
                                if (type != null)
                                {
                                    var ptype = (PlatformType)Convert.ToInt32(type.Value);
                                    var any = platformAttributes.Any(it => it.platform == ptype);
                                    if (!any)
                                    {
                                        context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status403Forbidden, "您无权访问类", null));
                                        return;
                                    }
                                }
                            }
                        }
                         
                    }
                    var attributes = actionDescriptor.MethodInfo.GetCustomAttributes(true);
                    if (attributes.Count() > 0)//方法的平台限制
                    {
                        var methodAttributes = attributes.Where(a => a.GetType() == typeof(PlatformLimitAttribute));
                        if (methodAttributes.Count() > 0)
                        {
                            List<PlatformLimitAttribute> platformAttributes = new List<PlatformLimitAttribute>();
                            foreach(var a in methodAttributes)
                            {
                                var ap = (PlatformLimitAttribute)a;
                                platformAttributes.Add(ap);
                            }
                            if(!platformAttributes.Exists(it=>it.platform== PlatformType.All))
                            {
                                var type = context.HttpContext.User.FindFirst(LZClaimTypes.PlatFormType);
                                if (type != null)
                                {
                                    var ptype = (PlatformType)Convert.ToInt32(type.Value);
                                    var any = platformAttributes.Any(it => it.platform == ptype);
                                    if (!any)
                                    {
                                        context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status403Forbidden, "您无权访问方法", null));
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    
                    var permissionAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<ManagePermissionAttribute>();//权限验证
                    if (permissionAttribute != null)
                    {
                        var uid = Convert.ToInt32(context.HttpContext.User.FindFirst(LZClaimTypes.UserId).Value);
                        var type = context.HttpContext.User.FindFirst(LZClaimTypes.PlatFormType);
                        if (type != null)
                        {
                            var ptype = (PlatformType)Convert.ToInt32(type.Value);
                            var permissionOperateAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<PermissionOperateAttribute>();
                            var hasPermission = await permissionService.ManagePermission(permissionAttribute.permission, permissionOperateAttribute != null ? permissionOperateAttribute.operate : "", uid, ptype);
                            if (!hasPermission)
                            {
                                context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status403Forbidden, "您无管理权限，您无权访问", null));
                                return;
                            }
                        }
                        else
                        {
                            context.Result = new RequestJsonResult(new RequestResultModel(StatusCodes.Status403Forbidden, "您无管理权限，您无权访问", null));
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
