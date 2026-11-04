namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 减免事项编辑请求
    /// </summary>
    public class InvestorReduceEditRequest
    {
        /// <summary>
        /// 企业税号，长度 18，非必填
        /// </summary>
        public string? BelongTaxNo { get; set; }

        /// <summary>
        /// 申报税号，长度 18，非必填
        /// </summary>
        public string? CustomerTaxNo { get; set; }

        /// <summary>
        /// 税款所属期，格式 yyyy-MM，长度 7，非必填
        /// </summary>
        public string? TaxPeriod { get; set; }

        /// <summary>
        /// 报表类型，0-综合所得，1-分类所得，长度 19，非必填
        /// </summary>
        public long? FormType { get; set; }

        /// <summary>
        /// 减免事项明细，非必填
        /// </summary>
        public List<InvestorReduceEditObject> ReduceItemList { get; set; } = new();
    }

    /// <summary>
    /// 减免事项明细（CsInvestorReduce_editObjectType）
    /// </summary>
    public class InvestorReduceEditObject
    {
        /// <summary>
        /// 姓名，长度 6，非必填
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 证件类型，长度 3，非必填
        /// </summary>
        public string? IdCardType { get; set; }

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string? IdCardNo { get; set; }

        /// <summary>
        /// 所得项目编码，长度 4，非必填
        /// </summary>
        public string? ObtainedProjectCode { get; set; }

        /// <summary>
        /// 减免类型，0-免税收入，1-减免税额，长度 19，非必填
        /// </summary>
        public long? ReduceType { get; set; }

        /// <summary>
        /// 减免事项代码，长度 12，非必填
        /// </summary>
        public string? ExemptionMatterCode { get; set; }

        /// <summary>
        /// 减免性质代码，长度 10，非必填
        /// </summary>
        public string? ReductionCode { get; set; }

        /// <summary>
        /// 减免金额，长度 19，非必填
        /// </summary>
        public decimal? ReductionIncome { get; set; }
    }
}
