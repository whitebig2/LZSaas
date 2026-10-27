using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_BusinessInsuranceConfig")]
    public class Live_BusinessInsuranceConfigExtension: Live_BusinessInsuranceConfig
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_BusinessInsuranceWorkNamePlan.BusinessInsuranceConfigID))]
        public List<Live_BusinessInsuranceWorkNamePlan> WorkNamePlans { get; set; }
       
    }
}
