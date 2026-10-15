using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_BankTransaction")]
    public class Live_BankTransactionExtension : Live_BankTransaction
    {

      

        /// <summary>
        /// 一对多
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Live_BankTransactionRecord.BankTransactionID))]
        public List<Live_BankTransactionRecord> Records { get; set; }
     
    }
}
