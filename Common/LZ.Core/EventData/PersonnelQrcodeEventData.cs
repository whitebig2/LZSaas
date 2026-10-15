using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 生成二维码的数据
    /// </summary>
    public class PersonnelQrcodeEventData : EventBase
    {
        /// <summary>
        /// 二维码ID
        /// </summary>
        public int FromID { get; set; }
        /// <summary>
        /// 驻厂id
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 人力公司id
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// 客户id
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 二维码类型
        /// </summary>
        public string QrcodeType { get; set; }=string.Empty;
    }
}
