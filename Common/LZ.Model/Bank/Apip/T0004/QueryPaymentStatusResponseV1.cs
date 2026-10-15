using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0004 应付单业务状态查询 —— 响应（对应 Java 版 QueryPaymentStatusResponseV1）。
///
/// 公共参数（继承自 <see cref="ApipResponse"/>，网关在业务报文内使用驼峰命名）：
/// returnCode 返回码（0 = 成功，网关以数字字符串上送如 "0000"）、returnMsg 返回码说明、msgId 消息编号；
/// 业务数据在 <see cref="ResultSet"/> 结果集中。
/// 字段说明见 docs/T0004-应付单业务状态查询V1API示例文档.md。
/// </summary>
public class QueryPaymentStatusResponseV1 : ApipResponse
{
    /// <summary>结果集（list，必输）。</summary>
    [JsonPropertyName("ResultSet")]
    public List<ResultSetItem> ResultSet { get; set; }

    /// <summary>DraftList 元素：票据明细。</summary>
    public class PaymentResponseDraftSubBO
    {
        /// <summary>财资云应付单号（str，最大 64，必输），如 YF26011611201576003001。</summary>
        [JsonPropertyName("OrderNo")] public string OrderNo { get; set; }
        /// <summary>业务状态（str，最大 8，必输），如 1=支付中。</summary>
        [JsonPropertyName("PayStatus")] public string PayStatus { get; set; }
        /// <summary>业务状态名称（str，最大 20，必输），如 支付中。</summary>
        [JsonPropertyName("PayStatusName")] public string PayStatusName { get; set; }
        /// <summary>失败原因（str，最大 512，必输）。</summary>
        [JsonPropertyName("FailReasons")] public string FailReasons { get; set; }
        /// <summary>实际付款日期（str，最大 10，选输），格式 yyyy-mm-dd。</summary>
        [JsonPropertyName("RelPaymentDate")] public string RelPaymentDate { get; set; }
        /// <summary>实际支付金额（str，最大 25，必输）。</summary>
        [JsonPropertyName("PayAmount")] public string PayAmount { get; set; }
        /// <summary>出票人账号（str，最大 64，选输）。</summary>
        [JsonPropertyName("PayAccountNo")] public string PayAccountNo { get; set; }
        /// <summary>出票人名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("PayAccountName")] public string PayAccountName { get; set; }
        /// <summary>出票人开户行行号（str，最大 64，选输）。</summary>
        [JsonPropertyName("PayBranchCode")] public string PayBranchCode { get; set; }
        /// <summary>出票人开户行名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("PayBranchName")] public string PayBranchName { get; set; }
        /// <summary>承兑人账号（str，最大 64，选输）。</summary>
        [JsonPropertyName("AcceptorAccountNo")] public string AcceptorAccountNo { get; set; }
        /// <summary>承兑人名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("AcceptorAccountName")] public string AcceptorAccountName { get; set; }
        /// <summary>承兑人开户行行号（str，最大 64，选输）。</summary>
        [JsonPropertyName("AcceptorBankCode")] public string AcceptorBankCode { get; set; }
        /// <summary>承兑人开户行名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("AcceptorBankName")] public string AcceptorBankName { get; set; }
        /// <summary>收票人账号（str，最大 64，选输）。</summary>
        [JsonPropertyName("ReceiveAccountNo")] public string ReceiveAccountNo { get; set; }
        /// <summary>收票人名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("ReceiveAccountName")] public string ReceiveAccountName { get; set; }
        /// <summary>收票人开户行行号（str，最大 64，选输）。</summary>
        [JsonPropertyName("ReceiveBranchCode")] public string ReceiveBranchCode { get; set; }
        /// <summary>收票人开户行名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("ReceiveBranchName")] public string ReceiveBranchName { get; set; }
        /// <summary>背书人账号（str，最大 64，选输）。</summary>
        [JsonPropertyName("EndorseAccountNo")] public string EndorseAccountNo { get; set; }
        /// <summary>背书人名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("EndorseAccountName")] public string EndorseAccountName { get; set; }
        /// <summary>背书人开户行行号（str，最大 64，选输）。</summary>
        [JsonPropertyName("EndorseBankCode")] public string EndorseBankCode { get; set; }
        /// <summary>背书人开户行名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("EndorseBankName")] public string EndorseBankName { get; set; }
        /// <summary>被背书人账号（str，最大 64，选输）。</summary>
        [JsonPropertyName("EndorserAccountNo")] public string EndorserAccountNo { get; set; }
        /// <summary>被背书人名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("EndorserAccountName")] public string EndorserAccountName { get; set; }
        /// <summary>被背书人开户行行号（str，最大 64，选输）。</summary>
        [JsonPropertyName("EndorserBankCode")] public string EndorserBankCode { get; set; }
        /// <summary>被背书人开户行名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("EndorserBankName")] public string EndorserBankName { get; set; }
        /// <summary>票据（包）号（str，最大 32，选输）。</summary>
        [JsonPropertyName("DraftCode")] public string DraftCode { get; set; }
        /// <summary>子票区间（str，最大 25，选输）。</summary>
        [JsonPropertyName("DraftRange")] public string DraftRange { get; set; }
        /// <summary>出票日（str，最大 10，选输）。</summary>
        [JsonPropertyName("DrawDate")] public string DrawDate { get; set; }
        /// <summary>到期日（str，最大 10，选输）。</summary>
        [JsonPropertyName("DueDate")] public string DueDate { get; set; }
        /// <summary>票据介质（str，最大 8，选输）。</summary>
        [JsonPropertyName("BillsProperty")] public string BillsProperty { get; set; }
        /// <summary>票据类型（str，最大 8，选输）。</summary>
        [JsonPropertyName("BillsType")] public string BillsType { get; set; }
        /// <summary>转让标识（str，最大 8，选输）。</summary>
        [JsonPropertyName("CanTransfer")] public string CanTransfer { get; set; }
        /// <summary>消息类型（str，最大 8，选输）。</summary>
        [JsonPropertyName("MessageType")] public string MessageType { get; set; }
    }

