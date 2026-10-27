using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public class CustomerContractFileDto
    {
        public Guid CustomerGID { get; set; }
        public List<Live_CustomerContractConfig> ContractConfigs { get; set; }
    }
}
