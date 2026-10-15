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
using NPOI.HSSF.Record.Chart;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Tls;
using SqlSugar;
using System.Linq.Expressions;
using System.Text;


namespace LZ.Order.Service.Services
{
    public class OrderService(IEventPublisher publisher,IPayService payService,IHttpService httpService,IOrderLogService orderLogService,IHupunERPService hupunERPService) : ServicesBase<Model.Shop_Order>, IOrderService, IScopedDependency
    {
        public async Task<List<Shop_Order>> List(Expression<Func<Shop_Order, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_Order>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_Order>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
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
        public async Task<bool> Submit(Shop_Order model, ShoppingCart shoppingCart, Live_User user=null, Live_Personnel personnel=null,bool isIntegral=true)
        {
            
            model.OrderDate = DateTime.Now;
            model.OrderID = Common.Tool.RandomName()+Common.Tool.GetRandomPureNumber(4);
            model.FeeMoney = 0;
            model.OrderStatus = (int)OrderStatus.WaitPay;
            var cartResult = shoppingCart.CalculateAmount();
            model.ProductTotal = cartResult.ProductTotal;
            model.OrderTotal = cartResult.PayableAmount + model.FeeMoney;
            model.Cash = model.OrderTotal;
            model.PayType = PayType.Integral.ToString();
            model.OrderGID = Guid.NewGuid();
            model.CouponTotal= cartResult.CouponTotal;
            model.Commission = 0;
            if (user != null)
            {
                model.UserID = user.UserID;
                model.FullName = user.FullName;
            }
            if (personnel != null)
            {
                model.PersonnelUID = personnel.UserID;
                model.FullName = personnel.FullName;
            }
            var isWelfare = false;
            if (model.OrderType != (int)OrderType.Recharge)//是否充值订单
            {
                model.Integral = 0;
                model.HasRefund = true;
                isWelfare = (model.PlaceOrderType == (int)PlaceOrderType.Welfare || model.PlaceOrderType == (int)PlaceOrderType.Task) && model.UserTaskID.HasValue && model.UserTaskID > 0;
                if (isWelfare)//判断是否兑换活动订单
                {
                    if (model.TaskPreferentialType == (int)PreferentialType.Money)
                    {
                        model.Cash = model.OrderTotal - model.PreferentialMoney;
                    }
                    else
                    {
                        model.Cash = 0;
                        model.Integral = 0;
                    }
                }
                if (model.Cash > 0)
                {
                    if (isIntegral)
                    {
                        var noUseIntegralList = shoppingCart.Items.Where(it => !it.IsIntegral);
                        if (noUseIntegralList.Count() == 0)
                        {
                            if (user.Integral > 0)
                            {
                                var integralMoney = user.Integral * 0.1m;//积分抵扣金额，10积分抵扣1元钱
                                if (integralMoney >= model.OrderTotal)
                                {
                                    model.PayDate = DateTime.Now;
                                    model.OrderStatus = model.OrderType == (int)OrderType.Group ? (int)OrderStatus.WaitReceipt : (int)OrderStatus.WaitShip;
                                    model.Integral = (int)(model.OrderTotal * 10);//抵扣所需积分
                                    model.PayType = PayType.Integral.ToString();
                                    model.Cash = 0;
                                }
                                else
                                {
                                    model.Integral = user.Integral;
                                    model.Cash = model.OrderTotal - integralMoney.Value;
                                }
                            }
                        }
                        else if(user.Integral > 0)
                        {
                            var productNames = new StringBuilder();
                            foreach (var m in noUseIntegralList)
                            {
                                productNames.AppendFormat("{0};", m.ProductName);
                            }
                            throw new ErrorCodeException(-1, $"{productNames.ToString()}不支持积分兑换哦！");
                        }
                    }
                }
                else
                {
                    model.Cash = 0;
                    model.Integral = 0;
                    model.OrderStatus =model.OrderType==(int)OrderType.Group ? (int)OrderStatus.WaitReceipt: (int)OrderStatus.WaitShip;
                }
            }
            var items = new List<Model.Shop_OrderItems>();
            var skus = new List<Model.Shop_SKUs>();
            var products = new List<Shop_Product>();
            foreach (var cart in shoppingCart.Items)
            {
                model.ShopID = cart.ShopID;
                var item = new Model.Shop_OrderItems();
                item.ProductCount = cart.ProductCount;
                item.SkuContent = cart.SkuContent;
                item.CostPrice = cart.CostPrice;
                item.ProductPrice = cart.SalePrice;
                item.SalePrice = cart.PromotionPrice;
                item.OrderID = model.OrderID;
                item.ProductID = cart.ProductID;
                item.Weight = cart.Weight.HasValue ? cart.Weight.Value : 0;
                item.SKU = cart.SKU;
                item.SkuID = cart.SkuID;
                item.Status = 1;
                item.TotalMoney = item.SalePrice * cart.ProductCount.Value;
                item.ProductName = cart.ProductName;
                item.RefundCount = 0;
                item.RefundAmount = 0;
                item.RefundIntegral = 0;
                item.RefundMoney = 0;
                if (cartResult.SkuDiscounts.Count > 0 && cartResult.SkuDiscounts.ContainsKey(item.SkuID))//获取分配到sku的优惠金额
                {
                    item.CouponMoney = cartResult.SkuDiscounts[item.SkuID];
                }
                else
                {
                    item.CouponMoney = 0;
                }
                if (model.OrderType != (int)OrderType.Recharge)
                {
                    if (cart.IsVirtual)//是否虚拟产品
                    {
                        model.OrderType = (int)OrderType.Virtual;
                    }
                    if (cart.ProductCount > cart.Stock)
                    {
                        throw new ErrorCodeException(-1, cart.ProductName + "库存不足！");
                    }
                    var sku = new Model.Shop_SKUs();
                    sku.SkuID = cart.SkuID;
                    sku.Stock = cart.ProductCount.Value;//扣除库存
                    skus.Add(sku);
                    var product = products.Find(it => it.ProductID == cart.ProductID);
                    if (product != null)
                    {
                        product.ProductCount += cart.ProductCount;
                        product.SaleCount += cart.ProductCount;
                    }
                    else
                    {
                        product = new Shop_Product();
                        product.ProductID = cart.ProductID.Value;
                        product.SaleCount = cart.ProductCount;
                        product.ProductCount = cart.ProductCount;
                        products.Add(product);
                    }
                    if (model.OrderType == (int)OrderType.Group)
                    {
                        item.ProductCount = 1;
                        item.ProductPrice = item.SalePrice;
                        item.TotalMoney = item.SalePrice;
                        var itemPayMoney = 0M;
                        var itemPayIntegral = 0;
                        if (model.Cash > 0)
                        {
                            itemPayMoney = model.Cash / cart.ProductCount.Value;
                        }
                        if (model.Integral > 0)
                        {
                            itemPayIntegral = model.Integral.Value / cart.ProductCount.Value;
                        }
                        for (var i = 0; i < cart.ProductCount; i++)
                        {
                            item.CouponNumber = String.Format("{0:D5}", model.ShopID) + model.UserID + Common.Tool.GetRandomPureNumber(8);
                            item.PayCash = itemPayMoney;
                            item.PayIntegral = itemPayIntegral;
                            items.Add(item);
                        }
                    }
                    else
                    {
                        var itemMoney = item.TotalMoney - item.CouponMoney;
                        if (model.Cash > 0)
                        {
                            if (model.Integral == 0)
                            {
                                item.PayCash = itemMoney;
                                item.PayIntegral = 0;
                            }
                            else
                            {
                                var prop = itemMoney / model.OrderTotal;//获得分摊比例
                                var refundIntegral = model.Integral * prop;
                                item.PayIntegral = (int)refundIntegral;
                                item.PayCash = itemMoney - item.PayIntegral.Value / 10M;
                            }
                        }
                        else
                        {
                            item.PayIntegral = (int)itemMoney * 10;
                            item.PayCash = 0;
                        }
                        if (cart.IsCommission && cart.Commission.HasValue && item.PayCash> 0 && cart.Commission>0 && cart.Commission<1)//是否启用佣金
                        {
                            item.Commission = item.PayCash * cart.Commission;
                            model.Commission += item.Commission;
                        }
                        items.Add(item);
                    }
                }
                else
                {
                    items.Add(item);
                }
            }
            var coupons = new List<Shop_CouponItems>();
            foreach (var item in cartResult.AppliedCoupons)
            {
                var userCoupon = new Shop_CouponItems();
                userCoupon.ID = item.ID;
                userCoupon.OrderID = model.OrderID;
                userCoupon.Status = (int)CouponItemStatus.HaveUse;
                userCoupon.CouponDiscount = item.MainCouponDiscount;
                userCoupon.CouponMoney = item.MainCouponMoney;
                userCoupon.CouponType = item.MainCouponType;
                userCoupon.WriteTime = model.OrderDate;
                coupons.Add(userCoupon);
            }
            var log=new Shop_OrderLog();
            log.CreateTime = DateTime.Now;
            log.Status=model.OrderStatus;
            if (user != null)
            {
                log.UserID = user.UserID;
                log.FullName = user.FullName;
            }
            if (personnel != null)
            {
                log.UserID = 0;
                log.FullName = personnel.FullName;
            }
            log.SysUserID = 0;
            log.OrderID = model.OrderID;
            log.Remark = isWelfare ? "兑换活动提交订单": "提交订单";
            if (model.Cash > 0) model.PayType = PayType.WeiXinPay.ToString();
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                await db.InsertableWithAttr(model).ExecuteReturnIdentityAsync();
                await db.InsertableWithAttr(items).ExecuteReturnIdentityAsync();
                var cartIds = shoppingCart.Items.Where(it=>it.CartID>0).Select(it => it.CartID).ToList();
                if (cartIds.Count > 0)
                {
                    await db.DeleteableWithAttr<Shop_ShoppingCart>().Where(it=>cartIds.Contains(it.CartID)).ExecuteCommandAsync();
                }
                await db.UpdateableWithAttr(skus).UpdateColumns(it => new { it.Stock }).PublicSetColumns(it => it.Stock, "-").ExecuteCommandAsync();//扣除sku库存
                if (products.Count > 0)
                {
                    await db.UpdateableWithAttr(products).UpdateColumns(it => new { it.ProductCount }).PublicSetColumns(it => it.ProductCount, "-").ExecuteCommandAsync();//扣除产品库存
                    await db.UpdateableWithAttr(products).UpdateColumns(it => new { it.SaleCount }).PublicSetColumns(it => it.SaleCount, "+").ExecuteCommandAsync();//增加销售数量
                }
                if (isWelfare)
                {
                    await db.UpdateableWithAttr<Shop_ProductUserTask>().SetColumns(it => it.Status == (int)Model.Enums.UserTaskStatus.Finish).Where(it=>it.ID==model.UserTaskID).ExecuteCommandAsync();
                }
                if (coupons.Count > 0)
                {
                    await db.UpdateableWithAttr(coupons).UpdateColumns(it => new { it.OrderID,it.Status,it.CouponType,it.CouponMoney,it.CouponDiscount,it.WriteTime}).ExecuteCommandAsync();//设置优惠券
                }
                await db.InsertableWithAttr(log).ExecuteCommandAsync();
                return true;
            });
            if (result.Data)
            {
                if (model.Integral > 0 && model.OrderType != (int)OrderType.Recharge)
                {
                    var record = new IntegralLogEventData();
                    record.UserID = model.UserID.Value;
                    record.Integral = model.Integral.Value * -1;
                    record.OrderID = model.OrderID;
                    record.IntegralType = IntegralType.Consume;
                    record.IsUpdate = true;
                    record.CompanyGID = model.CompanyGID.Value;
                    record.CompanyID = model.CompanyID.Value;
                    publisher.Publish(record);
                }
                if (model.OrderStatus == (int)OrderStatus.WaitShip)
                {
                    await SendOrder(model);
                }
            }
            else
            {
                Common.StaticLog.Error(result.ErrorMessage);
            }

