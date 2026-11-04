namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 下载累计数据响应
    /// </summary>
    public class PersonalDownloadCumulativeDataResponse
    {
        /// <summary>
        /// 累计数据，非必填
        /// </summary>
        public List<PersonalDownloadCumulativeData> CumulativeDataList { get; set; } = new();
    }

    /// <summary>
    /// 累计数据（CsPersonalDownload_cumulative_dataCumulativeData）
    /// </summary>
    public class PersonalDownloadCumulativeData
    {
        /// <summary>
        /// 序号，长度 2，非必填
        /// </summary>
        public string Seq { get; set; } = string.Empty;

        /// <summary>
        /// 部门编号，长度 4，非必填
        /// </summary>
        public string DepartmentNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报流水号，长度 5，非必填
        /// </summary>
        public string DeclareSerialNo { get; set; } = string.Empty;

        /// <summary>
        /// 申报序号，长度 4，非必填
        /// </summary>
        public string DeclarationNo { get; set; } = string.Empty;

        /// <summary>
        /// 累计数据明细，非必填
        /// </summary>
        public List<PersonalDownloadCumulativeDataDetail> CumulativeDataDetailList { get; set; } = new();
    }

    /// <summary>
    /// 累计数据明细（CsPersonalDownload_cumulative_dataCumulativeDataDetail）
    /// </summary>
    public class PersonalDownloadCumulativeDataDetail
    {
        /// <summary>
        /// 个人档案号，长度 6，非必填
        /// </summary>
        public string IndividualFileNo { get; set; } = string.Empty;

        /// <summary>
        /// 所得项目代码，长度 6，非必填
        /// </summary>
        public string ObtainedProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// 累计收入额，长度 4，非必填
        /// </summary>
        public string AccumulatedIncome { get; set; } = string.Empty;

        /// <summary>
        /// 累计费用，长度 4，非必填
        /// </summary>
        public string AccumulatedCost { get; set; } = string.Empty;

        /// <summary>
        /// 累计免税收入，长度 6，非必填
        /// </summary>
        public string AccumulatedTaxExemptIncome { get; set; } = string.Empty;

        /// <summary>
        /// 累计减除费用，长度 6，非必填
        /// </summary>
        public string AccumulatedDeductionExpenses { get; set; } = string.Empty;

        /// <summary>
        /// 累计专项扣除，长度 6，非必填
        /// </summary>
        public string AccumulatedSpecialDeductions { get; set; } = string.Empty;

        /// <summary>
        /// 累计其他扣除，长度 6，非必填
        /// </summary>
        public string AccumulatedOtherDeductions { get; set; } = string.Empty;

        /// <summary>
        /// 累计准予扣除的捐赠额，长度 10，非必填
        /// </summary>
        public string AccumulatedDonationAmountAllowedForDeduction { get; set; } = string.Empty;

        /// <summary>
        /// 累计减免税额，长度 6，非必填
        /// </summary>
        public string AccumulatedTaxReductionsExemptions { get; set; } = string.Empty;

        /// <summary>
        /// 已缴税额，长度 6，非必填
        /// </summary>
        public string AccumulatedPaidTaxAmount { get; set; } = string.Empty;

        /// <summary>
        /// 应补/退税额，长度 6，非必填
        /// </summary>
        public string TaxDeclaration { get; set; } = string.Empty;

        /// <summary>
        /// 证件号码，长度 20，非必填
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 姓名，长度 20，非必填
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
