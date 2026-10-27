using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IBrandService
    {
        Task<List<Model.Shop_Brand>> List(Expression<Func<Model.Shop_Brand, bool>> exp);

        Task<int> Add(Model.Shop_Brand model);

        Task<bool> Update(Model.Shop_Brand model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Brand, bool>> exp);
        Task<Model.Shop_Brand> Get(int id);
    }
}
