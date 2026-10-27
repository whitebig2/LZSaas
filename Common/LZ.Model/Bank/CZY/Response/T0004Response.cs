using LZ.Model.Bank.CZY.BodyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.Response
{
    public class T0004Response 
    {
        public CzyCommonBody<T0004ResultSetBodyData> Body { get; set; }

        public CzyCommonHead Head { get; set; }
    }
}
