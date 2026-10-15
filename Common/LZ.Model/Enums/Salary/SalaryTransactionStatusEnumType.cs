using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum SalaryTransactionStatusEnumType
    {
        待支付 = 0,
        待系统推送 = 1,
        待核对支付结果 = 2,
        支付成功 = 3,
        支付失败 = 4,
        待财务确认支付结果 = 5,
        取消支付 = 6,
    }
}
