//using LZ.Common;
//using LZ.Core.Redis;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Core.Filters
//{
//    public class ApiEncryptionFilter : IActionFilter
//    {
//        private readonly IConfiguration _config;
//        private readonly IRedisService _cache;
//        private const int RequestExpireSeconds = 300; // 5分钟防重放

//        public ApiEncryptionFilter(IConfiguration config, IRedisService cache)
//        {
//            _config = config;
//            _cache = cache;
//        }

//        public async Task OnActionExecuting(ActionExecutingContext context)
//        {
//            var request = context.HttpContext.Request;
//            // 1. 校验时间戳防重放
//            if (!request.Headers.TryGetValue("X-Timestamp", out var tsStr) ||
//                !long.TryParse(tsStr, out var timestamp) ||
//                DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime > TimeSpan.FromSeconds(RequestExpireSeconds))
//            {
//                context.Result = new BadRequestObjectResult(new { Code = 401, Msg = "请求已过期" });
//                return;
//            }

     
//            // 2. 校验请求ID防重复提交
//            //if (!request.Headers.TryGetValue("X-RequestId", out var requestId) || _cache.Get($"REQ_SIGN:{requestId}") != null)
//            //{
//            //    context.Result = new BadRequestObjectResult(new { Code = 402, Msg = "重复请求" });
//            //    return;
//            //}


//            request.Headers.TryGetValue("X-RequestId", out var requestId);

//           bool isb= await _cache.SetAsync($"X-Request-{requestId}", string.Empty, RequestExpireSeconds,CSRedis.RedisExistence.Nx);
//            if (!isb)
//            {
//                context.Result = new BadRequestObjectResult(new { Code = 402, Msg = "重复请求" });
//                return;
//            }



//            // 3. 提取并解密AES会话密钥
//            var wrappedAesKey = request.Headers["X-Aes-Key"].ToString();
//            var privateKey = _config["Crypto:RsaPrivateKey"]!;
//            var aesKey = CryptoHelper.RsaUnwrapAesKey(privateKey, wrappedAesKey);

//            // 4. 读取请求密文，验签后解密
//            using var reader = new StreamReader(request.Body);
//            var cipherBody = reader.ReadToEndAsync().Result;
//            var sign = request.Headers["X-Sign"].ToString();
//            var verifyContent = $"{timestamp}{requestId}{cipherBody}";
//            var hmacKey = aesKey;

//            if (!CryptoHelper.GenerateSign(hmacKey, verifyContent).Equals(Convert.FromBase64String(sign)))
//            {
//                context.Result = new BadRequestObjectResult(new { Code = 403, Msg = "签名验证失败" });
//                return;
//            }

//            // 替换请求体为明文，后续模型绑定直接读取
//            var plainBody = CryptoHelper.AesGcmDecrypt(aesKey, cipherBody, aad: verifyContent);
//            context.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(plainBody));
//        }

//        public void OnActionExecuted(ActionExecutedContext context)
//        {
//            // 响应加密逻辑：读取控制器返回结果，用当前AES密钥加密后返回
//            var originalResult = context.Result as ObjectResult;
//            if (originalResult?.Value == null) return;

//            var aesKey = (byte[])context.HttpContext.Items["CurrentAesKey"]!;
//            var responseJson = JsonSerializer.Serialize(originalResult.Value);
//            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
//            var cipherResponse = CryptoHelper.AesGcmEncrypt(aesKey, responseJson, aad: timestamp.ToString());

//            var hmacKey = Convert.FromBase64String(_config["Crypto:HmacKey"]!);
//            var responseSign = CryptoHelper.GenerateSign(hmacKey, $"{timestamp}{cipherResponse}");

//            context.Result = new OkObjectResult(new
//            {
//                Timestamp = timestamp,
//                CipherData = cipherResponse,
//                Sign = responseSign
//            });
//        }
//    }

//}
