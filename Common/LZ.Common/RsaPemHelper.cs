using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Common
{
    public class RsaPemHelper
    {
        private readonly RSA _rsaPrivate;
        private readonly RSA _rsaPublic;
        private readonly int _keySize;

        /// <summary>
        /// 构造函数：加载公钥和私钥
        /// </summary>
        /// <param name="privateKeyPem">PEM格式私钥字符串</param>
        /// <param name="publicKeyPem">PEM格式公钥字符串</param>
        public RsaPemHelper(string privateKeyPem, string publicKeyPem)
        {
            if (string.IsNullOrEmpty(privateKeyPem) && string.IsNullOrEmpty(publicKeyPem))
            {
                throw new ArgumentException("至少需要提供公钥或私钥之一");
            }

            // 初始化 RSA 实例 (.NET Core 推荐方式，自动适配底层 OS)
            _rsaPrivate = string.IsNullOrEmpty(privateKeyPem) ? null : CreateRsaFromPrivateKey(privateKeyPem);
            _rsaPublic = string.IsNullOrEmpty(publicKeyPem) ? null : CreateRsaFromPublicKey(publicKeyPem);

            // 获取密钥长度用于分段计算
            _keySize = _rsaPrivate?.KeySize ?? _rsaPublic?.KeySize ?? 2048;
        }

        #region 密钥加载辅助方法

        /// <summary>
        /// 从 PEM 格式私钥字符串创建 RSA 对象
        /// 支持 PKCS#1 (BEGIN RSA PRIVATE KEY) 和 PKCS#8 (BEGIN PRIVATE KEY)
        /// </summary>
        private static RSA CreateRsaFromPrivateKey(string privateKey)
        {
            var rsa = RSA.Create();
            var privateKeyBytes = Convert.FromBase64String(ExtractPemContent(privateKey));

            // 尝试导入 PKCS#8
            try
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            catch
            {
                // 如果失败，尝试导入 PKCS#1
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            }
            return rsa;
        }

        /// <summary>
        /// 从 PEM 格式公钥字符串创建 RSA 对象
        /// 支持 SubjectPublicKeyInfo (BEGIN PUBLIC KEY)
        /// </summary>
        private static RSA CreateRsaFromPublicKey(string publicKey)
        {
            var rsa = RSA.Create();
            var publicKeyBytes = Convert.FromBase64String(ExtractPemContent(publicKey));

            // 导入 SubjectPublicKeyInfo 格式
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            return rsa;
        }

        /// <summary>
        /// 提取 PEM 内容中的 Base64 部分
        /// </summary>
        private static string ExtractPemContent(string pem)
        {
            if (string.IsNullOrWhiteSpace(pem)) return string.Empty;

            // 移除头尾标识和换行符
            var sb = new StringBuilder();
            using (var reader = new StringReader(pem))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.Contains("BEGIN") && !line.Contains("END"))
                    {
                        sb.Append(line.Trim());
                    }
                }
            }
            return sb.ToString();
        }

        #endregion

        #region 加密与解密

        /// <summary>
        /// RSA 公钥加密
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (_rsaPublic == null) throw new InvalidOperationException("未加载公钥，无法加密");

            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            // RSA 最大加密明文大小 = 密钥长度/8 - 11 (PKCS1Padding)
            int maxBlockSize = _keySize / 8 - 11;

            // 如果数据长度超过限制，需要分段加密
            if (dataToEncrypt.Length <= maxBlockSize)
            {
                byte[] encryptedBytes = _rsaPublic.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(encryptedBytes);
            }
            else
            {
                return EncryptSegmented(dataToEncrypt, maxBlockSize, true);
            }
        }

        /// <summary>
        /// RSA 私钥解密
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (_rsaPrivate == null) throw new InvalidOperationException("未加载私钥，无法解密");

            byte[] dataToDecrypt = Convert.FromBase64String(cipherText);
            // RSA 最大解密密文大小 = 密钥长度/8
            int maxBlockSize = _keySize / 8;

            // 判断是否分段加密过 (简单判断：如果密文长度远大于单次最大密文长度)
            // 更严谨的做法是在加密时添加标记，这里假设如果超过单次限制则分段解密
            if (dataToDecrypt.Length <= maxBlockSize)
            {
                byte[] decryptedBytes = _rsaPrivate.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            else
            {
                return DecryptSegmented(dataToDecrypt, maxBlockSize);
            }
        }

        /// <summary>
        /// 分段加密实现
        /// </summary>
        private string EncryptSegmented(byte[] data, int blockSize, bool isEncrypt)
        {
            using (var memoryStream = new MemoryStream())
            {
                int offset = 0;
                while (offset < data.Length)
                {
                    int count = Math.Min(blockSize, data.Length - offset);
                    byte[] buffer = new byte[count];
                    Array.Copy(data, offset, buffer, 0, count);

                    byte[] encryptedBlock;
                    if (isEncrypt)
                        encryptedBlock = _rsaPublic.Encrypt(buffer, RSAEncryptionPadding.Pkcs1);
                    else
                        encryptedBlock = _rsaPrivate.Decrypt(buffer, RSAEncryptionPadding.Pkcs1); // 此处逻辑仅用于encrypt调用，decrypt见下方

                    memoryStream.Write(encryptedBlock, 0, encryptedBlock.Length);
                    offset += count;
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        // 修正：上面的 EncryptSegmented 混合了逻辑，下面拆分清晰的分段解密
        private string DecryptSegmented(byte[] data, int blockSize)
        {
            using (var memoryStream = new MemoryStream())
            {
                int offset = 0;
                while (offset < data.Length)
                {
                    int count = Math.Min(blockSize, data.Length - offset);
                    byte[] buffer = new byte[count];
                    Array.Copy(data, offset, buffer, 0, count);

                    byte[] decryptedBlock = _rsaPrivate.Decrypt(buffer, RSAEncryptionPadding.Pkcs1);
                    memoryStream.Write(decryptedBlock, 0, decryptedBlock.Length);
                    offset += count;
                }
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        // 重新整理 Encrypt 中的分段调用，避免逻辑混乱
        public string EncryptAdvanced(string plainText)
        {
            if (_rsaPublic == null) throw new InvalidOperationException("未加载公钥，无法加密");
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            int maxBlockSize = _keySize / 8 - 11;

            if (dataToEncrypt.Length <= maxBlockSize)
            {
                return Convert.ToBase64String(_rsaPublic.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1));
            }

            // 分段加密
            using (var memoryStream = new MemoryStream())
            {
                int offset = 0;
                while (offset < dataToEncrypt.Length)
                {
                    int count = Math.Min(maxBlockSize, dataToEncrypt.Length - offset);
                    byte[] buffer = new byte[count];
                    Array.Copy(dataToEncrypt, offset, buffer, 0, count);
                    byte[] encryptedBlock = _rsaPublic.Encrypt(buffer, RSAEncryptionPadding.Pkcs1);
                    memoryStream.Write(encryptedBlock, 0, encryptedBlock.Length);
                    offset += count;
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        #endregion

        #region 签名与验签

        /// <summary>
        /// 使用私钥签名
        /// </summary>
        public string Sign(string data)
        {
            if (_rsaPrivate == null) throw new InvalidOperationException("未加载私钥，无法签名");

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            // 推荐使用 SHA256
            byte[] signature = _rsaPrivate.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }

        /// <summary>
        /// 使用公钥验证签名
        /// </summary>
        public bool Verify(string data, string signature)
        {
            if (_rsaPublic == null) throw new InvalidOperationException("未加载公钥，无法验签");

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            return _rsaPublic.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        #endregion

        public void Dispose()
        {
            _rsaPrivate?.Dispose();
            _rsaPublic?.Dispose();
        }
    }
}
