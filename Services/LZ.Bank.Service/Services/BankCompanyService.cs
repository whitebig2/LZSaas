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
    public class BankCompanyService : ServicesBase<Live_BankCompany>, IBankCompanyService, IScopedDependency
    {

        public async Task<List<Live_V_BankCompany>> PageList(Expression<Func<Live_V_BankCompany, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BankCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BankCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankCompany>> List(Expression<Func<Live_BankCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_V_BankCompany>> ListView(Expression<Func<Live_V_BankCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BankCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_BankCompany model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
   
        public async Task<bool> Exist(Expression<Func<Live_BankCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCompany>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BankCompany model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
      
        public async Task<Live_BankCompany> Get(Expression<Func<Live_BankCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCompany>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BankCompany>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
