using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class EmpInsuranceNamesExcelTemplate
    {
        [DisplayName("序号")]
        public string No { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("证件类型")]
        public string  IdCardType { get; set; }
        [DisplayName("证件号")]
        public string IdCardNo { get; set; }
        [DisplayName("雇员工种")]
        public string WorkName { get; set; }
        [DisplayName("实际用工单位")]
        public string CustomerName { get; set; }
        [DisplayName("劳动合同单位")]
        public string InsuranceSigningUnitName { get; set; }
        [DisplayName("职业类别")]
        public string WorkClass { get; set; }
        [DisplayName("工作地区（省份）")]
        public string WorkProvince { get; set; }
        [DisplayName("工作地区（城市）")]
        public string WorkCity { get; set; }
        [DisplayName("订单编号")]
        public string InsuranceId { get; set; }

        [DisplayName("保险公司")]
        public string InsuranceCompany { get; set; }
        [DisplayName("参保方案")]
        public string PlanName { get; set; }
        [DisplayName("是否在职" )]
        public bool IsOnJob{ get; set; }
        [DisplayName("是否包含")]
        public bool IsContain { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }


  
    }
}
