using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 服务商发送通知
    /// </summary>
    public class PartnerSendNoticeEventData : EventBase
    {
        /// <summary>
        /// 服务商
        /// </summary>
        public int PartnerID { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>
        public PersonnelType UserType { get; set; }
        /// <summary>
        /// 通知标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 通知内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 小程序页面
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// 发送类型
        /// </summary>
        public int SendType { get; set; }
    }
}
