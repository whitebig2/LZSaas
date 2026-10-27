using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 公司员工实名消息体
    /// </summary>
    public class CompanyUserEventData : EventBase
    {
        /// <summary>
        /// 所属公司ID
        /// </summary>
        public int CompanyID { get; set; }
        /// <summary>
        /// 所属公司ID
        /// </summary>
        public Guid CompanyGID { get; set; }
        /// <summary>
        /// 所属服务商ID
        /// </summary>
        public int PartnerID { get; set; }
        /// <summary>
        /// 所属服务商ID
        /// </summary>
        public Guid PartnerGID { get; set; }
        /// <summary>
        /// 所属驻厂
        /// </summary>
        public int PersonnelID { get; set; }
        /// <summary>
        /// 所属驻厂
        /// </summary>
        public Guid PersonnelGID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string RoleIDS { get; set; }
        public string RoleNames { get; set; }
        public string Phone { get; set; }
        public int UserType { get; set; }
    }
}
