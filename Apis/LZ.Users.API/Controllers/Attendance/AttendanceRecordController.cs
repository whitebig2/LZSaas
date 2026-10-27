using LZ.Core.Attributes;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class AttendanceRecordController(IAttendanceRecordService attendanceRecordService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query,string statusList)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_AttendanceRecord>();
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            //{
            //    expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            //}
            //else
            //{
            //    expable.And(it => it.CompanyID == user.CompanyID);
            //}
            //if(CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            //{
            //    UserPagedModel userPagedModel = new UserPagedModel();
            //    userPagedModel.Page = 0;
            //    var list = await attendanceStructureService.List(t => t.CompanyID == cid && t.ParentID == 0, userPagedModel);
            //    if(list.Count>0)
            //    {
            //        List<int> arr = new List<int>();
            //        foreach(var x in list)
            //        {
            //            if (!x.CustomerID.HasValue) continue;
            //            arr.Add(x.CustomerID.Value);
            //        }
            //        if(arr.Count>0)
            //        {
            //            expable.And(t => t.CustomerID.HasValue && arr.Contains(t.CustomerID.Value));
            //        }

            //    }
            //}
            string d = "";
            if(query.Year.HasValue && query.Month.HasValue)
            {
                var month = query.Month < 10 ? ("0" + query.Month) : query.Month.ToString();
                d = query.Year + "-" + month;
                expable.And(t => t.SignDate.HasValue && t.SignDate.Value.ToString("yyyy-MM") == d);
            }
            if (!string.IsNullOrEmpty(statusList))
            {
                var arr = statusList.Split(',');
                expable.And(t=>arr.Contains(t.Status.ToString()));
            }
            expable.AndIF((query.Status.HasValue && query.Status != 0), t => t.Status == query.Status);
            expable.AndIF(query.UserID.HasValue, t => t.UserID == query.UserID);
            expable.AndIF(query.CompanyID.HasValue, t => t.CompanyID == query.CompanyID);
            expable.AndIF(query.AttendanceType.HasValue, t => t.AttendanceType == query.AttendanceType);
            expable.AndIF(query.CID.HasValue, t => t.CustomerID == query.CID);
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.UserName.Contains(query.Name));
            expable.AndIF(query.structureID > 0, t => t.StructureID == query.structureID);
            expable.AndIF(query.StartTime.HasValue, t => t.SignDate>=query.StartTime);
            expable.AndIF(!string.IsNullOrEmpty(query.Type), t => t.Type >= Convert.ToInt32(query.Type));
            expable.AndIF(query.EndTime.HasValue, t => t.SignDate < query.EndTime);
            var exp = expable.ToExpression();
            var models = await attendanceRecordService.ListView(exp, query);
            return Ok(new { items = models, count = query.Count, d });
        }
    }
}
