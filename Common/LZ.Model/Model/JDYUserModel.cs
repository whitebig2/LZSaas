using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class JDYUserModel
    {
        public List<JDYUserItemModel> Users { get; set; }
        public List<JDYUserItemModel> member_list { get; set; }
    }
    public class JDYUserItemModel
    {
        public string username { get; set; }
        public string name { get; set; }
    }
}
