using CSRedis;
using LZ.Common;
using LZ.Core.Redis;
using LZ.Model.Dtos.FinanceDo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.Filters
{
    public class AesRasRequestHandleFilterAttribute : Attribute, IAsyncResourceFilter
    {
        private const long RequestExpireMilliseconds = 5 * 60 * 1000; // 5 分钟

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            // 仅处理 POST 请求
            if (!string.Equals(request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }
            // 读取原始请求体：启用缓冲并从流起始位置读取，读取后将流位置重置以便后续中间件/模型绑定使用
            string raw = string.Empty;
            try
            {
                // 尝试启用缓冲（在 ASP.NET Core 中提供），允许多次读取请求流
                try { request.EnableBuffering(); } catch { /* 若扩展不存在则忽略 */ }

                // 如果流可寻址，确保从起始位置读取
                if (request.Body != null && request.Body.CanSeek)
                {
                    request.Body.Position = 0;
                }

                using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    raw = await reader.ReadToEndAsync();
                }

                // 将流位置重置，或替换为内存流以便后续读取
                if (request.Body != null && request.Body.CanSeek)
                {
                    request.Body.Position = 0;
                }
                else
                {
                    request.Body = new MemoryStream(Encoding.UTF8.GetBytes(raw ?? string.Empty));
                }
            }
            catch
            {
                raw = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(raw))
            {
                context.Result = new BadRequestObjectResult(new { Code = 400, Msg = "请求体为空" });
                return;
            }

            JObject payload;
            try
            {
                payload = JObject.Parse(raw);
            }
            catch
            {
                context.Result = new BadRequestObjectResult(new { Code = 400, Msg = "请求体不是合法的JSON" });
                return;
            }

            var reqId = payload.Value<string>("reqId")?.Trim();
            var timestampStr = payload.Value<string>("timestamp");
            var aad = payload.Value<string>("aad");
            var wrappedKey = payload.Value<string>("key");
            var cipherBody = payload.Value<string>("body");
            var sign = payload.Value<string>("sign");

            if (string.IsNullOrWhiteSpace(reqId))
            {
                context.Result = new BadRequestObjectResult(new { Code = 400, Msg = "reqId 不能为空" });
                return;
            }

            if (string.IsNullOrEmpty(timestampStr) || !long.TryParse(timestampStr, out var timestamp))
            {
                context.Result = new BadRequestObjectResult(new { Code = 400, Msg = "timestamp 非法" });
                return;
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (Math.Abs(now - timestamp) > RequestExpireMilliseconds)
            {
                context.Result = new BadRequestObjectResult(new { Code = 401, Msg = "请求已过期" });
                return;
            }

            // 获取私钥（优先从配置；Linux 环境尝试读取文件）
            var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            string privateKey = null;
            if (configuration != null)
            {
                privateKey = configuration.GetSection("RsaPayPrivateKey")?.Value;
            }
            if (string.IsNullOrWhiteSpace(privateKey) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    privateKey = await File.ReadAllTextAsync("./RsaKeys/private.pem");
                }
                catch { }
            }

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                context.Result = new BadRequestObjectResult(new { Code = 500, Msg = "服务未配置RSA私钥" });
                return;
            }

            byte[] aesKey;
            try
            {
                aesKey = CryptoHelper.RsaUnwrapAesKey(privateKey, wrappedKey);
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new { Code = 403, Msg = "解密会话密钥失败", Error = ex.Message });
                return;
            }

            // 验签：签名内容通常为 timestamp + reqId + cipherBody（与上游约定）
            var verifyContent = string.Concat(timestampStr ?? string.Empty, reqId, cipherBody ?? string.Empty);
            var expectedSign = CryptoHelper.GenerateSign(aesKey, verifyContent);
            if (!string.Equals(expectedSign, sign, StringComparison.Ordinal))
            {
                context.Result = new BadRequestObjectResult(new { Code = 403, Msg = "签名验证失败" });
                return;
            }

            var redisService = context.HttpContext.RequestServices.GetService(typeof(IRedisService)) as IRedisService;
            if (redisService == null)
            {
                context.Result = new BadRequestObjectResult(new { Code = 500, Msg = "服务未配置缓存" });
                return;
            }

            var expireSeconds = (int)(RequestExpireMilliseconds / 1000);
            var isNewReqId = await redisService.SetAsync($"AesRas_ReqId_{reqId}", reqId, expireSeconds, RedisExistence.Nx);
            if (!isNewReqId)
            {
                context.Result = new BadRequestObjectResult(new { Code = 402, Msg = "请求重放检测：reqId 已被使用" });
                return;
            }

            // 解密业务报文（AES-GCM），aad 使用 verifyContent 或上游提供的 aad
            string plainBody;
            try
            {
                var useAad = string.IsNullOrEmpty(aad) ? verifyContent : aad;
                plainBody = CryptoHelper.AesGcmDecrypt(aesKey, cipherBody, useAad);
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new { Code = 500, Msg = "解密业务数据失败", Error = ex.Message });
                return;
            }
            // 将解密后的（可能已修改）明文写入请求流，供后续模型绑定使用
            var bytes2 = Encoding.UTF8.GetBytes(plainBody);
            request.Body = new MemoryStream(bytes2);
            request.ContentLength = bytes2.Length;
            await next();
        }
    }
}
