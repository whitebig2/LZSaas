using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum NoticeEnumType
    {

        /// <summary>
        /// 入职通知
        /// </summary>
        UserEntry = 1,

        /// <summary>
        /// 离职通知
        /// </summary>
        UserLeave = 2,
        /// <summary>
        /// 工资条提醒
        /// </summary>
        UserPayslip = 3,
        /// <summary>
        /// 工资发放
        /// </summary>
        UserPayroll = 4,

        /// <summary>
        /// 预发通知
        /// </summary>
        UserAdvance = 5,
        /// <summary>
        /// 合同签署
        /// </summary>
        UserContract = 6,
        /// <summary>
        /// 业务办理或待办事情提醒
        /// </summary>
        Remind = 7
    }
}
