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
    internal class EntryRecordService : ServicesBase<Model.Live_UserEntryRecord>, IEntryRecordService, IScopedDependency
    {


        public async Task<Live_V_UserEntryRecord> Model(Expression<Func<Live_V_UserEntryRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntryRecord>();
            qb.Where(exp);
            qb.OrderByDescending(it=>it.ID);
            return await qb.FirstAsync();
        }
    }
}
