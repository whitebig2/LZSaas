using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Invoice
{
    public enum InvoiceOrderDetailTypeEnum
    {
        不含税 = 1,
        含税 = 2
    }
    public enum InvoiceStatusEnum
    {
        待开具 = 1,
        已开具 = 2,
        已冲红 = 3,
        已作废 = 4,
        开具失败 = 5,
        冲红失败 =6,
        开具中 = 7,
        冲红中 = 8,
        红单确认失败 = 9,
    }
}
