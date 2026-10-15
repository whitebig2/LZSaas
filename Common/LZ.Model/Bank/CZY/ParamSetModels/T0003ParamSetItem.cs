using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// 1.2.3. T0003-工资代发（批量对私）
    /// </summary>
    public class T0003ParamSetItem:ICzyParamSetItem
    {
        /// <summary>
        /// 外部系统单据流水号
        /// </summary>
        public string SerialNo { get; set; }
        /// <summary>
        /// 外部系统单据编号
        /// 长度 50
        /// </summary>
        public required string BillsPayOrderNo { get; set; }
        /// <summary>
        /// 付款类型 默认2.工资代发（批量对私）
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
        public string PayCurrencyName { get; set; }
        /// <summary>
        /// 是否加急 1.是 2.否(为空,默认: 2)
        /// </summary>
        public string WhetherUrgent { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public required string PayUse { get; set; }
        /// <summary>
        /// 对私用途，不得有非法字符，包括 ' " : %
        ///当付款方为工商银行账户时，对私用途需要遵循如下规范
        /// </summary>
        public required string Purpose { get; set; }
        /// <summary>
        /// 私收款账户（见以下列表信息）
        /// </summary>
        public required IEnumerable<T0003BatchPrivateRecItem> BatchPrivateRecList { get; set; }
    }

    public class T0003BatchPrivateRecItem
    {
        public required string BillsPayOrderNo { get; set; }
        public required string PrivateAmount { get; set; }
        public required string ReceiveAccountNo { get; set; }
        public required string ReceiveAccountName { get; set; }
        public string ReceiveAreaNameOfProvince { get; set; }
        public string ReceiveAreaNameOfCity { get; set; }
        public string ReceiveBranchCode { get; set; }
        public string ReceiveBranchName { get; set; }
        public string ReceiveNature { get; set; }
        public string ReceiveCurrencyCode { get; set; }
        public string ReceiveCurrencyName { get; set; }

    }
}
