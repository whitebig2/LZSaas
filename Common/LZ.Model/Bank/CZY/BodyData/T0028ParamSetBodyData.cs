using LZ.Model.Bank.CZY.ParamSetModels;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BodyData
{
    public class T0028ParamSetBodyData : ICzyBodyData
    {
        public ICollection<T0028ParamSetItem> ParamSet { get; set; }
    }
}
