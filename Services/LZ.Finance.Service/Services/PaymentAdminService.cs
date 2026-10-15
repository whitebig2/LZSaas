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
    public class PaymentAdminService : ServicesBase<Live_PaymentAdmin>, IPaymentAdminService, IScopedDependency
    {
        public async Task<List<Live_V_PaymentAdmin>> PageList(Expression<Func<Live_V_PaymentAdmin, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_PaymentAdmin>().IgnoreColumns("SecretKey").Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_PaymentAdmin>().IgnoreColumns("SecretKey").Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PaymentAdmin>> List(Expression<Func<Live_PaymentAdmin, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAdmin>().IgnoreColumns("SecretKey").Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<int> Add(Live_PaymentAdmin model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_PaymentAdmin, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAdmin>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_PaymentAdmin model)
        {
            return await base.Context.Updateable(model).IgnoreColumns("SecretKey").ExecuteCommandAsync() > 0;
        }

        public async Task<Live_PaymentAdmin> Get(Guid id)
        {
            return await base.Context.Queryable<Live_PaymentAdmin>().IgnoreColumns("SecretKey").FirstAsync(it => it.ID == id);
        }

        public async Task<Live_PaymentAdmin> Get(Expression<Func<Live_PaymentAdmin, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAdmin>().IgnoreColumns("SecretKey").OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<Live_PaymentAdmin> GetWithSecretkey(Expression<Func<Live_PaymentAdmin, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAdmin>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_PaymentAdmin>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
