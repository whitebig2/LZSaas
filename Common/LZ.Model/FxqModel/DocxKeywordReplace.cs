using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.FxqModel
{
    public class DocxKeywordReplace<T>
    {
        [JsonProperty(Order = 1)]
        public string Contract { get; set; }
        [JsonProperty(Order = 2)]
        public T Replaces { get; set; }
        [JsonProperty(Order = 3)]
        public int Rtype { get; set; }
    }
}
