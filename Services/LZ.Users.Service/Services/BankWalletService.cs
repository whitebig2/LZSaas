using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class BankWalletService : ServicesBase<Live_BankWallet>, IBankWalletService, IScopedDependency
    {
        public async Task<List<Live_BankWallet>> PageList(Expression<Func<Live_BankWallet, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BankWallet>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BankWallet>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankWallet>> List(Expression<Func<Live_BankWallet, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankWallet>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_BankWallet model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> AddList(List<Live_BankWallet> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Exist(Expression<Func<Live_BankWallet, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankWallet>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BankWallet model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_BankWallet> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_BankWallet> Get(Expression<Func<Live_BankWallet, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankWallet>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BankWallet>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
