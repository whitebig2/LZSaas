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
    /// 入职记录日志的数据
    /// </summary>
    public class EntryRecordEventData : EventBase
    {
        /// <summary>
        /// 员工
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 入职记录
        /// </summary>
        public int EntryID { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string About { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 二维码类型
        /// </summary>
        public int CID { get; set; }
        /// <summary>
        /// 驻厂人员
        /// </summary>
        public int PerUserID { get; set; }
        /// <summary>
        /// 劳务经纪人
        /// </summary>
        public int LaborUserID { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? EntryTime { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime? LeaveTime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public int OperateUserID { get; set; }
        /// <summary>
        /// 操作来源平台
        /// </summary>
        public Model.Enums.PlatformType OperateFrom { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateFullName { get; set; }
        /// <summary>
        /// 操作来源平台
        /// </summary>
        public string OperateFromName { get; set; }
    }
}
