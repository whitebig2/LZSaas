using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface IRegionAddressService
    {
        Task<List<Model.Live_RegionAddress>> List(Expression<Func<Model.Live_RegionAddress, bool>> exp, string order = "");
    }
}
