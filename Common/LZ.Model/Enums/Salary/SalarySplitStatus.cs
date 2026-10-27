using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum SalarySplitSendStatusEnumType
    {
        待处理 = 0,
        处理中 = 1,
        已处理 = 2,
    }
    public enum SalarySplitSendRecordStatusEnumType
    {
        待发放 = 0,
        已发放 = 1,
        发放失败 = 2,
        取消发放 = 3
    }
}
