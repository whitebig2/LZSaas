using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 发票真伪核验状态
    /// </summary>
    public enum InvoiceVerifyStatus : byte
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }
}
