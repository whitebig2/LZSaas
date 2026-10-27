using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Services
{
    public class RegionAddressService : ServicesBase<Live_RegionAddress>, IRegionAddressService, IScopedDependency
    {
        public async Task<List<Model.Live_RegionAddress>> List(Expression<Func<Model.Live_RegionAddress, bool>> exp, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_RegionAddress>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }

            return await sco.ToListAsync();
        }
    }
}
