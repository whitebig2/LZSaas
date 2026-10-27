using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{

    /// <summary>
    /// 1.2.6. T0006-账户余额查询接口
    /// </summary>
    public class T0006ResultSetItem : ICzyResultSetItem
    {
        /// <summary>
        /// 账户号
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AcctName { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string Balance { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public string UsableBalance { get; set; }

        /// <summary>
        /// 冻结金额
        /// </summary>
        public string FreezeAmount { get; set; }

        /// <summary>
        /// 余额日期
        /// </summary>
        public string BalanceDate { get; set; }

        /// <summary>
        /// 币种编码
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 币种名称
        /// </summary>
        public string CurrencyName { get; set; }
    }


}




