using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Model.ModelExtensions;
using LZ.Users.Service.Interface;


namespace LZ.Users.Service.Services
{
    public class BusinessApiOrderDetailService : ServicesBase<Live_BusinessApiOrderDetail>, IBusinessApiOrderDetailService, IScopedDependency
    {
        public async Task<List<Live_BusinessApiOrderDetail>> PageList(Expression<Func<Live_BusinessApiOrderDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusinessApiOrderDetail>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusinessApiOrderDetail>> List(Expression<Func<Live_BusinessApiOrderDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessApiOrderDetail>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<bool> Update(Live_BusinessApiOrderDetail model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(List<Live_BusinessApiOrderDetail> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_BusinessApiOrderDetail> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_BusinessApiOrderDetail> Get(Expression<Func<Live_BusinessApiOrderDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessApiOrderDetail>().FirstAsync(exp);
        }

      
    }
}
