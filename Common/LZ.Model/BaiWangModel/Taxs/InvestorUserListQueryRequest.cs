namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 人员列表查询请求
    /// </summary>
    public class InvestorUserListQueryRequest
    {
        /// <summary>
        /// 机构税号，长度 18，非必填
        /// </summary>
        public string? BelongTaxNo { get; set; }

        /// <summary>
        /// 申报税号，长度 18，非必填
        /// </summary>
        public string? CustomerTaxNo { get; set; }

        /// <summary>
        /// 证件号码，精确匹配，长度 18，非必填
        /// </summary>
        public string? IdCardNo { get; set; }

        /// <summary>
        /// 报送状态，见报送状态，非必填
        /// </summary>
        public List<long> DeclareStatusList { get; set; } = new();

        /// <summary>
        /// 身份验证状态，见身份验证状态，非必填
        /// </summary>
        public List<long> VerifyStatusList { get; set; } = new();

        /// <summary>
        /// 受雇从业类型，见受雇从业类型，长度 19，非必填
        /// </summary>
        public long? EmploymentType { get; set; }

        /// <summary>
        /// 页码，默认 1，长度 19，非必填
        /// </summary>
        public long? PageNum { get; set; }

        /// <summary>
        /// 每页条数，默认 20，长度 19，非必填
        /// </summary>
        public long? PageSize { get; set; }
    }
}
