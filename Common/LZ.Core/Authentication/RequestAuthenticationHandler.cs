using LZ.Common;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace LZ.Core.Authentication
{
    /// <summary>
    /// 请求认证处理器（Token校验）
    /// </summary>
    public class RequestAuthenticationHandler(IOptionsMonitor<RequestAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthService authService,IRedisService redisService) : AuthenticationHandler<RequestAuthenticationSchemeOptions>(options, logger, encoder, clock)
    {
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var request = Request;
                var method = request.Method.ToLower();
                if (request.Path != null)
                {
                    var url = request.Path.Value.ToLower();
                    if(url.IndexOf("/health") == -1 && url.IndexOf("/jobtask") == -1 && url.IndexOf("statistics") == -1 && url.IndexOf("im/message") == -1)
                    {
                        if (method.Equals("post") || method.Equals("put"))
                        {
                            HttpRequestRewindExtensions.EnableBuffering(request);
                            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                            {
                                string requestBody = await reader.ReadToEndAsync();

                                StaticLog.Debug($"url:{url},body:{requestBody}");
                            }
                            request.Body.Position = 0;
                        }
                        await redisService.IncrByAsync(CoreRedisConstants.DayTotalVisits);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
            }
            var token = Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(token))
            {
                token = token.Trim().Replace("Basic ", "");

                // 验证 Token 是否有效，并获取用户信息
                var userToken = await authService.ValidateTokenAsync(token);
                if (userToken == null)
                {
                    return AuthenticateResult.Fail("Invalid Token!");
                }
                try
                {
                    var claims = new List<Claim>
                {
                    new(LZClaimTypes.UserId, userToken.UserID.Value.ToString()),
                    new(LZClaimTypes.Token, token),
                    new(ClaimTypes.NameIdentifier, userToken.UserID.Value.ToString()),
                    new(LZClaimTypes.PlatFormType, userToken.PlatformType.HasValue?userToken.PlatformType.Value.ToString():"0"),
                    new(LZClaimTypes.CompanyID, userToken.CompanyID.HasValue?userToken.CompanyID.Value.ToString():"0"),
                    new(LZClaimTypes.CompanyGID, userToken.CompanyGID.HasValue?userToken.CompanyGID.Value.ToString():""),
                    new(LZClaimTypes.CompanyType, !string.IsNullOrEmpty(userToken.CompanyType)?userToken.CompanyType:""),
                    new(LZClaimTypes.FullName, !string.IsNullOrEmpty(userToken.FullName)?userToken.FullName:""),
                    new(LZClaimTypes.ShopID, userToken.ShopID.HasValue?userToken.ShopID.Value.ToString():"0"),
                   new(LZClaimTypes.UserType, userToken.UserType.HasValue?userToken.UserType.Value.ToString():"0")
                };
                    var claimsIdentity = new ClaimsIdentity(claims, nameof(RequestAuthenticationHandler));

                    var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                catch (Exception ex)
                {
                    Common.StaticLog.Error(ex);
                }
               

                // 将当前用户的所有角色添加到 Claims 中
                //userToken.Roles.ForEach(a =>
                //{
                //    claims.Add(new Claim(LZClaimTypes.Role, a));
                //});

                
            }
            return AuthenticateResult.NoResult();
        }
    }
}
