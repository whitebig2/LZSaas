using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0004 应付单业务状态查询 —— 请求（对应 Java 版 QueryPaymentStatusRequestV1）。
///
/// 接口路径：POST /api/cse/tmc/query/paymentstatusquery/V1
/// 报文结构（biz_content 明文）：
/// <code>
/// {
///     "mesgNo": "T0004",
///     "paramSet": [
///         { "billsPayOrderNo": "2026011410020004" }
///     ]
/// }
/// </code>
/// </summary>
public class QueryPaymentStatusRequestV1 : ApipRequest<QueryPaymentStatusResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/query/paymentstatusquery/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T0004";

    /// <summary>本接口用 POST（与接口文档一致；少数查询接口是 GET，按文档覆写即可）。</summary>
    public override string Method => "POST";

    /// <summary>本接口 biz_content 需要 AES 内容加密。</summary>
    public override bool NeedEncrypt => true;

    /// <summary>QueryPaymentStatusRequestParamSet：请求参数。</summary>
    public class QueryPaymentStatusRequestParamSet
    {
        /// <summary>外部系统单据编号（str，最大 64，必输），如 2026011410020004。</summary>
        [JsonPropertyName("billsPayOrderNo")]
        public string BillsPayOrderNo { get; set; }

        public static QueryPaymentStatusRequestParamSet Sample()
        {
            QueryPaymentStatusRequestParamSet item = new QueryPaymentStatusRequestParamSet();
            //item.BillsPayOrderNo = "2026011410020004";
            item.BillsPayOrderNo = "c520a5f8-4028-4503-a6b4-ba550e6a7bbc";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + paramSet。</summary>
    public class QueryPaymentStatusRequestBiz
    {
        /// <summary>报文标识号（str，必输），本接口固定 T0004。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表（list，必输），元素见 <see cref="QueryPaymentStatusRequestParamSet"/>。</summary>
        [JsonPropertyName("paramSet")]
        public List<QueryPaymentStatusRequestParamSet> ParamSet { get; set; }
    }
}

