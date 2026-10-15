using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank
{
    public class TransactionPayloadRequest
    {
        public string Algorithm { get; set; }
        public string Padding { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public string Signature { get; set; }
        public string Iv { get; set; }
        public long Timestamp { get; set; }
        public string Nonce { get; set; }
    }
}
