using LZ.Common;
using LZ.Core.Attributes;
using LZ.Core.Filters;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Bank;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace LZ.Core.Middleware
{

    public class TransactionEncryptMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionEncryptMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {


          var myAttr=  context.GetEndpoint()?.Metadata?.GetMetadata<CzyTransactionFilterAttribute>();

            // 仅针对特定路径或方法进行处理，避免性能损耗
            if (myAttr!=null && context.Request.Method == HttpMethods.Post)
            {
                RequestResultModel result = null;
                var logger = context.RequestServices.GetRequiredService<ILogger<TransactionEncryptMiddleware>>();
                var redisService = context.RequestServices.GetRequiredService<IRedisService>();
                var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();
                //var sugarClient = context.RequestServices.GetRequiredService<ISqlSugarClient>();
                var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
                // 1. 启用缓冲，允许流被多次读取
                context.Request.EnableBuffering();
                // 2. 读取原始请求体
                string body;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                }

                var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<TransactionPayloadRequest>(body);
                var isNonceExist = redisService.SetAsync($"TransactionFilterMiddleware_{payload.Nonce}", payload.Nonce, 300, CSRedis.RedisExistence.Nx).Result;
                if (!isNonceExist)
                {
                    result = new RequestResultModel(-1, "请求重放检测：Nonce 已被使用！");
                    await context.Response.WriteAsJsonAsync(result);
                    return;
                }
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (Math.Abs(now - payload.Timestamp) > 300 * 1000)
                {
                    result = new RequestResultModel(-1, "请求重放检测：时间戳过期！");
                    await context.Response.WriteAsJsonAsync(result);
                    return;
                }
                string privateKey = string.Empty;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    privateKey = configuration.GetSection("RsaPayPrivateKey")?.Value;

                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // 发布到容器 需要做目录挂载
                    privateKey = await System.IO.File.ReadAllTextAsync("/RsaKeys/private.pem");
                }

                if (string.IsNullOrWhiteSpace(privateKey))
                {
                    result = new RequestResultModel(-1, "Ras 私钥未配置！");
                    await context.Response.WriteAsJsonAsync(result);
                    return;
                }
                RsaPemHelper rsaHelper = new RsaPemHelper(privateKey, "");
                // 3. 解密AES 密钥
                var aesKey = rsaHelper.Decrypt(payload.Key);
                // 4. 组装签名 并且验签
                string content = $"{payload.Data}{payload.Iv}{payload.Timestamp}{payload.Nonce}";
                var isb = AesCBCHelper.VerifyHmac(content, payload.Signature, Convert.FromHexString(aesKey));
                if (!isb)
                {
                    result = new RequestResultModel(-1, "验签失败", null);
                    await context.Response.WriteAsJsonAsync(result);
                    return;
                }
                // 5. 业务密文 解密
                var businessBody = AesCBCHelper.Decrypt(payload.Data, Convert.FromHexString(aesKey), Convert.FromHexString(payload.Iv));
                byte[] bytes = Encoding.UTF8.GetBytes(businessBody);
                context.Request.Body = new MemoryStream(bytes);
            }

            await _next(context);
        }
    }
}
