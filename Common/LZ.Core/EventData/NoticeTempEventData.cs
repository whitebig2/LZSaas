using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 生成订阅消息的消息体
    /// </summary>
    public class NoticeTempEventData : EventBase
    {
        /// <summary>
        /// 应用
        /// </summary>
        public int ClientID { get; set; }
        public string Type { get; set; }
    }
}
