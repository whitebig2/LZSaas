using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface IAttributeValuesService
    {
        Task<List<Model.Shop_AttributeValues>> List(Expression<Func<Model.Shop_AttributeValues, bool>> exp);

        Task<int> Add(Model.Shop_AttributeValues model);

        Task<int> Add(List<Model.Shop_AttributeValues> models);

        Task<bool> UpdateAsync(Model.Shop_AttributeValues model);
        Task<bool> Delete(int id);
        Task<bool> AttributeDelete(int attributeId);
        Task<bool> AttributeTypeDelete(int typeId);
        Task<bool> Exist(Expression<Func<Model.Shop_AttributeValues, bool>> exp);
        Task<Model.Shop_AttributeValues> Get(int id);
    }
}
