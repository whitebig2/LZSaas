using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Order.Service.Interface
{
    public interface IOrderLogService
    {
        Task<List<Model.Shop_OrderLog>> List(Expression<Func<Model.Shop_OrderLog, bool>> exp);

        Task<int> Add(Model.Shop_OrderLog model);
        Task<int> Add(List<Model.Shop_OrderLog> models);
    }
}
