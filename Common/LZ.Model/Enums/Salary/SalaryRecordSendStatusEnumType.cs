using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum SalaryRecordSendStatusEnumType
    {
        不发放 = 0,
        待处理 = 1,
        处理中 = 2,
        待发放 = 3,
        发放中 = 4,
        已发放 = 5,
        发放异常 = 6,
        审核未通过 = 7,
        代发推送异常 = 8,
    }
}
