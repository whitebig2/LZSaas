//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Caching.Memory;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Core.Middleware
//{
//    public class SingleEndpointRateLimitMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly IMemoryCache _cache;
//        private const string TargetPath = "/api/SalaryCustomer/SalaryTransBatch";  // 指定接口路径

//        public SingleEndpointRateLimitMiddleware(RequestDelegate next, IMemoryCache cache)
//        {
//            _next = next;
//            _cache = cache;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            if (context.Request.Path != TargetPath)
//            {  // 仅限流目标接口‌:ml-citation{ref="1,6" data="citationList"}
//                await _next(context);
//                return;
//            }

//            var ip = context.Connection.RemoteIpAddress?.ToString();
//            var cacheKey = $"{ip}_count11";
//            var count = _cache.GetOrCreate(cacheKey, entry => {
//                //entry.AbsoluteExpiration = DateTime.Now.AddSeconds(1);
//                entry.AbsoluteExpirationRelativeToNow =TimeSpan.FromSeconds(5);
//                return 0;
//            });

//            Console.WriteLine( $"{ip}--{count}" );
//            if (count >= 1)
//            {  // 每分钟最多30次
//                context.Response.StatusCode = 429;
//                await context.Response.WriteAsync("请求过快");
//                return;
//            }

//            _cache.Set(cacheKey, count + 1);
//            await _next(context);
//        }
//    }

//}
