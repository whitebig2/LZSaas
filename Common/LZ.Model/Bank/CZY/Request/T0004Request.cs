using LZ.Model.Bank.CZY.BodyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.Request
{
    public class T0004Request 
    {
        public required CzyCommonBody<T0004ParamSetBodyData> Body { get; set; }
        public required CzyCommonHead Head { get; set; }

    }
}
