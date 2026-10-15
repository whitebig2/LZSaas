using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Insurance
{
    public class BusinessInsuranceAddOrDelEventData : EventBase
    {
        public int UserEntryID { get; set; }
        /// <summary>
        /// 是否增员
        /// </summary>
        public bool IsAdd { get; set; }
    }
}
