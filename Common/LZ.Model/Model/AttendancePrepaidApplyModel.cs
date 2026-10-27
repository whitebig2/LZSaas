using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Model
{
    public class AttendancePrepaidApplyModel 
    {
        
        public string BillMonth { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        public int? PartnerType { get; set; }
        public int? PartnerLevel { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 在职人数
        /// </summary>
        public int OnTheJobCount { get; set; }
        /// <summary>
        /// 今日入职人数
        /// </summary>
        public int ToDayEntryCount { get; set; }
        /// <summary>
        /// 今日离职人数
        /// </summary>
        public int ToDayLeaveCount { get; set; }

    }
}
