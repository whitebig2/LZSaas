using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T1019 余额对账信息查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/query/bankreconciliation/V1
/// </summary>
public class QueryBankreconciliationRequestV1 : ApipRequest<QueryBankreconciliationResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/query/bankreconciliation/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T1019";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>QueryBankreconciliationRequestParamSet：请求参数。</summary>
    public class QueryBankreconciliationRequestParamSet
    {
        /// <summary>银行账号。</summary>
        [JsonPropertyName("AccNo")]
        public string AccNo { get; set; }

        /// <summary>对账开始日期，格式 yyyyMMdd。</summary>
        [JsonPropertyName("BeginDate")]
        public string BeginDate { get; set; }

        /// <summary>对账结束日期，格式 yyyyMMdd。</summary>
        [JsonPropertyName("EndDate")]
        public string EndDate { get; set; }

        public static QueryBankreconciliationRequestParamSet Sample()
        {
            QueryBankreconciliationRequestParamSet item = new QueryBankreconciliationRequestParamSet();
            item.AccNo = "0200000309223101034";
            item.BeginDate = "20250101";
            item.EndDate = "20260601";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class QueryBankreconciliationRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T1019。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="QueryBankreconciliationRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<QueryBankreconciliationRequestParamSet> ParamSet { get; set; }
    }
}
