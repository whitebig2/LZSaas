using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Response
{
    public class CashSignResponse
    {
        public string KeyID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public string CardID { get; set; }
        public string CustomerName { get; set; }
        public string SignUnitName { get; set; }
        public string CashBase64 { get; set; }
        public string CashPath { get; set; }
        public string StatusName { get; set; }
        public string CashType { get; set; }
    }
}
