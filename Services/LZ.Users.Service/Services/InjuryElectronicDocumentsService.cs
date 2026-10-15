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
using LZ.Model.PagedModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class InjuryElectronicDocumentsService : ServicesBase<Live_InjuryElectronicDocuments>, IInjuryElectronicDocumentsService, IScopedDependency
    {
        public async Task<List<Model.Live_InjuryElectronicDocuments>> List(Expression<Func<Model.Live_InjuryElectronicDocuments, bool>> exp, Model.PagedModel.UserPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_InjuryElectronicDocuments>().Where(exp);
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

        public async Task<int> Add(Model.Live_InjuryElectronicDocuments model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InjuryElectronicDocuments model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InjuryElectronicDocuments>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InjuryElectronicDocuments>().AnyAsync(it => it.ID == id);
        }
        public async Task<Model.Live_InjuryElectronicDocuments> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryElectronicDocuments>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_InjuryElectronicDocuments> GetByLoan(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InjuryElectronicDocuments>().Where(t => t.LoanID == id).FirstAsync();
        }

    }
}
