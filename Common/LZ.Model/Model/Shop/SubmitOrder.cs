using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class SubmitOrder
    {
        public Shop_Order Order { get; set; }

        public List<SubmitOrderCart> Items { get; set; }

        public string CouponIDS { get; set; }

        public string SkuID { get; set; }
        public int? ProductCount { get; set; }

        public bool IsIntegral { get; set; }
    }
}
