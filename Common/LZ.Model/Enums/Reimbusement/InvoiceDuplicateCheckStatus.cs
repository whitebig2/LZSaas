using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 发票查重状态
    /// </summary>
    public enum InvoiceDuplicateCheckStatus : byte
    {
        Pending = 0,
        Passed = 1,
        Duplicate = 2
    }
}
