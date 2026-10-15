using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum JointAuditStatus
    {
        /// <summary>
        /// 未申请
        /// </summary>
        Not = 0,

        /// <summary>
        /// 审核中
        /// </summary>
        Audit = 1,

        /// <summary>
        /// 通过
        /// </summary>
        Pass = 2,
        /// <summary>
        /// 未通过
        /// </summary>
        Fail = 3,
       
    }
}
