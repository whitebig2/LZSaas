namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 减免事项编辑响应
    /// </summary>
    public class InvestorReduceEditResponse
    {
        /// <summary>
        /// 成功明细，非必填
        /// </summary>
        public List<InvestorReduceEditSuccess> SuccessList { get; set; } = new();

        /// <summary>
        /// 失败明细，非必填
        /// </summary>
        public List<InvestorReduceEditFail> FailList { get; set; } = new();
    }

    /// <summary>
    /// 减免事项编辑成功项（CsInvestorReduce_editObjectType）
    /// </summary>
    public class InvestorReduceEditSuccess
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
        /// 减免类型，0-免税收入，1-减免税额，长度 19，非必填
        /// </summary>
        public long ReduceType { get; set; }

        /// <summary>
        /// 所得项目编码，长度 4，非必填
        /// </summary>
        public string ObtainedProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目名称，长度 6，非必填
        /// </summary>
        public string ObtainedProjectName { get; set; } = string.Empty;

        /// <summary>
        /// 免税收入/减免税额上限，长度 19，非必填
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 本期已填报金额，长度 19，非必填
        /// </summary>
        public decimal FilledAmount { get; set; }

        /// <summary>
        /// 记录状态，长度 19，非必填
        /// </summary>
        public long Status { get; set; }
    }

    /// <summary>
    /// 减免事项编辑失败项（CsInvestorReduce_editFail）
    /// </summary>
    public class InvestorReduceEditFail
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
        /// 所得项目名称，长度 6，非必填
        /// </summary>
        public string ObtainedProjectName { get; set; } = string.Empty;

        /// <summary>
        /// 失败原因，长度 33，非必填
        /// </summary>
        public string FailReason { get; set; } = string.Empty;
    }
}
