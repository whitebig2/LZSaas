namespace LZ.Model.Options
{
    /// <summary>
    /// 百望发票旗舰版开放平台配置
    /// </summary>
    public class BaiWangOption
    {
        /// <summary>
        /// 开放平台 AppKey
        /// </summary>
        public string AppKey { get; set; } = string.Empty;

        /// <summary>
        /// 开放平台 AppSecret
        /// </summary>
        public string AppSecret { get; set; } = string.Empty;

        /// <summary>
        ///数电账号
        /// </summary>
        public string InvoiceTerminalCode { get; set; } = string.Empty;

        /// <summary>
        /// 百望云账号用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 百望云账号密码
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 密码加密盐值，加密规则：SHA256(明文密码 + 盐值)
        /// </summary>
        public string Salt { get; set; } = "";

        /// <summary>
        /// 接口地址，沙箱：https://sandbox-openapi.baiwang.com，正式：https://openapi.baiwang.com
        /// </summary>
        public string BaseUrl { get; set; } = "https://openapi.baiwang.com";

        /// <summary>
        /// 回调地址
        /// </summary>
        public string HostUrl { get; set; } = string.Empty;

        /// <summary>
        /// 接口版本号
        /// </summary>
        public string Version { get; set; } = "7.0";

        /// <summary>
        /// AccessToken 缓存 Key
        /// </summary>
        public string CacheKey { get; set; } = "baiwangAccessToken";

        /// <summary>
        /// 创建开票申请接口方法名
        /// </summary>
        public string AddPreInvoiceMethod { get; set; } = "baiwang.output.preinvoice.add";

        /// <summary>
        /// 查询开票申请接口方法名
        /// </summary>
        public string QueryPreInvoiceMethod { get; set; } = "baiwang.output.preinvoice.query";



        /// <summary>
        /// 查询发票接口方法名
        /// </summary>
        public string QueryInvoiceMethod { get; set; } = "baiwang.s.outputinvoice.queryPage";

        /// <summary>
        /// 开具发票接口方法名
        /// </summary>
        public string IssueInvoiceMethod { get; set; } = "baiwang.s.outputinvoice.invoice";

        /// <summary>
        /// 快捷冲红接口方法名
        /// </summary>
        public string IssueFastRedMethod { get; set; } = "baiwang.s.outputinvoice.fastRed";

        /// <summary>
        /// 红票确认单
        /// </summary>
        public string IssueRedConfirmMethod { get; set; } = "baiwang.s.redconfirm.query";
    }
}
