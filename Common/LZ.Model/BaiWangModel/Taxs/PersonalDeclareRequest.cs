namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税申报请求
    /// </summary>
    public class PersonalDeclareRequest
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
        public required string DeclarationPeriod { get; set; } = string.Empty;
    }
}
