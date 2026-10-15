using LZ.Model.Bank.CZY.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{



/// <summary>
/// 批量支付请求实体类
/// </summary>
public class T0022ParamSetItem : ICzyParamSetItem
    {
        /// <summary>
        /// 外部系统单据编号
        /// </summary>
        public required string BillsPayOrderNo { get; set; }

        /// <summary>
        /// 付款日期(期望支付日期)
        /// </summary>
        public required string PaymentDate { get; set; }

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
        /// 支付方式 1.直联支付 2.线下支付 3.全额支付 4.差额支付
        /// </summary>
        public required string PayChannel { get; set; }

        /// <summary>
        /// 合计付款金额
        /// </summary>
        public required string SumPayAmount { get; set; }

        /// <summary>
        /// 币种(为空,默认: CNY 有付款账号时，不能为空)
        /// </summary>
        public string? PayCurrencyType { get; set; }

        /// <summary>
        /// 币种名称(为空,默认: 人民币, 有付款账号时，不能为空)
        /// </summary>
        public string? PayCurrencyName { get; set; }

        /// <summary>
        /// 是否加急 1.是 2.否(为空,默认: 2)
        /// </summary>
        public required string WhetherUrgent { get; set; }

        /// <summary>
        /// 附言
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public required string Note { get; set; }

        /// <summary>
        /// 收款账户列表
        /// </summary>
        public required List<T0022PaymentRecListItem> PaymentRecList { get; set; }
    }

 

}
