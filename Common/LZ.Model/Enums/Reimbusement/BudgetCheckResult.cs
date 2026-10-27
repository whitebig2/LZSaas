using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 预算校验结果
    /// </summary>
    public enum BudgetCheckResult : byte
    {
        InBudget = 1,
        OverBudget = 2,
        Failed = 3
    }

}
