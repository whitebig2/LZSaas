using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IClientNoticeTemplateService
    {
        Task<List<Model.Live_V_ClientNoticeTemplate>> List(Expression<Func<Model.Live_V_ClientNoticeTemplate, bool>> exp, PagedModelBase query);

        Task<int> Add(List<Model.Live_ClientNoticeTemplate> models);
        Task<int> Add(Model.Live_ClientNoticeTemplate model);

        Task<bool> Update(Model.Live_ClientNoticeTemplate model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int clientId);
        Task<Model.Live_ClientNoticeTemplate> Get(int id);
        Task<Live_V_ClientNoticeTemplate> Get(int clientId, int noticeId);
    }
}
