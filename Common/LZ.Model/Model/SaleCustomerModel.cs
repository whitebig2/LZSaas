using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Model
{
    public class SaleCustomerModel
    {
        public int CustomerID { get; set; }

        public int? PersonnelID { get; set; }
        public int EntryCount { get; set; }

        public int EntryTodayCount { get; set; }

        public int LeaveTodayCount { get; set; }

        public int? Status { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}
