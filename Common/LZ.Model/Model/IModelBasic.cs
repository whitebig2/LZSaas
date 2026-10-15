using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
   public  interface IModelBasic
    {
        int? PerUserID { get; set; }
        int? UserID { get; set; }
        int? CompanyID { get; set; }
        int? PartnerID { get; set; }
        int? FactoryUserID { get; set; }
        int? HandUserID { get; set; }
        int? CustomerID { get; set; }
        int? PerPartnerID { get; set; }
        int? CustomerPartnerID { get; set; }
    }
}
