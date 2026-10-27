using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// 1.2.4. T0004-应付单业务状态查询接口
    /// </summary>
    public class T0004ParamSetItem : ICzyParamSetItem
    {
       
        /// <summary>
        /// 外部系统单据编号
        /// 长度 50
        /// </summary>
        public required string BillsPayOrderNo { get; set; }
        
    }


}
