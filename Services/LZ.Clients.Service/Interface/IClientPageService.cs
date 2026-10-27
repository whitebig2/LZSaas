using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IClientPageService
    {
        Task<List<Model.Live_V_ClientPage>> List(Expression<Func<Model.Live_V_ClientPage, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_ClientPage model);
        Task<int> Add(List<Live_ClientPage> models);
        Task<bool> Update(Model.Live_ClientPage model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<Model.Live_ClientPage> GetPage(int id, string type);
        Task<Model.Live_ClientPage> Get(int id);
        Task<Model.Live_V_ClientPage> GetView(int id);
    }
}
