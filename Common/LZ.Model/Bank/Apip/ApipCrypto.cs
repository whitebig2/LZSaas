using System.Security.Cryptography;
using System.Text;

namespace LZ.Model.Bank.Apip;

/// <summary>
/// APIP 加解密/签名工具（C# 原生实现，行为与 ICBC Java SDK 保持一致）：
/// <list type="bullet">
///   <item>内容加密：AES/CBC/PKCS5Padding，IV 固定为 16 个 0 字节，
///         密钥为 Base64 解码后的字节（长度由网关分配，常见 16/24/32 字节），密文输出 Base64。</item>
///   <item>请求签名：RSA2 = SHA256WithRSA，RSA = SHA1WithRSA（PKCS#1 v1.5），私钥为 Base64 的 PKCS#8，签名输出 Base64。</item>
///   <item>响应验签：公钥为 Base64 的 X.509 SubjectPublicKeyInfo；
///         注意网关对 RSA2 请求的响应使用 SHA1WithRSA 验签，此处保持一致。</item>
/// </list>
///
/// <para>本类是纯静态、无状态、线程安全（每次调用各自创建 RSA/AES 实例），可直接整体拷贝到业务工程使用。</para>
/// </summary>
public static class ApipCrypto
{
    private static readonly byte[] ZeroIv = new byte[16];

    // ------------------------------------------------------------ 密钥文本规范化

