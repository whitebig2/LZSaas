using LZ.Model.Bank.CZY.BodyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.Response
{
    public class T0005Response
    {
        public CzyCommonBody<T0005ResultSetBodyData> Body { get; set; }
        public CzyCommonHead Head { get; set; }
    }
}
