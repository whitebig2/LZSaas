using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// 1.2.11. T0014-金融网点查询接口
    /// URL:/api/dtxg
    /// </summary>
    public class T0014ParamSetItem : ICzyParamSetItem
    {
        /// <summary>
        /// 直连编号
        /// </summary>
        public  string DirectCode { get; set; }
        /// <summary>
        /// 交联行号
        /// </summary>
        public string CnapsCode { get; set; }
        /// <summary>
        /// 大行名称，支持查询全称或简称
        /// </summary>
        public required string BelongBankTypeName { get; set; }
        /// <summary>
        /// 分页标识，必须是正数，从1开始，每页返回500条数据，响应为0表示没有下一页，
        /// </summary>
        public required string NextTag { get; set; }

    }
}
