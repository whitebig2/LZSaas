using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum ContractStatus
    {
        /// <summary>
        /// 待生成入职合同
        /// </summary>
        [Description("待生成入职合同")]
        EntryWaitCreate = 1,
        /// <summary>
        /// 已生成入职合同
        /// </summary>
        [Description("已生成入职合同")]
        EntryCreate = 2,
        /// <summary>
        /// 已生成入职合同待网签
        /// </summary>
        [Description("已生成入职合同待网签")]
        EntryWaitSign = 3,
        /// <summary>
        /// 已完成入职合同
        /// </summary>
        [Description("已完成入职合同")] 
        EntryComplete = 4,
        /// <summary>
        /// 生成入职合同错误
        /// </summary>
        [Description("生成入职合同错误")] 
        EntryCreateError = 5,
        /// <summary>
        /// 待生成离职合同
        /// </summary>
        [Description("待生成离职合同")] 
        LeaveWaitCreate = 6,
        /// <summary>
        /// 已生成离职合同
        /// </summary>
        [Description("已生成离职合同")] 
        LeaveCreate = 7,
        /// <summary>
        /// 已生成离职合同待网签
        /// </summary>
        [Description("已生成离职合同待网签")] 
        LeaveWaitSign = 8,
        /// <summary>
        /// 已完成离职合同
        /// </summary>
        [Description("已完成离职合同")] 
        LeaveComplete = 9,
        /// <summary>
        /// 生成离职合同错误
        /// </summary>
        [Description("生成离职合同错误")] 
        LeaveCreateError = 10
    }
}
