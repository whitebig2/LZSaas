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
    public class InjuryAdvanceAccountService : ServicesBase<Live_InjuryAdvanceAccount>, IInjuryAdvanceAccountService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryAdvanceAccount>> List(Expression<Func<Model.Live_V_InjuryAdvanceAccount, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryAdvanceAccount>().Where(exp);
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

        public async Task<int> Add(Model.Live_InjuryAdvanceAccount model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryAdvanceAccount model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryAdvanceAccount>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Model.Live_InjuryAdvanceAccount, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryAdvanceAccount>().Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryAdvanceAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryAdvanceAccount>().AnyAsync(exp);
        }
        public async Task<Model.Live_InjuryAdvanceAccount> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryAdvanceAccount>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ChangeMoney(int id, decimal money)
        {
            var db = base.Context;
            var model = await db.Queryable<Model.Live_InjuryAdvanceAccount>().Where(t => t.ID == id).FirstAsync();
            if (model == null) return false;
            var used = model.AvailableAmount - money;
            if (used < 0) return false;
            model.AvailableAmount = used;
            model.UsedAmount = model.UsedAmount + money;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryAdvanceAccount>().AnyAsync(it => it.ID == id);
        }
        public async Task<Live_V_InjuryAdvanceAccount> GetView(Expression<Func<Live_V_InjuryAdvanceAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InjuryAdvanceAccount>().FirstAsync(exp);
        }
        public async Task<Live_InjuryAdvanceAccount> Get(Expression<Func<Live_InjuryAdvanceAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryAdvanceAccount>().FirstAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_InjuryApplyRecord>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
