using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class SalaryInvoiceExcelTemplate
    {
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public required string CustomerName { get; set; }
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("商务合作单位")]
        public required string CooperationUnitCompanyName { get; set; }
        /// <summary>
        /// 工资所属月
        /// </summary>
        [DisplayName("工资所属月")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime SalaryMonth { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public required string Name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [DisplayName("身份证号码")]
        public required string IdCardNo { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        [DisplayName("入职日期")]
        public DateTime? EntryDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        [DisplayName("离职日期")]
        public DateTime? LeaveDate { get; set; }


        [DisplayName("企业应发工资")]
        public decimal UnitSalaryAmount { get; set; }

        [DisplayName("企业扣款")]
        public decimal UnitDeductionAmount { get; set; }
        /// <summary>
        /// 服务费
        /// </summary>
        [DisplayName("服务费")]
        public decimal ServiceFeeAmount { get; set; }


        /// <summary>
        /// 合计金额
        /// </summary>
        [DisplayName("合计金额")]
        public decimal TotalAmount => UnitSalaryAmount+ServiceFeeAmount-UnitDeductionAmount;

        /// <summary>
        /// 备注
        /// </summary>		
        [DisplayName("备注")]
        public required string Remark { get; set; }


    }
}
