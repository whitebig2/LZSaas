using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum BusinessInsuranceStatusEnumType
    {
        待增员匹配 = 0,
        待增员 = 1,
        已增员 = 2,
        待减员匹配 = 3,
        待减员 = 4,
        已减员 = 5,
        增员失败= 6,
        减员失败=7,
        作废=8,
        
    }
}
