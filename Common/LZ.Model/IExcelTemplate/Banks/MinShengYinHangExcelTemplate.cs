using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.Banks
{
    //public class MinShengYinHangExcelTemplate
    //{
      
    //    public List<MinShengYinHangItemExcelTemplate> Items { get; set; }

    //    /// <summary>
    //    /// 总金额
    //    /// </summary>
    //    [DisplayName("总金额")]
    //    public decimal TotalAmount => Items.Sum(f => f.TransactionAmount);
    //    /// <summary>
    //    /// 总笔数
    //    /// </summary>
    //    [DisplayName("总笔数")]
    //    public int TotalCount => Items.Count;
    //}


    public class MinShengYinHangExcelTemplate
    {
        /// <summary>
        /// 收款账户
        /// </summary>
        [DisplayName("收款账户")]
        public string BankNo { get; set; }
        /// <summary>
        /// 收款账户名称
        /// </summary>
        [DisplayName("收款账户名称")]
        public string BankUserName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [DisplayName("金额")]
        public decimal TransactionAmount { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        [DisplayName("用途")]
        public string Summary { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Remark { get; set; }
        [DisplayName("联行号")]
        public string BankNameNo { get; set; }

    }
}
