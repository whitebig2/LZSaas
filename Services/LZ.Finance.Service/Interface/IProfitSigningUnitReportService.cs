using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IProfitSigningUnitReportService
    {
        Task<int> Add(Live_ProfitSigningUnitReport model);
        Task<int> Add(List<Live_ProfitSigningUnitReport> models);
        Task<bool> Delete(Guid id);
        Task<Live_ProfitSigningUnitReport> Get(Expression<Func<Live_ProfitSigningUnitReport, bool>> exp);
        Task<Live_ProfitSigningUnitReportExtension> GetDomain(Expression<Func<Live_ProfitSigningUnitReportExtension, bool>> exp);
        Task<List<Live_ProfitSigningUnitReport>> GroupBySigningUnit(DateTime billMonth);
        Task<List<Live_ProfitSigningUnitReport>> List(Expression<Func<Live_ProfitSigningUnitReport, bool>> exp);
        Task<List<Live_ProfitSigningUnitReport>> PageList(Expression<Func<Live_ProfitSigningUnitReport, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_ProfitSigningUnitReport model);
    }
}
