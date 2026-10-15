using LZ.Model.Bank.CZY.BodyData;
using LZ.Model.Bank.CZY.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.Response
{
    public class T0022Response
    {
        public CzyCommonBody<T0022ResultSetBodyData> Body { get; set; }

        public CzyCommonHead Head { get; set; }
    }
}
