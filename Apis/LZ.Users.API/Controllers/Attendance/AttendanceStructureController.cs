using LZ.Core.Services;
using LZ.Core;
using LZ.Model.PagedModel;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Core.Attributes;
using LZ.Users.Service.Interface;
using LZ.Model.Exceptions;
using LZ.Model.Enums.Attendance;
using LZ.Model.Model;
using LZ.Model;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class AttendanceStructureController(IAttendanceStructureService attendanceStructureService,IEntryService entryService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_AttendanceStructure>();
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            //{
            //    expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            //}
            //else
            //{
            //    expable.And(it => it.CompanyID == user.CompanyID);
            //}
            //if (query.From=="kq")
            //{
            //    expable.And(it => it.CompanyID == user.CompanyID);
            //}
            expable.AndIF(query.CompanyID.HasValue, t => t.CompanyID == query.CompanyID);
            expable.AndIF(query.CID.HasValue, t => t.CustomerID == query.CID);
            expable.AndIF(query.SID.HasValue, t => t.NavID == query.SID);
            expable.AndIF(query.structureID>0, t => t.ID == query.structureID);
            expable.AndIF(query.PartnerID.HasValue, t => t.ParentID == query.PartnerID);
            expable.AndIF(!string.IsNullOrEmpty(query.Type), t => t.AttendanceType == Convert.ToInt32(query.Type));
            var exp = expable.ToExpression();
            var models = await attendanceStructureService.List(exp, query);
            Dictionary<int, string> dics = new Dictionary<int, string>();
            if(query.From == "kq" && models.Count>0)
            {
                foreach(var x in models)
                {
                    if (!x.CustomerID.HasValue) continue;
                    if(!dics.ContainsKey(x.CustomerID.Value))
                    {
                        dics.Add(x.CustomerID.Value, x.CustomerName);
                    }
                }
            }
            return Ok(new { items = models, count = query.Count,dics });
        }

        [HttpPut]
        public async Task<IActionResult> Add([FromBody] Model.Live_AttendanceStructure model)
        {
            var expable = Expressionable.Create<Model.Live_AttendanceStructure>();
            model.Enable = true;
            int i = await attendanceStructureService.Add(model);
            if (i > 0 && model.ParentID == 0)
            {
                model.NavID = i;
                model.ID = i;
                await attendanceStructureService.Update(model);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await attendanceStructureService.Get(id);
            return Ok(new { items = model });
            //return await userService.Get(CurrentUser.UserID);
        }

        



        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_AttendanceStructure model)
        {
            if (!await attendanceStructureService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await attendanceStructureService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await attendanceStructureService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            if(await entryService.Exist(t=>t.StructureID==id && t.Status==2))
            {
                throw new ErrorCodeException(-1, "有在职员工入职到该车间，无法删除");
            }
            await attendanceStructureService.Delete(id);
            return Ok();
        }
    }
}
