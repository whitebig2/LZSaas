using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_V_InsuranceOrder")]
    public class Live_V_InsuranceOrderExtension : Live_V_InsuranceOrder
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Live_InsuranceOrderPlan.InsuranceOrderID), nameof(Live_V_InsuranceOrder.ID))]
        public List<Live_InsuranceOrderPlan> OrderPlans { get; set; }
        /// <summary>
        /// 保全名单
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Live_InsuranceOderNameRecord.InsuranceOrderID), nameof(Live_V_InsuranceOrder.ID))]
        public List<Live_InsuranceOderNameRecord> NameRecords { get; set; }
    }
}
