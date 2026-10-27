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
    public class CompanyDictionaryService : ServicesBase<SYS_CompanyDictionary>, ICompanyDictionaryService, IScopedDependency
    {
        public async Task<List<SYS_CompanyDictionary>> List(Expression<Func<SYS_CompanyDictionary, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_CompanyDictionary>().Where(exp).OrderBy(it=>it.Sort).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_CompanyDictionary>().Where(exp).OrderBy(it => it.Sort).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(SYS_CompanyDictionary model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<SYS_CompanyDictionary>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(SYS_CompanyDictionary model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).Where(it => it.ID == model.ID && it.CompanyID == model.CompanyID).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id,bool enabled)
        {
            return await base.Context.Updateable<SYS_CompanyDictionary>().SetColumns(it => it.Enabled == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_CompanyDictionary> Get(int id)
        {
            return await base.Context.Queryable<SYS_CompanyDictionary>().FirstAsync(it=>it.ID==id);
        }
        public async Task<SYS_CompanyDictionary> Get(Guid id)
        {
            return await base.Context.Queryable<SYS_CompanyDictionary>().FirstAsync(it => it.GUID == id);
        }
        public async Task<bool> Delete(Guid id,int companyId)
        {
            return await base.Context.Deleteable<SYS_CompanyDictionary>().Where(it => it.GUID == id && it.CompanyID== companyId).ExecuteCommandAsync() > 0;
        }
    }
}
