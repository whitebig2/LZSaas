using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.OpenAccountDtos
{
    public class PreOpenAccountRecordDto
    {

        public Guid? ID { get; set; }
        public required int UserID { get; set; }
        public required int UserEntryID { get; set; }
        public required string Name { get; set; }
        public required string CardID { get; set; }
        public required DateTime? CardStartDate { get; set; }
        public  DateTime? CardEndDate { get; set; }
        public  string Sex { get; set; }
        public required string Phone { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Address { get; set; }
        public required string CustomerName { get; set; }
        public required string CardFrontPath { get; set; }
        public required string CardBackPath { get; set; }
    }
}
