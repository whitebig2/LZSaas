using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0001 交易流水信息查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/account/qhisd/V1
/// </summary>
public class AccountQhisdRequestV1 : ApipRequest<AccountQhisdResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/account/qhisd/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T0001";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>AccountQhisdRequestParamSet：请求参数。</summary>
    public class AccountQhisdRequestParamSet
    {
        /// <summary>账户号（str，最大 128，必输）。</summary>
        [JsonPropertyName("acctNo")]
        public string AcctNo { get; set; }

        /// <summary>账户币种（str，最大 3，必输），如 CNY。</summary>
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }

        /// <summary>交易开始日期（str，最大 10，必输），格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("startTransDate")]
        public string StartTransDate { get; set; }

        /// <summary>交易结束日期（str，最大 10，必输），格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("endTransDate")]
        public string EndTransDate { get; set; }

        /// <summary>查询页码，大于 0 的正整数，第一次请求传 1。</summary>
        [JsonPropertyName("NextTag")]
        public string NextTag { get; set; }

        public static AccountQhisdRequestParamSet Sample()
        {
            AccountQhisdRequestParamSet item = new AccountQhisdRequestParamSet();
            item.AcctNo = "0302099619100000000";
            item.CurrencyCode = "CNY";
            item.StartTransDate = "2026-09-16";
            item.EndTransDate = "2026-09-20";
            item.NextTag = "1";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + paramSet。</summary>
    public class AccountQhisdRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T0001。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="AccountQhisdRequestParamSet"/>。</summary>
        [JsonPropertyName("paramSet")]
        public List<AccountQhisdRequestParamSet> ParamSet { get; set; }
    }
}
