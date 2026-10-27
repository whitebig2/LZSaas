using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.Interface
{
    public interface ITotpService
    {
        string GenerateSecretKey(int byteLength = 20);
        bool ValidateCode(string base32Secret, string code, int? window = null);
    }
}
