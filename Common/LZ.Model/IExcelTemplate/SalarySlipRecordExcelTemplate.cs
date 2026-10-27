using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class SalarySlipRecordExcelTemplate
    {
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("工资所属月")]
        public string SalaryMonth { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号码")]
        public string IdCardNo { get; set; }
    }
}
