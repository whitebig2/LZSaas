using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Statistics.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Globalization;

namespace LZ.Statistics.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class AIStatisticsController(
        IEntryStatisticsService entryStatisticsService,IAuthService authService
        ) : StatisticsControllerBase
    {
        public int[] businessModels = { (int)BusinessModel.Labor, (int)BusinessModel.Epiboly };
        [HttpGet]
        public async Task<IActionResult> IndexStatistics()
        {
            var counts = new Dictionary<string, object>();

            var count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.PartnerType == (int)PartnerType.Ordinary && it.Status == (int)UserStatus.Entry && businessModels.Contains(it.BusinessModel.Value));
            counts.Add(LowerFirstLetter(PersonnelFunction.StarServiceUserCount.ToString()), count);

            count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.Status == (int)UserStatus.Entry && it.BusinessModel == (int)BusinessModel.Agent && !it.CustomerName.Contains("顺丰"));
            counts.Add(LowerFirstLetter(PersonnelFunction.LaborrelationUserCount.ToString()), count + 1157);

            count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.PartnerType == (int)PartnerType.Self && it.Status == (int)UserStatus.Entry && businessModels.Contains(it.BusinessModel.Value));
            counts.Add(LowerFirstLetter(PersonnelFunction.SelfUserCount.ToString()), count);

            count = await entryStatisticsService.Count(it => it.CompanyID == 1 && it.PartnerType == (int)PartnerType.Hire && it.Status == (int)UserStatus.Entry && businessModels.Contains(it.BusinessModel.Value));
            counts.Add(LowerFirstLetter(PersonnelFunction.HirePartnerUserCount.ToString()), count);

            count = await entryStatisticsService.Count(it => it.CompanyID != 1 && it.Status == (int)UserStatus.Entry);
            counts.Add(LowerFirstLetter(PersonnelFunction.UnionUserCount.ToString()), count);

            count = 0;
            var auths = new string[] { "starServiceUserCount", "selfUserCount", "hirePartnerUserCount", "laborrelationUserCount", "unionUserCount" };
            foreach (var auth in auths)
            {
                if (counts.ContainsKey(auth))
                {
                    count += (int)counts[auth];
                }
            }
            counts.Add(LowerFirstLetter(PersonnelFunction.AllServiceUserCount.ToString()), count);

            return Ok(counts);
        }
        static string LowerFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToLower(input[0], CultureInfo.InvariantCulture) + input.Substring(1);
        }
        [HttpGet]
        public async Task<IActionResult> PartnerUsers(string name)
        {
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            if (user.CompanyType == CompanyType.Self.ToString().ToLower() && model.UserType == (int)PersonnelType.Manage)
            {
                var expable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.PartnerName.Contains(name) && it.Status == (int)UserStatus.Entry);
                expable.And(it => UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                var exp = expable.ToExpression();
                var items = await entryStatisticsService.PartnerList(exp);
                var subExpable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.PartnerName.Contains(name) && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
                subExpable.And(it => UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                var subExp = subExpable.ToExpression();
                var statuss = await entryStatisticsService.PartnerStatusList(subExp);
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
            else
            {
                throw new ErrorCodeException(-1, "您无权查看服务商的在职信息！");
            }
                
        }
        [HttpGet]
        public async Task<IActionResult> MyUser()
        {
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var counts = new Dictionary<string, object>();
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            expable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
            expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            expable.And(it => SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && (it.Status == (int)UserStatus.Entry || it.Status == (int)UserStatus.Leave));
            var exp = expable.ToExpression();
            var count = await entryStatisticsService.Count(exp);
            counts.Add("toDayEntryCount", count);

           var leveExpable = Expressionable.Create<Model.Live_V_UserEntry>();
            leveExpable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
            leveExpable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            leveExpable.And(it => SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave);
            count = await entryStatisticsService.Count(leveExpable.ToExpression());
            counts.Add("toDayLeaveCount", count);

            var statusExpable = Expressionable.Create<Model.Live_V_UserEntry>();
            statusExpable.And(UserEntryHandle.SetPersonnelActionStatistics<Model.Live_V_UserEntry>(model, user.CompanyType));
            statusExpable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
            statusExpable.And(it => statuss.Contains(it.Status.Value));
            var statusCount = await entryStatisticsService.StatusCount(statusExpable.ToExpression());
            if (statusCount.Count > 0)
            {
                var data = statusCount.Find(it => it.Status == (int)UserStatus.Entry);
                if (data != null)
                {
                    counts.Add("onTheJobCount", data.Count);
                }
                data = statusCount.Find(it => it.Status == (int)UserStatus.WaitEntry);
                if (data != null)
                {
                    counts.Add("waitEntryCount", data.Count);
                }
            }
            if (user.CompanyType==CompanyType.Self.ToString().ToLower() && model.UserType==(int)PersonnelType.Manage)
            {
                var allcount = 0;
                count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Ordinary && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                allcount += count;
                counts.Add(LowerFirstLetter(PersonnelFunction.StarServiceUserCount.ToString()), count);
                count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Self && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                allcount += count;
                counts.Add(LowerFirstLetter(PersonnelFunction.SelfUserCount.ToString()), count);
                count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.PartnerType == (int)PartnerType.Hire && it.Status == (int)UserStatus.Entry && UserEntryHandle.businessModels.Contains(it.BusinessModel.Value));
                allcount += count;
                counts.Add(LowerFirstLetter(PersonnelFunction.HirePartnerUserCount.ToString()), count);
                count = await entryStatisticsService.Count(it => it.CompanyID == user.CompanyID && it.Status == (int)UserStatus.Entry && it.BusinessModel == (int)BusinessModel.Agent && !it.CustomerName.Contains("顺丰"));
                allcount += count+ 1157;
                counts.Add(LowerFirstLetter(PersonnelFunction.LaborrelationUserCount.ToString()), count + 1157);
                count = await entryStatisticsService.Count(it => it.CompanyID != user.CompanyID && it.Status == (int)UserStatus.Entry);
                allcount += count;
                counts.Add(LowerFirstLetter(PersonnelFunction.UnionUserCount.ToString()), count);
                counts["onTheJobCount"] =allcount;
            }
            return Ok(counts);
        }
        [HttpGet]
        public async Task<IActionResult> CustomerUsers(string name,string pname="")
        {
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var expable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == 1 && it.CustomerName.Contains(name) && it.Status == (int)UserStatus.Entry);
            expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            var subExpable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == 1 && it.CustomerName.Contains(name) && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            subExpable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            var exp = expable.ToExpression();
            if (model.UserType == (int)PersonnelType.Manage && !string.IsNullOrEmpty(pname))
            {
                expable.And(it => it.PartnerName.Contains(pname));
                subExpable.And(it => it.PartnerName.Contains(pname));
            }
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
        public async Task<IActionResult> CustomerUsersNew(string name, string pname = "")
        {
            var user = CurrentUser;
            var model = await authService.UserInfo<Model.Live_Personnel>();
            var expable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == user.CompanyID && it.CustomerName.Contains(name) && it.Status == (int)UserStatus.Entry);
            expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));
            
            var subExpable = Expressionable.Create<Model.Live_V_UserEntry>().And(it => it.CompanyID == user.CompanyID && it.CustomerName.Contains(name) && ((SqlFunc.DateIsSame(it.EntryTime, DateTime.Now) && it.Status == (int)UserStatus.Entry) || (SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now) && it.Status == (int)UserStatus.Leave)));
            subExpable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(model));

            if (model.UserType == (int)PersonnelType.Manage && !string.IsNullOrEmpty(pname))
            {
                expable.And(it => it.PartnerName.Contains(pname));
                subExpable.And(it => it.PartnerName.Contains(pname));
            }
            var exp = expable.ToExpression();
            var cModel= new Model.Statistics.EntryModel() { Name = name };
            cModel.OnTheJobCount = await entryStatisticsService.Count(exp);
            var subExp = subExpable.ToExpression();
            var statuss = await entryStatisticsService.StatusCount(subExp);
            var entry = statuss.Find(it => it.Status == (int)UserStatus.Entry);
            if (entry != null)
            {
                cModel.ToDayEntryCount = entry.Count;
            }
            var leave = statuss.Find(it => it.Status == (int)UserStatus.Leave);
            if (leave != null)
            {
                cModel.ToDayLeaveCount = leave.Count;
            }
            return Ok(cModel);
        }
    }
}
