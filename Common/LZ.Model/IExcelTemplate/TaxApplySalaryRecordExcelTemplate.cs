using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class TaxApplySalaryRecordExcelTemplate
    {
        [DisplayName("签章单位")]
        public string SignUnitName { get; set; }
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("入职时间")]
        public string EntryDate { get; set; }
        [DisplayName("离职时间")]
        public string LeaveDate { get; set; }
        [DisplayName("工资批次")]
        public string SalaryBatchType { get; set; }
        [DisplayName("个税累计月")]
        public string TaxMonth { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("性别")] 
        public string Sex { get; set; }
        [DisplayName("手机号码")] 
        public string Phone { get; set; }
        [DisplayName("工资发放状态")] 
        public string StatusName { get; set; }
        [DisplayName("*证照类型")] 
        public string CardType => "身份证";
        [DisplayName("*证照号码")] 
        public string CardID { get; set; }
        [DisplayName("*本期收入")] 
        public decimal UnitSalaryAmount { get; set; }
        [DisplayName("本期免税收入")] 
        public decimal NoTaxSalaryAmount { get; set; }
        [DisplayName("基本养老保险费")] 
        public decimal YangLaoAmount { get; set; }
        [DisplayName("基本医疗保险费")] 
        public decimal YiLiaoAmount { get; set; }
        [DisplayName("失业保险费")] 
        public decimal ShiYeAmount { get; set; }
        [DisplayName("重大疾病险费")]
        public decimal DaBingAmount { get; set; }
        [DisplayName("住房公积金")] 
        public decimal GongJiJinAmount { get; set; }
        [DisplayName("累计收入额")]
        public decimal BeforeTaxSalaryTotalAmount { get; set; }
        [DisplayName("累计减除费用")]
        public decimal SpBasicTotalAmount { get; set; }
        [DisplayName("子女教育")]
        public decimal TaxSpZiNvTotalAmount { get; set; }
        [DisplayName("赡养老人")]
        public decimal TaxSpShanYangTotalAmount { get; set; }
        [DisplayName("住房贷款利息")]
        public decimal TaxSpDaiKuanTotalAmount { get; set; }
        [DisplayName("住房租金")]
        public decimal TaxSpZuJinTotalAmount { get; set; }
        [DisplayName("继续教育")]
        public decimal TaxSpJiaoYuTotalAmount { get; set; }
        [DisplayName("3岁以下婴幼儿照护")]
        public decimal TaxSpYingErTotalAmount { get; set; }
        [DisplayName("应纳税所得额")]
        public decimal TaxSalaryTotalAmount { get; set; }
        [DisplayName("应纳税额")]
        public decimal TaxTotalAmount { get; set; }
        [DisplayName("已缴纳税额")]
        public decimal LastTaxTotalAmount { get; set; }
        [DisplayName("应纳税额")]
        public decimal MonthTaxAmount { get; set; }
        [DisplayName("工资发放日期")]
        public string SendDate { get; set; }
    }
}
