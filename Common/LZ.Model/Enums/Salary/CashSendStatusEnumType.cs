using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum CashSendStatusEnumType
    {
        待处理 = 0,
        待生成签收单=1,
        签收单生成异常=2,
        待签署=3,
        签署失败=4,
        已签署=5,
        已存证=6,
        作废 = 7,
    }
}