    /// <summary>BatchList 元素：子单明细（普通支付子单）。</summary>
    public class PaymentResponseSettSubBO
    {
        /// <summary>外部系统单据编号（str，最大 64，必输）。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>财资云应付单号（str，最大 64，必输），如 YF26011611201576003001。</summary>
        [JsonPropertyName("OrderNo")] public string OrderNo { get; set; }
        /// <summary>业务状态（str，最大 8，必输），如 1=支付中。网关以字符串上送（如 "10"），勿用 int。</summary>
        [JsonPropertyName("PayStatus")] public string PayStatus { get; set; }
        /// <summary>业务状态名称（str，最大 20，必输），如 支付中。</summary>
        [JsonPropertyName("PayStatusName")] public string PayStatusName { get; set; }
        /// <summary>失败原因（str，最大 512，必输）。</summary>
        [JsonPropertyName("FailReasons")] public string FailReasons { get; set; }
        /// <summary>实际付款日期（str，最大 10，选输），格式 yyyy-mm-dd。</summary>
        [JsonPropertyName("RelPaymentDate")] public string RelPaymentDate { get; set; }
        /// <summary>实际支付金额（str，最大 25，必输）。</summary>
        [JsonPropertyName("PayAmount")] public string PayAmount { get; set; }
        /// <summary>合计退款金额（str，最大 25，选输）。</summary>
        [JsonPropertyName("RefundAmount")] public string RefundAmount { get; set; }
        /// <summary>最近退款日期（str，最大 10，选输），格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("RefundDate")] public string RefundDate { get; set; }
        /// <summary>支付失败银行反馈信息（str，最大 512，选输）。</summary>
        [JsonPropertyName("PayFailReason")] public string PayFailReason { get; set; }
        /// <summary>收款方银行账号（str，最大 64，选输）。</summary>
        [JsonPropertyName("ReceiveAccountNo")] public string ReceiveAccountNo { get; set; }
        /// <summary>收款方账户名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("ReceiveAccountName")] public string ReceiveAccountName { get; set; }
        /// <summary>收款方开户行所属省(名称)（str，最大 256，选输）。</summary>
        [JsonPropertyName("ReceiveAreaNameOfProvince")] public string ReceiveAreaNameOfProvince { get; set; }
        /// <summary>收款方开户行所属市(名称)（str，最大 256，选输）。</summary>
        [JsonPropertyName("ReceiveAreaNameOfCity")] public string ReceiveAreaNameOfCity { get; set; }
        /// <summary>收款方开户行行号（str，最大 64，选输）。</summary>
        [JsonPropertyName("ReceiveBranchCode")] public string ReceiveBranchCode { get; set; }
        /// <summary>收款方开户行名称（str，最大 256，选输）。</summary>
        [JsonPropertyName("ReceiveBranchName")] public string ReceiveBranchName { get; set; }
        /// <summary>收款方性质（str，最大 8，选输），1=公司，2=个人。</summary>
        [JsonPropertyName("ReceiveNature")] public string ReceiveNature { get; set; }
        /// <summary>收款账户币种编号（str，最大 64，选输），如 CNY。</summary>
        [JsonPropertyName("ReceiveCurrencyType")] public string ReceiveCurrencyType { get; set; }
        /// <summary>收款账户币种名称（str，最大 256，选输），如 人民币。</summary>
        [JsonPropertyName("ReceiveCurrencyName")] public string ReceiveCurrencyName { get; set; }
        /// <summary>摘要（str，最大 512，选输）。</summary>
        [JsonPropertyName("PayUse")] public string PayUse { get; set; }
        /// <summary>用途（str，最大 512，选输）。</summary>
        [JsonPropertyName("Purpose")] public string Purpose { get; set; }
        /// <summary>银行流水号（list，选输）。</summary>
        [JsonPropertyName("TransInfoNoList")] public List<string> TransInfoNoList { get; set; }
    }

