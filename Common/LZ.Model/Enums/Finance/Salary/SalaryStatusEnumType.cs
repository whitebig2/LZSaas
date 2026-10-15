using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public enum SalaryStatusEnumType
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Pending = 0,
        /// <summary>
        /// 待计算
        /// </summary>
        AwaitingCalculation = 1,
        /// <summary>
        /// 待推送
        /// </summary>
        PendingPush = 2,
        /// <summary>
        /// 待审核
        /// </summary>
        PendingReview = 3,
        /// <summary>
        /// 审核退回
        /// </summary>
        ReviewRejected = 4,
        /// <summary>
        /// 待发放
        /// </summary>
        PendingSend = 5,
        /// <summary>
        /// 已发放
        /// </summary>
        SucessSend = 6,
        /// <summary>
        /// 发放失败
        /// </summary>
        FailSend = 7,
        /// <summary>
        /// 作废
        /// </summary>
        Cancelled = 8,


    }
}
