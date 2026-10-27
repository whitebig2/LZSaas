using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T1002 单笔付款 —— 响应。ResultSet 仅在受理失败时返回失败明细。</summary>
public class PaymentOrdinarypayResponseV1 : ApipResponse
{
    /// <summary>结果集（失败明细）。</summary>
    [JsonPropertyName("ResultSet")]
    public List<PaymentFailResultItem> ResultSet { get; set; }
}
