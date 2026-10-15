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
using NPOI.SS.Formula.Functions;

namespace LZ.Finance.Service.Services
{
    public class CityPartnerEmpRecordService : ServicesBase<Live_CityPartnerEmpRecord>, ICityPartnerEmpRecordService, IScopedDependency
    {
        public async Task<List<Live_CityPartnerEmpRecord>> PageList(Expression<Func<Live_CityPartnerEmpRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_CityPartnerEmpRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public Task<List<Live_CityPartnerEmpRecord>> List(Expression<Func<Model.Live_CityPartnerEmpRecord, bool>> exp = null)
        {
            return   base.Context.Queryable<Live_CityPartnerEmpRecord>().Where(exp).ToListAsync();
        }
        public async Task<int> Add(Live_CityPartnerEmpRecord model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_CityPartnerEmpRecord> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }
        
        public async Task<bool> Update(Live_CityPartnerEmpRecord model)
        {
            return await   base.Context.Updateable<Live_CityPartnerEmpRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_CityPartnerEmpRecord> Get(Expression<Func<Live_CityPartnerEmpRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_CityPartnerEmpRecord>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await   base.Context.Deleteable<Live_CityPartnerEmpRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(Expression<Func<Model.Live_CityPartnerEmpRecord, bool>> exp = null)
        {
            return await   base.Context.Deleteable<Live_CityPartnerEmpRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }

    }
}
