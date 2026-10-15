//using LZ.Core.Interface;
//using LZ.Core.Redis;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Core.Filters
//{
//    /// <summary>
//    /// 支付接口安全过滤器：TOTP 动态令牌校验 + 幂等性 + 防重放
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
//    public class TransactionFilterAttribute : Attribute, IAsyncActionFilter
//    {
//        /// <summary>
//        /// 对应接口编号
//        /// </summary>
//        public string MesgNo { get; set; } = string.Empty;

//        /// <summary>
//        /// 是否启用 TOTP 动态令牌校验（默认 true）
//        /// </summary>
//        public bool EnableTotp { get; set; } = true;

//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<TransactionFilterAttribute>>();
//            var idempotencyService = context.HttpContext.RequestServices.GetRequiredService<IRedisService>();

//            var httpContext = context.HttpContext;
//            var request = httpContext.Request;

//            if (EnableTotp)
//            {
//                var totpService = context.HttpContext.RequestServices.GetRequiredService<ITotpService>();
//                var secretResolver = context.HttpContext.RequestServices.GetRequiredService<ITotpSecretResolver>();

//                var totpCodeHeader = request.Headers["X-Totp-Code"].FirstOrDefault();
//                var payAccountNo = ExtractPayAccountNo(context);

//                if (string.IsNullOrEmpty(totpCodeHeader))
//                {
//                    logger.LogWarning("[PaymentSecurity] Missing X-Totp-Code header, Path={Path}", request.Path);
//                    context.Result = new BadRequestObjectResult(new
//                    {
//                        ResultCode = "MISSING_TOTP",
//                        ResultMsg = "缺少 X-Totp-Code 动态令牌请求头"
//                    });
//                    return;
//                }

//                if (string.IsNullOrEmpty(payAccountNo))
//                {
//                    logger.LogWarning("[PaymentSecurity] Could not extract PayAccountNo from request, Path={Path}", request.Path);
//                    context.Result = new BadRequestObjectResult(new
//                    {
//                        ResultCode = "INVALID_REQUEST",
//                        ResultMsg = "无法从请求体中提取付款方银行账号"
//                    });
//                    return;
//                }

//                var secret = secretResolver.GetSecretByAccount(payAccountNo);
//                if (string.IsNullOrEmpty(secret))
//                {
//                    logger.LogWarning("[PaymentSecurity] TOTP secret not configured for account={Account}", payAccountNo);
//                    context.Result = new BadRequestObjectResult(new
//                    {
//                        ResultCode = "TOTP_NOT_CONFIGURED",
//                        ResultMsg = $"账户 {payAccountNo} 未配置 TOTP 密钥"
//                    });
//                    return;
//                }

//                var verifyResult = totpService.Verify(secret, totpCodeHeader);
//                if (!verifyResult.Valid)
//                {
//                    logger.LogWarning("[PaymentSecurity] Invalid TOTP code for account={Account}, code={Code}", payAccountNo, totpCodeHeader);
//                    context.Result = new BadRequestObjectResult(new
//                    {
//                        ResultCode = "INVALID_TOTP",
//                        ResultMsg = "TOTP 动态令牌校验失败，请重新获取后提交"
//                    });
//                    return;
//                }

//                logger.LogInformation("[PaymentSecurity] TOTP verified for account={Account}, offset={Offset}",
//                    payAccountNo, verifyResult.Offset);
//            }

//            var timestampHeader = request.Headers["X-Timestamp"].FirstOrDefault();
//            var nonceHeader = request.Headers["X-Nonce"].FirstOrDefault();

//            if (string.IsNullOrEmpty(timestampHeader) || string.IsNullOrEmpty(nonceHeader))
//            {
//                logger.LogWarning("[PaymentSecurity] Missing X-Timestamp or X-Nonce header, Path={Path}", request.Path);
//                context.Result = new BadRequestObjectResult(new
//                {
//                    ResultCode = "MISSING_HEADER",
//                    ResultMsg = "缺少 X-Timestamp 或 X-Nonce 请求头"
//                });
//                return;
//            }

