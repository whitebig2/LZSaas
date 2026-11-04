namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 三方协议下载请求
    /// </summary>
    public class PersonalTripleAgreementDownloadRequest
    {
        /// <summary>
        /// 机构税号，长度 20，必填
        /// </summary>
        public required string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，长度 20，必填
        /// </summary>
        public required string CustomerTaxNo { get; set; } = string.Empty;
    }
}
