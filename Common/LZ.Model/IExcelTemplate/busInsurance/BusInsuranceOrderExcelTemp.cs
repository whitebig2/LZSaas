using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsuranceOrderExcelTemp
    {
        public string SignUnitName { get; set; }
        public string OrderNo { get; set; }
        public string PlanName { get; set; }
        public decimal PlanNamePrice { get; set; }
        public string InsuranceCompany { get; set; }
    }
}
