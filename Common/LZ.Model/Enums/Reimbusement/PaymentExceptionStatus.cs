using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 异常跟踪状态
    /// </summary>
    public enum PaymentExceptionStatus : byte
    {
        [System.ComponentModel.Description("待补传发票")]
        PendingInvoice = 1,

        [System.ComponentModel.Description("已关闭")]
        Closed = 2
    }
}
