using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public class InsuranceConfigTempDto
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("保险公司")]
        public string InsuranceCompany { get; set; }
        [DisplayName("参保方案")]
        public string PlanName { get; set; }
    }
}
