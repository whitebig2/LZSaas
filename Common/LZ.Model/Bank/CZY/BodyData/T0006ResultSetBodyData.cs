using LZ.Model.Bank.CZY.BusinessModels;
using LZ.Model.Bank.CZY.ParamSetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BodyData
{
    public class T0006ResultSetBodyData : CommonResultSetBodyData
    {
        public string NextTag { get; set; }
        public ICollection<T0006ResultSetItem> ResultSet { get; set; }

    }
}
