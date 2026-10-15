using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.RabbitMQ
{
    /// <summary>
    /// 用于本地测试
    /// </summary>
    public class NullEventPublisher : EventBus.IEventPublisher
    {
        public void Publish<TEvent>(TEvent message) where TEvent : EventBus.IEvent
        {
            // 本地开发空实现，不发送消息
        }
    }

}
