using SqlSugar;
using System.Collections.Generic;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_BusInsuranceApiPush")]
    public class Live_BusInsuranceApiPushExtension : Live_BusInsuranceApiPush
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_BusInsuranceApiPushDetail.BusInsuranceApiPushID))]
        public List<Live_BusInsuranceApiPushDetail> Details { get; set; }
    }
}
