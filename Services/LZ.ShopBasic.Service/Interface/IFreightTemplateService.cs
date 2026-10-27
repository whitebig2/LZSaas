using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IFreightTemplateService
    {
        Task<List<Model.Shop_FreightTemplate>> List(Expression<Func<Model.Shop_FreightTemplate, bool>> exp);

        Task<int> Add(Model.Shop_FreightTemplate model);

        Task<bool> Update(Model.Shop_FreightTemplate model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_FreightTemplate, bool>> exp);
        Task<Model.Shop_FreightTemplate> Get(int id);
    }
}
