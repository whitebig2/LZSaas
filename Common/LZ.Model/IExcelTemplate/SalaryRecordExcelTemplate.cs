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
    public partial class SalaryRecordExcelTemplate
    {
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [DisplayName("身份证号码")]
        public string IdCardNo { get; set; }
        ///// <summary>
        ///// 公司名称
        ///// </summary>
        //[DisplayName("公司名称")]
        //public string CompanyName { get; set; }

        /// <summary>
        /// 车间(非必填项)
        /// </summary>
        [DisplayName("车间(非必填项)")]
        public string Workshop { get; set; }=string.Empty;

        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("工资日期")]
        [DisplayFormat(DataFormatString ="yyyy-MM-dd")]
        public DateTime SalaryMonth { get; set; }
        /// <summary>
        /// 服务费
        /// </summary>
        [DisplayName("服务费")]
        public decimal ServiceFeeAmount { get; set; }
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
        /// <summary>
        /// 税前补贴
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
        /// 是否暂缓发放
        /// </summary>		
        [DisplayName("是否暂缓发放")]
        public bool IsDelayPayment { get; set; }

        /// <summary>
        /// 个人医疗保险
        /// </summary>		
        [DisplayName("个人医疗保险")]
        public decimal SocialYiLiaoAmount { get; set; }

        /// <summary>
        /// 个人养老保险
        /// </summary>		
        [DisplayName("个人养老保险")]
        public decimal SocialYangLaoAmount { get; set; }

        /// <summary>
        /// 个人失业保险
        /// </summary>		
        [DisplayName("个人失业保险")]
        public decimal SocialShiYeAmount { get; set; }

        /// <summary>
        /// 个人重大疾病险
        /// </summary>		
        [DisplayName("个人重大疾病险")]
        public decimal SocialDaBingAmount { get; set; }
        /// <summary>
        /// 个人住房公积金
        /// </summary>		
        [DisplayName("个人住房公积金")]
        public decimal SocialGongJiJinAmount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>		
        [DisplayName("备注")]
        public string Remark { get; set; }


        /// <summary>
        /// 内部结算社保企业部分
        /// </summary>		
        [DisplayName("内部结算社保企业部分")]
        public decimal SocialUnitSumAmount { get; set; }
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

        /// <summary>
        /// 部门(平台专用)
        /// </summary>		
        [DisplayName("部门(平台专用)")]
        public string PlatformDepartment { get; set; }
        /// <summary>
        /// 银行卡所属行
        /// </summary>		
        [DisplayName("银行卡所属行")]
        public string Bank { get; set; }
        /// <summary>
        /// 银行卡卡号
        /// </summary>		
        [DisplayName("银行卡卡号")]
        public string BankNo { get; set; }
        /// <summary>
        /// 银行卡卡号
        /// </summary>		
        [DisplayName("银行卡所属人")]
        public string BankUserName { get; set; }
        public int UserEntryID { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>		
        [DisplayName("入职日期")]
        public DateTime EntryDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>		
        [DisplayName("离职日期")]
        public DateTime? LeaveDate { get; set; }
        [DisplayName("年终奖发放年份(平台专用)")]
        public int SalaryYear { get; set; }
        public bool IsCashSend { get; set; }
    }
}
