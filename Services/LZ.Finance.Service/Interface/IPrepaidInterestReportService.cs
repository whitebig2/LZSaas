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
    public interface IPrepaidInterestReportService
    {
        Task<bool> Add(Live_PrepaidInterestReport model);
        Task<bool> Add(List<Live_PrepaidInterestReport> models);
        Task<bool> Delete(Guid id);
        Task<bool> Delete(Expression<Func<Live_PrepaidInterestReport, bool>> exp);
        Task<Live_PrepaidInterestReport> Get(Expression<Func<Live_PrepaidInterestReport, bool>> exp);
        Task<List<Live_PrepaidInterestReport>> List(Expression<Func<Live_PrepaidInterestReport, bool>> exp);
        Task<List<Live_V_PrepaidInterestReport>> ListView(Expression<Func<Live_V_PrepaidInterestReport, bool>> exp);
        Task<List<Live_V_PrepaidInterestReport>> PageList(Expression<Func<Live_V_PrepaidInterestReport, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PrepaidInterestReport model);
    }
}
