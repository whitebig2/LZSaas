using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.BankDo
{
    public class BankResponseDto
    {
        public bool Success { get; set; }
        public int Code { get; set; } 
        public string Msg { get; set; } 
        public BankResponseDto()
        {
            Success = true;
            Code = 200;
            Msg = "成功";
        }
    }
}
