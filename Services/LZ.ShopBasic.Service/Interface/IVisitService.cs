using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.ShopBasic.Service.Interface
{
    public interface IVisitService
    {
        Task<List<Model.Shop_Visit>> List(Expression<Func<Model.Shop_Visit, bool>> exp, PagedModelBase query);
        Task<int> Add(Model.Shop_Visit model);
        Task<bool> Update(Model.Shop_Visit model);
        Task<bool> Leave(int id);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Visit, bool>> exp);
        Task<Model.Shop_Visit> Get(int id);
        Task ExtractProductKeywords(Model.Shop_Visit model);
        Task<List<VisitSearchStat>> SearchStats(Expression<Func<Model.Shop_Visit, bool>> exp);
    }
}
