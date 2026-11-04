namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 减免事项查询响应
    /// </summary>
    public class InvestorReduceQueryResponse
    {
        /// <summary>
        /// 申报税号，长度 18，非必填
        /// </summary>
        public string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 税款所属期，长度 7，非必填
        /// </summary>
        public string DeclarationPeriod { get; set; } = string.Empty;

        /// <summary>
        /// 申报表状态，见申报表生成状态，长度 1，非必填
        /// </summary>
        public string DeclarationFormStatus { get; set; } = string.Empty;

        /// <summary>
        /// 申报状态，见申报状态，长度 1，非必填
        /// </summary>
        public string DeclarationStatus { get; set; } = string.Empty;

        /// <summary>
        /// 附表填写人次，长度 19，非必填
        /// </summary>
        public long FillCount { get; set; }

        /// <summary>
        /// 金额合计，长度 19，非必填
        /// </summary>
        public decimal AmountTotal { get; set; }

        /// <summary>
        /// 附表整体填写状态，已填写/待完善，长度 10，非必填
        /// </summary>
        public string OverallFillStatus { get; set; } = string.Empty;

        /// <summary>
        /// 减免事项明细，非必填
        /// </summary>
        public List<InvestorReduceQueryReduceListItem> ReduceList { get; set; } = new();
    }

    /// <summary>
    /// 减免事项明细（CsInvestorReduce_queryReduceListItem）
    /// </summary>
    public class InvestorReduceQueryReduceListItem
    {
        /// <summary>
        /// 姓名，长度 50，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型编码，长度 3，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目编码，长度 4，非必填
        /// </summary>
        public string ObtainedProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目名称，长度 50，非必填
        /// </summary>
        public string ObtainedProjectName { get; set; } = string.Empty;

        /// <summary>
        /// 减免方式，见减免方式，长度 19，非必填
        /// </summary>
        public long ReduceType { get; set; }

        /// <summary>
        /// 总免税收入/减免税额，长度 19，非必填
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 已填写金额，长度 19，非必填
        /// </summary>
        public decimal FilledAmount { get; set; }

        /// <summary>
        /// 填写状态，未填写/已填写/待完善，长度 10，非必填
        /// </summary>
        public string FillStatus { get; set; } = string.Empty;

        /// <summary>
        /// 减免事项明细列表，非必填
        /// </summary>
        public List<InvestorReduceQueryDetail> DetailList { get; set; } = new();
    }

    /// <summary>
    /// 减免事项明细项（CsInvestorReduce_queryDetailListItem）
    /// </summary>
    public class InvestorReduceQueryDetail
    {
        /// <summary>
        /// 减免事项代码，长度 12，非必填
        /// </summary>
        public string ExemptionMatterCode { get; set; } = string.Empty;

        /// <summary>
        /// 减免事项名称，长度 100，非必填
        /// </summary>
        public string ExemptionMatterName { get; set; } = string.Empty;

        /// <summary>
        /// 减免性质代码，长度 10，非必填
        /// </summary>
        public string ReductionCode { get; set; } = string.Empty;

        /// <summary>
        /// 减免性质名称，长度 50，非必填
        /// </summary>
        public string ReductionName { get; set; } = string.Empty;

        /// <summary>
        /// 减免金额，长度 19，非必填
        /// </summary>
        public decimal ReductionIncome { get; set; }
    }
}
