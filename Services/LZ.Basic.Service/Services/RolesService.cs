using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Basic.Service.Services
{
    public class RolesService : ServicesBase<SYS_Roles>, IRolesService, IScopedDependency
    {
        public async Task<List<SYS_Roles>> List(Expression<Func<SYS_Roles, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_Roles>().Where(exp).OrderByDescending(it => it.Name).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_Roles>().Where(exp).OrderByDescending(it => it.Name).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<SYS_RolesFunciton>> Menu(Guid id)
        {
            return await base.Context.Queryable<SYS_RolesFunciton>().Where(it=>it.RGID==id).ToListAsync();
        }
        public async Task<int> Add(SYS_Roles model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<SYS_Roles>().AnyAsync(it => it.RGID == id);
        }
        public async Task<bool> Update(SYS_Roles model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(Guid id,bool enabled)
        {
            return await base.Context.Updateable<SYS_Roles>().SetColumns(it => it.Enabled == enabled).Where(it => it.RGID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_Roles> Get(Guid id)
        {
            return await base.Context.Queryable<SYS_Roles>().FirstAsync(it=>it.RGID == id);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<SYS_Roles>().Where(it => it.RGID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> SetMenu(List<Model.SYS_RolesFunciton> models)
        {
            if (models.Count > 0)
            {
                await base.Context.Deleteable<SYS_RolesFunciton>().Where(it => it.RGID == models[0].RGID).ExecuteCommandAsync();
                await base.Context.Insertable(models.ToArray()).ExecuteCommandAsync();
                return true;
            }
            else
            {
                return false;
            }
           
        }
    }
}
