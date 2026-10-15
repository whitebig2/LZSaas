using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 更新人员openid消息体
    /// </summary>
    public class UserOpenIDEventData : EventBase
    {
        /// <summary>
        /// 员工
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 应用
        /// </summary>
        public int ClientID { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public Model.Enums.PlatformType Platform { get; set; }
        /// <summary>
        /// OpenID
        /// </summary>
        public string OpenID { get; set; }
        public string OpenIdType { get; set; }
    }
}
