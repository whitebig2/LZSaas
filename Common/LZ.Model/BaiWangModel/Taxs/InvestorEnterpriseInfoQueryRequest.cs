namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 企业信息查询请求
    /// </summary>
    public class InvestorEnterpriseInfoQueryRequest
    {
        /// <summary>
        /// 机构税号，长度 18，非必填
        /// </summary>
        public string? BelongTaxNo { get; set; }

        /// <summary>
        /// 申报税号，长度 18，非必填
        /// </summary>
        public string? CustomerTaxNo { get; set; }
    }
}
