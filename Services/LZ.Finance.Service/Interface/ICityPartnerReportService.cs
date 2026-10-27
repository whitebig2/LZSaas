using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface ICityPartnerReportService
    {
        Task<Live_V_CityPartnerReport> GetView(Expression<Func<Live_V_CityPartnerReport, bool>> exp);
        Task<List<Live_V_CityPartnerReport>> ListView(Expression<Func<Live_V_CityPartnerReport, bool>> exp = null);
        Task<List<Live_V_CityPartnerReport>> PageList(Expression<Func<Live_V_CityPartnerReport, bool>> exp, PagedModelBase query);
    }
}
