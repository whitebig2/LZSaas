using LZ.Model.Enums;
using LZ.Model;

namespace LZ.Core.Interface
{
    public interface IAuthService
    {

        /// <summary>
        /// 验证用户 Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<SYS_Tokens> ValidateTokenAsync(string token);
        //Task<SYS_Tokens> GetToken<T>(string token);
        /// <summary>
        /// 生成用户 Token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        Task<SYS_Tokens> GenerateUserTokenAsync<T>(T user, int uid, PlatformType platform,int companyId = 0,Guid? companyGid = null,string companyType="", string fullName = "",int shopId=0,int userType=0);
        /// <summary>
        /// 更新用户缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <param name="uid"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        Task UpdateUserInfo<T>(T user, int uid, PlatformType platform);
        /// <summary>
        /// 根据token获取用户缓存消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> UserInfo<T>(string token="");
        /// <summary>
        /// 根据用户id和平台类型获取用户信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        Task<T> UserInfo<T>(int uid, PlatformType platform);
        /// <summary>
        /// 更新用户token缓存
        /// </summary>
        /// <param name="company"></param>
        /// <param name="uid"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        Task UpdateUserToken(Model.SYS_Company company, int uid, PlatformType platform);
        /// <summary>
        /// 删除token
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task DeleteToken(int uid, int type);
        /// <summary>
        /// 删除token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeleteToken(string token);
    }
}
