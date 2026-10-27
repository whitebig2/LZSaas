using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class ZhongAnInsuranceExcelTemp
    {
        [DisplayName("雇主单位")]
        public string SignUnitName { get; set; }
        [DisplayName("用工单位")]
        public string CustomerName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证")]
        public string CardID { get; set; }
        [DisplayName("职业类别")]
        public string WorkClass { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("加保生效日期")]
        public string StartDate { get; set; }
        [DisplayName("方案")]
        public string PlanName { get; set; }
    }
}
