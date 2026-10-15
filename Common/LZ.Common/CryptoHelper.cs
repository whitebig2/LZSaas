using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Common
{




    public static class CryptoHelper
    {
        // AES-GCM 固定参数：96位Nonce、128位Tag，跨平台全兼容
        public const int AesGcmNonceSize = 12;
        public const int AesGcmTagSize = 16;
        public const int AesKeySize = 32; // AES-256 密钥长度

        /// <summary>
        /// 生成AES-256随机密钥
        /// </summary>
        public static byte[] GenerateAesKey() => RandomNumberGenerator.GetBytes(AesKeySize);

        /// <summary>
        /// AES-GCM 加密：输出格式 [Nonce(12B) + Tag(16B) + 密文]
        /// </summary>
        public static string AesGcmEncrypt(byte[] key, string plainText, string aad = "")
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(AesGcmNonceSize);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherText = new byte[plainBytes.Length];
            byte[] tag = new byte[AesGcmTagSize];
            byte[] aadBytes = string.IsNullOrEmpty(aad) ? [] : Encoding.UTF8.GetBytes(aad);

            using var aesGcm = new AesGcm(key, AesGcmTagSize);
            aesGcm.Encrypt(nonce, plainBytes, cipherText, tag, aadBytes);

            // 拼接成完整传输包
            byte[] result = new byte[nonce.Length + tag.Length + cipherText.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherText, 0, result, nonce.Length + tag.Length, cipherText.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// AES-GCM 解密：自动从密文中拆分Nonce、Tag，校验失败直接抛异常
        /// </summary>
        public static string AesGcmDecrypt(byte[] key, string cipherBase64, string aad = "")
        {
            byte[] fullData = Convert.FromBase64String(cipherBase64);
            byte[] nonce = fullData.AsSpan(0, AesGcmNonceSize).ToArray();
            byte[] tag = fullData.AsSpan(AesGcmNonceSize, AesGcmTagSize).ToArray();
            byte[] cipherText = fullData.AsSpan(AesGcmNonceSize + AesGcmTagSize).ToArray();
            byte[] aadBytes = string.IsNullOrEmpty(aad) ? [] : Encoding.UTF8.GetBytes(aad);

            byte[] plainBytes = new byte[cipherText.Length];
            using var aesGcm = new AesGcm(key, AesGcmTagSize);
            aesGcm.Decrypt(nonce, cipherText, tag, plainBytes, aadBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <summary>
        /// HMAC-SHA256 生成签名
        /// </summary>
        public static string GenerateSign(byte[] key, string content)
        {
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(HMACSHA256.HashData(key, contentBytes));
        }

        /// <summary>
        /// RSA-OAEP 加密AES会话密钥（兼容Java PKCS#8公钥格式）
        /// </summary>
        public static string RsaWrapAesKey(string pemPublicKey, byte[] aesKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(pemPublicKey);
            return Convert.ToBase64String(rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256));
        }

        /// <summary>
        /// RSA-OAEP 解密AES会话密钥（兼容Java PKCS#8私钥格式）
        /// </summary>
        public static byte[] RsaUnwrapAesKey(string pemPrivateKey, string wrappedKeyBase64)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(pemPrivateKey);
            byte[] wrappedKey = Convert.FromBase64String(wrappedKeyBase64);
            return rsa.Decrypt(wrappedKey, RSAEncryptionPadding.OaepSHA256);
        }
    }

}
