using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class CustomerCountByCityPartnerModel
    {
        public Guid? CityPartnerGID { get; set; }
        public string CityPartnerName { get; set; } = string.Empty;

        public int Count { get; set; }

        public CustomerCountByCityPartnerModel()
        {
            Count = 0;
        }
    }
}
