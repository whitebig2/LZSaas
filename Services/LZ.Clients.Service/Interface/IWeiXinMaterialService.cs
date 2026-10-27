using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Clients.Service.Interface
{
    public interface IWeiXinMaterialService
    {
        Task<List<Model.Live_WeiXinMaterial>> List(Expression<Func<Model.Live_WeiXinMaterial, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_WeiXinMaterial model);
        Task<bool> Update(Model.Live_WeiXinMaterial model);
        Task<bool> Delete(int id);
        Task<Model.Live_WeiXinMaterial> Get(int id);
        Task<Model.Live_WeiXinMaterial> Get(Expression<Func<Model.Live_WeiXinMaterial, bool>> exp);
    }
}
