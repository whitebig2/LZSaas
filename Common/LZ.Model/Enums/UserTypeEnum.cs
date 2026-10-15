using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    [Flags]
    public enum UserTypeEnum
    {

        /// <summary>
        /// 员工
        /// </summary>
        [Description("员工")]
        User = 1,

        /// <summary>
        /// 核销员
        /// </summary>
        [Description("核销员")]
        WriteUser = 2,

        /// <summary>
        /// 推销员
        /// </summary>
        [Description("推销员")]
        PromoterUser = 4
    }
}
