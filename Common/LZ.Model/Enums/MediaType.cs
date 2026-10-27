using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum MediaType
    {
        
        /// <summary>
        /// 其他
        /// </summary>
        Other = 0,
        /// <summary>
        /// 扫码填写简介
        /// </summary>
        Input = 1,

        /// <summary>
        /// 扫码注册
        /// </summary>
        Reg = 2,
        /// <summary>
        /// 扫码推荐
        /// </summary>
        Recommend = 3,
        /// <summary>
        /// 客服封面
        /// </summary>
        Service = 4
    }
}
