using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Order.Service.Interface;
using LZ.Order.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Text;

namespace LZ.Order.API
{
    public class OrderReturnController(IOrderReturnService orderReturnService,IOrderLogService orderLogService,IOrderService orderService) : OrderControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] OrderPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_OrderReturn>();

            SetSearch(expable, query);
            var exp = expable.ToExpression();
            var models = await orderReturnService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        private void SetSearch(Expressionable<Model.Shop_OrderReturn> expable, OrderPagedModel query)
        {
            var user = CurrentUser;
            if (!string.IsNullOrEmpty(query.ProductName))
            {
                expable.And(it => SqlFunc.Subqueryable<Model.Shop_V_OrderReturnItems>().Where(s => s.ReturnID == it.ReturnID && s.ProductName.Contains(query.ProductName)).Any());
            }
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                if (query.CompanyID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CompanyID);
                }
            }
            else if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            else
            {
                expable.And(it => it.UserID == user.UserID);
            }
            if (!string.IsNullOrEmpty(query.TimeType) && (query.StartTime.HasValue || query.EndTime.HasValue))
            {
                var endTime = query.EndTime.HasValue ? query.EndTime.Value.AddDays(1) : query.EndTime;
                switch (query.TimeType)
                {
                    case "return":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.CreateTime >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.CreateTime < endTime);
                        }
                        break;
                    case "finish":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.ReturnDate >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.ReturnDate < endTime);
                        }
                        break;
                }

            }
            expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
            expable.AndIF(!string.IsNullOrEmpty(query.OrderID), it => it.OrderID == query.OrderID);
            expable.AndIF(!string.IsNullOrEmpty(query.ReturnID), it => it.ReturnID == query.ReturnID);
            expable.AndIF(!string.IsNullOrEmpty(query.Phone), it => it.ReceiverMobile == query.Phone);
        }
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] SubmitOrderReturn model)
        {
            var user = CurrentUser;
            if(model==null || model.Order==null || string.IsNullOrEmpty(model.Order.OrderID))
            {
                throw new ErrorCodeException(-1, "提交错误！");
            }
            var order = await orderService.Get(model.Order.OrderID);
            if (order==null || order.UserID!=user.UserID)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            if (!order.HasRefund)
            {
                throw new ErrorCodeException(-1, "该订单已关闭退单！");
            }
            if(order.OrderStatus!=(int)OrderStatus.HaveShip && order.OrderStatus != (int)OrderStatus.WaitReceipt)
            {
                throw new ErrorCodeException(-1, "该订单状态不可发起退单！");
            }
            var statuss = new int[] {(int)OrderReturnStatus.WaitFailure, (int)OrderReturnStatus.WaitReceipt, (int)OrderReturnStatus.RefundFailure, (int)OrderReturnStatus.WaitAuto, (int)OrderReturnStatus.WaitChange, (int)OrderReturnStatus.WaitExchange, (int)OrderReturnStatus.WaitReturn};
            if(await orderReturnService.Exist(it=>it.OrderID==order.OrderID && statuss.Contains(it.Status.Value)))
            {
                throw new ErrorCodeException(-1, "有一笔该订单正在售后中，请等待处理！");
            }
            var orderItems = new List<Shop_V_OrderItems>();
            model.Order.ReturnMoney = 0;
            model.Order.ReturnIntegral = 0;
            model.Order.ReturnTotal = 0;
            if (model.Items == null)
            {
                model.Items=new List<Shop_OrderReturnItems> ();
                orderItems = await orderService.Items(it=>it.OrderID==order.OrderID);
                foreach(var orderItem in orderItems)
                {
                    if (!orderItem.RefundCount.HasValue) orderItem.RefundCount = 0;
                    if (orderItem.RefundCount< orderItem.ProductCount)
                    {
                        var item = new Shop_OrderReturnItems();
                        item.SKU = orderItem.SKU;
                        item.OrderItemID = orderItem.ID;
                        item.ProductCount = orderItem.ProductCount- orderItem.RefundCount;
                        item.ProductID = orderItem.ProductID;
                        item.ProductName = orderItem.ProductName;
                        item.ProductPrice= orderItem.SalePrice;
                        item.SkuContent = orderItem.SkuContent;
                        item.SkuID= orderItem.SkuID;
                        item.RefundMoney = item.ProductCount.Value * item.ProductPrice;
                        if(orderItem.PayCash>0 || orderItem.PayIntegral.HasValue)//新版本将支付现金和积分分配到订单明细中，按照新规则退款
                        {
                            if (orderItem.PayCash > 0)
                            {
                                item.RefundCash = (orderItem.PayCash / orderItem.ProductCount.Value) * item.ProductCount.Value;
                            }
                            if (orderItem.PayIntegral.HasValue && orderItem.PayIntegral > 0)
                            {
                                item.RefundIntegral = (orderItem.PayIntegral / orderItem.ProductCount.Value) * item.ProductCount.Value;
                            }
                        }
                        if (orderItem.CouponMoney > 0)
                        {
                            var couponMoney = (orderItem.CouponMoney / orderItem.ProductCount.Value) * item.ProductCount.Value;
                            item.RefundMoney = item.RefundMoney - couponMoney;
                        }
                        model.Order.ReturnTotal += item.RefundMoney;
                        model.Order.ReturnMoney += item.RefundCash;
                        model.Order.ReturnIntegral += item.RefundIntegral;
                        model.Items.Add(item);
                    }
                }
            }
            else
            {
                var ids=model.Items.Select(x => x.OrderItemID).ToList();
                orderItems = await orderService.Items(it => ids.Contains(it.ID));
                foreach (var orderItem in orderItems)
                {
                    if (!orderItem.RefundCount.HasValue) orderItem.RefundCount = 0;
                    if (orderItem.RefundCount < orderItem.ProductCount)
                    {
                        var item = model.Items.Find(it => it.OrderItemID == orderItem.ID);
                        if(orderItem.ProductCount - orderItem.RefundCount < item.ProductCount)
                        {
                            throw new ErrorCodeException(-1, $"{orderItem.ProductName}可退数量不足！");
                        }
                        item.SKU = orderItem.SKU;
                        item.ProductID = orderItem.ProductID;
                        item.ProductName = orderItem.ProductName;
                        item.ProductPrice = orderItem.SalePrice;
                        item.SkuContent = orderItem.SkuContent;
                        item.SkuID = orderItem.SkuID;
                        item.RefundMoney = item.ProductCount.Value * item.ProductPrice;
                        if (orderItem.PayCash > 0 || orderItem.PayIntegral.HasValue)//新版本将支付现金和积分分配到订单明细中，按照新规则退款
                        {
                            if (orderItem.PayCash > 0)
                            {
                                item.RefundCash = (orderItem.PayCash / orderItem.ProductCount.Value) * item.ProductCount.Value;
                            }
                            if (orderItem.PayIntegral.HasValue && orderItem.PayIntegral > 0)
                            {
                                item.RefundIntegral = (orderItem.PayIntegral / orderItem.ProductCount.Value) * item.ProductCount.Value;
                            }
                        }
                        if (orderItem.CouponMoney > 0)
                        {
                            var couponMoney = (orderItem.CouponMoney / orderItem.ProductCount.Value) * item.ProductCount.Value;
                            item.RefundMoney = item.RefundMoney - couponMoney;
                        }
                        model.Order.ReturnTotal += item.RefundMoney;
                        model.Order.ReturnMoney += item.RefundCash;
                        model.Order.ReturnIntegral += item.RefundIntegral;
                    }
                }
            }

            if (model.Items.Count == 0)
            {
                throw new ErrorCodeException(-1, "该订单无商品可退！");
            }
            //var refundMoney = model.Items.Sum(it => it.RefundMoney);
            if (model.Order.ReturnTotal > order.OrderTotal)
            {
                throw new ErrorCodeException(-1, "退单金额不对！");
            }
            model.Order.ReturnPayType = order.PayType;
            //model.Order.ReturnTotal = refundMoney;
            //if (order.Cash > 0)
            //{
            //    if (order.Integral == 0)
            //    {
            //        model.Order.ReturnMoney = refundMoney;
            //        model.Order.ReturnIntegral = 0;
            //    }
            //    else
            //    {
            //        var prop = refundMoney / order.ProductTotal;//获得分摊比例
            //        var refundIntegral = order.Integral * prop;
            //        model.Order.ReturnIntegral = (int)refundIntegral;
            //        model.Order.ReturnMoney = refundMoney - model.Order.ReturnIntegral.Value / 10M;
            //    }
            //}
            //else
            //{
            //    model.Order.ReturnIntegral = order.Integral;
            //    model.Order.ReturnMoney = 0;
            //}
            model.Order.UserID = user.UserID;
            model.Order.ReturnID = Common.Tool.RandomName() + Common.Tool.GetRandomPureNumber(4);
            model.Order.CompanyGID=order.CompanyGID;
            model.Order.CompanyID = order.CompanyID;
            if(await orderReturnService.Submit(model.Order, model.Items))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderReturnStatus.WaitAuto;
                log.FullName = user.FullName;
                log.UserID = user.UserID;
                log.SysUserID = 0;
                log.OrderID = model.Order.ReturnID;
                log.Remark = "提交退单";
                log.LogType = 2;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "提交失败！");
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Shop_OrderReturn data)
        {
            if (data == null)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            var model = await orderReturnService.Get(data.ReturnID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该退单不存在");
            }
            var user = CurrentUser;
            if(model.UserID!= user.UserID)
            {
                throw new ErrorCodeException(-1, "您无法修改");
            }
            model.Pictures = data.Pictures;
            model.ReturnReason= data.ReturnReason;
            await orderReturnService.UpdateAsync(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Refund([FromBody] Shop_OrderReturn data)
        {
            if (data == null)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            var model = await orderReturnService.Get(data.ReturnID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该退单不存在");
            }
            var user = CurrentUser;
            if (model.UserID != user.UserID)
            {
                throw new ErrorCodeException(-1, "您无法修改");
            }
            model.ExpressName = data.ExpressName;
            model.ExpressNo = data.ExpressNo;
            model.ExpressType = data.ExpressType;
            await orderReturnService.Refund(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(string returnId)
        {
            var model = await orderReturnService.Get(returnId);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的退单不存在");
            }
            var items = await orderReturnService.Items(returnId);
            return Ok(new { order = model, items });
        }
        [HttpPost]
        public async Task<bool> Cancel([FromForm] string returnId)
        {
            var user = CurrentUser;
            var userId = 0;
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                userId = user.UserID;
            }
            if(await orderReturnService.Cancel(returnId, userId))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderReturnStatus.Cancel;
                if (userId > 0)
                {
                    log.FullName = user.FullName;
                    log.UserID = user.UserID;
                    log.Remark = "用户取消退单";
                }
                else
                {
                    log.SysUserID = user.UserID;
                    log.SysFullName = user.FullName;
                    log.Remark = "后台取消退单";
                }
                log.OrderID = returnId;
                log.LogType = 2;
                await orderLogService.Add(log);
            }
            return true;
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> SyncErp([FromForm] string returnId)
        {
            var res = await orderReturnService.SendOrderReturn(null, returnId);
            if (res != null && res.code == 0 && res.data.success)
            {
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, res.data.error_msg);
            }
        }
        [HttpPost]
        public async Task<bool> Audit([FromForm] string returnId, [FromForm] int status, [FromForm] string reason, [FromForm] int returnAddressId=0)
        {
            var user = CurrentUser;
            var has = await orderReturnService.ReturnAudit(returnId, status, reason, returnAddressId);
            if (has)
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.SysUserID = user.UserID;
                log.SysFullName = user.FullName;
                log.Remark = "后台审核退单;"+(status!=3?"原因:"+ reason : "");
                log.OrderID = returnId;
                log.LogType = 2;
                await orderLogService.Add(log);
            }
            return false;
        }
        [HttpPost]
        public async Task<IActionResult> Direct([FromForm] string returnId)
        {
            var user = CurrentUser;
            if (await orderReturnService.Finish(returnId, user.CompanyID, true))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderReturnStatus.RefundFinish;
                log.SysUserID = user.UserID;
                log.SysFullName = user.FullName;
                log.Remark = "后台直接退单";
                log.OrderID = returnId;
                log.LogType = 2;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "直接退单错误！");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Finish([FromForm] string returnId)
        {
            var user = CurrentUser;
            if (await orderReturnService.Finish(returnId, user.CompanyID,false))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderReturnStatus.RefundFinish;
                log.SysUserID = user.UserID;
                log.SysFullName = user.FullName;
                log.Remark = "后台完成退单";
                log.OrderID = returnId;
                log.LogType = 2;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "完成错误，只有待收货才可完成！");
            }
        }
    }
}
