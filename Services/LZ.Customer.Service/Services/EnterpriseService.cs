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
    public class EnterpriseService : ServicesBase<Live_Enterprise>, IEnterpriseService, IScopedDependency
    {
        public async Task<List<Live_Enterprise>> List(Expression<Func<Live_Enterprise, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_Enterprise>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_Enterprise>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_Enterprise>> List(Expression<Func<Live_Enterprise, bool>> exp)
        {
            return await base.Context.Queryable<Live_Enterprise>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_Enterprise model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_Enterprise>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_Enterprise model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_Enterprise> Get(Guid id)
        {
            return await base.Context.Queryable<Live_Enterprise>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_Enterprise> Get(Expression<Func<Live_Enterprise, bool>> exp)
        {
            return await base.Context.Queryable<Live_Enterprise>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_Enterprise>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
