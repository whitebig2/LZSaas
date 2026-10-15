using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Contract
{
    public class ContractSendSmsJobEventData: EventBase
    {
       public Guid BatchNo { get; set; }

        public ContractSendSmsJobEventData(Guid batchNo)
        {
            BatchNo = batchNo;
        }
    }
}
