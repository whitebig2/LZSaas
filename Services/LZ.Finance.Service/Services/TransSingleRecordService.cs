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
    public class TransSingleRecordService : ServicesBase<Live_TransSingleRecord>, ITransSingleRecordService, IScopedDependency
    {

        public async Task<List<Live_TransSingleRecord>> PageList(Expression<Func<Live_TransSingleRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_TransSingleRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_TransSingleRecord>> List(Expression<Func<Live_TransSingleRecord, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_TransSingleRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<bool> Add(Live_TransSingleRecord model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }

        public async Task<bool> Add(List<Live_TransSingleRecord> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandIdentityIntoEntityAsync();
        }

        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_TransSingleRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_TransSingleRecord model)
        {
            return await   base.Context.Updateable<Live_TransSingleRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_TransSingleRecord> Get(Expression<Func<Live_TransSingleRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_TransSingleRecord>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_TransSingleRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
