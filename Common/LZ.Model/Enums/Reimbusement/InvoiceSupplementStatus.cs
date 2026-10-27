using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 待补传发票状态 (付款申请专用)
    /// </summary>
    public enum InvoiceSupplementStatus : byte
    {
        [System.ComponentModel.Description("不适用")]
        NotApplicable = 0,

        [System.ComponentModel.Description("待补传发票")]
        PendingInvoice = 1,

        [System.ComponentModel.Description("已关闭")]
        Closed = 2
    }

}
