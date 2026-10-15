using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 员工实名消息体
    /// </summary>
    public class UserRealEventData : EventBase
    {
        /// <summary>
        /// 员工
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string Message { get; set; }
        public int RealLevel { get; set; }
    }
}
