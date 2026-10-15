using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum BusinessModel
    {
        /// <summary>
        /// 劳务派遣
        /// </summary>
        Labor = 1,
        /// <summary>
        /// 劳务外包
        /// </summary>
        Epiboly = 2,
        /// <summary>
        /// 劳动关系-外包
        /// </summary>
        Agent = 3,
        /// <summary>
        /// 代缴商保
        /// </summary>
        Payment = 4,
        /// <summary>
        /// 代缴商保风险金
        /// </summary>
        Rrisk = 5,
        /// <summary>
        /// 代缴商保
        /// </summary>
        Social = 6,
        /// <summary>
        /// 内部业务
        /// </summary>
        Internal = 7,
        /// <summary>
        /// 代发业务
        /// </summary>
        Strike = 8,
        /// <summary>
        /// 劳动关系-派遣
        /// </summary>
        AgentLabor = 9,

    }
}
