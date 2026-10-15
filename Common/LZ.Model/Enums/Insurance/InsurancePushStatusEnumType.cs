using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum InsurancePushStatusEnumType
    {
        待核对 = 0,
        待推送 = 1,
        已推送 = 2,
        推送失败 = 3,
        推送异常 = 4,
        取消推送 = 5,
    }
}
