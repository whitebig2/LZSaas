using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum UserTaskStatus
    {
        /// <summary>
        /// 进行中
        /// </summary>
        Oning = 1,
        /// <summary>
        /// 取消任务
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// 等待兑换
        /// </summary>
        Exchange = 3,
        /// <summary>
        /// 已完成
        /// </summary>
        Finish = 4,
        /// <summary>
        /// 超时失效
        /// </summary>
        Timeout = 5
    }
}
