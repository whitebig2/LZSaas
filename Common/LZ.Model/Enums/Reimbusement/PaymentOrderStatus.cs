using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Reimbusement
{
    /// <summary>
    /// 打款单状态
    /// </summary>
    public enum PaymentOrderStatus : byte
    {
        [System.ComponentModel.Description("待打款")]
        Pending = 1,

        [System.ComponentModel.Description("打款中")]
        Processing = 2,

        [System.ComponentModel.Description("打款成功")]
        Success = 3,

        [System.ComponentModel.Description("打款失败")]
        Failed = 4
    }

}
