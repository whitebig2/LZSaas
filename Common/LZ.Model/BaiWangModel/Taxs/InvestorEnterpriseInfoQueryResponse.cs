namespace LZ.Model.BaiWangModel.Taxs
{
    /// <summary>
    /// 企业信息查询响应
    /// </summary>
    public class InvestorEnterpriseInfoQueryResponse
    {
        /// <summary>
        /// 统一社会信用代码，长度 18，非必填
        /// </summary>
        public string UnifiedSocialCreditCode { get; set; } = string.Empty;

        /// <summary>
        /// 纳税人识别号，长度 18，非必填
        /// </summary>
        public string TaxIdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// 纳税人名称，长度 200，非必填
        /// </summary>
        public string TaxpayerName { get; set; } = string.Empty;

        /// <summary>
        /// 纳税人状态代码，长度 2，非必填
        /// </summary>
        public string TaxpayerStatusCode { get; set; } = string.Empty;

        /// <summary>
        /// 纳税人状态，正常/报验/非正常/清算，长度 20，非必填
        /// </summary>
        public string TaxpayerStatus { get; set; } = string.Empty;

        /// <summary>
        /// 登记序号，长度 32，非必填
        /// </summary>
        public string RegistrationNo { get; set; } = string.Empty;

        /// <summary>
        /// 登记日期，长度 10，非必填
        /// </summary>
        public string RegistrationDate { get; set; } = string.Empty;

        /// <summary>
        /// 办税人姓名，长度 50，非必填
        /// </summary>
        public string TaxOfficerName { get; set; } = string.Empty;

        /// <summary>
        /// 财务负责人姓名，长度 50，非必填
        /// </summary>
        public string FinancialOfficerName { get; set; } = string.Empty;

        /// <summary>
        /// 法定代表人姓名，长度 50，非必填
        /// </summary>
        public string LegalRepresentativeName { get; set; } = string.Empty;

        /// <summary>
        /// 行业代码，长度 10，非必填
        /// </summary>
        public string IndustryCode { get; set; } = string.Empty;

        /// <summary>
        /// 行业名称，长度 100，非必填
        /// </summary>
        public string IndustryName { get; set; } = string.Empty;

        /// <summary>
        /// 登记注册类型代码，长度 10，非必填
        /// </summary>
        public string RegistrationTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// 登记注册类型名称，长度 50，非必填
        /// </summary>
        public string RegistrationTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 经营地址，长度 255，非必填
        /// </summary>
        public string BusinessAddress { get; set; } = string.Empty;

        /// <summary>
        /// 注册地址，长度 255，非必填
        /// </summary>
        public string RegisteredAddress { get; set; } = string.Empty;

        /// <summary>
        /// 注册地联系电话，长度 20，非必填
        /// </summary>
        public string RegisteredAddressTelephone { get; set; } = string.Empty;

        /// <summary>
        /// 街道乡镇代码，长度 12，非必填
        /// </summary>
        public string StreetTownCode { get; set; } = string.Empty;

        /// <summary>
        /// 主管税务局代码，长度 6，非必填
        /// </summary>
        public string TaxBureauCode { get; set; } = string.Empty;

        /// <summary>
        /// 主管税务局名称，长度 100，非必填
        /// </summary>
        public string TaxBureauName { get; set; } = string.Empty;

        /// <summary>
        /// 主管税务所代码，长度 6，非必填
        /// </summary>
        public string TaxOfficeCode { get; set; } = string.Empty;

        /// <summary>
        /// 主管税务所名称，长度 100，非必填
        /// </summary>
        public string TaxOfficeName { get; set; } = string.Empty;

        /// <summary>
        /// 经营地址行政区划数字代码，长度 6，非必填
        /// </summary>
        public string BusinessAddressDistrictCode { get; set; } = string.Empty;
    }
}
