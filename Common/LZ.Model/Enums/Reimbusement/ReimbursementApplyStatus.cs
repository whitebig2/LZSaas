using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 申请主流程状态 (PRD 唯一标准 4 个主状态)
    /// </summary>
    public enum ReimbursementApplyStatus : byte
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [System.ComponentModel.Description("待审核")]
        PendingReview = 10,
        /// <summary>
        /// 审核退回
        /// </summary>

        [System.ComponentModel.Description("审核退回")]
        ReviewReturned = 20,
        /// <summary>
        /// 待打款
        /// </summary>

        [System.ComponentModel.Description("待打款")]
        PendingPayment = 30,
        /// <summary>
        /// 已打款
        /// </summary>

        [System.ComponentModel.Description("已打款")]
        Paid = 40
    }
}

