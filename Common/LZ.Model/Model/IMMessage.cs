using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model
{
    public class IMMessage
    {
        public string title { get; set; }
        public string content { get; set; }
        public string picture { get; set; }
        public int touid { get; set; }
        public int totype { get; set; }
        public int fromuid { get; set; }
        public int fromtype { get; set; }
        public string chattype { get; set; }
        public bool display { get; set; }
        public string type { get; set; }
        public int fromid { get; set; }
        public int fromstatus { get; set; }
        public DateTime intDate { get; set; }
        public DateTime entryDate { get; set; }
        public DateTime levelDate { get; set; }
        public int jid { get; set; }
        public int? busid { get; set; }
        public int? ruid { get; set; }
    }
}
