namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 公益慈善捐赠编辑请求
    /// </summary>
    public class InvestorCharityDonationEditRequest
    {
        /// <summary>
        /// 机构税号，长度 18，非必填
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
        /// 报表类型，见报表类型，长度 19，非必填
        /// </summary>
        public long? FormType { get; set; }

        /// <summary>
        /// 捐赠明细，非必填
        /// </summary>
        public List<InvestorCharityDonationEditItem> DonationItemList { get; set; } = new();
    }

    /// <summary>
    /// 捐赠明细（CsInvestorCharity_donation_editDonationItemListItem）
    /// </summary>
    public class InvestorCharityDonationEditItem
    {
        /// <summary>
        /// 姓名，长度 50，非必填
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 证件类型编码，见证件类型编码，长度 3，非必填
        /// </summary>
        public string? IdCardType { get; set; }

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string? IdCardNo { get; set; }

        /// <summary>
        /// 所得项目编码，见所得项目编码，长度 4，非必填
        /// </summary>
        public string? ObtainedProjectCode { get; set; }

        /// <summary>
        /// 捐赠凭证号，长度 32，非必填
        /// </summary>
        public string? DonationCertificateNumber { get; set; }

        /// <summary>
        /// 实际扣除金额，大于 0，长度 19，非必填
        /// </summary>
        public decimal? ActualDeductMoney { get; set; }

        /// <summary>
        /// 备注，长度 200，非必填
        /// </summary>
        public string? Remark { get; set; }
    }
}
