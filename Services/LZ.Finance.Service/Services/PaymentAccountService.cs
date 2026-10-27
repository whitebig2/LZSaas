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
    public class PaymentAccountService : ServicesBase<Live_PaymentAccount>, IPaymentAccountService, IScopedDependency
    {
        public async Task<List<Live_V_PaymentAccount>> PageList(Expression<Func<Live_V_PaymentAccount, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_PaymentAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_PaymentAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PaymentAccount>> List(Expression<Func<Live_PaymentAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_V_PaymentAccount>> ListView(Expression<Func<Live_V_PaymentAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_PaymentAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<int> Add(Live_PaymentAccount model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_PaymentAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAccount>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_PaymentAccount model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_PaymentAccount> Get(Guid id)
        {
            return await base.Context.Queryable<Live_PaymentAccount>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_PaymentAccount> Get(Expression<Func<Live_PaymentAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentAccount>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_PaymentAccount>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
