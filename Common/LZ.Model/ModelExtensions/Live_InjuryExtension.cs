using LZ.Model.ModelExtensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_InjuryInfo")]
    public class Live_InjuryExtension:Live_InjuryInfo
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_InjuryApplyRecord.InjuryID))]
        public List<Live_InjuryApplyRecord> Records { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(Live_InjuryRepay.InjuryID))]
        public List<Live_InjuryRepay> repay { get; set; }
        public void cal()
        {
            if(Records!=null)
            {
                foreach(var x in Records)
                {
                    x.Money = this.AdvanceApplyAmount??0;
                }
            }
        }

    }
}
