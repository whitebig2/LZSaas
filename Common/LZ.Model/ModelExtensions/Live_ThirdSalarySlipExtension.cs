using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_ThirdSalarySlip")]
    public class Live_ThirdSalarySlipExtension : Live_ThirdSalarySlip
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_ThirdSalarySlipRecord.ThirdSalarySlipID))]
        public List<Live_ThirdSalarySlipRecord> Records { get; set; }
    }
}
