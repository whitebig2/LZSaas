using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Common
{
    public class AesCBCHelper
    {
        /// <summary>
        /// AES 加密 (CBC模式, PKCS7填充)
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥 (16, 24, or 32 bytes)</param>
        /// <param name="iv">初始化向量 (16 bytes)</param>
        /// <returns>Base64编码的密文</returns>
        public static string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plain text cannot be null or empty.");

            // 验证密钥和IV长度
            if (key == null || key.Length != 16 && key.Length != 24 && key.Length != 32)
                throw new ArgumentException("Key must be 16, 24, or 32 bytes.");
            if (iv == null || iv.Length != 16)
                throw new ArgumentException("IV must be 16 bytes.");

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// AES 解密 (CBC模式, PKCS7填充)
        /// </summary>
        /// <param name="cipherText">Base64编码的密文</param>
        /// <param name="key">密钥 (必须与加密时一致)</param>
        /// <param name="iv">初始化向量 (必须与加密时一致)</param>
        /// <returns>解密后的明文</returns>
        public static string Decrypt(string cipherText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Cipher text cannot be null or empty.");

            // 验证密钥和IV长度
            if (key == null || key.Length != 16 && key.Length != 24 && key.Length != 32)
                throw new ArgumentException("Key must be 16, 24, or 32 bytes.");
            if (iv == null || iv.Length != 16)
                throw new ArgumentException("IV must be 16 bytes.");

            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成随机密钥和IV用于测试
        /// </summary>
        public static void GenerateKeyAndIV(out byte[] key, out byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                key = aes.Key;
                iv = aes.IV;
            }
        }


        public static string GenerateHmac(string message, byte[] secretKey)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentException("Message cannot be null or empty.");
            //if (string.IsNullOrEmpty(secretKey)) throw new ArgumentException("Secret Key cannot be null or empty.");

            // 1. 将密钥和消息转换为字节数组 (必须统一编码，推荐 UTF-8)
            byte[] keyBytes = secretKey;
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // 2. 计算 HMAC-SHA256
            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                // 3. 转换为 Base64 字符串以便传输
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyHmac(string message, string receivedSignature, byte[] secretKey)
        {
            if (string.IsNullOrEmpty(receivedSignature)) return false;

            // 1. 重新计算本地签名
            string computedSignature = GenerateHmac(message, secretKey);

            // 2. 安全比较 (防止时序攻击)
            // 注意：直接比较字符串可能泄露时间信息，建议比较字节数组或使用固定时间比较
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedSignature),
                Encoding.UTF8.GetBytes(receivedSignature)
            );
        }
    }
}
