using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ParamSetModels
{
    /// <summary>
    /// 1.2.10. T0013-电子回单文件获取接口（已废弃）
    /// </summary>
    public class T0013ParamSetItem:ICzyParamSetItem
    {

        /// <summary>
        /// 交易类型：A-全部、C-来账、D-往账、N-内部往来、W-外部交易
        /// </summary>
        public string? TransType { get; set; }

        /// <summary>
        /// 查询类型：1-通用型单笔 2-通用型汇总
        /// </summary>
        public string? QueryType { get; set; }

        /// <summary>
        /// 查询账号
        /// </summary>
        public required string AcctNo { get; set; }

        /// <summary>
        /// 币种，默认值“CNY”
        /// </summary>
        public required string CurrencyCode { get; set; }

        /// <summary>
        /// 开始日期，格式yyyyMMdd
        /// </summary>
        public required string BeginDate { get; set; }

        /// <summary>
        /// 截止日期，格式yyyyMMdd
        /// </summary>
        public required string EndDate { get; set; }

        /// <summary>
        /// 分页标识，为空或0默认第一次查询，响应中如果返回下一次分页标识，响应为空则没有下一页
        /// </summary>
        public string? NextTag { get; set; }
    }
}
