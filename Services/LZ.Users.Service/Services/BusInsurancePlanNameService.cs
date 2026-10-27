using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class BusInsurancePlanNameService : ServicesBase<Live_BusInsurancePlanName>, IBusInsurancePlanNameService, IScopedDependency
    {
        public async Task<List<Live_BusInsurancePlanName>> PageList(Expression<Func<Live_BusInsurancePlanName, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusInsurancePlanName>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusInsurancePlanName>> List(Expression<Func<Live_BusInsurancePlanName, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsurancePlanName>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusInsurancePlanName> Get(Expression<Func<Live_BusInsurancePlanName, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsurancePlanName>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }

        public async Task<bool> Update(Live_BusInsurancePlanName model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(Live_BusInsurancePlanName model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(List<Live_BusInsurancePlanName> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_BusInsurancePlanName> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task Delete(Expression<Func<Live_BusInsurancePlanName, bool>> exp)
        {
            await base.Context.Deleteable<Live_BusInsurancePlanName>().Where(exp).ExecuteCommandAsync();
        }
    }
}
