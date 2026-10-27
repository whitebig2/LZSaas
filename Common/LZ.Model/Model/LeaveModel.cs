using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class LeaveModel
    {
        public List<Live_UserLeaveReason>? leave { get; set; } = null;
        public Live_UserEntry? entry { get; set; } = null;
    }
}
