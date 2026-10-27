using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsuranceRecordExcelTemplate
    {
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }
        [DisplayName("年龄")]
        public int Age { get; set; }
        [DisplayName("雇佣工种")]
        public string WorkName { get; set; }
        [DisplayName("实际用工单位")]
        public string CustomerName { get; set; }
        [DisplayName("劳动关系所属公司")]
        public string InsuranceSigningUnitName { get; set; }
        [DisplayName("职业类别")]
        public string WorkClass { get; set; }
        [DisplayName("保障方案")]
        public string PlanName { get; set; }
        [DisplayName("起保日期")]
        public string StartDate { get; set; }
        [DisplayName("终保日期")]
        public string EndDate { get; set; }
        [DisplayName(displayName: "订单编号")]
        public string InsuranceId { get; set; }
        [DisplayName(displayName: "保险公司")]
        public string InsuranceCompany { get; set; }
        [DisplayName(displayName: "状态")]
        public string StatusName { get; set; }
        [DisplayName(displayName: "创建时间")]
        public string CreateTime { get; set; }
    }
}
