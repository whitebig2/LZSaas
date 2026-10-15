using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T0039 单笔回单查询 —— 响应。</summary>
public class QuerySingleElectronicReceiptResponseV1 : ApipResponse
{
    /// <summary>结果集。</summary>
    [JsonPropertyName("ResultSet")]
    public List<ResultSetItem> ResultSet { get; set; }

    /// <summary>ResultSet 元素：回单结果。</summary>
    public class ResultSetItem
    {
        /// <summary>文件名。</summary>
        [JsonPropertyName("FileName")]
        public string FileName { get; set; }

        /// <summary>文件内容（Base64）。</summary>
        [JsonPropertyName("FileContent")]
        public string FileContent { get; set; }
    }
}
