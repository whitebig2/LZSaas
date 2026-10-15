using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsuranceRecordImportExcelTemplate
    {
        [DisplayName("姓名(必填)")]
        public string Name { get; set; }
        [DisplayName("身份证号(必填)")]
        public string IdCardNo { get; set; }
        [DisplayName("雇佣工种(必填)")]
        public string WorkName { get; set; }
        [DisplayName("职业类别(必填)")]
        public string WorkClass { get; set; }
        [DisplayName("实际用工单位(必填)")]
        public string CustomerName { get; set; }
        [DisplayName("入职日期(必填)")]
        public string EntryDate { get; set; }
        [DisplayName("离职日期(选填)")]
        public string LeaveDate { get; set; }
        [DisplayName(displayName: "订单编号(必填))")]
        public string InsuranceId { get; set; }
        [DisplayName(displayName: "保险公司(必填)")]
        public string InsuranceCompany { get; set; }
        [DisplayName(displayName: "状态(必填)")]
        public string StatusName { get; set; }
   
    }
}
