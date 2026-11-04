namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个人养老金下载响应
    /// </summary>
    public class PersonalPensionDownloadResponse
    {
        /// <summary>
        /// 返回结果，非必填
        /// </summary>
        public PersonalPensionDownloadResult Result { get; set; } = new();
    }

    /// <summary>
    /// 个人养老金下载返回结果
    /// </summary>
    public class PersonalPensionDownloadResult
    {
        /// <summary>
        /// 人员维度错误信息，非必填
        /// </summary>
        public List<PersonalPensionDownloadPersonErrorInfo> PersonErrorInfoList { get; set; } = new();

        /// <summary>
        /// 下载的养老金明细，非必填
        /// </summary>
        public List<PersonalPensionDownloadPrivatePensionInfo> PrivatePensionInfos { get; set; } = new();
    }

    /// <summary>
    /// 个人养老金下载人员错误信息（CsPersonalPensionDownloadPersonErrorInfo）
    /// </summary>
    public class PersonalPensionDownloadPersonErrorInfo
    {
        /// <summary>
        /// 机构税号，长度 20，非必填
        /// </summary>
        public string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，长度 20，非必填
        /// </summary>
        public string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号，长度 18，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 证件类型，长度 6，非必填
        /// </summary>
        public string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 错误信息，长度 100，非必填
        /// </summary>
        public string ErrorMsg { get; set; } = string.Empty;
    }

    /// <summary>
    /// 个人养老金明细（CsPersonalPensionDownloadPrivatePensionInfo）
    /// </summary>
    public class PersonalPensionDownloadPrivatePensionInfo
    {
        /// <summary>
        /// 凭证编码，长度 10，非必填
        /// </summary>
        public string VoucherNum { get; set; } = string.Empty;

        /// <summary>
        /// 凭证类别，长度 9，非必填
        /// </summary>
        public string VoucherType { get; set; } = string.Empty;

        /// <summary>
        /// 缴款日期，长度 10，非必填
        /// </summary>
        public string PaymentDate { get; set; } = string.Empty;

        /// <summary>
        /// 缴款金额，长度 8，非必填
        /// </summary>
        public string PaymentAmount { get; set; } = string.Empty;

        /// <summary>
        /// 采集人类型代码，01 纳税人，长度 2，非必填
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 更新时间，长度 19，非必填
        /// </summary>
        public string UpdateDate { get; set; } = string.Empty;

        /// <summary>
        /// 验证状态，长度 3，非必填
        /// </summary>
        public string Validating { get; set; } = string.Empty;

        /// <summary>
        /// 自然人档案号，长度 20，非必填
        /// </summary>
        public string Zrrdah { get; set; } = string.Empty;
    }
}
