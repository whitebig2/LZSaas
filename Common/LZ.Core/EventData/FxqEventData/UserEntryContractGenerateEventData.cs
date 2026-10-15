using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.FxqEventData
{
    public class UserEntryContractGenerateEventData : EventBase
    {

        public Guid BatchNo { get; set; }
        public ContractEnumType ContractType { get; set; }

        public UserEntryContractGenerateEventData(Guid batchNo, ContractEnumType contractType)
        {
            BatchNo = batchNo;
            ContractType = contractType;
        }
    }
}
