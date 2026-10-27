using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 上传用途
    /// </summary>
    public enum InvoiceUploadPurpose : byte
    {
        [System.ComponentModel.Description("首次提交")]
        Initial = 1,

        [System.ComponentModel.Description("补传发票")]
        Supplement = 2
    }
}
