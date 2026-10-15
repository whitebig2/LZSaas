using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class SubmitOrderReturn
    {
        public Shop_OrderReturn Order { get; set; }

        public List<Shop_OrderReturnItems> Items { get; set; }

    }
}
