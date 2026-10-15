using LZ.Core.Attributes;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.Model;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Model.Enums.Attendance;
using static LZ.Model.Model.AttendanceModel;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.Model.Enums;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Core;
using LZ.Model.Enums.DayWork;
using NetTaste;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    public class AttendanceController(IEntryService entryService, IAttendanceStructureService attendanceStructureService, IAttendanceCalendarService attendanceCalendarService, IAttendanceDetailService attendanceDetailService, IAttendanceRecordService attendanceRecordService, IAttendanceConfigService attendanceConfigService, IAttendanceApplyService attendanceApplyService, IHttpService httpService,IAttendanceService attendanceService,IUserService userService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetModel(int id)
        {
            var user = CurrentUser;
            var date = DateTime.Now;
            DateTime time = DateTime.Today;
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            List<Live_AttendanceDetail> details = new List<Live_AttendanceDetail>();
            
            var entry = await entryService.View(t => t.UserID == id && t.Status == 2 && t.CompanyID==user.CompanyID);
            if (entry == null)
            {
                throw new ErrorCodeException(-1, "未入职_"+id);
            }
            AttendanceModel model = new AttendanceModel();
            model.entry = entry;
            model.UserID = id;



            if (entry.IsDayWork)
            {
                model.isWork = true;
                if (!entry.JobDayWorkID.HasValue)
                {
                    throw new ErrorCodeException(-1, "未关联灵活用工岗位");
                }
                List<int> arr = new List<int>();
                if(entry.AttendancePlan.HasValue)
                {
                    arr.Add(entry.AttendancePlan.Value);
                }
                if (entry.NightAttendancePlan.HasValue)
                {
                    arr.Add(entry.NightAttendancePlan.Value);
                }
                if(arr.Count==0)
                {
                    throw new ErrorCodeException(-1, "灵活用工岗位未配置白班或者夜班");
                }
                var structures = await attendanceStructureService.List(t => arr.Contains(t.ID), query);
                if(structures.Count==0)
                {
                    throw new ErrorCodeException(-1, "白班和夜班考勤配置错误");
                }
                var day = structures.Where(t => t.ID == entry.AttendancePlan).FirstOrDefault();
                var night = structures.Where(t => t.ID == entry.NightAttendancePlan).FirstOrDefault();
                if (day == null)
                {
                    //只有夜班
                    model.StructureID = night.ID;
                    model.structure = await attendanceStructureService.Get(model.StructureID);
                    model.details = await attendanceDetailService.List(t => t.ConfigID == night.AttendanceID.Value, query);
                    model.records = await attendanceRecordService.List(t => t.StructureID == entry.NightAttendancePlan && t.UserID == id && t.SignDate == time, query);
                    #region 判断是今日考勤还是昨天考勤
                    var first = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " " + model.details[0].TimeShow + ":00.000");//第一次上班卡时间
                    var firstTime = first.AddHours(-3);
                    if (date < firstTime)
                    {
                        date = date.AddDays(-1);//如果时间早于今天初始考勤时间的前三小时之前，判断是昨天的考勤
                        time = time.AddDays(-1);//设置为当前考勤时间的初始时间点
                    }
                    #endregion
                }
                else if (night==null)
                {
                    //只有白班
                    model.StructureID = day.ID;
                    model.structure = await attendanceStructureService.Get(model.StructureID);
                    model.details = await attendanceDetailService.List(t => t.ConfigID == day.AttendanceID.Value, query);
                    model.records = await attendanceRecordService.List(t => t.StructureID == entry.AttendancePlan && t.UserID == id && t.SignDate == time, query);
                    #region 判断是今日考勤还是昨天考勤
                    var first = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " " + model.details[0].TimeShow + ":00.000");//第一次上班卡时间
                    var firstTime = first.AddHours(-3);
                    if (date < firstTime)
                    {
                        date = date.AddDays(-1);//如果时间早于今天初始考勤时间的前三小时之前，判断是昨天的考勤
                        time = time.AddDays(-1);//设置为当前考勤时间的初始时间点
                    }
                    #endregion
                }
                else
                {

                    var dayWorkDetails = await attendanceDetailService.List(t => t.ConfigID == day.AttendanceID.Value || t.ConfigID == night.AttendanceID.Value, query);
                    var dayDetail_day_on = dayWorkDetails.Where(t => t.ConfigID == day.AttendanceID && t.Type == (int)AttendanceTypeEnum.上班).FirstOrDefault();
                    var dayDetail_night_on = dayWorkDetails.Where(t => t.ConfigID == night.AttendanceID && t.Type == (int)AttendanceTypeEnum.上班).FirstOrDefault();
                    #region 判断是今日考勤还是昨天考勤
                    var first = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " " + dayDetail_day_on.TimeShow + ":00.000");//第一次上班卡时间
                    var firstTime = first.AddHours(-3);
                    if (date < firstTime)
                    {
                        date = date.AddDays(-1);//如果时间早于今天初始考勤时间的前三小时之前，判断是昨天的考勤
                        time = time.AddDays(-1);//设置为当前考勤时间的初始时间点
                    }
                    #endregion

                    //日结工在未超过白班下班卡前1小时（1小时暂定）的时候，可以打白班卡，如果超过了白班下班时间，未打卡状态下可以打夜班卡
                    //20点的夜班，如果当前时间没有超过19点，算白班；如果已超过19点，已打卡了算白班，没打卡算夜班。不管白班几点下班

                    #region 判断是白班还是夜班的逻辑
                    var offDate = dayDetail_night_on.DayType == 1 ? date : date.AddDays(1);//判断下班卡是当日还是次日
                    var baiTime = Convert.ToDateTime(offDate.ToString("yyyy-MM-dd") + " " + dayDetail_night_on.TimeShow + ":00.000");
                    baiTime = baiTime.AddHours(-1);
                    if (date <= baiTime)
                    {
                        //当前时间未超过夜班上班（前1小时），算作白班
                        model.StructureID = day.ID;
                        model.structure = await attendanceStructureService.Get(model.StructureID);
                        model.details = dayWorkDetails.Where(t => t.ConfigID == day.AttendanceID).ToList();
                        model.records = await attendanceRecordService.List(t => t.StructureID == entry.AttendancePlan && t.UserID == id && t.SignDate == time, query);
                    }
                    else
                    {
                        //如果已经打了白班卡，算作白班，如果未打白班卡，算作夜班。
                        var records = await attendanceRecordService.List(t => t.StructureID == entry.AttendancePlan && t.UserID == id && t.SignDate == time, query);
                        if (records.Count >0)
                        {
                            model.StructureID = day.ID;
                            model.structure = await attendanceStructureService.Get(model.StructureID);
                            model.details = dayWorkDetails.Where(t => t.ConfigID == day.AttendanceID).ToList();
                            model.records = records;
                        }
                        else
                        {
                            model.StructureID = night.ID;
                            model.structure = await attendanceStructureService.Get(model.StructureID);
                            model.details = dayWorkDetails.Where(t => t.ConfigID == night.AttendanceID).ToList();
                            model.records = await attendanceRecordService.List(t => t.StructureID == entry.NightAttendancePlan && t.UserID == id && t.SignDate == time, query);
                        }
                    }
                   
                    #endregion
                }
                model.isWork = true;
                if (model.structure == null)
                {
                    throw new ErrorCodeException(-1, "未配置考勤规则");
                }
                model.AttendanceID = model.structure.AttendanceID.HasValue ? model.structure.AttendanceID.Value : 0;

                model.config = await attendanceConfigService.Get(model.AttendanceID);
            }
            else
            {
                if (!entry.StructureID.HasValue)
                {
                    throw new ErrorCodeException(-1, "未配置车间");
                }
                model.StructureID = entry.StructureID.Value;
                model.structure = await attendanceStructureService.Get(model.StructureID);
                model.AttendanceID = model.structure.AttendanceID.Value;
                if (model.structure == null)
                {
                    throw new ErrorCodeException(-1, "未配置考勤规则");
                }
                model.AttendanceID = model.structure.AttendanceID.HasValue ? model.structure.AttendanceID.Value : 0;

                model.config = await attendanceConfigService.Get(model.AttendanceID);
                model.isWork = await attendanceCalendarService.ExistDate(date, model.config.ID, model.config.Holiday);
                details = await attendanceDetailService.List(t=>t.ConfigID == model.structure.AttendanceID, query);
                string start = "";
                try
                {
                    if (details.Count > 0)
                    {
                        start = date.ToString("yyyy-MM-dd") + " " + details[0].TimeShow + ":00.000";
                        time = Convert.ToDateTime(start);
                        time = time.AddHours(-3);
                        if (date < time)
                        {
                            date = date.AddDays(-1);//如果时间早于今天初始考勤时间的前三小时之前，判断是昨天的考勤
                            time = time.AddDays(-1);//设置为当前考勤时间的初始时间点
                        }
                    }
                }
                catch (Exception e)
                {
                    return Ok(new { items = e.ToString() });
                }
                model.signDate = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));
                if (model.isWork && model.config.RuleType==1)
                {
                    model.details = details;
                }
                else if (model.config.RuleType == 2)
                {
                    model.FreeAttendance(model.AttendanceID);
                }
                else
                {
                    model.JiaBan(model.AttendanceID);
                }

                model.records = await attendanceRecordService.List(t => t.StructureID == entry.StructureID && t.UserID == id && t.SignDate == model.signDate, query);

            }


           

           


            model.applys = await attendanceApplyService.List(t => t.SignDate.HasValue && t.SignDate== model.signDate && t.Status == 2, query);


            return Ok(new { items = model });
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] AttendanceModel model)
        {
            var date = DateTime.Now;
            model.record.AttendanceDate = DateTime.Now;
            if (model.record.Status.HasValue) model.record.StatusName = ((AttendanceStatusEnum)model.record.Status).ToString();
            if (model.record.Type.HasValue) model.record.TypeName = ((AttendanceTypeEnum)model.record.Type).ToString();
            await attendanceRecordService.Update(model.record);
            return Ok(new { items = model });
        }
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AttendanceModel model)
        {
            var user = CurrentUser;
            Model.Live_PrepaidApplyRecord m = new Live_PrepaidApplyRecord();
            var date = DateTime.Now;
            model.record.AttendanceDate = date;
            model.record.CompanyID = user.CompanyID;
            model.record.CompanyGID = user.CompanyGID;
            model.record.StructureID = model.structure.ID;
            model.record.AttendanceType = model.config.Type;
            model.record.PartnerID = model.entry.PartnerID;
            model.record.PartnerName = model.entry.PartnerName;
            if (model.record.Status.HasValue) model.record.StatusName = ((AttendanceStatusEnum)model.record.Status).ToString();
            if (model.record.Type.HasValue) model.record.TypeName = ((AttendanceTypeEnum)model.record.Type).ToString();

            //if(model.record.Status==(int)AttendanceStatusEnum.迟到 || model.record.Status == (int)AttendanceStatusEnum.外勤迟到)
            //{
            //    var applys = await attendanceApplyService.List(t => t.SignDate == DateTime.Today && t.Status == (int)AttendanceApplyStatusEnum.已通过 && t.ApplyType == (int)AttendanceApplyTypeEnum.提前请假, new UserPagedModel { Page = 0 });
            //    if (applys.Count > 0)
            //    {
            //        var today = DateTime.Today;
            //        foreach (var x in applys)
            //        {
            //            var sdate = x.StartDayType == 1 ? today : today.AddDays(1);
            //            var s = sdate.ToString("yyyy-MM-dd") + " " + x.StartTime + ":00.000";
            //            var e = 
            //        }
            //    }
            //}
            model.record.Remark = "正常";

            
            //暂时取消考勤申请对考勤记录的影响，各自独立
            //if(model.record.Type==(int)AttendanceTypeEnum.下班)
            //{
            //    var applys = await attendanceApplyService.List(t => t.SignDate == DateTime.Today && t.Status == (int)AttendanceApplyStatusEnum.已通过 && t.ApplyType == (int)AttendanceApplyTypeEnum.提前请假, new UserPagedModel { Page = 0 });
            //    if(applys.Count>0)
            //    {
            //        model.record.Remark = "";
            //        int index = 0;
            //        foreach (var x in applys)
            //        {
            //            index++;
            //            var s = x.StartDayType == 2 ? "次日" + x.StartTime : x.StartTime;
            //            var e = x.EndDayType == 2 ? "次日" + x.EndTime : x.EndTime;
            //            var dot = index == applys.Count ? "" : ",";
            //            model.record.Remark += s + "-" + e + "已请假"+dot; 
            //        }
            //    }
            //}


            
            
            if(model.config.Type == 2)//日结工添加入职id，岗位id，员工编号
            {
                
                var entryContractRecord = await httpService.GetAsync<Model.Live_V_DayWorkUserContractRecord>(ServiceNames.CustomerService, "/api/DayWorkUserContractRecord/GetView?entry=" + model.entry.ID+"&status="+(int)DayWorkEntryContractTypeEnum.已盖章);
                try
                {
                    model.record.JobNo = entryContractRecord.JobNo;
                    model.record.EntryNo = entryContractRecord.EntryNo;
                }
                catch(Exception e)
                {
                    return Ok(new { msg=e.ToString(), entryContractRecord });
                }
            }

            if(model.record.Type == 2 && model.config.Type==2)//下班卡且是日结工
            {
                var onModel = await attendanceRecordService.Get(t => t.UserID==model.record.UserID && t.Type == 1 && t.SignDate == model.record.SignDate && t.Sort == (model.record.Sort - 1));
                model.record.WorkHour = 0;
                if (onModel != null)
                {
                    if (onModel.AttendanceDate.HasValue)
                    {
                        DateTime big = onModel.AttendanceDate.Value > onModel.DefaultTime.Value ? onModel.AttendanceDate.Value : onModel.DefaultTime.Value;
                        TimeSpan timeSpan = DateTime.Now - big;
                        int hours = timeSpan.Hours;
                        int minutes = timeSpan.Minutes;
                        decimal mi = minutes > 30 ? 0.5M : 0M;
                        if(model.config.TimeRule== (int)AttendanceTimeRuleEnum.一小时)
                        {
                            model.record.WorkHour = hours;
                        }
                        else if (model.config.TimeRule == (int)AttendanceTimeRuleEnum.半小时)
                        {
                            model.record.WorkHour = hours + mi;
                        }

                    }
                }
                #region 预发申请
                decimal workhour = 0M; //工时必须大于早餐扣时加晚餐扣时
                if (model.record.WorkHour.HasValue)
                {
                    workhour = model.record.WorkHour.Value;
                    if (model.config.LunchHours.HasValue && !string.IsNullOrEmpty(model.config.LunchTime))
                    {
                        string lunchstr = date.ToString("yyyy-MM-dd") + " " + model.config.LunchTime + ":00.000";
                        DateTime lunchTime = Convert.ToDateTime(lunchstr);
                        m.LunchHours = 0;
                        if (model.record.AttendanceDate > lunchTime)
                        {
                            workhour = workhour - model.config.LunchHours.Value;
                            m.LunchHours = model.config.LunchHours;
                        }
                       
                    }
                    if (model.config.DinnerHours.HasValue && !string.IsNullOrEmpty(model.config.DinnerTime))
                    {
                        string dinnerstr = date.ToString("yyyy-MM-dd") + " " + model.config.DinnerTime + ":00.000";
                        DateTime dinnerTime = Convert.ToDateTime(dinnerstr);
                        m.DinnerHours = 0;
                        if (model.record.AttendanceDate > dinnerTime)
                        {
                            workhour = workhour - model.config.DinnerHours.Value;
                            m.DinnerHours = model.config.DinnerHours;
                        }
                    }
                    workhour = workhour < 0 ? 0 : workhour;
                }
                if (model.config.Type == 2  && workhour > 0)//配置类型为日结工,下班卡
                {
                    
                    m.UserEntryID = model.entry.ID;
                    m.CompanyID = model.entry.CompanyID;
                    m.CompanyGID = model.entry.CompanyGID;
                    m.ApplyType = PrepaidApplyEnumType.日结工资申请;
                    m.ShiftStartTime = onModel.AttendanceDate;
                    m.ShiftEndTime = model.record.AttendanceDate;
                    m.ShiftHours = model.record.WorkHour;
                    m.ShiftMoney = model.config.ShiftMoney;
                    m.DayWorkType = model.config.DayWorkType;
                    m.MoneyHours = workhour;
                    m.StructureID = model.structure.ID;
                    m.SendType = 0;//0是待发，1是现金
                    m.ApplyAmount = workhour * (model.config.ShiftMoney?? 0M) + (model.config.NightShiftSubsidy ?? 0M);
                    m.NightShiftSubsidy = model.config.NightShiftSubsidy;
                    m.JobNo = model.record.JobNo;
                    m.EntryNo = model.record.EntryNo;
                    var modeljson = JsonConvert.SerializeObject(m);
                    var obj = await httpService.PostModelAsync(ServiceNames.FinanceService, "/api/PrepaidApplyRecord/Apply", modeljson);
                    if (obj.Message != "Success")
                    {
                        throw new ErrorCodeException(-1, obj.Message+ m.UserEntryID);
                    }
                }
                #endregion
                
            }
            
           var i =  await attendanceRecordService.Add(model.record);
            if(i==0) throw new ErrorCodeException(-1, "考勤记录生成失败");
            if (model.record.Type == (int)AttendanceTypeEnum.加班下班 && model.record.AttendanceDate.HasValue && model.record.SignDate.HasValue)
            {
                var shang = await attendanceRecordService.Get(t => t.SignDate.HasValue && t.SignDate.Value.ToString("yyyy-MM-dd") == model.record.SignDate.Value.ToString("yyyy-MM-dd") && t.Type == (int)AttendanceTypeEnum.加班上班 && t.UserID == model.record.UserID);
                if (shang != null)
                {
                    TimeSpan timeSpan = model.record.AttendanceDate.Value - (shang.AttendanceDate.HasValue ? shang.AttendanceDate.Value : model.record.AttendanceDate.Value);
                    int hours = timeSpan.Hours;
                    int minutes = timeSpan.Minutes;
                    decimal mi = minutes > 30 ? 0.5M : 0M;
                    if (model.config.TimeRule == (int)AttendanceTimeRuleEnum.一小时)
                    {
                        model.record.WorkHour = hours;
                    }
                    else if (model.config.TimeRule == (int)AttendanceTimeRuleEnum.半小时)
                    {
                        model.record.WorkHour = hours + mi;
                    } else
                    {
                        model.record.WorkHour = hours;
                    }
                    if (model.record.WorkHour > 0)
                    {
                        //加班下班自动生成加班申请
                        Live_AttendanceApply apply = new Live_AttendanceApply();
                        apply.CompanyID = model.structure.CompanyID;
                        apply.CompanyGID = model.structure.CompanyGID;
                        apply.UserID = model.UserID;
                        apply.CustomerID = model.structure.CustomerID;
                        apply.CustomerName = model.entry.CustomerName;
                        apply.RecordID = i;
                        apply.Status = 1;
                        apply.UserName = model.record.UserName;
                        apply.SignDate = model.record.SignDate;
                        apply.StartTime = shang.AttendanceDate.HasValue? shang.AttendanceDate.Value.ToString("yyyy-MM-dd HH:mm:ss"): "";
                        apply.EndTime = model.record.AttendanceDate.HasValue ? model.record.AttendanceDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
                        apply.StructureID = model.record.StructureID;
                        apply.Number = model.record.WorkHour;
                        apply.CreateTime = DateTime.Now;
                        apply.UpTime =  shang.AttendanceDate;
                        apply.OffTime = model.record.AttendanceDate;
                        apply.Content = model.record.Content;
                        apply.ApplyType = (int)AttendanceApplyTypeEnum.加班;
                        apply.FileInfo = model.record.FileInfo;
                        apply.Type = (int)AttendanceTypeEnum.加班申请;
                        apply.DayType = 1;
                        UserPagedModel query = new UserPagedModel();
                        query.Page = 0;
                        var list = await attendanceStructureService.List(t => t.NavID == model.record.StructureID, query);
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
                        apply.AuditStructure = pitem.ID;
                        apply.ManageGIDS = pitem.UserIDS;
                        var ii = await attendanceApplyService.Add(apply);


                    }
                    return Ok(new { shang, items = model.record, hour = model.record.WorkHour });
                }
                
            }

            return Ok(new { items = model });
        }
        [HttpGet]
        public async Task<IActionResult> StatisticsList([FromQuery] UserPagedModel query)
        {
            var date = DateTime.Now;
            UserPagedModel nopage = new UserPagedModel();
            DateTime start;
            DateTime end;
            string d = "";
            if (query.Year.HasValue && query.Month.HasValue)
            {
                d = query.Year + "-" + query.Month + "-01";
            }
            else
            {
                d = date.Year + "-" + date.Month + "-01";
            }
            start = Convert.ToDateTime(d);
            end = (date.ToString("yyyy-MM") == start.ToString("yyyy-MM")) ? DateTime.Today : start.AddMonths(1);
            //end = start.AddMonths(1);
            nopage.Page = 0;
            var structures = await attendanceStructureService.List(t => t.ParentID == 0 && t.CustomerID == query.customerID, nopage);
            List<int> configArr = new List<int>();
            foreach (var s in structures)
            {
                if (s.AttendanceID.HasValue)
                {
                    if (configArr.Contains(s.AttendanceID.Value)) continue;
                    configArr.Add(s.AttendanceID.Value);
                }
            }
            var configs = await attendanceConfigService.List(t => configArr.Contains(t.ID), nopage);
            var deatils = await attendanceDetailService.List(t => configArr.Contains(t.ConfigID), nopage);
            var expable_Calendar = Expressionable.Create<Model.Live_AttendanceCalendar>();
            expable_Calendar.And(t => t.Calendar >= start && t.Calendar < end);
            var calendars = await attendanceCalendarService.List(expable_Calendar.ToExpression(), nopage);

            var expable_entry = Expressionable.Create<Model.Live_V_UserEntry>();
            expable_entry.And(t => t.StructureID.HasValue);
            expable_entry.And(t => t.CustomerID == query.customerID);
            expable_entry.And(t => t.Status == 2 || (t.Status == 4 && t.LeaveTime.HasValue && t.LeaveTime.Value.ToString("yyyy-MM")== start.ToString("yyyy-MM")));//在职或者当月离职
            expable_entry.AndIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name));
            expable_entry.AndIF(query.structureID > 0, t => t.StructureID == query.structureID);
            expable_entry.AndIF(query.UserID.HasValue, t => t.UserID == query.UserID);
            var entrys = await entryService.List(expable_entry.ToExpression(), query);
            List<int> userArr = new List<int>();
            foreach (var x in entrys)
            {
                if (x.UserID.HasValue)
                {
                    if (userArr.Contains(x.UserID.Value)) continue;
                    userArr.Add(x.UserID.Value);
                }

            }
            var expable_record = Expressionable.Create<Model.Live_AttendanceRecord>();
            expable_record.And(t => t.SignDate >= start && t.SignDate < end);
            expable_record.And(t=>t.CustomerID == query.customerID);
            expable_record.And(t => t.UserID.HasValue && userArr.Contains(t.UserID.Value));
            var records = await attendanceRecordService.List(expable_record.ToExpression(), nopage);

            var expable_apply = Expressionable.Create<Model.Live_V_AttendanceApply>();
            expable_apply.And(t => Convert.ToDateTime(t.StartTime) >= start && Convert.ToDateTime(t.EndTime) < end);
            expable_apply.And(t => t.CustomerID == query.customerID);
            expable_apply.And(t => t.UserID.HasValue && userArr.Contains(t.UserID.Value));
            expable_apply.And(t => t.Status == 2);
            var applys = await attendanceApplyService.List(expable_apply.ToExpression(), nopage);

            List<AttendanceCustomerModel> list = new List<AttendanceCustomerModel>();
            foreach (var entry in entrys)
            {
                AttendanceCustomerModel item = new AttendanceCustomerModel(entry.UserID, start.Year, start.Month);
                item.entry = entry;
                item.structure = structures.Find(t => t.ID == entry.StructureID);
                var apply = applys.Where(t => t.UserID == entry.UserID).ToList();
                if (item.structure == null)
                {
                    item.success = false;
                    item.message += ",缺失车间配置";
                }
                else
                {
                    item.config = configs.Find(t => t.ID == item.structure.AttendanceID);
                    if (item.config == null)
                    {
                        item.success = false;
                        item.message += ",缺失考勤配置";
                        
                    }
                    else
                    {
                        if (item.structure.AttendanceType == 1 && item.config.RuleType==1 && string.IsNullOrEmpty(item.config.Days))
                        {
                            item.success = false;
                            item.message += ",未分配工作日";
                        }
                        var w = calendars.Where(t => t.Calendar < end);
                        if (item.structure.AttendanceType == 1 && item.structure.RuleType==1)
                        {
                            string[] arr = item.config.Days.Split(",");
                            //w.Where(t => arr.Contains(t.DayEnum.ToString()));
                            if (item.config.Holiday)
                            {
                                item.calendars = calendars.Where(t => t.Calendar < end && (t.Status==1 || t.Status==3) && arr.Contains(t.DayEnum.ToString())).ToList();
                            } else
                            {
                                item.calendars = calendars.Where(t => t.Calendar < end && arr.Contains(t.DayEnum.ToString())).ToList();
                            }
                        } 
                        else
                        {
                            item.calendars = calendars.Where(t => t.Calendar < end).ToList();
                        }
                        if (item.calendars.Count == 0)
                        {
                            item.success = false;
                            item.message += ",出勤天数为0";
                        }
                    }
                    
                    if(item.config.RuleType == 1)
                    {
                        item.details = deatils.Where(t => t.ConfigID == item.structure.AttendanceID).ToList();
                        if (item.details.Count == 0)
                        {
                            item.success = false;
                            item.message += ",缺失考勤时间配置";
                        }
                    }
                    
                }
                if (item.success && entry.UserID.HasValue)
                {
                    item.records = records.Where(t => t.UserID == entry.UserID.Value).ToList();
                    item.model.TotalDays = item.calendars.Count;
                    item.model.Days = item.records.Where(t=>t.Status!=(int)AttendanceStatusEnum.加班 && t.Status != (int)AttendanceStatusEnum.外勤加班).GroupBy(t => t.SignDate).Select(g => new { SignDate = g.Key, Count = g.Count() }).ToList().Count;
                    item.model.OutsideWorkCount = item.records.Where(t => t.Status == (int)AttendanceStatusEnum.外勤 || t.Status == (int)AttendanceStatusEnum.外勤迟到 || t.Status == (int)AttendanceStatusEnum.外勤早退).ToList().Count;
                    item.model.LateCount = item.records.Where(t => t.Status == (int)AttendanceStatusEnum.迟到 || t.Status == (int)AttendanceStatusEnum.外勤迟到).ToList().Count;
                    item.model.LeaveEarlyCount = item.records.Where(t => t.Status == (int)AttendanceStatusEnum.早退 || t.Status == (int)AttendanceStatusEnum.外勤早退).ToList().Count;
                    item.model.NoCardCount = 0;
                    item.model.AskForLeaveCount = apply.Where(t => t.ApplyType == (int)AttendanceApplyTypeEnum.请假 || t.ApplyType == (int)AttendanceApplyTypeEnum.补卡).ToList().Count;
                    item.model.OverWorkCount = apply.Where(t => t.ApplyType == (int)AttendanceApplyTypeEnum.加班).ToList().Count;
                    item.model.SignCount = records.Count;
                    //item.model.AskForLeaveCount = apply.Where(t=>t.ApplyType ==(int)AttendanceApplyTypeEnum.请假 || t.ApplyType == (int)AttendanceApplyTypeEnum.补卡).ToList().Count;
                    if (item.structure.RuleType == 2)
                    {
                        item.model.TotalDays = item.model.Days;
                        
                    } else
                    {
                        item.model.NoCardCount = item.model.TotalDays * item.details.Count - records.Where(t => t.Type != 3 && t.Type != 4).ToList().Count;
                    }
                }
                list.Add(item);
            }


            return Ok(new { items = list, structures, records,entrys, count = query.Count});
        }
        [HttpGet]
        public async Task<IActionResult> DetailShow([FromQuery] UserPagedModel query)
        {
            if(query.structureID==0) throw new ErrorCodeException(-1, "组织架构参数不全");
            if(query.customerID == 0) throw new ErrorCodeException(-1, "客户单位参数不全");
            if (!query.UserID.HasValue) throw new ErrorCodeException(-1, "员工参数不全");
            var entry = await entryService.View(t => t.UserID == query.UserID && t.Status == 2);
            var date = DateTime.Now;
            var today = DateTime.Today;
            UserPagedModel nopage = new UserPagedModel();
            DateTime start;
            DateTime end;
            string d = "";
            if (query.Year.HasValue && query.Month.HasValue)
            {
                d = query.Year + "-" + query.Month + "-01";
            }
            else
            {
                d = date.Year + "-" + date.Month + "-01";
            }
            start = Convert.ToDateTime(d);
            end = (date.ToString("yyyy-MM") == start.ToString("yyyy-MM")) ? DateTime.Today : start.AddMonths(1);
            nopage.Page = 0;
            var expable_record = Expressionable.Create<Model.Live_AttendanceRecord>();
            expable_record.And(t => t.CustomerID == query.customerID);
            
            expable_record.And(t => t.SignDate >= start && t.SignDate < end);
            expable_record.And(t => t.UserID==query.UserID);
            var records = await attendanceRecordService.List(expable_record.ToExpression(), nopage);


            var expable_apply = Expressionable.Create<Model.Live_V_AttendanceApply>();
            expable_apply.And(t => t.SignDate >= start && t.SignDate < end);
            expable_apply.And(t => t.CustomerID == query.customerID);
            expable_apply.And(t => t.Status == 1);
            expable_apply.And(t => t.UserID == query.UserID);
            var applys = await attendanceApplyService.List(expable_apply.ToExpression(), query);

            List<Model.Live_AttendanceRecord> list = new List<Live_AttendanceRecord>();

            if (query.Type == "1")//外勤
            {
                list = records.Where(t => t.Status == (int)AttendanceStatusEnum.外勤).ToList();
            }
            else if (query.Type == "2")//迟到
            {
                list = records.Where(t => t.Status == (int)AttendanceStatusEnum.迟到 || t.Status == (int)AttendanceStatusEnum.外勤迟到).ToList();
            }
            else if (query.Type == "3")//早退
            {
                list = records.Where(t => t.Status == (int)AttendanceStatusEnum.早退 || t.Status == (int)AttendanceStatusEnum.外勤早退).ToList();
            }
            else if (query.Type == "4")//缺卡
            {
                var structure = await attendanceStructureService.Get(t => t.ID == query.structureID);
                if (structure == null) throw new ErrorCodeException(-1, "没有匹配的组织架构");
                if (!structure.AttendanceID.HasValue) throw new ErrorCodeException(-1, "没有匹配的组织架构");
                var config = await attendanceConfigService.Get(structure.AttendanceID.Value);
                if (config == null) throw new ErrorCodeException(-1, "没有匹配的考勤配置");
                if (config.RuleType == 1)
                {
                    var deatils = await attendanceDetailService.List(t => t.ConfigID == config.ID, nopage);
                    if (deatils.Count == 0) throw new ErrorCodeException(-1, "没有设置考勤时间");
                    var expable_Calendar = Expressionable.Create<Model.Live_AttendanceCalendar>();
                    expable_Calendar.And(t => t.Calendar >= start && t.Calendar < end);
                    var calendars = await attendanceCalendarService.List(expable_Calendar.ToExpression(), nopage);
                    var w = calendars.Where(t => t.Calendar < end);
                    if (structure.AttendanceType == 1)
                    {
                        string[] arr = config.Days.Split(",");
                        w.Where(t => arr.Contains(t.DayEnum.ToString()));
                        if (config.Holiday)
                        {
                            w.Where(t => t.Status == 1 || t.Status == 3);
                        }

                    }
                    List<Live_AttendanceCalendar> calendarWorks = new List<Live_AttendanceCalendar>();
                    if (structure.AttendanceType == 1)
                    {
                        string[] arr = config.Days.Split(",");
                        if (config.Holiday)
                        {
                            calendarWorks = calendars.Where(t => t.Calendar < end && (t.Status == 1 || t.Status == 3) && arr.Contains(t.DayEnum.ToString())).ToList();
                        }
                        else
                        {
                            calendarWorks = calendars.Where(t => t.Calendar < end && arr.Contains(t.DayEnum.ToString())).ToList();
                        }
                    }
                    else
                    {
                        calendarWorks = w.ToList();
                    }

                    foreach (var c in calendarWorks)
                    {
                        if (!c.Calendar.HasValue) continue;
                        foreach (var detail in deatils)
                        {
                            var record = records.Where(t => t.SignDate.HasValue && t.SignDate.Value.ToString("yyyy-MM-dd") == c.Calendar.Value.ToString("yyyy-MM-dd") && t.Sort == detail.Sort).FirstOrDefault();
                            var signTime = Convert.ToDateTime(c.Calendar.Value.ToString("yyyy-MM-dd"));
                            //if (date.ToString("yyyy-MM") == c.Calendar.Value.ToString("yyyy-MM"))
                            //{
                            //    continue;//当前时间未到的打卡不能显示（未来）
                            //}
                            if (entry.EntryTime.HasValue)
                            {
                                var entryDate = Convert.ToDateTime(entry.EntryTime.Value.ToString("yyyy-MM-dd"));
                                var signDate = Convert.ToDateTime(signTime.ToString("yyyy-MM-dd"));
                                if (entryDate > signDate)
                                {
                                    continue;//入职前的日期不进入统计
                                }
                            }

                            if (record == null)
                            {
                                Model.Live_AttendanceRecord item = new Live_AttendanceRecord();
                                var dd = detail.DayType == 1 ? c.Calendar.Value.ToString("yyyy-MM-dd") : c.Calendar.Value.AddDays(1).ToString("yyyy-MM-dd");
                                dd += " " + detail.TimeShow + ":00";
                                item.SignDate = c.Calendar;
                                item.Sort = detail.Sort;
                                item.UserID = query.UserID;
                                item.UserName = query.Name;
                                item.CustomerID = query.customerID;
                                item.Status = (int)AttendanceStatusEnum.缺卡;
                                item.StatusName = AttendanceStatusEnum.缺卡.ToString();
                                item.Type = detail.Type;
                                item.AttendanceDate = Convert.ToDateTime(dd);
                                if (detail.Type.HasValue)
                                {
                                    item.TypeName = ((AttendanceTypeEnum)detail.Type.Value).ToString();
                                }
                                item.AddressName = config.AddressName;
                                item.TimeShow = detail.TimeShow;
                                item.OnTime = config.OnTime;
                                item.OffTime = config.OffTime;
                                item.DayType = detail.DayType;
                                list.Add(item);
                            }
                        }
                    }
                }


            }
            else if (query.Type == "5")//考勤记录
            {
                list = records;
            }

            return Ok(new { items = list, applys });
        }

        [HttpGet]
        public async Task<IActionResult> GetDayWorkStructure(int userID)
        {
            var entry = await entryService.View(t => t.UserID == userID && t.Status == 2);
            if(entry==null)
            {
                throw new ErrorCodeException(-1, "入职信息不存在");
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> StatisticsExportExcel([FromQuery] UserPagedModel query)
        {
            var date = DateTime.Now;
            if (!query.Year.HasValue)
            {
                query.Year = date.Year;
            }
            if (!query.Month.HasValue)
            {
                query.Month = date.Month;
            }
            string d = query.Year + "-" + query.Month + "-01";
            var start = Convert.ToDateTime(d);
            var end = start.AddMonths(1);
            

            //var end = (date.ToString("yyyy-MM") == start.ToString("yyyy-MM")) ? DateTime.Today.AddDays(1) : start.AddMonths(1);
            var expable = Expressionable.Create<Model.Live_V_AttendanceRecord>();
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.UserName == query.Name);
            expable.AndIF(query.customerID > 0, t => t.CustomerID == query.customerID);
            expable.AndIF(query.structureID > 0, t => t.StructureID == query.structureID);
            if (!string.IsNullOrEmpty(query.CustomerIDS))
            {
                var arr = query.CustomerIDS.Split(',');
                expable.And(t=>arr.Contains(t.CustomerID.ToString()));
            }
            expable.And(t => t.SignDate >= start && t.SignDate < end);
            var list = await attendanceRecordService.ListView(expable.ToExpression(),new UserPagedModel { Page=0,Order= "userName desc,AttendanceDate asc" });

            var expable_apply = Expressionable.Create<Model.Live_V_AttendanceApply>();
            expable_apply.AndIF(!string.IsNullOrEmpty(query.Name), t => t.UserName == query.Name);
            expable_apply.AndIF(query.customerID > 0, t => t.CustomerID == query.customerID);
            if (!string.IsNullOrEmpty(query.CustomerIDS))
            {
                var arr = query.CustomerIDS.Split(',');
                expable_apply.And(t => arr.Contains(t.CustomerID.ToString()));
            }
            expable_apply.AndIF(query.structureID > 0, t => t.StructureID == query.structureID);
            expable_apply.And(t => Convert.ToDateTime(t.StartTime) >= start && Convert.ToDateTime(t.EndTime) < end);
            expable_apply.And(t => t.Status == 2);
            var applys = await attendanceApplyService.List(expable_apply.ToExpression(), new UserPagedModel { Page = 0, Order = "userName desc,SignDate asc, sort asc" });


            //获取当前在职，或者离职时间为本月的员工
            var expable_entry = Expressionable.Create<Model.Live_V_UserEntry>();
            expable_entry.And(t => t.StructureID.HasValue);
            expable_entry.And(t => t.Status == 2 || (t.Status == 4 && t.LeaveTime.HasValue && t.LeaveTime.Value.ToString("yyyy-MM") == start.ToString("yyyy-MM")));
            if (!string.IsNullOrEmpty(query.CustomerIDS))
            {
                var arr = query.CustomerIDS.Split(',');
                expable_entry.And(t => arr.Contains(t.CustomerID.ToString()));
            }
            expable_entry.AndIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name));
            expable_entry.AndIF(query.structureID > 0, t => t.StructureID == query.structureID);
            //if (query.customerID > 0 && query.structureID > 0)
            //{
            //    expable_user.And(it => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.CustomerID == query.customerID && s.StructureID == query.structureID && (s.Status == 2 || (s.Status == 4 && s.LeaveTime.HasValue && s.LeaveTime.Value.ToString("yyyy-MM") == start.ToString("yyyy-MM"))) && s.UserID == it.UserID).Any());
            //}
            //else if (query.customerID > 0)
            //{
            //    expable_user.And(it => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.CustomerID == query.customerID && (s.Status == 2 || (s.Status == 4 && s.LeaveTime.HasValue && s.LeaveTime.Value.ToString("yyyy-MM") == start.ToString("yyyy-MM"))) && s.UserID == it.UserID).Any());
            //}
            //else if (query.structureID > 0)
            //{
            //    expable_user.And(it => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.StructureID == query.structureID && (s.Status == 2 || (s.Status == 4 && s.LeaveTime.HasValue && s.LeaveTime.Value.ToString("yyyy-MM") == start.ToString("yyyy-MM"))) && s.UserID == it.UserID).Any());
            //}
            //else
            //{
            //    expable_user.And(it => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => (s.Status == 2 || (s.Status == 4 && s.LeaveTime.HasValue && s.LeaveTime.Value.ToString("yyyy-MM") == start.ToString("yyyy-MM"))) && s.UserID == it.UserID).Any());
            //}
            try
            {
                var users = await entryService.List(expable_entry.ToExpression(), new UserPagedModel { Page = 0 });

                var result = await attendanceService.StatisticsExportExcel(list, applys, users, query.structureID, start, end.AddDays(-1),query);
                string base64String = Convert.ToBase64String(result);
                return Ok(new { base64String, fileName = "打卡记录", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",users });
            }
            catch(Exception e)
            {
                throw new ErrorCodeException(-1, e.ToString());
            }
        }
    }
}
