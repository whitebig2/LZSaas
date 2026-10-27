using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class EmpapiVerifyResponse:EmpapiResponse
    {
        public TokenResponse Data { get; set; }
    }

    public class TokenResponse 
    {
        public string Token { get; set; }
    }
}
