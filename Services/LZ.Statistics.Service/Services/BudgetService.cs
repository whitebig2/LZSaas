using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    public class BudgetService : ServicesBase<Model.Live_BudgetApplyRecord>, IBudgetService, IScopedDependency
    {
        public async Task<List<Model.Statistics.EntryModel>> StatusCount(Expression<Func<Model.Live_BudgetApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_BudgetApplyRecord>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.ProcessStatus });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Status = it.ProcessStatus, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
    }
}