                return result.Data;
        }
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> Cancel(string orderId,int userId=0,Model.Shop_Order model=null)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                var expable = Expressionable.Create<Model.Shop_Order>();
                expable.And(it => it.OrderID == orderId);
                expable.AndIF(userId > 0, it => it.UserID == userId);
                model = await base.Context.Queryable<Shop_Order>().FirstAsync(expable.ToExpression());
            }
            
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            if (model.OrderStatus != (int)OrderStatus.WaitPay)
            {
                throw new ErrorCodeException(-1, "该订单不在待付款状态不可取消！");
            }
            var items = await base.Context.Queryable<Model.Shop_V_OrderItems>().Where(it => it.OrderID == model.OrderID).ToListAsync();
            var skus = new List<Shop_SKUs>();
            var products = new List<Shop_Product>();
            foreach (var item in items)
            {
                var sku = new Shop_SKUs();
                sku.ProductID=item.ProductID;
                sku.SkuID=item.SkuID;
                sku.Stock = item.ProductCount;
                skus.Add(sku);
                var product = products.Find(it => it.ProductID == item.ProductID);
                if (product != null)
                {
                    product.ProductCount += item.ProductCount;
                    product.SaleCount += item.ProductCount;
                }
                else
                {
                    product = new Shop_Product();
                    product.ProductID = item.ProductID.Value;
                    product.SaleCount = item.ProductCount;
                    product.ProductCount = item.ProductCount;
                    products.Add(product);
                }
            }
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                await db.UpdateableWithAttr<Model.Shop_Order>().SetColumns(it => it.OrderStatus == (int)OrderStatus.Cancel).Where(it => it.OrderID == model.OrderID && it.OrderStatus == (int)OrderStatus.WaitPay).ExecuteCommandAsync();
                await db.UpdateableWithAttr<Shop_OrderItems>().SetColumns(it => it.Status == (int)OrderStatus.Cancel).Where(it => it.OrderID == model.OrderID).ExecuteCommandAsync();
                await db.UpdateableWithAttr(skus).UpdateColumns(it => new { it.Stock }).PublicSetColumns(it => it.Stock, "+").ExecuteCommandAsync();//返回sku库存
                if (products.Count > 0)
                {
                    await db.UpdateableWithAttr(products).UpdateColumns(it => new { it.ProductCount }).PublicSetColumns(it => it.ProductCount, "+").ExecuteCommandAsync();//返回产品库存
                    await db.UpdateableWithAttr(products).UpdateColumns(it => new { it.SaleCount }).PublicSetColumns(it => it.SaleCount, "-").ExecuteCommandAsync();//扣除销售数量
                }
                if (model.CouponTotal > 0)
                {
                    await db.UpdateableWithAttr<Shop_CouponItems>().SetColumns(it => it.Status == (int)CouponItemStatus.WaitUse).Where(it => it.OrderID == model.OrderID && it.UserID == model.UserID && it.Status == (int)CouponItemStatus.HaveUse).ExecuteCommandAsync();
                }
                return true;
            });
            if (result.Data)
            {
                if (model.Integral > 0)
                {
                    var record = new IntegralLogEventData();
                    record.UserID = model.UserID.Value;
                    record.Integral = model.Integral.Value;
                    record.OrderID = model.OrderID;
                    record.IntegralType = IntegralType.CancelConsume;
                    record.IsUpdate = true;
                    record.CompanyGID = model.CompanyGID.Value;
                    record.CompanyID = model.CompanyID.Value;
                    publisher.Publish(record);
                }
                model.OrderStatus = (int)OrderStatus.Cancel;
                await SendOrder(model);
            }
            return result.Data;
        }
        /// <summary>
        /// 退单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> Refund(string orderId,int userId = 0)
        {
            var expable = Expressionable.Create<Model.Shop_Order>();
            expable.And(it => it.OrderID == orderId);
            expable.AndIF(userId > 0, it => it.UserID == userId);
            var model = await base.Context.Queryable<Shop_Order>().FirstAsync(expable.ToExpression());
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            if (model.OrderType == (int)OrderType.Group)
            {
                if(model.OrderStatus != (int)OrderStatus.WaitReceipt)
                {
                    throw new ErrorCodeException(-1, "该订单不在待使用状态不可取消！");
                }
            }
            else if (model.OrderStatus != (int)OrderStatus.WaitShip)
            {
                throw new ErrorCodeException(-1, "该订单不在待发货状态不可退单！");
            }
            if (model.Cash > 0)
            {
                model.RefundDate= DateTime.Now;
                model.RefundMoney = model.Cash;
                model.RefundID=Common.Tool.RandomName();
                await payService.WeiXinRefund(model);
            }
            model.OrderStatus = (int)OrderStatus.Refund;
            var items = await base.Context.Queryable<Model.Shop_V_OrderItems>().Where(it => it.OrderID == orderId).ToListAsync();
            var skus = new List<Shop_SKUs>();
            var products = new List<Shop_Product>();
            foreach (var item in items)
            {
                var sku = new Shop_SKUs();
                sku.ProductID = item.ProductID;
                sku.SkuID = item.SkuID;
                sku.Stock = item.ProductCount;
                skus.Add(sku);
                var product = products.Find(it => it.ProductID == item.ProductID);
                if (product != null)
                {
                    product.ProductCount += item.ProductCount;
                    product.SaleCount += item.ProductCount;
                }
                else
                {
                    product = new Shop_Product();
                    product.ProductID = item.ProductID.Value;
                    product.SaleCount = item.ProductCount;
                    product.ProductCount = item.ProductCount;
                    products.Add(product);
                }
            }
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                await db.UpdateableWithAttr(model).UpdateColumns(it => new { it.OrderStatus, it.RefundID, it.RefundMoney, it.RefundDate }).ExecuteCommandAsync();
                await db.UpdateableWithAttr<Shop_OrderItems>().SetColumns(it=>it.Status==(int)OrderStatus.Refund).Where(it=>it.OrderID==model.OrderID && it.Status != (int)OrderStatus.Refund).ExecuteCommandAsync();
                await db.UpdateableWithAttr(skus).UpdateColumns(it => new { it.Stock }).PublicSetColumns(it => it.Stock, "+").ExecuteCommandAsync();//返回sku库存
                if (products.Count > 0)
                {
                    await db.UpdateableWithAttr(products).UpdateColumns(it => new { it.ProductCount }).PublicSetColumns(it => it.ProductCount, "+").ExecuteCommandAsync();//返回产品库存
                    await db.UpdateableWithAttr(products).UpdateColumns(it => new { it.SaleCount }).PublicSetColumns(it => it.SaleCount, "-").ExecuteCommandAsync();//扣除销售数量
                }
                if (model.CouponTotal > 0)
                {
                    await db.UpdateableWithAttr<Shop_CouponItems>().SetColumns(it => it.Status == (int)CouponItemStatus.WaitUse).Where(it => it.OrderID == model.OrderID && it.UserID==model.UserID && it.Status == (int)CouponItemStatus.HaveUse).ExecuteCommandAsync();
                }
                return true;
            });
            if (result.Data)
            {
                if(model.Integral > 0)
                {
                    var record = new IntegralLogEventData();
                    record.UserID = model.UserID.Value;
                    record.Integral = model.Integral.Value;
                    record.OrderID = model.OrderID;
                    record.IntegralType = IntegralType.CancelConsume;
                    record.IsUpdate = true;
                    record.Remark = "用户退单";
                    record.CompanyGID = model.CompanyGID.Value;
                    record.CompanyID = model.CompanyID.Value;
                    publisher.Publish(record);
                }
                
                await SendOrder(model);
            }
            return result.Data;
        }

        /// <summary>
        /// 单个商品退单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="count">数量</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> ItemRefund(int id,int count, int userId = 0)
        {
            var expable = Expressionable.Create<Model.Shop_V_OrderItems>();
            expable.And(it => it.ID == id);
            expable.AndIF(userId > 0, it => it.UserID == userId);
            var model = await base.Context.Queryable<Shop_V_OrderItems>().FirstAsync(expable.ToExpression());
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该订单明细不存在！");
            }
            var oldStatus = (int)OrderStatus.WaitShip;//根据订单类型设定是待收货还是待发货
            if (model.OrderType != (int)OrderType.Group)
            {
                throw new ErrorCodeException(-1, "目前暂不支持商品订单明细退款！");
            }
            if (model.OrderType == (int)OrderType.Group)
            {
                oldStatus = (int)OrderStatus.WaitReceipt;
                if (model.OrderStatus != (int)OrderStatus.WaitReceipt)
                {
                    throw new ErrorCodeException(-1, "该订单不在待使用状态不可退款！");
                }
                else if (model.Status != (int)OrderStatus.WaitReceipt)
                {
                    throw new ErrorCodeException(-1, "该订单明细不在待使用状态不可退款！");
                }
            }
            else if (model.OrderStatus != (int)OrderStatus.WaitShip || model.Status != (int)OrderStatus.WaitShip)
            {
                throw new ErrorCodeException(-1, "该订单不在待发货状态不可退单！");
            }
            else if (model.Status != (int)OrderStatus.WaitShip)
            {
                throw new ErrorCodeException(-1, "该订单明细不在待发货状态不可退款！");
            }
            if (model.RefundCount + count > model.ProductCount)
            {
                throw new ErrorCodeException(-1, "已达退货数量！");
            }
            if (!model.RefundCount.HasValue) model.RefundCount = 0;
            if (!model.RefundIntegral.HasValue) model.RefundIntegral = 0;
            model.RefundDate = DateTime.Now;
            if (model.PayCash > 0)//是否有现金支付
            {
                var order = await Get(model.OrderID);
                if (model.ProductCount == count)
                {
                    order.RefundMoney = model.PayCash;
                }
                else
                {
                    order.RefundMoney = (model.PayCash / model.ProductCount.Value) * count;
                }
                order.RefundDate = DateTime.Now;
                order.RefundID = Common.Tool.RandomName();
                order.CompanyGID = model.CompanyGID;
                //order.OrderID = model.OrderID;
                model.RefundMoney = model.RefundMoney + order.RefundMoney;
                if (string.IsNullOrEmpty(model.RefundID))
                {
                    model.RefundID =  order.RefundID;
                }
                else
                {
                    model.RefundID += "," + order.RefundID;
                }
                await payService.WeiXinRefund(order);
            }
            var refundIntegral = 0;
            if (model.PayIntegral > 0)//是否有积分支付
            {
                if (model.ProductCount == count)
                {
                    refundIntegral = model.PayIntegral.Value;
                }
                else
                {
                    refundIntegral = (model.PayIntegral.Value / model.ProductCount.Value) * count;
                }
            }
            model.RefundAmount += model.ProductPrice * count;
            model.RefundCount = model.RefundCount + count;
            model.RefundIntegral += refundIntegral;
            model.OrderStatus = model.RefundCount >= model.ProductCount ? (int)OrderStatus.Refund : oldStatus;
            //是否还有剩余未退的商品
            var isRemain = await base.Context.Queryable<Model.Shop_V_OrderItems>().Where(it => it.OrderID == model.OrderID && it.Status== oldStatus && it.ID!=model.ID).AnyAsync();
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                if (!isRemain && model.OrderStatus == (int)OrderStatus.Refund)
                {
                    await db.UpdateableWithAttr<Shop_Order>().SetColumns(it => it.OrderStatus == model.OrderStatus).Where(it => it.OrderID == model.OrderID).ExecuteCommandAsync();//所有商品都已退款，将整笔订单设为退款
                }
                await db.UpdateableWithAttr<Shop_OrderItems>().SetColumns(it => new Model.Shop_OrderItems() {Status=model.OrderStatus,RefundCount=model.RefundCount,RefundDate=model.RefundDate,RefundID=model.RefundID,RefundIntegral=model.RefundIntegral, RefundAmount=model.RefundAmount, RefundMoney=model.RefundMoney }).Where(it=>it.ID==model.ID).ExecuteCommandAsync();
                await db.UpdateableWithAttr<Shop_SKUs>().SetColumns(it => it.Stock== it.Stock+count).Where(it=>it.SkuID==model.SkuID).ExecuteCommandAsync();//返回sku库存
                await db.UpdateableWithAttr<Shop_Product>().SetColumns(it => it.ProductCount == it.ProductCount + count).SetColumns(it => it.SaleCount == it.SaleCount - model.ProductCount).Where(it => it.ProductID == model.ProductID).ExecuteCommandAsync();//返回产品库存及扣除销售数量
                return true;
            });
            if (result.Data)
            {
                if (refundIntegral > 0)
                {
                    var record = new IntegralLogEventData();
                    record.UserID = model.UserID.Value;
                    record.Integral = refundIntegral;
                    record.OrderID = model.OrderID;
                    record.IntegralType = IntegralType.CancelConsume;
                    record.IsUpdate = true;
                    record.Remark = "用户明细退单";
                    record.CompanyGID = model.CompanyGID.Value;
                    record.CompanyID = model.CompanyID.Value;
                    publisher.Publish(record);
                }

               // await SendOrder(null,model.OrderID);
            }
            return result.Data;
        }
        public async Task<List<Model.CountModel>> StatusCount(Expression<Func<Model.Shop_Order, bool>> exp)
        {
            var qb = base.Context.Queryable<Shop_Order>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.OrderStatus });
            return await qb.Select(it => new Model.CountModel() { FID = it.OrderStatus.Value, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        /// <summary>
        /// 数量
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<int> CountSync(Expression<Func<Model.Shop_Order, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Order>().CountAsync(exp);
        }
        /// <summary>
        /// 完成订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<bool> Finish(string orderId, int userId = 0, Model.Shop_Order model = null)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                var expable = Expressionable.Create<Model.Shop_Order>();
                expable.And(it => it.OrderID == orderId);
                expable.AndIF(userId > 0, it => it.UserID == userId);
                model = await base.Context.Queryable<Shop_Order>().FirstAsync(expable.ToExpression());
            }
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            if (model.OrderStatus != (int)OrderStatus.WaitReceipt)
            {
                throw new ErrorCodeException(-1, "该订单不在待收货状态，不可完成！");
            }
            model.OrderStatus = (int)OrderStatus.Have;
            model.FinishDate = DateTime.Now;
            var has = await base.Context.Updateable(model).UpdateColumns(it => new { it.OrderStatus, it.FinishDate }).ExecuteCommandAsync() > 0;
            if (has)
            {
                await base.Context.Updateable<Shop_OrderItems>().SetColumns(it => it.Status == model.OrderStatus).Where(it => it.OrderID == model.OrderID && it.Status!=(int)OrderStatus.Refund).ExecuteCommandAsync();
                if (model.OrderTotal > 0)
                {
                    var record = new IntegralLogEventData();
                    record.UserID = model.UserID.Value;
                    record.Integral = (int)(model.OrderTotal / 10);
                    record.OrderID = model.OrderID;
                    record.IntegralType = IntegralType.ConsumeReturn;
                    record.IsUpdate = true;
                    record.Remark = "消费得积分";
                    record.CompanyGID = model.CompanyGID.Value;
                    record.CompanyID = model.CompanyID.Value;
                    publisher.Publish(record);
                }
                await SendOrder(model);
            }
            return has;
        }
        /// <summary>
        /// 修改订单备注
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRemark(string orderId, string remark)
        {
            var has= await base.Context.Updateable<Shop_Order>().SetColumns(it=>it.SellerRemark == remark).Where(it=>it.OrderID==orderId).ExecuteCommandAsync() > 0;
            if (has)
            {
                await SendOrder(null, orderId);
            }
            return has;
        }
        /// <summary>
        /// 修改退货状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> ChangeReturnStatus(string orderId,int status)
        {
            var has= await base.Context.Updateable<Shop_Order>().SetColumns(it => it.RefundStatus == status).Where(it => it.OrderID == orderId).ExecuteCommandAsync() > 0;
            if (has && status== (int)OrderReturnStatus.RefundFinish)
            {
                await CloseOrderStatus(orderId);
            }
            return has;
        }
        /// <summary>
        /// 退货完成后去关闭订单的状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> CloseOrderStatus(string orderId)
        {
            var exist = await base.Context.Queryable<Shop_OrderItems>().AnyAsync(it=>it.OrderID==orderId && it.RefundCount!=it.ProductCount);
            if (!exist)
            {
               return await base.Context.Updateable<Shop_Order>().SetColumns(it => it.OrderStatus == (int)OrderStatus.Close).Where(it => it.OrderID == orderId).ExecuteCommandAsync() > 0;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 修改评价状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAssess(string orderId)
        {
            return await base.Context.Updateable<Shop_Order>().SetColumns(it => it.IsAssess == true).Where(it => it.OrderID == orderId).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 批量修改评价状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBatchAssess(Expression<Func<Model.Shop_Order, bool>> exp)
        {
            return await base.Context.Updateable<Shop_Order>().SetColumns(it => it.IsAssess == true).Where(exp).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 修改退货状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRefundStatus(string orderId,int status)
        {
            return await base.Context.Updateable<Shop_Order>().SetColumns(it => it.RefundStatus == status).Where(it => it.OrderID == orderId).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 修改订单金额
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOrderTotal(string orderId, decimal money)
        {
            var model = await base.Context.Queryable<Shop_Order>().FirstAsync(it => it.OrderID == orderId);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            if (model.OrderStatus != (int)OrderStatus.WaitPay)
            {
                throw new ErrorCodeException(-1, "只有待付款才可修改订单！");
            }
            if (model.Cash == 0)
            {
                throw new ErrorCodeException(-1, "实付金额为0，不可修改！");
            }
            model.Cash = money;
            if (model.Integral > 0)
            {
                model.OrderTotal = model.Integral.Value % 10.0M + money;
            }
            else
            {
                model.OrderTotal = money;
            }
            return await base.Context.Updateable<Shop_Order>().SetColumns(it => new Shop_Order() { OrderTotal = model.OrderTotal, Cash = model.Cash }).Where(it => it.OrderID == orderId).ExecuteCommandAsync() > 0;
        }

        public async Task<HupunResultModel?> SendOrder(Shop_Order model, string orderId = "", int status = 2, int has_refund = 0)
        {
            try
            {
                if (model == null)
                {
                    model = await base.Context.Queryable<Shop_Order>().FirstAsync(it => it.OrderID == orderId);
                }
                if (model.OrderType != (int)OrderType.Product)
                {
                    return new HupunResultModel() { code = 4444, data = new HupunResultDataModel() { success = false, error_msg = "只支持商品订单同步" } };
                }
                var items = await Items(model.OrderID);
                var res = await hupunERPService.Order(model, items, status, has_refund);
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
        public async Task<int> Add(Shop_Order model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_Order, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Order>().AnyAsync(exp);
        }
        public async Task<bool> ExistItems(Expression<Func<Shop_OrderItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_OrderItems>().AnyAsync(exp);
        }
        public async Task<bool> OrderSend(OrderSend model)
        {
            var has = await base.Context.Updateable<Shop_Order>().SetColumns(it => new Shop_Order() { ExpressName=model.ExpressName,ExpressNo=model.ExpressNo,ExpressType=model.ExpressType,ShippingDate=DateTime.Now,OrderStatus=(int)OrderStatus.WaitReceipt,IsPackage=model.IsPackage}).Where(it=>it.OrderID==model.OrderID && it.OrderStatus==(int)OrderStatus.WaitShip).ExecuteCommandAsync() > 0;
            if (has && !model.IsPackage)
            {
                await base.Context.Updateable<Shop_OrderItems>().SetColumns(it => it.Status == (int)OrderStatus.WaitReceipt).Where(it => it.OrderID == model.OrderID && it.Status == (int)OrderStatus.WaitShip).ExecuteCommandAsync();
            }
            return has;
        }
        public async Task<ErpPackageSendResult> ErpPackageSend(HupunSendDataModel model)
        {
            var result = new ErpPackageSendResult();
            if (model == null || string.IsNullOrEmpty(model.trade_id))
            {
                throw new ErrorCodeException(-1, "发货错误，物流信息不全！");
            }
            NormalizeExpressCode(model);
            var packageList = (model.packages ?? new List<HupunSendPackageModel>())
                .Where(it => it != null && !string.IsNullOrEmpty(it.wayBill)).ToList();
            if (packageList.Count == 0 && string.IsNullOrEmpty(model.waybill))
            {
                throw new ErrorCodeException(-1, "发货错误，物流信息不全！");
            }
            foreach (var package in packageList)
            {
                NormalizePackageExpressCode(package);
            }

            var order = await Get(model.trade_id);
            if (order == null)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            var orderItems = await base.Context.Queryable<Shop_OrderItems>().Where(it => it.OrderID == order.OrderID).ToListAsync();
            var firstExpressNo = packageList.Count > 0 ? packageList[0].wayBill : model.waybill;
            var firstExpressType = packageList.Count > 0 ? packageList[0].deliveryCode : model.delivery_code;
            var firstExpressName = packageList.Count > 0 ? packageList[0].deliveryName : model.delivery_name;
            var existWaybills = (await base.Context.Queryable<Shop_OrderPackage>().Where(it => it.OrderID == order.OrderID).Select(it => it.ExpressNo).ToListAsync()).ToHashSet();
            var newPackages = packageList.Where(it => !existWaybills.Contains(it.wayBill)).ToList();
            var isPackage = packageList.Count > 1
                || order.IsPackage
                || existWaybills.Count > 1
                || (existWaybills.Count == 1 && !existWaybills.Contains(firstExpressNo))
                || (!string.IsNullOrEmpty(order.ExpressNo) && order.ExpressNo != firstExpressNo);
         
            
            if (order.OrderStatus == (int)OrderStatus.WaitShip)
            {
                var sendModel = new OrderSend()
                {
                    OrderID = order.OrderID,
                    ExpressName = firstExpressName,
                    ExpressNo = firstExpressNo,
                    ExpressType = firstExpressType,
                    IsPackage = isPackage
                };
                if (!await OrderSend(sendModel))
                {
                    throw new ErrorCodeException(-1, "发货错误，只有已付款/待发货才能发货！");
                }
                order.ExpressName = sendModel.ExpressName;
                order.ExpressNo = sendModel.ExpressNo;
                order.ExpressType = sendModel.ExpressType;
                order.OrderStatus = (int)OrderStatus.WaitReceipt;
                order.IsPackage = isPackage;
                result.IsNewSend = true;
            }
            else if (order.OrderStatus == (int)OrderStatus.WaitReceipt || order.OrderStatus == (int)OrderStatus.HaveShip)
            {
                if (!isPackage)
                {
                    if (order.ExpressNo == firstExpressNo)
                    {
                        result.Success = true;
                        result.Order = order;
                        result.ExpressNo = firstExpressNo;
                        result.ExpressType = firstExpressType;
                        result.ExpressName = firstExpressName;
                        return result;
                    }
                    throw new ErrorCodeException(-1, "该订单不在待发货状态！");
                }
                if (newPackages.Count == 0)
                {
                    result.Success = true;
                    result.Order = order;
                    result.ExpressNo = firstExpressNo;
                    result.ExpressType = firstExpressType;
                    result.ExpressName = firstExpressName;
                    return result;
                }
                if (!order.IsPackage)
                {
                    await MigrateOrderExpressToPackage(order);
                    await base.Context.Updateable<Shop_Order>().SetColumns(it => it.IsPackage == true).Where(it => it.OrderID == order.OrderID).ExecuteCommandAsync();
                    order.IsPackage = true;
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "该订单不在待发货状态！");
            }

            var shippedItems = new List<HupunSendOrderItemModel>();
            if (isPackage)
            {
                foreach (var package in newPackages)
                {
                    await AddOrderPackage(order.OrderID, package, orderItems);
                    result.SubscribeItems.Add(new ErpExpressSubscribeItem()
                    {
                        ExpressNo = package.wayBill,
                        ExpressType = package.deliveryCode
                    });
                    if (package.orderInfos != null)
                    {
                        shippedItems.AddRange(package.orderInfos);
                    }
                }
                await UpdateShippedItemStatus(order.OrderID, shippedItems);
            }
            else if (result.IsNewSend)
            {
                result.SubscribeItems.Add(new ErpExpressSubscribeItem()
                {
                    ExpressNo = firstExpressNo,
                    ExpressType = firstExpressType
                });
            }

            result.Success = true;
            result.Order = order;
            result.ExpressNo = firstExpressNo;
            result.ExpressType = firstExpressType;
            result.ExpressName = firstExpressName;
            return result;
        }
        public async Task<bool> ReceiveExpressInfo(ExpressPushModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.number))
            {
                return false;
            }
            var details = model.details ?? new List<ExpressPushDetailModel>();
            var latest = details.OrderByDescending(it => it.time ?? 0).FirstOrDefault();
            var expressStatus = !string.IsNullOrEmpty(model.logisticsStatusDesc) ? model.logisticsStatusDesc : (latest?.logisticsStatus ?? model.logisticsStatus);
            var expressNewInfo = !string.IsNullOrEmpty(latest?.desc) ? latest.desc : expressStatus;

            await base.Context.Deleteable<Shop_ExpressInfo>().Where(it => it.ExpressNo == model.number).ExecuteCommandAsync();
            if (details.Count > 0)
            {
                var infos = details.Select(it => new Shop_ExpressInfo()
                {
                    ExpressNo = model.number,
                    CreateTime = ToExpressTime(it.time),
                    LogisticsStatus = it.logisticsStatus,
                    SubLogisticsStatus = it.subLogisticsStatus,
                    AreaCode = it.areaCode,
                    AreaName = it.areaName,
                    Desc=it.desc
                }).ToList();
                await base.Context.Insertable(infos).ExecuteCommandAsync();
            }
            else
            {
                await base.Context.Insertable(new Shop_ExpressInfo()
                {
                    ExpressNo = model.number,
                    CreateTime = DateTime.Now,
                    LogisticsStatus = model.logisticsStatus,
                    SubLogisticsStatus = latest?.subLogisticsStatus,
                    AreaCode = latest?.areaCode,
                    AreaName = latest?.areaName,
                    Desc= latest?.desc
                }).ExecuteCommandAsync();
            }

            var packages = await base.Context.Queryable<Shop_OrderPackage>().Where(it => it.ExpressNo == model.number).ToListAsync();
            if (packages.Count > 0)
            {
                await base.Context.Updateable<Shop_OrderPackage>().SetColumns(it => new Shop_OrderPackage()
                {
                    ExpressStatus = expressStatus,
                    ExpressNewInfo = expressNewInfo
                }).Where(it => it.ExpressNo == model.number).ExecuteCommandAsync();
            }
            else
            {
                await base.Context.Updateable<Shop_Order>().SetColumns(it => new Shop_Order()
                {
                    ExpressStatus = expressStatus,
                    ExpressNewInfo = expressNewInfo
                }).Where(it => it.ExpressNo == model.number).ExecuteCommandAsync();
            }
            return true;
        }
        private static void NormalizeExpressCode(HupunSendDataModel model)
        {
            switch (model.delivery_code)
            {
                case "POSTB":
                    model.delivery_code = "CHINAPOST";
                    break;
            }
        }
        private static void NormalizePackageExpressCode(HupunSendPackageModel package)
        {
            switch (package.deliveryCode)
            {
                case "POSTB":
                    package.deliveryCode = "CHINAPOST";
                    break;
            }
        }
        private async Task MigrateOrderExpressToPackage(Shop_Order order)
        {
            if (string.IsNullOrEmpty(order.ExpressNo))
            {
                return;
            }
            var exists = await base.Context.Queryable<Shop_OrderPackage>().AnyAsync(it => it.OrderID == order.OrderID && it.ExpressNo == order.ExpressNo);
            if (exists)
            {
                return;
            }
            await base.Context.Insertable(new Shop_OrderPackage()
            {
                PackageGID = Guid.NewGuid(),
                OrderID = order.OrderID,
                ExpressNo = order.ExpressNo,
                ExpressType = order.ExpressType,
                ExpressName = order.ExpressName,
                ExpressStatus = order.ExpressStatus,
                ExpressNewInfo = order.ExpressNewInfo,
                CreateTime = order.ShippingDate ?? DateTime.Now
            }).ExecuteCommandAsync();
        }
        private async Task AddOrderPackage(string orderId, HupunSendPackageModel packageModel, List<Shop_OrderItems> orderItems)
        {
            var package = new Shop_OrderPackage()
            {
                PackageGID = Guid.NewGuid(),
                OrderID = orderId,
                ExpressNo = packageModel.wayBill,
                ExpressType = packageModel.deliveryCode,
                ExpressName = packageModel.deliveryName,
                CreateTime = DateTime.Now
            };
            var sendItems = packageModel.orderInfos ?? new List<HupunSendOrderItemModel>();
            try
            {
                await itenant.BeginTranAsync();
                await base.Context.Insertable(package).ExecuteCommandAsync();
                var packageItems = new List<Shop_OrderPackageItems>();
                foreach (var sendItem in sendItems)
                {
                    var itemId = sendItem.GetOrderItemId();
                    if (!itemId.HasValue)
                    {
                        continue;
                    }
                    var orderItem = orderItems.FirstOrDefault(it => it.ID == itemId.Value);
                    if (orderItem == null)
                    {
                        continue;
                    }
                    var count = sendItem.num;
                    if (count <= 0)
                    {
                        count = orderItem.ProductCount ?? 0;
                    }
                    packageItems.Add(new Shop_OrderPackageItems()
                    {
                        PackageGID = package.PackageGID,
                        OrderItemID = itemId.Value,
                        Count = count
                    });
                }
                if (packageItems.Count > 0)
                {
                    await base.Context.Insertable(packageItems).ExecuteCommandAsync();
                }
                await itenant.CommitTranAsync();
            }
            catch
            {
                itenant.RollbackTran();
                throw;
            }
        }
        private async Task UpdateShippedItemStatus(string orderId, List<HupunSendOrderItemModel> sendItems)
        {
            var sentIds = sendItems?.Select(it => it.GetOrderItemId()).Where(it => it.HasValue).Select(it => it.Value).Distinct().ToList();
            if (sentIds != null && sentIds.Count > 0)
            {
                await base.Context.Updateable<Shop_OrderItems>().SetColumns(it => it.Status == (int)OrderStatus.WaitReceipt).Where(it => it.OrderID == orderId && sentIds.Contains(it.ID) && it.Status == (int)OrderStatus.WaitShip).ExecuteCommandAsync();
            }
            else
            {
                await base.Context.Updateable<Shop_OrderItems>().SetColumns(it => it.Status == (int)OrderStatus.WaitReceipt).Where(it => it.OrderID == orderId && it.Status == (int)OrderStatus.WaitShip).ExecuteCommandAsync();
            }
        }
        private static DateTime ToExpressTime(long? time)
        {
            if (!time.HasValue || time.Value <= 0)
            {
                return DateTime.Now;
            }
            if (time.Value > 1000000000000)
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(time.Value).LocalDateTime;
            }
            return DateTimeOffset.FromUnixTimeSeconds(time.Value).LocalDateTime;
        }
        public async Task<bool> OrderPay(Shop_Order model)
        {
            if (model.OrderType == (int)OrderType.Recharge)
            {
                model.OrderStatus = (int)OrderStatus.Have;
            }
            else if (model.OrderType == (int)OrderType.Group)
            {
                model.OrderStatus = (int)OrderStatus.WaitReceipt;
            }
            else
            {
                model.OrderStatus = (int)OrderStatus.WaitShip;
            }
            model.PayDate = DateTime.Now;
            var has = await base.Context.Updateable(model).UpdateColumns(it => new { it.PayDate,it.PayID,it.OrderStatus}).ExecuteCommandAsync() > 0;
            if (has)
            {
                await base.Context.Updateable<Shop_OrderItems>().SetColumns(it => it.Status == model.OrderStatus).Where(it => it.OrderID == model.OrderID).ExecuteCommandAsync();
                if (model.OrderType == (int)OrderType.Recharge)
                {
                    var record = new IntegralLogEventData();
                    record.UserID = 0;
                    record.Integral = model.Integral.Value;
                    record.IntegralType = IntegralType.PartnerRecharge;
                    record.CompanyGID = model.CompanyGID.Value;
                    record.CompanyID = model.CompanyID.Value;
                    record.PartnerID = model.PartnerID.Value;
                    record.PartnerGID = model.PartnerGID.Value;
                    record.OrderID = model.OrderID;
                    publisher.Publish(record);
                }
                else
                {
                    await SendOrder(model);
                }
            }
            return has;
        }
        public override async Task<bool> UpdateAsync(Shop_Order model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateItem(Shop_OrderItems model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Shop_Order> Get(string id)
        {
            return await base.Context.Queryable<Shop_Order>().FirstAsync(it => it.OrderID == id);
        }
        public async Task<List<OrderPackageModel>> PackageList(string orderId)
        {
            var packages = await base.Context.Queryable<Shop_OrderPackage>().Where(it => it.OrderID == orderId).OrderBy(it => it.ID).ToListAsync();
            if (packages.Count == 0)
            {
                return new List<OrderPackageModel>();
            }
            var packageGids = packages.Where(it => it.PackageGID.HasValue).Select(it => it.PackageGID.Value).ToList();
            var items = packageGids.Count == 0
                ? new List<Shop_V_OrderPackageItems>()
                : await base.Context.Queryable<Shop_V_OrderPackageItems>().Where(it => it.OrderID == orderId && it.PackageGID.HasValue && packageGids.Contains(it.PackageGID.Value)).ToListAsync();
            return packages.Select(it => new OrderPackageModel()
            {
                ID = it.ID,
                PackageGID = it.PackageGID,
                OrderID = it.OrderID,
                ExpressStatus = it.ExpressStatus,
                ExpressNewInfo = it.ExpressNewInfo,
                ExpressNo = it.ExpressNo,
                ExpressType = it.ExpressType,
                ExpressName = it.ExpressName,
                CreateTime = it.CreateTime,
                Items = items.Where(s => s.PackageGID == it.PackageGID).ToList()
            }).ToList();
        }
        public async Task<List<Shop_V_OrderItems>> Items(string id)
        {
            return await base.Context.Queryable<Shop_V_OrderItems>().Where(it => it.OrderID == id).ToListAsync();
        }
        public async Task<List<Shop_V_OrderItems>> Items(Expression<Func<Shop_V_OrderItems, bool>> exp, PagedModelBase query=null)
        {
            if (query != null)
            {
                var count = new RefAsync<int>();
                if (query.Page == 0)
                {
                    return await base.Context.Queryable<Shop_V_OrderItems>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
                }
                var list = await base.Context.Queryable<Shop_V_OrderItems>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
                query.Count = count.Value;
                return list;
            }
            else
            {
                return await base.Context.Queryable<Shop_V_OrderItems>().Where(exp).ToListAsync();
            }
        }
        public async Task<Shop_V_OrderItems> GetItem(Expression<Func<Shop_V_OrderItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_OrderItems>().FirstAsync(exp);
        }
        public async Task<Shop_Order> Get(Expression<Func<Shop_Order, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Order>().FirstAsync(exp);
        }
        public async Task<bool> Delete(string orderId)
        {
            var has = await base.Context.Deleteable<Shop_Order>().Where(it => it.OrderID == orderId && it.OrderStatus==(int)OrderStatus.Cancel).ExecuteCommandAsync() > 0;
            if (!has)
            {
                throw new ErrorCodeException(-1, "只有取消订单才可删除！");
            }
            return has;
        }
    }
}
