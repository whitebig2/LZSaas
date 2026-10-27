using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 流程版本: 1-审核, 2-预算 (由总后台账号配置决定)
    /// </summary>
    public enum ReimbursementFlowType : byte
    {
        [System.ComponentModel.Description("审核")]
        Audit = 1,

        [System.ComponentModel.Description("预算")]
        Budget = 2
    }
}
