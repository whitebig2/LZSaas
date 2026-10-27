using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Injury
{
    public enum InjuryProcessStatus
    {
        待添加 = 0,
        待审核 =1,
        审核失败 =2,
        待申请补助 =3,
        医疗期  = 4,
        恢复期 =5,
        工伤理赔期 =6,
        仲裁中 =7,
        工伤完成 =8,
        工伤撤回 =9,
        理赔待打款 = 10,
    }
}
