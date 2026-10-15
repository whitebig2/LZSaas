using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// APIP 网关客户端（对应 ICBC Java SDK 的 DefaultIcbcClient）。
///
/// <para>完整调用流程（业务代码只需调用 <see cref="ExecuteAsync{TResponse}"/>）：</para>
/// <code>
/// 1) 业务报文序列化为 JSON（biz_content 明文）
/// 2) 组装公共参数：app_id / sign_type / charset / format / msg_id / timestamp
///    （+ 内容加密时的 encrypt_type，+ app_auth_token 若设置），空值参数不上送
/// 3) biz_content 按配置做 AES 内容加密
/// 4) 签名串 = URL 路径 + "?" + 按 key 字典序排序的 k=v 对（原始值，不做 URL 编码），
///    RSA2(SHA256WithRSA) 私钥签名后放入 sign 参数
/// 5) POST application/x-www-form-urlencoded（请求头 APIGW-VERSION）
/// 6) 响应解析：取 response_biz_content 的原始文本（含引号，加密时是 Base64 字符串、
///    不加密时是嵌套 JSON 对象）用网关公钥验签（RSA/SHA1WithRSA，与 SDK 一致），
///    通过后 AES 解密 → 反序列化为响应类
/// </code>
/// </summary>
public sealed class ApipClient
{
    /// <summary>与 Java SDK 一致的网关版本请求头值。</summary>
    private const string ApiGwVersion = "v2_20190522";

    private const string Charset = "UTF-8";
    private const string Format = "json";

    /// <summary>响应验签默认使用 SHA1WithRSA（"RSA"）——这是 ICBC 网关对 RSA2 请求的实际行为。</summary>
    private const string DefaultResponseSignType = "RSA";

    /// <summary>
    /// 共享的 HttpClient。HttpClient 设计为可复用且线程安全，每次调用都新建会耗尽 socket
    /// （TIME_WAIT 堆积）；这里改为进程内单例。
    /// </summary>
    private static readonly HttpClient SharedHttp = new()
    {
        Timeout = TimeSpan.FromMinutes(2),
        // 网关为内网/专线环境，若证书链不受本机信任可在此放宽；默认严格校验
    };

    private readonly string _appId;
    private readonly string _signType;
    private readonly string _privateKey;
    private readonly string _gatewayPublicKey;
    private readonly string _encryptType;
    private readonly string _encryptKey;
    private readonly string _responseSignType;

    /// <summary>
    /// 调试/排障用的跟踪回调（可选）。设置后，客户端会在关键步骤回调一行文本：
    /// 签名串（参与签名的原始文本）、sign、请求行、响应原文等。
    /// <para>这些内容**不含私钥/AES 密钥**，可以安全地贴给行方或我方排查（仍建议按贵方日志规范脱敏）。</para>
    /// </summary>
    public Action<string> Trace { get; set; }

    private void TraceLine(string message)
    {
        Action<string> t = Trace;
        if (t != null)
        {
            try { t(message); } catch { /* 跟踪回调本身不应影响调用 */ }
        }
    }

    /// <summary>
    /// 构造客户端。参数含义与 Java 版配置一致（同一应用在同一网关下共用一组参数）：
    /// appId 应用编号；signType 签名方式（本示例固定 RSA2）；privateKey 应用 RSA 私钥（Base64 PKCS#8）；
    /// gatewayPublicKey 网关公钥（Base64 X.509 SPKI，响应验签用）；
    /// encryptType 内容加密方式（AES，可为空表示不加密）；encryptKey AES 密钥（Base64）。
    /// </summary>
    public ApipClient(
        string appId,
        string signType,
        string privateKey,
        string gatewayPublicKey,
        string encryptType,
        string encryptKey)
        : this(appId, signType, privateKey, gatewayPublicKey, encryptType, encryptKey, null)
    {
    }

