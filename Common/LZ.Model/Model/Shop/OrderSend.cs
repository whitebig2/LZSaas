using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class OrderSend
    {
        public string OrderID { get; set; }
        public string ExpressNo { get; set; }

        public string ExpressType { get; set; }
        public string ExpressName { get; set; }
        public bool IsPackage { get; set; }

    }
}
