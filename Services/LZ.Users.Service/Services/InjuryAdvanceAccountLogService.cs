using LZ.Core.Interface;
using LZ.Core;
using LZ.Model;
using LZ.Users.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class InjuryAdvanceAccountLogService : ServicesBase<Live_InjuryAdvanceAccountLog>, IInjuryAdvanceAccountLogService, IScopedDependency
    {
        public async Task<List<Model.Live_InjuryAdvanceAccountLog>> List(Expression<Func<Model.Live_InjuryAdvanceAccountLog, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_InjuryAdvanceAccountLog>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }

            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_InjuryAdvanceAccountLog model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryAdvanceAccountLog model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryAdvanceAccountLog>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Model.Live_InjuryAdvanceAccountLog, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryAdvanceAccountLog>().Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryAdvanceAccountLog, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryAdvanceAccountLog>().AnyAsync(exp);
        }
        public async Task<Model.Live_InjuryAdvanceAccountLog> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryAdvanceAccountLog>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryAdvanceAccountLog>().AnyAsync(it => it.ID == id);
        }
        public async Task<Live_InjuryAdvanceAccountLog> Get(Expression<Func<Live_InjuryAdvanceAccountLog, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryAdvanceAccountLog>().FirstAsync(exp);
        }
    }
}
