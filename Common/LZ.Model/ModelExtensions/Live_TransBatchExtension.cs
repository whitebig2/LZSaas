using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_TransBatch")]
    public class Live_TransBatchExtension: Live_TransBatch
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_TransBatchRecord.TransBatchID))]
        public List<Live_TransBatchRecord> Records { get; set; }
    }
}
