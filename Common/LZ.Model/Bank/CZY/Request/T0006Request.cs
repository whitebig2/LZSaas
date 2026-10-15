using LZ.Model.Bank.CZY.BodyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.Request
{
    public class T0006Request 
    {
        public required CzyCommonBody<T0006ParamSetBodyData> Body { get; set; }
        public required CzyCommonHead Head { get; set; }

    }
}
