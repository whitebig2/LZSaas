using LZ.Company.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Company.Service.Services
{
    public class CompanyFunctionService : ServicesBase<SYS_CompanyFunction>, ICompanyFunctionService, IScopedDependency
    {
        public async Task<List<SYS_CompanyFunction>> List(Expression<Func<SYS_CompanyFunction, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_CompanyFunction>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_CompanyFunction>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(List<SYS_CompanyFunction> models)
        {
            await base.Context.Deleteable<SYS_CompanyFunction>().Where(it => it.CompanyID == models[0].CompanyID).ExecuteCommandAsync();
          return  await base.Context.Insertable(models).ExecuteCommandAsync();
        }
    }
}
