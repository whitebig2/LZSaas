using LZ.Core;
using LZ.Core.Interface;
using LZ.Customer.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Customer.Service.Services
{
    public class CustomerJobsService : ServicesBase<Live_CustomerJobs>, ICustomerJobsService, IScopedDependency
    {
        public async Task<List<Live_V_CustomerJobs>> List(Expression<Func<Live_V_CustomerJobs, bool>> exp)
        {
             return await base.Context.Queryable<Live_V_CustomerJobs>().Where(exp).ToListAsync(); ;
        }
        public async Task<int> Add(Model.Live_CustomerJobs model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<int> UpdateDefault(int id, int jobid,bool isDefault)
        {
            return await base.Context.Updateable<Model.Live_CustomerJobs>().SetColumns(it=>it.IsDefault== isDefault).Where(it=>it.CustomerID==id && it.EntryJobID==jobid).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_V_CustomerJobs, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_CustomerJobs>().AnyAsync(exp);
        }
        public async Task<Live_V_CustomerJobs> Get(Expression<Func<Live_V_CustomerJobs, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_CustomerJobs>().FirstAsync(exp);
        }
        public async Task<Live_V_CustomerJobs> IsDefault(Expression<Func<Live_V_CustomerJobs, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_CustomerJobs>().OrderByDescending(it=>it.IsDefault).FirstAsync(exp);
        }
        public async Task<bool> Delete(int id, int jobid)
        {
            return await base.Context.Deleteable<Live_CustomerJobs>().Where(it => it.CustomerID == id && it.EntryJobID == jobid).ExecuteCommandAsync() > 0;
        }
    }
}
