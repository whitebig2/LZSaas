using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum CouponType
    {
        /// <summary>
        /// 抵扣券
        /// </summary>
        Deduct = 1,
        /// <summary>
        /// 代金券
        /// </summary>
        Voucher = 2,
        /// <summary>
        /// 折扣券
        /// </summary>
        Discount = 3,
        /// <summary>
        /// 体验券
        /// </summary>
        Experience = 4
    }
}
