using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class CustomerServiceChargeExcelTemplate
    {
        /// <summary>
        /// 收票公司名称
        /// </summary>
        [DisplayName("收票公司名称")]
        public string ReceiptCustomerName { get; set; }
        /// <summary>
        /// 客户公司名称
        /// </summary>
        [DisplayName("客户公司名称")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 开票公司名称
        /// </summary>
        [DisplayName("开票公司名称")]
        public string InvoiceCustomerName { get; set; }
        /// <summary>
        /// 工资所属月
        /// </summary>
        [DisplayName("工资所属月")]
        public string SalaryMonth { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        [DisplayName("开票日期")]
        public string InvoiceDate { get; set; }
        /// <summary>
        /// 工资发放时间
        /// </summary>
        [DisplayName("工资发放时间")]
        public string SendDate { get; set; }
        /// <summary>
        /// 开票金额
        /// </summary>
        [DisplayName("开票金额")]
        public decimal? InvoiceAmount { get; set; }
        /// <summary>
        /// 工资增值税额
        /// </summary>
        [DisplayName("工资增值税额")]
        public decimal? SalaryVAT { get; set; }
        /// <summary>
        /// 工资附加税额
        /// </summary>
        [DisplayName("工资附加税额")]
        public decimal? SalarySurtax { get; set; }
        /// <summary>
        /// 服务费
        /// </summary>
        [DisplayName("服务费")]
        public decimal? ServiceCharge { get; set; }
        /// <summary>
        /// 服务费增值税额
        /// </summary>
        [DisplayName("服务费增值税额")]
        public decimal? ServiceVAT { get; set; }
        /// <summary>
        /// 服务费附加税额
        /// </summary>
        [DisplayName("服务费附加税额")]
        public decimal? ServiceSurtax { get; set; }
        /// <summary>
        /// 社保公积金
        /// </summary>
        [DisplayName("社保公积金")]
        public decimal? SocialAmount { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        [DisplayName("人数")]
        public int? PeopleCount { get; set; }
        /// <summary>
        /// 合计金额
        /// </summary>
        [DisplayName("合计金额")]
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// 是否预估
        /// </summary>
        [DisplayName("是否预估")]
        public string IsPredict { get; set; }
        
    }
}
