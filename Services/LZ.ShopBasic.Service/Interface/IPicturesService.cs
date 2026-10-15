using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IPicturesService
    {
        Task<List<Model.Shop_V_Pictures>> List(Expression<Func<Model.Shop_V_Pictures, bool>> exp);

        Task<int> Add(Model.Shop_Pictures model);
        Task<int> Add(List<Model.Shop_Pictures> models);

        Task<bool> Update(Model.Shop_Pictures model);
        Task<bool> Update(string[] ids,int assortID);
        Task<bool> Update(int pictureId, string name);
        Task<bool> Delete(int id);
        Task<bool> Delete(string[] ids);
        Task<bool> Exist(Expression<Func<Model.Shop_Pictures, bool>> exp);
        Task<Model.Shop_Pictures> Get(int id);
    }
}
