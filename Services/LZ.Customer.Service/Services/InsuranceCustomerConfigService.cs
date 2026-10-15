using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Model.ModelExtensions;


namespace LZ.Customer.Service.Services
{
    public class InsuranceCustomerConfigService : ServicesBase<Live_InsuranceCustomerConfig>, IInsuranceCustomerConfigService, IScopedDependency
    {
        public async Task<List<Live_V_InsuranceCustomerConfig>> ListView(Expression<Func<Live_V_InsuranceCustomerConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_InsuranceCustomerConfig>().Where(exp).OrderBy(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_InsuranceCustomerConfig>().Where(exp).OrderBy(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_V_InsuranceCustomerConfig>> ListView(Expression<Func<Live_V_InsuranceCustomerConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InsuranceCustomerConfig>().Where(exp).OrderBy(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_InsuranceCustomerConfig>> List(Expression<Func<Live_InsuranceCustomerConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_InsuranceCustomerConfig>().Where(exp).OrderBy(it=>it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_InsuranceCustomerConfig>().Where(exp).OrderBy(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_InsuranceCustomerConfig model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_InsuranceCustomerConfig>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_InsuranceCustomerConfig model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
       

        public async Task<Live_InsuranceCustomerConfig> Get(Guid id)
        {
            return await base.Context.Queryable<Live_InsuranceCustomerConfig>().FirstAsync(it=>it.ID == id);
        }

        public async Task<Live_InsuranceCustomerConfig> Get(Expression<Func<Live_InsuranceCustomerConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsuranceCustomerConfig>().FirstAsync(exp);
        }

        public async Task<Live_V_InsuranceCustomerConfig> GetView(Expression<Func<Live_V_InsuranceCustomerConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InsuranceCustomerConfig>().FirstAsync(exp);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_InsuranceCustomerConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
