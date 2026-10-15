using LZ.Model.Bank.CZY.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BodyData
{
    public class T0001ResultSetBodyData: CommonResultSetBodyData
    {
        public string TotalNum { get; set; }
        public string NextTag { get; set; }
        public List<T0001ResultSetItem> ResultSet { get; set; }
    }
}
