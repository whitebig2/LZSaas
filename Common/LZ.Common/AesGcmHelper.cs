using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Common
{
    public static class AesGcmHelper
    {
        // 加密（自动处理Nonce和Tag）
        public static string Encrypt(string plaintext, byte[] key)
        {
            ValidateKey(key);

            var plainBytes = Encoding.UTF8.GetBytes(plaintext);

            var cipherBytes = new byte[plainBytes.Length];
            var nonce = new byte[12];
            var tag = new byte[16];
            RandomNumberGenerator.Fill(nonce);
            RandomNumberGenerator.Fill(tag);

            using var aesGcm = new AesGcm(key);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            // 数据格式: Nonce(12) + Tag(16) + CipherText
            var combinedBytes = CombineBytes(nonce, tag, cipherBytes);
            return Convert.ToBase64String(combinedBytes);
        }

        // 解密（自动验证完整性）
        public static string Decrypt(string encryptedData, byte[] key)
        {
            ValidateKey(key);

            var combinedBytes = Convert.FromBase64String(encryptedData);
            if (combinedBytes.Length < 28) // Nonce(12)+Tag(16)
                throw new ArgumentException("无效的加密数据");

            var nonce = new byte[12];
            var tag = new byte[16];
            RandomNumberGenerator.Fill(nonce);
            RandomNumberGenerator.Fill(tag);
            var cipherBytes = new byte[combinedBytes.Length - 28];

            Buffer.BlockCopy(combinedBytes, 0, nonce, 0, 12);
            Buffer.BlockCopy(combinedBytes, 12, tag, 0, 16);
            Buffer.BlockCopy(combinedBytes, 28, cipherBytes, 0, cipherBytes.Length);

            var plainBytes = new byte[cipherBytes.Length];

            using var aesGcm = new AesGcm(key);
            try
            {
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("解密失败：数据可能被篡改或密钥错误", ex);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }

        // 生成安全的256位密钥
        public static byte[] GenerateSecureKey()
        {
            var keyBytes = new byte[32];
            RandomNumberGenerator.Fill(keyBytes); // 256-bit
            return keyBytes;
        }

        private static byte[] CombineBytes(byte[] nonce, byte[] tag, byte[] cipherBytes)
        {
            var combined = new byte[nonce.Length + tag.Length + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherBytes, 0, combined, nonce.Length + tag.Length, cipherBytes.Length);
            return combined;
        }

        private static void ValidateKey(byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("密钥必须为256位(32字节)");
        }
    }
}
