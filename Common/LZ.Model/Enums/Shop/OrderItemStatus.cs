using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum OrderItemStatus
    {
        /// <summary>
        /// 待付款
        /// </summary>
        WaitPay = 1,
        /// <summary>
        /// 已付款待发货
        /// </summary>
        WaitShip = 2,
        /// <summary>
        /// 已发货
        /// </summary>
        HaveShip = 3,
        /// <summary>
        /// 待收货
        /// </summary>
        WaitReceipt = 4,
        /// <summary>
        /// 已完成
        /// </summary>
        Have = 5,
        /// <summary>
        /// 售后中
        /// </summary>
        Support = 6,
            /// <summary>
            /// 售后完成
            /// </summary>
        HaveSupport = 7,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 8,
        /// <summary>
        /// 已退单
        /// </summary>
        Refund = 9
    }
}
