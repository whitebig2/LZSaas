using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 员工合同状态消息体
    /// </summary>
    public class UserContractStatusEventData : EventBase
    {
        /// <summary>
        /// 入职ID
        /// </summary>
        public int EntryID { get; set; }
        public Model.Enums.ContractStatus Status { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
    }
}
