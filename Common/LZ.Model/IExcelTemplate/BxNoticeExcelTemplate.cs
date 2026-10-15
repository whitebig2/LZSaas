using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BxNoticeExcelTemplate
    {

        [DisplayName("保险公司")]
        public string InsuranceCompany { get; set; }

        [DisplayName("签章单位(商保)")]
        public string SignUnitName { get; set; }

        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }
        [DisplayName("工作类型")]
        public string WorkClass { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }

        [DisplayName("在职状态")]
        public string StatusName { get; set; }

        [DisplayName("入职日期")]
        public string EntryDate { get; set; }

        [DisplayName("保险状态")]
        public string PlanStatusName { get; set; }
        [DisplayName("手参员工")]
        public string IsManuallyInsurance { get; set; }

    }
}
