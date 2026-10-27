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
    public class CityPartnerService : ServicesBase<Live_CityPartner>, ICityPartnerService, IScopedDependency
    {
        public async Task<List<Live_CityPartner>> PageList(Expression<Func<Live_CityPartner, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_CityPartner>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_CityPartner>> List(Expression<Func<Live_CityPartner, bool>> exp)
        {
            return await base.Context.Queryable<Live_CityPartner>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_CityPartner> Get(Expression<Func<Live_CityPartner, bool>> exp)
        {
            return await base.Context.Queryable<Live_CityPartner>()
                .FirstAsync(exp);
        }

   
        public async Task<int> Add(Live_CityPartner model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> Update(Live_CityPartner model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_CityPartner, bool>> exp)
        {
            return await base.Context.Deleteable<Live_CityPartner>().Where(exp).ExecuteCommandAsync()>0 ;
        }

        

    }
}
