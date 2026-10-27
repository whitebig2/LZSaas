using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_ContractRecord")]
    public class Live_ContractRecordExtension : Live_ContractRecord
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_ContractRecordSigner.ContractRecordID))]
        public List<Live_ContractRecordSigner> Signers { get; set; }
    }
}
