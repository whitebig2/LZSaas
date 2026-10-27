using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T1003 工资代发 —— 请求。
/// 接口路径：POST /api/cse/tmc/bill/batchPrivate/V1
/// </summary>
public class BillBatchPrivateRequestV1 : ApipRequest<BillBatchPrivateResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/bill/batchPrivate/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T1003";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>收款方信息。</summary>
    public class BillBatchPrivateRequestBatchPrivateRec
    {
        /// <summary>外部系统单据编号。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>批量对私金额。</summary>
        [JsonPropertyName("PrivateAmount")] public string PrivateAmount { get; set; }
        /// <summary>收款方银行账号。</summary>
        [JsonPropertyName("ReceiveAccountNo")] public string ReceiveAccountNo { get; set; }
        /// <summary>收款方账户名称。</summary>
        [JsonPropertyName("ReceiveAccountName")] public string ReceiveAccountName { get; set; }
        /// <summary>收款方开户行所属省(名称)。</summary>
        [JsonPropertyName("ReceiveAreaNameOfProvince")] public string ReceiveAreaNameOfProvince { get; set; }
        /// <summary>收款方开户行所属市。</summary>
        [JsonPropertyName("ReceiveAreaNameOfCity")] public string ReceiveAreaNameOfCity { get; set; }
        /// <summary>收款方开户行行号。</summary>
        [JsonPropertyName("ReceiveBranchCode")] public string ReceiveBranchCode { get; set; }
        /// <summary>收款方开户行名称。</summary>
        [JsonPropertyName("ReceiveBranchName")] public string ReceiveBranchName { get; set; }
        /// <summary>收款方性质：1公司 2个人。</summary>
        [JsonPropertyName("ReceiveNature")] public string ReceiveNature { get; set; }
        /// <summary>收款账户币种编号。</summary>
        [JsonPropertyName("ReceiveCurrencyCode")] public string ReceiveCurrencyCode { get; set; }
        /// <summary>收款账户币种名称。</summary>
        [JsonPropertyName("ReceiveCurrencyName")] public string ReceiveCurrencyName { get; set; }
    }

    /// <summary>BillBatchPrivateRequestParamSet：请求参数。</summary>
    public class BillBatchPrivateRequestParamSet
    {
        /// <summary>外部系统单据编号。</summary>
        [JsonPropertyName("BillsPayOrderNo")] public string BillsPayOrderNo { get; set; }
        /// <summary>付款类型：1普通付款 2批量对私 6现金付款 9 TT付款 10资金调拨 16票据开票 17票据背书。</summary>
        [JsonPropertyName("PayType")] public string PayType { get; set; }
        /// <summary>付款日期，格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("PaymentDate")] public string PaymentDate { get; set; }
        /// <summary>业务类型编号。</summary>
        [JsonPropertyName("BusinessTypeCode")] public string BusinessTypeCode { get; set; }
        /// <summary>业务类型名称。</summary>
        [JsonPropertyName("BusinessTypeName")] public string BusinessTypeName { get; set; }
        /// <summary>付款单位编号。</summary>
        [JsonPropertyName("PayOrgCode")] public string PayOrgCode { get; set; }
        /// <summary>付款单位名称。</summary>
        [JsonPropertyName("PayOrgName")] public string PayOrgName { get; set; }
        /// <summary>付款方银行账号。</summary>
        [JsonPropertyName("PayAccountNo")] public string PayAccountNo { get; set; }
        /// <summary>支付方式：1直联支付 2线下支付 3全额支付 4差额支付。</summary>
        [JsonPropertyName("PayChannel")] public string PayChannel { get; set; }
        /// <summary>付款金额。</summary>
        [JsonPropertyName("PayAmount")] public string PayAmount { get; set; }
        /// <summary>币种。</summary>
        [JsonPropertyName("PayCurrencyType")] public string PayCurrencyType { get; set; }
        /// <summary>币种名称。</summary>
        [JsonPropertyName("PayCurrencyName")] public string PayCurrencyName { get; set; }
        /// <summary>是否加急。</summary>
        [JsonPropertyName("WhetherUrgent")] public string WhetherUrgent { get; set; }
        /// <summary>备注。</summary>
        [JsonPropertyName("Remark")] public string Remark { get; set; }
        /// <summary>摘要。</summary>
        [JsonPropertyName("PayUse")] public string PayUse { get; set; }
        /// <summary>接收人 Code。</summary>
        [JsonPropertyName("CreateByCode")] public string CreateByCode { get; set; }
        /// <summary>接收人名称。</summary>
        [JsonPropertyName("CreateByName")] public string CreateByName { get; set; }
        /// <summary>对私用途。</summary>
        [JsonPropertyName("Purpose")] public string Purpose { get; set; }
        /// <summary>收款方信息集合。</summary>
        [JsonPropertyName("BatchPrivateRecList")] public List<BillBatchPrivateRequestBatchPrivateRec> BatchPrivateRecList { get; set; }

        public static BillBatchPrivateRequestParamSet Sample()
        {
            BillBatchPrivateRequestBatchPrivateRec rec = new BillBatchPrivateRequestBatchPrivateRec();
            rec.BillsPayOrderNo = "工资代发落地17671678171999";
            rec.PrivateAmount = "12";
            rec.ReceiveAccountNo = "65432612370001";
            rec.ReceiveAccountName = "赵刘伟";
            rec.ReceiveBranchCode = "104659011027";
            rec.ReceiveNature = "2";
            rec.ReceiveCurrencyCode = "CNY";
            rec.ReceiveCurrencyName = "人民币";

            BillBatchPrivateRequestParamSet item = new BillBatchPrivateRequestParamSet();
            item.BillsPayOrderNo = "工资代发落地176716781718888";
            item.PayType = "2";
            item.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd");
            item.BusinessTypeName = "外部业务类型";
            item.PayOrgName = "刘伟企业用户1";
            item.PayAccountNo = "0200000309024100566";
            item.PayChannel = "1";
            item.PayAmount = "25";
            item.PayCurrencyType = "CNY";
            item.PayCurrencyName = "人民币";
            item.WhetherUrgent = "1";
            item.Remark = "外部对私批量json";
            item.PayUse = "1";
            item.CreateByCode = "1";
            item.CreateByName = "1";
            item.Purpose = "工资";
            item.BatchPrivateRecList = new List<BillBatchPrivateRequestBatchPrivateRec> { rec };
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class BillBatchPrivateRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T1003。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="BillBatchPrivateRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<BillBatchPrivateRequestParamSet> ParamSet { get; set; }
    }
}
