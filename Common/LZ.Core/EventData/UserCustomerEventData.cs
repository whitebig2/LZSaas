using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 获取客户固定的入职信息
    /// </summary>
    public class UserCustomerEventData : EventBase
    {
        /// <summary>
        /// 员工
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 入职记录
        /// </summary>
        public int EntryID { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerID { get; set; }
        
    }
}
