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
    public class SalarySplitSendService : ServicesBase<Live_SalarySplitSend>, ISalarySplitSendService, IScopedDependency
    {

        public async Task<List<Live_V_SalarySplitSendExtension>> PageList(Expression<Func<Live_V_SalarySplitSendExtension, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_SalarySplitSendExtension>()
                .Includes(f => f.Records)
                .Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<List<live_V_SalarySplitSendRecord>> PageRecordList(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<live_V_SalarySplitSendRecord>()
                .Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<live_V_SalarySplitSendRecord>> ListRecordViewList(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp)
        {
            var list = await   base.Context.Queryable<live_V_SalarySplitSendRecord>()
                .Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }


        //public async Task<List<Live_V_SalaryRecord>> ListView(Expression<Func<Live_V_SalaryRecord, bool>> exp)
        //{
        //    var list = await   base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        //    return list;
        //}


        public async Task<Live_V_SalarySplitSend> GetView(Expression<Func<Live_V_SalarySplitSend, bool>> exp)
        {
            var entity = await   base.Context.Queryable<Live_V_SalarySplitSend>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
            return entity;
        }

        public async Task<live_V_SalarySplitSendRecord> GetRecordView(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp)
        {
            var entity = await   base.Context.Queryable<live_V_SalarySplitSendRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
            return entity;
        }



        public async Task<bool> Add(Live_SalarySplitSend model)
        {
            return await   base.Context.Insertable<Live_SalarySplitSend>(model).ExecuteCommandAsync()>0;
        }

      
        public async Task<bool> Update(Live_SalarySplitSend model)
        {
            return await   base.Context.Updateable<Live_SalarySplitSend>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_SalarySplitSend, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_SalarySplitSend>(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_SalarySplitSend> Get(Expression<Func<Live_SalarySplitSend, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalarySplitSend>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }



        public async Task<Live_SalarySplitSendExtension> GetNav(Expression<Func<Live_SalarySplitSendExtension, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalarySplitSendExtension>()
                .Includes(f => f.Records)
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }


        public async Task<bool> UpdateNav(Live_SalarySplitSendExtension model)
        {
            return await   base.Context
                  .UpdateNav<Live_SalarySplitSendExtension>(model)
                  .Include(f => f.Records, new SqlSugar.UpdateNavOptions()
                  {
                      OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                  })
                  .ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Live_SalarySplitSendExtension model)
        {
            return await   base.Context
                  .DeleteNav<Live_SalarySplitSendExtension>(model)
                  .Include(f => f.Records)
                  .ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Expression<Func<Live_SalarySplitSendExtension, bool>> exp)
        {
            return await   base.Context
                  .DeleteNav<Live_SalarySplitSendExtension>(exp)
                  .Include(f => f.Records)
                  .ExecuteCommandAsync();
        }











    }
}
