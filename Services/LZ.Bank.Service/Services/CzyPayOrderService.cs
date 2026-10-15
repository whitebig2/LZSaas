using LZ.Bank.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Services
{
    public class CzyPayOrderService : ServicesBase<Czy_PayOrder>, ICzyPayOrderService, IScopedDependency
    {
        public async Task<List<Czy_PayOrder>> PageList(Expression<Func<Czy_PayOrder, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Czy_PayOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Czy_PayOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Czy_PayOrder>> List(Expression<Func<Czy_PayOrder, bool>> exp)
        {
            return await base.Context.Queryable<Czy_PayOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Czy_PayOrderExtension>> ListNav(Expression<Func<Czy_PayOrderExtension, bool>> exp)
        {
            return await base.Context.Queryable<Czy_PayOrderExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public async Task<Czy_PayOrder> Get(Expression<Func<Czy_PayOrder, bool>> exp)
        {
            return await base.Context.Queryable<Czy_PayOrder>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }

        public async Task<Czy_PayOrderExtension> GetNav(Expression<Func<Czy_PayOrderExtension, bool>> exp)
        {
            return await base.Context.Queryable<Czy_PayOrderExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .FirstAsync();
        }

        public async Task<bool> Exist(Expression<Func<Czy_PayOrder, bool>> exp)
        {
            return await base.Context.Queryable<Czy_PayOrder>().AnyAsync(exp);
        }

        public async Task<bool> ExistDetail(Expression<Func<Czy_PayOrderDetail, bool>> exp)
        {
            return await base.Context.Queryable<Czy_PayOrderDetail>().AnyAsync(exp);
        }

        public async Task<bool> AddNav(Czy_PayOrderExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> Update(Czy_PayOrder model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateNav(Czy_PayOrderExtension model)
        {
            return await base.Context.UpdateNav(model)
                .Include(f => f.Records, new UpdateNavOptions
                {
                    OneToManyInsertOrUpdate = true,
                })
                .ExecuteCommandAsync();
        }

        public async Task<List<Czy_PayOrderDetail>> PageDetailList(Expression<Func<Czy_PayOrderDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Czy_PayOrderDetail>().Where(exp).OrderByDescending(it => it.ReceiveAccountName).ToListAsync();
            }
            var list = await base.Context.Queryable<Czy_PayOrderDetail>().Where(exp).OrderByDescending(it => it.ReceiveAccountName).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<bool> DeleteNav(Guid id)
        {
            var order = await Get(f => f.ID == id);
            if (order == null)
            {
                return false;
            }
            return await base.Context.DeleteNav<Czy_PayOrderExtension>(f => f.ID == id)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }
    }
}
