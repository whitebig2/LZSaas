using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    /// <summary>
    /// 合作方代发申请
    /// </summary>
    public class PayrollSendApplyBody : IPayrollApplyBody
    {
        /// <summary>
        /// 代发账户账号（必填）
        /// </summary>
        public string DraweeAccNo { get; set; }
        /// <summary>
        /// 代发账户户名（必填）
        /// </summary>
        public string DraweeAccName { get; set; }
        /// <summary>
        /// 代理地区号（必填）
        /// </summary>
        public string ZoneNo { get; set; }
        /// <summary>
        /// 代理网点号（必填）
        /// </summary>
        public string Brno { get; set; }
        /// <summary>
        /// 合作方代发文件名称（必填）
        /// </summary>
        public string FileName { get; set; }
    }




    
}
