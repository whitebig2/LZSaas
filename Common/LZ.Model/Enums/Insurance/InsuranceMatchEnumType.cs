using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum InsuranceMatchEnumType
    {
        待匹配 = 0,
        身份证号匹配 = 1,
        同类匹配 = 2,
        异类匹配 = 3,
        无匹配项=4
    }
}
