using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IAdvertService
    {
        Task<List<Model.Shop_Advert>> List(Expression<Func<Model.Shop_Advert, bool>> exp, bool orderBySort = false);

        Task<int> Add(Model.Shop_Advert model);

        Task<bool> Update(Model.Shop_Advert model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Advert, bool>> exp);
        Task<Model.Shop_Advert> Get(int id);
        Task<Model.Shop_Advert> Get(Expression<Func<Model.Shop_Advert, bool>> exp);
    }
}
