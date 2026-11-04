namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 商业健康险编辑响应
    /// </summary>
    public class InvestorCommercialHealthEditResponse
    {
        /// <summary>
        /// 成功明细，非必填
        /// </summary>
        public List<InvestorCommercialHealthEditSuccess> SuccessList { get; set; } = new();

        /// <summary>
        /// 失败明细，非必填
        /// </summary>
        public List<InvestorCommercialHealthEditFail> FailList { get; set; } = new();
    }

    /// <summary>
    /// 商业健康险编辑成功项（CsInvestorCommercial_health_editObjectType）
    /// </summary>
    public class InvestorCommercialHealthEditSuccess
    {
        /// <summary>
        /// 姓名，长度 6，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，长度 3，非必填
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
        /// 已填写金额（含本次），长度 19，非必填
        /// </summary>
        public decimal FilledAmount { get; set; }

        /// <summary>
        /// 记录状态，0-未填写，1-待完善，2-已填写，长度 19，非必填
        /// </summary>
        public long Status { get; set; }
    }

    /// <summary>
    /// 商业健康险编辑失败项（CsInvestorCommercial_health_editFail）
    /// </summary>
    public class InvestorCommercialHealthEditFail
    {
        /// <summary>
        /// 姓名，长度 4，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，长度 3，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 失败原因，长度 33，非必填
        /// </summary>
        public string FailReason { get; set; } = string.Empty;
    }
}
