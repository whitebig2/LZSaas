using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum CashSignStatusEnumType
    {
        生成取消 = 0,
        待生成 = 1,
        生成异常 = 2,
        待发送短信 = 3,
        短信发送异常 = 4,
        待签署 = 5,
        签署异常 = 6,
        待存储 = 7,
        存储异常 = 8,
        待存证 = 9,
        存证失败 = 10,
        已存证 = 11,
    }
}
