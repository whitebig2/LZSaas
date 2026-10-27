using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Insurance
{
    public class InsurancePushJobEventData1:EventBase
    {
        public string InsuranceCompany { get; set; }
    }
}
