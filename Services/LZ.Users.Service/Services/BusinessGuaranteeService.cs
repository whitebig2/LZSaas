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
    public class BusinessGuaranteeService : ServicesBase<Live_BusinessGuarantee>, IBusinessGuaranteeService, IScopedDependency
    {
        public async Task<List<Live_BusinessGuarantee>> PageList(Expression<Func<Live_BusinessGuarantee, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusinessGuarantee>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusinessGuarantee>> List(Expression<Func<Live_BusinessGuarantee, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessGuarantee>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusinessGuarantee> Get(Expression<Func<Live_BusinessGuarantee, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessGuarantee>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Update(Live_BusinessGuarantee model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(List<Live_BusinessGuarantee> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_BusinessGuarantee> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task Delete(Expression<Func<Live_BusinessGuarantee, bool>> exp)
        {
            await base.Context.Deleteable<Live_BusinessGuarantee>().Where(exp).ExecuteCommandAsync();
        }





    }
}
