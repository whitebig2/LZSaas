using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 申请类型: 1-报销申请, 2-付款申请
    /// </summary>
    public enum ReimbursementApplyType : byte
    {
        [System.ComponentModel.Description("报销申请")]
        Reimbursement = 1,

        [System.ComponentModel.Description("付款申请")]
        Payment = 2
    }
}
