using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.profit
{
    public class ProfitManageDayMathRecordExcelDto
    {

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
        [DisplayName("当月保费批改")]
        public decimal BusInsuranceChangeAmount { get; set; }
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
