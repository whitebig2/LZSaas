using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 生成订阅消息的消息体
    /// </summary>
    public class SendNoticeEventData : EventBase
    {
        /// <summary>
        /// 员工
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public Model.Enums.PlatformType Platform { get; set; }
        /// <summary>
        /// 通知类型
        /// </summary>
        public Model.Enums.NoticeEnumType NoticeType { get; set; }
        public string Keyword1 { get; set; }
        public string Keyword2 { get; set; }
        public string Keyword3 { get; set; }
        public string Keyword4 { get; set; }
        public string Keyword5 { get; set; }
        /// <summary>
        /// 发送类型  是全部还是仅即时通讯
        /// </summary>
        public Model.Enums.NoticeSendEnumType SendType { get; set; } = Model.Enums.NoticeSendEnumType.All;
        /// <summary>
        /// 跳转的页面
        /// </summary>
        public string Page { get; set; }
    }
}
