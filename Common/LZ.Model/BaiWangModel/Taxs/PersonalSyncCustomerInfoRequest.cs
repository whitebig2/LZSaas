namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 同步客户信息请求
    /// </summary>
    public class PersonalSyncCustomerInfoRequest
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
        /// 客户名称，长度 100，必填
        /// </summary>
        public required string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 企业所在地省份 code，枚举值参考附表《企业所在地省份编码》，长度 4，必填
        /// </summary>
        public required string AreaCode { get; set; } = string.Empty;

        /// <summary>
        /// 纳税性质，0：一般纳税人，2：小规模纳税人（默认），长度 1，非必填
        /// </summary>
        public string? EnterpriseType { get; set; }

        /// <summary>
        /// 数电账号（手机号码），长度 11，非必填
        /// </summary>
        public string? DigitAccount { get; set; }

        /// <summary>
        /// 数电账号密码。数电账号和密码同时有值时，验证数电账号和密码正确性，若正确则保存数电账号和密码。长度 50，非必填
        /// </summary>
        public string? Password { get; set; }
    }
}
