using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T0023 认领结果查询 —— 响应。</summary>
public class ClaimQryclmrstResponseV1 : ApipResponse
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

    /// <summary>ResultSet 元素：认领结果。</summary>
    public class ResultSetItem
    {
        /// <summary>应收单已核销金额。</summary>
        [JsonPropertyName("ReceivedAmount")] public string ReceivedAmount { get; set; }
        /// <summary>应收单单据编号。</summary>
        [JsonPropertyName("ReceivableBusinessNo")] public string ReceivableBusinessNo { get; set; }
        /// <summary>认领单创建时间。</summary>
        [JsonPropertyName("CreateTime")] public string CreateTime { get; set; }
        /// <summary>收款单已认领金额。</summary>
        [JsonPropertyName("ReceiptClaimedAmount")] public string ReceiptClaimedAmount { get; set; }
        /// <summary>收款单外部系统编号。</summary>
        [JsonPropertyName("ReceiptExtBusinessNo")] public string ReceiptExtBusinessNo { get; set; }
        /// <summary>应收单单据状态。</summary>
        [JsonPropertyName("ClaimStatusDesc")] public string ClaimStatusDesc { get; set; }
        /// <summary>应收单应收金额。</summary>
        [JsonPropertyName("ReceivableTotalAmount")] public string ReceivableTotalAmount { get; set; }
        /// <summary>收款单单据状态。</summary>
        [JsonPropertyName("ReceiptBusinessStateDesc")] public string ReceiptBusinessStateDesc { get; set; }
        /// <summary>收款单财资云编号。</summary>
        [JsonPropertyName("ReceiptBusinessNo")] public string ReceiptBusinessNo { get; set; }
        /// <summary>应收单待核销金额。</summary>
        [JsonPropertyName("ReceivableWaitClaimAmount")] public string ReceivableWaitClaimAmount { get; set; }
        /// <summary>认领人。</summary>
        [JsonPropertyName("CreateByName")] public string CreateByName { get; set; }
        /// <summary>认领单单据编号。</summary>
        [JsonPropertyName("ClaimBusinessNo")] public string ClaimBusinessNo { get; set; }
        /// <summary>收款单单据总额。</summary>
        [JsonPropertyName("ReceiptTotalAmount")] public string ReceiptTotalAmount { get; set; }
        /// <summary>收款单待认领金额。</summary>
        [JsonPropertyName("ReceiptWaitClaimAmount")] public string ReceiptWaitClaimAmount { get; set; }
        /// <summary>认领状态。</summary>
        [JsonPropertyName("ClaimClaimStatusDesc")] public string ClaimClaimStatusDesc { get; set; }
        /// <summary>应收单外部系统编号。</summary>
        [JsonPropertyName("ExtBusinessNo")] public string ExtBusinessNo { get; set; }
        /// <summary>认领金额。</summary>
        [JsonPropertyName("ClaimTotalAmount")] public string ClaimTotalAmount { get; set; }
    }
}
