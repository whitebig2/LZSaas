using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Bank;
using LZ.Model.Dtos.BankDo;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg.OpenPgp;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace LZ.Core.Attributes
{
    public class CzyTransactionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var request = context.HttpContext.Request;
            var traceId = context.HttpContext.TraceIdentifier;
            var configId = typeof(Live_BankUser).GetCustomAttribute<TenantAttribute>().configId;
            using (var dbContext = DbScoped.SugarScope.GetConnection(configId))
            {
                var redisService = context.HttpContext.RequestServices.GetService<IRedisService>();
                var currentUser = context.HttpContext.RequestServices.GetService<ICurrentUser>();
                var totpService = context.HttpContext.RequestServices.GetService<ITotpService>();
                var bankUser = dbContext.Queryable<Live_BankUser>().FirstAsync(f => f.UserID == currentUser.UserID && !f.IsDelete).Result;
                if (bankUser == null)
                {
                    throw new ErrorCodeException(-1, "非法的支付用户！");
                }

                if (bankUser.IsLock)
                {
                    throw new ErrorCodeException(-1, "当前帐行号已被锁定，无法执行当前操作！");
                }
                if (!bankUser.IsEnable)
                {
                    throw new ErrorCodeException(-1, "当前帐行号未启用，无法执行当前操作！");
                }
                if (bankUser.IsDelete)
                {
                    throw new ErrorCodeException(-1, "当前帐行号不可用，无法执行当前操作！");
                }

                if (!bankUser.IsValidate)
                {
                    throw new ErrorCodeException(-1, "当前帐行号验证未通过，无法执行当前操作！");
                }
                request.Body.Position = 0;
                using (var reader = new StreamReader(request.Body))
                {
                    var bodyContent = reader.ReadToEnd();
                    Common.StaticLog.Debug($"{currentUser.UserID}代发：{bodyContent}");
                    var reqDto = Newtonsoft.Json.JsonConvert.DeserializeObject<TransactionRequest>(bodyContent);
                    var transactionModel = dbContext.Queryable<Live_BankTransaction>().FirstAsync(f => f.ID == reqDto.TransactionID).Result;
                    // 获取配置
                    var config = dbContext.Queryable<Live_BankCompanyConfig>().FirstAsync(f => f.ID == transactionModel.BankCompanyConfigID && f.IsEnable && !f.IsDelete).Result;
                    if (config == null)
                    {
                        context.Result = new BadRequestObjectResult(new RequestResultModel(-1, "当前配置【不可用】请重置后重新选择代发配置"));
                        return;
                    }
                    if (transactionModel.BankCompanyID != config.BankCompanyID)
                    {
                        context.Result = new BadRequestObjectResult(new RequestResultModel(-1, "当前代发配置关联的单位信息与交易记录中的单位信息不一致"));
                        return;
                    }
                    var isCompany = dbContext.Queryable<Live_BankCompany>().Where(f => f.ID == config.BankCompanyID && f.IsEnable && !f.IsDelete).Any();

                    if (!isCompany)
                    {
                        context.Result = new BadRequestObjectResult(new RequestResultModel(-1, "代发单位不可用"));
                        return;
                    }
                    var isCanSend = dbContext.Queryable<Live_BankUserCompany>().Where(f => f.BankUserID == bankUser.ID && f.BankCompanyID == config.BankCompanyID && f.IsEnable).Any();
                    if (!isCanSend)
                    {
                        context.Result = new BadRequestObjectResult(new RequestResultModel(-1, "无权代发此单位"));
                        return;
                    }
                    else
                    {


                        // 将动态动态令牌标记为已消费
                        var isTotpExist = redisService.SetAsync($"TransactionFilterMiddleware_TotpCode_{currentUser.UserID}_{reqDto.GetHashCode()}", "动态支付令牌", 300, CSRedis.RedisExistence.Nx).Result;
                        if (!isTotpExist)
                        {
                            context.Result = new BadRequestObjectResult(new RequestResultModel(-1, "请求重放检测：TOTP 动态令牌 已被使用！"));
                            return;
                        }

                        // 是否通过
                        var isSuc = totpService.ValidateCode(bankUser.SecretKey, reqDto.TotpCode);
                        if (!isSuc)
                        {
                            bankUser.FailTimes++;
                            if (bankUser.FailTimes <= 5)
                            {
                                bankUser.FailMsg = $"代发失败：【支付密码错误】，当前失败次数：【{bankUser.FailTimes}】，失败次数超过5次，账号将被锁定！";
                            }
                            else
                            {
                                bankUser.FailMsg = $"代发失败：【支付密码错误】，当前失败次数：【{bankUser.FailTimes}】,账号已被锁定！";
                                bankUser.IsLock = true;
                                bankUser.LockTime = DateTime.Now;
                            }
                            dbContext.Updateable<Live_BankUser>(bankUser).UpdateColumns(f => new { f.IsLock, f.LockTime, f.FailMsg, f.FailTimes }).ExecuteCommand();
                            context.Result = new BadRequestObjectResult(new RequestResultModel(-1, bankUser.FailMsg));
                            return;
                        }
                        else
                        {
                            // 充值失败次数
                            if (bankUser.FailTimes > 0)
                            {
                                bankUser.FailTimes = 0;
                                bankUser.FailMsg = string.Empty;
                                dbContext.Updateable<Live_BankUser>(bankUser).UpdateColumns(f => new { f.IsLock, f.LockTime, f.FailMsg, f.FailTimes }).ExecuteCommand();
                            }
                          
                        }
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
