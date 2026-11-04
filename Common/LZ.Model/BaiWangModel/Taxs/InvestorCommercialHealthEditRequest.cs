namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 商业健康险编辑请求
    /// </summary>
    public class InvestorCommercialHealthEditRequest
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
        /// 商业健康保险明细，非必填
        /// </summary>
        public List<InvestorCommercialHealthEditObject> HealthItemList { get; set; } = new();
    }

    /// <summary>
    /// 商业健康保险明细（CsInvestorCommercial_health_editObjectType）
    /// </summary>
    public class InvestorCommercialHealthEditObject
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
        /// 税优识别码，18 位，前 4 位大于等于 2015，末位为 1，长度 18，非必填
        /// </summary>
        public string? TaxPreferenceCode { get; set; }

        /// <summary>
        /// 保单生效日期，格式 yyyy-MM-dd，长度 10，非必填
        /// </summary>
        public string? PolicyEffectiveDate { get; set; }

        /// <summary>
        /// 年保费，大于 0，长度 19，非必填
        /// </summary>
        public decimal? AnnualPremium { get; set; }

        /// <summary>
        /// 月保费，大于 0 且小于等于年保费，长度 19，非必填
        /// </summary>
        public decimal? MonthPremium { get; set; }

        /// <summary>
        /// 本次扣除金额，大于 0 且小于等于月保费，长度 19，非必填
        /// </summary>
        public decimal? CurrentDeductionAmount { get; set; }
    }
}
