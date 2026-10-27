using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Services
{
    public class PaymentAdminSignUnitService : ServicesBase<Live_PaymentAdminSignUnit>, IPaymentAdminSignUnitService, IScopedDependency
    {
        public async Task<List<Live_PaymentAdminSignUnit>> PageList(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_PaymentAdminSignUnit>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_PaymentAdminSignUnit>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PaymentAdminSignUnit>> List(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAdminSignUnit>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<int> Add(Live_PaymentAdminSignUnit model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_PaymentAdminSignUnit> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAdminSignUnit>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_PaymentAdminSignUnit model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_PaymentAdminSignUnit> Get(Guid id)
        {
            return await base.Context.Queryable<Live_PaymentAdminSignUnit>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_PaymentAdminSignUnit> Get(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAdminSignUnit>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_PaymentAdminSignUnit>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_PaymentAdminSignUnit, bool>> exp)
        {
            return await base.Context.Deleteable<Live_PaymentAdminSignUnit>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
