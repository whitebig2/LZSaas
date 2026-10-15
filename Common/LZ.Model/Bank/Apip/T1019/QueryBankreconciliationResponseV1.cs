using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>T1019 余额对账信息查询 —— 响应。</summary>
public class QueryBankreconciliationResponseV1 : ApipResponse
{
    /// <summary>结果集。</summary>
    [JsonPropertyName("ResultSet")]
    public List<ResultSetItem> ResultSet { get; set; }

    /// <summary>ResultSet 元素：对账结果。</summary>
    public class ResultSetItem
    {
        /// <summary>防伪码。</summary>
        [JsonPropertyName("VerifyCode")] public string VerifyCode { get; set; }
        /// <summary>对账单编号。</summary>
        [JsonPropertyName("OrderNo")] public string OrderNo { get; set; }
        /// <summary>账户种类：1往来户、2贷款户、3定期户、4保证金户、5内部户、6个人贷款综合账户、9其他。</summary>
        [JsonPropertyName("AcctType")] public string AcctType { get; set; }
        /// <summary>币种。</summary>
        [JsonPropertyName("CurrType")] public string CurrType { get; set; }
        /// <summary>余额，单位：分。</summary>
        [JsonPropertyName("Balance")] public string Balance { get; set; }
        /// <summary>定期存款存期。</summary>
        [JsonPropertyName("FixDate")] public string FixDate { get; set; }
        /// <summary>到期日，格式 yyyyMMdd。</summary>
        [JsonPropertyName("DeadDate")] public string DeadDate { get; set; }
        /// <summary>存单号码。</summary>
        [JsonPropertyName("DepositNo")] public string DepositNo { get; set; }
        /// <summary>对帐情况：1平，2不平，3未对账。</summary>
        [JsonPropertyName("CheckFlag")] public string CheckFlag { get; set; }
        /// <summary>明细对账结果。</summary>
        [JsonPropertyName("CheckResult")] public string CheckResult { get; set; }
        /// <summary>余额调节表编制情况：1已编制 2未编制。</summary>
        [JsonPropertyName("BalanceFlag")] public string BalanceFlag { get; set; }
        /// <summary>备注。</summary>
        [JsonPropertyName("UserRemark")] public string UserRemark { get; set; }
        /// <summary>借据号。</summary>
        [JsonPropertyName("CreditNo")] public string CreditNo { get; set; }
        /// <summary>本金，单位：分。</summary>
        [JsonPropertyName("Corpus")] public string Corpus { get; set; }
        /// <summary>欠息，单位：分。</summary>
        [JsonPropertyName("BackInterest")] public string BackInterest { get; set; }
        /// <summary>贷款到息日，格式 yyyyMMdd。</summary>
        [JsonPropertyName("DebitDate")] public string DebitDate { get; set; }
        /// <summary>账号。</summary>
        [JsonPropertyName("AcctNo")] public string AcctNo { get; set; }
        /// <summary>户名。</summary>
        [JsonPropertyName("AcctName")] public string AcctName { get; set; }
        /// <summary>借贷标志：1借、2贷。</summary>
        [JsonPropertyName("Drcrf")] public string Drcrf { get; set; }
        /// <summary>对账单生成日期，格式 yyyyMMdd。</summary>
        [JsonPropertyName("DzgenDate")] public string DzgenDate { get; set; }
    }
}
