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
    public class CityPartnerSalaryRecordService : ServicesBase<Live_CityPartnerSalaryRecord>, ICityPartnerSalaryRecordService, IScopedDependency
    {
        public async Task<List<Live_CityPartnerSalaryRecord>> PageList(Expression<Func<Live_CityPartnerSalaryRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_CityPartnerSalaryRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public Task<List<Live_CityPartnerSalaryRecord>> List(Expression<Func<Model.Live_CityPartnerSalaryRecord, bool>> exp = null)
        {
            return   base.Context.Queryable<Live_CityPartnerSalaryRecord>().Where(exp).ToListAsync();
        }

        public Task<int> RecordCount(Expression<Func<Model.Live_CityPartnerSalaryRecord, bool>> exp = null)
        {
            return   base.Context.Queryable<Live_CityPartnerSalaryRecord>().Where(exp).CountAsync();
        }
        public async Task<int> Add(Live_CityPartnerSalaryRecord model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_CityPartnerSalaryRecord> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }
        
        public async Task<bool> Update(Live_CityPartnerSalaryRecord model)
        {
            return await   base.Context.Updateable<Live_CityPartnerSalaryRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_CityPartnerSalaryRecord> Get(Expression<Func<Live_CityPartnerSalaryRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_CityPartnerSalaryRecord>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await   base.Context.Deleteable<Live_CityPartnerSalaryRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Model.Live_CityPartnerSalaryRecord, bool>> exp = null)
        {
            return await   base.Context.Deleteable<Live_CityPartnerSalaryRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
