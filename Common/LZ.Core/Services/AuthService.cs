using LZ.Model.Enums;
using LZ.Model;
using LZ.Core.Interface;
using LZ.Core.Redis;
using Microsoft.AspNetCore.Http;
using LZ.Common;
using SqlSugar.IOC;
using LZ.Core.Attributes;
using System.Linq;
using System.ComponentModel.Design;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LZ.Core.Services
{
    public class AuthService(IRedisService redisService, IHttpContextAccessor httpContextAccessor) : ServicesBase<Model.SYS_Tokens>, IAuthService,IScopedDependency
    {

        /// <summary>
        /// 验证用户 Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<SYS_Tokens> ValidateTokenAsync(string token)
        {
            var tokenobj = await redisService.GetAsync<SYS_Tokens>(string.Format(CoreRedisConstants.UserToken, token));
            if (tokenobj == null)
            {
                tokenobj = await base.Context.Queryable<SYS_Tokens>().FirstAsync(it => it.Token == token);
                await redisService.SetAsync(string.Format(CoreRedisConstants.UserToken, token), tokenobj, TimeSpan.FromHours(10));
            }
            return tokenobj;
        }
        //public async Task<SYS_Tokens> GetToken<T>(string token)
        //{
        //    var model = await base.Context.Queryable<T>().Where("Token='" + token + "'").FirstAsync();
        //    return Tool.Trans<T, SYS_Tokens>(model);
        //}
        /// <summary>
        /// 生成用户 Token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public async Task<SYS_Tokens> GenerateUserTokenAsync<T>(T user,int uid, PlatformType platform,int companyId=0,Guid? companyGid=null,string companyType="", string fullName = "",int shopId=0,int userType=0)
        {
            var ipAddress = httpContextAccessor.HttpContext.Request.GetRemoteIpAddress();
            var dt = new Dictionary<string, object>();
            dt.Add("UserID", uid);
            dt.Add("LastIP", ipAddress);
            dt.Add("LastTime", DateTime.Now);
            await base.Context.Updateable(dt).AS(user.GetType().Name).WhereColumns("UserID").ExecuteCommandAsync();
           
            var userAgent = httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();

            var token = new SYS_Tokens()
            {
                EndTime = DateTime.Now.AddDays(7),
                IPAddress = ipAddress.ToString(),
                PlatformType = (int)platform,
                UserAgent = userAgent,
                UserID = uid,
                Token = Guid.NewGuid().ToString(),
                CompanyID= companyId,
                CompanyGID= companyGid,
                CompanyType= companyType,
                FullName= fullName,
                ShopID=shopId,
                UserType= userType
            };
            if(platform==PlatformType.Admin || platform == PlatformType.MiniAdmin)
            {
                token.EndTime = DateTime.Now.AddDays(1);
            }
            //await base.Context.Deleteable<SYS_Tokens>().Where(it=>it.UserID==uid && it.PlatformType==token.PlatformType).ExecuteCommandAsync();
            await DeleteToken(uid, token.PlatformType.Value);
            await base.Context.Insertable(token).ExecuteCommandAsync();
            // 将 Token 信息存储到 Redis，有效期 10 小时
            await redisService.SetAsync(string.Format(CoreRedisConstants.UserToken,token.Token), token, TimeSpan.FromHours(10));
            await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, token.Token), user, TimeSpan.FromHours(10));
            await redisService.SetAsync(string.Format(CoreRedisConstants.UserPlatformToken, platform, uid), token.Token, TimeSpan.FromHours(10));
            return token;
        }
        public async Task DeleteToken(int uid,int type)
        {
            var models = await base.Context.Queryable<Model.SYS_Tokens>().Where(it => it.UserID == uid && it.PlatformType == type).ToListAsync();
            foreach (var m in models)
            {
                await redisService.DeleteAsync(string.Format(CoreRedisConstants.UserToken, m.Token));
                await redisService.DeleteAsync(string.Format(CoreRedisConstants.UserInfo, m.Token));
            }
            await base.Context.Deleteable<SYS_Tokens>().Where(it => it.UserID == uid && it.PlatformType == type).ExecuteCommandAsync();
        }
        public async Task DeleteToken(string token)
        {
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.UserToken, token));
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.UserInfo, token));
            await base.Context.Deleteable<Model.SYS_Tokens>().Where(it => it.Token == token).ExecuteCommandAsync();
        }
        /// <summary>
        /// 更新用户token缓存
        /// </summary>
        /// <param name="company"></param>
        /// <param name="uid"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public async Task UpdateUserToken(Model.SYS_Company company, int uid, PlatformType platform)
        {
            var models = await base.Context.Queryable<Model.SYS_Tokens>().Where(it => it.UserID == uid && it.PlatformType == (int)platform).ToListAsync();
            foreach(var m in models)
            {
                m.CompanyID = company.CompanyID;
                m.CompanyGID = company.CompanyGID;
                m.CompanyType = company.CutType;
                await redisService.SetAsync(string.Format(CoreRedisConstants.UserToken, m.Token), m, TimeSpan.FromHours(10));
            }
            await base.Context.Updateable(models).ExecuteCommandAsync();
        }
        /// <summary>
        /// 更新用户缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <param name="uid"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public async Task UpdateUserInfo<T>(T user, int uid, PlatformType platform)
        {
            var models = await base.Context.Queryable<Model.SYS_Tokens>().Where(it => it.UserID == uid && it.PlatformType == (int)platform).ToListAsync();
            foreach (var m in models)
            {
                await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, m.Token), user, TimeSpan.FromHours(10));
            }
        }
        public async Task<T> UserInfo<T>(string token="")
        {
            if (string.IsNullOrEmpty(token))
            {
                token = httpContextAccessor.HttpContext.User.FindFirst(LZClaimTypes.Token).Value;
            }
            var user = await redisService.GetAsync<T>(string.Format(CoreRedisConstants.UserInfo, token));
            if (user == null)
            {
                var tokenobj =await ValidateTokenAsync(token);
                if (tokenobj != null)
                {
                    user = await base.Context.Queryable<T>().Where("UserID=" + tokenobj.UserID).FirstAsync();
                    await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, token), user, TimeSpan.FromHours(10));
                }
            }
            return user;
        }
        public async Task<T> UserInfo<T>(int uid, PlatformType platform)
        {
            try
            {
                var token = await redisService.GetAsync<string>(string.Format(CoreRedisConstants.UserPlatformToken, platform, uid));
                if (string.IsNullOrEmpty(token))
                {
                    var userToken = await base.Context.Queryable<Model.SYS_Tokens>().Where(it => it.UserID == uid && it.PlatformType==(int)platform).FirstAsync();
                    token = userToken.Token;
                    await redisService.SetAsync(string.Format(CoreRedisConstants.UserPlatformToken, platform, uid), token, TimeSpan.FromHours(10));
                }
                var user = await redisService.GetAsync<T>(string.Format(CoreRedisConstants.UserInfo, token));
                if (user == null)
                {
                    var tokenobj = await ValidateTokenAsync(token);
                    if (tokenobj != null)
                    {
                        user = await base.Context.Queryable<T>().Where("UserID=" + tokenobj.UserID).FirstAsync();
                        await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, token), user, TimeSpan.FromHours(10));
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return default;
            }
        }
    }
}
