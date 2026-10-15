using LZ.Model.Enums;
using System;
using System.Collections.Generic;

namespace LZ.Model.Dtos.FinanceDo
{
    /// <summary>
    /// 支付订单新增
    /// </summary>
    public class PaymentOrderAddDto
    {
        /// <summary>
        /// 公司编号，创建时由当前用户写入
        /// </summary>
        public int CompanyID { get; set; }
        /// <summary>
        /// 签约单位名称，用于匹配默认付款账户
        /// </summary>
        public string SigningUnitName { get; set; }
        /// <summary>
        /// 指定付款账户；未指定时按签约单位名称匹配
        /// </summary>
        public Guid? PaymentAccountID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        /// <summary>
        /// 业务单号
        /// </summary>
        public string BusinessID { get; set; }
        public int SalaryType { get; set; }
        /// <summary>
        /// TransactionDataTypeEnumType
        /// </summary>
        public TransactionDataTypeEnumType DataType { get; set; }
        public string PayUse { get; set; }
        public string Purpose { get; set; }
        public string Remark { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string BillsPayOrderNo { get; set; }
        public List<PaymentOrderDetailAddDto> Records { get; set; }
    }

    /// <summary>
    /// 支付订单明细
    /// </summary>
    public class PaymentOrderDetailAddDto
    {
        public string BusinessID { get; set; }
        public string Name { get; set; }
        public string CardID { get; set; }
        public string BankName { get; set; }
        public string BankNo { get; set; }
        public decimal TransAmount { get; set; }
        public string BillsPayOrderNo { get; set; }
    }
}
