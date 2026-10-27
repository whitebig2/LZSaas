using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    /// <summary>
    /// 1.2.11. T0014-金融网点查询接口
    /// URL:/api/dtxg
    /// </summary>
    public class T0014ResultSetItem : ICzyResultSetItem
    {
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnName { get; set; }

        /// <summary>
        /// 直连编号
        /// </summary>
        public string DirectCode { get; set; }

        /// <summary>
        /// 银行机构号
        /// </summary>
        public string BranchCode { get; set; }

        /// <summary>
        /// 联行号
        /// </summary>
        public string CnapsCode { get; set; }

        /// <summary>
        /// 银行科目编号
        /// </summary>
        public string SubjectCode { get; set; }

        /// <summary>
        /// 银行科目名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// 银行地址（省）
        /// </summary>
        public string ExtProvinceName { get; set; }

        /// <summary>
        /// 银行地址（市）
        /// </summary>
        public string ExtCityName { get; set; }

        /// <summary>
        /// 所属大行
        /// </summary>
        public string BelongBankTypeName { get; set; }
    }
}
