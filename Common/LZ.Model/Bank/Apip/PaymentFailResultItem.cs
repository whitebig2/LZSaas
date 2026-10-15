using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>付款类接口失败明细（T1002 / T1003 / T1018 / T1022 的 ResultSet 元素）。</summary>
public class PaymentFailResultItem
{
    /// <summary>外部系统单据编号（str，最大 64）。</summary>
    [JsonPropertyName("BillsPayOrderNo")]
    public string BillsPayOrderNo { get; set; }

    /// <summary>错误描述。</summary>
    [JsonPropertyName("FailReasons")]
    public string FailReasons { get; set; }
}
