using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_ContractFileConfig")]
    public class Live_ContractFileConfigExtension:Live_ContractFileConfig
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_ContractFileSigner.ContractFileConfigID))]
        public List<Live_ContractFileSigner> Signers { get; set; }
    }
}
