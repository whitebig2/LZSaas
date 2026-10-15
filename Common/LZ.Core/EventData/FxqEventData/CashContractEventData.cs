using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.FxqEventData
{
    public class CashContractEventData : EventBase
    {
        public Guid CustomerGID { get; set; }
        public Guid SalaryCashSendRecordID { get; set; }
    }
}
