using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_V_SalarySplitSend")]
    public class Live_V_SalarySplitSendExtension: Live_V_SalarySplitSend
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_SalarySplitSendRecord.SalarySplitSendID), nameof(Live_V_SalarySplitSend.ID))]
        public List<Live_SalarySplitSendRecord> Records { get; set; }
    }
}
