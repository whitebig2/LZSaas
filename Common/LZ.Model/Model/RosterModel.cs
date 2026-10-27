using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class RosterModel
    {
        public string Name { get; set; }
        public int EntryCount { get; set; }
        public int LeaveCount { get; set; }
        public List<Live_V_UserEntry> Items { get; set; }
        public int ID { get; set; }
        public Guid GID { get; set; }
    }
}
