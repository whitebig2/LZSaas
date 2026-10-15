using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Sale
{
    public enum SaleCustomerStatusEnum
    {
        未分配 = 0,
        分配成功 = 1,
        分配异常 = 2,
        非销售系统客户 = 3,
    }
}
