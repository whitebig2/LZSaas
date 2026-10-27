using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum InvoiceStatusEnumType
    {
        待锁定 = 0,
        待审核 = 1,
        审核退回 = 2,
        待开具 = 3,
        已开具 = 4,
        冲红 = 5,
        冲红_红字 = 6,
    }
}
