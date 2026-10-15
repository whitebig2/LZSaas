using SqlSugar;
using System.Collections.Generic;

namespace LZ.Model
{
    [SugarTable("Live_BusInsuracneOrder")]
    public class Live_BusInsuracneOrderExtension : Live_BusInsuracneOrder
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_BusInsuracneOrderPlan.BusInsuranceOrderID))]
        public List<Live_BusInsuracneOrderPlan> Records { get; set; }
    }
}
