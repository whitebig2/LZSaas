using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T0005 账户信息查询 —— 响应。</summary>
public class AccountQacctinfResponseV1 : ApipResponse
{
    /// <summary>下页页码，0 或空说明没有下一页，非 0 代表下次可查页码入参。</summary>
    [JsonPropertyName("NextTag")]
    public string NextTag { get; set; }

    /// <summary>总条数。</summary>
    [JsonPropertyName("Total")]
    public string Total { get; set; }

    /// <summary>结果集。</summary>
    [JsonPropertyName("ResultSet")]
    public List<ResultSetItem> ResultSet { get; set; }

    /// <summary>币种余额信息。</summary>
    public class CurrencyItem
    {
        /// <summary>币种编码。</summary>
        [JsonPropertyName("CurrencyCode")] public string CurrencyCode { get; set; }
        /// <summary>余额。</summary>
        [JsonPropertyName("Balance")] public string Balance { get; set; }
        /// <summary>余额日期。</summary>
        [JsonPropertyName("BalanceDate")] public string BalanceDate { get; set; }
        /// <summary>可用余额。</summary>
        [JsonPropertyName("UsableBalance")] public string UsableBalance { get; set; }
        /// <summary>币种名称。</summary>
        [JsonPropertyName("CurrencyName")] public string CurrencyName { get; set; }
    }

    /// <summary>使用单位信息。</summary>
    public class UseOrgItem
    {
        /// <summary>使用单位名称。</summary>
        [JsonPropertyName("UseOrgName")] public string UseOrgName { get; set; }
        /// <summary>使用单位 ID。</summary>
        [JsonPropertyName("UseOrgKey")] public string UseOrgKey { get; set; }
        /// <summary>使用单位编号。</summary>
        [JsonPropertyName("UseOrgCode")] public string UseOrgCode { get; set; }
    }

    /// <summary>ResultSet 元素：账户信息。</summary>
    public class ResultSetItem
    {
        /// <summary>是否加入资金池：1是 2否。</summary>
        [JsonPropertyName("CashPoolFlag")] public string CashPoolFlag { get; set; }
        /// <summary>开户区域名称。</summary>
        [JsonPropertyName("CountryName")] public string CountryName { get; set; }
        /// <summary>开户行所在城市编码。</summary>
        [JsonPropertyName("AcctCityCode")] public string AcctCityCode { get; set; }
        /// <summary>法人单位名称。</summary>
        [JsonPropertyName("LegalPersonOrgName")] public string LegalPersonOrgName { get; set; }
        /// <summary>账户性质：1基本存款账户；2一般存款账户；3专用存款账户；4临时存款账户。</summary>
        [JsonPropertyName("AcctNature")] public string AcctNature { get; set; }
        /// <summary>合作金融机构名称。</summary>
        [JsonPropertyName("CooperativeInstitutionName")] public string CooperativeInstitutionName { get; set; }
        /// <summary>账户号。</summary>
        [JsonPropertyName("AcctNo")] public string AcctNo { get; set; }
        /// <summary>账户开户日期。</summary>
        [JsonPropertyName("AcctOpenDate")] public string AcctOpenDate { get; set; }
        /// <summary>业务模块使用的账户类型编号（1银行账户 2内部账户）。</summary>
        [JsonPropertyName("BusiAcctTypeCode")] public string BusiAcctTypeCode { get; set; }
        /// <summary>是否开通网银：1是 2否。</summary>
        [JsonPropertyName("AcctEbankType")] public string AcctEbankType { get; set; }
        /// <summary>英文银行地址1。</summary>
        [JsonPropertyName("EnLocation1")] public string EnLocation1 { get; set; }
        /// <summary>swift/bic 码。</summary>
        [JsonPropertyName("SwiftCode")] public string SwiftCode { get; set; }
        /// <summary>开户行所在省名称。</summary>
        [JsonPropertyName("AcctProvinceName")] public string AcctProvinceName { get; set; }
        /// <summary>开户区域编码。</summary>
        [JsonPropertyName("CountryCode")] public string CountryCode { get; set; }
        /// <summary>合作金融机构网点编号。</summary>
        [JsonPropertyName("CooperativeFinancNetCode")] public string CooperativeFinancNetCode { get; set; }
        /// <summary>英文银行地址3。</summary>
        [JsonPropertyName("EnLocation3")] public string EnLocation3 { get; set; }
        /// <summary>英文银行地址2。</summary>
        [JsonPropertyName("EnLocation2")] public string EnLocation2 { get; set; }
        /// <summary>币种余额信息集合。</summary>
        [JsonPropertyName("CurrencyList")] public List<CurrencyItem> CurrencyList { get; set; }
        /// <summary>银行全地址（国外）。</summary>
        [JsonPropertyName("AbroadAddressDetail")] public string AbroadAddressDetail { get; set; }
        /// <summary>收付属性：1收入户、2支出户、3收支户。</summary>
        [JsonPropertyName("PayReceiveAttr")] public string PayReceiveAttr { get; set; }
        /// <summary>法人单位编号。</summary>
        [JsonPropertyName("LegalPersonOrgCode")] public string LegalPersonOrgCode { get; set; }
        /// <summary>使用单位信息集合。</summary>
        [JsonPropertyName("UseOrgList")] public List<UseOrgItem> UseOrgList { get; set; }
        /// <summary>直连属性：1直连 2非直连 3swift。</summary>
        [JsonPropertyName("AcctInWay")] public string AcctInWay { get; set; }
        /// <summary>合作金融机构编码。</summary>
        [JsonPropertyName("CooperativeInstitutionCode")] public string CooperativeInstitutionCode { get; set; }
        /// <summary>账户名称。</summary>
        [JsonPropertyName("AcctName")] public string AcctName { get; set; }
        /// <summary>账户状态：1正常；2变更中；3销户中；4已销户。</summary>
        [JsonPropertyName("AcctStatus")] public string AcctStatus { get; set; }
        /// <summary>直联编号。</summary>
        [JsonPropertyName("DirectCode")] public string DirectCode { get; set; }
        /// <summary>开户行所在城市名称。</summary>
        [JsonPropertyName("AcctCityName")] public string AcctCityName { get; set; }
        /// <summary>开户行所在省编码。</summary>
        [JsonPropertyName("AcctProvinceCode")] public string AcctProvinceCode { get; set; }
        /// <summary>合作金融机构网点名称。</summary>
        [JsonPropertyName("CooperativeFinancNetName")] public string CooperativeFinancNetName { get; set; }
        /// <summary>业务模块使用的账户类型名称（1银行账户 2内部账户）。</summary>
        [JsonPropertyName("BusiAcctTypeName")] public string BusiAcctTypeName { get; set; }
    }
}
