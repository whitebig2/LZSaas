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
    public class CustomerContractConfigService : ServicesBase<Live_CustomerContractConfig>, ICustomerContractConfigService, IScopedDependency
    {
        public async Task<List<Live_CustomerContractConfig>> List(Expression<Func<Live_CustomerContractConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_CustomerContractConfig>().Where(exp).OrderBy(it=>it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_CustomerContractConfig>().Where(exp).OrderBy(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_CustomerContractConfig model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_CustomerContractConfig>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_CustomerContractConfig model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
       

        public async Task<Live_CustomerContractConfig> Get(Guid id)
        {
            return await base.Context.Queryable<Live_CustomerContractConfig>().FirstAsync(it=>it.ID == id);
        }

        public async Task<Live_CustomerContractConfig> Get(Expression<Func<Live_CustomerContractConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_CustomerContractConfig>().FirstAsync(exp);
        }
       
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_CustomerContractConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_CustomerContractConfig> GetByCustomerGID(Guid CustomerGID)
        {
            return await base.Context.Queryable<Live_CustomerContractConfig>()
                .Where(f => f.CustomerGID == CustomerGID).FirstAsync();
        }


    
     
    }
}
