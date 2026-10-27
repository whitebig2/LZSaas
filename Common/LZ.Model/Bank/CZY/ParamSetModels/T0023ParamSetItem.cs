using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// 1.2.19. T0023-认领结果查询接口
    /// URL:/api/dtxg
    /// </summary>
    public class T0023ParamSetItem : ICzyParamSetItem
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public required string BeginDate { get; set; }
        /// <summary>
        /// 对账开始日期
        /// </summary>
        public required string EndDate { get; set; }
        /// <summary>
        /// 应收单财资云编号
        /// </summary>
        public string ReceivableBusinessNo { get; set; }
        /// <summary>
        /// 应收单外部系统编号
        /// </summary>
        public string ExtBusinessNo { get; set; }
        /// <summary>
        /// 查询下页标识，起始1
        /// </summary>
        public string NextTag { get; set; }

     
    }
 


}
