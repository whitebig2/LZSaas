using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_SalarySlip")]
    public class Live_SalarySlipExtension: Live_SalarySlip
    {

        [Navigate(NavigateType.OneToMany, nameof(Live_SalarySlipRecord.SalarySlipID))]
        public List<Live_SalarySlipRecord> Records { get; set; }
    }
}
