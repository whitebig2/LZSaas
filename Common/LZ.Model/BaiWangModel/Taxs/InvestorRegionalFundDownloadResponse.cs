namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 地区基金下载响应
    /// </summary>
    public class InvestorRegionalFundDownloadResponse
    {
        /// <summary>
        /// 社会平均工资，非必填
        /// </summary>
        public List<InvestorRegionalFundDownloadAverageSalary> AverageSalaryList { get; set; } = new();

        /// <summary>
        /// 公积金扣除上限，非必填
        /// </summary>
        public List<InvestorRegionalFundDownloadProvidentFundLimit> ProvidentFundLimitList { get; set; } = new();

        /// <summary>
        /// 年金扣除上限，非必填
        /// </summary>
        public List<InvestorRegionalFundDownloadAnnuityLimit> AnnuityLimitList { get; set; } = new();
    }

    /// <summary>
    /// 社会平均工资（CsInvestorRegional_fund_downloadAverageSalaryListItem）
    /// </summary>
    public class InvestorRegionalFundDownloadAverageSalary
    {
        /// <summary>
        /// 有效期起，长度 10，非必填
        /// </summary>
        public string EffectBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 有效期止，长度 10，非必填
        /// </summary>
        public string EffectEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 平均工资，长度 19，非必填
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// 公积金扣除上限（CsInvestorRegional_fund_downloadProvidentFundLimitListItem）
    /// </summary>
    public class InvestorRegionalFundDownloadProvidentFundLimit
    {
        /// <summary>
        /// 有效期起，长度 10，非必填
        /// </summary>
        public string EffectBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 有效期止，长度 10，非必填
        /// </summary>
        public string EffectEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除上限，长度 19，非必填
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// 年金扣除上限（CsInvestorRegional_fund_downloadAnnuityLimitListItem）
    /// </summary>
    public class InvestorRegionalFundDownloadAnnuityLimit
    {
        /// <summary>
        /// 有效期起，长度 10，非必填
        /// </summary>
        public string EffectBeginDate { get; set; } = string.Empty;

        /// <summary>
        /// 有效期止，长度 10，非必填
        /// </summary>
        public string EffectEndDate { get; set; } = string.Empty;

        /// <summary>
        /// 扣除上限，长度 19，非必填
        /// </summary>
        public decimal Amount { get; set; }
    }
}
