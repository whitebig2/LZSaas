using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.PagedModel;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Services
{


    public class BusInsuranceRecordService : ServicesBase<Live_BusInsuranceRecord>, IBusInsuranceRecordService, IScopedDependency
    {
        public async Task<List<Live_BusInsuranceRecord>> PageList(Expression<Func<Live_BusInsuranceRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusInsuranceRecord>> List(Expression<Func<Live_BusInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusInsuranceRecord> Get(Expression<Func<Live_BusInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Update(Live_BusInsuranceRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Add(Live_BusInsuranceRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Add(List<Live_BusInsuranceRecord> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_BusInsuranceRecord> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task Delete(Expression<Func<Live_BusInsuranceRecord, bool>> exp)
        {
            await base.Context.Deleteable<Live_BusInsuranceRecord>().Where(exp).ExecuteCommandAsync();
        }

        public async Task<int> UpdateRecordMatchStatus(List<Live_BusInsuranceRecord> models)
        {
            return await base.Context.Updateable<Live_BusInsuranceRecord>(models).UpdateColumns(f => new { f.Status, f.MatchDate }).ExecuteCommandAsync();
        }



    }
}
