using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    /// <summary>
    /// 导入意外身故记录
    /// </summary>
    public class DeathInsuranceExcelTemplate
    {
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证")]
        public string CardID { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("保障开始时间")]
        public string StartDate { get; set; }
        [DisplayName("保障结束时间")]
        public string EndDate { get; set; }
        [DisplayName("用工单位")]
        public string SignUnitName { get; set; }
        [DisplayName("状态")]
        public string Status { get; set; }

    }
}
