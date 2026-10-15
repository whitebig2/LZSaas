using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// 1.2.16. T0019-余额对账信息查询接口
    /// URL:/api/dtxg
    /// </summary>
    public class T0019ParamSetItem : ICzyParamSetItem
    {
        /// <summary>
        /// 账号
        /// </summary>
        public required string AccNo { get; set; }
        /// <summary>
        /// 对账开始日期
        /// </summary>
        public required string StartDate { get; set; }
        /// <summary>
        /// 对账结束日期
        /// </summary>
        public required string EndDate { get; set; }

     
    }


}
