using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Contract
{
    public class ContractSigningSendSmsEventData : EventBase
    {
        /// <summary>
        /// 合同记录ID
        /// </summary>
        public Guid ContractRecordID { get; set; }
        public string SmsName { get; set; }
        public string SmsCode { get; set; }
        public string SmsContent { get; set; }
        public string SmsPhone { get; set; }
    }
}
