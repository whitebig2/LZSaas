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
    /// 积分日志
    /// </summary>
    public class IntegralLogEventData : EventBase
    {
        /// <summary>
        /// 所属会员ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 所属会员ID
        /// </summary>
        public int UserGID { get; set; }
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
        /// 人力公司ID
        /// </summary>
        public int CompanyID { get; set; }
        /// <summary>
        /// 人力公司ID
        /// </summary>
        public Guid CompanyGID { get; set; }
        /// <summary>
        /// 来源会员
        /// </summary>
        public int FromUserID { get; set; }
        /// <summary>
        /// 岗位ID
        /// </summary>
        public int FromJobID { get; set; }
        /// <summary>
        /// 职喜用户ID
        /// </summary>
        public int ToLZUserID { get; set; }
        /// <summary>
        /// 积分类型
        /// </summary>
        public IntegralType IntegralType { get; set; }
        /// <summary>
        /// 是否更新会员积分
        /// </summary>
        public bool IsUpdate { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }
        public string Remark { get; set; }
        public string RefundID { get; set; }
    }
}
