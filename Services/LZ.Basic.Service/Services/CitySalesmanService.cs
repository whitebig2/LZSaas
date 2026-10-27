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
    public class CitySalesmanService : ServicesBase<Live_CitySalesman>, ICitySalesmanService, IScopedDependency
    {
        public async Task<List<Live_CitySalesman>> PageList(Expression<Func<Live_CitySalesman, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_CitySalesman>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_CitySalesman>> List(Expression<Func<Live_CitySalesman, bool>> exp)
        {
            return await base.Context.Queryable<Live_CitySalesman>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_CitySalesman> Get(Expression<Func<Live_CitySalesman, bool>> exp)
        {
            return await base.Context.Queryable<Live_CitySalesman>()
                .FirstAsync(exp);
        }

   
        public async Task<int> Add(Live_CitySalesman model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Update(Live_CitySalesman model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_CitySalesman, bool>> exp)
        {
            return await base.Context.Deleteable<Live_CitySalesman>().Where(exp).ExecuteCommandAsync()>0 ;
        }

        

    }
}
