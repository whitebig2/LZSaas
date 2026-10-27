using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Common
{
   public  class AppSettings
    {
        public string ConnectionStrings { set; get; }

        public bool ConStringEncrypt { set; get; }
    }
    public class ConnectionString
    {
        public string MsSQL { set; get; }
    }
}
