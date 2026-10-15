using LZ.Core.Interface;
using LZ.Core;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Bank.Service.Interface;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Bank.Service.Services
{
    public class IcbcOpenAccountRecordService : ServicesBase<Live_IcbcOpenAccountRecord>, IIcbcOpenAccountRecordService, IScopedDependency
    {
        public async Task<List<Live_IcbcOpenAccountRecord>> PageList(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_IcbcOpenAccountRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_IcbcOpenAccountRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_IcbcOpenAccountRecord>> List(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_IcbcOpenAccountRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_IcbcOpenAccountRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Add(List<Live_IcbcOpenAccountRecord> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Exist(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_IcbcOpenAccountRecord>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_IcbcOpenAccountRecord model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_IcbcOpenAccountRecord> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_IcbcOpenAccountRecord> Get(Expression<Func<Live_IcbcOpenAccountRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_IcbcOpenAccountRecord>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_IcbcOpenAccountRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
