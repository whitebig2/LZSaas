using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.PagedModel;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class BusInsuranceCompanyService : ServicesBase<Live_BusInsuranceCompany>, IBusInsuranceCompanyService, IScopedDependency
    {
        public async Task<List<Live_BusInsuranceCompany>> PageList(Expression<Func<Live_BusInsuranceCompany, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusInsuranceCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusInsuranceCompany>> List(Expression<Func<Live_BusInsuranceCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceCompany>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusInsuranceCompany> Get(Expression<Func<Live_BusInsuranceCompany, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceCompany>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }

        public async Task<bool> Update(Live_BusInsuranceCompany model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(Live_BusInsuranceCompany model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(List<Live_BusInsuranceCompany> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_BusInsuranceCompany> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task Delete(Expression<Func<Live_BusInsuranceCompany, bool>> exp)
        {
            await base.Context.Deleteable<Live_BusInsuranceCompany>().Where(exp).ExecuteCommandAsync();
        }
    }
}
