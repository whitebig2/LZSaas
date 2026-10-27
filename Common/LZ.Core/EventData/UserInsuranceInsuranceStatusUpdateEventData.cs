using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    public class UserInsuranceInsuranceStatusUpdateEventData : EventBase
    {
        public int UserEntryID { get; set; }
        public UserEntryInsuranceStatusEnumType InsuranceStatus { get; set; }
    }
}
