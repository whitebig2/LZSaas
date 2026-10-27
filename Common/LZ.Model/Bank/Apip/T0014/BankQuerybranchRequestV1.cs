using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0014 金融网点查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/bank/querybranch/V1
/// </summary>
public class BankQuerybranchRequestV1 : ApipRequest<BankQuerybranchResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/bank/querybranch/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T0014";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>BankQuerybranchRequestParamSet：请求参数。</summary>
    public class BankQuerybranchRequestParamSet
    {
        /// <summary>直连编号。</summary>
        [JsonPropertyName("DirectCode")]
        public string DirectCode { get; set; }

        /// <summary>联行号，与 BelongBankTypeName 选一必填。</summary>
        [JsonPropertyName("CnapsCode")]
        public string CnapsCode { get; set; }

        /// <summary>大行名称，与 CnapsCode 选一必填。</summary>
        [JsonPropertyName("BelongBankTypeName")]
        public string BelongBankTypeName { get; set; }

        /// <summary>查询页码，大于 0 的正整数，第一次请求传 1。</summary>
        [JsonPropertyName("NextTag")]
        public string NextTag { get; set; }

        public static BankQuerybranchRequestParamSet Sample()
        {
            BankQuerybranchRequestParamSet item = new BankQuerybranchRequestParamSet();
            item.DirectCode = "B0001H111000001";
            item.BelongBankTypeName = "中国工商银行";
            item.NextTag = "1";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class BankQuerybranchRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T0014。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="BankQuerybranchRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<BankQuerybranchRequestParamSet> ParamSet { get; set; }
    }
}
