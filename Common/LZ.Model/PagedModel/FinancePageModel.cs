using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.PagedModel
{
    public class FinancePageModel : PagedModelBase
    {
        public string PartnerName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        public decimal? Money { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? ProcessStatus { get; set; }
        public int? Type { get; set; }

        public string BillMonth { get; set; }

    }
}