    /// <summary>ResultSet 元素：查询结果（按外部系统单据编号一条）。</summary>
    public class ResultSetItem
    {
        /// <summary>外部系统单据编号（str，最大 64，必输）。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>财资云应付单号（str，最大 64，必输），如 YF26011611201576003001。</summary>
        [JsonPropertyName("RelBillsCode")] public string RelBillsCode { get; set; }
        /// <summary>外部系统单据流水号（str，最大 64，选输）。</summary>
        [JsonPropertyName("SerialNo")] public string SerialNo { get; set; }
        /// <summary>业务状态（str，最大 8，必输），如 1=支付中。网关以字符串上送（如 "10"），勿用 int。</summary>
        [JsonPropertyName("PayStatus")] public string PayStatus { get; set; }
        /// <summary>业务状态名称（str，最大 20，必输），如 支付中。</summary>
        [JsonPropertyName("PayStatusName")] public string PayStatusName { get; set; }
        /// <summary>实际付款日期（str，最大 10，选输），格式 yyyy-mm-dd。</summary>
        [JsonPropertyName("RelPaymentDate")] public string RelPaymentDate { get; set; }
        /// <summary>失败原因（str，最大 512，必输）。</summary>
        [JsonPropertyName("FailReasons")] public string FailReasons { get; set; }
        /// <summary>实际支付金额（str，最大 25，必输）。</summary>
        [JsonPropertyName("PayAmount")] public string PayAmount { get; set; }
        /// <summary>应付单总金额（str，最大 25，选输）。</summary>
        [JsonPropertyName("Amount")] public string Amount { get; set; }
        /// <summary>合计退款金额（str，最大 25，选输）。</summary>
        [JsonPropertyName("RefundAmount")] public string RefundAmount { get; set; }
        /// <summary>最近退款日期（str，最大 10，选输），格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("RefundDate")] public string RefundDate { get; set; }
        /// <summary>实际付款类型（str，最大 8，必输）。</summary>
        [JsonPropertyName("PayType")] public string PayType { get; set; }
        /// <summary>支付失败银行反馈信息（str，最大 512，选输）。</summary>
        [JsonPropertyName("PayFailReason")] public string PayFailReason { get; set; }
        /// <summary>交易流水号集合（Array，选输）。</summary>
        [JsonPropertyName("TransInfoNoList")] public List<string> TransInfoNoList { get; set; }
        /// <summary>子单明细（List，选输），元素见 <see cref="PaymentResponseSettSubBO"/>。</summary>
        [JsonPropertyName("BatchList")] public List<PaymentResponseSettSubBO> BatchList { get; set; }
        /// <summary>票据明细（List，选输），元素见 <see cref="PaymentResponseDraftSubBO"/>。</summary>
        [JsonPropertyName("DraftList")] public List<PaymentResponseDraftSubBO> DraftList { get; set; }
    }
}
