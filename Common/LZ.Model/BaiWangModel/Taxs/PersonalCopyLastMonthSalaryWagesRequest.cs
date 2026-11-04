namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 复制上月工资薪金请求
    /// </summary>
    public class PersonalCopyLastMonthSalaryWagesRequest
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
        /// 税款所属期，长度 7，非必填
        /// </summary>
        public string? TaxPeriod { get; set; }

        /// <summary>
        /// 是否覆盖，长度 1，非必填
        /// </summary>
        public string? IsCover { get; set; }
    }
}
