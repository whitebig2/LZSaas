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
    public class PaymentSignUnitService : ServicesBase<Live_PaymentSignUnit>, IPaymentSignUnitService, IScopedDependency
    {
        public async Task<List<Live_V_PaymentSignUnit>> PageList(Expression<Func<Live_V_PaymentSignUnit, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_PaymentSignUnit>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_PaymentSignUnit>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PaymentSignUnit>> List(Expression<Func<Live_PaymentSignUnit, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentSignUnit>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<int> Add(Live_PaymentSignUnit model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_PaymentSignUnit, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentSignUnit>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_PaymentSignUnit model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_PaymentSignUnit> Get(Guid id)
        {
            return await base.Context.Queryable<Live_PaymentSignUnit>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_PaymentSignUnit> Get(Expression<Func<Live_PaymentSignUnit, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentSignUnit>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_PaymentSignUnit>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
