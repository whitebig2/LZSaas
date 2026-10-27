using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BodyData
{
    public class CommonResultSetBodyData : ICzyBodyData
    {
        public string ResultCode { get; set; }
        public string ResultMsg { get; set; }
    }
}
