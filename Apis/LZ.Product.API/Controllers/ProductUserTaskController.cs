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
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Product.API
{
    public class ProductUserTaskController(IProductUserTaskService productUserTaskService,IProductTaskService productTaskService,ICompanyConfigService companyConfigService, IEventPublisher publisher, IAuthService authService) : ProductControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_ProductUserTask>();
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
                if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
                {
                    if (query.From == "my" || !query.Type.HasValue)
                    {
                        expable.And(it => it.UserID == user.UserID);
                    }
                    expable.AndIF(query.Type.HasValue,it => it.TaskType == query.Type);
                }
            }
            var exp = expable.ToExpression();
            var models = await productUserTaskService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<int> TurntableCount()
        {
            var user = await authService.UserInfo<Model.Live_User>();
            if (!user.TurntableDate.HasValue || user.TurntableDate.Value.Date != DateTime.Now.Date)
            {
                user.TurntableCount = 0;
            }
            return user.TurntableCount.HasValue ? user.TurntableCount.Value : 0;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] int taskId)
        {
            var umodel = await authService.UserInfo<Model.Live_User>();
            //if (!umodel.IsShopPay)
            //{
            //    throw new ErrorCodeException(-1, "抱歉！目前商城仅限内部人员使用，暂未对外开放！");
            //}
            if (taskId == 0)
            {
                throw new ErrorCodeException(-1, "来源错误!");
            }
            var task = await productTaskService.Get(taskId);
            if (task == null)
            {
                throw new ErrorCodeException(-1, "该活动不存在!");
            }
            if (task.Status!=(int)Model.Enums.ProductStatus.List)
            {
                throw new ErrorCodeException(-1, "该活动已下架!");
            }
            var now = DateTime.Now;
            if (task.StartTime> now)
            {
                throw new ErrorCodeException(-1, "该活动还未开始!");
            }
            if (task.EndTime < now)
            {
                throw new ErrorCodeException(-1, "该活动已结束!");
            }
            var user = CurrentUser;
            if (task.ProductCount <= task.UseCount)
            {
                throw new ErrorCodeException(-1, "该活动已被抢完!");
            }
            if (await productUserTaskService.Exist(it => it.UserID == user.UserID && (it.ProductID == task.ProductID || it.TaskID == task.TaskID)))
            {
                throw new ErrorCodeException(-1, "您已领取过该商品任务!");
            }

            var model = new Model.Shop_ProductUserTask();
            model.CreateTime = now;
            model.UserID = user.UserID;
            model.TaskID = task.TaskID;
            model.Status = (int)Model.Enums.UserTaskStatus.Oning;
            model.StartTime = now;
            model.HaveCount = 0;
            await productUserTaskService.Add(model);
            if (model.ID > 0)
            {
                await productTaskService.AddUseCount(model.TaskID.Value);
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Cancel([FromForm] int id)
        {
            if (id == 0)
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var user = CurrentUser;
            if (!await productUserTaskService.Cancel(id,user.UserID))
            {
                throw new ErrorCodeException(-1, "取消错误!");
            }
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_V_ProductUserTask> Get(int id)
        {
            var model = await productUserTaskService.GetView(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPost]
        public async Task<IActionResult> ShareJob([FromForm] int id)
        {
            var user = CurrentUser;
            if (id == 0 || user.UserID==0) return Ok();
            
            var now = DateTime.Now;
            var model = await productUserTaskService.GetView(it=>it.UserID== user.UserID && it.TaskMethod==(int)TaskMethod.ShareJob && it.Status==(int)UserTaskStatus.Oning && it.TaskStartTime<=now && it.TaskEndTime>=now);
            if (model != null)
            {
                if (model.ShareJobType == 1 || (model.ShareJobType==2 && id==model.JobID))//任务岗位分享
                {
                    var itemModel = new Shop_ProductUserTaskItems();
                    itemModel.FromID = id;
                    itemModel.TaskMethod = model.TaskMethod;
                    itemModel.UserTaskID = model.ID;
                    itemModel.CreateTime = now;
                    await productUserTaskService.AddItem(itemModel);
                    if (itemModel.ID>0 && model.HaveCount + 1 >= model.ProductCount)
                    {
                        await productUserTaskService.SetStatus(model.ID, UserTaskStatus.Exchange);
                    }
                }
            }
            var record = new IntegralLogEventData();
            record.UserID = user.UserID;
            record.FromJobID = id;
            record.IntegralType = IntegralType.ShareJob;
            record.CompanyGID = user.CompanyGID.Value;
            record.CompanyID = user.CompanyID;
            record.IsUpdate = true;
            publisher.Publish(record);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            if (!await productUserTaskService.Delete(id, user.UserID))
            {
                throw new ErrorCodeException(-1, "删除错误!");
            }
            return Ok();
        }
    }
}