    /// <summary>
    /// 构造客户端（可指定响应验签算法）。
    /// responseSignType 传 null/空时使用 <see cref="DefaultResponseSignType"/>（"RSA" = SHA1WithRSA），
    /// 与 ICBC SDK / Java SDK 的固定行为一致；个别网关若改用 SHA256WithRSA 签响应，可传 "RSA2"。
    /// </summary>
    public ApipClient(
        string appId,
        string signType,
        string privateKey,
        string gatewayPublicKey,
        string encryptType,
        string encryptKey,
        string responseSignType)
    {
        _appId = appId;
        _signType = signType;
        _privateKey = privateKey;
        _gatewayPublicKey = gatewayPublicKey;
        _encryptType = string.IsNullOrEmpty(encryptType) ? null : encryptType;
        _encryptKey = string.IsNullOrEmpty(encryptKey) ? null : encryptKey;
        _responseSignType = string.IsNullOrEmpty(responseSignType) ? DefaultResponseSignType : responseSignType;
    }

    /// <summary>执行一次 APIP 调用：组装 → 加密 → 签名 → POST → 验签 → 解密 → 反序列化。</summary>
    public async Task<TResponse> ExecuteAsync<TResponse>(ApipRequest<TResponse> request)
        where TResponse : ApipResponse
    {
        // 1) 业务报文 JSON
        string bizContentStr = request.BizContent == null
            ? null
            : JsonSerializer.Serialize(request.BizContent, BizJsonOptions);

        // 2) 公共参数（Ordinal 排序，等价 Java TreeMap；空值不上送，等价 IcbcHashMap.put 的过滤）
        var parameters = new SortedDictionary<string, string>(StringComparer.Ordinal);
        PutIfNotEmpty(parameters, "app_id", _appId);
        PutIfNotEmpty(parameters, "sign_type", _signType);
        PutIfNotEmpty(parameters, "charset", Charset);
        PutIfNotEmpty(parameters, "format", Format);
        PutIfNotEmpty(parameters, "msg_id", Guid.NewGuid().ToString("N"));
        PutIfNotEmpty(parameters, "timestamp",
            DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-dd HH:mm:ss"));
        PutIfNotEmpty(parameters, "app_auth_token", request.AppAuthToken);

        // 3) biz_content（按需 AES 内容加密）
        if (request.NeedEncrypt)
        {
            if (_encryptType == null || _encryptKey == null)
            {
                throw new ApipException("request need be encrypted, encrypt type and encrypt key can not be null.");
            }
            PutIfNotEmpty(parameters, "encrypt_type", _encryptType);
            PutIfNotEmpty(parameters, "biz_content",
                bizContentStr != null ? ApipCrypto.AesEncrypt(bizContentStr, _encryptKey) : null);
        }
        else
        {
            PutIfNotEmpty(parameters, "biz_content", bizContentStr);
        }

        // 4) 签名
        if (_signType != "RSA2" && _signType != "RSA")
        {
            throw new ApipException("not support signType: " + _signType);
        }
        string strToSign = BuildOrderedSignStr(new Uri(request.ServiceUrl).AbsolutePath, parameters);
        TraceLine("参与签名的原始文本(strToSign): " + strToSign);
        parameters["sign"] = ApipCrypto.RsaSign(strToSign, _signType, _privateKey);
        TraceLine("签名(sign): " + parameters["sign"]);

        // 5) HTTP 调用（仅支持 GET / POST，与 Java SDK 一致）
        TraceLine("请求: " + request.Method + " " + request.ServiceUrl + "  头: APIGW-VERSION=" + ApiGwVersion
                  + "; Content-Type=" + (request.Method == "POST" ? "application/x-www-form-urlencoded;charset=UTF-8" : "(无)"));
        string respStr;
        if (request.Method == "GET")
        {
            respStr = await DoGetAsync(request.ServiceUrl, parameters);
        }
        else if (request.Method == "POST")
        {
            respStr = await DoPostAsync(request.ServiceUrl, parameters);
        }
        else
        {
            throw new ApipException("only support GET or POST, method: " + request.Method);
        }

        // 6) 响应解析（验签 + 解密 + 反序列化）
        TraceLine("响应原文: " + respStr);
        return ParseResponse<TResponse>(request, respStr);
    }

    // ------------------------------------------------------------------ 响应解析

    private TResponse ParseResponse<TResponse>(ApipRequest<TResponse> request, string respStr)
        where TResponse : ApipResponse
    {
        if (!TryExtractResponseFields(respStr, out string rawBizContent, out string bizContentPayload,
                                      out bool payloadIsString, out string sign))
        {
            throw new ApipException("response is not format json. respStr :\n" + respStr);
        }

        // 验签：对 RSA2 请求，网关的响应是按 "RSA"（SHA1WithRSA）签的，此处默认保持一致
        bool passed = ApipCrypto.RsaVerify(rawBizContent, _responseSignType, _gatewayPublicKey, sign);
        if (!passed)
        {
            throw new ApipException("icbc sign verify not passed.");
        }

        // 内容解密：
        //  - 正常业务响应：response_biz_content 是 Base64 密文 → AES 解密；
        //  - 网关的校验类错误信封（如 return_code=400017 签名验证失败）：response_biz_content 是**明文 JSON**
        //    （既可能是嵌套对象，也可能是 JSON 字符串），此时不能当密文解，直接按业务报文解析，
        //    这样调用方才能看到网关真正返回的 returnCode/returnMsg（与 Java SDK 的行为一致）。
        string bizContentStr;
        if (!request.NeedEncrypt)
        {
            bizContentStr = bizContentPayload;
        }
        else if (payloadIsString && LooksLikeBase64(bizContentPayload))
        {
            bizContentStr = ApipCrypto.AesDecrypt(bizContentPayload, _encryptKey);
        }
        else
        {
            bizContentStr = bizContentPayload;
            TraceLine("响应未按 AES 加密（网关这类报文通常是校验类错误信封），按明文业务报文解析");
        }

        try
        {
            return JsonSerializer.Deserialize<TResponse>(bizContentStr, ResponseJsonOptions);
        }
        catch (Exception e)
        {
            throw new ApipException("response can not transform to defined class. response: " + respStr, e);
        }
    }

    /// <summary>粗判是否像 Base64 密文（用于区分“加密的业务响应”和“网关的明文错误信封”，不抛异常）。</summary>
    private static bool LooksLikeBase64(string s)
    {
        if (string.IsNullOrEmpty(s) || s.Length % 4 != 0)
        {
            return false;
        }
        foreach (char c in s)
        {
            bool ok = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')
                      || c == '+' || c == '/' || c == '=';
            if (!ok)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 从响应 JSON 中取出验签所需的信息：<c>response_biz_content</c> 的原始文本（含引号，
    /// 与网关实际签名的字节完全一致）、其内容、该内容是否为 JSON 字符串（加密时才为字符串）、以及 <c>sign</c>。
    ///
    /// <para>按字符串位置切分（IndexOf/LastIndexOf）会隐含“紧凑 JSON + sign 是最后一个字段”两个假设；
    /// 这里改用 JSON 解析取原始片段，对响应格式（空格、字段顺序）不敏感，且仍保证验签输入与网关签名的一致。
    /// 不加密的响应中 <c>response_biz_content</c> 是嵌套 JSON 对象，加密时是 JSON 字符串，
    /// 网关的校验类错误信封则通常是明文对象，三种情况都已兼容。</para>
    /// </summary>
    private static bool TryExtractResponseFields(
        string respStr,
        out string rawBizContent,
        out string bizContentPayload,
        out bool payloadIsString,
        out string sign)
    {
        rawBizContent = null;
        bizContentPayload = null;
        payloadIsString = false;
        sign = null;
        try
        {
            using var doc = JsonDocument.Parse(respStr);
            JsonElement root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }
            if (!root.TryGetProperty("response_biz_content", out JsonElement bizEl))
            {
                return false;
            }
            if (!root.TryGetProperty("sign", out JsonElement signEl) || signEl.ValueKind != JsonValueKind.String)
            {
                return false;
            }
            // GetRawText() 返回该元素在原文中的原始文本：字符串值带引号，对象值即对象原文，
            // 与网关签名时使用的字节一致。
            rawBizContent = bizEl.GetRawText();
            payloadIsString = bizEl.ValueKind == JsonValueKind.String;
            bizContentPayload = payloadIsString ? bizEl.GetString() : rawBizContent;
            sign = signEl.GetString();
            return rawBizContent != null && sign != null;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    // ------------------------------------------------------------------ 公共参数 / 签名串

    private static void PutIfNotEmpty(SortedDictionary<string, string> map, string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            map[key] = value;
        }
    }

    /// <summary>
    /// 构建签名串：URL 路径 + "?" + 按 key 字典序排序的 k=v 对（&amp; 连接，原始值不做 URL 编码，空值跳过）。
    /// </summary>
    private static string BuildOrderedSignStr(string path, SortedDictionary<string, string> parameters)
    {
        var sb = new StringBuilder(path).Append('?');
        bool hasParam = false;
        foreach (var kv in parameters)
        {
            if (string.IsNullOrEmpty(kv.Value))
            {
                continue;
            }
            if (hasParam)
            {
                sb.Append('&');
            }
            hasParam = true;
            sb.Append(kv.Key).Append('=').Append(kv.Value);
        }
        return sb.ToString();
    }

    // ------------------------------------------------------------------ HTTP

    private static async Task<string> DoPostAsync(string url, SortedDictionary<string, string> parameters)
    {
        var sb = new StringBuilder();
        bool first = true;
        foreach (var kv in parameters)
        {
            if (!first)
            {
                sb.Append('&');
            }
            first = false;
            sb.Append(kv.Key).Append('=').Append(FormUrlEncode(kv.Value));
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(sb.ToString()));
        request.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "UTF-8" };
        request.Headers.Add("APIGW-VERSION", ApiGwVersion);

        return await SendAsync(request).ConfigureAwait(false);
    }

    private static async Task<string> DoGetAsync(string url, SortedDictionary<string, string> parameters)
    {
        var sb = new StringBuilder(url);
        if (parameters.Count > 0)
        {
            sb.Append(url.Contains('?') ? (url.EndsWith('&') ? "" : "&") : "?");
            bool hasParam = false;
            foreach (var kv in parameters)
            {
                if (string.IsNullOrEmpty(kv.Value))
                {
                    continue;
                }
                if (hasParam)
                {
                    sb.Append('&');
                }
                hasParam = true;
                sb.Append(kv.Key).Append('=').Append(FormUrlEncode(kv.Value));
            }
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, sb.ToString());
        request.Headers.Add("APIGW-VERSION", ApiGwVersion);

        return await SendAsync(request).ConfigureAwait(false);
    }

    private static async Task<string> SendAsync(HttpRequestMessage request)
    {
        string respStr;
        int statusCode;
        try
        {
            using HttpResponseMessage resp = await SharedHttp.SendAsync(request).ConfigureAwait(false);
            statusCode = (int)resp.StatusCode;
            respStr = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (HttpRequestException e)
        {
            throw new ApipException("execute request exception. " + e.Message, e);
        }
        catch (Exception e)
        {
            throw new ApipException("io exception.", e);
        }

        if (statusCode != 200)
        {
            throw new ApipException(
                "response status code is not valid. status code: " + statusCode + " resp is:" + respStr);
        }
        return respStr;
    }

    /// <summary>
    /// 表单 URL 编码（与 Java URLEncoder 语义一致：未保留字符 A-Z a-z 0-9 - _ . *，
    /// 空格编码为 +，其余按 UTF-8 字节逐字节 %XX）。
    /// </summary>
    internal static string FormUrlEncode(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value ?? string.Empty;
        }
        var sb = new StringBuilder(value.Length);
        foreach (byte b in Encoding.UTF8.GetBytes(value))
        {
            char c = (char)b;
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')
                || c == '-' || c == '_' || c == '.' || c == '*')
            {
                sb.Append(c);
            }
            else if (c == ' ')
            {
                sb.Append('+');
            }
            else
            {
                sb.Append('%').Append(((int)b).ToString("X2"));
            }
        }
        return sb.ToString();
    }

    // ------------------------------------------------------------------ JSON 选项

    /// <summary>biz_content 序列化：字段名以 [JsonPropertyName] 为准，null 字段不上送。</summary>
    internal static readonly JsonSerializerOptions BizJsonOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// 响应反序列化：字段名大小写不敏感（网关返回 payStatus/PayStatus 均可映射）；
    /// 数字字段允许从字符串读取（网关 returnCode 以 "0000" 这类数字字符串上送，与 Java SDK 宽松转换行为一致）。
    /// </summary>
    internal static readonly JsonSerializerOptions ResponseJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
    };
}
