using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 服务商消息体
    /// </summary>
    public class PartnerEventData : EventBase
    {
        /// <summary>
        /// 所属公司
        /// </summary>
        public int CompanyID { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid CompanyGID { get; set; }
        /// <summary>
        /// 所属客户
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 所属客户
        /// </summary>
        public Guid CustomerGID { get; set; }
        /// <summary>
        /// 服务商名称
        /// </summary>
        public string PartnerName { get; set; }
        /// <summary>
        /// 登录手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 默认姓名
        /// </summary>
        public string FullName { get; set; }
        public int PartnerType { get; set; }
    }
}
