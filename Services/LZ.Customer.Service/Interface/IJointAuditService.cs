using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface IJointAuditService
    {
        Task<List<Model.Live_V_JointAudit>> List(Expression<Func<Model.Live_V_JointAudit, bool>> exp, PagedModelBase query);
        Task<int> Add(Model.Live_JointAudit model);

        Task<bool> Update(Model.Live_JointAudit model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id,int companyId=0);
        Task<bool> ChangeStatus(int id, int status, string remarks);
        Task<bool> ChangePartner(int id, int partnerId, Guid partnerGID);
        Task<Model.Live_JointAudit> Get(int id);
    }
}
