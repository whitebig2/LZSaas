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
using LZ.Model.ModelExtensions;

namespace LZ.Finance.Service.Services
{
    public class TransBatchService : ServicesBase<Live_TransBatch>, ITransBatchService, IScopedDependency
    {

        public async Task<List<Live_TransBatch>> PageList(Expression<Func<Live_TransBatch, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_TransBatch>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_TransBatch>> List(Expression<Func<Live_TransBatch, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_TransBatch>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<bool> Add(Live_TransBatch model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }

        public async Task<List<Live_TransBatchExtension>> ListNav(Expression<Func<Live_TransBatchExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_TransBatchExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .ToListAsync();
        }

        public async Task<Live_TransBatchExtension> GetNav(Expression<Func<Live_TransBatchExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_TransBatchExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .FirstAsync();
        }

        public async Task<bool> AddNav(Live_TransBatchExtension model)
        {
            return await   base.Context.InsertNav(model)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_TransBatchExtension> models)
        {
            return await   base.Context.InsertNav(models)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_TransBatch>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_TransBatch model)
        {
            return await   base.Context.Updateable<Live_TransBatch>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_TransBatch> Get(Expression<Func<Live_TransBatch, bool>> exp)
        {
            return await   base.Context.Queryable<Live_TransBatch>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_TransBatch>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateNav(Live_TransBatchExtension model)
        {
            return await   base.Context.UpdateNav<Live_TransBatchExtension>(model)
                .Include(f=>f.Records, new SqlSugar.UpdateNavOptions()
                {
                    OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                })
                .ExecuteCommandAsync();
        }
    }
}
