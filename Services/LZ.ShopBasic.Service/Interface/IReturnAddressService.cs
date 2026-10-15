using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IReturnAddressService
    {
        Task<List<Model.Shop_ReturnAddress>> List(Expression<Func<Model.Shop_ReturnAddress, bool>> exp);

        Task<int> Add(Model.Shop_ReturnAddress model);

        Task<bool> Update(Model.Shop_ReturnAddress model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_ReturnAddress, bool>> exp);
        Task<Model.Shop_ReturnAddress> Get(int id);
    }
}
