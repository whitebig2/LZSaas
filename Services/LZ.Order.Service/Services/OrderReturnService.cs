using Aliyun.OSS;
using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.ApiModel;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Order.Service.Interface;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using System.ComponentModel.Design;
using System.Linq.Expressions;


namespace LZ.Order.Service.Services
{
    public class OrderReturnService(IEventPublisher publisher,IPayService payService,IHttpService httpService,IHupunERPService hupunERPService,IOrderService orderService) : ServicesBase<Model.Shop_OrderReturn>, IOrderReturnService, IScopedDependency
    {
        public async Task<List<Shop_OrderReturn>> List(Expression<Func<Shop_OrderReturn, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_OrderReturn>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_OrderReturn>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="shoppingCart"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> Submit(Shop_OrderReturn model, List<Shop_OrderReturnItems> items)
        {
            model.CreateTime = DateTime.Now;
            model.Status = (int)OrderReturnStatus.WaitAuto;
            var orderItems = new List<Model.Shop_OrderItems>();
            foreach (var item in items)
            {
                item.ReturnID = model.ReturnID;
                var orderItem = new Shop_OrderItems();
                orderItem.IsRefund = true;
                orderItem.SkuID=item.SkuID;
                orderItem.ID = item.OrderItemID.Value;
                orderItem.RefundCount = item.ProductCount;
                orderItem.RefundAmount = item.RefundMoney;
                orderItems.Add(orderItem);
            }
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                await db.InsertableWithAttr(model).ExecuteReturnIdentityAsync();
                await db.InsertableWithAttr(items).ExecuteReturnIdentityAsync();
                await db.UpdateableWithAttr<Shop_Order>().SetColumns(it => it.RefundStatus == model.Status).Where(it => it.OrderID == model.OrderID).ExecuteCommandAsync();
                await db.UpdateableWithAttr(orderItems).PublicSetColumns(it => it.RefundCount, "+").PublicSetColumns(it => it.RefundAmount, "+").UpdateColumns(it => new { it.RefundCount,it.IsRefund,it.RefundAmount } ).ExecuteCommandAsync();
                return true;
            });
            return result.Data;
        }
        /// <summary>
        /// 取消退单
        /// </summary>
        /// <param name="returnId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> Cancel(string returnId, int userId=0,string fullName="",bool isAudit=false,string reason="")
        {
            var expable = Expressionable.Create<Model.Shop_OrderReturn>();
            expable.And(it => it.ReturnID == returnId);
            expable.AndIF(userId>0, it => it.UserID == userId);
            var model = await base.Context.Queryable<Shop_OrderReturn>().FirstAsync(expable.ToExpression());
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该售后订单不存在！");
            }
            if (model.Status != (int)OrderReturnStatus.WaitAuto && model.Status != (int)OrderReturnStatus.WaitReturn)
            {
                throw new ErrorCodeException(-1, "该售后订单不在待审核或待退货状态不可取消！");
            }
            var items = await base.Context.Queryable<Shop_OrderReturnItems>().Where(it => it.ReturnID == returnId).ToListAsync();
            var orderItems = new List<Model.Shop_OrderItems>();
            foreach (var item in items)
            {
                item.ReturnID = model.ReturnID;
                var orderItem = new Shop_OrderItems();
                orderItem.IsRefund = false;
                orderItem.SkuID = item.SkuID;
                orderItem.ID = item.OrderItemID.Value;
                orderItem.RefundCount = item.ProductCount;
                orderItem.RefundAmount = item.RefundMoney;
                orderItems.Add(orderItem);
            }
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                if (isAudit)
                {
                    await db.UpdateableWithAttr<Model.Shop_OrderReturn>().SetColumns(it => it.AuditReason == reason).SetColumns(it => it.Status == (int)OrderReturnStatus.NoPass).Where(it => it.ReturnID == returnId).ExecuteCommandAsync();
                }
                else
                {
                    await db.UpdateableWithAttr<Model.Shop_OrderReturn>().SetColumns(it => it.Status == (int)OrderReturnStatus.Cancel).Where(it => it.ReturnID == returnId).ExecuteCommandAsync();
                }
                await db.UpdateableWithAttr<Shop_Order>().SetColumns(it => it.RefundStatus == (int)OrderReturnStatus.Cancel).Where(it => it.OrderID == model.OrderID).ExecuteCommandAsync();
                await db.UpdateableWithAttr(orderItems).PublicSetColumns(it => it.RefundCount, "-").PublicSetColumns(it => it.RefundAmount, "-").UpdateColumns(it => new { it.RefundCount, it.IsRefund, it.RefundAmount }).ExecuteCommandAsync();
                return true;
            });
            if (result.Data)
            {
                model.Status = (int)OrderReturnStatus.Cancel;
                await SendOrderReturn(model);
            }
            return result.Data;
        }
        public async Task<int> Count(Expression<Func<Model.Shop_OrderReturn, bool>> exp)
        {
            return await base.Context.Queryable<Shop_OrderReturn>().CountAsync(exp);
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="returnId"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> ReturnAudit(string returnId, int status,string reason,int returnAddressId)
        {
            var model = await base.Context.Queryable<Shop_OrderReturn>().FirstAsync(it => it.ReturnID == returnId);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该售后订单不存在！");
            }
            if (model.Status != (int)OrderReturnStatus.WaitAuto)
            {
                throw new ErrorCodeException(-1, "该售后订单不在待审核状态不可审核！");
            }

            if (status == 3)//审核通过
            {
                if (model.ReturnType == 1)
                {
                    var payReturn = true;
                    if (model.ReturnMoney > 0)
                    {
                        var order = await orderService.Get(model.OrderID);
                        order.RefundID= returnId;
                        order.RefundMoney=model.ReturnMoney;
                        payReturn = await payService.WeiXinRefund(order);
                    }
                    if(payReturn)
                    {
                        if(model.ReturnIntegral > 0)
                        {
                            var record = new IntegralLogEventData();
                            record.UserID = model.UserID.Value;
                            record.Integral = model.ReturnIntegral.Value;
                            record.OrderID = model.OrderID;
                            record.IntegralType = IntegralType.CancelConsume;
                            record.IsUpdate = true;
                            record.Remark = "售后订单退单";
                            record.RefundID = returnId;
                            record.CompanyGID = model.CompanyGID.Value;
                            record.CompanyID = model.CompanyID.Value;
                            publisher.Publish(record);
                        }
                        var has = await base.Context.Updateable<Model.Shop_OrderReturn>().SetColumns(it => it.Status == (int)OrderReturnStatus.RefundFinish).Where(it => it.ReturnID == returnId).ExecuteCommandAsync() > 0;
                        if (has)
                        {
                            model.ReturnDate = DateTime.Now;
                            model.Status = (int)OrderReturnStatus.RefundFinish;
                            await orderService.ChangeReturnStatus(model.OrderID, model.Status.Value);
                            await SendOrderReturn(model);
                        }
                        return has;
                    }
                    return payReturn;
                }
                else if (model.ReturnType == 2)
                {
                    var has = await base.Context.Updateable<Model.Shop_OrderReturn>().SetColumns(it => it.Status == (int)OrderReturnStatus.WaitReturn).SetColumns(it=>it.ReturnAddressID== returnAddressId).Where(it => it.ReturnID == returnId).ExecuteCommandAsync() > 0;
                    if (has)
                    {
                        model.Status = (int)OrderReturnStatus.WaitReturn;
                        await orderService.ChangeReturnStatus(model.OrderID, model.Status.Value);
                        await SendOrderReturn(model);
                    }
                    return has;
                }
                else
                {
                    throw new ErrorCodeException(-1, "不支持换货！");
                }
            }
            else if (status == 2)//审核不通过
            {
                return await Cancel(returnId, isAudit: true, reason: reason);
            }
            else if(status == 1) //退回修改
            {
                return await base.Context.Updateable<Model.Shop_OrderReturn>().SetColumns(it=>it.AuditReason== reason).SetColumns(it => it.Status == (int)OrderReturnStatus.WaitChange).Where(it => it.ReturnID == returnId).ExecuteCommandAsync() > 0;
            }
            else
            {
                throw new ErrorCodeException(-1, "无法审核！");
            }
        }
        /// <summary>
        /// 完成退单/直接推荐
        /// </summary>
        /// <param name="returnId"></param>
        /// <param name="companyId"></param>
        /// /// <param name="isDirect">是否直接退单</param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> Finish(string returnId, int companyId,bool isDirect)
        {
            var expable = Expressionable.Create<Model.Shop_OrderReturn>();
            expable.And(it => it.ReturnID == returnId);
            expable.And(it => it.CompanyID == companyId);
            var model = await base.Context.Queryable<Shop_OrderReturn>().FirstAsync(expable.ToExpression());
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该退单不存在！");
            }
            if (isDirect)
            {
                var statuss = new int[] { (int)OrderReturnStatus.WaitAuto, (int)OrderReturnStatus.WaitChange, (int)OrderReturnStatus.WaitExchange, (int)OrderReturnStatus.WaitFailure, (int)OrderReturnStatus.WaitReceipt, (int)OrderReturnStatus.WaitReturn };
                if (!statuss.Contains(model.Status.Value))
                {
                    throw new ErrorCodeException(-1, "该退单不在可直接退款的状态！");
                }
            }
            else
            {
                if (model.Status != (int)OrderReturnStatus.WaitReceipt)
                {
                    throw new ErrorCodeException(-1, "该退单不在待收货状态，不可完成！");
                }
            }
           
            var payReturn = true;
            if (model.ReturnMoney > 0)
            {
                var order = await orderService.Get(model.OrderID);
                order.RefundID = returnId;
                order.RefundMoney = model.ReturnMoney;
                payReturn = await payService.WeiXinRefund(order);
            }
            var has = false;
            if (payReturn)
            {
                has = await base.Context.Updateable<Model.Shop_OrderReturn>().SetColumns(it => it.ReturnDate == DateTime.Now).SetColumns(it => it.Status == (int)OrderReturnStatus.RefundFinish).Where(it => it.ReturnID == returnId).ExecuteCommandAsync() > 0;
                if (has)
                {
                    await orderService.ChangeReturnStatus(model.OrderID, (int)OrderReturnStatus.RefundFinish);
                    model.ReturnDate = DateTime.Now;
                    model.Status = (int)OrderReturnStatus.RefundFinish;
                    if (model.ReturnIntegral > 0)
                    {
                        var record = new IntegralLogEventData();
                        record.UserID = model.UserID.Value;
                        record.Integral = model.ReturnIntegral.Value;
                        record.OrderID = model.OrderID;
                        record.IntegralType = IntegralType.CancelConsume;
                        record.IsUpdate = true;
                        record.Remark = "售后订单退单";
                        record.RefundID = returnId;
                        record.CompanyGID = model.CompanyGID.Value;
                        record.CompanyID = model.CompanyID.Value;
                        publisher.Publish(record);
                    }
                    await SendOrderReturn(model);
                }
            }
            return has;
        }
        /// <summary>
        /// 修改退单
        /// </summary>
        /// <param name="returnId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSync(Shop_OrderReturn model)
        {
            model.Status= (int)OrderReturnStatus.WaitAuto;
            return await base.Context.Updateable(model).UpdateColumns(it=>new  { it.ExpressName,it.ExpressNo,it.ExpressType,it.ReturnReason,it.Status,it.Pictures}).Where(it=>it.ReturnID==model.ReturnID).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 用户退回
        /// </summary>
        /// <param name="returnId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<bool> Refund(Shop_OrderReturn model)
        {
            model.Status = (int)OrderReturnStatus.WaitReceipt;
            var has= await base.Context.Updateable(model).UpdateColumns(it => new { it.ExpressName, it.ExpressNo, it.ExpressType,it.Status }).Where(it => it.ReturnID == model.ReturnID).ExecuteCommandAsync() > 0;
            if (has)
            {
                await orderService.ChangeReturnStatus(model.OrderID, model.Status.Value);
            }
            return has;
        }

        public async Task<HupunResultModel?> SendOrderReturn(Shop_OrderReturn model, string returnId = "")
        {
            try
            {
                if (model == null)
                {
                    model = await base.Context.Queryable<Shop_OrderReturn>().FirstAsync(it => it.ReturnID == returnId);
                }
                var items = await Items(model.ReturnID);
                var res = await hupunERPService.OrderReturn(model, items);
                if (res == null)
                {
                    return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = "对接错误" } };
                }
                if (res.code != 0 || !res.data.success)
                {
                    Common.StaticLog.Debug(JsonConvert.SerializeObject(res));
                }
                return res;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = ex.Message } };
            }

        }
        public async Task<bool> Exist(Expression<Func<Shop_OrderReturn, bool>> exp)
        {
            return await base.Context.Queryable<Shop_OrderReturn>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_OrderReturn model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_OrderReturn> Get(string id)
        {
            return await base.Context.Queryable<Shop_OrderReturn>().FirstAsync(it => it.ReturnID == id);
        }
        public async Task<List<Shop_V_OrderReturnItems>> Items(string id)
        {
            return await base.Context.Queryable<Shop_V_OrderReturnItems>().Where(it => it.ReturnID == id).ToListAsync();
        }
        public async Task<List<Shop_V_OrderReturnItems>> Items(Expression<Func<Shop_V_OrderReturnItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_OrderReturnItems>().Where(exp).ToListAsync();
        }
        public async Task<Shop_OrderReturn> Get(Expression<Func<Shop_OrderReturn, bool>> exp)
        {
            return await base.Context.Queryable<Shop_OrderReturn>().FirstAsync(exp);
        }
    }
}
