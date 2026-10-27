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
    public class BankCompanyConfigService : ServicesBase<Live_BankCompanyConfig>, IBankCompanyConfigService, IScopedDependency
    {

        public async Task<List<Live_V_BankCompanyConfig>> PageList(Expression<Func<Live_V_BankCompanyConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BankCompanyConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BankCompanyConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BankCompanyConfig>> List(Expression<Func<Live_BankCompanyConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCompanyConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_V_BankCompanyConfig>> ListView(Expression<Func<Live_V_BankCompanyConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BankCompanyConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_BankCompanyConfig model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
   
        public async Task<bool> Exist(Expression<Func<Live_BankCompanyConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCompanyConfig>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BankCompanyConfig model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
      
        public async Task<Live_BankCompanyConfig> Get(Expression<Func<Live_BankCompanyConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BankCompanyConfig>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<Live_V_BankCompanyConfig> GetView(Expression<Func<Live_V_BankCompanyConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BankCompanyConfig>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BankCompanyConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
