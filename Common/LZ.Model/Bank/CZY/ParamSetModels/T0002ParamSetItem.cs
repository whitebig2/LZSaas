using LZ.Model.Bank.CZY.BaseModels;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// T0002-单笔付款接口
    /// api/dtxg
    /// </summary>
    public class T0002ParamSetItem:ICzyParamSetItem
    {
        /// <summary>
        /// 外部系统单据编号
        /// 长度：50
        /// </summary>
        public required string BillsPayOrderNo { get; set; }
        /// <summary>
        /// 付款类型 默认1.单笔付款，表示一对一（对公/对私）支付
        /// </summary>
        public required string PayType { get; set; }
        /// <summary>
        /// 付款日期(期望支付日期)
        /// </summary>
        public required string PaymentDate { get; set; }
        /// <summary>
        /// 业务类型编号
        /// </summary>
        public string BusinessTypeCode { get; set; }
        /// <summary>
        /// 业务类型名称
        /// </summary>
        public required string BusinessTypeName { get; set; }
        /// <summary>
        /// 付款单位编号
        /// </summary>
        public string PayOrgCode { get; set; }
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
        /// 付款金额
        /// </summary>
        public required string PayAmount { get; set; }
        /// <summary>
        /// 币种(为空,默认: CNY 有付款账号时，不能为空)
        /// </summary>
        public string PayCurrencyType { get; set; }
        /// <summary>
        /// 币种名称(为空,默认: 人民币, 有付款账号时，不能为空)
        /// </summary>
        public required string PayCurrencyName { get; set; }
        /// <summary>
        /// 是否加急 1.是 2.否(为空,默认: 2)
        /// </summary>
        public string WhetherUrgent { get; set; }
        /// <summary>
        /// 附言
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public required string PayUse { get; set; }
        /// <summary>
        /// 用途，不得有非法字符，包括 ' " : %当付款方为工商银行账户时，对私用途需要遵循如下规范
        /// </summary>
        public required string Purpose { get; set; }
        /// <summary>
        /// 收款方信息（见以下子对象说明）
        /// </summary>
        public required PaymentRec PaymentRec { get; set; }
    }


}
