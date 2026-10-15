using LZ.Model.Enums;
using System;
using System.Collections.Generic;

namespace LZ.Model.Dtos.FinanceDo
{
    /// <summary>
    /// 预先生成支付订单（不落库，供前端展示）
    /// </summary>
    public class PrePaymentOrderDto
    {
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public string SignUnitName { get; set; }
        public string SignUnitNo { get; set; }
        public Guid PaymentSignUnitID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string BusinessID { get; set; }
        public int SalaryType { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string PayUse { get; set; }
        public string Purpose { get; set; }
        public string Remark { get; set; }
        public TransactionDataTypeEnumType DataType { get; set; }

        public CzyPayTypeEnumType PayType { get; set; }
        public CzyPayChannelEnumType PayChannel { get; set; }
        public bool WhetherUrgent { get; set; }
        public DateTime? PaymentDate { get; set; }
        public Guid? PaymentAccountID { get; set; }
        public string PayAccountNo { get; set; }
        public string PayOrgName { get; set; }
        public string PayOrgCode { get; set; }
        public string BusinessTypeName { get; set; }
        public string AppName { get; set; }
        public string AppID { get; set; }
        public Guid PaymentApplicationID { get; set; }
        public List<PrePaymentOrderDetailDto> Records { get; set; }
    }

    public class PrePaymentOrderDetailDto
    {
        public string BusinessID { get; set; }
        public string Name { get; set; }
        public string CardID { get; set; }
        public string BankName { get; set; }
        public string BankNo { get; set; }
        public decimal TransAmount { get; set; }
    }
}
