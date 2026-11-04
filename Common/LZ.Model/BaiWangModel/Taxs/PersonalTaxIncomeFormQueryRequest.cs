namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个税收入明细查询请求
    /// </summary>
    public class PersonalTaxIncomeFormQueryRequest
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
        /// 税款所属期，格式 yyyy-mm，长度 10，必填
        /// </summary>
        public required string DeclarationPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 个税收入报表类型，0-综合所得，1-分类所得，长度 1，必填
        /// </summary>
        public required string PersonalTaxFormType { get; set; } = string.Empty;

        /// <summary>
        /// 个税申报表状态，3 已生成，5 不限，不传默认已生成，长度 1，非必填
        /// </summary>
        public string? PersonalTaxStatus { get; set; }

        /// <summary>
        /// 申报流水号，长度 20，非必填
        /// </summary>
        public string? DeclareSerialNo { get; set; }

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string? IdCardNo { get; set; }

        /// <summary>
        /// 分页查询页数，默认 1，长度 10，非必填
        /// </summary>
        public string? PageNo { get; set; }

        /// <summary>
        /// 分页查询单页查询条数，默认 500，长度 10，非必填
        /// </summary>
        public string? PageSize { get; set; }
    }
}
