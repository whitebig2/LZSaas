namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税申报响应
    /// </summary>
    public class PersonalDeclareResponse
    {
        /// <summary>
        /// 税款所属期，长度 7，非必填
        /// </summary>
        public string DeclarationPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 申报人次，长度 2，非必填
        /// </summary>
        public string DeclarationNumber { get; set; } = string.Empty;

        /// <summary>
        /// 本期收入，长度 9，非必填
        /// </summary>
        public string TotalIncome { get; set; } = string.Empty;

        /// <summary>
        /// 应补退税额，长度 7，非必填
        /// </summary>
        public string TotalTaxAmount { get; set; } = string.Empty;

        /// <summary>
        /// 申报类型，长度 1，非必填
        /// </summary>
        public string DeclarationType { get; set; } = string.Empty;

        /// <summary>
        /// 申报日期，长度 10，非必填
        /// </summary>
        public string DeclareTime { get; set; } = string.Empty;

        /// <summary>
        /// 申报序号，长度 9，非必填
        /// </summary>
        public string DeclarationNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报流水号，长度 20，非必填
        /// </summary>
        public string DeclareSerialNo { get; set; } = string.Empty;

        /// <summary>
        /// 是否需要缴款，长度 1，非必填
        /// </summary>
        public string NeedPay { get; set; } = string.Empty;
    }
}
