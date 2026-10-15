using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_BusinessInsurancePush")]
    public class Live_BusinessInsurancePushExtension : Live_BusinessInsurancePush
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_BusinessInsurancePushRecord.BusinessInsurancePushID))]
        public List<Live_BusinessInsurancePushRecord> Records { get; set; }
    }
}
