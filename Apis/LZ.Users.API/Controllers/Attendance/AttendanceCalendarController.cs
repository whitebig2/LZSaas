using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums.Attendance;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class AttendanceCalendarController(IAuthService authService, IAttendanceCalendarService attendanceCalendarService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            var expable = Expressionable.Create<Model.Live_AttendanceCalendar>();
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            //{
            //    expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            //}
            //else
            //{
            //    expable.And(it => it.CompanyID == cid);
            //}
            expable.AndIF(query.Year.HasValue, t => t.Year == query.Year);
            expable.AndIF(query.Month.HasValue, t => t.Month == query.Month);
            expable.AndIF(query.Status.HasValue, t => t.Status == query.Status);
            var exp = expable.ToExpression();
            var models = await attendanceCalendarService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        
        [HttpPost]
        public async Task<IActionResult> AddBatch()
        {
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                cid = CurrentUser.CompanyID;
                cgid = CurrentUser.CompanyGID;
            }
            var year = DateTime.Now.Year;
            var now = Convert.ToDateTime(year + "-01-01");
            var list = await attendanceCalendarService.List(t => t.Year == year, query);
            List<Live_AttendanceCalendar> models = new List<Live_AttendanceCalendar>();
            while(now.Year==year)
            {
                var item = list.Find(t => t.Year == now.Year && t.Month == now.Month && t.Day == now.Day);
                if (item == null)
                {
                    Live_AttendanceCalendar model = new Live_AttendanceCalendar();
                    model.CompanyID = cid;
                    model.CompanyGID = cgid;
                    model.Calendar = now;
                    model.Year = now.Year;
                    model.Month = now.Month;
                    model.Day = now.Day;
                    model.Status = 1;
                    model.DayEnum = (int)now.DayOfWeek;
                    var week = ((AttendanceDayOfWeekEnum)now.DayOfWeek).ToString();
                    model.WeekDay = week;
                    models.Add(model);
                }
                now = now.AddDays(1);
            }
            await attendanceCalendarService.Add(models);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await attendanceCalendarService.Get(id);
            return Ok(new { items = model });
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetails(int cid)
        {
            var date = DateTime.Today;
            if(await attendanceCalendarService.ExistDate(date,cid,true))
            {

            }
            return Ok();
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_AttendanceCalendar model)
        {
            if (!await attendanceCalendarService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await attendanceCalendarService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await attendanceCalendarService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await attendanceCalendarService.Delete(id);
            return Ok();
        }
    }
}
