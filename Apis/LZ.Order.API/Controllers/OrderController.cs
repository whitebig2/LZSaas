using LZ.ApiService.Interface;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.ApiModel;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Order.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Senparc.CO2NET.Cache.Redis;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sec;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;

namespace LZ.Order.API
{
    public class OrderController(IOrderService orderService,IHttpService httpService,IAuthService authService,IPayService payService,IOrderLogService orderLogService,ICompanyConfigService companyConfigService,IOrderReturnService orderReturnService,IAliyunCommonService aliyunCommonService,ISettingsService settingsService) : OrderControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] OrderPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Order>();

             SetSearch(expable, query);
             var exp = expable.ToExpression();
            var models = await orderService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Lists([FromQuery] OrderPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Order>();
           SetSearch(expable, query);
            var exp = expable.ToExpression();
            var models = await orderService.List(exp, query);
            var orderIds = models.Select(it => it.OrderID).ToList();
            var orderItems = await orderService.Items(it => orderIds.Contains(it.OrderID));
            return Ok(new { items = models, orderItems, count = query.Count });
        }
        private void SetSearch(Expressionable<Model.Shop_Order> expable, OrderPagedModel query)
        {
            var user = CurrentUser;
            if (!string.IsNullOrEmpty(query.ProductName))
            {
                expable.And(it => SqlFunc.Subqueryable<Model.Shop_V_OrderItems>().Where(s => s.OrderID == it.OrderID && s.ProductName.Contains(query.ProductName)).Any());
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
                if (user.UserType==(int)CompanyUserType.Shop)
                {
                    expable.And(it => it.ShopID == user.ShopID);
                }
            }
            else
            {
                expable.And(it => it.UserID == user.UserID);
            }
            expable.AndIF(!string.IsNullOrEmpty(query.ExpressNo),it=>it.ExpressNo==query.ExpressNo);
            expable.AndIF(!string.IsNullOrEmpty(query.PayType), it => it.PayType == query.PayType);
            expable.AndIF(query.OrderType.HasValue, it => it.OrderType == query.OrderType);
            if (!string.IsNullOrEmpty(query.TimeType) && (query.StartTime.HasValue || query.EndTime.HasValue))
            {
                var endTime = query.EndTime.HasValue ? query.EndTime.Value.AddDays(1) : query.EndTime;
                switch (query.TimeType)
                {
                    case "order":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.OrderDate >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.OrderDate < endTime);
                        }
                        break;
                    case "pay":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.PayDate >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.PayDate < endTime);
                        }
                        break;
                    case "ship":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.ShippingDate >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.ShippingDate < endTime);
                        }
                        break;
                    case "finish":
                        {
                            expable.AndIF(query.StartTime.HasValue, it => it.FinishDate >= query.StartTime);
                            expable.AndIF(query.EndTime.HasValue, it => it.FinishDate < endTime);
                        }
                        break;
                }

            }
            expable.AndIF(query.Status.HasValue, it => it.OrderStatus == query.Status);
            expable.AndIF(!string.IsNullOrEmpty(query.OrderID), it => it.OrderID == query.OrderID);
            expable.AndIF(!string.IsNullOrEmpty(query.Phone), it => it.ShippPhone == query.Phone);
            expable.AndIF(query.Assess.HasValue, it => it.IsAssess == query.Assess && it.OrderStatus == (int)OrderStatus.Have);
        }
        [HttpGet]
        public async Task<IActionResult> Items([FromQuery] OrderPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_OrderItems>();
            var items = await orderService.Items(expable.ToExpression(), query);
            return Ok(items);
        }
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var model = await orderService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的订单不存在");
            }
            var items = await orderService.Items(id);
            return Ok(new { order = model, items });
        }
        [HttpGet]
        public async Task<IActionResult> PackageList(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ErrorCodeException(-1, "订单号错误");
            }
            var items = await orderService.PackageList(orderId);
            return Ok(items);
        }
        [HttpGet]
        public async Task<bool> ExistProduct(int productId)
        {
           return await orderService.Exist(it=>it.ProductID==productId);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TaksCancel()
        {
            var now= DateTime.Now;
            var items = await orderService.List(it=>it.OrderStatus==(int)OrderStatus.WaitPay && it.OrderDate.Value<= SqlFunc.DateAdd(now, -15, DateType.Minute), new Model.PagedModel.PagedModelBase() { Page=0});
            var logs = new List<Shop_OrderLog>();
            try
            {
                foreach (var item in items)
                {
                    if (await orderService.Cancel("", 0, item))
                    {
                        var log = new Shop_OrderLog();
                        log.CreateTime = now;
                        log.Status = (int)OrderStatus.Cancel;
                        log.FullName = "系统";
                        log.UserID = 0;
                        log.Remark = "系统超时自动取消订单";
                        log.OrderID = item.OrderID;
                        logs.Add(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
            if (logs.Count > 0)
            {
                await orderLogService.Add(logs);
            }
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TaksFinish()
        {
            var now = DateTime.Now;
            var items = await orderService.List(it => it.OrderStatus == (int)OrderStatus.WaitReceipt && it.OrderType==(int)OrderType.Product && it.OrderDate.Value <= SqlFunc.DateAdd(now, -15, DateType.Day), new Model.PagedModel.PagedModelBase() { Page = 0 });
            var logs = new List<Shop_OrderLog>();
            try
            {
                foreach (var item in items)
                {
                    if (await orderService.Finish("", 0, item))
                    {
                        var log = new Shop_OrderLog();
                        log.CreateTime = now;
                        log.Status = (int)OrderStatus.Have;
                        log.FullName = "系统";
                        log.UserID = 0;
                        log.Remark = "系统超时自动收货";
                        log.OrderID = item.OrderID;
                        logs.Add(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
            if (logs.Count > 0)
            {
                await orderLogService.Add(logs);
            }
            return Ok();
        }
        /// <summary>
        /// 自动任务，自动评价
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TaksComment()
        {
            var now = DateTime.Now;
            var items = await orderService.UpdateBatchAssess(it => (!SqlFunc.HasValue(it.IsAssess) || !it.IsAssess) && it.OrderStatus==(int)OrderStatus.Have && it.OrderDate.Value <= SqlFunc.DateAdd(now, -30, DateType.Day));
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Cancel([FromForm] string orderId)
        {
            var user = CurrentUser;
            var userId = 0;
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                userId = user.UserID;
            }
            if (await orderService.Cancel(orderId, userId))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderStatus.Cancel;
                if (userId > 0)
                {
                    log.FullName = user.FullName;
                    log.UserID = user.UserID;
                    log.Remark = "用户取消订单";
                }
                else
                {
                    log.SysUserID = user.UserID;
                    log.SysFullName = user.FullName;
                    log.Remark = "后台取消订单";
                }
                log.OrderID = orderId;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "取消错误，只有待付款才可取消！");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ItemRefund([FromForm] int id, [FromForm] int count=1)
        {
            var user = CurrentUser;
            var userId = 0;
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                userId = user.UserID;
            }
            if (await orderService.ItemRefund(id, count, userId))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderStatus.Refund;
                if (userId > 0)
                {
                    log.FullName = user.FullName;
                    log.UserID = userId;
                    log.Remark = "用户退单";
                }
                else
                {
                    log.SysUserID = userId;
                    log.SysFullName = user.FullName;
                    log.Remark = "后台退单";
                }
                //log.OrderID = orderId;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "取消错误，只有待发货才可退单！");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Refund([FromForm] string orderId)
        {
            var user = CurrentUser;
            var userId = 0;
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                userId = user.UserID;
            }
            if (await orderService.Refund(orderId,user.UserID))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderStatus.Refund;
                if (userId > 0)
                {
                    log.FullName = user.FullName;
                    log.UserID = user.UserID;
                    log.Remark = "用户退单";
                }
                else
                {
                    log.SysUserID = user.UserID;
                    log.SysFullName = user.FullName;
                    log.Remark = "后台退单";
                }
                log.OrderID = orderId;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "取消错误，只有待发货才可退单！");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Finish([FromForm] string orderId)
        {
            var user = CurrentUser;
            var userId = 0;
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                userId=user.UserID;
            }
            if (await orderService.Finish(orderId, userId))
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderStatus.Have;
                if (userId > 0)
                {
                    log.FullName = user.FullName;
                    log.UserID = user.UserID;
                    log.Remark = "用户完成订单";
                }
                else
                {
                    log.SysUserID = user.UserID;
                    log.SysFullName = user.FullName;
                    log.Remark = "后台完成订单";
                }
                log.OrderID = orderId;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "完成错误，只有待收货才可完成！");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ErpSend([FromForm] string data, [FromForm] string app_key)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(app_key))
            {
                throw new ErrorCodeException(-1, "发货错误，物流信息不全！");
            }
            if (app_key != "3068922137")
            {
                throw new ErrorCodeException(-1, "key错误！");
            }
            Common.StaticLog.Debug(data);
            var wrapper = JsonConvert.DeserializeObject<HupunSendModel>(data);
            var model = wrapper?.data ?? JsonConvert.DeserializeObject<HupunSendDataModel>(data);
            var expModel = new List<Model.Shop_Express>();
            try
            {
                var packageList = (model.packages ?? new List<HupunSendPackageModel>())
               .Where(it => it != null && !string.IsNullOrEmpty(it.wayBill)).ToList();
                var types = packageList.Select(it => it.deliveryCode);
                expModel = await httpService.GetAsync<List<Model.Shop_Express>>(ServiceNames.ShopBasicService, $"/api/express/list", isItem: true);//获取快递信息，匹配erp的快递编码
                foreach (var package in packageList)
                {
                    var exp = expModel.Find(it => it.ErpCode == package.deliveryCode);
                    if (exp != null)
                    {
                        package.deliveryCode = exp.ExpressType;
                    }
                }
                var modelExp = expModel.Find(it => it.ErpCode == model.delivery_code);
                if (modelExp != null)
                {
                    model.delivery_code = modelExp.ExpressType;
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
            var result = await orderService.ErpPackageSend(model);
            if (!result.Success)
            {
                throw new ErrorCodeException(-1, "发货错误，只有已付款/待发货才能发货！");
            }
            if (result.IsNewSend)
            {
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderStatus.WaitReceipt;
                log.Remark = "ERP发货";
                log.OrderID = result.Order.OrderID;
                await orderLogService.Add(log);
                var expInfo = expModel.Find(it => it.ExpressType == result.ExpressType);
                var wxExp = result.ExpressType;
                if (expInfo!=null)
                {
                    wxExp=expInfo.ExpressType;
                }
                await SyncWxOpen(result.Order, new OrderSend()
                {
                    OrderID = result.Order.OrderID,
                    ExpressNo = result.ExpressNo,
                    ExpressType = wxExp,
                    ExpressName = result.ExpressName
                });
            }
            if (result.SubscribeItems != null)
            {
                foreach (var item in result.SubscribeItems)
                {
                    item.ExpressType = "";
                    await SubscribeExpress(result.Order, item.ExpressNo, item.ExpressType);
                }
            }
            else
            {
                await SubscribeExpress(result.Order, result.ExpressNo, "");
            }
            return Content("{\"success\":true}", "application/json");
        }
        /// <summary>
        /// 接收快递物流轨迹推送
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExpressCallback()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var json = await reader.ReadToEndAsync();
                Common.StaticLog.Debug(json);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return Content("{\"success\":false,\"msg\":\"快递数据为空\"}", "application/json");
                }
                var model = JsonConvert.DeserializeObject<ExpressPushModel>(json);
                if (model == null || string.IsNullOrEmpty(model.number))
                {
                    return Content("{\"success\":false,\"msg\":\"快递单号为空\"}", "application/json");
                }
                var ok = await orderService.ReceiveExpressInfo(model);
                if (ok)
                {
                    return Content("{\"success\":true}", "application/json");
                }
                return Content("{\"success\":false,\"msg\":\"接收失败\"}", "application/json");
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return Content("{\"success\":false,\"msg\":\"接收失败\"}", "application/json");
            }
        }
        private async Task SubscribeExpress(Shop_Order order, string expressNo, string expressType)
        {
            try
            {
                if (string.IsNullOrEmpty(expressNo))
                {
                    return;
                }
                var settings = await settingsService.GetMasterSettings();
                var callBackUrl = (settings.ApiServicesUrl ?? "").TrimEnd('/') + "/api/order/order/ExpressCallback";
                await aliyunCommonService.ExpressSubscribe(expressNo, expressType, callBackUrl, order?.ShippPhone);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] Model.OrderSend model)
        {
            if(model==null || string.IsNullOrEmpty(model.ExpressNo) || string.IsNullOrEmpty(model.ExpressType))
            {
                throw new ErrorCodeException(-1, "发货错误，物流信息不全！");
            }
            var order = await orderService.Get(model.OrderID);
            if (order == null)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            if (order.OrderStatus!=(int)OrderStatus.WaitShip)
            {
                throw new ErrorCodeException(-1, "该订单不在待发货状态！");
            }
            if (await orderService.OrderSend(model))
            {
                var user = CurrentUser;
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.Status = (int)OrderStatus.WaitReceipt;
                log.SysUserID = user.UserID;
                log.SysFullName = user.FullName;
                log.Remark = "后台发货";
                log.OrderID = model.OrderID;
                await orderLogService.Add(log);
                await SyncWxOpen(order, model);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "发货错误，只有已付款/待发货才能发货！");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SyncWeiXinSend([FromForm] string orderIds)
        {
            if (string.IsNullOrEmpty(orderIds))
            {
                throw new ErrorCodeException(-1, "订单号错误");
            }
            var ids= orderIds.Split(',');
            var status = new int[] { (int)OrderStatus.HaveShip,(int)OrderStatus.WaitReceipt, (int)OrderStatus.Have };
            var models = await orderService.List(it=>ids.Contains(it.OrderID) && it.OrderType==(int)OrderType.Product && it.PayType== PayType.WeiXinPay.ToString() && status.Contains(it.OrderStatus.Value) && it.CompanyID==CurrentUser.CompanyID,new PagedModelBase() { Page=0});
            var expModels = await httpService.GetAsync<List<Model.Shop_Express>>(ServiceNames.ShopBasicService, $"/api/express/list", isItem: true);//获取快递信息，匹配erp的快递编码
            foreach (var model in models)
            {
                var expModel = expModels.Find(it => it.ExpressType == model.ExpressType);
                var expType = expModel != null ? expModel.WeiXinCode : model.ExpressType;
                await SyncWxOpen(model, new OrderSend() { ExpressName = model.ExpressName, ExpressType = expType, ExpressNo = model.ExpressNo });
            }
            return Ok();
        }
        private async Task SyncWxOpen(Model.Shop_Order model, OrderSend? sendModel=null)
        {
            if (model.PayType == PayType.WeiXinPay.ToString())
            {
                try
                {
                    var company = await companyConfigService.Get(model.CompanyGID);
                    var info = new UploadShippingInfoModel();
                    info.order_key = new OrderKeyModel();
                    info.order_key.order_number_type = 1;
                    info.order_key.mchid = company.Config.WeiXinMerchantID;
                    info.order_key.out_trade_no = model.OrderPayID;
                    info.delivery_mode = 1;
                    info.shipping_list = new List<ShippingListModel>();
                    var shipInfo = new ShippingListModel();
                    if (model.OrderType == (int)OrderType.Product)
                    {
                        info.logistics_type = 1;
                        if (sendModel != null)
                        {
                            shipInfo.express_company = sendModel.ExpressType;
                            shipInfo.tracking_no = sendModel.ExpressNo;
                        }
                    }
                    else
                    {
                        info.logistics_type = 3;
                    }
                    var items = await orderService.Items(it => it.OrderID == model.OrderID);
                    var descs = items.Select(it => it.ProductName);
                    shipInfo.item_desc = string.Join(';', descs);
                    info.shipping_list.Add(shipInfo);
                    info.upload_time = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
                    info.payer = new PayerModel();
                    info.payer.openid = model.OpenID;
                  
                   var res =   await Senparc.Weixin.WxOpen.AdvancedAPIs.Sec.Order.UploadShippingInfoAsync(company.Config.WeiXinOpenAppId, info);
                    if (res != null)
                    {
                        Common.StaticLog.Debug(JsonConvert.SerializeObject(res));
                    }
                   
                }
               catch(Exception ex)
                {
                    Common.StaticLog.Error(ex);
                }
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> UserOrderCount()
        {
            var user = CurrentUser;
            var list = await orderService.StatusCount(it => it.UserID == user.UserID);
            var counts=new Dictionary<string, int>();
            foreach (var item in list)
            {
                switch (item.FID)
                {
                    case (int)OrderStatus.WaitPay:
                        counts.Add("waitPay", item.Count);
                        break;
                    case (int)OrderStatus.WaitShip:
                        counts.Add("waitShip", item.Count);
                        break;
                    case (int)OrderStatus.WaitReceipt:
                        counts.Add("waitReceipt", item.Count);
                        break;
                }
            }
            var statuss = new int[] { (int)OrderReturnStatus.WaitFailure, (int)OrderReturnStatus.WaitReceipt , (int)OrderReturnStatus.WaitAuto , (int)OrderReturnStatus.WaitChange , (int)OrderReturnStatus.WaitExchange , (int)OrderReturnStatus.WaitReturn };
            var commentCount = await orderService.CountSync(it=>it.UserID==user.UserID && !it.IsAssess && it.OrderStatus==(int)OrderStatus.Have);
            counts.Add("return", await orderReturnService.Count(it=>it.UserID==user.UserID && statuss.Contains(it.Status.Value)));
            counts.Add("comment", commentCount);
            return Ok(counts);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> SyncErp([FromForm] string orderId)
        {
            var res = await orderService.SendOrder(null, orderId);
            if (res != null && res.code == 0 && res.data.success)
            {
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, res.data.error_msg);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateRemark([FromForm] string orderId, [FromForm] string remark)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(remark))
            {
                throw new ErrorCodeException(-1, "修改错误，信息不全！");
            }
            if (await orderService.UpdateRemark(orderId, remark))
            {
                var user = CurrentUser;
                var log = new Shop_OrderLog();
                log.CreateTime = DateTime.Now;
                log.SysUserID = user.UserID;
                log.SysFullName = user.FullName;
                log.Remark = "后台修改备注";
                log.OrderID = orderId;
                await orderLogService.Add(log);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "修改错误！");
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrderTotal([FromForm] string orderId, [FromForm] decimal money)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ErrorCodeException(-1, "修改错误，信息不全！");
            }
            var user = CurrentUser;
            if (await orderService.UpdateOrderTotal(orderId, money,user))
            {
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "修改错误！");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Recharge([FromForm] int integral)
        {
            try
            {
                var user = CurrentUser;
                if (integral <= 0)
                {
                    throw new ErrorCodeException(-1, "充值积分必须大于零！");
                }
                var company = await companyConfigService.Get(user.CompanyGID);
                if (company.Config.RechargeProductID <= 0)
                {
                    throw new ErrorCodeException(-1, "未配置充值商品！");
                }
                var skuid = $"{company.Config.RechargeProductID}_0";
                var clientId = 2;//职喜智能体
                var clientModel = await httpService.GetAsync<Model.Live_Clients>(ServiceNames.ClientsService, $"/api/clients/get?id={clientId}");//获取客户端信息
                if (clientModel == null)
                {
                    throw new ErrorCodeException(-1, "该客户端不存在！");
                }
                var userOpenIdObj = await httpService.GetAsync<Model.Live_PersonnelOpenIds>(ServiceNames.UsersService, $"/api/personnel/getopenid?id={user.UserID}&clientId={clientId}");//获取用户的openid
                if (userOpenIdObj == null)
                {
                    throw new ErrorCodeException(-1, "该用户不存在！");
                }
                var openId = userOpenIdObj.OpenId;
                var model = new Model.Shop_Order();
                model.OpenID = openId;
                model.CompanyGID = user.CompanyGID;
                model.CompanyID = user.CompanyID;
                model.ClientID = clientId;
                model.OrderType = (int)OrderType.Recharge;
                model.PreferentialMoney = 0;
                model.PlaceOrderType = (int)PlaceOrderType.Normal;
                var items = new List<Model.Shop_V_ShoppingCart>();
                var product = await httpService.GetAsync<Model.Shop_V_ShoppingCart>(ServiceNames.ProductService, $"/api/product/getsku?skuid={skuid}");
                if (product == null)
                {
                    throw new ErrorCodeException(-1, "该产品sku不存在！");
                }
                product.ProductCount = integral;
                items.Add(product);
                var shoppingCarts = new Model.ShoppingCart();
                shoppingCarts.Items = items;
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                model.PartnerID= umodel.PartnerID;
                model.PartnerGID= umodel.PartnerGID;
                model.Integral = integral;
                var has = await orderService.Submit(model, shoppingCarts, personnel:umodel);
                if (has)
                {
                    if (model.OrderStatus == (int)OrderStatus.WaitPay)
                    {
                        var payres = await payService.WeiXinPay(model, clientModel.AppID);
                        if (payres.Code == 0)
                        {
                            return Ok(payres.Data);
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "生成支付信息错误！");
                        }
                    }
                    else
                    {
                        return Ok();
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, "订单生成失败！");
                }

            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] Model.SubmitOrder order)
        {
            try
            {
                var now = DateTime.Now;
                if (string.IsNullOrEmpty(order.SkuID) && (order.Items == null || order.Items.Count == 0) && !order.Order.UserTaskID.HasValue)
                {
                    throw new ErrorCodeException(-1, "产品来源错误！");
                }
                var clientId = HttpContext.Request.Headers["client-id"].ToString();
                var user = CurrentUser;
                var userOpenIdObj = await httpService.GetAsync<Model.Live_UserOpenIds>(ServiceNames.UsersService, $"/api/user/getopenid?id={user.UserID}&clientId={clientId}");//获取用户的openid
                if (userOpenIdObj == null)
                {
                    throw new ErrorCodeException(-1, "该用户不存在！");
                }
                var openId = userOpenIdObj.OpenId;
                var model = order.Order;
                model.OpenID = openId;
                model.CompanyGID = user.CompanyGID;
                model.CompanyID = user.CompanyID;
                model.ClientID = Convert.ToInt32(clientId);
                model.OrderType = (int)OrderType.Product;
                model.PreferentialMoney = 0;
                model.PlaceOrderType = (int)PlaceOrderType.Normal;
                var items = new List<Model.Shop_V_ShoppingCart>();
                if (order.Order.UserTaskID.HasValue && order.Order.UserTaskID > 0)//完成活动任务提交订单
                {
                    if (await orderService.Exist(it => it.UserTaskID == order.Order.UserTaskID))
                    {
                        throw new ErrorCodeException(-1, "该活动任务已兑换！");
                    }
                    var userTask = await httpService.GetAsync<Model.Shop_V_ProductUserTask>(ServiceNames.ProductService, $"/api/productusertask/get?id={order.Order.UserTaskID}");
                    if (userTask == null)
                    {
                        throw new ErrorCodeException(-1, "该活动不存在！");
                    }
                    if (userTask.UserID != user.UserID)
                    {
                        throw new ErrorCodeException(-1, "您无法兑换该活动！");
                    }
                    if (userTask.Status == (int)Model.Enums.UserTaskStatus.Exchange)
                    {
                        throw new ErrorCodeException(-1, "该活动任务还未完成或已被兑换！");
                    }
                    model.TaskID = userTask.TaskID;
                    model.TaskMethod = userTask.TaskMethod;
                    model.TaskType = userTask.TaskType;
                    model.UserTaskID = userTask.ID;
                    if (model.TaskType == (int)TaskType.Turntable)
                    {
                        order.SkuID = !string.IsNullOrEmpty(userTask.UserSkuID) ? userTask.UserSkuID : userTask.UserProductID.ToString();//如果活动里有sku就用sku，没有就用productid
                    }
                    else
                    {
                        model.TaskPreferentialType = userTask.PreferentialType;
                        order.SkuID = !string.IsNullOrEmpty(userTask.SkuID) ? userTask.SkuID : userTask.ProductID.ToString();//如果活动里有sku就用sku，没有就用productid
                        if (userTask.PreferentialType.HasValue && userTask.PreferentialType == (int)PreferentialType.Money)
                        {
                            model.PreferentialMoney = userTask.PreferentialMoney;
                        }
                    }
                    switch (userTask.TaskType)
                    {
                        case (int)TaskType.Welfare:
                            {
                                model.PlaceOrderType = (int)PlaceOrderType.Welfare;
                            }
                            break;
                        case (int)TaskType.Task:
                            {
                                model.PlaceOrderType = (int)PlaceOrderType.Task;
                            }
                            break;
                        case (int)TaskType.Turntable:
                            {
                                model.PlaceOrderType = (int)PlaceOrderType.Turntable;
                            }
                            break;
                    }
                }
                var shoppingCarts = new Model.ShoppingCart();
                if (!string.IsNullOrEmpty(order.SkuID))//单个产品提交
                {
                    if (order.SkuID.IndexOf("_") < 0)
                    {
                        order.SkuID = order.SkuID + "_0";
                    }
                    shoppingCarts = await httpService.GetAsync<Model.ShoppingCart>(ServiceNames.ProductService, $"/api/product/getordersku?userId={user.UserID}&skuid={order.SkuID}");
                    if (shoppingCarts == null || shoppingCarts.Items==null || shoppingCarts.Items.Count==0)
                    {
                        throw new ErrorCodeException(-1, "该产品sku不存在！");
                    }
                    var product = shoppingCarts.Items[0];
                    if (product.ProductType == (int)ProductType.GroupCoupon || product.ProductType == (int)ProductType.GroupProduct)
                    {
                        model.OrderType = (int)OrderType.Group;
                    }
                    else if (product.ProductType == (int)ProductType.Virtual)
                    {
                        model.OrderType = (int)OrderType.Virtual;
                    }
                    else if (product.ProductType == (int)ProductType.Recharge)
                    {
                        model.OrderType = (int)OrderType.Recharge;
                    }
                    else
                    {
                        model.OrderType = (int)OrderType.Product;
                    }
                    model.ShopID = product.ShopID;
                    product.ProductCount = order.ProductCount ?? 1;
                }
                else//购物车提交
                {
                    model.OrderType = (int)OrderType.Product;
                    var cartIds = order.Items.Select(it => it.CartID).ToList();
                    //var list = await httpService.GetAsync<List<Model.Shop_V_ShoppingCart>>(ServiceNames.ProductService, $"/api/shoppingcart/list?page=0&ids={String.Join(',', cartIds)}", null, true);
                    //if (list == null || list.Count == 0)
                    //{
                    //    throw new ErrorCodeException(-1, "购物车里没有产品！");
                    //}
                    //if (order.Items.Count != list.Count)
                    //{
                    //    throw new ErrorCodeException(-1, "购物车发送变化，请返回重新操作！");
                    //}
                    //foreach (var item in list)
                    //{
                    //    var cart = order.Items.Find(it => it.CartID == item.CartID);
                    //    if (cart != null)
                    //    {
                    //        item.ProductCount = cart.ProductCount;
                    //    }
                    //}
                    //items.AddRange(list);
                    shoppingCarts= await httpService.GetAsync<Model.ShoppingCart>(ServiceNames.ProductService, $"/api/shoppingcart/getordercartamount?userId={user.UserID}&ids={String.Join(',', cartIds)}");
                    Common.StaticLog.Debug(JsonConvert.SerializeObject(shoppingCarts));
                    if (shoppingCarts==null || shoppingCarts.Items==null || shoppingCarts.Items.Count==0)
                    {
                        throw new ErrorCodeException(-1, "购物车里没有产品！");
                    }
                    if (order.Items.Count != shoppingCarts.Items.Count)
                    {
                        throw new ErrorCodeException(-1, "购物车发送变化，请返回重新操作！");
                    }
                    foreach (var item in shoppingCarts.Items)
                    {
                        model.ShopID = item.ShopID;
                        var cart = order.Items.Find(it => it.CartID == item.CartID);
                        if (cart != null)
                        {
                            item.ProductCount = cart.ProductCount;
                        }
                    }
                }
                var umodel = await authService.UserInfo<Model.Live_User>();
                if(!string.IsNullOrEmpty(order.CouponIDS))
                {
                    var coupons = await httpService.GetAsync<List<Model.Shop_V_CouponItems>>(ServiceNames.ProductService, $"/api/couponitems/list?ids={order.CouponIDS}",isItem:true);
                    var productIds = shoppingCarts.Items.Select(it => it.ProductID).ToList();
                    var couponProducts = await httpService.GetAsync<List<Model.Shop_V_CouponProduct>>(ServiceNames.ProductService, $"/api/coupon/products?ids={String.Join(',', productIds)}", isItem: true);
                    foreach (var coupon in coupons)
                    {
                        if (coupon.UserID != umodel.UserID)
                        {
                            throw new ErrorCodeException(-1, $"您无权使用{coupon.CouponName}优惠券！");
                        }
                        if (coupon.StartTime> now)
                        {
                            throw new ErrorCodeException(-1, $"{coupon.CouponName}优惠券还未到使用时间！");
                        }
                        if (coupon.EndTime < now)
                        {
                            throw new ErrorCodeException(-1, $"{coupon.CouponName}优惠券已过期！");
                        }
                        if (coupon.Status!=(int)CouponItemStatus.WaitUse)
                        {
                            throw new ErrorCodeException(-1, $"{coupon.CouponName}优惠券已被使用！");
                        }
                        if (coupon.CouponUseType!=(int)CouponUseType.ProductCurrency && couponProducts.Any(it => it.CouponID == coupon.CouponID))
                        {
                            throw new ErrorCodeException(-1, $"{coupon.CouponName}优惠券未找到可使用的商品！");
                        }
                    }
                    shoppingCarts.CouponProducts = couponProducts;
                    shoppingCarts.Coupons = coupons;
                }
                var has = await orderService.Submit(model, shoppingCarts, umodel,isIntegral: order.IsIntegral);
                if (has)
                {
                    if (model.OrderStatus == (int)OrderStatus.WaitPay)
                    {
                        var payres = await payService.WeiXinPay(model);
                        if (payres.Code == 0)
                        {
                            return Ok(payres.Data);
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "生成支付信息错误！");
                        }
                    }
                    else
                    {
                        return Ok();
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, "订单生成失败！");
                }

            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Write([FromForm] string number)
        {
            var user = CurrentUser;
            if (user.UserType != (int)UserTypeEnum.WriteUser)
            {
                throw new ErrorCodeException(-1, "您无权核销！");
            }
            var model = await orderService.GetItem(it => it.CouponNumber == number && it.CompanyID == user.CompanyID && it.ShopID == user.ShopID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该团购券不存在或你无权核销");
            }
            var product = await httpService.GetAsync<Model.Shop_Product>(ServiceNames.ProductService, $"/api/product/get?id={model.ProductID}");
            if (product == null)
            {
                throw new ErrorCodeException(-1, "关联的团购商品不存在");
            }
            if (product.EndTime < DateTime.Now)
            {
                throw new ErrorCodeException(-1, "该团购套餐已过期！");
            }
            var item = new Model.Shop_OrderItems();
            item.Status = (int)OrderStatus.Have;
            item.WriteTime = DateTime.Now;
            item.WriteUserID = user.UserID;
            item.WriteFullName = user.FullName;
            
            await orderService.UpdateItem(item);
            await orderService.Finish(model.OrderID);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Pay([FromForm] string orderId)
        {
            var clientId = HttpContext.Request.Headers["client-id"].ToString();
            
            var model = await orderService.Get(orderId);
            if(model == null)
            {
                throw new ErrorCodeException(-1, "该订单不存在！");
            }
            if (model.OrderStatus!=(int)OrderStatus.WaitPay)
            {
                throw new ErrorCodeException(-1, "该订单不在待付款状态或已支付！");
            }
            var user = CurrentUser;
            if (string.IsNullOrEmpty(model.OpenID))
            {
                var userOpenIdObj = await httpService.GetAsync<Model.Live_UserOpenIds>(ServiceNames.UsersService, $"/api/user/getopenid?id={user.UserID}&clientId={clientId}");//获取用户的openid
                if (userOpenIdObj == null)
                {
                    throw new ErrorCodeException(-1, "该用户不存在！");
                }
                model.OpenID = userOpenIdObj.OpenId;
            }
            try
            {
                var payres = await payService.WeiXinPay(model);
                if (payres.Code == 0)
                {
                    return Ok(payres.Data);
                }
                else
                {
                    throw new ErrorCodeException(-1, "生成支付信息错误！");
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "生成支付信息错误！");
            }
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ErrorCodeException(-1, "订单号错误！");
            }
            await orderService.Delete(orderId);
            var user = CurrentUser;
            var log = new Shop_OrderLog();
            log.CreateTime = DateTime.Now;
            log.SysUserID = user.UserID;
            log.SysFullName = user.FullName;
            log.Remark = "后台删除订单";
            log.OrderID = orderId;
            await orderLogService.Add(log);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        [HttpGet]
        public async Task<IActionResult> UpdateAssess(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ErrorCodeException(-1, "订单号错误！");
            }
            await orderService.UpdateAssess(orderId);
            return Ok();
        }
    }
}
