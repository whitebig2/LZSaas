using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum PrepaidRecordStatusEnumType
    {
        待结算=0,
        待审核 = 1,
        待发放 = 2,
        已发放 = 3,
        审核退回 = 4,
        作废 = 5,
        发放失败 = 6,
  
    }
}
