using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Insurance
{
    public enum InsuranceProcessStatusEnumType
    {
        待处理 = 1,
        待增员 = 2,
        已增员 = 3,
        待减员 = 4,
        已减员 = 5,
        增员撤销 = 6,
        作废 = 8,
    }
}
