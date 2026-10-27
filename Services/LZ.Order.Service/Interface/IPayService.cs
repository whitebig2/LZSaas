using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Order.Service.Interface
{
    public interface IPayService
    {
        Task<Model.RequestResultModel> WeiXinPay(Model.Shop_Order model,string appId="");
        Task<bool> WeiXinRefund(Model.Shop_Order model);
        Task<Model.RequestResultModel> CloseOrder(Shop_Order model, string appId = "");
    }
}
