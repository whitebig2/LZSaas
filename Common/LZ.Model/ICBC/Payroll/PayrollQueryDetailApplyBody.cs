using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    /// <summary>
    /// 合作方代发结果查询
    /// </summary>
    public class PayrollQueryDetailApplyBody: IPayrollApplyBody
    {
        public string BatchId { get; set; }
    }
}
