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
    public class SalaryTransactionService : ServicesBase<Live_SalaryTransaction>, ISalaryTransactionService, IScopedDependency
    {

        public async Task<List<Live_SalaryTransaction>> PageList(Expression<Func<Live_SalaryTransaction, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_SalaryTransaction>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_SalaryTransaction>> List(Expression<Func<Live_SalaryTransaction, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_SalaryTransaction>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<bool> Add(Live_SalaryTransaction model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }

        public async Task<List<Live_SalaryTransactionExtension>> ListNav(Expression<Func<Live_SalaryTransactionExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalaryTransactionExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .ToListAsync();
        }

        public async Task<Live_SalaryTransaction> Get(Expression<Func<Live_SalaryTransaction, bool>> exp)
        {
            return await base.Context.Queryable<Live_SalaryTransaction>()
                .Where(exp)
                .FirstAsync();
        }


        public async Task<bool> Exist(Expression<Func<Live_SalaryTransaction, bool>> exp)
        {
            return await base.Context.Queryable<Live_SalaryTransaction>()
                .Where(exp)
                .AnyAsync();
        }


        public async Task<Live_SalaryTransactionExtension> GetNav(Expression<Func<Live_SalaryTransactionExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalaryTransactionExtension>()
                .Includes(f => f.Records)
                .Where(exp)
                .FirstAsync();
        }

        public async Task<bool> AddNav(Live_SalaryTransactionExtension model)
        {
            return await   base.Context.InsertNav(model)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_SalaryTransactionExtension> models)
        {
            return await   base.Context.InsertNav(models)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_SalaryTransaction>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_SalaryTransaction model)
        {
            return await   base.Context.Updateable<Live_SalaryTransaction>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
   
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_SalaryTransaction>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateNav(Live_SalaryTransactionExtension model)
        {
            return await   base.Context.UpdateNav<Live_SalaryTransactionExtension>(model)
                .Include(f=>f.Records, new SqlSugar.UpdateNavOptions()
                {
                    OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                })
                .ExecuteCommandAsync();
        }


        public async Task<List<Live_SalaryTransactionRecord>> PageRecordList(Expression<Func<Live_SalaryTransactionRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_SalaryTransactionRecord>().Where(exp).OrderByDescending(it => it.CardID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<bool> DeleteNav(Guid id)
        {
            return await base.Context.DeleteNav<Live_SalaryTransactionExtension>(f=>f.ID==id)
                .Include(f=>f.Records).ExecuteCommandAsync();
        }
    }
}
