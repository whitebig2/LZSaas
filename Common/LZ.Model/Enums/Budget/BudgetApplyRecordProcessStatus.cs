using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Budget
{
    public enum BudgetApplyRecordProcessStatus
    {
        待处理 = 0,
        审核退回 = 1,
        待审核 = 2,
        待打款 = 3,
        已打款 = 4,
        作废 = 5,
    }
}
