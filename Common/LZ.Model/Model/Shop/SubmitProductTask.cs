using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class SubmitProductTask
    {
        public Shop_ProductTask TaskModel { get; set; }

        public List<Shop_ProductTaskItems> Items { get; set; }

    }
}
