using Azure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class SalaryCustomerExcelTemplate
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("批次")]
        public string SalaryBatchType { get; set; }
        [DisplayName("工资类型")]
        public string SalaryType { get; set; }
        [DisplayName("状态")]
        public string Status { get; set; }
        [DisplayName("锁定状态")]
        public string IsLocked { get; set; }
        [DisplayName("工资所属月")]
        public string SalaryMonth { get; set; }
        [DisplayName("个税累计月")]
        public string TaxMonth { get; set; }
        [DisplayName("实发工资合计")]
        public decimal ActualSalarySumAmount { get; set; }
        [DisplayName("内部结算")]
        public decimal UnitSettlementAmount { get; set; }
        [DisplayName("允许民生代发")]
        public string IsCanCmbcSend { get; set; }
        [DisplayName("民生代发")]
        public string IsCmbcSend { get; set; }
        [DisplayName("代发结果")]
        public string IsCmbcSendFail { get; set; }
        [DisplayName("年终奖年份")]
        public int SalaryYear { get; set; }
        [DisplayName("发放时间")]
        public string SendTime { get; set; }
        [DisplayName("创建时间")]
        public string CreateTime { get; set; }

    }
}
