using LZ.Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LZ.Core
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private static readonly JsonSerializerOptions CamelCaseOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ICurrentUser CurrentUser
        {
            get
            {
                return HttpContext.RequestServices.GetService<ICurrentUser>();
            }
        }

        /// <summary>
        /// 将 IActionResult 返回的 JSON 属性名转换为小驼峰
        /// </summary>
        protected IActionResult ToCamelCase(IActionResult result)
        {
            if (result is ObjectResult objectResult)
            {
                var value = objectResult.Value;

                if (value == null)
                {
                    return result;
                }

                var json = JsonSerializer.Serialize(value, CamelCaseOptions);
                var camelCaseValue = JsonSerializer.Deserialize<JsonElement>(json);

                return new ObjectResult(camelCaseValue)
                {
                    StatusCode = objectResult.StatusCode,
                    ContentTypes = objectResult.ContentTypes
                };
            }

            return result;
        }

    }
}
