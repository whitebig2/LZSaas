using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public class ContractFileConfigDtos
    {
        public Live_ContractFileConfig Config { get; set; }
        public Live_ContractFileSigner Signers { get; set; }
        public ContractFileConfigDtos()
        {
            Signers=new Live_ContractFileSigner();
        }
    }
}
