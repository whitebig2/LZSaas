using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Cash
{
    public class CashSignRecordEventData : EventBase
    {
        public int UserEntryID { get; set; }
        public int UserID { get; set; }

    }
}
