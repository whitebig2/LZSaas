using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.BankDo
{
    public class BankUserDto
    {
        public Guid ID { get; set; }
        public int UserName { get; set; }
        public int UserID { get; set; }
        public bool IsEnable { get; set; }
    }
}
