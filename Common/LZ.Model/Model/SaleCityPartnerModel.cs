using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class CityPartnerBill
    {
        public int ID { get; set; }

        public DateTime BillMonth { get; set; }
        public int Money { get; set; }

        public int? userID { get; set; }

        public string UserName { get; set; }

        public string CreateTime { get; set; }

        public Guid? CityPartnerGID { get; set; }

        public string CityPartnerName { get; set; }
    }

    public class CityPartnerBillRecord
    {
        public int ID { get; set; }
        public int? Status { get; set; }
        public string FullName { get; set; }

        public string CustomerName { get; set; }

        public string StatusName { get; set; }

        public int Money { get; set; }

        public int? userID { get; set; }

        public DateTime? CreateTime { get; set; }

        public Guid? CityPartnerGID { get; set; }

        public string CityPartnerName { get; set; }
    }

    public class SaleCityPartnerResult
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<CityPartnerBill> Bills { get; set; }
        public List<CityPartnerBillRecord> Records { get; set; }

    }
}
