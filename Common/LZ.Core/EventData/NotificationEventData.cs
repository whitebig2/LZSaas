using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 系统通知
    /// </summary>
    public class NotificationEventData : EventBase
    {
        public int? CompanyID { get; set; }
        public Guid? CompanyGID { get; set; }
        public int? PartnerID { get; set; }
        public Guid? PartnerGID { get; set; }
        public string Content { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public string TypeName { get; set; }
        public string Title { get; set; }
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
