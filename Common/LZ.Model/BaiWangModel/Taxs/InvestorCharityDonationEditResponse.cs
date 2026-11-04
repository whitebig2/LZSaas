namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 公益慈善捐赠编辑响应
    /// </summary>
    public class InvestorCharityDonationEditResponse
    {
        /// <summary>
        /// 成功明细，非必填
        /// </summary>
        public List<InvestorCharityDonationEditSuccess> SuccessList { get; set; } = new();

        /// <summary>
        /// 失败明细，非必填
        /// </summary>
        public List<InvestorCharityDonationEditFail> FailList { get; set; } = new();
    }

    /// <summary>
    /// 公益慈善捐赠编辑成功项（CsInvestorCharity_donation_editSuccessListItem）
    /// </summary>
    public class InvestorCharityDonationEditSuccess
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
        /// 捐赠凭证号，长度 32，非必填
        /// </summary>
        public string DonationCertificateNumber { get; set; } = string.Empty;

        /// <summary>
        /// 实际扣除金额，长度 19，非必填
        /// </summary>
        public decimal ActualDeductMoney { get; set; }

        /// <summary>
        /// 可扣除金额上限，长度 19，非必填
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 已填写金额（含本次），长度 19，非必填
        /// </summary>
        public decimal FilledAmount { get; set; }

        /// <summary>
        /// 记录状态，0-未填写，1-待完善，2-已填写，长度 19，非必填
        /// </summary>
        public long Status { get; set; }

        /// <summary>
        /// 是否为更新已有记录，非必填
        /// </summary>
        public bool Update { get; set; }
    }

    /// <summary>
    /// 公益慈善捐赠编辑失败项（CsInvestorCharity_donation_editFailListItem）
    /// </summary>
    public class InvestorCharityDonationEditFail
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
        /// 失败原因，长度 200，非必填
        /// </summary>
        public string FailReason { get; set; } = string.Empty;
    }
}
