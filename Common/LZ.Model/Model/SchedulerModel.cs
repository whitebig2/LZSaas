using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Model
{
    public class SchedulerModel
    {
        public string jobName { get; set; }
        public string jobGroup { get; set; }
        public string triggerName { get; set; }
        public string triggerGroup { get; set; }
        public string Cron { get; set; }
        public string statusName { get; set; }
    }
}
