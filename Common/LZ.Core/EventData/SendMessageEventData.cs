using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 发送Hub消息
    /// </summary>
    public class SendMessageEventData : EventBase
    {
        /// <summary>
        /// 发送对象
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 传递参数
        /// </summary>
        public string Json { get; set; }
    }
}
