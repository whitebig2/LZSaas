using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 员工银行卡消息体
    /// </summary>
    public class UserBankEventData : EventBase
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
        /// 卡号
        /// </summary>
        public string BankNumber { get; set; }
    }
}
