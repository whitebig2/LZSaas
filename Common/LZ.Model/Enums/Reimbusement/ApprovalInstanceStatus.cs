using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 审批实例状态
    /// </summary>
    public enum ApprovalInstanceStatus : byte
    {
        /// <summary>
        /// 进行中
        /// </summary>
        [System.ComponentModel.Description("进行中")]
        InProgress = 1,
        /// <summary>
        /// 已通过
        /// </summary>

        [System.ComponentModel.Description("已通过")]
        Approved = 2,
        /// <summary>
        /// 已退回
        /// </summary>

        [System.ComponentModel.Description("已退回")]
        Returned = 3
    }
}
