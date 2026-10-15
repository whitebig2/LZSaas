namespace LZ.Model.Bank.Apip;

/// <summary>
/// APIP 请求基类（对应 Java SDK 的 AbstractIcbcRequest）。
/// 子类声明 HTTP 方法与是否需要内容加密；业务报文（BizContent）为普通 POCO，
/// 字段名通过 [JsonPropertyName] 与接口文档保持一致。
///
/// <para>新增一个接口时只需三步：① 写请求类继承本类（覆写 <see cref="Method"/>、<see cref="NeedEncrypt"/>）；
/// ② 写业务报文 POCO（字段名 = 接口文档的 JSON key）；③ 写响应类继承 <see cref="ApipResponse"/>。
/// 完整模板与示例见 docs/如何新增一个接口.md。</para>
/// </summary>
public abstract class ApipRequest<TResponse> where TResponse : ApipResponse
{
    /// <summary>完整请求 URL = 网关地址(prefix) + 接口路径。</summary>
    public string ServiceUrl { get; set; }

    /// <summary>业务报文（biz_content 明文对象），序列化为 JSON 后按需加密上送。</summary>
    public object BizContent { get; set; }

    /// <summary>
    /// 应用授权令牌（app_auth_token，可选）。ISV/服务商代商户调用时需要；
    /// 与工行 SDK 一样，为空时该参数不上送（不影响签名串）。
    /// </summary>
    public string AppAuthToken { get; set; }

    /// <summary>HTTP 方法，仅支持 GET / POST（本示例接口为 POST）。</summary>
    public abstract string Method { get; }

    /// <summary>是否需要内容加密（AES）。默认 false。</summary>
    public virtual bool NeedEncrypt => false;
}
