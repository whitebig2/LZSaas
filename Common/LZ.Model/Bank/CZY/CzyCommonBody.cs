using LZ.Model.Bank.CZY.BodyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY
{
    public class CzyCommonBody<T> 
    {
        public required T Data { get; set; }
        public required string Sign { get; set; }
    }
}
