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
    public class CompanyFunctionMenuService : ServicesBase<SYS_CompanyFunctionMenu>, ICompanyFunctionMenuService, IScopedDependency
    {
        public async Task<List<SYS_CompanyFunctionMenu>> List(Expression<Func<SYS_CompanyFunctionMenu, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_CompanyFunctionMenu>().Where(exp).OrderBy(it=>it.Sort).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_CompanyFunctionMenu>().Where(exp).OrderBy(it => it.Sort).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<SYS_CompanyFunctionMenu>> UserMenu(int uid)
        {
            return await base.Context.Queryable<SYS_CompanyFunctionMenu>().Where(it => it.FGID == SqlFunc.Subqueryable<SYS_ManageRoles>().LeftJoin<SYS_RolesFunciton>((a, b) => a.RGID == b.RGID).Where((a, b) => a.UserID == uid).GroupBy((a,b)=>b.FGID).Select((a, b) => b.FGID)).OrderBy(it=>it.Sort).ToListAsync();
        }
        public async Task<int> Add(SYS_CompanyFunctionMenu model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<SYS_CompanyFunctionMenu>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(SYS_CompanyFunctionMenu model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id,bool enabled)
        {
            return await base.Context.Updateable<SYS_CompanyFunctionMenu>().SetColumns(it => it.Enabled == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_CompanyFunctionMenu> Get(int id)
        {
            return await base.Context.Queryable<SYS_CompanyFunctionMenu>().FirstAsync(it=>it.ID==id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<SYS_CompanyFunctionMenu>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
