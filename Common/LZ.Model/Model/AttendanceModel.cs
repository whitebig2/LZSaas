using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.Enums.Attendance;

namespace LZ.Model.Model
{

    public class AttendanceModel
    {
        public Live_AttendanceCalendar? calendar { get; set; }

        public Live_V_UserEntry? entry { get; set; }
        public Live_AttendanceStructure? structure { get; set; }

        public Live_AttendanceConfig? config { get; set; }

        public List<Live_AttendanceRecord>? records { get; set; }

        public List<Live_AttendanceDetail>? details { get; set; }

        public Live_AttendanceRecord? record { get; set; }

        public List<Live_V_AttendanceApply>? applys { get; set; }

        public Live_FileUpload? files { get; set; }

        public bool isWork { get; set; }
        public int UserID { get; set; }
        public int StructureID { get; set; }
        public int AttendanceID { get; set; }

        public DateTime? signDate { get; set; }
        public void JiaBan(int configID)
        {
            this.details = new List<Live_AttendanceDetail>();
            Live_AttendanceDetail shang = new Live_AttendanceDetail();
            shang.ConfigID = configID;
            shang.Type = (int)AttendanceTypeEnum.加班上班;
            shang.TimeType = "上班";
            shang.TimeShow = "加班";
            shang.DayType = 1;
            shang.IsAuto = false;
            Live_AttendanceDetail xia = new Live_AttendanceDetail();
            xia.ConfigID = configID;
            xia.Type = (int)AttendanceTypeEnum.加班下班;
            xia.TimeType = "下班";
            xia.TimeShow = "加班";
            xia.DayType = 1;
            xia.IsAuto = false;
            this.details.Add(shang);
            this.details.Add(xia);

        }
        public void FreeAttendance(int configID)
        {
            this.details = new List<Live_AttendanceDetail>();
            Live_AttendanceDetail shang = new Live_AttendanceDetail();
            shang.ConfigID = configID;
            shang.Type = (int)AttendanceTypeEnum.上班;
            shang.TimeType = "上班";
            shang.TimeShow = "";
            shang.DayType = 1;
            shang.IsAuto = false;
            Live_AttendanceDetail xia = new Live_AttendanceDetail();
            xia.ConfigID = configID;
            xia.Type = (int)AttendanceTypeEnum.下班;
            xia.TimeType = "下班";
            xia.TimeShow = "";
            xia.DayType = 1;
            xia.IsAuto = false;
            this.details.Add(shang);
            this.details.Add(xia);

        }
    }
    public class AttendanceCustomerModel
    {
        public int? userID { get; set; }
        public Live_V_UserEntry? entry { get; set; }
        public AttendanceStatisticsModel? model { get; set; }

        public List<Live_AttendanceCalendar>? calendars { get; set; }

        public Live_V_AttendanceStructure? structure { get; set; }

        public Live_AttendanceConfig? config { get; set; }

        public List<Live_AttendanceRecord>? records { get; set; }

        public List<Live_AttendanceDetail>? details { get; set; }
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public AttendanceCustomerModel(int? id,int?year,int?month)
        {
            this.model = new AttendanceStatisticsModel(id, year,month);
            if (id.HasValue)
            {
                this.success = true;
                this.message = "关联员工正常";
                this.userID = id;
            }
            else
            {
                this.success = false;
                this.message = "未关联员工";
                this.userID = 0;
            }

        }

    }

    public class AttendanceStatisticsModel
    {
        public int? UserID { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        //public Live_AttendanceStructure? structure { get; set; }
        /// <summary>
        /// 异常数量
        /// </summary>
        public int AbnormalCount { get; set; }
        /// <summary>
        /// 应出勤天数
        /// </summary>
        public int TotalDays { get; set; }
        /// <summary>
        /// 出勤天数
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// 外勤次数
        /// </summary>
        public int OutsideWorkCount { get; set; }
        /// <summary>
        /// 迟到数量
        /// </summary>
        public int LateCount { get; set; }

        /// <summary>
        /// 早退数量
        /// </summary>
        public int LeaveEarlyCount { get; set; }
        /// <summary>
        /// 缺卡数量
        /// </summary>
        public int NoCardCount { get; set; }
        /// <summary>
        /// 加班数量
        /// </summary>
        public int OverWorkCount { get; set; }

        /// <summary>
        /// 请假数量
        /// </summary>
        public int AskForLeaveCount { get; set; }

        /// <summary>
        /// 打卡数量
        /// </summary>
        public int SignCount { get; set; }

        public AttendanceStatisticsModel(int? id,int?year,int? month)
        {
            this.UserID = id;
            this.Year = year;
            this.Month = month;
            this.AbnormalCount = 0;
            this.Days = 0;
            this.TotalDays = 0;
            this.OutsideWorkCount = 0;
            this.LateCount = 0;
            this.LeaveEarlyCount = 0;
            this.NoCardCount = 0;
        }
    }
    public class AttendanceOverWorkMode
    {
        public string text { get; set; }

        public int value { get; set; }
        public AttendanceOverWorkMode(string name,int value)
        {
            this.text = name;
            this.value = value;
        }
    } 


}
