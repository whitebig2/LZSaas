using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    /// <summary>
    /// 发票类型
    /// </summary>
    public enum InvoiceTypeEnumType
    {
        /// <summary>
        /// 增值税专用发票
        /// </summary>
        SpecialInvoice = 0,
        /// <summary>
        /// 增值税普通发票
        /// </summary>
        CommonInvoice = 1,
        /// <summary>
        /// 值税电子普通发票
        /// </summary>
        ECommonInvoice = 2,
        /// <summary>
        /// 增值税电子专用发票
        /// </summary>
        ESpecialInvoice = 3,
    }
}
