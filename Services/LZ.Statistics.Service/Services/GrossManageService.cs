using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Statistics.Service.Interface;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    public class GrossManageService : ServicesBase<Model.Live_ProfitManageRecord>, IGrossManageService, IScopedDependency
    {

        public async Task<decimal> ProfitBusInsuranceSumAmount(Expression<Func<Live_ProfitManageRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_ProfitManageRecord>();
            qb.Where(exp);
            return await qb.SumAsync(f => f.BusInsuranceSumAmount);
        }
    }
}