//            if (!long.TryParse(timestampHeader, out var timestampMs))
//            {
//                logger.LogWarning("[PaymentSecurity] Invalid X-Timestamp format: {Ts}", timestampHeader);
//                context.Result = new BadRequestObjectResult(new
//                {
//                    ResultCode = "INVALID_TIMESTAMP",
//                    ResultMsg = "X-Timestamp 格式错误，应为毫秒级 Unix 时间戳"
//                });
//                return;
//            }

//            if (!idempotencyService.ValidateAndRegisterNonce(timestampMs, nonceHeader))
//            {
//                context.Result = new BadRequestObjectResult(new
//                {
//                    ResultCode = "REPLAY_ATTACK",
//                    ResultMsg = "请求重放检测：时间戳过期或 Nonce 已被使用"
//                });
//                return;
//            }

//            var businessKey = ExtractBusinessKey(context, logger);
//            if (string.IsNullOrEmpty(businessKey))
//            {
//                logger.LogWarning("[PaymentSecurity] Could not extract business key from request, Path={Path}", request.Path);
//                context.Result = new BadRequestObjectResult(new
//                {
//                    ResultCode = "INVALID_REQUEST",
//                    ResultMsg = "无法从请求体中提取业务唯一键（BillsPayOrderNo 或 SerialNo）"
//                });
//                return;
//            }

//            var idempotencyKey = $"{MesgNo}:{businessKey}";
//            if (idempotencyService.TryGetCachedResult(MesgNo, businessKey, out var cachedResult))
//            {
//                logger.LogInformation("[PaymentSecurity] Returning cached result for {Key}", idempotencyKey);
//                context.Result = new ContentResult
//                {
//                    StatusCode = 200,
//                    ContentType = "application/json",
//                    Content = cachedResult
//                };
//                return;
//            }

//            var executedContext = await next();

//            if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
//            {
//                var resultData = objectResult.Value;
//                if (resultData != null)
//                {
//                    try
//                    {
//                        var resultJson = JsonSerializer.Serialize(resultData);
//                        idempotencyService.CacheResult(MesgNo, businessKey, resultJson);
//                    }
//                    catch (Exception ex)
//                    {
//                        logger.LogWarning(ex, "[PaymentSecurity] Failed to cache result for {Key}", idempotencyKey);
//                    }
//                }
//            }
//        }

//        private static string ExtractPayAccountNo(ActionExecutingContext context)
//        {
//            var requestBody = context.ActionArguments.Values.FirstOrDefault();
//            if (requestBody == null) return string.Empty;

//            var paramSetProp = requestBody.GetType().GetProperty("ParamSet");
//            if (paramSetProp == null) return string.Empty;

//            var paramSet = paramSetProp.GetValue(requestBody) as System.Collections.IList;
//            if (paramSet == null || paramSet.Count == 0) return string.Empty;

//            var firstParam = paramSet[0];
//            if (firstParam == null) return string.Empty;

//            var payAccountNo = firstParam.GetType().GetProperty("PayAccountNo")?.GetValue(firstParam)?.ToString();
//            return payAccountNo ?? string.Empty;
//        }

//        private static string ExtractBusinessKey(ActionExecutingContext context, ILogger logger)
//        {
//            var requestBody = context.ActionArguments.Values.FirstOrDefault();
//            if (requestBody == null) return string.Empty;

//            var paramSetProp = requestBody.GetType().GetProperty("ParamSet");
//            if (paramSetProp == null) return string.Empty;

//            var paramSet = paramSetProp.GetValue(requestBody) as System.Collections.IList;
//            if (paramSet == null || paramSet.Count == 0) return string.Empty;

//            var firstParam = paramSet[0];
//            if (firstParam == null) return string.Empty;

//            var type = firstParam.GetType();

//            var billsPayOrderNo = type.GetProperty("BillsPayOrderNo")?.GetValue(firstParam)?.ToString();
//            if (!string.IsNullOrEmpty(billsPayOrderNo))
//                return billsPayOrderNo;

//            var serialNo = type.GetProperty("SerialNo")?.GetValue(firstParam)?.ToString();
//            if (!string.IsNullOrEmpty(serialNo))
//                return serialNo;

//            return string.Empty;
//        }
//    }
//}
