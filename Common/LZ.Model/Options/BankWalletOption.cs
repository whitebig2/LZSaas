using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Options
{
    public class BankWalletOption
    {
        public string Url { get; set; }
        public string AppId { get; set; }
        public string CorpNo { get; set; }
        public string OutUserId { get; set; }
        public string AESkey { get; set; }
        public string CamsPublicKey { get; set; }
        public string MyPrivateKey { get; set; }
        public string ApiGWPublicKey { get; set; }
    }
}
