using LZ.Model.Enums;
using System;
using System.Collections.Generic;

namespace LZ.Model.Dtos.BankDo
{
    public class CzyPayOrderCreateDto
    {
        /// <summary>
        /// 应付单号
        /// </summary>
        public string BillsPayOrderNo { get; set; }
        /// <summary>
        /// 来源单号，如 PaymentOrder.ID
        /// </summary>
        public string RelBillsCode { get; set; }
        public CzyPayTypeEnumType PayType { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string BusinessTypeCode { get; set; }
        public string BusinessTypeName { get; set; }
        public string PayOrgCode { get; set; }
        public string PayOrgName { get; set; }
        public string PayAccountNo { get; set; }
        public CzyPayChannelEnumType PayChannel { get; set; }
        public decimal PayAmount { get; set; }
        public string PayCurrencyType { get; set; }
        public string PayCurrencyName { get; set; }
        public bool WhetherUrgent { get; set; }
        public string Remark { get; set; }
        public string PayUse { get; set; }
        public string Purpose { get; set; }
        public string CreateByCode { get; set; }
        public string CreateByName { get; set; }
        public string SerialNo { get; set; }
        public List<CzyPayOrderDetailCreateDto> Records { get; set; }

        public CzyPayOrderCreateDto()
        {
            Records = new List<CzyPayOrderDetailCreateDto>();
        }
    }

    public class CzyPayOrderDetailCreateDto
    {
        public string BillsPayOrderNo { get; set; }
        public decimal PrivateAmount { get; set; }
        public string ReceiveAccountNo { get; set; }
        public string ReceiveAccountName { get; set; }
        //public string ReceiveAreaNameOfProvince { get; set; }
        //public string ReceiveAreaNameOfCity { get; set; }
        //public string ReceiveBranchCode { get; set; }
        //public string ReceiveBranchName { get; set; }
        public string ReceiveNature { get; set; }
        //public string ReceiveCurrencyCode { get; set; }
        //public string ReceiveCurrencyName { get; set; }
        public string PayUse { get; set; }
        public string Purpose { get; set; }
    }
}
