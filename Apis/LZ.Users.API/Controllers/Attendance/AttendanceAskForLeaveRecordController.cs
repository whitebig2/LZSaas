using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    public class AttendanceAskForLeaveRecordController(IAuthService authService,IAttendanceAskForLeaveRecordService attendanceAskForLeaveRecordService,IEntryService entryService,IHttpService httpService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_AttendanceAskForLeaveRecord>();
            var puser = await authService.UserInfo<Model.SYS_CompanyUser>();
            var data = DateTime.Now;
            //{ id: 1, name: '客服', enabled: true },
            //{ id: 2, name: '服务商', enabled: true },
            //{ id: 3, name: '管理员', enabled: true },
            //{ id: 4, name: '财务', enabled: true },
            //{ id: 5, name: '人事', enabled: false }
            if (puser.UserType == 2)
            {
                expable.And(t => t.PartnerID == puser.PartnerID);
            }
            expable.And(t => t.Year == data.Year);
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.UserName.Contains(query.Name));
            }
            //expable.And(t => t.Status == 2 || (t.Status==4 && t.LeaveTime.HasValue && t.LeaveTime.Value.Year == data.Year));
            var exp = expable.ToExpression();
            var models = await attendanceAskForLeaveRecordService.List(exp, query);

            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_AttendanceAskForLeaveRecord model)
        {
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var expable = Expressionable.Create<Model.Live_AttendanceAskForLeaveRecord>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin)
            {
                expable.And(t => t.CompanyID == 0);
            }
            else
            {
                cid = CurrentUser.CompanyID;
                cgid = CurrentUser.CompanyGID;
                expable.And(t => t.CompanyID == 0 || t.CompanyID == cid);
                if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (info.UserType == 2)
                    {
                        model.PartnerID = info.PartnerID;
                    }
                }
                else if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    var info = await authService.UserInfo<Model.Live_Partner>();
                    model.PartnerID = info.PartnerID;
                }

            }
            expable.And(t => t.EntryID == model.EntryID);
            if (await attendanceAskForLeaveRecordService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "已添加!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            await attendanceAskForLeaveRecordService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_AttendanceAskForLeaveRecord> Get(int id)
        {
            return await attendanceAskForLeaveRecordService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<Model.Live_AttendanceAskForLeaveRecord> GetLeaveData(int id,int year)
        {
            return await attendanceAskForLeaveRecordService.Get(t=>t.EntryID==id && t.Year==year);
            //return await userService.Get(CurrentUser.UserID);
        }
        public record RealNameMessageRequest(string PersonName, string IdNo, string Phone, string IdType, string RedirectUrl);
        [HttpPost]
        public async Task<IActionResult> RealName([FromBody] Live_V_AttendanceAskForLeaveRecord model)
        {
            RealNameMessageRequest request = new RealNameMessageRequest(model.UserName,model.CardID,model.Phone??"18768541109","0","https://www.baidu.com");

            var modeljson = JsonConvert.SerializeObject(request);
            var obj = await httpService.PostModelAsync(ServiceNames.CustomerService, "/api/FxqApi/PersonRealNameMessage", modeljson);
            return Ok(new {obj, request,data = modeljson });
        }
        [HttpPost]
        public async Task<IActionResult> BatchCreate()
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            var puser = await authService.UserInfo<Model.SYS_CompanyUser>();
            var data = DateTime.Now;
            //{ id: 1, name: '客服', enabled: true },
            //{ id: 2, name: '服务商', enabled: true },
            //{ id: 3, name: '管理员', enabled: true },
            //{ id: 4, name: '财务', enabled: true },
            //{ id: 5, name: '人事', enabled: false }
            if (puser.UserType == 2)
            {
                expable.And(t => t.PartnerID == puser.PartnerID);
            }
            expable.And(t => t.Status==2);
            expable.And(t => SqlFunc.Subqueryable<Model.Live_AttendanceStructure>().Where(s => s.CustomerID == t.CustomerID).Any());
            //expable.And(t => t.Status == 2 || (t.Status==4 && t.LeaveTime.HasValue && t.LeaveTime.Value.Year == data.Year));

            var exp = expable.ToExpression();
            var models = await entryService.List(exp, new UserPagedModel() { Page=0 });

            var expable_leave = Expressionable.Create<Model.Live_V_AttendanceAskForLeaveRecord>();
            expable_leave.And(t => t.Year == data.Year);
            var lmodel = await attendanceAskForLeaveRecordService.List(expable_leave.ToExpression(), new UserPagedModel() { Page = 0 });
            List<Model.Live_AttendanceAskForLeaveRecord> list = new List<Model.Live_AttendanceAskForLeaveRecord>();
            foreach (var x in models)
            {
                var e = lmodel.Find(t => t.EntryID == x.ID);
                if (e != null)
                {
                    continue;
                }
                Model.Live_AttendanceAskForLeaveRecord item = new Model.Live_AttendanceAskForLeaveRecord();
                item.CompanyID = x.CompanyID;
                item.CompanyGID = x.CompanyGID;
                item.CustomerID = x.CustomerID;
                item.EntryID = x.ID;
                item.PartnerID = x.PartnerID;
                item.Year = data.Year;
                item.UserID = x.UserID;
                item.UserName = x.Name;
                item.CardID = x.CardID;
                item.CreateTime = data;
                list.Add(item);
            }
            if (list.Count > 0)
            {
                await attendanceAskForLeaveRecordService.Add(list);
            }




            return Ok(new { items = models });
        }
    }
}
