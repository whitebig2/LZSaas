using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.profit
{
    public class ProfitManageRecordExcelDto
    {
        [DisplayName("毛利月")]
        public string ProfitMonth { get; set; }
        [DisplayName(displayName: "工资发放月")]
        public string TaxMonth { get; set; }
        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("合伙人")]
        public string PartnerNamer { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("渠道")]
        public string FromType { get; set; }

        [DisplayName("签章单位")]
        public string SigningUnitName { get; set; }


        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("身份证号")]
        public string IdCardNo { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("企业工资")]
        public decimal UnitSalaryTotalAmount { get; set; }




        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("服务费")]
        public decimal ServiceFeeTotalAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("社保")]
        public decimal UnitSocialTotalAmount { get; set; }



        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("岗位类别")]
        public string WorkClass { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("工种")]
        public string WorkName { get; set; }
        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("参保方案")]
        public string PlanName1 { get; set; }

      

        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("入职日期")]
        public string EntryDate { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("离职日期")]
        public string LeaveDate { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("是否在职")]
        public string IsInJob { get; set; }
        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("工伤风险金")]
        public decimal RiskCurrentAmount { get; set; }

        [DisplayName("疾病身故险")]
        public decimal DeathRiskCurrentAmount { get; set; }

        [DisplayName("社保补缴风险金")]
        public decimal SocialRepayRiskAmount { get; set; }


        [DisplayName("客情费")]
        public decimal CustomerThingAmount { get; set; }
        [DisplayName("联营驻场管理费（支）")]
        public decimal JoinManageAmount { get; set; }


        ///// <summary>
        ///// 
        ///// </summary>		
        //[DisplayName("保费应收")]
        //public decimal BusInsuranceCurrentAmount { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>		
        //[DisplayName("保费退补")]
        //public decimal BusInsuranceBalanceAmount { get; set; }


        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("当月保费批改(旧)")]
        public decimal BusInsuranceChangeAmount { get; set; }


        [DisplayName("上月已预收")]
        public decimal BusInsuranceLastAmount { get; set; }


        [DisplayName("当月产生保费")]
        public decimal BusInsuranceCurrentAmount { get; set; }


        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("次月保费预收")]
        public decimal BusInsuranceNextAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("保费实收")]
        public decimal BusInsuranceSumAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("残保金")]
        public decimal DisabilityAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("代缴费")]
        public decimal AgentPaidAmount { get; set; }
        [DisplayName("代缴税费")]
        public decimal AgentPaidTaxAmount { get; set; }
        [DisplayName("新业务开发费")]
        public decimal BusinessAmount { get; set; }
        [DisplayName("毛利金额")]
        public decimal ProfitAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("结算周期天数")]
        public int Days { get; set; }
        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("结算在职天数")]
        public int InJobDays { get; set; }
        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("结算周期起")]
        public string StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       [DisplayName("结算周期止")]
        public string EndDate { get; set; }



        [DisplayName("保费按天计算")]
        public string IsDayMatch { get; set; }
    }


}
