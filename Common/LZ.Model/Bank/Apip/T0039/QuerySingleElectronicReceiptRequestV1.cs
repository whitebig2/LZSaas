using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0039 单笔回单查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/query/singleElectronicReceipt/V1
/// </summary>
public class QuerySingleElectronicReceiptRequestV1 : ApipRequest<QuerySingleElectronicReceiptResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/query/singleElectronicReceipt/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T0039";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>QuerySingleElectronicReceiptRequestParamSet：请求参数。</summary>
    public class QuerySingleElectronicReceiptRequestParamSet
    {
        /// <summary>账号。</summary>
        [JsonPropertyName("AcctNo")]
        public string AcctNo { get; set; }

        /// <summary>交易流水号。</summary>
        [JsonPropertyName("transInfoNo")]
        public string TransInfoNo { get; set; }

        /// <summary>客户外部系统名称（账户所属公司名称）。</summary>
        [JsonPropertyName("OrgNameOut")]
        public string OrgNameOut { get; set; }

        /// <summary>客户外部系统用户编码（账号对应的用户信息）。</summary>
        [JsonPropertyName("UserCodeOut")]
        public string UserCodeOut { get; set; }

        public static QuerySingleElectronicReceiptRequestParamSet Sample()
        {
            QuerySingleElectronicReceiptRequestParamSet item = new QuerySingleElectronicReceiptRequestParamSet();
            item.AcctNo = "0200000309024100566";
            item.TransInfoNo = "Tfc6eb7c503cc542d2047aee00ee27df5";
            item.OrgNameOut = "刘伟企业用户1";
            item.UserCodeOut = "zhaoliuwei";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class QuerySingleElectronicReceiptRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T0039。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="QuerySingleElectronicReceiptRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<QuerySingleElectronicReceiptRequestParamSet> ParamSet { get; set; }
    }
}
