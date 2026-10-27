using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface ICategoriesService
    {
        Task<List<Model.Shop_Categories>> List(Expression<Func<Model.Shop_Categories, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Shop_Categories model);

        Task<bool> UpdateAsync(Model.Shop_Categories model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Categories, bool>> exp);
        Task<Model.Shop_Categories> Get(int id);
    }
}
