using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.Enums;
using LZ.Common.Extensions;
using LZ.Core.Redis;
using LZ.Core.Interface;
using LZ.Core.Attributes;

namespace LZ.Core
{
    /// <summary>
    /// 当前登录用户
    /// </summary>
    public class CurrentUser: ICurrentUser,ISingletonDependency
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 当前登录用户 Id
        /// </summary>
        public int UserID => this.FindClaimValue<int>(LZClaimTypes.UserId);

        /// <summary>
        /// 是否通过认证
        /// </summary>
        public bool IsAuthenticated => this.httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string Token => this.FindClaimValue(LZClaimTypes.Token);

        public string FullName => this.FindClaimValue(LZClaimTypes.FullName);

        public PlatformType PlatformType => this.FindEnumClaimValue(LZClaimTypes.PlatFormType);
        public int CompanyID => this.FindClaimValue<int>(LZClaimTypes.CompanyID);
        public int ShopID => this.FindClaimValue<int>(LZClaimTypes.ShopID);
        public int UserType => this.FindClaimValue<int>(LZClaimTypes.UserType);
        public string CompanyType => this.FindClaimValue(LZClaimTypes.CompanyType);

        public Guid? CompanyGID => this.FindClaimValue<Guid>(LZClaimTypes.CompanyGID);

        public Claim FindClaim(string claimType)
        {
            return this.httpContextAccessor.HttpContext?.User?.FindFirst(claimType);
        }
        public string FindClaimValue(string claimType)
        {
            return FindClaim(claimType)?.Value;
        }

        public T FindClaimValue<T>(string claimType) where T : struct
        {
            var value = FindClaimValue(claimType);

            if (value == null)
            {
                return default;
            }

            return value.To<T>();
        }
        public PlatformType FindEnumClaimValue(string claimType)
        {
            var value = FindClaimValue<int>(claimType);

            return (PlatformType)value;
        }
    }
}
