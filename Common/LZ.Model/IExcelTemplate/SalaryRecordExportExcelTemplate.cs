using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ExcelTemplateModel
{
    public partial class SalaryRecordExportExcelTemplate
    {
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("批次")]
        public string SalaryBatchType { get; set; }

        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 年度累计次数
        /// </summary>
        [DisplayName("年度累计次数")]
        public int TaxTimes { get; set; }
        [DisplayName("工资所属月")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public string SalaryMonth { get; set; }
        [DisplayName("入职日期")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public string EntryDate { get; set; }
        [DisplayName("离职日期")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public string LeaveDate { get; set; }
    
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号码")]
        public string IdCardNo { get; set; }

        [DisplayName("联系方式")]
        public string Phone { get; set; }
        [DisplayName("招聘渠道")]
        public string EntryChannel { get; set; }
        [DisplayName("是否现金发放")]
        public string IsCashSend { get; set; }
        [DisplayName("是否暂缓发放")]
        public string IsDelayPayment { get; set; }
        /// <summary>
        /// 企业应发工资
        /// </summary>		
        [DisplayName("企业应发工资")]
        public decimal UnitSalaryAmount { get; set; }
        /// <summary>
        /// 企业扣款
        /// </summary>		
        [DisplayName("企业扣款")]
        public decimal UnitDeductionAmount { get; set; }

        [DisplayName("实收企业工资合计")]
        public decimal CompanySalarySumAmount { get; set; }
        /// <summary>
        /// 其它补贴
        /// </summary>		
        [DisplayName("税前补贴")]
        public decimal SubsidyAmount { get; set; }

        /// <summary>
        /// 税后补贴
        /// </summary>		
        [DisplayName("税后补贴")]
        public decimal SubsidyNoTaxAmount { get; set; }

        /// <summary>
        /// 税后扣除
        /// </summary>		
        [DisplayName("税后扣除")]
        public decimal DeductionNoTaxAmount { get; set; }
        /// <summary>
        /// 提成(平台专用)
        /// </summary>		
        [DisplayName("提成(平台专用)")]
        public decimal PlatformCommissionAmount { get; set; }

        /// <summary>
        /// 福利(平台专用)
        /// </summary>		
        [DisplayName("福利(平台专用)")]
        public decimal PlatformWelfareAmount { get; set; }


        [DisplayName("应发工资合计")]
        public decimal BeforeTaxSalaryAmount { get; set; }



        /// <summary>
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
        [DisplayName("预发工资")]
        public decimal PrepaidRepaymentAmount { get; set; }
        [DisplayName("客服扣补金额")]
        public decimal ChangeAmount { get; set; }
        [DisplayName("客服扣补备注")]
        public string ChangeRemark { get; set; }

        [DisplayName("实发工资")]
        public decimal ActualSalarySendCardAmount { get; set; }
        [DisplayName("子女教育")]
        public decimal SpZiNvAmount { get; set; }
        [DisplayName("个人教育")]
        public decimal SpJiaoYuAmount { get; set; }
        [DisplayName("住房租金")]
        public decimal SpZuJinAmount { get; set; }
        [DisplayName("住房贷款")]
        public decimal SpDaiKuanAmount { get; set; }
        [DisplayName("赡养老人")]
        public decimal SpShanYangAmount { get; set; }
        [DisplayName("婴幼儿照护")]
        public decimal SpYingErAmount { get; set; }
        [DisplayName("银行卡所属人")]
        public string BankUserName { get; set; }
        [DisplayName("银行卡所属行")]
        public string Bank { get; set; }
        [DisplayName("银行卡号")]
        public string BankNo { get; set; }
        [DisplayName("累计收入")]
        public decimal BeforeTaxSalaryTotalAmount { get; set; }
        [DisplayName("社保个人部分合计")]
        public decimal SocialEmpSumAmount { get; set; }
          
        [DisplayName("累计专项扣除")]
        public decimal TaxSocialTotalAmount { get; set; }

        [DisplayName("累计专项附加扣除")]
        public decimal TaxSpTotalAmount { get; set; }
        [DisplayName("累计减除费用")]
        public decimal TaxSpBasicTotalAmount { get; set; }

        
    [DisplayName("累计应纳税所得额")]
        public decimal TaxSalaryTotalAmount { get; set; }
        [DisplayName("累计应缴")]
        public decimal TaxTotalAmount { get; set; }

        [DisplayName("暂缓发放")]
        public decimal DelaySendAmount { get; set; }

        [DisplayName("累计已预缴")]
        public decimal LastTaxTotalAmount { get; set; }



        [DisplayName("备注")]
        public string Remark { get; set; }

        [DisplayName("商务合作单位")]
        public string CooperationUnitCompanyName { get; set; }


        [DisplayName("实发工资单位")]
        public string SigningUnitCompanyName { get; set; }

        [DisplayName("社保企业部分")]
        public decimal SocialUnitSumAmount { get; set; }

        [DisplayName("管理费")]
        public decimal UnitManageAmount { get; set; }
        [DisplayName("合计金额")]
        public decimal CompanyTotalAmount { get; set; }

    }
}
