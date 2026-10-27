using LZ.Core.Interface;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class InjuryFinancialWriteOffService : ServicesBase<Live_InjuryFinancialWriteOff>, IInjuryFinancialWriteOffService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryFinancialWriteOff>> List(Expression<Func<Model.Live_V_InjuryFinancialWriteOff, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryFinancialWriteOff>().Where(exp);
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

        public async Task<int> Add(Model.Live_InjuryFinancialWriteOff model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryFinancialWriteOff model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryFinancialWriteOff>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Model.Live_InjuryFinancialWriteOff, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryFinancialWriteOff>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Exist(Expression<Func<Live_InjuryFinancialWriteOff, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryFinancialWriteOff>().AnyAsync(exp);
        }

        public async Task<bool> UpdateStatus(int id, int status, string msg = "")
        {

            var db = base.Context;
            var ub = db.Updateable<Live_InjuryFinancialWriteOff>();
            if (status > 0)
            {
                ub.SetColumns(it => it.Status == status);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                ub.SetColumns(it => it.Remark == msg);
            }
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_InjuryFinancialWriteOff> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryFinancialWriteOff>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_V_InjuryFinancialWriteOff> View(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_InjuryFinancialWriteOff>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_InjuryFinancialWriteOff> GetByInjuryID(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryFinancialWriteOff>().Where(t => t.InjuryID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryFinancialWriteOff>().AnyAsync(it => it.ID == id);
        }
    }
}
