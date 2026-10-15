using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]

    public class AttendanceConfigController(IAttendanceConfigService attendanceConfigService, IAttendanceDetailService attendanceDetailService ,IAuthService authService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            var expable = Expressionable.Create<Model.Live_V_AttendanceConfig>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == cid);
            }
            expable.AndIF(query.CID.HasValue, t => t.CustomerID==query.CID);
            expable.AndIF(!string.IsNullOrEmpty(query.DayWorkType), t => t.DayWorkType == query.DayWorkType);
            expable.AndIF(!string.IsNullOrEmpty(query.Type), t => t.Type == Convert.ToInt32(query.Type));
            var exp = expable.ToExpression();
            var models = await attendanceConfigService.ListV(exp, query);
            
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Model.Live_AttendanceConfig model, [FromForm] List<Model.Live_AttendanceDetail> list)
        {
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var expable = Expressionable.Create<Model.Live_AttendanceConfig>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin)
            {
                expable.And(t => t.CompanyID == 0);
            }
            else
            {
                cid = CurrentUser.CompanyID;
                cgid = CurrentUser.CompanyGID;
                expable.And(t => t.CompanyID == 0 || t.CompanyID == cid);
            }
            expable.And(t => t.Name == model.Name);
            if (await attendanceConfigService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "名称已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.GID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            int i =  await attendanceConfigService.Add(model);
            if (list.Count>0)
            {
                int sort = 1;
                foreach(var x in list)
                {
                    x.CompanyID = cid;
                    x.CompanyGID = cgid;
                    x.ConfigID = i;
                    x.Sort = sort;
                    x.CreateTime = model.CreateTime;
                    sort++;
                }
                await attendanceDetailService.Add(list);
            }
            return Ok(new { model, list });
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await attendanceConfigService.Get(id);
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            var expable = Expressionable.Create<Model.Live_AttendanceDetail>();
            expable.And(t => t.ConfigID == id);
            var list = await attendanceDetailService.List(expable.ToExpression(), query);
            return Ok(new { model,list });
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetDayWorkDetail([FromBody] Live_V_JobsDayWork job)
        {
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            List<int> ids = new List<int>();
            if(job.AttendancePlan.HasValue)
            {
                ids.Add(job.AttendancePlan.Value);
            }
            if (job.NightAttendancePlan.HasValue)
            {
                ids.Add(job.NightAttendancePlan.Value);
            }
            if(ids.Count==0)
            {
                return Ok(new { });
            }
            else
            {
                var expable = Expressionable.Create<Model.Live_AttendanceDetail>();
                expable.And(t => ids.Contains(t.ConfigID));
                var list = await attendanceDetailService.List(expable.ToExpression(), query);
                return Ok(new { items = list });
            }
            
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Model.Live_AttendanceConfig model, [FromForm] List<Model.Live_AttendanceDetail> list)
        {
            if (!await attendanceConfigService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await attendanceConfigService.Update(model);
            await attendanceDetailService.Delete(t => t.ConfigID == model.ID);
            if (list.Count > 0)
            {
                int sort = 1;
                foreach (var x in list)
                {
                    x.CompanyID = model.CompanyID;
                    x.CompanyGID = model.CompanyGID;
                    x.ConfigID = model.ID;
                    x.Sort = sort;
                    x.CreateTime = DateTime.Now;
                    sort++;
                }
                await attendanceDetailService.Add(list);
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await attendanceConfigService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await attendanceConfigService.Delete(id);
            await attendanceDetailService.Delete(t => t.ConfigID == id);
            return Ok();
        }
    }
}
