using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface IAttributeService
    {
        Task<List<Model.Shop_Attribute>> List(Expression<Func<Model.Shop_Attribute, bool>> exp);

        Task<int> Add(Model.Shop_Attribute model);
        Task<int> Add(Model.Shop_Attribute model,List<Shop_AttributeValues> items);

        Task<bool> UpdateAsync(Model.Shop_Attribute model);
        Task<bool> Delete(int id);
        Task<bool> DeleteType(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Attribute, bool>> exp);
        Task<Model.Shop_Attribute> Get(int id);
    }
}
