using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IClientTemplateService
    {
        Task<List<Model.Live_ClientTemplate>> List(Expression<Func<Model.Live_ClientTemplate, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_ClientTemplate model);

        Task<bool> Update(Model.Live_ClientTemplate model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<Model.Live_ClientTemplate> Get(int id);
        Task<Model.Live_ClientTemplate> Get(Expression<Func<Model.Live_ClientTemplate, bool>> exp);
    }
}
