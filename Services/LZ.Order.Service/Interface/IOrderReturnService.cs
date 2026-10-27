using LZ.Model;
using LZ.Model.ApiModel;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Order.Service.Interface
{
    public interface IOrderReturnService
    {
        Task<List<Model.Shop_OrderReturn>> List(Expression<Func<Model.Shop_OrderReturn, bool>> exp, PagedModelBase query);

        Task<bool> Submit(Shop_OrderReturn model, List<Shop_OrderReturnItems> items);

        Task<bool> UpdateAsync(Model.Shop_OrderReturn model);
        Task<bool> Cancel(string returnId, int userId = 0, string fullName = "", bool isAudit = false, string reason = "");
        Task<bool> ReturnAudit(string returnId, int status, string reason,int returnAddressId);
        Task<int> Count(Expression<Func<Model.Shop_OrderReturn, bool>> exp);
        Task<bool> Refund(Shop_OrderReturn model);
        Task<bool> Finish(string returnId, int companyId, bool isDirect);
        Task<bool> Exist(Expression<Func<Model.Shop_OrderReturn, bool>> exp);
        Task<Model.Shop_OrderReturn> Get(string id);
        Task<List<Shop_V_OrderReturnItems>> Items(string id);
        Task<List<Shop_V_OrderReturnItems>> Items(Expression<Func<Shop_V_OrderReturnItems, bool>> exp);
        Task<Model.Shop_OrderReturn> Get(Expression<Func<Model.Shop_OrderReturn, bool>> exp);
        /// <summary>
        /// 推送退单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HupunResultModel?> SendOrderReturn(Shop_OrderReturn model, string returnId = "");
    }
}
