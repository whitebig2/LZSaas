using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_SalaryTransaction")]
    public class Live_SalaryTransactionExtension:Live_SalaryTransaction
    {
        /// <summary>
        /// 一对多
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Live_SalaryTransactionRecord.SalaryTransactionID))]
        public List<Live_SalaryTransactionRecord> Records { get; set; }
     
    }
}
