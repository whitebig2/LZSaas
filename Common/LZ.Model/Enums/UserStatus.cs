using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum UserStatus
    {
        /// <summary>
        /// 传0获取所有
        /// </summary>
        All = 0,

        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        Untreated = 1,

        /// <summary>
        /// 已入职
        /// </summary>
        [Description("已入职")]
        Entry = 2,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Cancel = 3,
        /// <summary>
        /// 已离职
        /// </summary>
        [Description("已离职")]
        Leave = 4,
        /// <summary>
        /// 已面试
        /// </summary>
        [Description("已面试")]
        Interviewed = 5,
        /// <summary>
        /// 待入职
        /// </summary>
        [Description("待入职")]
        WaitEntry = 6,
        /// <summary>
        /// 未面试
        /// </summary>
        [Description("未面试")]
        NotInterviewed = 7,
        /// <summary>
        /// 面试未通过
        /// </summary>
        [Description("面试未通过")]
        InterviewFailed = 8,
        /// <summary>
        /// 	面试跟踪中
        /// </summary>
        [Description("面试跟踪中")]
        InterviewTrack = 9,
        /// <summary>
        /// 待面试
        /// </summary>
        [Description("待面试")]
        WaitInterview = 10,
        /// <summary>
        /// 通知填写入职信息
        /// </summary>
        [Description("通知填写入职信息")]
        NoticeEntry = 11,
        /// <summary>
        /// 未入职
        /// </summary>
        [Description("未入职")]
        NotEntry = 12,
        /// <summary>
        /// 待扫码
        /// </summary>
        [Description("待扫码")]
        WaitScanCode = 13,
    }
}
