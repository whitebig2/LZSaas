namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 地区基金下载请求
    /// </summary>
    public class InvestorRegionalFundDownloadRequest
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
