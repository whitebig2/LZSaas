using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T0014 金融网点查询 —— 响应。</summary>
public class BankQuerybranchResponseV1 : ApipResponse
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

    /// <summary>ResultSet 元素：网点结果。</summary>
    public class ResultSetItem
    {
        /// <summary>银行名称。</summary>
        [JsonPropertyName("BankName")] public string BankName { get; set; }
        /// <summary>银行科目编号。</summary>
        [JsonPropertyName("SubjectCode")] public string SubjectCode { get; set; }
        /// <summary>英文名称。</summary>
        [JsonPropertyName("EnName")] public string EnName { get; set; }
        /// <summary>银行科目名称。</summary>
        [JsonPropertyName("SubjectName")] public string SubjectName { get; set; }
        /// <summary>国家。</summary>
        [JsonPropertyName("CountryName")] public string CountryName { get; set; }
        /// <summary>所属大行名称。</summary>
        [JsonPropertyName("BelongBankTypeName")] public string BelongBankTypeName { get; set; }
        /// <summary>直联编号。</summary>
        [JsonPropertyName("DirectCode")] public string DirectCode { get; set; }
        /// <summary>市。</summary>
        [JsonPropertyName("ExtCityName")] public string ExtCityName { get; set; }
        /// <summary>省。</summary>
        [JsonPropertyName("ExtProvinceName")] public string ExtProvinceName { get; set; }
        /// <summary>联行号。</summary>
        [JsonPropertyName("CnapsCode")] public string CnapsCode { get; set; }
        /// <summary>银行机构号。</summary>
        [JsonPropertyName("BranchCode")] public string BranchCode { get; set; }
    }
}
