using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_InsuranceOrder")]
    public class Live_InsuranceOrderExtension:Live_InsuranceOrder
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_InsuranceOrderPlan.InsuranceOrderID))]
        public List<Live_InsuranceOrderPlan> OrderPlans { get; set; }

        /// <summary>
        /// 保全名单
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Live_InsuranceOderNameRecord.InsuranceOrderID))]
        public List<Live_InsuranceOderNameRecord> NameRecords { get; set; }
    }
}
