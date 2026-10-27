using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum InsuranceProcessEnumType
    {
        待匹配 = 0,
        匹配失败工种 = 1,
        匹配失败订单 = 2,
        待推送 = 3,
        已推送 = 4,
        非系统参保 = 5,
    }
}
