namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 商业健康险查询响应
    /// </summary>
    public class InvestorCommercialHealthQueryResponse
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
        /// 商业健康保险费金额合计，长度 19，非必填
        /// </summary>
        public decimal AmountTotal { get; set; }

        /// <summary>
        /// 附表整体填写状态，已填写/待完善，长度 10，非必填
        /// </summary>
        public string OverallFillStatus { get; set; } = string.Empty;

        /// <summary>
        /// 商业健康保险明细，非必填
        /// </summary>
        public List<InvestorCommercialHealthQueryHealthListItem> HealthList { get; set; } = new();
    }

    /// <summary>
    /// 商业健康保险明细（CsInvestorCommercial_health_queryHealthListItem）
    /// </summary>
    public class InvestorCommercialHealthQueryHealthListItem
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
        /// 可扣除金额上限，长度 19，非必填
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 已填写金额，长度 19，非必填
        /// </summary>
        public decimal FilledAmount { get; set; }

        /// <summary>
        /// 记录状态，0-未填写，1-待完善，2-已填写，长度 19，非必填
        /// </summary>
        public long Status { get; set; }

        /// <summary>
        /// 保单明细，非必填
        /// </summary>
        public List<InvestorCommercialHealthQueryHealthDetail> DetailList { get; set; } = new();
    }

    /// <summary>
    /// 保单明细（CsInvestorCommercial_health_queryHealthDetailListItem）
    /// </summary>
    public class InvestorCommercialHealthQueryHealthDetail
    {
        /// <summary>
        /// 税优识别码，长度 18，非必填
        /// </summary>
        public string TaxPreferenceCode { get; set; } = string.Empty;

        /// <summary>
        /// 保单生效日期，格式 yyyy-MM-dd，长度 10，非必填
        /// </summary>
        public string PolicyEffectiveDate { get; set; } = string.Empty;

        /// <summary>
        /// 年度保费，长度 19，非必填
        /// </summary>
        public decimal AnnualPremium { get; set; }

        /// <summary>
        /// 月度保费，长度 19，非必填
        /// </summary>
        public decimal MonthPremium { get; set; }

        /// <summary>
        /// 本期扣除金额，长度 19，非必填
        /// </summary>
        public decimal CurrentDeductionAmount { get; set; }
    }
}
