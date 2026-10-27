using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface IProductTypesService
    {
        Task<List<Model.Shop_ProductTypes>> List(Expression<Func<Model.Shop_ProductTypes, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Shop_ProductTypes model);

        Task<bool> UpdateAsync(Model.Shop_ProductTypes model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_ProductTypes, bool>> exp);
        Task<Model.Shop_ProductTypes> Get(int id);
    }
}
