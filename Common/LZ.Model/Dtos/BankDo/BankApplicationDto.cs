using LZ.Model.Enums;
using System;

namespace LZ.Model.Dtos.BankDo
{
    public class BankApplicationDto
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 银行类型
        /// </summary>
        public BankTypeEnumType BankType { get; set; }

        public int CompanyID { get; set; }

        public Guid CompanyGID { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// APP编号（开放平台注册生成）
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 签名类型（CA/RSA2/SM2，缺省RSA2）
        /// </summary>
        public string AppSignType { get; set; }

        /// <summary>
        /// 应用私钥
        /// </summary>
        public string AppPrivateKey { get; set; }

        /// <summary>
        /// 字符集（缺省UTF-8）
        /// </summary>
        public string AppCharset { get; set; }

        /// <summary>
        /// 请求参数格式（仅支持json）
        /// </summary>
        public string AppFormat { get; set; }

        /// <summary>
        /// 应用公钥
        /// </summary>
        public string AppPulicKey { get; set; }

        /// <summary>
        /// 加密类型（SM4/AES）
        /// </summary>
        public string AppEncryptType { get; set; }

        /// <summary>
        /// 加密密钥
        /// </summary>
        public string AppEncryptKey { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
