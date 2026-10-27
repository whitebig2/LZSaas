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
        [System.ComponentModel.Description("进行中")]
        InProgress = 1,

        [System.ComponentModel.Description("已通过")]
        Approved = 2,

        [System.ComponentModel.Description("已退回")]
        Returned = 3
    }
}
