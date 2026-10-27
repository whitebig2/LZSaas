using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T0006 账户余额查询 —— 响应。</summary>
public class AccountQaccbalResponseV1 : ApipResponse
{
    /// <summary>结果集。</summary>
    [JsonPropertyName("ResultSet")]
    public List<ResultSetItem> ResultSet { get; set; }

    /// <summary>ResultSet 元素：余额结果。</summary>
    public class ResultSetItem
    {
        /// <summary>账户号。</summary>
        [JsonPropertyName("AcctNo")] public string AcctNo { get; set; }
        /// <summary>账户名称。</summary>
        [JsonPropertyName("AcctName")] public string AcctName { get; set; }
        /// <summary>余额。</summary>
        [JsonPropertyName("Balance")] public string Balance { get; set; }
        /// <summary>可用余额。</summary>
        [JsonPropertyName("UsableBalance")] public string UsableBalance { get; set; }
        /// <summary>冻结金额。</summary>
        [JsonPropertyName("FreezeAmount")] public string FreezeAmount { get; set; }
        /// <summary>余额日期。</summary>
        [JsonPropertyName("BalanceDate")] public string BalanceDate { get; set; }
        /// <summary>币种编码。</summary>
        [JsonPropertyName("CurrencyCode")] public string CurrencyCode { get; set; }
        /// <summary>币种名称。</summary>
        [JsonPropertyName("CurrencyName")] public string CurrencyName { get; set; }
    }
}
