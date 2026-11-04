namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 同步客户账号信息请求
    /// </summary>
    public class PersonalSyncCustomerAccountInfoRequest
    {
        /// <summary>
        /// 机构税号，长度 20，必填
        /// </summary>
        public required string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，长度 20，必填
        /// </summary>
        public required string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 登录类型，长度 11，必填
        /// </summary>
        public required long LoginType { get; set; }

        /// <summary>
        /// 实名认证账号，个税实名登录必填，申报密码登录时填写无效，长度 64，条件必填
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// 实名认证密码，个税实名登录必填，申报密码登录时填写无效，长度 20，条件必填
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 申报密码，申报密码登录时必填，个人实名登录时填写无效，长度 20，条件必填
        /// </summary>
        public string? DeclarePassword { get; set; }
    }
}
