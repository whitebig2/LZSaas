using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T0001 交易流水信息查询 —— 响应。</summary>
public class AccountQhisdResponseV1 : ApipResponse
{
    /// <summary>下页页码，0 或空说明没有下一页，非 0 代表下次可查页码入参。</summary>
    [JsonPropertyName("NextTag")]
    public string NextTag { get; set; }

    /// <summary>结果集。</summary>
    [JsonPropertyName("ResultSet")]
    public List<ResultSetItem> ResultSet { get; set; }

    /// <summary>关联应付单据。</summary>
    public class PaymentBillItem
    {
        /// <summary>外部系统单据编号。</summary>
        [JsonPropertyName("BillsPayOrderNo")]
        public string BillsPayOrderNo { get; set; }
    }

    /// <summary>ResultSet 元素：流水结果。</summary>
    public class ResultSetItem
    {
        /// <summary>本方账户号码。</summary>
        [JsonPropertyName("AccountNo")] public string AccountNo { get; set; }
        /// <summary>本方账户名称。</summary>
        [JsonPropertyName("AccountName")] public string AccountName { get; set; }
        /// <summary>本方账户开户行。</summary>
        [JsonPropertyName("BranchName")] public string BranchName { get; set; }
        /// <summary>交易流水号。</summary>
        [JsonPropertyName("TransInfoNo")] public string TransInfoNo { get; set; }
        /// <summary>银行类别（银行直连编号，不是字典值）。</summary>
        [JsonPropertyName("BankType")] public string BankType { get; set; }
        /// <summary>币种编码。</summary>
        [JsonPropertyName("CurrencyType")] public string CurrencyType { get; set; }
        /// <summary>交易金额。</summary>
        [JsonPropertyName("TransAmount")] public string TransAmount { get; set; }
        /// <summary>交易后余额。</summary>
        [JsonPropertyName("TransBalance")] public string TransBalance { get; set; }
        /// <summary>收支方向：1付(借)，2收(贷)。</summary>
        [JsonPropertyName("TransDirection")] public string TransDirection { get; set; }
        /// <summary>对方账号。</summary>
        [JsonPropertyName("OppAccountNo")] public string OppAccountNo { get; set; }
        /// <summary>对方账户名。</summary>
        [JsonPropertyName("OppAccountName")] public string OppAccountName { get; set; }
        /// <summary>对方开户行。</summary>
        [JsonPropertyName("OppBankName")] public string OppBankName { get; set; }
        /// <summary>对方开户行地址。</summary>
        [JsonPropertyName("OppBankAddr")] public string OppBankAddr { get; set; }
        /// <summary>交易日期。</summary>
        [JsonPropertyName("TransTime")] public string TransTime { get; set; }
        /// <summary>摘要。</summary>
        [JsonPropertyName("Memo")] public string Memo { get; set; }
        /// <summary>用途。</summary>
        [JsonPropertyName("UseInfo")] public string UseInfo { get; set; }
        /// <summary>备注。</summary>
        [JsonPropertyName("Remark")] public string Remark { get; set; }
        /// <summary>交易抹账标识：1.被抹账 2.正常。</summary>
        [JsonPropertyName("DeleteByBank")] public string DeleteByBank { get; set; }
        /// <summary>关联应付单据信息。</summary>
        [JsonPropertyName("PaymentBills")] public List<PaymentBillItem> PaymentBills { get; set; }
    }
}
