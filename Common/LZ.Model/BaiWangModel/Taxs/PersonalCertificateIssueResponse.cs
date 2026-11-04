namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 完税证明开具响应
    /// </summary>
    public class PersonalCertificateIssueResponse
    {
        /// <summary>
        /// 执行结果，非必填
        /// </summary>
        public PersonalCertificateIssueResult Result { get; set; } = new();
    }

    /// <summary>
    /// 完税证明开具执行结果
    /// </summary>
    public class PersonalCertificateIssueResult
    {
        /// <summary>
        /// 完税证明文件 base64 字符串，长度 67，非必填
        /// </summary>
        public string TaxPaymentCertificate { get; set; } = string.Empty;

        /// <summary>
        /// 机构税号，长度 18，非必填
        /// </summary>
        public string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 客户税号，长度 18，非必填
        /// </summary>
        public string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 税款所属期，长度 7，非必填
        /// </summary>
        public string TaxPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 完税证明下载 url，长度 70，非必填
        /// </summary>
        public string TaxPaymentCertificateUrl { get; set; } = string.Empty;

        /// <summary>
        /// 电子税票号，长度 18，非必填
        /// </summary>
        public string DigitTaxReceiptNo { get; set; } = string.Empty;
    }
}
