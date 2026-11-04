namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 三方协议缴款响应
    /// </summary>
    public class PersonalTripleAgreementPaymentResponse
    {
        /// <summary>
        /// 执行结果，非必填
        /// </summary>
        public PersonalTripleAgreementPaymentResult Result { get; set; } = new();
    }

    /// <summary>
    /// 三方协议缴款执行结果
    /// </summary>
    public class PersonalTripleAgreementPaymentResult
    {
        /// <summary>
        /// 申报序号，长度 9，非必填
        /// </summary>
        public string DeclarationNo { get; set; } = string.Empty;

        /// <summary>
        /// 电子税票号，长度 18，非必填
        /// </summary>
        public string DigitTaxReceiptNo { get; set; } = string.Empty;

        /// <summary>
        /// 滞纳金，长度 4，非必填
        /// </summary>
        public string TotalLateFee { get; set; } = string.Empty;

        /// <summary>
        /// 实缴金额（应补退税额）总计，长度 7，非必填
        /// </summary>
        public string TotalpaidAmount { get; set; } = string.Empty;

        /// <summary>
        /// 机构税号，长度 18，非必填
        /// </summary>
        public string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 客户税号，长度 18，非必填
        /// </summary>
        public string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 三方协议号，长度 14，非必填
        /// </summary>
        public string TripleAgreementNo { get; set; } = string.Empty;

        /// <summary>
        /// 税款所属期，格式 yyyy-mm，长度 7，非必填
        /// </summary>
        public string TaxPeriod { get; set; } = string.Empty;
    }
}
