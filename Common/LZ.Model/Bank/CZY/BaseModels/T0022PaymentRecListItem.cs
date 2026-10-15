using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BaseModels
{
    /// <summary>
    /// 收款账户明细实体类
    /// </summary>
    public class T0022PaymentRecListItem
    {
        /// <summary>
        /// 收款方单据编号
        /// </summary>
        public required string BillsPayOrderNo { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public required string PrivateAmount { get; set; }

        /// <summary>
        /// 收款方银行账号
        /// </summary>
        public required string ReceiveAccountNo { get; set; }

        /// <summary>
        /// 收款方账户名称
        /// </summary>
        public required string ReceiveAccountName { get; set; }

        /// <summary>
        /// 收款方开户行所属省(名称)
        /// </summary>
        public string? ReceiveAreaNameOfProvince { get; set; }

        /// <summary>
        /// 收款方开户行所属市(名称)
        /// </summary>
        public string? ReceiveAreaNameOfCity { get; set; }

        /// <summary>
        /// 收款方开户行行号
        /// </summary>
        public string? ReceiveBranchCode { get; set; }

        /// <summary>
        /// 收款方开户行名称
        /// </summary>
        public string? ReceiveBranchName { get; set; }

        /// <summary>
        /// 收款方性质 1.公司 2.个人 (为空，根据收款方名称来判断)
        /// </summary>
        public string? ReceiveNature { get; set; }

        /// <summary>
        /// 收款账户币种编号 (为空,默认CNY)
        /// </summary>
        public string? ReceiveCurrencyCode { get; set; }

        /// <summary>
        /// 收款账户币种名称 (为空,默认人民币)
        /// </summary>
        public string? ReceiveCurrencyName { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public required string PayUse { get; set; }

        /// <summary>
        /// 用途：不得有非法字符，包括 ' " : %
        /// </summary>
        public required string Purpose { get; set; }
    }
}
