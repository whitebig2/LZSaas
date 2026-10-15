using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface IShoppingCartService
    {
        Task<List<Model.Shop_V_ShoppingCart>> List(Expression<Func<Model.Shop_V_ShoppingCart, bool>> exp, PagedModelBase query);
        Task<List<Model.Shop_ShoppingCart>> List(Expression<Func<Model.Shop_ShoppingCart, bool>> exp);

        Task<int> Add(Model.Shop_ShoppingCart model);
        Task<int> CartCount(Expression<Func<Model.Shop_V_ShoppingCart, bool>> exp);
        Task<bool> UpdateCount(int id, int count);
        Task<bool> UpdateCheck(int id, bool check);
        Task<bool> UpdateAllCheck(int userId, bool check);
        Task<bool> UpdateIncrement(int productId,string skuId,int userId);
        Task<bool> Delete(int id, int userId);
        Task<bool> Delete(string ids, int userId);
        Task<bool> Exist(Expression<Func<Model.Shop_ShoppingCart, bool>> exp);
    }
}
