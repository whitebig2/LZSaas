using LZ.Model.Bank.CZY.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// 1.2.7. T0010-开票付款接口
    /// </summary>
    public class T0010ParamSetItem
    {
        /// <summary>
        /// 外部系统单据编号(附件关联字段)
        /// </summary>
        public required string BillsPayOrderNo { get; set; }

        /// <summary>
        /// 付款类型 16.票据开票
        /// </summary>
        public required string PayType { get; set; }

        /// <summary>
        /// 业务类型编号
        /// </summary>
        public string? BusinessTypeCode { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public required string BusinessTypeName { get; set; }

        /// <summary>
        /// 付款单位编号
        /// </summary>
        public string? PayOrgCode { get; set; }

        /// <summary>
        /// 付款单位名称
        /// </summary>
        public required string PayOrgName { get; set; }

        /// <summary>
        /// 付款方银行账号
        /// </summary>
        public required string PayAccountNo { get; set; }

        /// <summary>
        /// 付款方银行账户名称
        /// </summary>
        public required string PayAccountName { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public required string PayAmount { get; set; }

        /// <summary>
        /// 付款方开户行名称
        /// </summary>
        public required string PayBranchName { get; set; }

        /// <summary>
        /// 付款方开户行号
        /// </summary>
        public string? PayBranchCode { get; set; }

        /// <summary>
        /// 收票人信息
        /// </summary>
        public required T0010PaymentRec PaymentRec { get; set; }

        /// <summary>
        /// 承兑人信息
        /// </summary>
        public required T0010TakerWarrantor TakerWarrantor { get; set; }

        /// <summary>
        /// 票据信息
        /// </summary>
        public required T0010NoteApply NoteApply { get; set; }
    }



 


}
