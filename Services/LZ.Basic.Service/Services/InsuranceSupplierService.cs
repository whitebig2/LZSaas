using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Services
{
    public class InsuranceSupplierService : ServicesBase<Live_InsuranceSupplier>, IInsuranceSupplierService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InsuranceSupplier>> List(Expression<Func<Model.Live_V_InsuranceSupplier, bool>> exp, BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_InsuranceSupplier>().Where(exp);
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

        public async Task<int> Add(Model.Live_InsuranceSupplier model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_InsuranceSupplier model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_InsuranceSupplier>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_InsuranceSupplier, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceSupplier>().AnyAsync(exp);
        }

        public async Task<int> Count(Expression<Func<Live_InsuranceSupplier, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceSupplier>().CountAsync(exp);
        }
        public async Task<Model.Live_InsuranceSupplier> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_InsuranceSupplier>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_InsuranceSupplier>().AnyAsync(it => it.ID == id);
        }
    }
}