    /// <summary>
    /// 规范化密钥/密钥文本：去掉 PEM 头尾行（<c>-----BEGIN/END ...-----</c>）与所有空白字符。
    /// 工行下发的密钥有时带换行或 PEM 包装，Java SDK 的密钥读取也会去掉头尾与换行；此处保持同样的宽松度。
    /// </summary>
    internal static string NormalizeKeyText(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }
        var sb = new StringBuilder(key.Length);
        foreach (string rawLine in key.Split('\n'))
        {
            string line = rawLine.Trim();
            if (line.StartsWith("-----", StringComparison.Ordinal))
            {
                continue;   // PEM armor: -----BEGIN PRIVATE KEY----- / -----END PUBLIC KEY-----
            }
            sb.Append(line);
        }
        var compact = new StringBuilder(sb.Length);
        foreach (char c in sb.ToString())
        {
            if (!char.IsWhiteSpace(c))
            {
                compact.Append(c);
            }
        }
        return compact.ToString().Trim('"', '\'');
    }

    /// <summary>导入 RSA 私钥（Base64）：优先 PKCS#8，失败则按 PKCS#1 再试一次。</summary>
    private static RSA ImportPrivateKey(string privateKeyBase64)
    {
        byte[] key;
        try
        {
            key = Convert.FromBase64String(NormalizeKeyText(privateKeyBase64));
        }
        catch (FormatException e)
        {
            throw new ApipException("privateKey 不是合法的 Base64（应为单行 Base64 的 PKCS#8/PKCS#1 私钥）。", e);
        }
        var rsa = RSA.Create();
        try
        {
            rsa.ImportPkcs8PrivateKey(key, out _);
            return rsa;
        }
        catch (CryptographicException)
        {
            try
            {
                rsa.ImportRSAPrivateKey(key, out _);   // PKCS#1 ("BEGIN RSA PRIVATE KEY")
                return rsa;
            }
            catch (CryptographicException e)
            {
                rsa.Dispose();
                throw new ApipException("privateKey 解析失败（既不是 PKCS#8 也不是 PKCS#1 私钥）。", e);
            }
        }
    }

    /// <summary>导入 RSA 公钥（Base64 的 X.509 SubjectPublicKeyInfo）。</summary>
    private static RSA ImportPublicKey(string publicKeyBase64)
    {
        byte[] key;
        try
        {
            key = Convert.FromBase64String(NormalizeKeyText(publicKeyBase64));
        }
        catch (FormatException e)
        {
            throw new ApipException("gatewayPublicKey 不是合法的 Base64（应为单行 Base64 的 X.509 公钥）。", e);
        }
        var rsa = RSA.Create();
        try
        {
            rsa.ImportSubjectPublicKeyInfo(key, out _);
            return rsa;
        }
        catch (CryptographicException e)
        {
            rsa.Dispose();
            throw new ApipException("gatewayPublicKey 解析失败（应为 X.509 SubjectPublicKeyInfo，Base64、单行）。", e);
        }
    }

    /// <summary>解析 AES 密钥并校验长度（AES 只支持 16/24/32 字节）。</summary>
    private static byte[] ImportAesKey(string aesKeyBase64)
    {
        byte[] key;
        try
        {
            key = Convert.FromBase64String(NormalizeKeyText(aesKeyBase64));
        }
        catch (FormatException e)
        {
            throw new ApipException("AES 密钥不是合法的 Base64。", e);
        }
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
        {
            throw new ApipException("AES 密钥 Base64 解码后必须是 16/24/32 字节（AES-128/192/256），实际 " + key.Length + " 字节。");
        }
        return key;
    }

    // ------------------------------------------------------------ AES 内容加解密

    /// <summary>AES 加密：UTF-8 明文 → 密文 Base64。</summary>
    public static string AesEncrypt(string content, string aesKeyBase64)
    {
        try
        {
            byte[] key = ImportAesKey(aesKeyBase64);
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = ZeroIv;
            using var enc = aes.CreateEncryptor();
            byte[] data = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(enc.TransformFinalBlock(data, 0, data.Length));
        }
        catch (ApipException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ApipException("AES加密失败：Aescontent = " + content + "; charset = UTF-8", e);
        }
    }

    /// <summary>AES 解密：密文 Base64 → UTF-8 明文。</summary>
    public static string AesDecrypt(string contentBase64, string aesKeyBase64)
    {
        try
        {
            byte[] key = ImportAesKey(aesKeyBase64);
            byte[] cipher = Convert.FromBase64String(NormalizeKeyText(contentBase64));
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = ZeroIv;
            using var dec = aes.CreateDecryptor();
            return Encoding.UTF8.GetString(dec.TransformFinalBlock(cipher, 0, cipher.Length));
        }
        catch (ApipException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ApipException("AES解密失败：Aescontent = " + contentBase64 + "; charset = UTF-8", e);
        }
    }

    // ------------------------------------------------------------ 密钥自检（排障用：只输出摘要，不输出密钥本身）

    /// <summary>
    /// 私钥摘要：密钥位数 + **由私钥推导出的公钥指纹**（SHA256 前 16 位十六进制）。
    /// 用于与开放平台上为该 appid 维护的公钥核对（签名验证失败时最常查的就是这里）。
    /// 不输出任何密钥内容；解析失败时返回失败原因。
    /// </summary>
    public static string DescribePrivateKey(string privateKeyBase64)
    {
        try
        {
            using var rsa = ImportPrivateKey(privateKeyBase64);
            return "RSA-" + rsa.KeySize + "，由私钥推导的公钥指纹(sha256/16)=" + Fingerprint(rsa.ExportSubjectPublicKeyInfo());
        }
        catch (Exception e)
        {
            return "解析失败：" + e.Message;
        }
    }

    /// <summary>网关公钥摘要：密钥位数 + 指纹（SHA256 前 16 位），用于与网关侧核对。</summary>
    public static string DescribePublicKey(string publicKeyBase64)
    {
        try
        {
            using var rsa = ImportPublicKey(publicKeyBase64);
            return "RSA-" + rsa.KeySize + "，指纹(sha256/16)=" + Fingerprint(rsa.ExportSubjectPublicKeyInfo());
        }
        catch (Exception e)
        {
            return "解析失败：" + e.Message;
        }
    }

    /// <summary>AES 密钥摘要：Base64 解码后的字节数（16/24/32 → AES-128/192/256）。</summary>
    public static string DescribeAesKey(string aesKeyBase64)
    {
        try
        {
            byte[] key = ImportAesKey(aesKeyBase64);
            return key.Length * 8 + " 位（" + key.Length + " 字节）";
        }
        catch (Exception e)
        {
            return "解析失败：" + e.Message;
        }
    }

    private static string Fingerprint(byte[] data)
    {
        byte[] hash = SHA256.HashData(data);
        var sb = new StringBuilder(16);
        for (int i = 0; i < 8; i++)
        {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();
    }

    // ------------------------------------------------------------ RSA 签名 / 验签

    /// <summary>
    /// 请求签名：RSA = SHA1WithRSA，RSA2 = SHA256WithRSA（PKCS#1 v1.5），输出 Base64。
    /// </summary>
    public static string RsaSign(string content, string signType, string privateKeyBase64)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(content);
            using var rsa = ImportPrivateKey(privateKeyBase64);
            var hash = signType == "RSA2" ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA1;
            return Convert.ToBase64String(rsa.SignData(data, hash, RSASignaturePadding.Pkcs1));
        }
        catch (ApipException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ApipException("sign exception, please check the privateKey and signType are correct.", e);
        }
    }

    /// <summary>
    /// 响应验签：RSA = SHA1WithRSA，RSA2 = SHA256WithRSA。
    /// 注意：对 RSA2 请求，网关返回的报文是按 **SHA1WithRSA** 签的，因此默认即用 "RSA" 校验
    /// （见 <c>ApipClient</c> 的 responseSignType）。
    /// </summary>
    public static bool RsaVerify(string content, string signType, string publicKeyBase64, string signBase64)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(content);
            byte[] sig = Convert.FromBase64String(NormalizeKeyText(signBase64));
            using var rsa = ImportPublicKey(publicKeyBase64);
            var hash = signType == "RSA2" ? HashAlgorithmName.SHA256 : HashAlgorithmName.SHA1;
            return rsa.VerifyData(data, sig, hash, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }
}
