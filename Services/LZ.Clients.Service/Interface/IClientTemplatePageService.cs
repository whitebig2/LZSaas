using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IClientTemplatePageService
    {
        Task<List<Model.Live_ClientTemplatePage>> List(Expression<Func<Model.Live_ClientTemplatePage, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_ClientTemplatePage model);

        Task<bool> Update(Model.Live_ClientTemplatePage model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<Model.Live_ClientTemplatePage> Get(int id);
    }
}
