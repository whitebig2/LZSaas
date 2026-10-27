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
    public class InjuryLoanService : ServicesBase<Live_InjuryLoan>, IInjuryLoanService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryLoan>> List(Expression<Func<Model.Live_V_InjuryLoan, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InjuryLoan>().Where(exp);
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

        public async Task<int> Add(Model.Live_InjuryLoan model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<Model.Live_V_InjuryLoan> AddAndGet(Model.Live_InjuryLoan model)
        {
            var db = base.Context;
            int i = await db.Insertable(model).ExecuteReturnIdentityAsync();
            return await View(i);
        }
        public async Task<bool> Update(Model.Live_InjuryLoan model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryLoan>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InjuryLoan, bool>> exp)
        {
            return await base.Context.Queryable<Live_InjuryLoan>().AnyAsync(exp);
        }
        public async Task<Model.Live_InjuryLoan> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryLoan>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_V_InjuryLoan> View(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_InjuryLoan>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> UpdateStatus(int id, int status, string msg = "")
        {

            var db = base.Context;
            var ub = db.Updateable<Live_InjuryLoan>();
            if (status > 0)
            {
                ub.SetColumns(it => it.Status == status);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                ub.SetColumns(it => it.Content == msg);
            }
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryLoan>().AnyAsync(it => it.ID == id);
        }
    }
}
