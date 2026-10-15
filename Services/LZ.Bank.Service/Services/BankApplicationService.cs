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
    public class BankApplicationService : ServicesBase<Czy_BankApplication>, IBankApplicationService, IScopedDependency
    {
        public async Task<List<Czy_BankApplication>> PageList(Expression<Func<Czy_BankApplication, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Czy_BankApplication>().IgnoreColumns("AppPrivateKey", "AppEncryptKey").Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Czy_BankApplication>().IgnoreColumns("AppPrivateKey", "AppEncryptKey").Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Czy_BankApplication>> List(Expression<Func<Czy_BankApplication, bool>> exp)
        {
            return await base.Context.Queryable<Czy_BankApplication>().IgnoreColumns("AppPrivateKey", "AppEncryptKey").Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<int> Add(Czy_BankApplication model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Czy_BankApplication, bool>> exp)
        {
            return await base.Context.Queryable<Czy_BankApplication>().AnyAsync(exp);
        }

        public async Task<bool> Update(Czy_BankApplication model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<Czy_BankApplication> Get(Expression<Func<Czy_BankApplication, bool>> exp)
        {
            return await base.Context.Queryable<Czy_BankApplication>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Czy_BankApplication>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
