using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Company.Service.Interface
{
    public interface ILeaveReasonService
    {
        Task<List<Model.Live_LeaveReason>> List(Expression<Func<Model.Live_LeaveReason, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_LeaveReason model);

        Task<bool> Update(Model.Live_LeaveReason model);
        Task<bool> Delete(int id, int companyId);
        Task<bool> Exist(int id);
        Task<Model.Live_LeaveReason> Get(int id);
    }
}
