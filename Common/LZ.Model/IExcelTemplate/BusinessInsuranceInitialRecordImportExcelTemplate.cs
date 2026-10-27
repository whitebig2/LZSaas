using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BusinessInsuranceInitialRecordImportExcelTemplate
    {
        [DisplayName("姓名(必填)")]
        public string Name { get; set; }
        [DisplayName("身份证号(必填)")]
        public string IdCardNo { get; set; }
        [DisplayName(displayName: "实际用工单位(必填)")]
        public string CustomerName { get; set; }
        [DisplayName(displayName: "商保劳动合同单位(必填)")]
        public string InsuranceSigningUnitName { get; set; }
        [DisplayName("雇佣工种(必填)")]
        public string WorkName { get; set; }
        [DisplayName("职业类别(必填)")]
        public string WorkClass { get; set; }
        [DisplayName("入职日期(必填)")]
        public DateTime EntryDate { get; set; }
        [DisplayName("保险公司(必填)")]
        public string InsuranceCompany { get; set; }
        [DisplayName("参保方案(必填)")]
        public string PlanName { get; set; }
        [DisplayName("订单编号(必填)")]
        public string InsuranceId { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }
    }
}
