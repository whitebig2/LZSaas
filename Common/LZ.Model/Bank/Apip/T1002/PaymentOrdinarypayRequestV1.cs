using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T1002 单笔付款 —— 请求。
/// 接口路径：POST /api/cse/tmc/payment/ordinarypay/V1
/// </summary>
public class PaymentOrdinarypayRequestV1 : ApipRequest<PaymentOrdinarypayResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/payment/ordinarypay/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T1002";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>收款方信息。</summary>
    public class PaymentOrdinarypayRequestPaymentRec
    {
        /// <summary>收款方单位编号。</summary>
        [JsonPropertyName("ReceiveOrgCode")] public string ReceiveOrgCode { get; set; }
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
    }

    /// <summary>PaymentOrdinarypayRequestParamSet：请求参数。</summary>
    public class PaymentOrdinarypayRequestParamSet
    {
        /// <summary>外部系统单据编号。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>付款类型，固定值传 1。</summary>
        [JsonPropertyName("PayType")] public string PayType { get; set; }
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
        /// <summary>付款金额。</summary>
        [JsonPropertyName("PayAmount")] public string PayAmount { get; set; }
        /// <summary>币种编码。</summary>
        [JsonPropertyName("PayCurrencyType")] public string PayCurrencyType { get; set; }
        /// <summary>币种名称。</summary>
        [JsonPropertyName("PayCurrencyName")] public string PayCurrencyName { get; set; }
        /// <summary>是否加急：1是，2否。</summary>
        [JsonPropertyName("WhetherUrgent")] public string WhetherUrgent { get; set; }
        /// <summary>附言。</summary>
        [JsonPropertyName("Remark")] public string Remark { get; set; }
        /// <summary>摘要。</summary>
        [JsonPropertyName("PayUse")] public string PayUse { get; set; }
        /// <summary>用途。</summary>
        [JsonPropertyName("Purpose")] public string Purpose { get; set; }
        /// <summary>对私类型。</summary>
        [JsonPropertyName("BusCode")] public string BusCode { get; set; }
        /// <summary>是否落地。</summary>
        [JsonPropertyName("whetherGround")] public string WhetherGround { get; set; }
        /// <summary>收款方信息。</summary>
        [JsonPropertyName("PaymentRec")] public PaymentOrdinarypayRequestPaymentRec PaymentRec { get; set; }

        public static PaymentOrdinarypayRequestParamSet Sample()
        {
            PaymentOrdinarypayRequestPaymentRec rec = new PaymentOrdinarypayRequestPaymentRec();
            rec.ReceiveAccountNo = "530381199100394";
            rec.ReceiveAccountName = "天津瀛海天淇农牧发展有限公司394";
            rec.ReceiveBranchName = "招商银行股份有限公司北京月坛支行";

            PaymentOrdinarypayRequestParamSet item = new PaymentOrdinarypayRequestParamSet();
            item.BillsPayOrderNo = "2026011410020004";
            item.PayType = "1";
            item.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd");
            item.BusinessTypeName = "外部业务类型";
            item.PayOrgName = "刘伟企业用户1";
            item.PayAccountNo = "0200000309223101034";
            item.PayChannel = "2";
            item.PayAmount = "100";
            item.WhetherUrgent = "1";
            item.Remark = "附言";
            item.PayUse = "摘要";
            item.Purpose = "用途";
            item.WhetherGround = "1";
            item.PaymentRec = rec;
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class PaymentOrdinarypayRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T1002。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="PaymentOrdinarypayRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<PaymentOrdinarypayRequestParamSet> ParamSet { get; set; }
    }
}
