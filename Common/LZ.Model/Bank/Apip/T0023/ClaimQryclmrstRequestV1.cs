using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0023 认领结果查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/claim/qryclmrst/V1
/// </summary>
public class ClaimQryclmrstRequestV1 : ApipRequest<ClaimQryclmrstResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/claim/qryclmrst/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T0023";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>ClaimQryclmrstRequestParamSet：请求参数。</summary>
    public class ClaimQryclmrstRequestParamSet
    {
        /// <summary>应收单外部系统编号。</summary>
        [JsonPropertyName("ExtBusinessNo")]
        public string ExtBusinessNo { get; set; }

        /// <summary>应收单财资云编号。</summary>
        [JsonPropertyName("ReceivableBusinessNo")]
        public string ReceivableBusinessNo { get; set; }

        /// <summary>开始日期，格式 YYYY-MM-DD。</summary>
        [JsonPropertyName("BeginDate")]
        public string BeginDate { get; set; }

        /// <summary>结束日期，格式 YYYY-MM-DD。</summary>
        [JsonPropertyName("EndDate")]
        public string EndDate { get; set; }

        /// <summary>查询页码，大于 0 的正整数，第一次请求传 1。</summary>
        [JsonPropertyName("NextTag")]
        public string NextTag { get; set; }

        public static ClaimQryclmrstRequestParamSet Sample()
        {
            ClaimQryclmrstRequestParamSet item = new ClaimQryclmrstRequestParamSet();
            item.BeginDate = "2025-12-01";
            item.EndDate = "2025-12-31";
            item.NextTag = "1";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class ClaimQryclmrstRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T0023。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="ClaimQryclmrstRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<ClaimQryclmrstRequestParamSet> ParamSet { get; set; }
    }
}
