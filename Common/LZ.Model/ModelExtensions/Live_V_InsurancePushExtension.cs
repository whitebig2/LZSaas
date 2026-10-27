using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_V_InsurancePush")]
    public class Live_V_InsurancePushExtension : Live_V_InsurancePush
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_InsurancePushRecord.InsurancePushID), nameof(Live_V_InsurancePushExtension.ID))]
        public List<Live_InsurancePushRecord> PushRecords { get; set; }
    }
}
