using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public record CheckbodyResultRecord
    {
        public string  Code { get; set; }
        public string Message { get; set; }
        public CheckbodyResultItemRecord Result { get; set; }
    }

    public record CheckbodyResultItemRecord
    {
        public string Name { get; set; }
        public string Idcard { get; set; }
        public string Res { get; set; }
        public string Description { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string Address { get; set; }
    }
}
