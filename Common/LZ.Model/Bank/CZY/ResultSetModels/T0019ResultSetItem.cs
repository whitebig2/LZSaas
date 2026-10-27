using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    /// <summary>
    /// 1.2.16. T0019-余额对账信息查询接口
    /// URL:/api/dtxg
    /// </summary>
    public class T0019ResultSetItem : ICzyResultSetItem
    {
        /// <summary>
        /// 防伪码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 对帐单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 存款种类（帐户种类）：1、往来户, 2、贷款户, 3、定期户, 4、保证金户, 5、内部户, 6、个人贷款综合账户, 9、其他
        /// </summary>
        public string AcctType { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string CurrType { get; set; }

        /// <summary>
        /// 余额：单位分
        /// </summary>
        public string Balance { get; set; }

        /// <summary>
        /// 定期存款存期
        /// </summary>
        public string FixDate { get; set; }

        /// <summary>
        /// 到期日 (格式: yyyyMMdd)
        /// </summary>
        public string DeadDate { get; set; }

        /// <summary>
        /// 存单号码
        /// </summary>
        public string DepositNo { get; set; }

        /// <summary>
        /// 对帐情况：0默认, 1平, 2不平, 3未对账
        /// </summary>
        public string CheckFlag { get; set; }

        /// <summary>
        /// 明细对账结果：1-企业已收银行未收 2-企业已付银行未付 3-银行已收企业未收(未达) 4-银行已收企业未收(疑义) 5-银行已付企业未付(未达) 6-银行已付企业未付(疑义)
        /// </summary>
        public string CheckResult { get; set; }

        /// <summary>
        /// 余额调节表编制情况：1: 已编制 2: 未编制
        /// </summary>
        public string BalanceFlag { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string UserRemark { get; set; }

        /// <summary>
        /// 借据号
        /// </summary>
        public string CreditNo { get; set; }

        /// <summary>
        /// 本金，单位：分
        /// </summary>
        public string Corpus { get; set; }

        /// <summary>
        /// 欠息，单位：分
        /// </summary>
        public string BackInterest { get; set; }

        /// <summary>
        /// 贷款到息日 (格式: yyyyMMdd)
        /// </summary>
        public string DebitDate { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string AccNo { get; set; }

        /// <summary>
        /// 户名
        /// </summary>
        public string AccName { get; set; }

        /// <summary>
        /// 借贷标志：1借 2贷
        /// </summary>
        public string Drcrf { get; set; }

        /// <summary>
        /// 对账单生成日期 (格式: yyyyMMdd)
        /// </summary>
        public string DzgenDate { get; set; }


    }


}
