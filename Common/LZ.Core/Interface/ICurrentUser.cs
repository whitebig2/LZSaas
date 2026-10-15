using LZ.Model.Enums;

namespace LZ.Core.Interface
{
    /// <summary>
    /// 当前登录用户
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        int UserID { get; }

        /// <summary>
        /// 是否通过认证
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 当前 Token
        /// </summary>
        string Token { get; }
        string FullName { get; }
        string CompanyType { get; }

        /// <summary>
        /// 所属平台
        /// </summary>
        PlatformType PlatformType { get; }
        /// <summary>
        /// 公司编号
        /// </summary>
        int CompanyID { get; }
        int ShopID { get; }
        int UserType { get; }

        Guid? CompanyGID { get; }
    }
}
