using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.OpenAccountDtos
{
    public record AddOpenAccountRecordDto
    {
        public required int UserEntryID { get; set; }
        public required int UserID { get; set; }
        public required string Name { get; set; }
        public required string CardID { get; set; }
        public required DateTime CardStartDate { get; set; }
        public required DateTime CardEndDate { get; set; }
        public required string Sex { get; set; }
        public required string Phone { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Address { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerNo { get; set; }
        public required int CompanyID { get; set; }
        public required string CompanyName { get; set; }
        public required string CardFrontPath { get; set; }
        public required string CardBackPath { get; set; }
        public required int BankType { get; set; }
        public required string CompanyAddress { get; set; }
    }
}
