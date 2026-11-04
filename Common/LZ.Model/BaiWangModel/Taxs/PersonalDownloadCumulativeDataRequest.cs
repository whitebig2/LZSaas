namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 下载累计数据请求
    /// </summary>
    public class PersonalDownloadCumulativeDataRequest
    {
        /// <summary>
        /// 机构税号，长度 6，非必填
        /// </summary>
        public string? BelongTaxNo { get; set; }

        /// <summary>
        /// 申报税号，长度 4，非必填
        /// </summary>
        public string? CustomerTaxNo { get; set; }

        /// <summary>
        /// 税款所属期，长度 17，非必填
        /// </summary>
        public string? DeclarationPeriod { get; set; }
    }
}
