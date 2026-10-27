using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.PagedModel
{
    public class PromotionPageTaskModel : PagedModelBase
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
}
