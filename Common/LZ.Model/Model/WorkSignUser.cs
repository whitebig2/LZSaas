using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class WorkSignUser
    {
        public bool IsDayOver { get; set; }
        public bool IsDaySign { get; set; }
        public bool IsNight { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CompanyName { get; set; }
        public string DayStartTime { get; set; }
        public string NightStartTime { get; set; }
        public string DayEndTime { get; set; }
        public string NightEndTime { get; set; }
        public string DayOverStartTime { get; set; }
        public string DayOverEndTime { get; set; }
        public int ID { get; set; }
    }
}
