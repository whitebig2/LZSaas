using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Options
{
    public class CzyOption
    {
        public string TenantId { get; set; }
        public string SystemNo { get; set; }
        public string SystemName { get; set; }
        public string AppId { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string SignMethod { get; set; }
        public string EncryptMethod { get; set; }
        public string EncryptKey { get; set; }
        /// <summary>
        /// APIP 网关地址，如 https://apipcssaasDMZ4.dccnet.com.cn
        /// </summary>
        public string GatewayUrl { get; set; }
    }
}
