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
    public class PrepaidInterestReportService : ServicesBase<Live_PrepaidInterestReport>, IPrepaidInterestReportService, IScopedDependency
    {

        public async Task<List<Live_V_PrepaidInterestReport>> PageList(Expression<Func<Live_V_PrepaidInterestReport, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_PrepaidInterestReport>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PrepaidInterestReport>> List(Expression<Func<Live_PrepaidInterestReport, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_PrepaidInterestReport>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }

        public async Task<List<Live_V_PrepaidInterestReport>> ListView(Expression<Func<Live_V_PrepaidInterestReport, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_V_PrepaidInterestReport>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<bool> Add(Live_PrepaidInterestReport model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }
        public async Task<bool> Add(List<Live_PrepaidInterestReport> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandIdentityIntoEntityAsync();
        }

      

      
        public async Task<bool> Update(Live_PrepaidInterestReport model)
        {
            return await   base.Context.Updateable<Live_PrepaidInterestReport>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_PrepaidInterestReport> Get(Expression<Func<Live_PrepaidInterestReport, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidInterestReport>().Where(exp).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_PrepaidInterestReport>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_PrepaidInterestReport, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_PrepaidInterestReport>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
