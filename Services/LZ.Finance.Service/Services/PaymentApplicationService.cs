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
    public class PaymentApplicationService : ServicesBase<Live_PaymentApplication>, IPaymentApplicationService, IScopedDependency
    {
        public async Task<List<Live_PaymentApplication>> PageList(Expression<Func<Live_PaymentApplication, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_PaymentApplication>()
                    .Where(exp)
                    .OrderByDescending(it => it.IsDefault)
                    .OrderByDescending(it => it.CreateTime)
                    .ToListAsync();
            }
            var list = await base.Context.Queryable<Live_PaymentApplication>()
                .Where(exp)
                .OrderByDescending(it => it.IsDefault)
                .OrderByDescending(it => it.CreateTime)
                .ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PaymentApplication>> List(Expression<Func<Live_PaymentApplication, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentApplication>()
                .Where(exp)
                .OrderByDescending(it => it.IsDefault)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public async Task<int> Add(Live_PaymentApplication model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_PaymentApplication, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentApplication>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_PaymentApplication model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_PaymentApplication> Get(Guid id)
        {
            return await base.Context.Queryable<Live_PaymentApplication>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_PaymentApplication> Get(Expression<Func<Live_PaymentApplication, bool>> exp)
        {
            return await base.Context.Queryable<Live_PaymentApplication>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_PaymentApplication>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task ClearOtherDefault(int companyId, Guid excludeId)
        {
            await base.Context.Updateable<Live_PaymentApplication>()
                .SetColumns(it => it.IsDefault == false)
                .SetColumns(it => it.UpdateTime == DateTime.Now)
                .Where(it => it.CompanyID == companyId && it.IsDefault && !it.IsDelete && it.ID != excludeId)
                .ExecuteCommandAsync();
        }
    }
}
