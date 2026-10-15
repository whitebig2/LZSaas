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
using System.Text;
using Dm.filter;
namespace LZ.Core.Filters
{
    /// <summary>
    /// 异步请求结果过滤器
    /// </summary>
    public class RecordInterfaceFilter : IAsyncActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var request = context.HttpContext.Request;
                if (request.Path != null && request.Path.Value.IndexOf("/health") == -1)
                {
                    if (request.ContentLength.HasValue && request.ContentLength.Value>0)
                    {
                        HttpRequestRewindExtensions.EnableBuffering(request);

                        using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                        {
                            string requestBody = await reader.ReadToEndAsync();

                            StaticLog.Debug(requestBody);
                        }

                        request.Body.Position = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
            await next();
        }
    }
}
