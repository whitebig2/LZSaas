using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class ProfitManageRecordService : ServicesBase<Live_ProfitManageRecord>, IProfitManageRecordService, IScopedDependency
    {

        public async Task<List<Live_ProfitManageRecord>> PageList(Expression<Func<Live_ProfitManageRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_ProfitManageRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_ProfitManageRecord>> List(Expression<Func<Live_ProfitManageRecord, bool>> exp)
        {
           
                return await   base.Context.Queryable<Live_ProfitManageRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_ProfitManageRecord model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_ProfitManageRecord> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_ProfitManageRecord>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Update(List<Live_ProfitManageRecord> models)
        {
            return await   base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(Live_ProfitManageRecord model)
        {
            return await   base.Context.Updateable<Live_ProfitManageRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
       
        public async Task<Live_ProfitManageRecord> Get(Expression<Func<Live_ProfitManageRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_ProfitManageRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_ProfitManageRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_ProfitManageRecord, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_ProfitManageRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }


        public async Task<decimal> SumBusinessAmount(Expression<Func<Live_ProfitManageRecord, bool>> exp)
        {

            return await   base.Context.Queryable<Live_ProfitManageRecord>().Where(exp).SumAsync(it => it.BusinessAmount);
        }
    }
}
