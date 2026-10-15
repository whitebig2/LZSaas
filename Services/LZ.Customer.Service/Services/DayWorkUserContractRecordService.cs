using LZ.Core.Interface;
using LZ.Core;
using LZ.Customer.Service.Interface;
using LZ.Model;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Services
{
    public class DayWorkUserContractRecordService : ServicesBase<Live_DayWorkUserContractRecord>, IDayWorkUserContractRecordService, IScopedDependency
    {
        public async Task<List<Model.Live_V_DayWorkUserContractRecord>> List(Expression<Func<Model.Live_V_DayWorkUserContractRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_DayWorkUserContractRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_DayWorkUserContractRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            //var count = new RefAsync<int>();
            //return await base.Context.Queryable<Live_V_DayWorkUserContractRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_DayWorkUserContractRecord model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_DayWorkUserContractRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_DayWorkUserContractRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_DayWorkUserContractRecord> Get(int id)
        {
            return await base.Context.Queryable<Live_DayWorkUserContractRecord>().FirstAsync(it => it.ID == id);
        }
        public async Task<Model.Live_DayWorkUserContractRecord> Get(Expression<Func<Model.Live_DayWorkUserContractRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_DayWorkUserContractRecord>().Where(exp).FirstAsync();
        }
        public async Task<Model.Live_V_DayWorkUserContractRecord> GetView(Expression<Func<Model.Live_V_DayWorkUserContractRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_DayWorkUserContractRecord>().Where(exp).FirstAsync();
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_DayWorkUserContractRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> ChangeStatus(int id, int status)
        {
            return await base.Context.Updateable<Live_DayWorkUserContractRecord>().SetColumns(it => it.Status == status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
