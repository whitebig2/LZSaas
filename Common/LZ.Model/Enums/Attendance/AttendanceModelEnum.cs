using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Attendance
{
    public enum AttendanceTypeEnum
    {
        上班 = 1,
        下班 = 2,
        加班上班 = 3,
        加班下班 = 4,
        请假申请 = 5,
        加班申请 = 6,
    }
    public enum AttendanceStatusEnum
    {
        正常 = 1,
        迟到 = 2,
        早退 = 3,
        外勤 = 4,
        外勤迟到 = 5,
        外勤早退 = 6,
        缺卡 = 7,
        请假 = 8,
        提前请假 = 9,
        加班 = 10,
        补卡 = 11,
        外勤加班 = 12
    }

    public enum AttendanceDayTypeEnum
    {
        当日 = 1,
        次日 = 2,
    }
    public enum AttendanceApplyTypeEnum
    {
        请假 = 1,
        补卡 = 2,
        加班 = 3,
    }

    public enum AttendanceTimeRuleEnum
    {
        一小时 = 1,
        半小时 = 2,

    }

    public enum AttendanceCustomerTypeEnum
    {
        正常考勤 = 1,
        日结工 = 2,

    }
    public enum AttendanceApplyStatusEnum
    {
        申请中 = 1,
        已通过 = 2,
        未通过 = 3,
        已撤销 = 4,
    }
    public enum AttendanceDayWorkApplyStatusEnum
    {
        申请中 = 1,
        已通过 = 2,
        未通过 = 3,
        已撤销 = 4,
    }
    public enum AttendancePrepaidApplyCustomerStatusEnum
    {
        待审核 = 1,
        已审核 = 2,
    }

    public enum DayWorkCustomerContractRecordStatusEnum
    {
        待盖章 = 1,
        已盖章 = 2,
        已过期 = 3
    }
    public enum AskForLeaveEnum
    {
        调休假 = 1,
        年休假 = 2,
        工伤假 = 3,
        婚假 = 4,
        丧假 = 5,
        产检假 = 6,
        产假 = 7,
        陪产假 = 8,
        流产假 = 9,
        哺乳假 = 10,
        陪护假 = 11,
        事假 = 12,
        病假 = 13,
    }
    public enum OverWorkEnum
    {
        工作日 = 1,
        休息日 = 2,
        节假日 = 3,
        其他 = 4,
    }
}
