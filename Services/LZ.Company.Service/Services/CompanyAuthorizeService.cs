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
    public class CompanyAuthorizeService : ServicesBase<Live_CompanyAuthorize>, ICompanyAuthorizeService, IScopedDependency
    {
        public async Task<List<Live_V_CompanyAuthorize>> List(Expression<Func<Live_V_CompanyAuthorize, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_CompanyAuthorize>().Where(exp).OrderByDescending(it => it.CompanyID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_CompanyAuthorize>().Where(exp).OrderByDescending(it => it.CompanyID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_CompanyAuthorize model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_CompanyAuthorize, bool>> exp)
        {
            return await base.Context.Queryable<Live_CompanyAuthorize>().AnyAsync(exp);
        }

        public async Task<Model.Live_CompanyAuthorize> Get(Expression<Func<Model.Live_CompanyAuthorize, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_CompanyAuthorize>().Where(exp).FirstAsync();
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_CompanyAuthorize>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
