using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public class CashSignRecordDto
    {
        public int UserEntryID { get; set; }
        public int UserID { get; set; }
        public string CardID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }
        public int CompanyID { get; set; }
        public int PartnerID { get; set; }
        public int CustomerID { get; set; }
        public string CompanyName{ get; set; }
        public string CustomerName{ get; set; }
        public string SignUnitName { get; set; }
        public string SignUnitCompanyNo { get; set; }
        public string PartnerName { get; set; }
        public string CashType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public CashSignStatusEnumType Status { get; set; }
        public string FailMsg { get; set; }
    
    }
}
