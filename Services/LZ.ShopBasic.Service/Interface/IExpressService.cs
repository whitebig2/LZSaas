using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IExpressService
    {
        Task<List<Model.Shop_Express>> List(Expression<Func<Model.Shop_Express, bool>> exp);

        Task<int> Add(Model.Shop_Express model);

        Task<bool> Update(Model.Shop_Express model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Express, bool>> exp);
        Task<Model.Shop_Express> Get(int id);
        Task<List<Model.Shop_ExpressInfo>> InfoList(string expressNo);
    }
}
