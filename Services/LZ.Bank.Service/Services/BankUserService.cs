using LZ.Bank.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Services
{
    public class BankUserService :ServicesBase<Live_BankUser>, IBankUserService, IScopedDependency
    {

        public async Task<List<Live_V_BankUser>> PageList(Expression<Func<Live_V_BankUser, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BankUser>().IgnoreColumns("SecretKey").Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BankUser>().IgnoreColumns("SecretKey").Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankUser>> List(Expression<Func<Live_BankUser, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankUser>().IgnoreColumns("SecretKey").Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_V_BankUser>> ListView(Expression<Func<Live_V_BankUser, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BankUser>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_V_BankUser> GetView(Expression<Func<Live_V_BankUser, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BankUser>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<int> Add(Live_BankUser model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
   
        public async Task<bool> Exist(Expression<Func<Live_BankUser, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankUser>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BankUser model)
        {
            return await base.Context.Updateable(model).IgnoreColumns("SecretKey").ExecuteCommandAsync() > 0;
        }
      
        public async Task<Live_BankUser> Get(Expression<Func<Live_BankUser, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankUser>().IgnoreColumns("SecretKey").OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BankUser>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_BankUser> GetWithSecretkey(Expression<Func<Live_BankUser, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankUser>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
    }
}
