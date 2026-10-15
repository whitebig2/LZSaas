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
    public class InjuryClaimRatioService : ServicesBase<Live_InjuryClaimRatio>, IInjuryClaimRatioService, IScopedDependency
    {
        public async Task<List<Model.Live_InjuryClaimRatio>> List(Expression<Func<Model.Live_InjuryClaimRatio, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_InjuryClaimRatio>().Where(exp);
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
        public async Task<List<Model.Live_V_InjuryClaimRatio>> ListView(Expression<Func<Model.Live_V_InjuryClaimRatio, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryClaimRatio>().Where(exp);
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
        public async Task<int> Add(Model.Live_InjuryClaimRatio model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Model.Live_InjuryClaimRatio> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryClaimRatio model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryClaimRatio>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Model.Live_InjuryClaimRatio, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryClaimRatio>().Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryClaimRatio, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryClaimRatio>().AnyAsync(exp);
        }
        public async Task<Model.Live_InjuryClaimRatio> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryClaimRatio>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryClaimRatio>().AnyAsync(it => it.ID == id);
        }
        public async Task<Live_InjuryClaimRatio> Get(Expression<Func<Live_InjuryClaimRatio, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryClaimRatio>().FirstAsync(exp);
        }
        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_InjuryClaimRatio>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
