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
    public class PersonnelFunctionService : ServicesBase<Live_PersonnelFunction>, IPersonnelFunctionService, IScopedDependency
    {
        public async Task<List<Live_PersonnelFunction>> List(Expression<Func<Live_PersonnelFunction, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_PersonnelFunction>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_PersonnelFunction>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_PersonnelFunctionItems>> Items(Expression<Func<Live_PersonnelFunctionItems, bool>> exp)
        {
            return await base.Context.Queryable<Live_PersonnelFunctionItems>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }
        public async Task<int> Add(Live_PersonnelFunction model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_PersonnelFunction>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_PersonnelFunction model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id,bool enabled)
        {
            return await base.Context.Updateable<Live_PersonnelFunction>().SetColumns(it => it.Enabled == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_PersonnelFunction> Get(int id)
        {
            return await base.Context.Queryable<Live_PersonnelFunction>().FirstAsync(it => it.ID == id);
        }
        public async Task UserTypeFunction(List<Model.Live_PersonnelFunctionItems> list)
        {
            if (list.Count > 0)
            {
                var m = list[0];
                await base.Context.Deleteable<Model.Live_PersonnelFunctionItems>().Where(it => it.CompanyID == m.CompanyID && it.UserType == m.UserType).ExecuteCommandAsync();
                await base.Context.Insertable(list).ExecuteCommandAsync();
            }
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_PersonnelFunction>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
