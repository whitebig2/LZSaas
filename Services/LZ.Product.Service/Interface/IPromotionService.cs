using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface IPromotionService
    {
        Task<List<Model.Shop_Promotion>> List(Expression<Func<Model.Shop_Promotion, bool>> exp, PagedModelBase query);
        Task<List<Shop_V_PromotionProduct>> Items(Expression<Func<Shop_V_PromotionProduct, bool>> exp, PagedModelBase query=null);
        Task<int> Add(Model.Shop_Promotion model);
        Task<int> AddItems(List<Shop_PromotionProduct> models);
        Task<bool> UpdateAsync(Shop_Promotion model);
        Task<bool> UpdateItems(List<Shop_PromotionProduct> models, int promotionId, Guid promotionGid);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Promotion, bool>> exp);
        Task<Shop_Promotion> Get(Expression<Func<Model.Shop_Promotion, bool>> exp);
        Task<Model.Shop_Promotion> Get(int id);
    }
}
