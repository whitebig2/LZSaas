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
    public class ClientReleaseEventData : EventBase
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public int ClientID { get; set; }
    }
}
