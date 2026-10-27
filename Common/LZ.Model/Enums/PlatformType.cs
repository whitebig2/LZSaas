using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum PlatformType
    {
        /// <summary>
        /// 传0获取所有
        /// </summary>
        [Description("所有")]
        All = 0,

        /// <summary>
        /// 超管平台
        /// </summary>
        [Description("总后台")]
        Admin = 1,

        /// <summary>
        /// 平台后台
        /// </summary>
        [Description("商家后台")]
        MiniAdmin = 2,

        /// <summary>
        /// 幸福职微信小程序
        /// </summary>
        [Description("员工端小程序")]
        XFZMini = 3,

        /// <summary>
        /// 蓝招招聘微信小程序
        /// </summary>
        [Description("驻厂端小程序")]
        LZMini = 4,
        /// <summary>
        /// 劳务微信小程序
        /// </summary>
        [Description("劳务")]
        LWMini = 5,
        /// <summary>
        /// api
        /// </summary>
        [Description("API")]
        Api = 6,
    }
}
