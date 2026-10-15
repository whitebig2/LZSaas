using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.Extensions;
using System.Security.Policy;
using System.Threading.Tasks;

namespace LZ.Users.API.Controllers
{
    public class IntegralController(IIntegralService integralService, IEventPublisher publisher, IUserService userService, IAuthService authService, IHttpService httpService, IPermissionService permissionService, IEntryService entryService, ICompanyConfigService companyConfigService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_IntegralLog>();
            //if (!string.IsNullOrEmpty(query.Name))
            //{
            //    expable.And(it => it.FullName.Contains(query.UserName));
            //}
            //是否总后台账号和自营公司
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
                if (user.PlatformType == PlatformType.XFZMini)
                {
                    expable.And(it => it.UserID == user.UserID);
                }
                else
                {
                    expable.AndIF(query.UserID.HasValue, it => it.UserID == query.UserID);
                }

            }
            else
            {
                expable.AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
            }
            var types = new int[] { };
            if (user.PlatformType == PlatformType.XFZMini)
            {
                expable.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType == PlatformType.LZMini)
            {
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(it => it.PartnerID == umodel.PartnerID);
                expable.AndIF(query.UserID.HasValue, it => it.IntegralType == (int)IntegralType.PartnerGift);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                var users = await userService.List(it => it.FullName.Contains(query.Name), new UserPagedModel { Page = 0 });
                var userIds = users.Select(x => x.UserID);
                expable.And(it => userIds.Contains(it.UserID.Value));
            }
            if (!string.IsNullOrEmpty(query.PartnerName))
            {
                var partnerList = await httpService.GetAsync<List<Model.Live_Partner>>(ServiceNames.BasicService, $"/api/partner/list?page=0&name={query.PartnerName}", isItem: true);
                var partnerIds = partnerList.Select(x => x.PartnerID);
                expable.And(it => partnerIds.Contains(it.PartnerID.Value));
            }
            expable.AndIF(query.UserID.HasValue, it => it.UserID == query.UserID);
            expable.AndIF(query.JobID.HasValue, it => it.FromJobID == query.JobID);
            var exp = expable.ToExpression();
            var models = await integralService.List(exp, query);
            if (user.PlatformType != PlatformType.XFZMini && models.Count > 0)
            {
                var userIds = models.Select(x => x.UserID);
                var users = await userService.List(it => userIds.Contains(it.UserID), new UserPagedModel { Page = 0 });
                foreach (var model in models)
                {
                    var u = users.Find(it => it.UserID == model.UserID);
                    if (u != null)
                    {
                        model.FullName = u.FullName;
                    }
                }
                var pids = models.Where(it => it.PartnerID.HasValue).Where(it => it.PartnerID > 0).Select(x => x.PartnerID);//获得服务商名称
                if (pids.Count() > 0)
                {
                    var partnerList = await httpService.GetAsync<List<Model.Live_Partner>>(ServiceNames.BasicService, $"/api/partner/list?page=0&partnerIDS={String.Join(',', pids)}", isItem: true);
                    foreach (var model in models)
                    {
                        var u = partnerList.Find(it => it.PartnerID == model.PartnerID);
                        if (u != null)
                        {
                            model.PartnerName = u.PartnerName;
                        }
                    }
                }

            }
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> CatagoryStatistics([FromQuery] IntegralCatagoryStatisticsDto query)
        {
            if(!query.startTime.HasValue)
            {
                throw new ErrorCodeException(-1, "开始时间不能为空！");
            }
            if(!query.endTime.HasValue)
            {
                throw new ErrorCodeException(-1, "结束时间不能为空！");
            }
            var result = await integralService.CatagoryStatistics(query.startTime.Value, query.endTime.Value);
            return Ok(result);
        }
        [HttpPost]
        [OperationLogAction("积分充值：{userId};金额：{integral}")]
        public async Task<IActionResult> Recharge([FromForm] int userId, [FromForm] int integral, [FromForm] string remark, [FromForm] int integralType = 10)
        {
            var user = CurrentUser;
            var auth = await permissionService.CompanyLicense("users", "integralRecharge", user.UserID);
            if (!auth)
            {
                throw new ErrorCodeException(-1, "您无权操作充值！");
            }
            var record = new IntegralLogEventData();
            record.UserID = userId;
            record.Integral = integral;
            record.IntegralType = integralType == (int)IntegralType.Company ? IntegralType.Company : IntegralType.Recharge;
            record.IsUpdate = true;
            record.CompanyGID = user.CompanyGID.Value;
            record.CompanyID = user.CompanyID;
            record.Remark = remark;
            publisher.Publish(record);
            return Ok();
        }
        [HttpPost]
        [OperationLogAction("服务商赠送积分：{userId};金额：{integral}")]
        public async Task<IActionResult> PartnerGift([FromForm] int userId, [FromForm] int integral, [FromForm] string remark)
        {
            if (integral <= 0)
            {
                throw new ErrorCodeException(-1, "充值积分必须大于零！");
            }
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var partner = await httpService.GetAsync<Model.Live_Partner>(ServiceNames.BasicService, $"/api/partner/getinfo?id={model.PartnerGID}");
            if (partner == null)
            {
                throw new ErrorCodeException(-1, "该服务商不存在！");
            }
            if (partner.Integral < integral)
            {
                throw new ErrorCodeException(-1, "该服务商积分不足！");
            }
            var record = new IntegralLogEventData();
            record.UserID = userId;
            record.Integral = integral;
            record.IntegralType = IntegralType.PartnerGift;
            record.IsUpdate = true;
            record.CompanyGID = user.CompanyGID.Value;
            record.CompanyID = user.CompanyID;
            record.PartnerID = model.PartnerID.Value;
            record.PartnerGID = model.PartnerGID.Value;
            record.Remark = remark;
            publisher.Publish(record);
            return Ok();
        }
        //[HttpPost]
        //[OperationLogAction("服务商积分充值：{partnerId};金额：{integral}")]
        //public IActionResult PartnerRecharge([FromForm] int partnerId, [FromForm] Guid partnerGid, [FromForm] int integral, [FromForm] string remark)
        //{
        //    var user = CurrentUser;
        //    var record = new IntegralLogEventData();
        //    record.UserID = 0;
        //    record.Integral = integral;
        //    record.IntegralType = IntegralType.PartnerRecharge;
        //    record.CompanyGID = user.CompanyGID.Value;
        //    record.CompanyID = user.CompanyID;
        //    record.PartnerID = partnerId;
        //    record.PartnerGID = partnerGid;
        //    record.Remark= remark;
        //    publisher.Publish(record);
        //    return Ok();
        //}
        [HttpPost]
        [OperationLogAction("岗位积分充值：{jobId};金额：{integral}")]
        public async Task<IActionResult> JobRecharge([FromForm] int jobId, [FromForm] int integral, [FromForm] string remark)
        {
            if (integral <= 0)
            {
                throw new ErrorCodeException(-1, "充值积分必须大于零！");
            }
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var partner = await httpService.GetAsync<Model.Live_Partner>(ServiceNames.BasicService, $"/api/partner/getinfo?id={model.PartnerGID}");
            if (partner == null)
            {
                throw new ErrorCodeException(-1, "该服务商不存在！");
            }
            if (partner.Integral < integral)
            {
                throw new ErrorCodeException(-1, "该服务商积分不足！");
            }
            var record = new IntegralLogEventData();
            record.UserID = 0;
            record.Integral = integral;
            record.IntegralType = IntegralType.JobRecharge;
            record.CompanyGID = user.CompanyGID.Value;
            record.CompanyID = user.CompanyID;
            record.PartnerID = model.PartnerID.Value;
            record.PartnerGID = model.PartnerGID.Value;
            record.FromJobID = jobId;
            record.Remark = remark;
            publisher.Publish(record);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ReceiveRegister()
        {
            var user = CurrentUser;
            var companyConfig = await companyConfigService.Get(user.CompanyGID);
            if (!companyConfig.Config.IsShopRegisterIntegral)
            {
                throw new ErrorCodeException(-1, "不支持赠送积分！");
            }
            if (!await entryService.Exist(it => it.UserID == user.UserID && it.Status == (int)UserStatus.Entry && it.CompanyID == user.CompanyID))
            {
                throw new ErrorCodeException(-1, "您未入职，无法获得赠送积分！");
            }
            if (await integralService.Exist(it => it.UserID == user.UserID && it.IntegralType == (int)IntegralType.Register))
            {
                throw new ErrorCodeException(-1, "您已获得过注册积分，无法再次获得！");
            }
            var record = new IntegralLogEventData();
            record.UserID = user.UserID;
            record.Integral = companyConfig.Config.ShopRegisterIntegral;
            record.IntegralType = IntegralType.Register;
            record.CompanyGID = user.CompanyGID.Value;
            record.CompanyID = user.CompanyID;
            record.Remark = "在职赠送积分";
            record.IsUpdate = true;
            publisher.Publish(record);
            return Ok(true);
        }
    }
}
