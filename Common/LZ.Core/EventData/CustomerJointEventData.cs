using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 员工实名消息体
    /// </summary>
    public class CustomerJointEventData : EventBase
    {
        /// <summary>
        /// 客户
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 服务商
        /// </summary>
        public string PartnerIDS { get; set; }
        /// <summary>
        /// 服务商对象类型
        /// </summary>
        public string JointObjType { get; set; }
    }
}
