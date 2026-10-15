using LZ.Basic.Service.Interface;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class NotificationController(INotificationService notificationService, IAuthService authService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_Notification>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(it => it.PartnerID == umodel.PartnerID);
                //服务商下关联了客户单位并且是管理者才有权限查看系统通知
                expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.IsManage).Any());
                if (umodel.UserGID.HasValue)
                {
                    expable.And(it => !it.UserIDS.Contains(umodel.UserGID.Value.ToString()));
                }

            }
            if (query.Status.HasValue && query.Status > 0)
            {
                expable.And(it => it.Status == query.Status);
            }
            if (!string.IsNullOrEmpty(query.Title))
            {
                expable.And(t => t.Title.Contains(query.Title));
            }
            var exp = expable.ToExpression();
            var models = await notificationService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Read([FromForm] int id)
        {
            var model = await notificationService.Get(id);
            if(model == null)
            {
                throw new ErrorCodeException(-1, "信息不存在");
            }
            var umodel = await authService.UserInfo<Model.Live_Personnel>();
            if (string.IsNullOrEmpty(model.UserIDS))
            {
                model.UserIDS = umodel.UserGID + ",";
            } else
            {
                model.UserIDS += umodel.UserGID + ",";
            }
            await notificationService.Update(model);
            return Ok();
        }
    }
}
