using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum PersonnelFunction
    {
        /// <summary>
        /// 待入职网签员工
        /// </summary>
        WaitEntrySignCount = 1,

        /// <summary>
        /// 待离职网签员工
        /// </summary>
        WaitLeaveSignCount = 2,
        /// <summary>
        /// 现金签收员工
        /// </summary>
        CashSignCount = 3,
        /// <summary>
        /// 员工预付工资
        /// </summary>
        DeadHorseCount = 4,

        /// <summary>
        /// 员工日结工资
        /// </summary>
        DayWagesCount = 5,
        /// <summary>
        /// 员工工资异常
        /// </summary>
        WagesErrorCount = 6,
        /// <summary>
        /// 待开户人数
        /// </summary>
        WaitAccountCount = 7,
        /// <summary>
        /// 无银行卡员工
        /// </summary>
        NoBankCount = 8,
        /// <summary>
        /// 工伤人数
        /// </summary>
        InjuryCount = 9,
        /// <summary>
        /// 5类参保
        /// </summary>
        FiveTypeCount = 10,
        /// <summary>
        /// 合作企业
        /// </summary>
        CustomerCount = 11,
        /// <summary>
        /// 蓝领用户
        /// </summary>
        UserCount = 12,
        /// <summary>
        /// 星级服务商(人数)
        /// </summary>
        StarServiceUserCount = 13,
        /// <summary>
        /// 招聘服务商(家数)
        /// </summary>
        HirePartnerCount = 15,
        /// <summary>
        /// 服务商汇总
        /// </summary>
        AllServiceUserCount = 19,
        /// <summary>
        /// 星级服务商(个数)
        /// </summary>
        StarServiceCount = 20,
        /// <summary>
        /// 孵化服务商
        /// </summary>
        HatchServiceCount = 21,
        /// <summary>
        /// 自营公司
        /// </summary>
        SelfUserCount = 22,
        /// <summary>
        /// 联营公司
        /// </summary>
        JointUserCount = 23,
        /// <summary>
        /// 招聘服务商(人数)
        /// </summary>
        HirePartnerUserCount = 24,
        /// <summary>
        /// 劳动关系
        /// </summary>
        LaborrelationUserCount = 25,
        /// <summary>
        /// 劳务联盟
        /// </summary>
        UnionUserCount = 26,
        /// <summary>
        /// 在职人数
        /// </summary>
        OnTheJobCount = 27,
        /// <summary>
        /// 今日入职
        /// </summary>
        ToDayEntryCount = 28,
        /// <summary>
        /// 今日离职
        /// </summary>
        ToDayLeaveCount = 29,
        /// <summary>
        /// 待入职
        /// </summary>
        WaitEntryCount = 30,
        /// <summary>
        /// 入职网签完成
        /// </summary>
        EntrySignCount = 31,
        /// <summary>
        /// 离职网签完成
        /// </summary>
        LeaveSignCount = 32,
        /// <summary>
        /// 待网签
        /// </summary>
        WaitSignCount = 33,
        /// <summary>
        /// 本月收入
        /// </summary>
        NowMonthIncome = 34,
        /// <summary>
        /// 本月成本
        /// </summary>
        NowMonthCost = 35,
        /// <summary>
        /// 本月结算
        /// </summary>
        NowMonthSettlement = 36,
        /// <summary>
        /// 社保费用
        /// </summary>
        SociaCostl = 37,
        /// <summary>
        /// 代缴税费
        /// </summary>
        TakePayTaxation = 38,
        /// <summary>
        /// 工伤保险
        /// </summary>
        InjuryInsurance = 39,
        /// <summary>
        /// 风险金
        /// </summary>
        RiskFund = 40,
        /// <summary>
        /// 业务派单费
        /// </summary>
        BusinessFee = 41,

        /// <summary>
        /// 工资暂缓
        /// </summary>
        SalayDelayCount = 60,
        /// <summary>
        /// 预发工资现金签收单
        /// </summary>
        PerpaidCashCount = 61,
        /// <summary>
        /// 服务商在职人数
        /// </summary>
        OnPartnerTheJobCount = 62,
        /// <summary>
        /// 服务商今日入职
        /// </summary>
        ToPartnerDayEntryCount = 63,
        /// <summary>
        /// 服务商今日离职
        /// </summary>
        ToPartnerDayLeaveCount = 64,
        /// <summary>
        /// 服务商待入职
        /// </summary>
        PartnerWaitEntryCount = 65,

        /// <summary>
        /// 合同超期人数
        /// </summary>
        WaitEntrySignTimeoutCount = 66,

        /// <summary>
        /// 批量交付人数
        /// </summary>
        // BatchUserCount = 68,
        BatchUserCount = 71,
        /// <summary>
        /// 日结结算单
        /// </summary>
        AttendancePrepaidApply = 70,

        /// <summary>
        /// 系统通知
        /// </summary>
        NoticeCount = 79,
        /// <summary>
        /// 现金代理人转账状态
        /// </summary>
        CashAgentTransCount = 80,

        /// <summary>
        /// 考勤申请
        /// </summary>
        AttendanceApply = 83



    }
}
