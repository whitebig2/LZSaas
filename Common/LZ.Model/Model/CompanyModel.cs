using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class CompanyModel
    {
        public SYS_Company? Company { get; set; } = null;
        public CompanyConfig? Config { get; set; } = null;
        public List<JDYFormModel>? JDYForms { get; set; }
    }
}
