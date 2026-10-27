using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.busInsurance
{
    public class NewYangGuangExcelTemplate
    {
        [DisplayName("原雇员姓名*")]
        public string ExistName { get; set; }
        [DisplayName("原雇员证件类型（请输入证件类型例如01-身份证）*")]
        public string ExistCardType { get; set; }
        [DisplayName("原雇员证件号码*")]
        public string ExistCardID { get; set; }
        [DisplayName("批改标志*")]
        public string ChangeFlag { get; set; }
        [DisplayName("新雇员姓名*")]
        public string Name { get; set; }
        [DisplayName("新证件类型*")]
        public string CardType { get; set; }
        [DisplayName("新证件号码*")]
        public string CardID { get; set; }

        [DisplayName("年龄*")]
        public int Age { get; set; }
        [DisplayName("性别*")]
        public string Sex { get; set; }

        [DisplayName("身体状况")]
        public string HealthInfo { get; set; }
        [DisplayName("工作单位")]
        public string CustomerName { get; set; }
        [DisplayName("月工资币别")]
        public string SalaryCurrencyName { get; set; }
        [DisplayName("月工资金额")]
        public string MonthSalaryAmount { get; set; }
        [DisplayName("职业小类")]
        public string InsuranceWorkName { get; set; }
    }
}
