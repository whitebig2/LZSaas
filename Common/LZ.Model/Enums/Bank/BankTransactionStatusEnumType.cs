using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum BankTransactionStatusEnumType
    {
        待锁定 = 0,
        待推送 = 1,
        推送失败 = 2,
        已推送待查询 = 3,
        代发失败 = 4,
        发放成功 = 5,
        发放失败 = 6,
        作废 = 7,
    }
}
