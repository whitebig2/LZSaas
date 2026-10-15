using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class VoteUserModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string LikeProduct { get; set; }
        public List<VoteItems> Items { get; set; }
    }
}
