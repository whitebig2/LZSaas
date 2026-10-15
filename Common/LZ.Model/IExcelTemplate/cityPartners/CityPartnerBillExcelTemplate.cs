using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.cityPartners
{
    public class CityPartnerBillExcelTemplate
    {
        [DisplayName("城市合伙人名称")]
        public string CityPartnerTitle { get; set; }
        [DisplayName("账单月")]
        public DateTime? BillMonth { get; set; }
        [DisplayName("代理人")]
        public string CityPartnerAgentName { get; set; }

        /// <summary>
        /// 商保返利金额
        /// </summary>		
        [DisplayName("商保返利")]
        public decimal InsuranceRebateAmount { get; set; }

        /// <summary>
        /// 劳动关系财政奖励
        /// </summary>		
        [DisplayName("劳动关系财政奖励")]
        public decimal LaborRebateAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("外包财政奖励")]
        public decimal OutsourcingRebateAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("招聘业务提成")]
        public decimal SalesmanAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("劳动关系毛利提成")]
        public decimal LaborProfitAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("招聘业务毛利提成")]
        public decimal RecruitmentProfitAmount { get; set; }

    }

    public class CityPartnerEmpRecordExcelTemplate
    {
        [DisplayName("城市合伙人名称")]
        public string CityPartnerTitle { get; set; }
        [DisplayName("代理人")]
        public string CityPartnerAgentName { get; set; }
        [DisplayName("员工服务商")]
        public string PerPartnerName { get; set; }
        [DisplayName("客户单位名")]
        public string CustomerName { get; set; }
        [DisplayName("账单月")]
        public DateTime? RecordMonth { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }
        [DisplayName("入职日期")]
        public string EntryDate { get; set; }
        [DisplayName("离职日期")]
        public string LeaveDate { get; set; }
        [DisplayName("返利金额")]
        public decimal RebateAmount { get; set; }
    }
    public class CityPartnerSalaryRecordExcelTemplate
    {
        [DisplayName("城市合伙人名称")]
        public string CityPartnerTitle { get; set; }
        [DisplayName("员工服务商")]
        public string PerPartnerName { get; set; }
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string CardID { get; set; }
        [DisplayName("企业应发工资")]
        public decimal UnitSalaryAmount { get; set; }
        [DisplayName("服务费")]
        public decimal ServiceAmount { get; set; }
        [DisplayName("")]
        public DateTime? TaxMonth { get; set; }
        [DisplayName("账单月")]
        public DateTime? RecordMonth { get; set; }
        [DisplayName("入职日期")]
        public string EntryDate { get; set; }
        [DisplayName("离职日期")]
        public string LeaveDate { get; set; }
        [DisplayName("提成金额")]
        public decimal RebateAmount { get; set; }
		
        [DisplayName("业务员")]
        public string Salesman { get; set; }
        [DisplayName("业务类型")]
        public string BusinessModelName { get; set; }
    }

    public class CityPartnerSalesmanProfitExcelTemplate
    {
        [DisplayName("城市合伙人名称")]
        public string CityPartnerTitle { get; set; }
        [DisplayName("员工服务商")]
        public string PerPartnerName { get; set; }
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string CardID { get; set; }
        [DisplayName("企业应发工资")]
        public decimal UnitSalaryAmount { get; set; }
        [DisplayName("服务费")]
        public decimal ServiceAmount { get; set; }
     
        [DisplayName("毛利月")]
        public DateTime? ProfitMonth { get; set; }
        [DisplayName("入职日期")]
        public string EntryDate { get; set; }
        [DisplayName("离职日期")]
        public string LeaveDate { get; set; }
        [DisplayName("业务员")]
        public string Salesman { get; set; }
        [DisplayName("业务类型")]
        public decimal BusinessAmount { get; set; }
        [DisplayName("客户业务类型")]
        public string BusinessModelName { get; set; }
    }

}
