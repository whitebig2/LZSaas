using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.profit
{
    public class ProfitManageExcelDto
    {
        [DisplayName("合伙人")]
        public string PartnerName { get; set; }
        [DisplayName("签章单位")]
        public string SigningUnitName { get; set; }
        [DisplayName("招聘渠道")]
        public string FromType { get; set; }
        [DisplayName("毛利月份")]
        public string ProfitMonth { get; set; }
        [DisplayName("工资发放月")]
        public string TaxMonth { get; set; }
        [DisplayName("服务费条数")]
        public int ServiceFeeRecordCount { get; set; }
        [DisplayName("在职人数")]
        public int InJobRecordCount { get; set; }
        [DisplayName("企业工资")]
        public decimal UnitSalaryTotalAmount { get; set; }
        [DisplayName("服务费")]
        public decimal ServiceFeeTotalAmount { get; set; }
        [DisplayName("企业社保")]
        public decimal UnitSocialTotalAmount { get; set; }
        [DisplayName("工伤风险金")]
        public decimal RiskTotalAmount { get; set; }
        [DisplayName("疾病身故险")]
        public decimal DeathRiskTotalAmount { get; set; }

        [DisplayName("社保补缴风险金")]
        public decimal SocialRepayRiskAmount { get; set; }
        [DisplayName("保费实收")]
        public decimal BusInsuranceSumAmount { get; set; }
        [DisplayName("残保金")]
        public decimal DisabilityTotalAmount { get; set; }
        [DisplayName("代缴费")]
        public decimal AgentPaidTotalAmount { get; set; }
        [DisplayName("代缴税费")]
        public decimal AgentPaidTaxTotalAmount { get; set; }
        [DisplayName("新业务开发费")]
        public decimal BusinessTotalAmount { get; set; }
        [DisplayName("客情费")]
        public decimal CustomerThingTotalAmount { get; set; }
        [DisplayName("驻厂管理费（收）")]
        public decimal JoinManageIncomeAmount { get; set; }
        [DisplayName("驻厂管理费（支）")]
        public decimal JoinManagePaidAmount { get; set; }
        [DisplayName("毛利结算")]
        public decimal ProfitTotalAmount { get; set; }
        [DisplayName("扣除金额")]
        public decimal DeductionTotalAmount { get; set; }
        [DisplayName("补贴金额")]
        public decimal SubsidyTotalAmount { get; set; }
        [DisplayName("扣除描述")]
        public string DeductionRemark { get; set; }
        [DisplayName("补贴描述")]
        public string SubsidyRemark { get; set; }
    }
}
