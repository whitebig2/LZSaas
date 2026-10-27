using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using LZ.Product.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Security.Policy;

namespace LZ.Product.API
{
    public class ProductTaskController(IProductTaskService productTaskService, ICompanyConfigService companyConfigService, IProductService productService,ISKUsService sKUsService, IEventPublisher publisher, IAuthService authService,IProductUserTaskService productUserTaskService,IHttpService httpService) : ProductControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_ProductTask>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.TaskName.Contains(query.Name));
            }
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                if (query.CompanyID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CompanyID);
                }
            }
            else
            {
                var companygid = HttpContext.Request.Headers["company-guid"].ToString();
                if (!string.IsNullOrEmpty(companygid))
                {
                    var companyinfo = await companyConfigService.Get(new Guid(companygid));
                    expable.And(it => it.CompanyID == companyinfo.Company.CompanyID);
                }
                else
                {
                    expable.And(it => it.CompanyID == user.CompanyID);
                }

            }
            expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
            expable.AndIF(!string.IsNullOrEmpty(query.Types), it => it.TaskType == Convert.ToInt32(query.Types));
            var exp = expable.ToExpression();
            var models = await productTaskService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Lottery([FromForm] int taskId)
        {
            var umodel = await authService.UserInfo<Model.Live_User>();
            //if (!umodel.IsShopPay)
            //{
            //    throw new ErrorCodeException(-1, "抱歉！目前商城仅限内部人员使用，暂未对外开放！");
            //}
            var company = await companyConfigService.Get(umodel.CompanyGID);
            var isClear = false;//抽奖次数是否初始化
            if (!umodel.TurntableDate.HasValue || umodel.TurntableDate.Value.Date != DateTime.Now.Date || !umodel.TurntableCount.HasValue)
            {
                isClear= true;
                umodel.TurntableCount = 0;
            }
            if (umodel.TurntableCount>= company.Config.TurntableCount)
            {
                throw new ErrorCodeException(-1, "您今日的次数已用完!");
            }
            if (taskId == 0)
            {
                throw new ErrorCodeException(-1, "来源错误!");
            }
            var task = await productTaskService.Get(taskId);
            if (task == null)
            {
                throw new ErrorCodeException(-1, "该活动不存在!");
            }
            if (task.Status != (int)Model.Enums.ProductStatus.List)
            {
                throw new ErrorCodeException(-1, "该活动已下架!");
            }
            var now = DateTime.Now;
            if (task.StartTime > now)
            {
                throw new ErrorCodeException(-1, "该活动还未开始!");
            }
            if (task.EndTime < now)
            {
                throw new ErrorCodeException(-1, "该活动已结束!");
            }
            if (task.TaskType != (int)TaskType.Turntable)
            {
                throw new ErrorCodeException(-1, "活动错误!");
            }
            var items = await productTaskService.WeightItems(it => it.TaskID == task.TaskID && it.ProductCount > it.UseCount);
            var totalWeight = 0;
            totalWeight = items.Sum(p => p.Weight.Value);
            var random = new Random();
            double randomValue = random.NextDouble() * totalWeight;
            double cumulativeWeight = 0;
            Model.Shop_ProductTaskItems? prize = null;
            // 遍历奖项，累加权重
            foreach (var item in items)
            {
                cumulativeWeight += item.Weight.Value;
                // 当随机数小于累加权重时，说明落在了当前奖项的区间内
                if (randomValue < cumulativeWeight)
                {
                    prize = item;
                    break;
                }
            }
            if (prize == null)
            {
                prize = items.Find(it => it.RewardType == (int)RewardType.Not);
                if (prize == null)
                {
                    throw new ErrorCodeException(-1, "奖品设置错误!");
                }
            }
            if (prize.RewardType == (int)RewardType.Integrate && prize.Integral.HasValue && prize.Integral > 0)
            {
                var model = new Model.Shop_ProductUserTask();
                model.CreateTime = now;
                model.UserID = umodel.UserID;
                model.TaskID = task.TaskID;
                model.Status = (int)Model.Enums.UserTaskStatus.Finish;
                model.Integral = prize.Integral;
                model.ProductName = prize.Name;
                model.TaskItemID = prize.ID;
                model.FullName = umodel.FullName;
                model.Picture = umodel.Picture;
                var has = await productUserTaskService.Add(model);
                if (has>0)
                {
                    var record = new IntegralLogEventData();
                    record.UserID = umodel.UserID;
                    record.Integral = prize.Integral.Value;
                    record.IntegralType = IntegralType.Lottery;
                    record.IsUpdate = true;
                    record.CompanyGID = umodel.CompanyGID.Value;
                    record.CompanyID = umodel.CompanyID.Value;
                    publisher.Publish(record);
                }
                
            }
            else if (prize.RewardType == (int)RewardType.Product)
            {
                var model = new Model.Shop_ProductUserTask();
                model.CreateTime = now;
                model.UserID = umodel.UserID;
                model.TaskID = task.TaskID;
                model.Status = (int)Model.Enums.UserTaskStatus.Exchange;
                model.StartTime = now;
                model.ProductCount = prize.ProductCount;
                model.ProductID = prize.ProductID;
                model.SkuID = prize.SkuID;
                model.SkuContent = prize.SkuContent;
                model.ProductName = prize.Name;
                model.TaskItemID = prize.ID;
                model.FullName = umodel.FullName;
                model.Picture = umodel.Picture;
                await productUserTaskService.Add(model);
                await productTaskService.AddItemUseCount(prize.ID);
            }
            try
            {
                var paras = new Dictionary<string, string>();
                paras.Add("userId", umodel.UserID.ToString());
                paras.Add("isClear", isClear.ToString().ToLower());
                await httpService.PutAsync(ServiceNames.UsersService, "/api/user/updateturntable", paras);
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            return Ok(new { id = prize.ID, name = prize.Name });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.SubmitProductTask data)
        {
            if (data.TaskModel==null)
            {
                throw new ErrorCodeException(-1, "信息错误!");
            }
            var model = data.TaskModel;
            if(model.TaskType==(int)TaskType.Task || model.TaskType == (int)TaskType.Welfare)
            {
                if (!model.ProductID.HasValue)
                {
                    throw new ErrorCodeException(-1, "请先选择产品!");
                }
            }
            if (model.ProductID.HasValue)
            {
                var product = await productService.Get(model.ProductID.Value);
                if (product == null)
                {
                    throw new ErrorCodeException(-1, "该产品不存在!");
                }
                if (!product.HasSKU)
                {
                    model.SkuID = product.ProductID + "_0";
                }
                var sku = await sKUsService.Get(model.SkuID);
                if (sku == null)
                {
                    throw new ErrorCodeException(-1, "该sku不存在!");
                }
                if (sku.Stock < model.ProductCount || sku.Stock <= 0)
                {
                    throw new ErrorCodeException(-1, "该产品库存不足!");
                }
                if (model.PreferentialType == (int)PreferentialType.Money && sku.SalePrice < model.PreferentialMoney)
                {
                    throw new ErrorCodeException(-1, "优惠价格不能大于产品价格!");
                }
                if (await productTaskService.Exist(it => it.ProductID == model.ProductID && it.Status == (int)Model.Enums.ProductStatus.List))
                {
                    throw new ErrorCodeException(-1, "该产品任务正在进行中!");
                }
            }
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.CreateTime = DateTime.Now;
            if (data.Items != null && data.Items.Count > 0)//奖品设置
            {
                foreach (var item in data.Items)
                {
                    if (!item.ProductCount.HasValue) item.ProductCount = 0;
                    if (item.RewardType ==(int)RewardType.Product && item.ProductID.HasValue)
                    {
                        var product = await productService.Get(item.ProductID.Value);
                        if (product == null)
                        {
                            throw new ErrorCodeException(-1, "该产品不存在!");
                        }
                        if (!product.HasSKU)
                        {
                            item.SkuID = product.ProductID + "_0";
                        }
                        var sku = await sKUsService.Get(item.SkuID);
                        if (sku == null)
                        {
                            throw new ErrorCodeException(-1, "该sku不存在!");
                        }
                        if (sku.Stock < model.ProductCount || sku.Stock <= 0)
                        {
                            throw new ErrorCodeException(-1, "该产品库存不足!");
                        }
                    }
                    item.UseCount = 0;
                }
            }
            model.TaskID = await productTaskService.Add(model);
            model.UseCount = 0;
            if (!model.ProductCount.HasValue) model.ProductCount = 0;
            if (model.TaskID > 0 && data.Items!=null && data.Items.Count>0)
            {
                foreach (var item in data.Items)
                {
                    item.TaskID=model.TaskID;
                }
                await productTaskService.AddItems(data.Items);
            }
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SubmitProductTask data)
        {
            if (data.TaskModel == null)
            {
                throw new ErrorCodeException(-1, "信息错误!");
            }
            var model = data.TaskModel;
            if (model.TaskType == (int)TaskType.Task || model.TaskType == (int)TaskType.Welfare)
            {
                if (!model.ProductID.HasValue)
                {
                    throw new ErrorCodeException(-1, "请先选择产品!");
                }
            }
            if (model.ProductID.HasValue)
            {
                var product = await productService.Get(model.ProductID.Value);
                if (product == null)
                {
                    throw new ErrorCodeException(-1, "该产品不存在!");
                }
                if (!product.HasSKU)
                {
                    model.SkuID = product.ProductID + "_0";
                }
                var sku = await sKUsService.Get(model.SkuID);
                if (sku == null)
                {
                    throw new ErrorCodeException(-1, "该sku不存在!");
                }
                if (sku.Stock < model.ProductCount || sku.Stock <= 0)
                {
                    throw new ErrorCodeException(-1, "该产品库存不足!");
                }
                if (model.PreferentialType == (int)PreferentialType.Money && sku.SalePrice < model.PreferentialMoney)
                {
                    throw new ErrorCodeException(-1, "优惠价格不能大于产品价格!");
                }
                if (await productTaskService.Exist(it => it.ProductID == model.ProductID && it.Status == (int)Model.Enums.ProductStatus.List && it.TaskID != model.TaskID))
                {
                    throw new ErrorCodeException(-1, "该产品任务正在进行中!");
                }
            }
            var user = CurrentUser;
            var updateItems = new List<Shop_ProductTaskItems>();
            var insertItems = new List<Shop_ProductTaskItems>();
            var itemIds= new List<int>();
            if (data.Items != null && data.Items.Count > 0)//奖品设置
            {
              
                foreach (var item in data.Items)
                {
                    if (!item.ProductCount.HasValue) item.ProductCount = 0;
                    if (item.RewardType == (int)RewardType.Product && item.ProductID.HasValue)
                    {
                        var product = await productService.Get(item.ProductID.Value);
                        if (product == null)
                        {
                            throw new ErrorCodeException(-1, "该产品不存在!");
                        }
                        if (!product.HasSKU)
                        {
                            item.SkuID = product.ProductID + "_0";
                        }
                        var sku = await sKUsService.Get(item.SkuID);
                        if (sku == null)
                        {
                            throw new ErrorCodeException(-1, "该sku不存在!");
                        }
                        if (sku.Stock < model.ProductCount || sku.Stock <= 0)
                        {
                            throw new ErrorCodeException(-1, "该产品库存不足!");
                        }
                    }
                    if (!item.UseCount.HasValue) item.UseCount = 0;
                    if (item.ID == 0)
                    {
                        item.TaskID = model.TaskID;
                        insertItems.Add(item);
                    }
                    else
                    {
                        itemIds.Add(item.ID);
                        updateItems.Add(item);
                    }
                }
            }
           var has = await productTaskService.UpdateAsync(model);
            if (has)
            {
                if (itemIds.Count > 0)
                {
                    await productTaskService.DeleteItem(itemIds.ToArray(), model.TaskID);
                }
                
                if (updateItems.Count > 0)
                {
                    itemIds = updateItems.Select(x => x.ID).ToList();
                    await productTaskService.UpdateItems(updateItems);
                }
                if (insertItems.Count > 0)
                {
                    await productTaskService.AddItems(insertItems);
                }
            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetActivity(int type)
        {
            var companygid = HttpContext.Request.Headers["company-guid"].ToString();
            var model = await productTaskService.Get(it => it.TaskType == type && it.Status == (int)ProductStatus.List && it.CompanyGID == new Guid(companygid));
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var items = await productTaskService.Items(it => it.TaskID == model.TaskID);
            return Ok(new { model, items });
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await productTaskService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var items = await productTaskService.Items(it => it.TaskID == model.TaskID);
            return Ok(new { model, items });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<bool> SetStatus([FromForm] int id, [FromForm] int status)
        {
            return await productTaskService.SetStatus(id, status);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await productTaskService.Delete(id);
            return Ok();
        }
    }
}
