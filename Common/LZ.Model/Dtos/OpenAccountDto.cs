using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{


    public record  OpenAccountDto
    {
        public required Guid BatchNo { get; set; }
        public required Guid BusinessID { get; set; }
        public required string Name { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Sex { get; set; }
        public required string CardType { get; set; }
        public required string CardID { get; set; }
        public required DateTime CardEndDate { get; set; }
        public required DateTime CardStartDate { get; set; }
        public required string Address { get; set; }
        public required string Phone { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerAddress { get; set; }
        public required string CardFrontPath { get; set; }
        public required string CardBackPath { get; set; }
    }
}
