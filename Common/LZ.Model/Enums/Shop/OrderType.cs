using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum OrderType
    {
        /// <summary>
        /// 商品订单
        /// </summary>
        Product = 1,
        /// <summary>
        /// 虚拟订单
        /// </summary>
        Virtual = 2,
        /// <summary>
        /// 充值订单
        /// </summary>
        Recharge = 3,
        /// <summary>
        /// 团购订单
        /// </summary>
        Group = 4,
        /// <summary>
        /// 优惠券订单
        /// </summary>
        Coupon = 5
    }
}
