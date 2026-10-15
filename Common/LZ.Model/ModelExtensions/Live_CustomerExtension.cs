using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_Customer")]
    public class Live_CustomerExtension : Live_Customer
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_Customer.CustomerGID), nameof(Live_CustomerContractConfig.CustomerGID))]
        public List<Live_CustomerContractConfig> ContractConfigs { get; set; }
    }
}
