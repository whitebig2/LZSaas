using LZ.Statistics.Service.Interface;
using LZ.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Model.Enums;
using System.Globalization;
using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.Statistics.Service.Services;
using NPOI.SS.Formula.Functions;
using LZ.Model.Enums.Attendance;

namespace LZ.Statistics.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class PersonnelStatisticsController(
        IPrepaidApplyRecordService prepaidApplyRecordService,
        IEntryStatisticsService entryStatisticsService,
        IAuthService authService, IUserStatisticsService userStatisticsService, 
        ICustomerStatisticsService customerStatisticsService, 
        IPartnerStatisticsService partnerStatisticsService,
        IBasicDataService basicDataService,
        ICompanyConfigService companyConfigService, 
        ISalaryStatisticsService salaryStatisticsService, 
        IPrepaidApplyStatisticsService prepaidApplyStatisticsService,
        IContractStatisticsService contractStatisticsService,
        IInvoiceService invoiceService,
        IAttendanceStatistics attendanceStatistics,
        IInjuryStatisticService injuryStatisticService,
        IJobVedioService jobVedioService,
        INotificationStatisticService notificationStatisticService
        ) : StatisticsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> HomeStatistics()
        {
            var user = CurrentUser;
            var company = await companyConfigService.Get(user.CompanyGID);
            var companyInfo = company.Company;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            //int
            if (!model.UserType.HasValue)
            {
                model.UserType = (int)Model.Enums.PersonnelType.Personnel;
            }
            //获取权限
            var funItems = await basicDataService.PersonnelFunctionItems(it => it.UserType == model.UserType && it.CompanyID == user.CompanyID);
            var counts = new Dictionary<string, object>();
            //权限判断
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.OnTheJobCount || it.PFID == (int)PersonnelFunction.WaitEntryCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
                var statuss = new int[] { (int)UserStatus.Entry,(int)UserStatus.WaitEntry };
                expable.And(it => statuss.Contains(it.Status.Value));
                var exp = expable.ToExpression();
                var statusCount = await entryStatisticsService.StatusCount(exp);
                if (statusCount.Count > 0)
                {
                    var data = statusCount.Find(it => it.Status == (int)UserStatus.Entry);
                    if (data != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.OnTheJobCount.ToString()), data.Count);
                    }
                    data = statusCount.Find(it => it.Status == (int)UserStatus.WaitEntry);
                    if (data != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.WaitEntryCount.ToString()), data.Count);
                    }
                }
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.ToDayEntryCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
                expable.And(it => SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && (it.Status==(int)UserStatus.Entry || it.Status==(int)UserStatus.Leave));
                var exp = expable.ToExpression();
                var count = await entryStatisticsService.Count(exp);
                counts.Add(LowerFirstLetter(PersonnelFunction.ToDayEntryCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.ToDayLeaveCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
                expable.And(it =>SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave);
                var exp = expable.ToExpression();
                var count = await entryStatisticsService.Count(exp);
                counts.Add(LowerFirstLetter(PersonnelFunction.ToDayLeaveCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.OnPartnerTheJobCount || it.PFID == (int)PersonnelFunction.PartnerWaitEntryCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                UserEntryHandle.SetPersonnelAuthNoFactory(expable, model, user.CompanyType);
                var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
                expable.And(it => statuss.Contains(it.Status.Value));
                var exp = expable.ToExpression();
                var statusCount = await entryStatisticsService.StatusCount(exp);
                if (statusCount.Count > 0)
                {
                    var data = statusCount.Find(it => it.Status == (int)UserStatus.Entry);
                    if (data != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.OnPartnerTheJobCount.ToString()), data.Count);
                    }
                    data = statusCount.Find(it => it.Status == (int)UserStatus.WaitEntry);
                    if (data != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.PartnerWaitEntryCount.ToString()), data.Count);
                    }
                }
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.ToPartnerDayEntryCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                UserEntryHandle.SetPersonnelAuthNoFactory(expable, model, user.CompanyType);
                expable.And(it => SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && (it.Status == (int)UserStatus.Entry || it.Status == (int)UserStatus.Leave));
                var exp = expable.ToExpression();
                var count = await entryStatisticsService.Count(exp);
                counts.Add(LowerFirstLetter(PersonnelFunction.ToPartnerDayEntryCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.ToPartnerDayLeaveCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                UserEntryHandle.SetPersonnelAuthNoFactory(expable, model, user.CompanyType);
                expable.And(it => SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave);
                var exp = expable.ToExpression();
                var count = await entryStatisticsService.Count(exp);
                counts.Add(LowerFirstLetter(PersonnelFunction.ToPartnerDayLeaveCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.UserCount))
            {
                var count = await userStatisticsService.UserCount(it => 1>0);
                counts.Add(LowerFirstLetter(PersonnelFunction.UserCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.CustomerCount))
            {
                var count = await customerStatisticsService.Count(it => it.CompanyID == user.CompanyID);
                counts.Add(LowerFirstLetter(PersonnelFunction.CustomerCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.StarServiceCount))
            {
                var count = await partnerStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Ordinary);
                counts.Add(LowerFirstLetter(PersonnelFunction.StarServiceCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.HirePartnerCount))
            {
                var count = await partnerStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Hire);
                counts.Add(LowerFirstLetter(PersonnelFunction.HirePartnerCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.HirePartnerUserCount))
            {
                var count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Hire && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                counts.Add(LowerFirstLetter(PersonnelFunction.HirePartnerUserCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.SelfUserCount))
            {
                var count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Self && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                counts.Add(LowerFirstLetter(PersonnelFunction.SelfUserCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.JointUserCount))
            {
                var ops = new int[] { (int)OperateModel.Joint,(int)OperateModel.PlatformJoint};
                var count = await entryStatisticsService.Count(it => ops.Contains(it.OperateModel.Value) && it.Status == (int)UserStatus.Entry && it.CompanyID == user.CompanyID && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                counts.Add(LowerFirstLetter(PersonnelFunction.JointUserCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.StarServiceUserCount))
            {
                var count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Ordinary && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                counts.Add(LowerFirstLetter(PersonnelFunction.StarServiceUserCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.LaborrelationUserCount))
            {
                var count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.BusinessModel == (int)BusinessModel.Agent && !it.CustomerName.Contains("顺丰"));
                counts.Add(LowerFirstLetter(PersonnelFunction.LaborrelationUserCount.ToString()), count+1157);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.BatchUserCount))
            {
                var count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.CustomerPartnerID==164 && it.Status == (int)UserStatus.Entry);
                counts.Add(LowerFirstLetter(PersonnelFunction.BatchUserCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.HatchServiceCount))
            {
                var count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Hatch && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                counts.Add(LowerFirstLetter(PersonnelFunction.HatchServiceCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.UnionUserCount))
            {
                var count = await entryStatisticsService.Count(it => it.CompanyID != user.CompanyID && it.Status == (int)UserStatus.Entry);
                counts.Add(LowerFirstLetter(PersonnelFunction.UnionUserCount.ToString()), count);
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.AllServiceUserCount))
            {
                //var count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.BusinessModel == (int)BusinessModel.Labor && it.PartnerType<(int)PartnerType.Other);
                var count = 0;
                var auths = new string[] { "starServiceUserCount", "selfUserCount", "hirePartnerUserCount", "laborrelationUserCount", "unionUserCount" };
                foreach (var auth in auths) {
                    if (counts.ContainsKey(auth))
                    {
                        count += (int)counts[auth];
                    }
                }
                counts.Add(LowerFirstLetter(PersonnelFunction.AllServiceUserCount.ToString()), count);
            }
            #region 工资统计
            // 现金签收
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.CashSignCount))
            {
                var expable = Expressionable.Create<Model.Live_V_SalaryCashSendRecord>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_SalaryCashSendRecord>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryCashSendRecord>(model));
                expable.And(it => it.UserEntryID != null && it.Status == CashSendStatusEnumType.待签署);
                var count = await salaryStatisticsService.SalaryCashCount(expable.ToExpression());
                counts.Add(LowerFirstLetter(PersonnelFunction.CashSignCount.ToString()), count);
            }
            // 工资已发放，待服务商转账给员工记录
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.CashAgentTransCount))
            {
                var expable = Expressionable.Create<Model.Live_V_SalaryCashSendRecord>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_SalaryCashSendRecord>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryCashSendRecord>(model));
                expable.And(it => it.Status >= CashSendStatusEnumType.已签署 && it.AuditStatus == 2 && it.AgentStatus == 1 && it.SalaryRecordStatus == 5);
                var statusCount = await salaryStatisticsService.SalaryCashCount(expable.ToExpression());
                counts.Add(LowerFirstLetter(PersonnelFunction.CashAgentTransCount.ToString()), statusCount);
            }

            // 异常工资
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.WagesErrorCount))
            {
                var expable = Expressionable.Create<Model.Live_V_SalaryFailRecord>();
                expable.And(it => it.UserEntryID != null && it.Status == SalaryFailRecordStatusEnumType.待处理);
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_SalaryFailRecord>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryFailRecord>(model));
                var count = await salaryStatisticsService.SalaryFailCount(expable.ToExpression());
                counts.Add(LowerFirstLetter(PersonnelFunction.WagesErrorCount.ToString()), count);
            }
            // 暂缓统计
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.SalayDelayCount))
            {
                var expable = Expressionable.Create<Model.Live_V_SalaryDelaySendRecord>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_SalaryDelaySendRecord>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_SalaryDelaySendRecord>(model));
                expable.And(it => it.UserEntryID != null && it.Status != SalaryDelaySendStatusEnumType.已处理);
                var count = await salaryStatisticsService.ListSalaryDelayCount(expable.ToExpression());
                counts.Add(LowerFirstLetter(PersonnelFunction.SalayDelayCount.ToString()), count);
            }

            // 预发工资现金签收单
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.PerpaidCashCount))
            {
                var expable = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_PrepaidApplyRecord>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_PrepaidApplyRecord>(model));
                expable.And(it =>it.Status == PrepaidRecordStatusEnumType.待发放
                && it.CashStatus == PrepaidCashStatusEnumType.待签署);
                var count = await prepaidApplyRecordService.PrepaidApplyRecordList(expable.ToExpression());
                counts.Add(LowerFirstLetter(PersonnelFunction.PerpaidCashCount.ToString()), count.Count);
            }
            #endregion


            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.NoBankCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                expable.And(it=>string.IsNullOrEmpty(it.BankNumber) && it.Status== (int)UserStatus.Entry && it.IsUser);
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
                var exp = expable.ToExpression();
                var count = await entryStatisticsService.Count(exp);
                counts.Add(LowerFirstLetter(PersonnelFunction.NoBankCount.ToString()), count);
            }

            //工伤人数
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.InjuryCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                expable.And(it => it.InjuryStatus == 1);
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
                var exp = expable.ToExpression();
                var count = await entryStatisticsService.Count(exp);
                counts.Add(LowerFirstLetter(PersonnelFunction.InjuryCount.ToString()), count);
            }

            #region 系统通知
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.NoticeCount))
            {
                int notice = 0;
                var expable_notice = Expressionable.Create<Model.Live_Notification>();
                expable_notice.And(it => it.Status == 1);
                expable_notice.And(t=>t.PartnerID==model.PartnerID);
                var exp = expable_notice.ToExpression();
                notice = await notificationStatisticService.InfoCount(exp);
                counts.Add(LowerFirstLetter(PersonnelFunction.NoticeCount.ToString()), notice);
                //var expable_jobvedio = Expressionable.Create<Model.Live_V_JobVideo>();
                //expable_jobvedio.And(it => it.Status == 3);
                //expable_jobvedio.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_JobVideo>(model, user.CompanyType));
                //expable_jobvedio.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_JobVideo>(model));
                //var exp_jobvedio = expable_jobvedio.ToExpression();
                //var count1 = await injuryStatisticService.InfoCount(exp_injury);




            }
            //var iExpable = Expressionable.Create<Model.Live_V_InjuryInfo>();
            //injuryExpable.And(it => it.ProcessStatus == 2);
            //if (isParty)
            //{
            //    injuryExpable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            //}
            //else if (companyInfo.CutType != CompanyType.Self.ToString().ToLower())
            //{
            //    injuryExpable.And(it => it.CompanyID == user.CompanyID);
            //}
            //injuryExpable.AndIF(partnerID > 0, it => it.PartnerID == partnerID);
            //var injury_fail = await injuryStatisticService.InfoCount(injuryExpable.ToExpression());
            //counts.Add("noticeCount", injury_fail);
            #endregion


            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.WaitEntrySignCount) || funItems.Exists(it => it.PFID == (int)PersonnelFunction.WaitLeaveSignCount)|| funItems.Exists(it => it.PFID == (int)PersonnelFunction.WaitEntrySignTimeoutCount))
            {
                var expable = Expressionable.Create<Model.Live_V_ContractRecord>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_ContractRecord>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_ContractRecord>(model));
                var dt = DateTime.Today.AddDays(-3);

                expable.And(it => it.Status == (int)SignStatusEnumType.待签署);
                expable.And(it => (it.UserEntryStatus == 2 && it.ContractType == ContractEnumType.入职合同) || (it.UserEntryStatus==4&& it.ContractType == ContractEnumType.离职合同&& it.LeaveDate!=null&& it.LeaveDate.Value>= dt));
                var statusCount = await contractStatisticsService.ListEntryContract(expable.ToExpression());
                if (statusCount.Count > 0)
                {
                    var entryData = statusCount.Where(it => it.ContractType == ContractEnumType.入职合同&&it.EntryDate.Value>DateTime.Now.AddDays(-30)).ToList();
                    if (entryData != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.WaitEntrySignCount.ToString()), entryData.Count);
                    }
                    var LeaveData = statusCount.Where(it => it.ContractType == ContractEnumType.离职合同).ToList();
                    if (LeaveData != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.WaitLeaveSignCount.ToString()), LeaveData.Count);
                    }

                    var entryTimeoutData = statusCount.Where(it => it.ContractType == ContractEnumType.入职合同 && it.EntryDate.Value <= DateTime.Now.AddDays(-30)).ToList();
                    if (entryData != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.WaitEntrySignTimeoutCount.ToString()), entryTimeoutData.Count);
                    }

                }
            }

           

            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.EntrySignCount) || funItems.Exists(it => it.PFID == (int)PersonnelFunction.LeaveSignCount))
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));

                expable.And(it => it.Status == (int)ContractStatus.EntryComplete || it.Status>=(int)ContractStatus.LeaveWaitCreate);
                var statusCount = await entryStatisticsService.ContractStatusCount(expable.ToExpression());
                if (statusCount.Count > 0)
                {
                    var entryData = statusCount.Where(it => it.Status != (int)ContractStatus.LeaveComplete).ToList();
                    if (entryData != null)
                    {
                        var count = 0;
                        foreach (var item in entryData)
                        {
                            count += item.Count;
                        }
                        counts.Add(LowerFirstLetter(PersonnelFunction.EntrySignCount.ToString()), count);
                    }
                    var LeaveData = statusCount.Where(it => it.Status == (int)ContractStatus.LeaveComplete).ToList();
                    if (LeaveData != null)
                    {
                        var count = 0;
                        foreach (var item in LeaveData)
                        {
                            count += item.Count;
                        }
                        counts.Add(LowerFirstLetter(PersonnelFunction.LeaveSignCount.ToString()), count);
                    }
                }
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.DeadHorseCount) || funItems.Exists(it => it.PFID == (int)PersonnelFunction.DayWagesCount))
            {
                var expable = Expressionable.Create<Model.Live_V_PrepaidApplyRecord>();
                expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_PrepaidApplyRecord>(model, user.CompanyType));
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_PrepaidApplyRecord>(model));
                var status = new PrepaidRecordStatusEnumType[] {  PrepaidRecordStatusEnumType.待审核, PrepaidRecordStatusEnumType.待发放 };
                //expable.And(it => status.Contains(it.Status));
                if (model.UserType == (int)PersonnelType.Daywork)
                {
                    expable.And(it => (it.ApplyType == 1 && it.UserIDS.Contains(model.UserGID.Value.ToString()) && it.Status == PrepaidRecordStatusEnumType.待审核) || (it.ApplyType != 1 && status.Contains(it.Status)));
                }
                else if(model.UserType == (int)PersonnelType.DayworkManage)
                {
                    expable.And(it => (it.ApplyType == 1 && it.Status == PrepaidRecordStatusEnumType.待审核) || (it.ApplyType != 1 && status.Contains(it.Status)));
                }
                else
                {
                    expable.And(it => status.Contains(it.Status));
                }

                var exp = expable.ToExpression();
                var statusCount = await prepaidApplyStatisticsService.TypeCount(exp);
                if (statusCount.Count > 0)
                {
                    var data = statusCount.Find(it => it.Status == (int)PrepaidApplyEnumType.预发工资申请);
                    if (data != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.DeadHorseCount.ToString()), data.Count);
                    }
                    var sdata = statusCount.Find(it => it.Status == (int)PrepaidApplyEnumType.日结工资申请 );
                    if (sdata != null)
                    {
                        counts.Add(LowerFirstLetter(PersonnelFunction.DayWagesCount.ToString()), sdata.Count);
                    }
                }
            }
            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.AttendancePrepaidApply))
            {
                var expable = Expressionable.Create<Model.Live_V_AttendancePrepaidApplyCustomer>();
                expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any());
                expable.And(t => t.Status == (int)AttendancePrepaidApplyCustomerStatusEnum.待审核);
                var statusCount = await attendanceStatistics.AttendancePrepaidApplyCustomerCount(expable.ToExpression());
                counts.Add(LowerFirstLetter(PersonnelFunction.AttendancePrepaidApply.ToString()), statusCount);
            }

            if (funItems.Exists(it => it.PFID == (int)PersonnelFunction.AttendanceApply))
            {
                var expable = Expressionable.Create<Model.Live_V_AttendanceApply>();
                expable.And(t => t.ManageGIDS.Contains(model.UserGID.ToString()));
                expable.And(t => t.Status == 1);
                var applyCount = await attendanceStatistics.AttendanceApplyCount(expable.ToExpression());
                counts.Add(LowerFirstLetter(PersonnelFunction.AttendanceApply.ToString()), applyCount);
            }

            if (companyInfo.CutType == Model.Enums.CompanyType.Self.ToString().ToLower())
            {
                var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                var currentYear = new DateTime(currentMonth.Year, 1, 1);
                var invoiceYearSumAmount = await invoiceService.SumAmount(f => f.IssuedTime.HasValue && f.IssuedTime.Value >= currentYear);
                counts.Add("invoiceYearSumAmount", invoiceYearSumAmount + 200000000);
            }
           
            return Ok(counts);
        }
        static string LowerFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToLower(input[0], CultureInfo.InvariantCulture) + input.Substring(1);
        }
        [HttpGet]
        public async Task<IActionResult> PartnerUserList(Guid? cityPartnerID, int type=0)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry);
            if (type != (int)PartnerType.Labor)
            {
                expable.And(it=> UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            }
            expable.AndIF(type>0,it=>it.PartnerType==type);
            expable.AndIF(cityPartnerID.HasValue, it => it.PerCityPartnerID == cityPartnerID);
            var exp = expable.ToExpression();
            var items = await entryStatisticsService.PartnerList(exp);
            var subExpable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == user.CompanyID && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            if (type != (int)PartnerType.Labor)
            {
                subExpable.And(it => UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            }
            subExpable.AndIF(type > 0, it => it.PartnerType == type);
            var subExp = subExpable.ToExpression();
            var statuss = await entryStatisticsService.PartnerStatusList(subExp);
            foreach(var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry!=null)
                {
                    item.ToDayEntryCount=entry.Count;
                }
                var leave = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            if (type == (int)PartnerType.Labor)
            {
                var laModel = new Model.Statistics.EntryModel();
                laModel.ID = 11111111;
                laModel.OnTheJobCount = 1157;
                items.Add(laModel);
            }
            return Ok(items);
        }
        [HttpGet]
        public async Task<IActionResult> PartnerJointUserList()
        {
            var user = CurrentUser;
            var items = await entryStatisticsService.PartnerJointList(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.OperateModel == (int)OperateModel.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            var statuss = await entryStatisticsService.PartnerJointStatusList(it => it.CompanyID == user.CompanyID && it.OperateModel == (int)OperateModel.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value) && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            foreach (var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            var myItems = await entryStatisticsService.PartnerList(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.PartnerType==(int)PartnerType.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            var myStatuss = await entryStatisticsService.PartnerStatusList(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value) && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            foreach (var item in myItems)
            {
                item.OnTheJobCount = item.Count;
                var entry = myStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = myStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            return Ok(new { joint = items, self = myItems });
        }
        [HttpGet]
        public async Task<IActionResult> FromTypeList(int customerId)
        {
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var expable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry);
            expable.And(it => it.CustomerID == customerId);
            expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            var exp = expable.ToExpression();
            var items = await entryStatisticsService.FromTypeList(exp);
            var myStatuss = await entryStatisticsService.FromTypeStatusList(it => it.CompanyID == user.CompanyID && it.CustomerID == customerId && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            foreach (var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = myStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = myStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            return Ok(items);
        }
        [HttpGet]
        public async Task<IActionResult> CustomerUserList(string from, string ids,int partner=0)
        {
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var expable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry);
            var subExpable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == user.CompanyID && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            if (!string.IsNullOrEmpty(ids))
            {
                var idList = new List<int>();
                var idItems = ids.Split(',');
                foreach (var idItem in idItems)
                {
                    if (!string.IsNullOrEmpty(idItem)) idList.Add(Convert.ToInt32(idItem));
                }
                expable.And(it => idList.Contains(it.CustomerID.Value));
                subExpable.And(it => idList.Contains(it.CustomerID.Value));
                if(from== "partner" && partner>0)
                {
                    expable.And(it => it.PartnerID== partner);
                    subExpable.And(it => it.PartnerID == partner);
                }
            }
            else
            {
                if (from == "my")
                {
                    expable.And(it => SqlFunc.Subqueryable<Model.Live_PartnerCustomer>().Where(s => s.CID == it.CustomerID && s.PID == model.PartnerID).Any());
                    subExpable.And(it => SqlFunc.Subqueryable<Model.Live_PartnerCustomer>().Where(s => s.CID == it.CustomerID && s.PID == model.PartnerID).Any());
                }
                else
                {
                    expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == model.UserID).Any());
                    subExpable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == model.UserID).Any());
                }
            }
            expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            subExpable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            var exp = expable.ToExpression();
            var items = await entryStatisticsService.CustomerList(exp);
            var subExp = subExpable.ToExpression();
            var statuss = await entryStatisticsService.CustomerStatusList(subExp);
            foreach (var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            return Ok(items);
        }
        [HttpGet]
        public async Task<IActionResult> CompanyUserList()
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_UserEntry>().And(it => SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any() && it.Status == (int)UserStatus.Entry).ToExpression();
            var items = await entryStatisticsService.CompanyList(exp);
            var subExp = Expressionable.Create<Model.Live_V_UserEntry>().And(it => SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any() && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave))).ToExpression();
            var statuss = await entryStatisticsService.CompanyStatusList(subExp);
            foreach (var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            return Ok(items);
        }
        [HttpGet]
        public async Task<IActionResult> SysCompanyUserList()
        {
            var user = CurrentUser;
            var exp = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID != user.CompanyID && it.Status == (int)UserStatus.Entry).ToExpression();
            var items = await entryStatisticsService.CompanyList(exp);
            var subExp = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID != user.CompanyID && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave))).ToExpression();
            var statuss = await entryStatisticsService.CompanyStatusList(subExp);
            foreach (var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = statuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            return Ok(items);
        }
        [HttpGet]
        public async Task<IActionResult> CustomerStatistics(int id)
        {
            var items = await entryStatisticsService.CustomerStatusList(it=>it.CustomerID==id);
            var counts = new Dictionary<UserStatus, decimal>();
            for (var i = 1; i < 17; i++)
            {
                counts.Add((UserStatus)i, 0);
            }
            foreach (var item in items) {
                if (item.Status.HasValue)
                {
                    counts[(UserStatus)item.Status] = item.Count;
                }
            }
            var viewCount= counts[UserStatus.Entry] + counts[UserStatus.Leave] + counts[UserStatus.Interviewed] + counts[UserStatus.WaitEntry] + counts[UserStatus.NotEntry];
            var viewAllCount = viewCount + counts[UserStatus.InterviewFailed];
            var viewProp= string.Format("{0:F2}", viewCount == 0 || viewAllCount == 0 ? 0 : (viewCount / viewAllCount) * 100);

            var entryCount = counts[UserStatus.Entry] + counts[UserStatus.Leave];
            var entryAllCount = entryCount + counts[UserStatus.NotEntry];
            var entryProp = string.Format("{0:F2}", entryCount == 0 || entryAllCount == 0 ? 0 : (entryCount / entryAllCount) * 100);

            var levelProp = string.Format("{0:F2}", entryCount == 0 || counts[UserStatus.Leave] == 0 ? 0 : (counts[UserStatus.Leave] / entryCount) * 100);

            return Ok(new { waitViewCount= counts[UserStatus.WaitInterview] , waitEntryCount = counts[UserStatus.WaitEntry],entryCount= counts[UserStatus.Entry], leaveCount = counts[UserStatus.Leave], viewProp, entryProp, levelProp });
        }
        [HttpGet]
        public async Task<IActionResult> PartnerTypeStatistics()
        {
            var user = CurrentUser;
            var items = await entryStatisticsService.PartnerTypeList(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.PartnerType!=(int)PartnerType.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            var statuss = await entryStatisticsService.PartnerTypeStatusList(it => it.CompanyID == user.CompanyID && it.PartnerType != (int)PartnerType.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value) && it.PartnerType != (int)PartnerType.Joint && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            ///联营统计
            //var jointItems = await entryStatisticsService.PartnerJointList(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.OperateModel == (int)OperateModel.Joint && it.BusinessModel == (int)BusinessModel.Labor);
            //var jointStatuss = await entryStatisticsService.PartnerJointStatusList(it => it.CompanyID == user.CompanyID && it.OperateModel == (int)OperateModel.Joint && it.BusinessModel == (int)BusinessModel.Labor && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));

            foreach (var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = statuss.Find(it => it.PartnerType == item.PartnerType && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = statuss.Find(it => it.PartnerType == item.PartnerType && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            //var jointCounts = new Model.Statistics.EntryModel();
            //jointCounts.OnTheJobCount = 0;
            //jointCounts.ToDayEntryCount = 0;
            //jointCounts.ToDayLeaveCount = 0;
            //jointCounts.PartnerType = (int)Model.Enums.PartnerType.Joint;
            //foreach (var item in jointItems)
            //{
            //    jointCounts.OnTheJobCount += item.Count;
            //    var entry = jointStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
            //    if (entry != null)
            //    {
            //        jointCounts.ToDayEntryCount += entry.Count;
            //    }
            //    var leave = jointStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
            //    if (leave != null)
            //    {
            //        jointCounts.ToDayLeaveCount += leave.Count;
            //    }
            //}
            //items.Add(jointCounts);
            return Ok(items);
        }
        [HttpGet]
        public async Task<IActionResult> PartnerTypeStatisticsold()
        {
            var user = CurrentUser;
            var items = await entryStatisticsService.PartnerTypeList(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && (it.OperateModel == (int)OperateModel.Routine || it.OperateModel == (int)OperateModel.Common) && it.PartnerType != (int)PartnerType.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            var statuss = await entryStatisticsService.PartnerTypeStatusList(it => it.CompanyID == user.CompanyID && it.PartnerType != (int)PartnerType.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value) && it.PartnerType != (int)PartnerType.Joint && (it.OperateModel == (int)OperateModel.Routine || it.OperateModel == (int)OperateModel.Common) && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            ///联营统计
            var jointItems = await entryStatisticsService.PartnerJointList(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.OperateModel == (int)OperateModel.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
            var jointStatuss = await entryStatisticsService.PartnerJointStatusList(it => it.CompanyID == user.CompanyID && it.OperateModel == (int)OperateModel.Joint && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value) && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));

            foreach (var item in items)
            {
                item.OnTheJobCount = item.Count;
                var entry = statuss.Find(it => it.PartnerType == item.PartnerType && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    item.ToDayEntryCount = entry.Count;
                }
                var leave = statuss.Find(it => it.PartnerType == item.PartnerType && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    item.ToDayLeaveCount = leave.Count;
                }
            }
            var jointCounts = new Model.Statistics.EntryModel();
            jointCounts.OnTheJobCount = 0;
            jointCounts.ToDayEntryCount = 0;
            jointCounts.ToDayLeaveCount = 0;
            jointCounts.PartnerType = (int)Model.Enums.PartnerType.Joint;
            foreach (var item in jointItems)
            {
                jointCounts.OnTheJobCount += item.Count;
                var entry = jointStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Entry);
                if (entry != null)
                {
                    jointCounts.ToDayEntryCount += entry.Count;
                }
                var leave = jointStatuss.Find(it => it.ID == item.ID && it.Status == (int)UserStatus.Leave);
                if (leave != null)
                {
                    jointCounts.ToDayLeaveCount += leave.Count;
                }
            }
            items.Add(jointCounts);
            return Ok(items);
        }

        #region 工资统计
        [HttpGet]
        public async Task<IActionResult> SalaryCashList()
        {

            var user = CurrentUser;
            var company = await companyConfigService.Get(user.CompanyGID);
            var companyInfo = company.Company;
            var model = await authService.UserInfo<Model.Live_Personnel>();
         
                var records = await salaryStatisticsService.ListSalaryCashRecord(it =>
                it.CompanyID == user.CompanyID && it.UserEntryPartnerType != null
                && it.UserEntryPartnerType < (int)PartnerType.Other
                && it.UserEntryPartnerID != null
                && model.PartnerIDS.Contains(it.UserEntryPartnerID.ToString())
                && it.Status ==  CashSendStatusEnumType.待签署);

            return Ok(records);
        }
        [HttpGet]
        public async Task<IActionResult> SalaryFailList()
        {
            var user = CurrentUser;
            var company = await companyConfigService.Get(user.CompanyGID);
            var companyInfo = company.Company;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var records = await salaryStatisticsService.ListSalaryFailRecord(it =>
            it.CompanyID == user.CompanyID && it.UserEntryPartnerType != null
            && it.UserEntryPartnerType < (int)PartnerType.Other
            && it.UserEntryPartnerID != null
            && model.PartnerIDS.Contains(it.UserEntryPartnerID.ToString())
            && it.Status == SalaryFailRecordStatusEnumType.待处理);
            return Ok(records);
        }
        #endregion
    }
}
