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
    public class InsurancePushRecordService : ServicesBase<Live_InsurancePushRecord>, IInsurancePushRecordService, IScopedDependency
    {
        public async Task<List<Live_V_InsurancePushRecord>> PageList(Expression<Func<Live_V_InsurancePushRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_V_InsurancePushRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_InsurancePushRecord>> List(Expression<Func<Live_InsurancePushRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsurancePushRecord>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }

        public async Task<List<Live_V_InsurancePushRecord>> ListView(Expression<Func<Live_V_InsurancePushRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_InsurancePushRecord>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }
        public async Task<int> Add(Live_InsurancePushRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_InsurancePushRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_InsurancePushRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_InsurancePushRecord> Get(Guid id)
        {
            return await base.Context.Queryable<Live_InsurancePushRecord>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_InsurancePushRecord> Get(Expression<Func<Live_InsurancePushRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_InsurancePushRecord>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_InsurancePushRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
