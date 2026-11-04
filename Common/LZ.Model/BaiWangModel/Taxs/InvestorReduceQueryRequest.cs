namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 减免事项查询请求
    /// </summary>
    public class InvestorReduceQueryRequest
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

        /// <summary>
        /// 报表类型，见报表类型，长度 19，非必填
        /// </summary>
        public long? FormType { get; set; }
    }
}
