using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class JDYFormModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FormID { get; set; }
        public string AppID { get; set; }
        public List<JDYFieldModel> Fields { get; set; }
    }
    public class JDYFieldModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Field { get; set; }
        public string JDYField { get; set; }
        public string FieldType { get; set; }
        public string ToJDYField { get; set; }
        public List<JDYFieldModel> Items { get; set; }
    }
}
