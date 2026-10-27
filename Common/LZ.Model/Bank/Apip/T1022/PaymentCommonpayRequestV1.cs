using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T1022 批量付款 —— 请求。
/// 接口路径：POST /api/cse/tmc/payment/commonpay/V1
/// </summary>
public class PaymentCommonpayRequestV1 : ApipRequest<PaymentCommonpayResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/payment/commonpay/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T1022";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>收款方信息。</summary>
    public class PaymentCommonpayRequestPaymentRec
    {
        /// <summary>外部系统单据编号。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>付款金额。</summary>
        [JsonPropertyName("PrivateAmount")] public string PrivateAmount { get; set; }
        /// <summary>收款方银行账号。</summary>
        [JsonPropertyName("ReceiveAccountNo")] public string ReceiveAccountNo { get; set; }
        /// <summary>收款方账户名称。</summary>
        [JsonPropertyName("ReceiveAccountName")] public string ReceiveAccountName { get; set; }
        /// <summary>收款方开户行所属省。</summary>
        [JsonPropertyName("ReceiveAreaNameOfProvince")] public string ReceiveAreaNameOfProvince { get; set; }
        /// <summary>收款方开户行所属市。</summary>
        [JsonPropertyName("ReceiveAreaNameOfCity")] public string ReceiveAreaNameOfCity { get; set; }
        /// <summary>收款方开户行行号。</summary>
        [JsonPropertyName("ReceiveBranchCode")] public string ReceiveBranchCode { get; set; }
        /// <summary>收款方开户行名称。</summary>
        [JsonPropertyName("ReceiveBranchName")] public string ReceiveBranchName { get; set; }
        /// <summary>收款方性质。</summary>
        [JsonPropertyName("ReceiveNature")] public string ReceiveNature { get; set; }
        /// <summary>收款账户币种编号。</summary>
        [JsonPropertyName("ReceiveCurrencyCode")] public string ReceiveCurrencyCode { get; set; }
        /// <summary>收款账户币种名称。</summary>
        [JsonPropertyName("ReceiveCurrencyName")] public string ReceiveCurrencyName { get; set; }
        /// <summary>摘要。</summary>
        [JsonPropertyName("PayUse")] public string PayUse { get; set; }
        /// <summary>用途。</summary>
        [JsonPropertyName("Purpose")] public string Purpose { get; set; }
    }

    /// <summary>PaymentCommonpayRequestParamSet：请求参数。</summary>
    public class PaymentCommonpayRequestParamSet
    {
        /// <summary>外部系统单据编号。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>付款日期。</summary>
        [JsonPropertyName("PaymentDate")] public string PaymentDate { get; set; }
        /// <summary>业务类型编号。</summary>
        [JsonPropertyName("BusinessTypeCode")] public string BusinessTypeCode { get; set; }
        /// <summary>业务类型名称。</summary>
        [JsonPropertyName("BusinessTypeName")] public string BusinessTypeName { get; set; }
        /// <summary>付款单位编号。</summary>
        [JsonPropertyName("PayOrgCode")] public string PayOrgCode { get; set; }
        /// <summary>付款单位名称。</summary>
        [JsonPropertyName("PayOrgName")] public string PayOrgName { get; set; }
        /// <summary>付款方银行账号、虚拟账号。</summary>
        [JsonPropertyName("PayAccountNo")] public string PayAccountNo { get; set; }
        /// <summary>虚拟账号对应的结算账号，虚拟账户付款时必输。</summary>
        [JsonPropertyName("MainAcctNo")] public string MainAcctNo { get; set; }
        /// <summary>支付方式：1直联支付，2线下支付。</summary>
        [JsonPropertyName("PayChannel")] public string PayChannel { get; set; }
        /// <summary>合计付款金额。</summary>
        [JsonPropertyName("SumPayAmount")] public string SumPayAmount { get; set; }
        /// <summary>币种编码。</summary>
        [JsonPropertyName("PayCurrencyType")] public string PayCurrencyType { get; set; }
        /// <summary>币种名称。</summary>
        [JsonPropertyName("PayCurrencyName")] public string PayCurrencyName { get; set; }
        /// <summary>是否加急：1是，2否。</summary>
        [JsonPropertyName("WhetherUrgent")] public string WhetherUrgent { get; set; }
        /// <summary>备注。</summary>
        [JsonPropertyName("Note")] public string Note { get; set; }
        /// <summary>附言。</summary>
        [JsonPropertyName("Remark")] public string Remark { get; set; }
        /// <summary>收款方信息。</summary>
        [JsonPropertyName("PaymentRecList")] public List<PaymentCommonpayRequestPaymentRec> PaymentRecList { get; set; }

        public static PaymentCommonpayRequestParamSet Sample()
        {
            PaymentCommonpayRequestPaymentRec rec = new PaymentCommonpayRequestPaymentRec();
            rec.BillsPayOrderNo = "20260204002200070001";
            rec.PrivateAmount = "3";
            rec.ReceiveAccountNo = "0200000319201908655";
            rec.ReceiveAccountName = "出策涤势婚平频识";
            rec.ReceiveAreaNameOfProvince = "北京";
            rec.ReceiveAreaNameOfCity = "北京市";
            rec.ReceiveBranchCode = "102100000030";
            rec.ReceiveBranchName = "中国工商银行股份有限公司北京市分行营业部";
            rec.ReceiveNature = "1";
            rec.ReceiveCurrencyCode = "CNY";
            rec.ReceiveCurrencyName = "人民币";
            rec.PayUse = "摘要";
            rec.Purpose = "用途";

            PaymentCommonpayRequestParamSet item = new PaymentCommonpayRequestParamSet();
            item.BillsPayOrderNo = "2026020400220007";
            item.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd");
            item.BusinessTypeName = "外部业务类型";
            item.PayOrgName = "刘伟企业用户1";
            item.PayChannel = "2";
            item.SumPayAmount = "3";
            item.PayCurrencyType = "CNY";
            item.PayCurrencyName = "人民币";
            item.WhetherUrgent = "2";
            item.Note = "备注";
            item.Remark = "附言";
            item.PaymentRecList = new List<PaymentCommonpayRequestPaymentRec> { rec };
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class PaymentCommonpayRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T1022。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="PaymentCommonpayRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<PaymentCommonpayRequestParamSet> ParamSet { get; set; }
    }
}
