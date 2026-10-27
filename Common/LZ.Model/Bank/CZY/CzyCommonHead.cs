using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY
{
    public class CzyCommonHead
    {
        /// <summary>
        /// 报文标识号（必填）
        /// </summary>
        public string MesgNo { get; set; }
        /// <summary>
        /// 商户号（必填）
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// 时间戳（必填）
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 接入系统编号（必填）
        /// </summary>
        public string SystemNo { get; set; }
        /// <summary>
        /// 接入系统名称（必填）
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// 报文请求号（必填）
        /// </summary>
        public string RequestNo { get; set; }
    }
}
