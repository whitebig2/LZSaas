using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum SalaryApiSendStatusEnumType
    {
        None = 0,
        /// <summary>
        /// 待推送
        /// </summary>
        PendingPush = 1,
        /// <summary>
        /// 推送失败
        /// </summary>
        PendingPushFail = 2,
        /// <summary>
        /// 待发放
        /// </summary>
        PendingSend = 3,
        /// <summary>
        /// 待核对
        /// </summary>
        PendingCheck = 4,
        /// <summary>
        /// 发放成功
        /// </summary>
        SendSuc = 5,
        /// <summary>
        /// 发放失败
        /// </summary>
        SendFail = 6,
    }
}
