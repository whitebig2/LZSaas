using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Enums.Attendance;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    public class AttendanceApplyController(IAttendanceApplyService attendanceApply, IAttendanceStructureService attendanceStructureService, IAuthService authService, IAttendanceApplyLogService attendanceApplyLogService, IAttendanceRecordService attendanceRecordService, IEntryService entryService, IHttpService httpService,IAttendanceDetailService attendanceDetailService,IAttendanceAskForLeaveRecordService attendanceAskForLeaveRecordService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_AttendanceApply>();
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
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                if (umodel.UserGID.HasValue)
                {
                    expable.And(it => it.ManageGIDS.Contains(umodel.UserGID.Value.ToString()));
                }
            }
            if ((!string.IsNullOrEmpty(query.Type)) && query.Type != "0")
            {
                expable.And(t => t.ApplyType == Convert.ToInt32(query.Type));
            }
            else
            {
                if (!string.IsNullOrEmpty(query.Types))
                {
                    var arr = query.Types.Split(",");
                    expable.And(t => arr.Contains(t.ApplyType.ToString()));

                }
            }
            string d = "";
            if (query.Year.HasValue && query.Month.HasValue)
            {
                var month = query.Month < 10 ? ("0" + query.Month) : query.Month.ToString();
                d = query.Year + "-" + month;
                expable.And(t => Convert.ToDateTime(t.StartTime).ToString("yyyy-MM") == d);
            }
            expable.AndIF(query.UserID.HasValue, t => t.UserID == query.UserID);
            expable.AndIF(query.AttendanceType.HasValue, t => t.AttendanceType == query.AttendanceType);
            expable.AndIF((query.Status.HasValue && query.Status!=0), t => t.Status == query.Status);
            expable.AndIF(query.CID.HasValue, t => t.CustomerID == query.CID);
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.UserName.Contains(query.Name));
            expable.AndIF(query.structureID > 0, t => t.StructureID == query.structureID);
            var exp = expable.ToExpression();
            var models = await attendanceApply.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPut]
        public async Task<IActionResult> Add([FromBody] Model.Live_AttendanceApply model)
        {
            var user = CurrentUser;
            var now = DateTime.Now;
            var expable = Expressionable.Create<Model.Live_AttendanceApply>();
            //if(model.ApplyType!=(int)AttendanceApplyTypeEnum.提前请假 && model.ApplyType != (int)AttendanceApplyTypeEnum.加班)
            //{
            //    if (await attendanceApply.Exist(t => t.SignDate == model.SignDate && t.Sort == model.Sort && t.UserID == model.UserID))
            //    {
            //        throw new ErrorCodeException(-1, "已申请");
            //    }
            //}
            if(!model.EntryID.HasValue || string.IsNullOrEmpty(model.StartTime) || string.IsNullOrEmpty(model.EndTime))
            {
                throw new ErrorCodeException(-1, "信息不全");
            }
            var start = Convert.ToDateTime(model.StartTime);
            var leaveRecord = await attendanceAskForLeaveRecordService.Get(t => t.EntryID == model.EntryID && t.Year == start.Year);
            if (leaveRecord == null)
            {
                throw new ErrorCodeException(-1, "员工考勤配置未创建");
            }
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            var list = await attendanceStructureService.List(t => t.NavID == model.StructureID, query);
            var pitem = list.Find(t => t.ParentID == 0);
            if (pitem == null) throw new ErrorCodeException(-1, "车间匹配错误");
            Live_V_AttendanceStructure sub = new Live_V_AttendanceStructure();
            sub = pitem;
            while (sub != null)
            {
                sub = list.Find(t => t.ParentID == pitem.ID);
                if (sub != null)
                {
                    pitem = sub;
                }
            }
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.AuditStructure = pitem.ID;
            model.ManageGIDS = pitem.UserIDS;
            model.CreateTime = DateTime.Now;
            model.Status = 1;
            //model.CreateTime = DateTime.Now;
            //try
            //{
            //    if (!string.IsNullOrEmpty(model.StartTime) && model.SignDate.HasValue)
            //    {
            //        var date = model.StartDayType == 2 ? model.SignDate.Value.AddDays(1) : model.SignDate.Value;
            //        model.UpTime = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " " + model.StartTime + ":00.000");
            //    }
            //    if (!string.IsNullOrEmpty(model.EndTime) && model.SignDate.HasValue)
            //    {
            //        var date = model.EndDayType == 2 ? model.SignDate.Value.AddDays(1) : model.SignDate.Value;
            //        model.OffTime = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " " + model.EndTime + ":00.000");
            //    }
            //}
            //catch(Exception e)
            //{
            //    throw new ErrorCodeException(-1, "上下班时间转化失败");
            //}
            if (model.ApplyType == (int)AttendanceApplyTypeEnum.请假 && model.InfoType.HasValue)
            {
                model.InfoTypeName = ((AskForLeaveEnum)model.InfoType).ToString();
            }
            else if (model.ApplyType == (int)AttendanceApplyTypeEnum.加班 && model.InfoType.HasValue)
            {
                model.InfoTypeName = ((OverWorkEnum)model.InfoType).ToString();
            }
            int i = await attendanceApply.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> AddRecord([FromBody] Model.Live_AttendanceApply model)
        {
            var user = CurrentUser;
            var now = DateTime.Now;
            var expable = Expressionable.Create<Model.Live_AttendanceApply>();

            var structure = await attendanceStructureService.GetView(t => t.ID == model.StructureID);
            if (structure == null) throw new ErrorCodeException(-1, "车间配置错误");
            if (structure.IsApplicationLimit)
            {

                var count = await attendanceApply.Count(t => t.UserID == model.UserID && t.CustomerID == t.CustomerID && t.SignDate.HasValue && t.SignDate.Value.ToString("yyyy-MM") == now.ToString("yyyy-MM") && t.ApplyType == (int)AttendanceApplyTypeEnum.补卡);
                if (count >= structure.ApplicationCount)
                {
                    throw new ErrorCodeException(-1, "补卡次数已用完");
                }
            }

            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            var list = await attendanceStructureService.List(t => t.NavID == model.StructureID, query);
            var pitem = list.Find(t => t.ParentID == 0);
            if (pitem == null) throw new ErrorCodeException(-1, "车间匹配错误");
            Live_V_AttendanceStructure sub = new Live_V_AttendanceStructure();
            sub = pitem;
            while (sub != null)
            {
                sub = list.Find(t => t.ParentID == pitem.ID);
                if (sub != null)
                {
                    pitem = sub;
                }
            }
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.AuditStructure = pitem.ID;
            model.ManageGIDS = pitem.UserIDS;
            model.CreateTime = DateTime.Now;
            model.Status = 1;
            int i = await attendanceApply.Add(model);
            return Ok();
        }



        [HttpPut]
        public async Task<IActionResult> AddLeave([FromForm] Model.Live_AttendanceApply model, [FromForm] int type, [FromForm] int id)
        {
            var user = CurrentUser;
            var now = DateTime.Now;
            var expable = Expressionable.Create<Model.Live_AttendanceApply>();
            //if (model.ApplyType != (int)AttendanceApplyTypeEnum.提前请假 && model.ApplyType != (int)AttendanceApplyTypeEnum.加班)
            //{
            //    if (await attendanceApply.Exist(t => t.SignDate == model.SignDate && t.Sort == model.Sort && t.UserID == model.UserID))
            //    {
            //        throw new ErrorCodeException(-1, "已申请");
            //    }
            //}

            if (model.ApplyType == (int)AttendanceApplyTypeEnum.补卡)
            {
                var structure = await attendanceStructureService.GetView(t => t.ID == model.StructureID);
                if (structure == null) throw new ErrorCodeException(-1, "车间配置错误");
                if (structure.IsApplicationLimit)
                {

                    var count = await attendanceApply.Count(t => t.UserID == model.UserID && t.CustomerID == t.CustomerID && Convert.ToDateTime(t.StartTime).ToString("yyyy-MM") == now.ToString("yyyy-MM") && t.ApplyType == model.ApplyType);
                    if (count >= structure.ApplicationCount)
                    {
                        throw new ErrorCodeException(-1, "补卡次数已用完");
                    }
                }
            }

            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            var list = await attendanceStructureService.List(t => t.NavID == model.StructureID, query);
            var pitem = list.Find(t => t.ParentID == 0);
            if (pitem == null) throw new ErrorCodeException(-1, "车间匹配错误");
            Live_V_AttendanceStructure sub = new Live_V_AttendanceStructure();
            sub = pitem;
            while (sub != null)
            {
                sub = list.Find(t => t.ParentID == pitem.ID);
                if (sub != null)
                {
                    pitem = sub;
                }
            }
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.AuditStructure = pitem.ID;
            model.ManageGIDS = pitem.UserIDS;
            model.Status = 1;
            //int i = await attendanceApply.Add(model);
            //if (i > 0)
            //{
            //    var record = await attendanceAskForLeaveRecordService.Get(id);
            //}
            return Ok(new {item = model,id,type});
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await attendanceApply.Get(id);
            return Ok(new { items = model });
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] int id, [FromForm] int status, [FromForm] string content, [FromForm] string from, [FromForm] string message)
        {
            var user = CurrentUser;
            string  msg = "";
            var model = await attendanceApply.GetV(id);
            if (!model.AttendanceID.HasValue)
            {
                throw new ErrorCodeException(-1, "车间未关联考勤配置");
            }
            var details = await attendanceDetailService.List(t=>t.ConfigID==model.AttendanceID,new UserPagedModel { Page=0 });
            if (details.Count==0)
            {
                throw new ErrorCodeException(-1, "未设置考勤打卡时间");
            }
            var entry = await entryService.View(t => t.UserID == model.UserID && t.Status == 2);
            if (entry == null)
            {
                throw new ErrorCodeException(-1, "未入职");
            }
            if(!model.StructureID.HasValue)
            {
                throw new ErrorCodeException(-1, "未分配车间");
            }

            
            Live_AttendanceApplyLog log = new Live_AttendanceApplyLog();
            var objs = new Dictionary<string, object>();
            objs.Add("ID", model.ID);
            var isok = from == "pc" || (from == "wx" && model.StructureID == model.AuditStructure) || status==3;
            if (isok)
            {
                objs.Add("status", status);
                
            }
            else
            {
                
                var structure = await attendanceStructureService.Get(model.AuditStructure.Value);
                if (!structure.ParentID.HasValue)
                {
                    throw new ErrorCodeException(-1, "ParentID没有数据");
                }
                var parent = await attendanceStructureService.Get(structure.ParentID.Value);
                objs.Add("AuditStructure", parent.ID);
                objs.Add("ManageGIDS", parent.UserIDS);
            }
            
            if (!string.IsNullOrEmpty(message))
            {
                objs.Add("message", message);
            }
            if (!string.IsNullOrEmpty(content))
            {
                objs.Add("content", content);
            }
            
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                log.AuditedID = umodel.UserID;
                log.AuditedName = umodel.FullName;

            }
            else if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                log.AuditedID = info.UserID;
                log.AuditedName = info.FullName;
            }
                
            log.UserID = model.UserID;
            log.UserName = model.UserName;
            
            log.From = from??"";
            log.Content = message??"";
            log.ApplyID = model.ID;
            log.CreateTime = DateTime.Now;
            log.Status = status;
            log.CustomerID = model.CustomerID;
            if (isok)
            {
                //if(model.ApplyType==(int)AttendanceApplyTypeEnum.请假 && !model.ApplyDate.HasValue)
                //{
                //    //请假时间区间
                //    var start = model.StartDayType == 1 ? model.SignDate.Value.ToString("yyyy-MM-dd") : model.SignDate.Value.AddDays(1).ToString("yyyy-MM-dd");
                //    var sdate = Convert.ToDateTime(start + " " + model.StartTime + ":00.000");
                //    var end = model.EndDayType == 1 ? model.SignDate.Value.ToString("yyyy-MM-dd") : model.SignDate.Value.AddDays(1).ToString("yyyy-MM-dd");
                //    var edate = Convert.ToDateTime(end + " " + model.EndTime + ":00.000");
                //    //打卡时间区间
                //    var sdetail = model.SignDate.Value.ToString("yyyy-MM-dd");
                //    var edetail = details[details.Count - 1].DayType == 1 ? model.SignDate.Value.ToString("yyyy-MM-dd") : model.SignDate.Value.AddDays(1).ToString("yyyy-MM-dd");
                //    var sdetailDate = Convert.ToDateTime(sdetail + " " + details[0].TimeShow + ":00.000");
                //    var edetailDate = Convert.ToDateTime(edetail + " " + details[details.Count - 1].TimeShow + ":00.000");
                //    if (model.IsDay)
                //    {
                //        List<Live_AttendanceRecord> list = new List<Live_AttendanceRecord>();
                //        Live_AttendanceRecord onrecord = new Live_AttendanceRecord();
                //        onrecord.CompanyID = user.CompanyID;
                //        onrecord.CompanyGID = user.CompanyGID;
                //        onrecord.SignDate = model.SignDate;
                //        onrecord.UserID = model.UserID;
                //        onrecord.UserName = model.UserName;
                //        onrecord.CustomerID = model.CustomerID;
                //        onrecord.AttendanceDate = sdate;
                //        onrecord.Remark = "请假一整天";
                //        onrecord.Status = (int)AttendanceStatusEnum.请假;
                //        onrecord.StatusName = AttendanceStatusEnum.请假.ToString();
                //        if(model.Sort.HasValue)
                //        {
                //            onrecord.Sort = model.Sort;
                //        }
                        
                //        if(model.DefaultTime.HasValue)
                //        {
                //            onrecord.DefaultTime = model.DefaultTime;
                //        }
                //        onrecord.Type = model.Type;
                //        if(onrecord.Type.HasValue)
                //        {
                //            onrecord.TypeName = ((AttendanceTypeEnum)onrecord.Type).ToString();
                //        }
                //        onrecord.AddressName = model.AddressName;
                //        onrecord.DayType = model.DayType;
                //        onrecord.ApplyID = model.ID;
                //        onrecord.StructureID = model.StructureID;
                //        onrecord.MobileDevice = "请假自动生成";
                //        onrecord.TimeShow = model.StartTime;
                //        onrecord.PartnerID = model.PartnerID;
                //        onrecord.PartnerName = model.PartnerName;
                //        list.Add(onrecord);
                //        Live_AttendanceRecord offrecord = new Live_AttendanceRecord();
                //        offrecord.CompanyID = user.CompanyID;
                //        offrecord.CompanyGID = user.CompanyGID;
                //        offrecord.SignDate = model.SignDate;
                //        offrecord.UserID = model.UserID;
                //        offrecord.UserName = model.UserName;
                //        offrecord.CustomerID = model.CustomerID;
                //        offrecord.AttendanceDate = edate;
                //        offrecord.Status = (int)AttendanceStatusEnum.请假;
                //        offrecord.StatusName = AttendanceStatusEnum.请假.ToString();
                //        offrecord.Remark = "请假一整天";
                //        if (model.Sort.HasValue)
                //        {
                //            offrecord.Sort = model.Sort;
                //        }
                //        if (model.DefaultTime.HasValue)
                //        {
                //            offrecord.DefaultTime = model.DefaultTime;
                //        }
                //        offrecord.Type = model.Type;
                //        if (offrecord.Type.HasValue)
                //        {
                //            offrecord.TypeName = ((AttendanceTypeEnum)offrecord.Type).ToString();
                //        }
                //        offrecord.AddressName = model.AddressName;
                //        offrecord.DayType = model.DayType;
                //        offrecord.ApplyID = model.ID;
                //        offrecord.StructureID = model.StructureID;
                //        offrecord.MobileDevice = "请假自动生成";
                //        offrecord.TimeShow = model.EndTime;
                //        offrecord.LeaveHour = model.Number;
                //        offrecord.PartnerID = model.PartnerID;
                //        offrecord.PartnerName = model.PartnerName;
                //        list.Add(offrecord);
                //        await attendanceRecordService.Add(list);
                //    }

                //}
                if (model.ApplyType == (int)AttendanceApplyTypeEnum.加班 && model.RecordID.HasValue)
                {
                    var record = await attendanceRecordService.Get(model.RecordID.Value);
                    if (record != null)
                    {
                        var dic = new Dictionary<string, object>();
                        dic.Add("ID", record.ID);
                        //dic.Add("WorkHour", model.Number.HasValue? model.Number.Value.ToString() : 0);
                        dic.Add("Remark", model.Message ?? "");
                        dic.Add("ApplyID", model.ID);
                        await attendanceRecordService.DictionarySet(dic);
                    }
                    //var start = model.StartDayType == 1 ? model.SignDate.Value.ToString("yyyy-MM-dd") : model.SignDate.Value.AddDays(1).ToString("yyyy-MM-dd");
                    //var sdate = Convert.ToDateTime(start + " " + model.StartTime + ":00.000");
                    //var end = model.EndDayType == 1 ? model.SignDate.Value.ToString("yyyy-MM-dd") : model.SignDate.Value.AddDays(1).ToString("yyyy-MM-dd");
                    //var edate = Convert.ToDateTime(end + " " + model.EndTime + ":00.000");
                    //List<Live_AttendanceRecord> list = new List<Live_AttendanceRecord>();
                    //Live_AttendanceRecord onrecord = new Live_AttendanceRecord();
                    //onrecord.CompanyID = user.CompanyID;
                    //onrecord.CompanyGID = user.CompanyGID;
                    //onrecord.SignDate = model.SignDate;
                    //onrecord.UserID = model.UserID;
                    //onrecord.UserName = model.UserName;
                    //onrecord.CustomerID = model.CustomerID;
                    //onrecord.AttendanceDate = sdate;
                    //onrecord.Remark = "加班申请";
                    //onrecord.Status = (int)AttendanceStatusEnum.正常;
                    //onrecord.StatusName = AttendanceStatusEnum.正常.ToString();
                    //onrecord.Sort = model.Sort;
                    //if (model.DefaultTime.HasValue)
                    //{
                    //    onrecord.DefaultTime = model.DefaultTime;
                    //}
                    //onrecord.Type = model.Type;
                    //if (onrecord.Type.HasValue)
                    //{
                    //    onrecord.TypeName = ((AttendanceTypeEnum)onrecord.Type).ToString();
                    //}
                    //onrecord.AddressName = model.AddressName;
                    //onrecord.DayType = model.DayType;
                    //onrecord.ApplyID = model.ID;
                    //onrecord.StructureID = model.StructureID;
                    //onrecord.MobileDevice = "加班申请自动生成";
                    //onrecord.PartnerID = model.PartnerID;
                    //onrecord.PartnerName = model.PartnerName;
                    //list.Add(onrecord);
                    //Live_AttendanceRecord offrecord = new Live_AttendanceRecord();
                    //offrecord.CompanyID = user.CompanyID;
                    //offrecord.CompanyGID = user.CompanyGID;
                    //offrecord.SignDate = model.SignDate;
                    //offrecord.UserID = model.UserID;
                    //offrecord.UserName = model.UserName;
                    //offrecord.CustomerID = model.CustomerID;
                    //offrecord.AttendanceDate = edate;
                    //offrecord.Status = (int)AttendanceStatusEnum.正常;
                    //offrecord.StatusName = AttendanceStatusEnum.正常.ToString();
                    //offrecord.Remark = "加班申请";
                    //offrecord.Sort = model.Sort;
                    //if (model.DefaultTime.HasValue)
                    //{
                    //    offrecord.DefaultTime = model.DefaultTime;
                    //}
                    //offrecord.Type = model.Type;
                    //if (offrecord.Type.HasValue)
                    //{
                    //    offrecord.TypeName = ((AttendanceTypeEnum)offrecord.Type).ToString();
                    //}
                    //offrecord.AddressName = model.AddressName;
                    //offrecord.DayType = model.DayType;
                    //offrecord.ApplyID = model.ID;
                    //offrecord.StructureID = model.StructureID;
                    //offrecord.MobileDevice = "加班申请自动生成";
                    //offrecord.LeaveHour = model.Number;
                    //offrecord.PartnerID = model.PartnerID;
                    //offrecord.PartnerName = model.PartnerName;
                    //list.Add(offrecord);
                    //await attendanceRecordService.Add(list);
                }
                else if (model.ApplyType == (int)AttendanceApplyTypeEnum.请假)
                {
                    //var record = await attendanceRecordService.Get(model.RecordID.Value);
                    //if (record != null)
                    //{
                    //    var dic = new Dictionary<string, object>();
                    //    dic.Add("ID", record.ID);
                    //    dic.Add("status", (int)AttendanceStatusEnum.请假);
                    //    dic.Add("LeaveHour", model.Number.ToString());
                    //    dic.Add("Remark",model.Message??"");
                    //    dic.Add("StatusName", AttendanceStatusEnum.请假.ToString());
                    //    dic.Add("ApplyID", model.ID);
                    //    await attendanceRecordService.DictionarySet(dic);
                    //}
                    //在请假范围内的考勤记录(迟到和早退),标记未请假
                    if(model.EntryID.HasValue)
                    {
                        var recordList = await attendanceRecordService.List(t => t.EntryID == model.EntryID && t.AttendanceDate >= Convert.ToDateTime(model.StartTime) && t.AttendanceDate < Convert.ToDateTime(model.EndTime) && (t.Status == (int)AttendanceStatusEnum.迟到 || t.Status == (int)AttendanceStatusEnum.早退 || t.Status == (int)AttendanceStatusEnum.外勤早退 || t.Status == (int)AttendanceStatusEnum.外勤迟到), new UserPagedModel() { Page = 0 });
                        if (recordList.Count > 0)
                        {
                            foreach(var x in recordList)
                            {
                                x.Status = (int)AttendanceStatusEnum.请假;
                                x.StatusName = AttendanceStatusEnum.请假.ToString();
                            }
                            await attendanceRecordService.Update(recordList);
                        }
                    }
                    


                    if (model.EntryID.HasValue && !string.IsNullOrEmpty(model.StartTime))
                    {
                        var start = Convert.ToDateTime(model.EndTime);
                        var leaveRecord = await attendanceAskForLeaveRecordService.Get(t => t.EntryID == model.EntryID && t.Year == start.Year);
                        if (leaveRecord != null)
                        {
                            switch (model.InfoType)
                            {
                                case (int)AskForLeaveEnum.调休假:
                                    leaveRecord.CompensatoryLeaveUsed += (int)(model.Number?? 0);
                                    break;
                                case (int)AskForLeaveEnum.年休假:
                                    leaveRecord.YearLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.工伤假:
                                    leaveRecord.InjuryLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.婚假:
                                    leaveRecord.MarryLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.丧假:
                                    leaveRecord.BereavementLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.产检假:
                                    leaveRecord.PrenatalCheckLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.产假:
                                    leaveRecord.MaternityLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.陪产假:
                                    leaveRecord.PrenatalLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.流产假:
                                    leaveRecord.MiscarriageLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.哺乳假:
                                    leaveRecord.LactationLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.陪护假:
                                    leaveRecord.AccompanyLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.事假:
                                    leaveRecord.PersonalLeaveUsed += (int)(model.Number ?? 0);
                                    break;
                                case (int)AskForLeaveEnum.病假:
                                    leaveRecord.SickLeaveUsed += (int)(model.Number ?? 0);
                                    break;

                            }
                            await attendanceAskForLeaveRecordService.Update(leaveRecord);
                        }
                    }
                    
                }
                else if (model.ApplyType == (int)AttendanceApplyTypeEnum.补卡)
                {
                    Live_AttendanceRecord record = new Live_AttendanceRecord();
                    record.CompanyID = user.CompanyID;
                    record.CompanyGID = user.CompanyGID;
                    record.SignDate = model.SignDate;
                    record.UserID = model.UserID;
                    record.UserName = model.UserName;
                    record.CustomerID = model.CustomerID;
                    record.AttendanceDate = model.ApplyDate;
                    record.Status = (int)AttendanceStatusEnum.补卡;
                    record.StatusName = AttendanceStatusEnum.补卡.ToString();
                    record.Sort = model.Sort;
                    record.DefaultTime = model.DefaultTime;
                    record.Type = model.Type;
                    if (record.Type.HasValue)
                    {
                        record.TypeName = ((AttendanceTypeEnum)record.Type).ToString();
                    }
                    record.AddressName = model.AddressName;
                    record.DayType = model.DayType;
                    record.ApplyID = model.ID;
                    record.StructureID = model.StructureID;
                    if (record.AttendanceDate.HasValue)
                    {
                        record.TimeShow = record.AttendanceDate.Value.ToString("HH:mm");
                    }
                    if (model.AttendanceType == 2 && model.Type == (int)AttendanceTypeEnum.上班)
                    {
                        Model.Live_PrepaidApplyRecord m = new Live_PrepaidApplyRecord();
                        int sort = model.Sort.Value + 1;
                        var detail = await attendanceRecordService.Get(t => t.SignDate == model.SignDate && t.Sort == sort && t.UserID == model.UserID);
                        if (detail != null)
                        {
                            DateTime big = record.AttendanceDate.Value > record.DefaultTime.Value ? record.AttendanceDate.Value : record.DefaultTime.Value;
                            TimeSpan timeSpan = detail.AttendanceDate.Value - big;
                            int hours = timeSpan.Hours;
                            int minutes = timeSpan.Minutes;
                            decimal mi = minutes > 30 ? 0.5M : 0M;
                            if (model.TimeRule == (int)AttendanceTimeRuleEnum.一小时)
                            {
                                detail.WorkHour = hours;
                            }
                            else if (model.TimeRule == (int)AttendanceTimeRuleEnum.半小时)
                            {
                                detail.WorkHour = hours + mi;
                            }

                            detail.WorkHour = hours + mi;

                            #region 补上班卡预发申请
                            decimal workhour = 0M; //工时必须大于早餐扣时加晚餐扣时
                            if (detail.WorkHour.HasValue)
                            {
                                workhour = detail.WorkHour.Value;
                                if (model.LunchHours.HasValue && !string.IsNullOrEmpty(model.LunchTime))
                                {

                                    string lunchstr = detail.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.LunchTime + ":00.000";
                                    DateTime lunchTime = Convert.ToDateTime(lunchstr);
                                    m.LunchHours = 0;
                                    if (detail.AttendanceDate.Value > lunchTime)
                                    {
                                        workhour = workhour - model.LunchHours.Value;
                                        m.LunchHours = model.LunchHours.Value;
                                    }

                                }
                                if (model.DinnerHours.HasValue && !string.IsNullOrEmpty(model.DinnerTime))
                                {

                                    string dinnerstr = detail.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.DinnerTime + ":00.000";
                                    DateTime dinnerTime = Convert.ToDateTime(dinnerstr);
                                    m.DinnerHours = 0;
                                    if (detail.AttendanceDate.Value > dinnerTime)
                                    {
                                        workhour = workhour - model.DinnerHours.Value;
                                        m.DinnerHours = model.DinnerHours.Value;
                                    }

                                }
                                workhour = workhour < 0 ? 0 : workhour;
                            }
                            if (workhour > 0)//配置类型为日结工,下班卡
                            {
                                m.UserEntryID = entry.ID;
                                m.CompanyID = entry.CompanyID;
                                m.CompanyGID = entry.CompanyGID;
                                m.ApplyType = PrepaidApplyEnumType.日结工资申请;
                                m.ShiftStartTime = record.AttendanceDate;
                                m.ShiftEndTime = detail.AttendanceDate;
                                m.ShiftHours = detail.WorkHour;
                                m.ShiftMoney = model.ShiftMoney;
                                m.DayWorkType = model.DayWorkType;
                                m.StructureID = model.StructureID;
                                m.MoneyHours = workhour;
                                m.ApplyAmount = workhour * (model.ShiftMoney ?? 0M) + (model.NightShiftSubsidy ?? 0M);
                                m.LunchHours = model.LunchHours;
                                m.DinnerHours = model.DinnerHours;
                                m.NightShiftSubsidy = model.NightShiftSubsidy;
                                var modeljson = JsonConvert.SerializeObject(m);
                                var obj = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/PrepaidApplyRecord/Apply", modeljson);
                                msg = obj.Message;
                            }
                            #endregion
                            if (msg != "Success")
                            {
                                throw new ErrorCodeException(-1, msg);
                            }
                            var dic = new Dictionary<string, object>();
                            dic.Add("ID", detail.ID);
                            dic.Add("WorkHour", detail.WorkHour);
                            await attendanceRecordService.DictionarySet(dic);
                            await attendanceRecordService.Add(record);
                        }

                    }
                    else if (model.AttendanceType == 2 && model.Type == (int)AttendanceTypeEnum.下班)
                    {
                        Model.Live_PrepaidApplyRecord m = new Live_PrepaidApplyRecord();
                        int sort = model.Sort.Value - 1;
                        var detail = await attendanceRecordService.Get(t => t.SignDate == model.SignDate && t.Sort == sort && t.UserID == model.UserID);
                        if (detail != null)
                        {
                            DateTime big = detail.AttendanceDate.Value > detail.DefaultTime.Value ? detail.AttendanceDate.Value : detail.DefaultTime.Value;
                            TimeSpan timeSpan = record.AttendanceDate.Value - big;
                            int hours = timeSpan.Hours;
                            int minutes = timeSpan.Minutes;
                            decimal mi = minutes > 30 ? 0.5M : 0M;
                            if (model.TimeRule == (int)AttendanceTimeRuleEnum.一小时)
                            {
                                record.WorkHour = hours;
                            }
                            else if (model.TimeRule == (int)AttendanceTimeRuleEnum.半小时)
                            {
                                record.WorkHour = hours + mi;
                            }
                            #region 补下班卡预发申请
                            decimal workhour = 0M; //工时必须大于早餐扣时加晚餐扣时
                            if (record.WorkHour.HasValue)
                            {
                                workhour = record.WorkHour.Value;
                                if (model.LunchHours.HasValue && !string.IsNullOrEmpty(model.LunchTime))
                                {
                                    string lunchstr = record.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.LunchTime + ":00.000";
                                    DateTime lunchTime = Convert.ToDateTime(lunchstr);
                                    m.LunchHours = 0;
                                    if (record.AttendanceDate.Value > lunchTime)
                                    {
                                        workhour = workhour - model.LunchHours.Value;
                                        m.LunchHours = model.LunchHours.Value;
                                    }

                                }
                                if (model.DinnerHours.HasValue && !string.IsNullOrEmpty(model.DinnerTime))
                                {
                                    string dinnerstr = record.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.DinnerTime + ":00.000";
                                    DateTime dinnerTime = Convert.ToDateTime(dinnerstr);
                                    m.DinnerHours = 0;
                                    if (record.AttendanceDate.Value > dinnerTime)
                                    {
                                        workhour = workhour - model.DinnerHours.Value;
                                        m.DinnerHours = model.DinnerHours.Value;
                                    }

                                }
                                workhour = workhour < 0 ? 0 : workhour;
                            }
                            if (workhour > 0)//配置类型为日结工,下班卡
                            {

                                m.UserEntryID = entry.ID;
                                m.CompanyID = entry.CompanyID;
                                m.CompanyGID = entry.CompanyGID;
                                m.ApplyType = PrepaidApplyEnumType.日结工资申请;
                                m.ShiftStartTime = detail.AttendanceDate;
                                m.ShiftEndTime = record.AttendanceDate;
                                m.ShiftHours = record.WorkHour;
                                m.ShiftMoney = model.ShiftMoney;
                                m.DayWorkType = model.DayWorkType;
                                m.MoneyHours = workhour;
                                m.StructureID = model.StructureID;
                                m.ApplyAmount = workhour * (model.ShiftMoney ?? 0M) + (model.NightShiftSubsidy ?? 0M);
                                m.LunchHours = model.LunchHours;
                                m.StructureID = model.StructureID;
                                m.DinnerHours = model.DinnerHours;
                                m.NightShiftSubsidy = model.NightShiftSubsidy;
                                var modeljson = JsonConvert.SerializeObject(m);
                                var obj = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/PrepaidApplyRecord/Apply", modeljson);
                                msg = obj.Message;
                            }
                            #endregion
                            if (msg != "Success")
                            {
                                throw new ErrorCodeException(-1, msg);
                            }
                            await attendanceRecordService.Add(record);
                        }


                    }
                    else
                    {

                        record.LeaveHour = model.Number;
                        await attendanceRecordService.Add(record);
                    }

                }

            }
            
            
            await attendanceApply.DictionarySet(objs);

            await attendanceApplyLogService.Add(log);


            return Ok(new { items = model, msg });
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await attendanceApply.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await attendanceApply.Delete(id);
            return Ok();
        }
    }
}
