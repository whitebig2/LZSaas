using LZ.Model.InsuranceApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Budget
{
    public class RecordMonthModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public bool Exist { get; set; }
        public bool IsTimeCheck { get; set; }
        public int? PartnerID { get; set; }
        public Guid? PartnerGID { get; set; }
        public string PartnerName { get; set; }
    }
}
