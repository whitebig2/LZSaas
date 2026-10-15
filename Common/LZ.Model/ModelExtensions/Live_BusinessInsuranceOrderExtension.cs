using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_BusinessInsuranceOrder")]
    public class Live_BusinessInsuranceOrderExtension: Live_BusinessInsuranceOrder
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_BusinessInsuranceOrderPlan.BusinessInsuranceOrderID))]
        public List<Live_BusinessInsuranceOrderPlan> Records { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(Live_BusinessInsuranceOrderEmpoyee.BusinessInsuranceOrderID))]
        public List<Live_BusinessInsuranceOrderEmpoyee> EmployeeRecords { get; set; }
    }
}
