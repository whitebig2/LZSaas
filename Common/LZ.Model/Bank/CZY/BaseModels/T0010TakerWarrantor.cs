using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BaseModels
{
    public class T0010TakerWarrantor
    {
        /// <summary>
        /// 承兑人账号
        /// </summary>
        public required string AccountNo { get; set; }

        /// <summary>
        /// 承兑人账户名称
        /// </summary>
        public required string AccountName { get; set; }

        /// <summary>
        /// 承兑人开户行号
        /// </summary>
        public string? BranchCode { get; set; }

        /// <summary>
        /// 承兑人开户行名称
        /// </summary>
        public required string BranchName { get; set; }
    }
}
