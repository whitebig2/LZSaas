namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 三方协议下载响应
    /// </summary>
    public class PersonalTripleAgreementDownloadResponse
    {
        /// <summary>
        /// 缴款账户名称，长度 50，非必填
        /// </summary>
        public string TaxName { get; set; } = string.Empty;

        /// <summary>
        /// 开户银行，长度 50，非必填
        /// </summary>
        public string BankName { get; set; } = string.Empty;

        /// <summary>
        /// 缴款账号，长度 50，非必填
        /// </summary>
        public string BankNumber { get; set; } = string.Empty;

        /// <summary>
        /// 三方协议号，长度 50，非必填
        /// </summary>
        public string TripleAgreementNo { get; set; } = string.Empty;
    }
}
