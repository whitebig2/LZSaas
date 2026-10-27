using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Clients.Service.Interface
{
    public interface IClientMyTemplateService
    {
        Task<List<Model.Live_ClientMyTemplate>> List(Expression<Func<Model.Live_ClientMyTemplate, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_ClientMyTemplate model);
        Task<int> AddToClient(int id, int tempID=0, string appType = "xfz");
        Task<bool> Update(Model.Live_ClientMyTemplate model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<Model.Live_ClientMyTemplate> Get(int id);
        Task<Live_ClientMyTemplate> GetDefault(int clientId);
    }
}
