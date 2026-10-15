using LZ.Model.Enums;
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
    public partial class SalarySplitSendRecordExcelTemplate
    {
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("签章单位")]
        public string SigningUnitCompanyName { get; set; }
        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("工资所属月")]
        public string SalaryMonth { get; set; }
        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("个税累计月")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public string TaxMonth { get; set; }
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

        [DisplayName("所属行")]
        public string Bank { get; set; }
        [DisplayName("银行卡所属人")]
        public string BankUserName { get; set; }

        [DisplayName("银行卡号")]
        public string BankNo { get; set; }


        //[DisplayName("工资记录状态")]
        //public string SalaryRecordStatus { get; set; }
        [DisplayName("状态")]
        public SalarySplitSendRecordStatusEnumType Status { get; set; }
        [DisplayName("金额")]
        public decimal SplitAmount { get; set; }
        [DisplayName("发放时间")]
        public string SendTime { get; set; }
        [DisplayName("创建时间")]
        public string  CreateTime { get; set; }



    }
}
