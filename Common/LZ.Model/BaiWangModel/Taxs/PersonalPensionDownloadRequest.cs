namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 个人养老金下载请求
    /// </summary>
    public class PersonalPensionDownloadRequest
    {
        /// <summary>
        /// 机构税号，必填
        /// </summary>
        public required string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，必填
        /// </summary>
        public required string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 扣除年份，必填
        /// </summary>
        public required string DeductionYear { get; set; } = string.Empty;

        /// <summary>
        /// 个税人员收入明细，必填
        /// </summary>
        public required List<PersonalPensionDownloadObject> PersonalTaxFormList { get; set; } = new();
    }

    /// <summary>
    /// 个人养老金下载人员明细（CsPersonalPensionDownloadObjectType）
    /// </summary>
    public class PersonalPensionDownloadObject
    {
        /// <summary>
        /// 证件类型，201 居民身份证，非必填
        /// </summary>
        public string? IdCardType { get; set; }

        /// <summary>
        /// 证件号码，非必填
        /// </summary>
        public string? IdCardNo { get; set; }
    }
}
