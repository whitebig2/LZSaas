using LZ.Model.Bank.CZY.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BodyData
{
    public class T0014ResultSetBodyData : CommonResultSetBodyData
    {
        public string NextTag { get; set; }
        public string Total { get; set; }
        public List<T0014ResultSetItem> ResultSet { get; set; }
    }
}
