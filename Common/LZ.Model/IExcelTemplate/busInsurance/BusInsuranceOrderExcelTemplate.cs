using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsuranceOrderExcelTemplate
    {
        [DisplayName("保险公司")]
        public string InsuranceCompany { get; set; }
        [DisplayName("订单编号")]
        public string OrderNo { get; set; }
        [DisplayName("保单号")]
        public string PolicyNum { get; set; }
        [DisplayName("参保方案")]
        public string PlanName { get; set; }
        [DisplayName("签章单位")]
        public string SignUnitNames { get; set; }

        [DisplayName("订单月份")]
        public string OrderMonth { get; set; }
        [DisplayName("开始日期")]
        public string StartDate { get; set; }
        [DisplayName("结束日期")]
        public string EndDate { get; set; }

        [DisplayName("是否启用")]
        public string IsEnable { get; set; }
    }
}
