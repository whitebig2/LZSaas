using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum CzyPayStatusEnumType
    {
        支付中=1,
        支付成功 = 2,
        支付失败 = 3,
        已退款 = 4,
        部分支付成功 = 5,
        部分退款 = 6,
        未知 = 10,
    }
}
