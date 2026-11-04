namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税收入明细删除请求
    /// </summary>
    public class PersonalTaxIncomeFormDeleteRequest
    {
        /// <summary>
        /// 机构税号，长度 20，必填
        /// </summary>
        public required string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，长度 20，必填
        /// </summary>
        public required string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 税款所属期，格式 yyyy-mm，长度 21，必填
        /// </summary>
        public required string DeclarationPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 收入 uuid 列表，最多 100 条，必填
        /// </summary>
        public required List<string> UuidList { get; set; } = new();
    }
}
