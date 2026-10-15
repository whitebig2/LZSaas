using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// T0001-交易流水信息查询接口
    /// URL:/api/dtxg
    /// </summary>
    public class T0001ParamSetItem: ICzyParamSetItem
    {
        /// <summary>
        /// 本方账号（必填）
        /// </summary>
        public required string AcctNo { get; set; }
        /// <summary>
        /// 交易币种编号（必填）
        /// </summary>
        public required string CurrencyCode { get; set; }
        /// <summary>
        /// 开始交易日期（必填）
        /// </summary>
        public required string StartTransDate { get; set; }
        /// <summary>
        /// 结束交易日期（必填）
        /// </summary>
        public required string EndTransDate { get; set; }
        /// <summary>
        /// 查询下页标识，起始1（必填）
        /// </summary>
        public required string NextTag { get; set; }

    }
}
