using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BaseModels
{
    public class T0010PaymentRec
    {
        /// <summary>
        /// 收票人账号
        /// </summary>
        public required string ReceiveAccountNo { get; set; }

        /// <summary>
        /// 收票人账户名称
        /// </summary>
        public required string ReceiveAccountName { get; set; }

        /// <summary>
        /// 收票人开户行号
        /// </summary>
        public string? ReceiveBranchCode { get; set; }

        /// <summary>
        /// 收票人开户行名称
        /// </summary>
        public required string ReceiveBranchName { get; set; }
    }
}
