using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Common;
using LZ.Core.EventData;
namespace LZ.Core.Filters
{
    /// <summary>
    /// 异步请求结果过滤器
    /// </summary>
    public class OperationLogActionFilter(IEventPublisher publisher, ICurrentUser currentUser, IAuthService authService) : IAsyncActionFilter
    {
        /// <summary>
        /// 执行时机可通过代码中的的位置（await next();）来分辨
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                var logAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<OperationLogActionAttribute>();
                if (logAttribute != null)
                {
                    //前端传递的接口参数
                    var json = JsonConvert.SerializeObject(context.ActionArguments);
                    string logMessage = null;
                    logMessage = logAttribute.MessageTemplate;
                    if (logMessage is not null)
                    {
                        CreateOperationLogContent(json, ref logMessage);
                    }
                    var namespaceStr = actionDescriptor.ControllerTypeInfo.Namespace;
                    var controllerName = actionDescriptor.ControllerName;
                    var actionName = actionDescriptor.ActionName;
                    var method = context.HttpContext.Request.Method;
                    var code = $"{namespaceStr}.{controllerName}.{actionName}";
                    var log = new OperationLogEventData();
                    log.Code = code;
                    log.Content = logMessage;
                    log.Json = json;
                    log.IpAddress = context.HttpContext.Request.GetRemoteIpAddress();
                    log.UserAgent = context.HttpContext.Request.Headers.UserAgent;
                    log.Method = method;
                    log.UserID = currentUser.UserID;
                    log.Platform = (int)currentUser.PlatformType;
                    log.CompanyID = currentUser.CompanyID;
                    log.FullName = currentUser.FullName;
                    if (string.IsNullOrEmpty(currentUser.FullName) && currentUser.UserID > 0)
                    {
                        switch (currentUser.PlatformType)
                        {
                            case Model.Enums.PlatformType.Admin:
                                {
                                    var user = await authService.UserInfo<Model.SYS_ManageUser>();
                                    if (user != null)
                                    {
                                        log.FullName = user.FullName;
                                    }
                                    break;
                                }
                            case Model.Enums.PlatformType.MiniAdmin:
                                {
                                    var user = await authService.UserInfo<Model.SYS_CompanyUser>();
                                    if (user != null)
                                    {
                                        log.FullName = user.FullName;
                                    }
                                    break;
                                }
                            case Model.Enums.PlatformType.XFZMini:
                                {
                                    var user = await authService.UserInfo<Model.Live_User>();
                                    if (user != null)
                                    {
                                        log.FullName = user.FullName;
                                    }
                                    break;
                                }
                            case Model.Enums.PlatformType.LZMini:
                                {
                                    var user = await authService.UserInfo<Model.Live_Personnel>();
                                    if (user != null)
                                    {
                                        log.FullName = user.FullName;
                                    }
                                    break;
                                }
                        }
                    }
                    // 待处理发布事件
                    publisher.Publish(log);
                }
                await next();
            }
        }

        /// <summary>
        /// 根据OperationLogActionAttribute和参数生成操作日志的内容
        /// </summary>
        /// <param name="json"></param>
        /// <param name="logMessage"></param>
        private void CreateOperationLogContent(string json, ref string logMessage)
        {
            JObject jObject = JObject.Parse(json);

            // 正则提取参数
            Regex regex = new Regex(@"\{([\w\.]+)\}");
            MatchCollection matches = regex.Matches(logMessage);

            // 替换
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Groups[1].Value);
                var key = match.Groups[1].Value;
                logMessage = logMessage.Replace($"{{{key}}}", jObject.SelectToken(key).Value<string>());
            }
        }

    }
}
