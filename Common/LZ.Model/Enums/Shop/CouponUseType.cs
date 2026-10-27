using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum CouponUseType
    {
        /// <summary>
        /// 商品通用券
        /// </summary>
        ProductCurrency = 1,
        /// <summary>
        /// 指定商品券
        /// </summary>
        AssignProduct = 2,
        /// <summary>
        /// 团购核销券
        /// </summary>
        GroupWrite = 3,
        /// <summary>
        /// 团购商品券
        /// </summary>
        GroupProduct = 4

    }
}
