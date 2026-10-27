using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    /// <summary>
    /// 部门类型
    /// </summary>
    public enum DepartmentType
    {
        [Description("分公司")]
        BranchCompany = 1,
        [Description("普通部门")]
        Normal = 2,
        [Description("服务商")]
        ServiceProvider = 3,
    }
}
