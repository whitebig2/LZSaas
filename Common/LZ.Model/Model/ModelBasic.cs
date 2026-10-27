using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
   public class ModelBasic
    {
       public virtual int? PerUserID { get; set; }
        public virtual int? UserID { get; set; }
        public virtual int? CompanyID { get; set; }
        public virtual int? PartnerID { get; set; }
        public virtual int? CustomerID { get; set; }
        public virtual int? PerPartnerID { get; set; }
        public virtual int? CustomerPartnerID { get; set; }
    }
}
