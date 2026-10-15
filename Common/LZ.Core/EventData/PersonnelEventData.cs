using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 人事注册
    /// </summary>
    public class PersonnelEventData : EventBase
    {
        /// <summary>
        /// 服务商ID
        /// </summary>
        public int PartnerID { get; set; }
        /// <summary>
        ///服务商ID
        /// </summary>
        public Guid PartnerGID { get; set; }
        /// <summary>
        /// 所属客户
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        ///所属客户
        /// </summary>
        public Guid CustomerGID { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        /// <summary>
        /// 姓名
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// 服务商名称
        /// </summary>
        public string PartnerName { get; set; } = string.Empty;
        /// <summary>
        /// 人力公司
        /// </summary>
        public int CompanyID { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// 人力公司
        /// </summary>
        public Guid CompanyGID { get; set; }
        /// <summary>
        /// 人力公司
        /// </summary>
        public bool IsRegisterCompanyUser { get; set; }

    }
}
