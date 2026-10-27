using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SqlSugar.SugarTable("Live_V_PrepaidApplyRecord")]
    public class Live_V_PrepaidApplyRecordExtension : Live_V_PrepaidApplyRecord
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_PrepaidRepaymentRecord.PrepaidApplyRecordID),nameof(ID))]
        public List<Live_PrepaidRepaymentRecord> Records { get; set; }
    }
}
