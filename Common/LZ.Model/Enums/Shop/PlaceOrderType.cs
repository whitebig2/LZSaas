using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum PlaceOrderType
    {
        /// <summary>
        /// 正常下单
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 福利兑换
        /// </summary>
        Welfare = 2,
        /// <summary>
        /// 任务兑换
        /// </summary>
        Task = 3,
        /// <summary>
        /// 转盘兑换
        /// </summary>
        Turntable = 4
    }
}
