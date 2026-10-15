using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Finance.Service.Interface
{
    public interface IProfitSummaryService
    {
        Task<List<Live_ProfitSummary>> List(Expression<Func<Live_ProfitSummary, bool>> exp, PagedModelBase query);


        Task<int> Add(Live_ProfitSummary model);
        Task<int> Add(List<Live_ProfitSummary> models);

        Task<bool> Update(Live_ProfitSummary model);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Live_ProfitSummary, bool>> exp);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_ProfitSummary, bool>> exp);

        Task<Live_ProfitSummary> Get(int id);
        Task<Live_ProfitSummary> Get(Expression<Func<Live_ProfitSummary, bool>> exp);
    }
}
