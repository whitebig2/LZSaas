using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsurancePushFailExcelTemplate
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("签章单位")]
        public string SignUnitName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string CardID { get; set; }
        [DisplayName("工种类别")]
        public string WorkClass { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("保险公司")]
        public string InsuranceCompany { get; set; }
        [DisplayName("保险公司工种")]
        public string InsuranceWorkName { get; set; }
        [DisplayName("方案")]
        public string PlanName { get; set; }
        [DisplayName("订单号")]
        public string OrderNo { get; set; }
        [DisplayName("异常信息")]
        public string FailMsg { get; set; }
        [DisplayName("操作类型")]
        public string PushActionName { get; set; }
    }
}
