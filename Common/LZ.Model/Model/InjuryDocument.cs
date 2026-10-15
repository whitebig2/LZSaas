using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Model
{
    public class InjuryDocument
    {
        
        public Guid? ID { get; set; }
        public string Name { get; set; }
        public int? ElectronicDocumentID { get; set; }
        
        public string TypeName { get; set; }

        public int InjuryID { get; set; }
        public string Path { get; set; } = "";
        public bool isOK { get; set; } = false;
    }
}
