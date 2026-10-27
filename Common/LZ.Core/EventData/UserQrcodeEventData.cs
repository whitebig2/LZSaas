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
    public class UserQrcodeEventData : EventBase
    {
        /// <summary>
        /// 二维码ID
        /// </summary>
        public int FromID { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string FromAction { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 人力公司id
        /// </summary>
        public int CompanyID { get; set; }
        /// <summary>
        /// 人力公司id
        /// </summary>
        public Guid CompanyGID { get; set; }
        /// <summary>
        /// 来源类型
        /// </summary>
        public string FromType { get; set; }=string.Empty;
    }
}
