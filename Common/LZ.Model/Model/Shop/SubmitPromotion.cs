using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class SubmitPromotion
    {
        public Shop_Promotion Promotion { get; set; }

        public List<Shop_PromotionProduct> Items { get; set; }
    }
}
