using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IWeiXinMenuService
    {
        Task<List<Model.Live_WeiXinMenu>> List(Expression<Func<Model.Live_WeiXinMenu, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_WeiXinMenu model);

        Task<bool> Update(Model.Live_WeiXinMenu model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Model.Live_WeiXinMenu, bool>> exp);
        Task<Model.Live_WeiXinMenu> Get(int id);
    }
}
