using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class EntryChangeModel
    {
        public Live_UserEntry? Entry { get; set; } = null;
        public EntryParas? Paras { get; set; } = null;
    }
    public class EntryParas
    {
        public string PartnerName { get; set; }
        public string PersonnelName { get; set; }
        public string CustomerName { get; set; }
        public string StructureName { get; set; }
}
}
