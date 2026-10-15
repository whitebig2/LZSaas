using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T1028 电子回单查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/bill/queryElectronicReceipt/V1
/// </summary>
public class BillQueryelectronicreceiptRequestV1 : ApipRequest<BillQueryelectronicreceiptResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/bill/queryElectronicReceipt/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T1028";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>BillQueryelectronicreceiptRequestParamSet：请求参数。</summary>
    public class BillQueryelectronicreceiptRequestParamSet
    {
        /// <summary>交易类型：A全部、C来账、D往账、N内部往来、P部分内部往来、W外部交易。</summary>
        [JsonPropertyName("TransType")]
        public string TransType { get; set; }

        /// <summary>查询类型：1通用型单笔 2通用型汇总。</summary>
        [JsonPropertyName("QueryType")]
        public string QueryType { get; set; }

        /// <summary>账户账号。</summary>
        [JsonPropertyName("AcctNo")]
        public string AcctNo { get; set; }

        /// <summary>币种。</summary>
        [JsonPropertyName("CurrencyCode")]
        public string CurrencyCode { get; set; }

        /// <summary>查询开始日期，格式 yyyyMMdd。</summary>
        [JsonPropertyName("BeginDate")]
        public string BeginDate { get; set; }

        /// <summary>查询截止日期，格式 yyyyMMdd。</summary>
        [JsonPropertyName("EndDate")]
        public string EndDate { get; set; }

        /// <summary>查询页码，大于 0 的正整数，第一次请求传 1。</summary>
        [JsonPropertyName("NextTag")]
        public string NextTag { get; set; }

        public static BillQueryelectronicreceiptRequestParamSet Sample()
        {
            BillQueryelectronicreceiptRequestParamSet item = new BillQueryelectronicreceiptRequestParamSet();
            item.TransType = "A";
            item.QueryType = "1";
            item.AcctNo = "0302099619100000000";
            item.CurrencyCode = "CNY";
            item.BeginDate = "20260916";
            item.EndDate = "20260920";
            item.NextTag = "1";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class BillQueryelectronicreceiptRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T1028。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="BillQueryelectronicreceiptRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<BillQueryelectronicreceiptRequestParamSet> ParamSet { get; set; }
    }
}
