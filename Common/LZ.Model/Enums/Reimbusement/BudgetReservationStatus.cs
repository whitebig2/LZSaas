using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 预算预留状态
    /// </summary>
    public enum BudgetReservationStatus : byte
    {
        None = 0,
        Reserved = 1,
        Committed = 2,
        Released = 3
    }
}
