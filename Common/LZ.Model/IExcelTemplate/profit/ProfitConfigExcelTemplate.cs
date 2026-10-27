using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.profit
{
    public class ProfitConfigExcelTemplate
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("保费计算方式")]
        public string AgentCountTypeName { get; set; }
        [DisplayName("保费计算方式")]
        public string BusInsuranceCountTypeName { get; set; }
        [DisplayName("商保费4类/人/天")]
        public decimal BusInsuranceAmount { get; set; }
        [DisplayName("商保费5类/人/天")]
        public decimal BusInsurance5Amount { get; set; }
        [DisplayName("保障金/人")]
        public decimal RiskAmount { get; set; }
        [DisplayName("业务开发比例%")]
        public decimal BusinessPercent { get; set; }
        [DisplayName("代缴税费比例%")]
        public decimal AgentPaidPercent { get; set; }
        [DisplayName("联营驻厂比例%")]
        public decimal JoinManagePercent { get; set; } 
        [DisplayName("客情费比例%")]
        public decimal CustomerThingPercent { get; set; } 
        [DisplayName("扣除金额")]
        public decimal DeductionAmount { get; set; }   
        [DisplayName("补贴金额")]
        public decimal SubsidyAmount { get; set; }  
        [DisplayName("预付商保费")]
        public decimal PreBusInsuranceTotalAmount { get; set; } 
        [DisplayName("社保补缴风险金基数")]
        public decimal SocialRepayRiskBaseAmount { get; set; }

        [DisplayName("疾病身故险")]
        public decimal DeathRiskAmount { get; set; }

        

        [DisplayName("启用")]
        public string IsEnabled { get; set; }
    }
}
