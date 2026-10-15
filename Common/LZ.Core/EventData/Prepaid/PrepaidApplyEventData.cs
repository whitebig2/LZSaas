using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    public class PrepaidApplyEventData: EventBase
    {
        public int UserEntryID { get; set; }
        public decimal ApplyAmount { get; set; }
        public PrepaidApplyEnumType ApplyType { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public decimal ShiftHours { get; set; }
        public decimal ShiftMoney { get; set; }
        public decimal LunchHours { get; set; }
        public decimal NightShiftSubsidy { get; set; }
        public decimal DinnerHours { get; set; }
        public decimal MoneyHours { get; set; }
        public int WorkType { get; set; }
    }
}
