using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// T0005 账户信息查询 —— 请求。
/// 接口路径：POST /api/cse/tmc/account/qacctinf/V1
/// </summary>
public class AccountQacctinfRequestV1 : ApipRequest<AccountQacctinfResponseV1>
{
    /// <summary>接口路径</summary>
    public const string ServicePath = "/api/cse/tmc/account/qacctinf/V1";
    /// <summary>报文标识号</summary>
    public const string MesgNoValue = "T0005";

    public override string Method => "POST";
    public override bool NeedEncrypt => true;

    /// <summary>AccountQacctinfRequestParamSet：请求参数。</summary>
    public class AccountQacctinfRequestParamSet
    {
        /// <summary>币种编号数组。</summary>
        [JsonPropertyName("CurrencyCodeList")]
        public List<string> CurrencyCodeList { get; set; }

        /// <summary>账户类别。</summary>
        [JsonPropertyName("CategoryCodeList")]
        public List<string> CategoryCodeList { get; set; }

        /// <summary>账户编号数组。</summary>
        [JsonPropertyName("AcctNoList")]
        public List<string> AcctNoList { get; set; }

        /// <summary>账户名称。</summary>
        [JsonPropertyName("AcctName")]
        public string AcctName { get; set; }

        /// <summary>账户模块使用的账户类型编号列表。</summary>
        [JsonPropertyName("CooperativeInstitutionTypeList")]
        public List<string> CooperativeInstitutionTypeList { get; set; }

        /// <summary>合作金融机构编码。</summary>
        [JsonPropertyName("CooperativeInstitutionCode")]
        public string CooperativeInstitutionCode { get; set; }

        /// <summary>合作金融机构名称。</summary>
        [JsonPropertyName("CooperativeInstitutionName")]
        public string CooperativeInstitutionName { get; set; }

        /// <summary>开户区域（基础模块中的国家项）。</summary>
        [JsonPropertyName("OpenAreaCode")]
        public string OpenAreaCode { get; set; }

        /// <summary>账户开户开始日期，格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("AcctOpenDateStart")]
        public string AcctOpenDateStart { get; set; }

        /// <summary>账户开户结束日期，格式 yyyy-MM-dd。</summary>
        [JsonPropertyName("AcctOpenDateEnd")]
        public string AcctOpenDateEnd { get; set; }

        /// <summary>账户状态：1正常；2变更中；3销户中；4已销户。</summary>
        [JsonPropertyName("AcctStatus")]
        public string AcctStatus { get; set; }

        /// <summary>账户收支属性。</summary>
        [JsonPropertyName("PayReceiveAttrList")]
        public List<string> PayReceiveAttrList { get; set; }

        /// <summary>账户性质：1基本存款账户；2一般存款账户；3专用存款账户；4临时存款账户。</summary>
        [JsonPropertyName("AcctNature")]
        public string AcctNature { get; set; }

        /// <summary>账户用途编码。</summary>
        [JsonPropertyName("AcctUseCodes")]
        public List<string> AcctUseCodes { get; set; }

        /// <summary>查询页码，大于 0 的正整数，第一次请求传 1。</summary>
        [JsonPropertyName("NextTag")]
        public string NextTag { get; set; }

        public static AccountQacctinfRequestParamSet Sample()
        {
            AccountQacctinfRequestParamSet item = new AccountQacctinfRequestParamSet();
            item.AcctNoList = new List<string> { "0302099619100000000" };
            item.NextTag = "1";
            return item;
        }
    }

    /// <summary>业务报文（biz_content）：mesgNo + ParamSet。</summary>
    public class AccountQacctinfRequestBiz
    {
        /// <summary>报文标识号，本接口固定 T0005。</summary>
        [JsonPropertyName("mesgNo")]
        public string MesgNo { get; set; }

        /// <summary>请求参数列表，元素见 <see cref="AccountQacctinfRequestParamSet"/>。</summary>
        [JsonPropertyName("ParamSet")]
        public List<AccountQacctinfRequestParamSet> ParamSet { get; set; }
    }
}
