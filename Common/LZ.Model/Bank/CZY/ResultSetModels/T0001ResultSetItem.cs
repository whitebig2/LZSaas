using LZ.Model.Bank.CZY.BaseModels;
using LZ.Model.Enums;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    /// <summary>
    /// T0001-交易流水信息查询接口
    /// URL:/api/dtxg
    /// </summary>
    public class T0001ResultSetItem: ICzyResultSetItem
    {

        /// <summary>
        /// 本方账户号码
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 本方账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 本方账户开户行
        /// </summary>
        public string BranchName { get; set; }


        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TransInfoNo { get; set; }

        /// <summary>
        /// 银行类别
        /// </summary>
        public string BankType { get; set; }

        /// <summary>
        /// 币种编码
        /// </summary>
        public string CurrencyType { get; set; }


        /// <summary>
        /// 交易金额
        /// </summary>
        public string TransAmount { get; set; }

        /// <summary>
        /// 交易后余额
        /// </summary>
        public string TransBalance { get; set; }


        /// <summary>
        /// 收支方向 1付(借)，2收(贷)
        /// </summary>
        public string TransDirection { get; set; }

        /// <summary>
        /// 对方账号
        /// </summary>
        public string OppAccountNo { get; set; }


        /// <summary>
        /// 对方账户名
        /// </summary>
        public string OppAccountName { get; set; }

        /// <summary>
        /// 对方开户行
        /// </summary>
        public string OppBankName { get; set; }

        /// <summary>
        /// 对方开户行地址
        /// </summary>
        public string OppBankAddr { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public string TransTime { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string UseInfo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 交易抹账标识 1.被抹账 2.正常
        /// </summary>
        public string DeleteByBank { get; set; }

        /// <summary>
        /// 交关联应付单据信息
        /// </summary>
        public IEnumerable<PaymentBill> PaymentBills { get; set; }
    }

   
}
