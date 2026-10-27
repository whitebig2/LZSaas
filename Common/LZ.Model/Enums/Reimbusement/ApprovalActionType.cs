using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 审批操作类型
    /// </summary>
    public enum ApprovalActionType : byte
    {
        [System.ComponentModel.Description("通过")]
        Approve = 1,

        [System.ComponentModel.Description("驳回")]
        Reject = 2
    }
}
