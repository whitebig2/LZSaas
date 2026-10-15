using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Users.Service.Services
{
    public class BusinessInsurancePushRecordService : ServicesBase<Live_BusinessInsurancePushRecord>, IBusinessInsurancePushRecordService, IScopedDependency
    {
        public async Task<List<Live_BusinessInsurancePushRecord>> PageList(Expression<Func<Live_BusinessInsurancePushRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusinessInsurancePushRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BusinessInsurancePushRecord>> List(Expression<Func<Live_BusinessInsurancePushRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsurancePushRecord>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }

        public async Task<List<Live_V_BusinessInsurancePushRecord>> ListView(Expression<Func<Live_V_BusinessInsurancePushRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsurancePushRecord>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }
        public async Task<bool> Update(Live_BusinessInsurancePushRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
     
        public async Task<Live_BusinessInsurancePushRecord> Get(Expression<Func<Live_BusinessInsurancePushRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsurancePushRecord>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BusinessInsurancePushRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
