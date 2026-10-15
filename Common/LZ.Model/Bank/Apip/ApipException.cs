namespace LZ.Model.Bank.Apip;

/// <summary>
/// APIP 调用异常（对应 Java SDK 的 IcbcApiException）。
/// 调用失败统一抛此异常，由业务代码按自己的异常处理策略捕获。
/// </summary>
public class ApipException : Exception
{
    /// <summary>用错误信息构造异常（错误文案与 ICBC Java SDK 保持一致，便于与行方 / Java 侧对照排查）。</summary>
    public ApipException(string message) : base(message) { }

    /// <summary>用错误信息 + 内部异常构造（内部异常保留原始调用栈，便于定位是配置、网络还是加解密问题）。</summary>
    public ApipException(string message, Exception inner) : base(message, inner) { }
}
