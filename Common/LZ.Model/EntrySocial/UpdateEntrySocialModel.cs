using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.EntrySocial
{
    public class UpdateEntrySocialModel
    {
        public int? SocialID { get; set; }
        public Guid? SocialGID { get; set; }
        public int? CustomerID { get; set; }
        public string CardID { get; set; }
        public int? Status { get; set; }
        public int? entryID { get; set; }
    }
}
