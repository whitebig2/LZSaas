using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public class CaAuthIdentityDto
    {
        public Guid UserGid { get; set; }
        public string Name { get; set; }
        public string CardID { get; set; }
        public string Phone { get; set; }
        public string AuthType { get; set; }
        public string StatusName { get; set; }
        public string  AuthDate { get; set; }
        public string  FxqAppName { get; set; }

    }
}
