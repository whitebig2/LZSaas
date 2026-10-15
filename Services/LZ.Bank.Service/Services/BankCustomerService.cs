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
    public class BankCustomerService :ServicesBase<Live_BankCustomer>, IBankCustomerService, IScopedDependency
    {

        public async Task<List<Live_V_BankCustomer>> PageList(Expression<Func<Live_V_BankCustomer, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BankCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BankCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankCustomer>> List(Expression<Func<Live_BankCustomer, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_V_BankCustomer>> ListView(Expression<Func<Live_V_BankCustomer, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BankCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_BankCustomer model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
   
        public async Task<bool> Exist(Expression<Func<Live_BankCustomer, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCustomer>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BankCustomer model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
      
        public async Task<Live_BankCustomer> Get(Expression<Func<Live_BankCustomer, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCustomer>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BankCustomer>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
