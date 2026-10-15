using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum EntryModel
    {
        /// <summary>
        /// 正常入职
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 审核后直接入职
        /// </summary>
        AuditDirectEntry = 2,
        /// <summary>
        /// 直接入职
        /// </summary>
        DirectEntry = 3,
        
    }
}
