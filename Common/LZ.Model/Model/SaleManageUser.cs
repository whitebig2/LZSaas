using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class SaleManageUser
    {
        public int UserID { get; set; }

        public string UserName { get; set; }
        public Guid? CityPartnerGID { get; set; }

        public string CityPartnerName { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public int? UserType { get; set; }

        public bool Enabled { get; set; }
        public string RoleIDS { get; set; }
        public string RoleNames { get; set; }
    }


    public class SaleMessageResult()
    {
        public string Msg { get; set; }

        public int code { get; set; }

    }

}
