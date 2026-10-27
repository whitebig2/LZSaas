using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.busInsurance
{
    public class FiveOtherExcelTemplate
    {
      
        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("身份证号码")]
        public string CardID { get; set; }

        [DisplayName("被派遣单位")]
        public string CustomerName { get; set; }

        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("投保主体")]
        public string SignUnitName { get; set; }

        [DisplayName("生效日期")]
        public string StartDate { get; set; }
    }
}
