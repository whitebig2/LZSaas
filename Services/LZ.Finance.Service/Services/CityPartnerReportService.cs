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
    public class CityPartnerReportService : ServicesBase<Live_V_CityPartnerReport>, ICityPartnerReportService, IScopedDependency
    {
        public async Task<List<Live_V_CityPartnerReport>> PageList(Expression<Func<Live_V_CityPartnerReport, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_CityPartnerReport>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public Task<List<Live_V_CityPartnerReport>> ListView(Expression<Func<Model.Live_V_CityPartnerReport, bool>> exp = null)
        {
            return   base.Context.Queryable<Live_V_CityPartnerReport>().Where(exp).ToListAsync();
        }
        public async Task<Live_V_CityPartnerReport> GetView(Expression<Func<Live_V_CityPartnerReport, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_CityPartnerReport>().FirstAsync(exp);
        }
     
    }
}
