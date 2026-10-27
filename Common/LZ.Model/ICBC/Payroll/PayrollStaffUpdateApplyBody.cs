using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    public class PayrollStaffUpdateApplyBody: IPayrollApplyBody
    {

        /// <summary>
        /// 员工号（必填）
        /// </summary>
        public string StaffId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 工作单位
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string WorkPosition { get; set; }
        /// <summary>
        /// 薪资等级
        /// </summary>
        public string WagesLevel { get; set; }
        /// <summary>
        /// 就职状态
        /// 1-在职 2-离职
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }

    }
}
