using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Options
{
    public class FxqConfigOption
    {
        public List<FxqConfigOptionItem> Items { get; set; }
    }

    public class FxqConfigOptionItem
    {
        public string AppName { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string IdentityUrl { get; set; }
        public string RestapiUrl { get; set; }
    }


}
