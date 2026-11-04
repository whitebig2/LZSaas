namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 商业健康险查询请求
    /// </summary>
    public class InvestorCommercialHealthQueryRequest
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
        /// 税款所属期，格式 yyyy-MM，长度 7，非必填
        /// </summary>
        public string? TaxPeriod { get; set; }
    }
}
