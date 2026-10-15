using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum BusInsuranceApiPushStatusEnumType
    {
        待推送=0,
        已推送待处理=1,
        已推送待确认=2,
        已确认 =3,
        作废 =4,
    }

    public enum BusInsuranceApiPushDetailStatusEnumType
    {
        待核对 = 0,
        核对失败 = 1,
        已核对 = 2,
    }
}
