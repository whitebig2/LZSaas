using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    public class SchedulerTestEventData : EventBase
    {
        
        public string Content { get; set; }
        public string Remark { get; set; }
    }
}
