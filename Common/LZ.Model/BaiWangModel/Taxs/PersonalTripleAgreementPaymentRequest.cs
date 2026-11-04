namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 三方协议缴款请求
    /// </summary>
    public class PersonalTripleAgreementPaymentRequest
    {
        /// <summary>
        /// 机构税号，长度 18，必填
        /// </summary>
        public required string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，长度 18，必填
        /// </summary>
        public required string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 税款所属期，格式 yyyy-mm，长度 7，必填
        /// </summary>
        public required string TaxPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 申报序号，长度 9，必填
        /// </summary>
        public required string DeclarationNo { get; set; } = string.Empty;

        /// <summary>
        /// 三方协议号，长度 14，必填
        /// </summary>
        public required string TripleAgreementNo { get; set; } = string.Empty;
    }
}
