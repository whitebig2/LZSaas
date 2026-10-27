using System.Text.Json.Serialization;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// APIP 响应基类（对应 Java SDK 的 IcbcResponse）。
/// 公共返回码的 key：网关在**业务报文**里用驼峰（returnCode / returnMsg / msgId，如 "0000"）；
/// 而网关的**校验类错误信封**（如签名验证失败）用的是下划线（return_code / return_msg）。
/// 两种都做了映射，调用方只需看 <see cref="ReturnCode"/> / <see cref="ReturnMsg"/>。
/// 业务数据由各接口的响应类自行定义（如 T0004 的 ResultSet）。
/// </summary>
public class ApipResponse
{
    /// <summary>返回码，0 = 成功（网关以数字字符串上送，如 "0000"，反序列化时按数值读取）。</summary>
    [JsonPropertyName("returnCode")]
    public int ReturnCode { get; set; } = -1;

    /// <summary>返回码说明。</summary>
    [JsonPropertyName("returnMsg")]
    public string ReturnMsg { get; set; } = "unexpected error.";

    /// <summary>消息通讯唯一编号。</summary>
    [JsonPropertyName("msgId")]
    public string MsgId { get; set; }

    // ---- 以下三项只用于兼容“网关错误信封”的下划线命名，因此只有 setter（不参与序列化）

    /// <summary>兼容网关错误信封的 <c>return_code</c>（如 400017 = 签名验证失败）。</summary>
    [JsonPropertyName("return_code")]
    public int ReturnCodeLegacyAlias { set { ReturnCode = value; } }

    /// <summary>兼容网关错误信封的 <c>return_msg</c>（如 "sign verify failed."）。</summary>
    [JsonPropertyName("return_msg")]
    public string ReturnMsgLegacyAlias { set { if (!string.IsNullOrEmpty(value)) { ReturnMsg = value; } } }

    /// <summary>兼容网关错误信封的 <c>msg_id</c>。</summary>
    [JsonPropertyName("msg_id")]
    public string MsgIdLegacyAlias { set { if (!string.IsNullOrEmpty(value)) { MsgId = value; } } }

    /// <summary>技术上是否成功（returnCode == 0）。业务是否成功还需按接口约定进一步判断（如 payStatus）。</summary>
    public bool IsSuccess => ReturnCode == 0;
}
