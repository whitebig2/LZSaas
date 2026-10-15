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
    public class BankUserCompanyService : ServicesBase<Live_BankUserCompany>, IBankUserCompanyService, IScopedDependency
    {

        public async Task<List<Live_BankUserCompany>> PageList(Expression<Func<Live_BankUserCompany, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BankUserCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BankUserCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankUserCompany>> List(Expression<Func<Live_BankUserCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankUserCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_BankUserCompany model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_BankUserCompany> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_BankUserCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankUserCompany>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BankUserCompany model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
      
        public async Task<Live_BankUserCompany> Get(Expression<Func<Live_BankUserCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankUserCompany>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BankUserCompany>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(Expression<Func<Live_BankUserCompany, bool>> exp)
        {
            return await base.Context.Deleteable<Live_BankUserCompany>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
