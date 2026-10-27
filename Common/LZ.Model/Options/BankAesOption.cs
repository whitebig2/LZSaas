using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Options
{
    public class BankAesOption
    {
        public string Key { get; set; }
        public string Iv { get; set; }
        public byte[] KeyBytes => Convert.FromHexString(Key);
        public byte[] IvBytes => Convert.FromHexString(Iv);
    }
}
