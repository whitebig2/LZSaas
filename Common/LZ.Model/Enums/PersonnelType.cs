using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum PersonnelType
    {
        /// <summary>
        /// 人事
        /// </summary>
        [Description("人事")]
        Personnel = 1,

        /// <summary>
        /// 招聘专员
        /// </summary>
        [Description("招聘专员")]
        Platform = 2,
        /// <summary>
        /// 驻厂专员
        /// </summary>
        [Description("驻厂专员")]
        Factory = 3,
        /// <summary>
        /// 负责人
        /// </summary>
        [Description("负责人")]
        Headed = 4,

        /// <summary>
        /// 观察者
        /// </summary>
        [Description("观察者")]
        View = 5,
        /// <summary>
        /// 管理
        /// </summary>
        [Description("管理")]
        Manage = 6,
        /// <summary>
        /// 联营
        /// </summary>
        [Description("联营")]
        Joint = 7,
        /// <summary>
        /// 日结
        /// </summary>
        [Description("日结")]
        Daywork = 8,
        /// <summary>
        /// 日结管理
        /// </summary>
        [Description("日结管理")]
        DayworkManage = 9,

    }
}
