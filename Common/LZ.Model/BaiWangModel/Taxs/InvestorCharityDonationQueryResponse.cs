namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 公益慈善捐赠查询响应
    /// </summary>
    public class InvestorCharityDonationQueryResponse
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
        /// 准予扣除的捐赠额合计，长度 19，非必填
        /// </summary>
        public decimal AmountTotal { get; set; }

        /// <summary>
        /// 附表整体填写状态，已填写/待完善，长度 10，非必填
        /// </summary>
        public string OverallFillStatus { get; set; } = string.Empty;

        /// <summary>
        /// 捐赠明细，非必填
        /// </summary>
        public List<InvestorCharityDonationQueryDonationListItem> DonationList { get; set; } = new();
    }

    /// <summary>
    /// 捐赠明细（CsInvestorCharity_donation_queryDonationListItem）
    /// </summary>
    public class InvestorCharityDonationQueryDonationListItem
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
        /// 所得项目名称，长度 50，非必填
        /// </summary>
        public string ObtainedProjectName { get; set; } = string.Empty;

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
        /// 捐赠明细列表，非必填
        /// </summary>
        public List<InvestorCharityDonationQueryDonationDetail> DetailList { get; set; } = new();
    }

    /// <summary>
    /// 捐赠凭证明细（CsInvestorCharity_donation_queryDonationDetailListItem）
    /// </summary>
    public class InvestorCharityDonationQueryDonationDetail
    {
        /// <summary>
        /// 受赠单位名称，长度 100，非必填
        /// </summary>
        public string DonationOrganizationName { get; set; } = string.Empty;

        /// <summary>
        /// 受赠单位纳税人识别号（统一社会信用代码），长度 18，非必填
        /// </summary>
        public string DonationOrganizationCode { get; set; } = string.Empty;

        /// <summary>
        /// 捐赠凭证号，长度 32，非必填
        /// </summary>
        public string DonationCertificateNumber { get; set; } = string.Empty;

        /// <summary>
        /// 捐赠日期，长度 10，非必填
        /// </summary>
        public string DonationDate { get; set; } = string.Empty;

        /// <summary>
        /// 捐赠金额，长度 19，非必填
        /// </summary>
        public decimal DonationMoney { get; set; }

        /// <summary>
        /// 捐赠比例，长度 19，非必填
        /// </summary>
        public decimal DonationRate { get; set; }

        /// <summary>
        /// 实际扣除金额，长度 19，非必填
        /// </summary>
        public decimal ActualDeductMoney { get; set; }

        /// <summary>
        /// 备注，长度 200，非必填
        /// </summary>
        public string Remark { get; set; } = string.Empty;
    }
}
