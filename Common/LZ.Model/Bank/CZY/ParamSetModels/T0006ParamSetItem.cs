using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{

    /// <summary>
    /// 1.2.6. T0006-账户余额查询接口
    /// </summary>
    public class T0006ParamSetItem : ICzyParamSetItem
    {
        /// <summary>
        /// 账户号
        /// </summary>
        public string AcctNo { get; set; }
        /// <summary>
        /// 账户币种
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 余额开始日期
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 余额结束日期
        /// </summary>
        public string EndDate { get; set; }
    }


}




