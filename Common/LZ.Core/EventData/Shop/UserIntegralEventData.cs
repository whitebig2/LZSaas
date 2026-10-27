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
    /// 会员积分计算
    /// </summary>
    public class UserIntegralEventData : EventBase
    {
        /// <summary>
        /// 所属会员ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 积分数量
        /// </summary>
        public int Integral { get; set; }
        /// <summary>
        /// 所属服务商ID
        /// </summary>
        public int PartnerID { get; set; }
        /// <summary>
        /// 所属服务商ID
        /// </summary>
        public Guid PartnerGID { get; set; }
        /// <summary>
        /// 来源会员
        /// </summary>
        public int FromUserID { get; set; }
        /// <summary>
        /// 积分类型
        /// </summary>
        public IntegralType IntegralType { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }
        public string Remark { get; set; }
    }
}
