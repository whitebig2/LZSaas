using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum ProductType
    {
        /// <summary>
        /// 普通商品
        /// </summary>
        Ordinary = 1,
        /// <summary>
        /// 虚拟商品
        /// </summary>
        Virtual = 2,
        /// <summary>
        /// 充值商品
        /// </summary>
        Recharge = 3,
        /// <summary>
        /// 团购券
        /// </summary>
        GroupCoupon = 4,
        /// <summary>
        /// 团购套餐
        /// </summary>
        GroupProduct = 5
    }
}
