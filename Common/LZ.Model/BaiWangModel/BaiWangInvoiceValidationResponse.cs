using Newtonsoft.Json.Linq;

namespace LZ.Model.BaiWangModel
{
    /// <summary>
    /// 百望云发票真伪查验结果；完整票面字段会随发票类型变化。
    /// </summary>
    public class BaiWangInvoiceValidationResponse
    {
        /// <summary>百望云发票类型代码。</summary>
        public string? invoiceType { get; set; }

        /// <summary>完整票面信息，包含发票状态、金额、购销方及明细等字段。</summary>
        public JObject? data { get; set; }
    }
}
