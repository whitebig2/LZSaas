using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T1028 电子回单查询 —— 响应。</summary>
public class BillQueryelectronicreceiptResponseV1 : ApipResponse
{
    /// <summary>返回总条数。</summary>
    [JsonPropertyName("TotalNum")]
    public string TotalNum { get; set; }

    /// <summary>下页页码，0 或空说明没有下一页，非 0 代表下次可查页码入参。</summary>
    [JsonPropertyName("NextTag")]
    public string NextTag { get; set; }

    /// <summary>结果集。</summary>
    [JsonPropertyName("ResultSet")]
    public List<ResultSetItem> ResultSet { get; set; }

    /// <summary>ResultSet 元素：回单结果。</summary>
    public class ResultSetItem
    {
        /// <summary>文件内容（Base64）。</summary>
        [JsonPropertyName("FileContent")] public string FileContent { get; set; }
        /// <summary>外部系统应付单编号。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>本系统内部流水号。</summary>
        [JsonPropertyName("ReceiptInnerNo")] public string ReceiptInnerNo { get; set; }
        /// <summary>文件名。</summary>
        [JsonPropertyName("FileName")] public string FileName { get; set; }
        /// <summary>查询批次号。</summary>
        [JsonPropertyName("ReceiptBatchNo")] public string ReceiptBatchNo { get; set; }
        /// <summary>本系统付款单编号。</summary>
        [JsonPropertyName("RelBillsCode")] public string RelBillsCode { get; set; }
    }
}
