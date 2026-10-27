using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0006 账户余额查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/account/qaccbal/V1
/// </summary>
public class AccountQaccbalRequestV1 : ApipRequest<AccountQaccbalResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/account/qaccbal/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T0006";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>AccountQaccbalRequestParamSet：请求参数。</summary>
    public class AccountQaccbalRequestParamSet
    {
        /// <summary>账户号。</summary>
        [JsonPropertyName("AcctNo")]
        public string AcctNo { get; set; }

        /// <summary>账户币种。</summary>
        [JsonPropertyName("CurrencyCode")]
        public string CurrencyCode { get; set; }

        /// <summary>余额开始日期，格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("StartDate")]
        public string StartDate { get; set; }

        /// <summary>余额结束日期，格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("EndDate")]
        public string EndDate { get; set; }

        public static AccountQaccbalRequestParamSet Sample()
        {
            AccountQaccbalRequestParamSet item = new AccountQaccbalRequestParamSet();
            item.AcctNo = "0200000309223101034";
            item.CurrencyCode = "CNY";
            item.StartDate = "2022-03-16";
            item.EndDate = "2023-03-16";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class AccountQaccbalRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T0006。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="AccountQaccbalRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<AccountQaccbalRequestParamSet> ParamSet { get; set; }
    }
}
