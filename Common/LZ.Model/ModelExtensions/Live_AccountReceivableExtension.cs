using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_AccountReceivable")]
    public class Live_AccountReceivableExtension:Live_AccountReceivable
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_AccountReceivableRecord.AccountReceivableID))]
        public List<Live_AccountReceivableRecord> Records { get; set; }
    }
}
