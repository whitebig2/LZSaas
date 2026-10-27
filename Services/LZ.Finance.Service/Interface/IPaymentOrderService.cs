using LZ.Model;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPaymentOrderService
    {
        /// <summary>
        /// 新增支付订单：按签约单位名称匹配默认付款账户
        /// </summary>
        Task<Live_PaymentOrderExtension> Add(PaymentOrderAddDto dto);
        /// <summary>
        /// 根据预览支付订单落库，付款账户及机构信息以账户为准
        /// </summary>
        Task<Live_PaymentOrderExtension> AddFromPre(PrePaymentOrderDto dto);
        Task<bool> AddNav(Live_PaymentOrderExtension model);
        Task<bool> AddNav(List<Live_PaymentOrderExtension> models);
        Task<bool> DeleteNav(Guid id);
        Task<bool> Exist(Expression<Func<Live_PaymentOrder, bool>> exp);
        Task<Live_PaymentOrder> Get(Expression<Func<Live_PaymentOrder, bool>> exp);
        Task<Live_PaymentOrderExtension> GetNav(Expression<Func<Live_PaymentOrderExtension, bool>> exp);
        Task<List<Live_PaymentOrder>> List(Expression<Func<Live_PaymentOrder, bool>> exp);
        Task<List<Live_PaymentOrderExtension>> ListNav(Expression<Func<Live_PaymentOrderExtension, bool>> exp);
        Task<List<Live_PaymentOrder>> PageList(Expression<Func<Live_PaymentOrder, bool>> exp, PagedModelBase query);
        Task<List<Live_PaymentOrderDetail>> PageDetailList(Expression<Func<Live_PaymentOrderDetail, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_PaymentOrderExtension model);
        Task<bool> UpdateNav(Live_PaymentOrderExtension model);
        /// <summary>
        /// 提交支付交易：校验后推送到银行，并回写订单状态
        /// </summary>
        Task<bool> SubmitPay(Live_PaymentOrderExtension model);
        /// <summary>
        /// 订单作废：待支付、支付失败可作废；客户工资来源回滚推送标记
        /// </summary>
        Task<bool> Cancel(Live_PaymentOrderExtension model);
        /// <summary>
        /// 推送到银行 CzyPayOrder 接口
        /// </summary>
        Task<(bool Success, string Message)> PushToBank(Live_PaymentOrderExtension model);
        /// <summary>
        /// 财资云推送结果通知：成功待核对，失败置为支付失败
        /// </summary>
        Task<bool> HandlePushNotice(PaymentOrderPushNoticeDto dto);
    }
}
