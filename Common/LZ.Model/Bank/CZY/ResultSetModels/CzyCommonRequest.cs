using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.ResultSetModels
{
    public class CzyCommonRequest<T>
    {
        public CzyCommonHead Head { get; set; }
        public T Body { get; set; }
    }
}
