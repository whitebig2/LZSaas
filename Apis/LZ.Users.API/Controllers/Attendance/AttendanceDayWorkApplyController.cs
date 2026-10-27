using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Enums.Attendance;
using LZ.Model.Enums.DayWork;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    public class AttendanceDayWorkApplyController(IAttendanceDayWorkApplyService attendanceDayWorkApplyService, IAuthService authService, IEntryService entryService,IHttpService httpService, IAttendanceApplyLogService attendanceApplyLogService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_AttendanceDayWorkApply>();
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            //{
            //    expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            //}
            //else
            //{
            //    expable.And(it => it.CompanyID == cid);
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
            expable.AndIF(query.Status.HasValue, t => t.Status == query.Status);
            expable.AndIF(query.UserID.HasValue, t => t.UserID == query.UserID);
            expable.AndIF(query.CID.HasValue, t => t.CustomerID == query.CID);
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.UserName.Contains(query.Name));
            expable.AndIF(query.structureID > 0, t => t.StructureID == query.structureID);
            var exp = expable.ToExpression();
            var models = await attendanceDayWorkApplyService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPut]
        public async Task<int> Add([FromBody] Model.Live_AttendanceDayWorkApply model)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_AttendanceApply>();
            model.Status = (int)AttendanceDayWorkApplyStatusEnum.申请中;
            model.Money = decimal.Round((model.Price ?? 0M) * (model.WorkHour ?? 0M), 2);
            model.Title = "补卡申请";
            model.CreateTime = DateTime.Now;
            var entryContractRecord = await httpService.GetAsync<Model.Live_V_DayWorkUserContractRecord>(ServiceNames.CustomerService, "/api/DayWorkUserContractRecord/GetView?entry=" + model.EntryID + "&status=" + (int)DayWorkEntryContractTypeEnum.已盖章);
            if (entryContractRecord == null)
            {
                throw new ErrorCodeException(-1, "未查询到入职编号");
            }
            model.EntryNo = entryContractRecord.EntryNo;
            model.JobNo = entryContractRecord.JobNo;
            int i = await attendanceDayWorkApplyService.Add(model);
            return i;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_AttendanceDayWorkApply model)
        {
            await attendanceDayWorkApplyService.Update(model);
            return Ok();
        }
       [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await attendanceDayWorkApplyService.Get(id);
            return Ok(new { items = model });
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] int id, [FromForm] int status, [FromForm] string message)
        {
            var user = CurrentUser;
            string msg = "";
            var model = await attendanceDayWorkApplyService.Get(id);
            var entry = await entryService.View(t => t.UserID == model.UserID && t.Status == 2);
            if (entry == null)
            {
                throw new ErrorCodeException(-1, "未入职");
            }

            Live_AttendanceApplyLog log = new Live_AttendanceApplyLog();
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();

            log.UserID = model.UserID;
            log.UserName = model.UserName;
            log.AuditedID = info.UserID;
            log.AuditedName = info.FullName;
            log.From = "pc";
            log.Content = model.Status == 2 ? "日结工补卡申请成功": "日结工补卡申请失败";
            log.ApplyID = model.ID;
            log.CreateTime = DateTime.Now;
            log.Status = status;
            log.CustomerID = model.CustomerID;
            if (status==(int)AttendanceDayWorkApplyStatusEnum.已通过)
            {
                Model.Live_PrepaidApplyRecord m = new Live_PrepaidApplyRecord();
                m.UserEntryID = entry.ID;
                m.CompanyID = entry.CompanyID;
                m.CompanyGID = entry.CompanyGID;
                m.ApplyType = PrepaidApplyEnumType.日结工资申请;
                m.ShiftStartTime = Convert.ToDateTime(model.SignTime.Value.ToString("yyyy-MM-dd") + " "+model.StartTime+ ":00.000");
                m.ShiftEndTime = Convert.ToDateTime(model.SignTime.Value.ToString("yyyy-MM-dd") + " "  + model.EndTime + ":00.000");
                m.ShiftHours = model.WorkHour;
                m.ShiftMoney = model.Price;
                m.DayWorkType = model.DayWorkType;
                m.MoneyHours = model.WorkHour;
                m.StructureID = model.StructureID;
                m.ApplyAmount = model.Money?? 0M + (model.Subsidy ?? 0M);
                m.LunchHours = 0M;
                m.StructureID = model.StructureID;
                m.DinnerHours = 0M;
                m.NightShiftSubsidy = model.Subsidy;
                m.JobNo = model.JobNo;
                m.EntryNo = model.EntryNo;
                var modeljson = JsonConvert.SerializeObject(m);
                var obj = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/PrepaidApplyRecord/Apply", modeljson);
                msg = obj.Message;




                #region 注释代码
                //if (model.RecordID.HasValue && model.ApplyType == (int)AttendanceApplyTypeEnum.请假)
                //{
                //    var record = await attendanceRecordService.Get(model.RecordID.Value);
                //    if (record != null)
                //    {
                //        var dic = new Dictionary<string, object>();
                //        dic.Add("ID", record.ID);
                //        dic.Add("status", (int)AttendanceStatusEnum.请假);
                //        dic.Add("LeaveHour", model.Number.ToString());
                //        dic.Add("StatusName", AttendanceStatusEnum.请假.ToString());
                //        dic.Add("ApplyID", model.ID);
                //        await attendanceRecordService.DictionarySet(dic);
                //    }
                //}
                //else if (model.ApplyType == (int)AttendanceApplyTypeEnum.补卡)
                //{
                //    Live_AttendanceRecord record = new Live_AttendanceRecord();
                //    record.CompanyID = user.CompanyID;
                //    record.CompanyGID = user.CompanyGID;
                //    record.SignDate = model.SignDate;
                //    record.UserID = model.UserID;
                //    record.UserName = model.UserName;
                //    record.CustomerID = model.CustomerID;
                //    record.AttendanceDate = model.ApplyDate;
                //    record.Status = (int)AttendanceStatusEnum.缺卡;
                //    record.StatusName = AttendanceStatusEnum.缺卡.ToString();
                //    record.Sort = model.Sort;
                //    record.DefaultTime = model.DefaultTime;
                //    record.Type = model.Type;
                //    record.TypeName = ((AttendanceTypeEnum)record.Type).ToString();
                //    record.AddressName = model.AddressName;
                //    record.DayType = model.DayType;
                //    record.ApplyID = model.ID;
                //    record.StructureID = model.StructureID;
                //    if (record.AttendanceDate.HasValue)
                //    {
                //        record.TimeShow = record.AttendanceDate.Value.ToString("HH:mm");
                //    }
                //    if (model.AttendanceType == 2 && model.Type == (int)AttendanceTypeEnum.上班)
                //    {
                //        Model.Live_PrepaidApplyRecord m = new Live_PrepaidApplyRecord();
                //        int sort = model.Sort.Value + 1;
                //        var detail = await attendanceRecordService.Get(t => t.SignDate == model.SignDate && t.Sort == sort && t.UserID == model.UserID);
                //        if (detail != null)
                //        {
                //            DateTime big = record.AttendanceDate.Value > record.DefaultTime.Value ? record.AttendanceDate.Value : record.DefaultTime.Value;
                //            TimeSpan timeSpan = detail.AttendanceDate.Value - big;
                //            int hours = timeSpan.Hours;
                //            int minutes = timeSpan.Minutes;
                //            decimal mi = minutes > 30 ? 0.5M : 0M;
                //            if (model.TimeRule == (int)AttendanceTimeRuleEnum.一小时)
                //            {
                //                detail.WorkHour = hours;
                //            }
                //            else if (model.TimeRule == (int)AttendanceTimeRuleEnum.半小时)
                //            {
                //                detail.WorkHour = hours + mi;
                //            }

                //            detail.WorkHour = hours + mi;

                //            #region 补上班卡预发申请
                //            decimal workhour = 0M; //工时必须大于早餐扣时加晚餐扣时
                //            if (detail.WorkHour.HasValue)
                //            {
                //                workhour = detail.WorkHour.Value;
                //                if (model.LunchHours.HasValue && !string.IsNullOrEmpty(model.LunchTime))
                //                {

                //                    string lunchstr = detail.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.LunchTime + ":00.000";
                //                    DateTime lunchTime = Convert.ToDateTime(lunchstr);
                //                    m.LunchHours = 0;
                //                    if (detail.AttendanceDate.Value > lunchTime)
                //                    {
                //                        workhour = workhour - model.LunchHours.Value;
                //                        m.LunchHours = model.LunchHours.Value;
                //                    }

                //                }
                //                if (model.DinnerHours.HasValue && !string.IsNullOrEmpty(model.DinnerTime))
                //                {

                //                    string dinnerstr = detail.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.DinnerTime + ":00.000";
                //                    DateTime dinnerTime = Convert.ToDateTime(dinnerstr);
                //                    m.DinnerHours = 0;
                //                    if (detail.AttendanceDate.Value > dinnerTime)
                //                    {
                //                        workhour = workhour - model.DinnerHours.Value;
                //                        m.DinnerHours = model.DinnerHours.Value;
                //                    }

                //                }
                //                workhour = workhour < 0 ? 0 : workhour;
                //            }
                //            if (workhour > 0)//配置类型为日结工,下班卡
                //            {
                //                m.UserEntryID = entry.ID;
                //                m.CompanyID = entry.CompanyID;
                //                m.CompanyGID = entry.CompanyGID;
                //                m.ApplyType = PrepaidApplyEnumType.日结工资申请;
                //                m.ShiftStartTime = record.AttendanceDate;
                //                m.ShiftEndTime = detail.AttendanceDate;
                //                m.ShiftHours = detail.WorkHour;
                //                m.ShiftMoney = model.ShiftMoney;
                //                m.DayWorkType = model.DayWorkType;
                //                m.StructureID = model.StructureID;
                //                m.MoneyHours = workhour;
                //                m.ApplyAmount = workhour * (model.ShiftMoney ?? 0M) + (model.NightShiftSubsidy ?? 0M);
                //                m.LunchHours = model.LunchHours;
                //                m.DinnerHours = model.DinnerHours;
                //                m.NightShiftSubsidy = model.NightShiftSubsidy;
                //                var modeljson = JsonConvert.SerializeObject(m);
                //                var obj = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/PrepaidApplyRecord/Apply", modeljson);
                //                msg = obj.Message;
                //            }
                //            #endregion
                //            if (msg != "Success")
                //            {
                //                throw new ErrorCodeException(-1, msg);
                //            }
                //            var dic = new Dictionary<string, object>();
                //            dic.Add("ID", detail.ID);
                //            dic.Add("WorkHour", detail.WorkHour);
                //            await attendanceRecordService.DictionarySet(dic);
                //            await attendanceRecordService.Add(record);
                //        }

                //    }
                //    else if (model.AttendanceType == 2 && model.Type == (int)AttendanceTypeEnum.下班)
                //    {
                //        Model.Live_PrepaidApplyRecord m = new Live_PrepaidApplyRecord();
                //        int sort = model.Sort.Value - 1;
                //        var detail = await attendanceRecordService.Get(t => t.SignDate == model.SignDate && t.Sort == sort && t.UserID == model.UserID);
                //        if (detail != null)
                //        {
                //            DateTime big = detail.AttendanceDate.Value > detail.DefaultTime.Value ? detail.AttendanceDate.Value : detail.DefaultTime.Value;
                //            TimeSpan timeSpan = record.AttendanceDate.Value - big;
                //            int hours = timeSpan.Hours;
                //            int minutes = timeSpan.Minutes;
                //            decimal mi = minutes > 30 ? 0.5M : 0M;
                //            if (model.TimeRule == (int)AttendanceTimeRuleEnum.一小时)
                //            {
                //                record.WorkHour = hours;
                //            }
                //            else if (model.TimeRule == (int)AttendanceTimeRuleEnum.半小时)
                //            {
                //                record.WorkHour = hours + mi;
                //            }
                //            #region 补下班卡预发申请
                //            decimal workhour = 0M; //工时必须大于早餐扣时加晚餐扣时
                //            if (record.WorkHour.HasValue)
                //            {
                //                workhour = record.WorkHour.Value;
                //                if (model.LunchHours.HasValue && !string.IsNullOrEmpty(model.LunchTime))
                //                {
                //                    string lunchstr = record.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.LunchTime + ":00.000";
                //                    DateTime lunchTime = Convert.ToDateTime(lunchstr);
                //                    m.LunchHours = 0;
                //                    if (record.AttendanceDate.Value > lunchTime)
                //                    {
                //                        workhour = workhour - model.LunchHours.Value;
                //                        m.LunchHours = model.LunchHours.Value;
                //                    }

                //                }
                //                if (model.DinnerHours.HasValue && !string.IsNullOrEmpty(model.DinnerTime))
                //                {
                //                    string dinnerstr = record.SignDate.Value.ToString("yyyy-MM-dd") + " " + model.DinnerTime + ":00.000";
                //                    DateTime dinnerTime = Convert.ToDateTime(dinnerstr);
                //                    m.DinnerHours = 0;
                //                    if (record.AttendanceDate.Value > dinnerTime)
                //                    {
                //                        workhour = workhour - model.DinnerHours.Value;
                //                        m.DinnerHours = model.DinnerHours.Value;
                //                    }

                //                }
                //                workhour = workhour < 0 ? 0 : workhour;
                //            }
                //            if (workhour > 0)//配置类型为日结工,下班卡
                //            {

                //                m.UserEntryID = entry.ID;
                //                m.CompanyID = entry.CompanyID;
                //                m.CompanyGID = entry.CompanyGID;
                //                m.ApplyType = PrepaidApplyEnumType.日结工资申请;
                //                m.ShiftStartTime = detail.AttendanceDate;
                //                m.ShiftEndTime = record.AttendanceDate;
                //                m.ShiftHours = record.WorkHour;
                //                m.ShiftMoney = model.ShiftMoney;
                //                m.DayWorkType = model.DayWorkType;
                //                m.MoneyHours = workhour;
                //                m.StructureID = model.StructureID;
                //                m.ApplyAmount = workhour * (model.ShiftMoney ?? 0M) + (model.NightShiftSubsidy ?? 0M);
                //                m.LunchHours = model.LunchHours;
                //                m.StructureID = model.StructureID;
                //                m.DinnerHours = model.DinnerHours;
                //                m.NightShiftSubsidy = model.NightShiftSubsidy;
                //                var modeljson = JsonConvert.SerializeObject(m);
                //                var obj = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/PrepaidApplyRecord/Apply", modeljson);
                //                msg = obj.Message;
                //            }
                //            #endregion
                //            if (msg != "Success")
                //            {
                //                throw new ErrorCodeException(-1, msg);
                //            }
                //            await attendanceRecordService.Add(record);
                //        }


                //    }
                //    else
                //    {
                //        record.LeaveHour = model.Number;
                //        await attendanceRecordService.Add(record);
                //    }

                //}
                #endregion

            }



            await attendanceDayWorkApplyService.UpdateStatus(id,status,message);

            await attendanceApplyLogService.Add(log);


            return Ok(new { items = model, msg });
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await attendanceApplyLogService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await attendanceApplyLogService.Delete(id);
            return Ok();
        } 
    }
}
