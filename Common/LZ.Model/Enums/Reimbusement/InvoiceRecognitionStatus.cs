using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 发票识别状态
    /// </summary>
    public enum InvoiceRecognitionStatus : byte
    {
        Pending = 0,
        Success = 1,
        Failed = 2
    }

}
