using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class SubmitCoupon
    {
        public Shop_Coupon Coupon { get; set; }

        public List<Shop_CouponProduct> Products { get; set; }
    }
}
