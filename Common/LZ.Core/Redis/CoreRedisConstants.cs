using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.Redis
{
    public class CoreRedisConstants
    {

        /// <summary>
        /// 用户Token缓存 Key
        /// </summary>
        public static string UserToken = "UserToken:{0}";

        /// <summary>
        /// 用户信息 Key
        /// </summary>
        public static string UserInfo = "UserInfo:{0}";
        
        /// <summary>
        /// 人力公司配置 Key
        /// </summary>
        public static string CompanyConfig = "CompanyConfig:{0}";
        /// <summary>
        /// 总管理后台权限
        /// </summary>
        public static string ManageUserRoles = "ManageUserRoles:{0}";
        /// <summary>
        /// 分后台权限
        /// </summary>
        public static string CompanyUserRoles = "CompanyUserRoles:{0}";
        /// <summary>
        /// 客户端
        /// </summary>
        public static string AppID = "AppID:{0}";
        /// <summary>
        /// 客户端页面
        /// </summary>
        public static string AppPage = "AppPage-{1}:{0}";
        /// <summary>
        /// 数据字典缓存
        /// </summary>
        public static string Dictionary = "Dictionary:{0}";
        /// <summary>
        /// 数据字典缓存
        /// </summary>
        public static string CompanyDictionary = "CompanyDictionary:{0}";
        /// <summary>
        /// 各公司的工种
        /// </summary>
        public static string CompanyWorks = "CompanyWorks:{0}";
        /// <summary>
        /// 各公司的离职原因
        /// </summary>
        public static string CompanyLeaveReason = "CompanyLeaveReason:{0}";
        /// <summary>
        /// 用户id对应的token
        /// </summary>
        public static string UserPlatformToken = "UserPlatformToken:{0}-{1}";
        /// <summary>
        /// 短信验证
        /// </summary>
        public static string SmsVerify = "SmsVerify:{0}";
        /// <summary>
        /// 总访问量
        /// </summary>
        public static string TotalVisits = "TotalVisits";
        /// <summary>
        /// 日访问量
        /// </summary>
        public static string DayTotalVisits = "DayTotalVisits";
        /// <summary>
        /// 总更换工种数量
        /// </summary>
        public static string TotalChangeWork = "TotalChangeWork";
        /// <summary>
        /// 日更换工种数量
        /// </summary>
        public static string DayTotalChangeWork = "DayTotalChangeWork";
        /// <summary>
        /// 总更换单位数量
        /// </summary>
        public static string TotalChangeCompany = "TotalChangeCompany";
        /// <summary>
        /// 日更换单位数量
        /// </summary>
        public static string DayTotalChangeCompany = "DayTotalChangeCompany";
    }
}
