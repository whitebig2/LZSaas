namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 专项附加扣除下载请求
    /// </summary>
    public class PersonalAdditionDownloadRequest
    {
        /// <summary>
        /// 机构税号，长度 20，必填
        /// </summary>
        public required string BelongTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报税号，长度 20，必填
        /// </summary>
        public required string CustomerTaxNo { get; set; } = string.Empty;

        /// <summary>
        /// 扣除年份，例：2026，长度 20，必填
        /// </summary>
        public required string DeductionYear { get; set; } = string.Empty;

        /// <summary>
        /// 个税人员收入明细，非必填
        /// </summary>
        public List<PersonalAdditionDownloadObject> PersonalTaxFormList { get; set; } = new();
    }

    /// <summary>
    /// 专项附加扣除下载人员明细（CsPersonalAdditionDownloadObjectType）
    /// </summary>
    public class PersonalAdditionDownloadObject
    {
        /// <summary>
        /// 证件类型，201 居民身份证，长度 6，必填
        /// </summary>
        public required string IdCardType { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 18，必填
        /// </summary>
        public required string IdCardNo { get; set; } = string.Empty;
    }
}
