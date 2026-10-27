using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class SalaryRecordPrintExcelTemplate
    {
        [DisplayName(displayName: "签章单位")]
        public string SignUnitName { get; set; }
        [DisplayName(displayName: "客户单位")]
        public string CustomerName { get; set; }
      
        [DisplayName("工资所属月")]
        public string SalaryMonth { get; set; }
        [DisplayName("入职日期")]
        public string EntryDate { get; set; }
        [DisplayName("离职日期")]
        public string LeaveDate { get; set; }


        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("身份证号码")]
        public string IdCardNo { get; set; }
        [DisplayName("联系方式")]
        public string Phone { get; set; }
        [DisplayName("税前补贴")]
        public decimal SubsidyAmount { get; set; }

        [DisplayName("税后补贴")]
        public decimal SubsidyNoTaxAmount { get; set; }

        /// <summary>
        /// 税后扣除
        /// </summary>		
        [DisplayName("税后扣除")]
        public decimal DeductionNoTaxAmount { get; set; }

        [DisplayName(displayName: "应发工资合计")]
        public decimal UnitSalaryAmount { get; set; }

        // <summary>
        /// 个人养老保险
        /// </summary>		
        [DisplayName("个人养老保险")]
        public decimal SocialYangLaoAmount { get; set; }

        /// <summary>
        /// 个人医疗保险
        /// </summary>		
        [DisplayName("个人医疗保险")]
        public decimal SocialYiLiaoAmount { get; set; }

        /// <summary>
        /// 个人失业保险
        /// </summary>		
        [DisplayName("个人失业保险")]
        public decimal SocialShiYeAmount { get; set; }
        /// <summary>
        /// 个人住房公积金
        /// </summary>		
        [DisplayName("个人住房公积金")]
        public decimal SocialGongJiJinAmount { get; set; }
        /// <summary>
        /// 个人重大疾病险
        /// </summary>		
        [DisplayName("个人重大疾病险")]
        public decimal SocialDaBingAmount { get; set; }


        [DisplayName("个人所得税")]
        public decimal TaxMonthAmount { get; set; }



        [DisplayName(displayName: "预发工资")]
        public decimal PrepaidRepaymentAmount { get; set; }

        [DisplayName(displayName: "客服扣补金额")]
        public decimal ChangeAmount { get; set; }
        [DisplayName(displayName: "客服扣补备注")]
        public string ChangeRemark { get; set; }

        [DisplayName(displayName: "实发工资")]
        public decimal ActualSalarySendCardAmount { get; set; }

        [DisplayName(displayName: "银行卡所属人")]
        public string BankUserName { get; set; }

        [DisplayName(displayName: "银行卡所属行")]
        public string Bank { get; set; }

        [DisplayName(displayName: "银行卡号")]
        public string BankNo { get; set; }


      
       


       


 
      





   
    }
}
