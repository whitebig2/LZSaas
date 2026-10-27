using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.FxqModel
{
    public class FxqResponse<T>
    {
        public int Code { get; set; }
        public T Data { get; set; }
        public string Msg { get; set; }
        public string TradeNo { get; set; }
        public string VerifyId { get; set; }
    }
}
