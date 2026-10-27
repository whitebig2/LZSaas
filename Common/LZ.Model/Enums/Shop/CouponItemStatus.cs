using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum CouponItemStatus
    {
        /// <summary>
        /// 待绑定
        /// </summary>
        WaitExchange = 1,
        /// <summary>
        /// 待使用
        /// </summary>
        WaitUse = 2,
        /// <summary>
        /// 已使用
        /// </summary>
        HaveUse = 3,
        /// <summary>
        /// 作废
        /// </summary>
        Void = 4
    }
}
