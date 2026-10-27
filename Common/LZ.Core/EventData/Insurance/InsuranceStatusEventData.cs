using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Insurance
{
    public class InsuranceStatusEventData:EventBase
    {
        public int UserEntryID { get; set; }

        public InsuranceStatusEventData(int userEntryID)
        {
            UserEntryID = userEntryID;
        }
    }
}
