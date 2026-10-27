using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum SignStatusEnumType
    {
        待生成 = 0,
        生成异常 = 1,
        待发送短信 = 2,
        短信发送失败 = 3,
        待签署 = 4,
        签署失败 = 5,
        待存储合同 = 6,
        存储合同失败 =7,
        待存证 = 8,
        存证失败 = 9,
        存证完成 = 10,
    }
}
