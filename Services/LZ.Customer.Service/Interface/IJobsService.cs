using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Customer.Service.Interface
{
    public interface IJobsService
    {
        Task<List<Model.Live_V_Jobs>> List(Expression<Func<Model.Live_V_Jobs, bool>> exp, PagedModelBase query);
        Task<List<Model.Live_V_Jobs>> Nearby(Expression<Func<Model.Live_V_Jobs, bool>> exp, PagedModelBase query, double latitude, double longitude, double radiusKm);

        Task<List<Model.Live_Jobs>> ListImport(Expression<Func<Model.Live_Jobs, bool>> exp, PagedModelBase query);
        Task<List<string>> GetCity(int companyId);
        Task<int> Add(Model.Live_Jobs model);

        Task<bool> Update(Model.Live_Jobs model);
        Task<bool> UpdateList(List<Model.Live_Jobs> list);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id,int companyId=0);
        Task<bool> ChangeStatus(int id, int status);
        Task<bool> ChangeIntegral(int jobId, int integral);
        Task<Live_UserJobQrcode> GetUserQrcode(int id, int userId);
        Task<int> AddUserQrcode(Live_UserJobQrcode model);
        Task<Model.Live_Jobs> Get(int id);
        Task<Model.Live_V_Jobs> GetView(int id);
        Task<bool> Set(int id, string filed, bool enabled);
    }
}
