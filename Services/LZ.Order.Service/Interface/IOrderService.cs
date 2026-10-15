using LZ.Model;
using LZ.Model.ApiModel;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Order.Service.Interface
{
    public interface IOrderService
    {
        Task<List<Model.Shop_Order>> List(Expression<Func<Model.Shop_Order, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Shop_Order model);

        Task<bool> UpdateAsync(Model.Shop_Order model);
        Task<bool> Delete(string orderId);
        Task<bool> Cancel(string orderId, int userId=0, Model.Shop_Order model=null);
        Task<bool> Refund(string orderId,int userId = 0);
        Task<bool> ItemRefund(int id, int count, int userId = 0);
        Task<bool> Finish(string orderId, int userId = 0, Model.Shop_Order model = null);
        Task<bool> UpdateRemark(string orderId, string remark);
        Task<int> CountSync(Expression<Func<Model.Shop_Order, bool>> exp);
        /// <summary>
        /// 同步退单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<bool> UpdateRefundStatus(string orderId, int status);
        /// <summary>
        /// 修改评价状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<bool> UpdateAssess(string orderId);
        /// <summary>
        /// 修改评价状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<bool> ChangeReturnStatus(string orderId,int status);
        Task<bool> UpdateBatchAssess(Expression<Func<Model.Shop_Order, bool>> exp);
        Task<bool> UpdateItem(Shop_OrderItems model);
        Task<bool> UpdateOrderTotal(string orderId, decimal money);
        Task<bool> OrderPay(Shop_Order model);
        Task<bool> Exist(Expression<Func<Model.Shop_Order, bool>> exp);
        Task<List<Model.CountModel>> StatusCount(Expression<Func<Model.Shop_Order, bool>> exp);
        Task<bool> OrderSend(OrderSend model);
        /// <summary>
        /// ERP发货同步（支持多包裹）
        /// </summary>
        Task<ErpPackageSendResult> ErpPackageSend(HupunSendDataModel model);
        /// <summary>
        /// 接收快递轨迹推送，写入快递信息并回写订单/包裹
        /// </summary>
        Task<bool> ReceiveExpressInfo(ExpressPushModel model);
        Task<Model.Shop_Order> Get(string id);
        Task<List<OrderPackageModel>> PackageList(string orderId);
        Task<List<Shop_V_OrderItems>> Items(string id);
        Task<List<Shop_V_OrderItems>> Items(Expression<Func<Shop_V_OrderItems, bool>> exp, PagedModelBase query = null);
        Task<Shop_V_OrderItems> GetItem(Expression<Func<Shop_V_OrderItems, bool>> exp);
        Task<bool> ExistItems(Expression<Func<Shop_OrderItems, bool>> exp);
        Task<bool> Submit(Shop_Order model, ShoppingCart shoppingCart, Live_User user = null, Live_Personnel personnel = null,bool isIntegral=true);
        Task<Model.Shop_Order> Get(Expression<Func<Model.Shop_Order, bool>> exp);
        /// <summary>
        /// 推送订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <param name="has_refund">1为退款0为未退款</param>
        /// <returns></returns>
        Task<HupunResultModel?> SendOrder(Shop_Order model, string orderId = "", int status = 2, int has_refund = 0);
    }
}
