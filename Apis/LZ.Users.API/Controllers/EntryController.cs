using Consul;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.EventData.Insurance;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using LZ.Users.API.CommonHelp;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System.Data;
using System.Text;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    public class EntryController(IEntryService entryService,IAuthService authService,ICompanyConfigService companyConfigService, IEventPublisher publisher,IUserService userService, IBlackListService blackListService,IInjuryInfoService injuryInfoService,IHttpService httpService, IPermissionService permissionService,IPersonnelService personnelService,IPersonnelItemsService personnelItemsService,IRedisService redisService,IBusinessInsuranceRecordService businessInsuranceRecordService, INoteService noteService, IJDYSyncService jdySyncService,IBusInsuranceCompanyService busInsuranceCompanyService,IBusInsuranceWorkPlanService busInsuranceWorkPlanService,IAttendanceCalendarService attendanceCalendarService) : UsersControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            await SetSearchWhere(expable, query);
             var exp = expable.ToExpression();
            var models = await entryService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Roster([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            SetRosterSearchWhere(expable, query);
            var exp = expable.ToExpression();
            query.Page = 0;
            var models = await entryService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        [OperationLogAction("导出花名册")]
        public async Task<IActionResult> RosterExportExcel([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            SetRosterSearchWhere(expable, query);
            var exp = expable.ToExpression();
            query.Page = 0;
            var models = await entryService.List(exp, query);
            var list = new List<Model.RosterModel>();
            var customers=new Dictionary<int, string>();
            var partners = new Dictionary<int, string>();
            var signingUnits = new Dictionary<Guid, string>();
            var nameTitle = "";
            var fileName = "";
            if (query.SummaryType==1)
            {
                nameTitle = "客户名称";
                foreach (var item in models)
                {
                    if (item.CustomerID.HasValue && !customers.ContainsKey(item.CustomerID.Value))
                    {
                        customers.Add(item.CustomerID.Value,item.CustomerName);
                    }
                }
                foreach (var customer in customers)
                {
                    var items = models.Where(it => it.CustomerID== customer.Key);
                    if (items.Count() > 0)
                    {
                        var roster = new Model.RosterModel();
                        roster.ID = customer.Key;
                        roster.Name = customer.Value;
                        roster.Items = items.ToList();
                        roster.EntryCount = 0;
                        roster.LeaveCount = 0;
                        foreach (var item in items)
                        {
                            if(item.Status==2 || (item.Status==4 && item.LeaveTime> query.EntryStartTime))
                            {
                                roster.EntryCount++;
                            }
                            else
                            {
                                roster.LeaveCount++;
                            }
                        }
                        list.Add(roster);
                    }
                }
            }
            else if (query.SummaryType == 2)
            {
                nameTitle = "服务商名称";
                foreach (var item in models)
                {
                    if (item.PartnerID.HasValue && !partners.ContainsKey(item.PartnerID.Value))
                    {
                        partners.Add(item.PartnerID.Value, item.PartnerName);
                    }
                }
                foreach (var partner in partners)
                {
                    var items = models.Where(it => it.PartnerID == partner.Key);
                    if (items.Count() > 0)
                    {
                        var roster = new Model.RosterModel();
                        roster.ID = partner.Key;
                        roster.Name = partner.Value;
                        roster.Items = items.ToList();
                        roster.EntryCount = 0;
                        roster.LeaveCount = 0;
                        foreach (var item in items)
                        {
                            if (item.Status == 2 || (item.Status == 4 && item.LeaveTime > query.EntryStartTime))
                            {
                                roster.EntryCount++;
                            }
                            else
                            {
                                roster.LeaveCount++;
                            }
                        }
                        list.Add(roster);
                    }
                }
            }
            else if (query.SummaryType == 3)
            {
                nameTitle = "签章单位名称";
                foreach (var item in models)
                {
                    if (item.SigningUnitGID.HasValue && !signingUnits.ContainsKey(item.SigningUnitGID.Value))
                    {
                        signingUnits.Add(item.SigningUnitGID.Value, item.SignName);
                    }
                }
                foreach (var signingUnit in signingUnits)
                {
                    var items = models.Where(it => it.SigningUnitGID == signingUnit.Key);
                    if (items.Count() > 0)
                    {
                        var roster = new Model.RosterModel();
                        roster.GID = signingUnit.Key;
                        roster.Name = signingUnit.Value;
                        roster.Items = items.ToList();
                        roster.EntryCount = 0;
                        roster.LeaveCount = 0;
                        foreach (var item in items)
                        {
                            if (item.Status == 2 || (item.Status == 4 && item.LeaveTime > query.EntryStartTime))
                            {
                                roster.EntryCount++;
                            }
                            else
                            {
                                roster.LeaveCount++;
                            }
                        }
                        list.Add(roster);
                    }
                }
            }
            if (list.Count > 0)
            {
                
                if (!string.IsNullOrEmpty(query.CustomerIDS))
                {
                    var ids = query.CustomerIDS.Split(',');
                    foreach (var id in ids)
                    {
                        var data = models.Find(it=>it.CustomerID== Convert.ToInt32(id));
                        if (data != null)
                        {
                            fileName = data.CustomerName+(ids.Length>1? "等多个客户的" : "");
                            break;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(query.SigningUnitIDS))
                {
                    var ids = query.SigningUnitIDS.Split(',');
                    foreach (var id in ids)
                    {
                        var data = models.Find(it => it.SigningUnitGID == new Guid(id));
                        if (data != null)
                        {
                            fileName = data.SignName + (ids.Length > 1 ? "等多个签章单位的" : "");
                            break;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(query.PartnerIDS))
                {
                    var ids = query.PartnerIDS.Split(',');
                    foreach (var id in ids)
                    {
                        var data = models.Find(it => it.PartnerID == Convert.ToInt32(id));
                        if (data != null)
                        {
                            fileName = data.PartnerName + (ids.Length > 1 ? "等多个服务商的" : "");
                            break;
                        }
                    }
                }
                string strTemp = "<td>{0}</td>", strTemp2 = "<td style=\"vnd.ms-excel.numberformat:@\">{0}</td>";
                var customerTitle = $"<tr><th>{nameTitle}</th><th>在职人数</th><th>离职人数</th><th colspan=\"9\"></th></tr>";
                var dataTitle = "<tr><th>姓名</th><th>年龄</th><th>性别</th><th>所属客户</th><th>所属服务商</th><th>招聘专员</th><th>手机号</th><th>身份证号码</th><th>工种</th><th>工种类型</th><th>招聘渠道</th><th>入职时间</th><th>离职时间</th><th>签章单位</th><th>系统状态</th><th>民族</th><th>婚姻状况</th><th>学历</th><th>出生年月</th><th>居住地址</th><th>户籍地址</th><th>紧急人姓名</th><th>紧急号码</th><th>保险状态</th><th>保险品种</th><th>开户行</th><th>银行卡号</th><th>银行卡所属省份</th><th>是否实名</th><th>备注</th><th>新增时间</th></tr>";
                StringBuilder htmls = new StringBuilder();
                htmls.Append("<table width=\"1000\" border=\"1\" cellspacing=\"0\" style=\"text-align:center;\" cellpadding=\"0\">");
                foreach(var m in list)
                {
                    htmls.Append(customerTitle);
                    htmls.Append("<tr>");
                    htmls.AppendFormat(strTemp,m.Name);
                    htmls.AppendFormat(strTemp, m.EntryCount);
                    htmls.AppendFormat(strTemp, m.LeaveCount);
                    htmls.Append("<td colspan=\"9\"></td>");
                    htmls.Append("</tr>");
                    htmls.Append(dataTitle);
                    foreach (var entry in m.Items)
                    {
                        htmls.Append("<tr>");
                        htmls.AppendFormat(strTemp, entry.Name);
                        htmls.AppendFormat(strTemp, entry.Age);
                        htmls.AppendFormat(strTemp, entry.Sex);
                        htmls.AppendFormat(strTemp, entry.CustomerName);
                        htmls.AppendFormat(strTemp, entry.PartnerName);
                        htmls.AppendFormat(strTemp, entry.PersonnelName);
                        htmls.AppendFormat(strTemp2, entry.Phone);
                        htmls.AppendFormat(strTemp2, entry.CardID);
                        htmls.AppendFormat(strTemp, entry.WorkName);
                        htmls.AppendFormat(strTemp, entry.WorkClass);
                        htmls.AppendFormat(strTemp, entry.FromType);
                        htmls.AppendFormat(strTemp, entry.EntryTime.HasValue ? entry.EntryTime.Value.ToString("yyyy-MM-dd") : "");
                        htmls.AppendFormat(strTemp, entry.LeaveTime.HasValue ? entry.LeaveTime.Value.ToString("yyyy-MM-dd") : "");
                        htmls.AppendFormat(strTemp, entry.SignName);
                        htmls.AppendFormat(strTemp, entry.Status == 2 || (entry.Status == 4 && entry.LeaveTime > query.EntryStartTime) ? "在职" : "离职");
                        htmls.AppendFormat(strTemp, entry.Nation);
                        htmls.AppendFormat(strTemp, entry.MaritalStatus);
                        htmls.AppendFormat(strTemp, entry.Education);
                        htmls.AppendFormat(strTemp, entry.BirthDate.HasValue ? entry.BirthDate.Value.ToString("yyyy-MM-dd") : "");
                        htmls.AppendFormat(strTemp, entry.Address);
                        htmls.AppendFormat(strTemp, entry.IdcardAddress);
                        htmls.AppendFormat(strTemp, entry.EmergencyContact);
                        htmls.AppendFormat(strTemp, entry.EmergencyContactPhone);
                        htmls.AppendFormat(strTemp, entry.PlanStatus1.HasValue ? ((UserEntryPlanStatusEnumType)entry.PlanStatus1).ToString() : "");
                        htmls.AppendFormat(strTemp, entry.PlanName1);
                        htmls.AppendFormat(strTemp, entry.BankName);
                        htmls.AppendFormat(strTemp2, entry.BankNumber);
                        htmls.AppendFormat(strTemp, entry.BankProvince);
                        htmls.AppendFormat(strTemp, entry.IsReal ? "已实名认证" : "");
                        htmls.AppendFormat(strTemp, entry.Remarks);
                        htmls.AppendFormat(strTemp, entry.CreateTime.HasValue ? entry.CreateTime.Value.ToString("yyyy-MM-dd") : "");
                        htmls.Append("</tr>");
                    }
                }
                htmls.Append("</table>");
                byte[] arr = Encoding.UTF8.GetBytes(htmls.ToString());
                MemoryStream ms = new MemoryStream(arr);
                string base64String = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return Ok(new { base64String, fileName = query.EntryStartTime.Value.ToString("yyyy-MM-dd")+ fileName + "花名册", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            }
            else
            {
                throw new ErrorCodeException(-1, "无数据！");
            }
        }
        private void SetRosterSearchWhere(Expressionable<Live_V_UserEntry> expable, UserPagedModel query)
        {
            var user = CurrentUser;
            if (!query.EntryStartTime.HasValue)
            {
                query.EntryStartTime = DateTime.Now;
            }
            expable.And(it => it.EntryTime <= query.EntryStartTime.Value && ( it.Status == (int)UserStatus.Entry || (it.Status == (int)UserStatus.Leave && it.LeaveTime > query.EntryStartTime.Value)));
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
            }
            if (query.From == "spmini" || user.CompanyType == Model.Enums.CompanyType.Party.ToString().ToLower())//甲方公司可以看到授权人员
            {
                expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (!string.IsNullOrEmpty(query.PartnerIDS))
            {
                var ids = query.PartnerIDS.Split(',');
                var idList = new List<int>();
                foreach (var id in ids)
                {
                    idList.Add(Convert.ToInt32(id));
                }
                expable.And(it => idList.Contains(it.PartnerID.Value));
            }
            if (!string.IsNullOrEmpty(query.CustomerIDS))
            {
                var ids = query.CustomerIDS.Split(',');
                var idList = new List<int>();
                foreach (var id in ids)
                {
                    idList.Add(Convert.ToInt32(id));
                }
                expable.And(it => idList.Contains(it.CustomerID.Value));
            }
            if (!string.IsNullOrEmpty(query.SigningUnitIDS))
            {
                var ids = query.SigningUnitIDS.Split(',');
                var idList = new List<Guid>();
                foreach (var id in ids)
                {
                    idList.Add(new Guid(id));
                }
                expable.And(it => idList.Contains(it.SigningUnitGID.Value));
            }
        }
        private async Task SetSearchWhere(Expressionable<Live_V_UserEntry> expable, UserPagedModel query)
        {
            var user = CurrentUser;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                expable.And(it => it.Name.Contains(query.Keyword) || it.Phone == query.Keyword);
            }
            if (query.CID.HasValue)
            {
                expable.And(it => it.CustomerID== query.CID.Value);
            }
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => it.CustomerName.Contains(query.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.PartnerName))
            {
                expable.And(it => it.PartnerName.Contains(query.PartnerName));
            }
            if (!string.IsNullOrEmpty(query.PersonnelName))
            {
                expable.And(it => it.PersonnelName.Contains(query.PersonnelName));
            }
            if (!string.IsNullOrEmpty(query.SignCustomerName))
            {
                expable.And(it => it.SignName.Contains(query.SignCustomerName));
            }
            if (!string.IsNullOrEmpty(query.InsuranceCompany1))
            {
                expable.And(it => it.InsuranceCompany1.Contains(query.InsuranceCompany1));
            }
            expable.AndIF(query.UserGID.HasValue, it => it.UserGID == query.UserGID);
            expable.AndIF(query.ContractStatus.HasValue, it => it.ContractStatus == query.ContractStatus);
            expable.AndIF(query.IsUser.HasValue, it => it.IsUser == query.IsUser);
            expable.AndIF(query.IsNoBank, it =>string.IsNullOrEmpty(it.BankNumber));
            expable.AndIF(query.IsBlack.HasValue, it => it.IsBlack == query.IsBlack);
            if (!string.IsNullOrEmpty(query.CardID))
            {
                expable.And(it => it.CardID == query.CardID);
            }
            if (!string.IsNullOrEmpty(query.FromType))
            {
                expable.And(it => it.FromType == query.FromType);
            }
            if (!string.IsNullOrEmpty(query.Phone))
            {
                expable.And(it => it.Phone == query.Phone);
            }
            if (query.EntryStartTime.HasValue)
            {
                expable.And(it => it.EntryTime >= query.EntryStartTime.Value);
            }
            if (query.EntryEndTime.HasValue)
            {
                expable.And(it => it.EntryTime <= query.EntryEndTime.Value);
            }
            if (query.LeaveStartTime.HasValue)
            {
                expable.And(it => it.LeaveTime >= query.LeaveStartTime.Value);
            }
            if (query.LeaveEndTime.HasValue)
            {
                expable.And(it => it.LeaveTime <= query.LeaveEndTime.Value);
            }
            if (query.Mark.HasValue)
            {
                expable.And(it => it.SocialMark == query.Mark);
            }
            if (query.IsManuallyInsurance.HasValue)
            {
                expable.And(it => it.IsManuallyInsurance == query.IsManuallyInsurance.Value);
            }


            
            if ((query.From == "lzmini" || query.From == "spmini") && query.Status == (int)UserStatus.NotEntry)
            {
                var statuss = new int[] { (int)UserStatus.NotEntry, (int)UserStatus.NotInterviewed, (int)UserStatus.InterviewFailed };
                expable.And(it => statuss.Contains(it.Status.Value));
            }
            else
            {
                expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
            }

            if (!string.IsNullOrEmpty(query.TimeType))
            {
                switch (query.TimeType)
                {
                    case "entryToDate":
                        {
                            expable.And(it => SqlFunc.DateIsSame(it.EntryTime, DateTime.Now));
                        }
                        break;
                    case "leaveToDate":
                        {
                            expable.And(it => SqlFunc.DateIsSame(it.LeaveTime, DateTime.Now));
                        }
                        break;
                }
            }
            //是否总后台账号和自营公司
            //if (user.PlatformType != Model.Enums.PlatformType.Admin && user.CompanyType != Model.Enums.CompanyType.Self.ToString())

            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
            }
            if (query.From == "spmini" || user.CompanyType == Model.Enums.CompanyType.Party.ToString().ToLower())//甲方公司可以看到授权人员
            {
                expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if(user.PlatformType != Model.Enums.PlatformType.Admin && user.PlatformType != Model.Enums.PlatformType.Api)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var puser = await authService.UserInfo<Model.Live_Personnel>();
                if (query.From == "lzcustomer" && query.UserID.HasValue)
                {
                    expable.And(it => it.UserID == query.UserID);
                }
                else
                {
                    if (query.OperateAuth.HasValue && query.OperateAuth.Value)
                    {
                        expable.And(UserEntryHandle.SetPersonnelOperateAuth<Model.Live_V_UserEntry>(puser));
                    }
                    else
                    {
                        var pitems = await personnelItemsService.List(it => it.UserID == user.UserID && it.IsManage, new UserPagedModel() { Page = 0 });
                        var cids = pitems.Select(it=>it.CID);
                        var type = (Model.Enums.PersonnelType)puser.UserType;
                        switch (type)
                        {
                            case PersonnelType.Personnel:
                            case PersonnelType.View:
                            case PersonnelType.Daywork:
                            case PersonnelType.DayworkManage:
                                {
                                    expable.And( it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any());
                                }
                                break;
                            case PersonnelType.Factory:
                                {
                                    expable.And(it => cids.Contains(it.CustomerID) || it.PerUserID == user.UserID);
                                }
                                break;
                            case PersonnelType.Headed:
                                {
                                    expable.And(it => cids.Contains(it.CustomerID) || it.PartnerID == puser.PartnerID || it.PerUserID == user.UserID || it.CustomerPartnerID == puser.PartnerID);
                                }
                                break;
                            case PersonnelType.Manage:
                                break;
                            default:
                                expable.And(it => it.PerUserID == user.UserID);
                                break;
                        }
                        //expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(puser));

                    }
                    expable.AndIF(query.IsPartner.HasValue, it => it.PartnerID == puser.PartnerID);
                }
                var companyConfig = await companyConfigService.Get(puser.CompanyGID);
                if (companyConfig.Config.IsUseJobChannel && !string.IsNullOrEmpty(puser.UserFroms))//是否启动根据招聘渠道区分
                {
                    var froms = puser.UserFroms.Split(',');
                    expable.And(it => froms.Contains(it.FromType));
                }
            }
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var auth = await permissionService.CompanyLicenseList("users", "manage,service,show,edit,add", user.UserID);
                if (!auth[0] && !auth[1] && !auth[2])
                {
                    var puser = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (puser.PartnerID.HasValue)
                    {
                        expable.And(it => it.CustomerPartnerID == puser.PartnerID || it.PartnerID == puser.PartnerID);
                    }
                }
            }
        }
       
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> PartnerUsers([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            string types = "2,4,14,15,16";
            List<Live_V_UserEntry> list = new List<Live_V_UserEntry>();
            var arr = types.Split(",");

            if(!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.CardID))
            {
                expable.And(it => it.CardID.Contains(query.CardID));
            }
            expable.And(t => arr.Contains(t.Status.ToString()));

            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                //expable.And(it => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.UserID == it.UserID && s.PartnerID == info.PartnerID && arr.Contains(s.Status.ToString())).Any());
                expable.AndIF(info.UserType == 2 && info.PartnerID.HasValue, t => t.PartnerID == info.PartnerID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var info = await authService.UserInfo<Model.Live_Personnel>();
                if (!info.PartnerID.HasValue)
                {
                    throw new ErrorCodeException(-1, "该账号没有关联服务商");
                }
                expable.And(t => t.PartnerID == info.PartnerID);
            }
            list = await entryService.List(expable.ToExpression(), query);


            return Ok(new {items = list, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        [OperationLogAction("导出人员信息")]
        public async Task<IActionResult> ExportExcel([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            await SetSearchWhere(expable, query);
            var exp = expable.ToExpression();
            query.Page = 0;
            var models = await entryService.List(exp, query);
            var ids = models.Select(it=>it.ID);
            // var notes = await noteService.List(it => SqlFunc.Subqueryable<Model.Live_V_UserEntry>().Where(s=>s.ID==it.EntryID).Where(exp).Any(), new UserPagedModel() { Page = 0 });
            var notes = await noteService.List(it => ids.Contains(it.EntryID.Value), new UserPagedModel() { Page = 0 });
            var excelData = models.Select(it => new UserEntryExcelTemplate()
            {
                Name = it.Name,
                Age = it.Age,
                Sex = it.Sex,
                CustomerName = it.CustomerName,
                PartnerName = it.PartnerName,
                PersonnelName = it.PersonnelName,
                Phone = it.Phone,
                CardID = it.CardID,
                WorkName = it.WorkName,
                WorkClass = it.WorkClass,
                FromType = it.FromType,
                EntryTime = it.EntryTime.HasValue ? it.EntryTime.Value.ToString("yyyy-MM-dd") : "",
                LeaveTime = it.LeaveTime.HasValue ? it.LeaveTime.Value.ToString("yyyy-MM-dd") : "",
                SignName = it.SignName,
                StatusName = Common.Tool.GetEnumDescription((UserStatus)it.Status),
                Nation = it.Nation,
                MaritalStatus = it.MaritalStatus,
                Education = it.Education,
                BirthDate = it.BirthDate.HasValue ? it.BirthDate.Value.ToString("yyyy-MM-dd") : "",
                Address = it.Address,
                IdcardAddress = it.IdcardAddress,
                EmergencyContact = it.EmergencyContact,
                EmergencyContactPhone = it.EmergencyContactPhone,
                InsuranceStatus = it.PlanStatus1.HasValue ? ((UserEntryPlanStatusEnumType)it.PlanStatus1).ToString() : "",
                PlanName = it.PlanName1,
                CustomerProvince = it.CustomerProvince,
                CustomerCity = it.CustomerCity,
                BankName = it.BankName,
                BankNumber = it.BankNumber,
                BankProvince = it.BankProvince,
                Real = it.IsReal ? "已实名认证" : "",
                InDays = UserEntryHandle.NowMonthInDay(it),
                Remarks = it.Remarks,
                CreateTime = it.CreateTime.HasValue ? it.CreateTime.Value.ToString("yyyy-MM-dd") : "",
                Note = GetEntryNote(notes,it.ID),
                InsuranceCompany1=it.InsuranceCompany1
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(excelData, "人员信息");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = DateTime.Now.ToString("yyyy-MM-dd") + "人员信息", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        private string GetEntryNote(List<Model.Live_V_UserEntryNote> list,int id)
        {
            var contents = new StringBuilder();
            var models = list.Where(it=>it.EntryID==id);
            foreach (var model in models)
            {
                contents.AppendFormat("{0}--{1};",model.Remarks,model.CreateTime);
            }
            return contents.ToString();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ScanAdd([FromBody] Model.Live_UserEntry model)
        {
            if (!model.QrcodeGID.HasValue)
            {
                throw new ErrorCodeException(-1, "无扫码编号");
            }
            var codeNumber = string.Format(CoreRedisConstants.SmsVerify, model.IntAbout);
            var code = await redisService.GetAsync<string>(codeNumber);
            if (code != model.FromType)
            {
                throw new ErrorCodeException(-1, "验证码错误");
            }
            var qrcodeInfo = await personnelItemsService.Get(model.QrcodeGID.Value);
            if (qrcodeInfo == null || !qrcodeInfo.Enabled)
            {
                throw new ErrorCodeException(-1, "该二维码已失效");
            }
            if (!qrcodeInfo.Enabled)
            {
                throw new ErrorCodeException(-1, "该二维码已停止招聘");
            }
            if (string.IsNullOrEmpty(model.Phone))
            {
                throw new ErrorCodeException(-1, "请输入手机号");
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ErrorCodeException(-1, "请输入姓名");
            }
            if (string.IsNullOrEmpty(model.IntAbout))
            {
                throw new ErrorCodeException(-1, "缺少验证码");
            }
            var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
            if (await entryService.Exist(it => it.Phone == model.Phone && it.CompanyID == qrcodeInfo.CompanyID && it.CustomerID == qrcodeInfo.CID && statuss.Contains(it.Status.Value)))
            {
                throw new ErrorCodeException(-1, "该手机号已在同一家单位有入职信息，请联系招聘专员！");
            }
            var user = await userService.Get(it => it.Phone == model.Phone && it.CompanyID == qrcodeInfo.CompanyID);
            if (user == null) {
                user = new Model.Live_User();
                user.Phone = model.Phone;
                user.CreateTime= DateTime.Now;
                user.UserGID = Guid.NewGuid();
                user.CompanyGID = qrcodeInfo.CompanyGID;
                user.CompanyID = qrcodeInfo.CompanyID;
                user.Status = 6;
            }
            user.FullName = model.Name;
            user.Sex = model.Sex;
            user.Age = model.Age;
            user.BirthDate = model.IntDate;
            user.IdcardFrontImage = model.IdcardFrontImage;
            user.IdcardBackImage = model.IdcardBackImage;
            user.BankImage = model.BankImage;
            user.IdcardAddress = model.IdcardAddress;
            user.Nation = model.Nation;
            user.Authority = model.Authority;
            user.CardStartDate = model.CardStartDate;
            user.CardEndDate = model.CardEndDate;
            user.BankNumber = model.BankNumber;
            user.BankName = model.BankName;
            user.BankID = model.BankID;
            user.BankCardName = model.BankCardName;
            user.BankCardType = model.BankCardType;
            user.MaritalStatus = model.MaritalStatus;
            user.MajorDiseases = model.MajorDiseases;
            user.EmergencyContact = model.EmergencyContact;
            user.EmergencyContactPhone = model.EmergencyContactPhone;
            user.CardID = model.CardID;
            user.IdPhoto = model.IdPhoto;
            
            
            //黑名单
            var company = await companyConfigService.Get((Guid)qrcodeInfo.CompanyGID);
            var expable = Expressionable.Create<Model.Live_BlackList>();
            expable.And(it => it.IdentityCard == model.CardID);
            if (company.Config != null && !company.Config.UserIsUsePublicBlcak)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            user.IsBlack = await blackListService.Exist(exp);
            if (!string.IsNullOrEmpty(user.CardID))
            {
                //实名认证
                //user.IsReal = await RealAuth(company.Config.RealAuthModel, user);
                // 扫码入职强制使用三要素认证，需要签署劳动合同
                user.IsReal = await RealAuth( RealAuthModel.Three, user);
                user.IsCaAuth = await CaAuth(user.CardID);
            }
            
            if (user.UserID > 0)
            {
                await userService.Update(user);
            }
            else
            {
                user.UserID = await userService.Add(user);
            }
            model.IsReal= user.IsReal;
            model.UserID= user.UserID;
            model.CreateTime = DateTime.Now;
            model.CustomerGID = qrcodeInfo.CustomerGID;
            model.CompanyID = qrcodeInfo.CompanyID;
            model.CustomerID = qrcodeInfo.CID;
            model.CompanyGID = qrcodeInfo.CompanyGID;
            model.PerUserID = qrcodeInfo.UserID;
            model.PartnerGID = qrcodeInfo.PartnerGID;
            model.PartnerID= qrcodeInfo.PartnerID;
            model.OperateModel = qrcodeInfo.OperateModel;
            model.IsUser = true;
            model.Status = 6;
            model.IntAbout = "";
            model.IntDate = null;
            model.FromType = "";
            model.ID = await entryService.Add(model);
            if (model.ID > 0)
            {
                await redisService.DeleteAsync(codeNumber);
                publisher.Publish(new EntryRecordEventData()
                {
                    UserID = model.UserID.HasValue ? model.UserID.Value : 0,
                    EntryID = model.ID,
                    PerUserID = model.PerUserID.HasValue ? model.PerUserID.Value : 0,
                    About = "扫网页二维码",
                    CID = model.CustomerID.Value,
                    Status = model.Status.Value,
                    OperateFrom = PlatformType.XFZMini,
                    OperateFullName=model.Name,
                    OperateUserID=0
                });
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "提交错误");
            }
        }

        private async Task<bool> RealAuth(RealAuthModel type, Model.Live_User model=null, Model.Live_UserEntry entry=null)
        {

            var url = "";
            switch (type)
            {
                case RealAuthModel.Two:
                    url = "/api/fxqapi/realnameperson2";
                    break;
                case RealAuthModel.Three:
                    url = "/api/fxqapi/realnameperson3";
                    break;
                case RealAuthModel.BankThree:
                    url = "/api/fxqapi/realnamepersonbank3";
                    break;
                case RealAuthModel.Four:
                    url = "/api/fxqapi/realnameperson4";
                    break;
            }
            if (url != "")
            {
                var paras = new Dictionary<string, string>();
                if (model != null)
                {
                    paras.Add("name", model.FullName);
                    paras.Add("idCardNo", model.CardID);
                    paras.Add("phone", model.Phone);
                    paras.Add("bankNo", model.BankNumber);
                }
                else
                {
                    paras.Add("name", entry.Name);
                    paras.Add("idCardNo", entry.CardID);
                    paras.Add("phone", entry.Phone);
                    paras.Add("bankNo", entry.BankNumber);
                }
                var p = System.Text.Json.JsonSerializer.Serialize(paras);
                var res = await httpService.PostAsync<Model.Live_ContractSigner>(ServiceNames.CustomerService, url, p);
                if (res != null && res.Code == 200)
                {
                    var signer = res.Data;
                    if (!signer.IsRealNameSuc)
                    {
                        throw new ErrorCodeException(500, signer.Remark);
                    }
                    return true;
                }
                else
                {
                    Common.StaticLog.Debug(System.Text.Json.JsonSerializer.Serialize(res));
                    throw new ErrorCodeException(500, "认证错误！");
                }
            }
            return false;
        }


        private async Task<bool> CaAuth(string cardID)
        {
            try
            {
                var caAuthResp = await httpService.GetAsync<CaAuthCardIdDto>(ServiceNames.CustomerService, $"/api/CaAuthIdentity/GetCaAuthStatus?cardID={cardID}");
                if (caAuthResp != null)
                {
                    return caAuthResp.IsCaAuth;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Debug($"获取Ca认证信息失败，Message:{ex.Message}");
                return false;
            }
        }
        [HttpPost]
        public async Task<RequestResultModel> Import()
        {
            var now= DateTime.Now;
            if(now>Convert.ToDateTime(now.ToString("yyyy-MM-dd 22:00:00")))
            {
                return new RequestResultModel(500, "22点后不可导入数据！");
            }
            var user = CurrentUser;
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var path  =await Tool.UploadFile(files[0], "uploadfiles/temp/");
                if (path.IndexOf("error")==-1)
                {
                    Common.StaticLog.Debug(path);
                    var table=Common.ExcelHeler.ReadExcelToDataSet(path);
                    var customers = await httpService.GetAsync<List<Model.Live_Customer>>(ServiceNames.CustomerService, "/api/customer/list?page=0&companyId=" + user.CompanyID, isItem: true);
                    
                    if (customers == null)
                    {
                        return new RequestResultModel(500, "无客户数据");
                    }
                    var cardIdList = new List<string>();
                    var names = new List<string>();
                    var colCount = table.Tables[0].Columns.Count;
                    foreach (DataRow row in table.Tables[0].Rows)
                    {
                        if (row[2].ToString() != "")
                        {
                            cardIdList.Add(row[2].ToString());
                        }
                        if (colCount > 9 && row[9].ToString() != "")
                        {
                            names.Add(row[9].ToString().Trim());
                        }
                    }
                    var expable = Expressionable.Create<Model.Live_V_UserEntry>();
                    var statuss = new int[] { (int)UserStatus.Entry,(int)UserStatus.WaitEntry};
                    expable.And(it=>it.CompanyID==user.CompanyID  && statuss.Contains(it.Status.Value));
                    expable.And(it=>cardIdList.Contains(it.CardID));
                    var exp = expable.ToExpression();
                    var models = await entryService.List(exp,new Model.PagedModel.UserPagedModel() { Page=0});

                    var userModels = new List<Live_V_Personnel>();
                    if (names.Count > 0)
                    {
                        userModels = await personnelService.List(it => names.Contains(it.FullName) && it.CompanyID == user.CompanyID, new Model.PagedModel.UserPagedModel() { Page = 0 });
                    }
                    //黑名单
                    var blackExpable = Expressionable.Create<Model.Live_BlackList>();
                    if(user.PlatformType!= PlatformType.Admin)
                    {
                        var company = await companyConfigService.Get((Guid)user.CompanyGID);
                        blackExpable.AndIF(!company.Config.UserIsUsePublicBlcak, it => it.CompanyID == user.CompanyID);
                        blackExpable.And(it => cardIdList.Contains(it.IdentityCard));
                    }
                    var blackExp = blackExpable.ToExpression();
                    var blackModels = await blackListService.List(blackExp, new Model.PagedModel.UserPagedModel() { Page = 0 });
                    var jobs =  await httpService.GetAsync<List<Model.Live_EntryJob>>(ServiceNames.BasicService, "/api/entryjob/list?from=cache&page=0", isItem: true);
                    //Common.StaticLog.Debug(JsonConvert.SerializeObject(jobs));
                    var addModels = new List<Model.Live_UserEntry>();
                    var addInsureModels = new List<Model.Live_UserEntry>();
                    var cusLog = new StringBuilder();
                    var userLog = new StringBuilder();
                    var log = new StringBuilder();
                    var i = 0;
                    foreach (DataRow row in table.Tables[0].Rows)
                    {
                        i++;
                        //Common.StaticLog.Debug(row[0].ToString() + ";" + row[1].ToString() + ";" + row[2].ToString() + ";" + row[3].ToString());
                        if (row[0].ToString() != "" && row[1].ToString() != "" && row[2].ToString() != "" && row[3].ToString() != "")
                        {
                            var cus = customers.Find(it=>it.CustomerName== row[0].ToString().Trim());
                            if (cus != null)
                            {
                                if(models.Exists(it=>it.CardID== row[2].ToString().Trim()))
                                {
                                    userLog.AppendFormat("{0},", row[2].ToString());
                                }
                                var model = new Model.Live_UserEntry();
                                if (user.PlatformType != PlatformType.Admin)
                                {
                                    model.CompanyID = user.CompanyID;
                                    model.CompanyGID = user.CompanyGID;
                                }
                                model.PartnerGID = cus.PartnerGID;
                                model.PartnerID = cus.PartnerID;
                                model.OperateModel = (int)OperateModel.Routine;
                                model.IsUser = false;
                                model.Name = row[1].ToString().Trim();
                                model.CardID = row[2].ToString().Trim();
                                model.CustomerID = cus.CustomerID;
                                model.CustomerGID = cus.CustomerGID;
                                model.WorkName = row[3].ToString().Trim();
                                model.GID = Guid.NewGuid();
                                if (!string.IsNullOrEmpty(model.WorkName))
                                {
                                    var job = jobs.Find(it=>it.Name==model.WorkName);
                                    if (job != null)
                                    {
                                        model.WorkClass = job.Type;
                                    }
                                    else
                                    {
                                        return new RequestResultModel(500, model.Name + "的"+ model.WorkName + "工种不存在！");
                                    }
                                }
                                else
                                {
                                    return new RequestResultModel(500, model.Name+"缺少工种！");
                                }
                                if (!string.IsNullOrEmpty(row[4].ToString()))
                                {
                                    try
                                    {
                                        model.EntryTime = Convert.ToDateTime(row[4].ToString().Trim());
                                    }
                                    catch(Exception ex)
                                    {
                                        return new RequestResultModel(500, $"第{i}行的入职时间不正确！");
                                    }
                                    model.Status = (int)UserStatus.Entry;
                                }
                                else
                                {
                                    model.Status = (int)UserStatus.WaitEntry;
                                }
                                model.FromType = !string.IsNullOrEmpty(row[5].ToString()) ? row[5].ToString().Trim() : "";
                                model.Phone = !string.IsNullOrEmpty(row[6].ToString()) ? row[6].ToString().Trim() : "";
                                model.InsureType = !string.IsNullOrEmpty(row[7].ToString()) ? row[7].ToString().Trim() : "";
                                if(row[8]!=null && !string.IsNullOrEmpty(row[8].ToString()) && row[8].ToString().Trim()== "待增员")
                                {
                                    model.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                                }
                                if (colCount > 9 && row[9].ToString() != "")
                                {
                                    var nameModel = userModels.Find(it => it.FullName == row[9].ToString().Trim());
                                    if (nameModel != null)
                                    {
                                        if (nameModel.PartnerGID.HasValue && !model.PartnerGID.HasValue)
                                        {
                                            model.PartnerGID = nameModel.PartnerGID;
                                            model.PartnerID = nameModel.PartnerID;
                                        }
                                        model.PerUserID = nameModel.UserID;
                                    }
                                }
                                if (colCount > 10)
                                {
                                    model.BankName = !string.IsNullOrEmpty(row[10].ToString()) ? row[10].ToString().Trim() : "";
                                    model.BankNumber = !string.IsNullOrEmpty(row[11].ToString()) ? row[11].ToString().Trim() : "";
                                    model.Sex = !string.IsNullOrEmpty(row[12].ToString()) ? row[12].ToString().Trim() : "";
                                    model.Age = !string.IsNullOrEmpty(row[13].ToString()) ? Convert.ToInt32(row[13].ToString().Trim()) : null;
                                    model.Nation = !string.IsNullOrEmpty(row[14].ToString()) ? row[14].ToString().Trim() : "";
                                    model.IdcardAddress = !string.IsNullOrEmpty(row[15].ToString()) ? row[15].ToString().Trim() : "";
                                }
                                model.CreateTime = DateTime.Now;
                                model.IsBlack = blackModels.Exists(it=>it.IdentityCard== model.CardID);
                                if (model.InsureStatus == (int)Model.Enums.UserEntryPlanStatusEnumType.待增员)
                                {
                                    addInsureModels.Add(model);
                                }
                                else
                                {
                                    addModels.Add(model);
                                }
                            }
                            else
                            {
                                cusLog.AppendFormat("{0},", row[1].ToString());
                            }
                        }
                        else
                        {
                            log.AppendFormat("第{0}行,", i);
                        }
                    }
                    if (log.Length > 0)
                    {
                        log.Append("信息不全，如果该几行数据都为空，请先删除！");
                    }
                    if (cusLog.Length > 0)
                    {
                        cusLog.Append("客户单位不存在！");
                        log.Append(cusLog.ToString());
                    }
                    if (userLog.Length > 0)
                    {
                        userLog.Append("员工身份证信息已存在！");
                        log.Append(userLog.ToString());
                    }
                    if (log.Length > 0)
                    {
                        //return Ok("部分导入成功，但存在"+log.ToString());
                        //throw new ErrorCodeException(-1, log.ToString());
                        return new RequestResultModel(500, log.ToString());
                    }
                    else
                    {
                        foreach (var m in addModels)
                        {
                            if (!await RealAuth(RealAuthModel.Two, entry: m))
                            {
                                log.Append(m.Name + ",");
                            }
                        }
                        foreach (var m in addInsureModels)
                        {
                            if (!await RealAuth(RealAuthModel.Two, entry: m))
                            {
                                log.Append(m.Name + ",");
                            }
                        }
                        if (log.Length > 0)
                        {
                            log.Append("身份证错误或与姓名不匹配！");
                            return new RequestResultModel(500, log.ToString());
                        }

                        var errorName = new List<string>();
                        if (addModels.Count > 0)
                        {
                            await entryService.Add(addModels);
                        }
                        if (addInsureModels.Count > 0)
                        {
                            var ids = await entryService.Add(addInsureModels);
                            var reIds = new List<int>();
                            foreach (var id in ids)
                            {
                                try
                                {
                                    var has = await businessInsuranceRecordService.ManageUserEntryPlan(id);
                                    if (!has) reIds.Add(id);
                                }
                                catch (Exception ex)
                                {
                                    reIds.Add(id);
                                    Common.StaticLog.Error(ex);
                                }
                            }
                            if (reIds.Count > 0)
                            {
                                var reModels = await entryService.List(it => reIds.Contains(it.ID));
                                foreach (var rem in reModels)
                                {
                                    errorName.Add(rem.Name);
                                }
                            }
                        }
                        if (errorName.Count > 0)
                        {
                            var reLog = "未参保人员：" + String.Join(',', errorName);
                            Common.StaticLog.Debug(reLog);
                            return new RequestResultModel(201, "导入成功！有"+ reLog);
                        }
                        else
                        {
                            return new RequestResultModel(200, "导入成功！");
                        }
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, path);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请选择文件");
            }
        }
        [HttpPost]
        [OperationLogAction("新增线下员工")]
        public async Task<IActionResult> Add([FromBody] Model.Live_UserEntry model,bool isBack=false)
        {
            var user = CurrentUser;
            var company = await companyConfigService.Get((Guid)user.CompanyGID);
            if (model.Status == 2)
            {
                EntryCommon.NoEntryTime(company.Config);//判断是否在入职时间段
            }
           
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                model.CompanyID = user.CompanyID;
                model.CompanyGID = user.CompanyGID;
            }
            if(company.Config.IsOfflinePhone && string.IsNullOrEmpty(model.Phone))
            {
                throw new ErrorCodeException(500, "请填写手机号！");
            }
            
            if (!await RealAuth(RealAuthModel.Two, entry: model))
            {
                throw new ErrorCodeException(500, "实名认证未通过！");
            }

            var umodel = await authService.UserInfo<Model.Live_Personnel>();
            model.PartnerGID = umodel.PartnerGID;
            model.PartnerID = umodel.PartnerID;
            model.CreateTime = DateTime.Now;
            model.IsUser = false;
            model.UserID = 0;
            //招聘模式
            if (model.OperateModel.HasValue)
            {
                if(model.OperateModel == (int)OperateModel.Common && model.JointPartnerID== umodel.PartnerID)
                {
                    model.JointPartnerGID = null;
                    model.JointPartnerID = null;
                }
            }
            else
            {
                model.OperateModel = (int)OperateModel.Routine;
            }
            if (!model.PerUserID.HasValue)
            {
                model.PerUserID = user.UserID;
            }
           
            model.IntDate = DateTime.Now;
           
            var statuss = new int[] { (int)UserStatus.WaitEntry, (int)UserStatus.Entry };
            var entryExpable = Expressionable.Create<Model.Live_UserEntry>();
            entryExpable.AndIF(user.PlatformType != Model.Enums.PlatformType.Admin, it => it.CompanyID == user.CompanyID);
            entryExpable.And(it => statuss.Contains(it.Status.Value));
            if (!string.IsNullOrEmpty(model.CardID))
            {
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    entryExpable.And(it => it.Phone == model.Phone || it.CardID == model.CardID);
                }
                else
                {
                    entryExpable.And(it => it.CardID == model.CardID);
                }
            }
            else if(!string.IsNullOrEmpty(model.Phone))
            {
                entryExpable.And(it => it.Phone == model.Phone);
            }
            var entryExp = entryExpable.ToExpression();
            var exist= await entryService.Exist(entryExp);
            if (exist)
            {
                throw new ErrorCodeException(500, "该手机或身份证已存在待入职、已入职的记录！");
            }
            //if (!string.IsNullOrEmpty(model.CardID))//修改为前端单独接口判断
            //{
            //    //黑名单

            //    var expable = Expressionable.Create<Model.Live_BlackList>();
            //    expable.And(it => it.IdentityCard == model.CardID);
            //    if (company.Config != null && !company.Config.UserIsUsePublicBlcak)
            //    {
            //        expable.And(it => it.CompanyID == user.CompanyID);
            //    }
            //    var exp = expable.ToExpression();
            //    var blackList = await blackListService.List(exp, new Model.PagedModel.UserPagedModel() { Page = 0 });
            //    if (blackList.Count > 0)
            //    {

            //    }
            //    else
            //    {
            //        model.IsBlack = false;
            //    }
            //}
            //if (model.WorkClass == "5类" && model.PartnerID != 202)//5类员工入职限制
            //{
            //    var now = DateTime.Now;
            //    var week = now.DayOfWeek;
            //    model.Tabs = "," + TabsEnum.五类过渡参保.ToString();
            //}
            //FiveNotEntry(model);
            if (model.Status == 2 && !model.EntryTime.HasValue)
            {
                model.EntryTime = DateTime.Now;
            }
           model.ID =  await entryService.Add(model);
            if (model.ID > 0)
            {
                if (model.Status == 2)
                {
                    var insurance = new BusinessInsuranceAddOrDelEventData();//发送商保是否增减员
                    insurance.UserEntryID = model.ID;
                    insurance.IsAdd = false;
                    publisher.Publish(insurance);
                }
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = model.ID;
                record.PerUserID = user.UserID;
                record.Status = model.Status.HasValue ? model.Status.Value : 0;
                record.About = "小程序新增线下员工操作！";
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                await jdySyncService.UserSync(model.ID);
            }
            return Ok();
        }
        [HttpPost]
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [OperationLogAction("日结工入职")]
        public async Task<IActionResult> AddDayWork([FromForm] int id, [FromForm] int cid, [FromForm] string message, [FromForm] string sendTime)
        {
            var job = await httpService.GetAsync<Model.Live_V_JobsDayWork>(ServiceNames.CustomerService, "/api/jobsdaywork/getView?id=" + id);
            var file = await httpService.GetAsync<Model.Live_DayWorkUserContractFile>(ServiceNames.CustomerService, "/api/DayWorkUserContractRecord/get?id=" + cid);
            if (job == null)
            {
                throw new ErrorCodeException(-1, "日结岗位不存在！");
            }
            
            var user = CurrentUser;
            var umodel = await authService.UserInfo<Model.Live_User>();
            var model = new Live_UserEntry();
            model.CustomerID = job.CustomerID;
            model.CustomerGID=job.CustomerGID;
            model.IdcardAddress = umodel.IdcardAddress;
            model.IdcardBackImage = umodel.IdcardBackImage;
            model.IdcardFrontImage = umodel.IdcardFrontImage;
            model.IdPhoto = umodel.IdPhoto;
            model.BankProvince = umodel.BankProvince;
            model.BankCardName = umodel.BankCardName;
            model.BankCardType= umodel.BankCardType;
            model.BankCity= umodel.BankCity;
            model.BankID = umodel.BankID;
            model.BankImage = umodel.BankImage;
            model.BankName= umodel.BankName;
            model.BankNumber= umodel.BankNumber;
            model.CardID = umodel.CardID;
            model.Age = umodel.Age;
            model.Name = umodel.FullName;
            model.Sex=umodel.Sex;
            model.IsReal= umodel.IsReal;
            model.JobDayWorkID = id;
            model.JobDayWorkNo = file.EntryNo;
            model.MajorDiseases=umodel.MajorDiseases;
            model.MaritalStatus = umodel.MaritalStatus;
            model.Nation = umodel.Nation;
            model.Phone = umodel.Phone;
            model.IsDayWork = true;
            model.CardStartDate = umodel.CardStartDate;
            model.Authority = umodel.Authority;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.Status = 2;
            model.PartnerID = job.PartnerID;
            model.PartnerGID = job.PartnerGID;
            model.OperateModel = 1;

            if (!umodel.IsReal)
            {
                throw new ErrorCodeException(-1, "未实名认证！");
            }
            model.CreateTime = DateTime.Now;
            model.IsUser = true;
            model.UserID = user.UserID;
            var entryExpable = Expressionable.Create<Model.Live_UserEntry>();
            entryExpable.AndIF(user.PlatformType != Model.Enums.PlatformType.Admin, it => it.CompanyID == user.CompanyID);
            entryExpable.And(it => it.Status == (int)UserStatus.Entry);
            if (!string.IsNullOrEmpty(model.CardID))
            {
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    entryExpable.And(it => it.Phone == model.Phone || it.CardID == model.CardID);
                }
                else
                {
                    entryExpable.And(it => it.CardID == model.CardID);
                }
            }
            else if (!string.IsNullOrEmpty(model.Phone))
            {
                entryExpable.And(it => it.Phone == model.Phone);
            }
            var entryExp = entryExpable.ToExpression();
            var exist = await entryService.Exist(entryExp);
            if (exist)
            {
                throw new ErrorCodeException(-1, "该手机或身份证已存在已入职的记录！");
            }

            if (!string.IsNullOrEmpty(model.CardID))
            {
                //黑名单
                var company = await companyConfigService.Get((Guid)user.CompanyGID);
                var expable = Expressionable.Create<Model.Live_BlackList>();
                expable.And(it => it.IdentityCard == model.CardID);
                if (company.Config != null && !company.Config.UserIsUsePublicBlcak)
                {
                    expable.And(it => it.CompanyID == user.CompanyID);
                }
                var exp = expable.ToExpression();
                model.IsBlack = await blackListService.Exist(exp);
            }
            model.EntryTime = DateTime.Now;
            model.ID = await entryService.Add(model);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("jobID", id.ToString());
            dic.Add("fileID", cid.ToString());
            dic.Add("entryID", model.ID.ToString());
            dic.Add("message", message);
            dic.Add("sendTime", sendTime);
            var result =  await httpService.PostAsync(ServiceNames.CustomerService, "/api/DayWorkUserContractRecord/AddWX", dic);
            var json = JsonConvert.DeserializeObject(result.Data.ToString()) as JObject;
            if (json["code"].ToString() != "0")
            {
                await entryService.Delete(model.ID);
                return Ok(new { msg = json["msg"].ToString() });
            }
            //if (model.ID > 0)
            //{
            //    var entryContact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.入职合同, Guid.NewGuid());
            //    publisher.Publish(entryContact);
            var record = new EntryRecordEventData();
            record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
            record.EntryID = model.ID;
            record.PerUserID = user.UserID;
            record.Status = model.Status.HasValue ? model.Status.Value : 0;
            record.About = "小程序日结工入职操作！";
            record.OperateFrom = user.PlatformType;
            record.OperateFullName = user.FullName;
            record.OperateUserID = user.UserID;
            publisher.Publish(record);
            //}
            return Ok(new { json });
        }
        private void FiveNotEntry(Model.Live_UserEntry model)
        {
            if (model.WorkClass == "5类" && model.PartnerID!=202)
            {
                var now = DateTime.Now;
                var week = now.DayOfWeek;
                if (week == 0 || week == DayOfWeek.Saturday || now > Convert.ToDateTime(now.ToString("yyyy-MM-dd 15:00:00")))
                {
                    throw new ErrorCodeException(-1, "5类员工非工作日或工作日15后不准入职或离职！");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> LeaveDayWork([FromForm] int id)
        {

            var limitLeaveDay = new DateTime(2026, 10, 14);
            var dt = DateTime.Today.Date;
            if (dt == limitLeaveDay && DateTime.Now.Hour < 16)
            {
                throw new ErrorCodeException(-1, "离职流程调整，暂不能处理离职，离职操作2026年10月14日16点（今日）恢复！");
            }

            var user = CurrentUser;
            var model = await entryService.Get(id);
            if (model.InjuryStatus == 1)
            {
                throw new ErrorCodeException(-1, "工伤状态下不能离职！");
            }
            model.Status = (int)UserStatus.Leave;
            model.LeaveTime = DateTime.Now;
            model.LeaveAbout = model.Name + "日结工离职";
            await entryService.Update(model);

            var record = new EntryRecordEventData();
            record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
            record.EntryID = id;
            record.PerUserID = model.UserID.Value;
            record.Status = (int)UserStatus.Leave;
            record.About = model.LeaveAbout;
            record.OperateFrom = user.PlatformType;
            record.OperateFullName = user.FullName;
            record.OperateUserID = user.UserID;
            publisher.Publish(record);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("jobID", model.JobDayWorkID.ToString());
            dic.Add("number", "-1");
            var result = await httpService.PostAsync(ServiceNames.CustomerService, "/api/DayWorkUserContractRecord/ChangeNumber", dic);

            return Ok();
        }
        [HttpGet]
        public async Task<bool> Exist(string value,string type)
        {
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            var user = CurrentUser;
            var statuss = new int[] { (int)UserStatus.WaitEntry, (int)UserStatus.Entry };
            expable.AndIF(user.PlatformType != Model.Enums.PlatformType.Admin, it => it.CompanyID == user.CompanyID);
            expable.And(it => statuss.Contains(it.Status.Value));
            switch (type)
            {
                case "cardid":
                    expable.And(it=>it.CardID==value);
                    break;
                case "phone":
                    expable.And(it => it.Phone == value);
                    break;
            }
            var exp = expable.ToExpression();
            return await entryService.Exist(exp);
        }
        [HttpGet]
        public async Task<List<Model.CountModel>> GetStatusCount()
        {
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            var user = CurrentUser;
            //是否总后台账号和自营公司
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                if (user.CompanyType == CompanyType.Party.ToString().ToLower().ToLower())
                {
                    expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
                }
                else
                {
                    expable.And(it => it.CompanyID == user.CompanyID);
                }
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var puser = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(UserEntryHandle.SetPersonnelAuth<Model.Live_V_UserEntry>(puser));
            }

            var exp = expable.ToExpression();
            return await entryService.StatusCount(exp);
        }
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [HttpGet]
        public async Task<IActionResult> UserEntryInfo()
        {
            var user = CurrentUser;
            var model = await entryService.View(it => it.UserID == user.UserID && it.Status == 2);
            Model.Live_Personnel factory = null;
            var other = new StringBuilder();
            if (model == null)
            {
                model = await entryService.View(it => it.UserID == user.UserID && it.Status == 4);
            }
            if (model != null)
            {
                if(model.FactoryUserID.HasValue && model.FactoryUserID > 0)
                {
                    factory = await personnelService.Get(model.FactoryUserID.Value);
                }
                if (factory == null)
                {
                    var type =new int[] { (int)PersonnelType.Factory, (int)PersonnelType.Headed};
                    factory = await personnelService.Get(it=> type.Contains(it.UserType.Value) && SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == model.CustomerID && s.UserID==it.UserID).Any());
                }
               var res = await httpService.GetAsync(ServiceNames.FinanceService, "/api/SalaryCustomer/getSalarySendTime?customerId=" + model.CustomerGID);
                if (res != null && res.Code==200) {
                    other.AppendFormat("发工资日期:{0};", res.Data);
                }
            }
            var umodel= await authService.UserInfo<Model.Live_User>();
            if (!string.IsNullOrEmpty(umodel.City))
            {
                other.AppendFormat("期望工作地:{0};", umodel.City);
            }
            if (umodel.MinMoney.HasValue && umodel.MaxMoney.HasValue && umodel.MaxMoney>0)
            {
                other.AppendFormat("期望工资:{0}-{1};", umodel.MinMoney, umodel.MaxMoney);
            }
            //if (!string.IsNullOrEmpty(umodel.JobNames))
            //{
            //    other.AppendFormat("期望岗位:{0};", umodel.JobNames);
            //}
            return Ok(new { info = model, factory, other=other.ToString() });
        }
        [HttpGet]
        public async Task<Model.Live_UserEntry> Get(int id)
        {
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower() || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                companyId = 0;
            }
            return await entryService.Get(id, companyId);
        }
        [HttpGet]
        public async Task<Model.Live_UserEntry> GetInfo(Guid id)
        {
            var model= await entryService.Get(it=>it.GID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            return model;
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        [HttpGet]
        public async Task<Model.Live_V_UserEntry> ViewInfo(Guid id)
        {
            var model = await entryService.View(it => it.GID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            return model;
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        [HttpGet]
        public async Task<Model.Live_V_UserEntry> View(int id)
        {
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower() || user.CompanyType == Model.Enums.CompanyType.Party.ToString().ToLower() || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                companyId = 0;
            }
            var model = await entryService.View(id, companyId);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            if(user.PlatformType != Model.Enums.PlatformType.Admin && user.PlatformType != Model.Enums.PlatformType.MiniAdmin)
            {
                //model.BankImage = "";
                ////model.BankName = "";
                ////model.BankNumber = "";
                ////model.CardID = "";
                //model.IdcardBackImage = "";
                //model.IdcardFrontImage = "";
                //model.Authority = "";
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_V_UserEntry> EntryInfo([FromQuery] UserPagedModel query)
        {
            if(string.IsNullOrEmpty(query.Name) && string.IsNullOrEmpty(query.Phone) && string.IsNullOrEmpty(query.CardID))
            {
                throw new ErrorCodeException(-1, "请输入正确信息查询！");
            }
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            expable.AndIF(!string.IsNullOrEmpty(query.Name), it => it.Name == query.Name);
            expable.AndIF(!string.IsNullOrEmpty(query.Phone), it => it.Phone == query.Phone);
            expable.AndIF(!string.IsNullOrEmpty(query.CardID), it => it.CardID == query.CardID);
            var model = await entryService.View(expable.ToExpression());
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工入职信息不存在！");
            }
            //model.BankImage = "";
            ////model.BankName = "";
            ////model.BankNumber = "";
            ////model.CardID = "";
            //model.IdcardBackImage = "";
            //model.IdcardFrontImage = "";
            //model.Authority = "";
            return model;
        }
        /// <summary>
        /// 入职
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPost]
        [OperationLogAction("操作入职姓名:{m.Name};id:{m.ID}")]
        public async Task<IActionResult> UserEntry([FromBody] Model.Live_UserEntry m)
        {
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString())
            {
                companyId = 0;
            }
            var model = await entryService.Get(m.ID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            if (companyId > 0 && model.CompanyID!= companyId)
            {
                throw new ErrorCodeException(-1, "您无权查看！");
            }
            if (model.Status == (int)UserStatus.Entry)
            {
                throw new ErrorCodeException(-1, "该员工已入职！");
            }
            if (model.Status != (int)UserStatus.WaitEntry)
            {
                throw new ErrorCodeException(-1, "该员工未处于待入职状态，不可操作！");
            }
            //if (m.WorkClass == "5类" && m.PartnerID != 202)//5类员工入职限制
            //{
            //    var now = DateTime.Now;
            //    var week = now.DayOfWeek;
            //    model.Tabs = "," + TabsEnum.五类过渡参保.ToString();
            //}
            //FiveNotEntry(m);
            if (model.IsUser && model.UserID.HasValue)
            {
                if (!model.IsReal)
                {
                    throw new ErrorCodeException(-1, "该员工未实名，请联系员工填写入职信息！");
                }
               //var userModel =  await userService.Get(model.UserID.Value);
               // if (!userModel.IsReal)
               // {
               //     throw new ErrorCodeException(-1, "该员工未实名，请联系员工填写入职信息！");
               // }
            }
            var companyInfo = await companyConfigService.Get((Guid)model.CompanyGID);
            EntryCommon.NoEntryTime(companyInfo.Config);//判断是否在入职时间段
            
            if (m.CustomerID != model.CustomerID && m.OperateModel.HasValue)
            {
                model.OperateModel = m.OperateModel;
                model.JointPartnerID = m.JointPartnerID;
                model.JointPartnerGID = m.JointPartnerGID;
            }
            model.CustomerID = m.CustomerID;
            model.CustomerGID = m.CustomerGID;
            model.EntryTime = m.EntryTime;
            model.WorkClass = m.WorkClass;
            model.WorkName = m.WorkName;
            model.FromType = m.FromType;
            model.IsDayOver = m.IsDayOver;
            model.HandUserID = user.UserID;
            model.ContractDate = m.ContractDate;
            model.StructureID = m.StructureID;
            var about = "小程序入职操作！";
            var userId = 0;
            if(user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                userId = user.UserID;
                var userInfo = await authService.UserInfo<Model.Live_Personnel>();
                about = userInfo.FullName + "在小程序入职操作！";
            }
            else
            {
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    var userInfo = await authService.UserInfo<Model.SYS_CompanyUser>();
                    about = user.FullName+ "在后台入职操作！";
                }
                else if (user.PlatformType == Model.Enums.PlatformType.Admin)
                {
                    about = "总后台入职操作！";
                }
                else
                {
                    about = "后台入职操作(其他)！";
                }
            }
            if (companyInfo.Config.EntryChangePerUser && m.PerUserID.HasValue && m.PerUserID>0 && m.PerUserID!=model.PerUserID)
            {
                model.PerUserID = m.PerUserID;
                about += "；修改招聘专员ID为" + m.PerUserID;
            }
            var has = await entryService.UserEntryAffairs(model, companyInfo.Config.UserIsEntryOther,user);
            if (has>0)
            {
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = model.ID;
                record.PerUserID = userId;
                record.Status = (int)UserStatus.WaitEntry;
                record.About = about;
                record.EntryTime = m.EntryTime;
                record.CID = m.CustomerID.HasValue ? m.CustomerID.Value : 0;
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                return Ok();
            }
            else
            {
              
                    throw new ErrorCodeException(-1, "入职失败，你无权修改！");
            }
        }
        /// <summary>
        /// 待入职
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [OperationLogAction("操作待入职姓名:{m.Name};id:{m.ID}")]
        public async Task<IActionResult> WaitEntry([FromBody] Model.Live_UserEntry m)
        {
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString())
            {
                companyId = 0;
            }
            var model = await entryService.Get(m.ID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            if (companyId > 0 && model.CompanyID != companyId)
            {
                throw new ErrorCodeException(-1, "您无权查看！");
            }
            if (model.Status == (int)UserStatus.WaitEntry)
            {
                throw new ErrorCodeException(-1, "该记录已待入职！");
            }
            if (model.Status != (int)UserStatus.WaitInterview)
            {
                throw new ErrorCodeException(-1, "该记录未处于待面试状态，不可操作！");
            }
            m.UserID = model.UserID;
            m.IsUser = model.IsUser;
            var has = await entryService.UpdateInt(m, (int)UserStatus.WaitEntry, companyId);
            if (has)
            {
                //if(model.IsUser && model.UserID.HasValue)
                //{
                //    var nostatus = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
                //    await userService.ChangeStatus(model.UserID.Value, (int)UserStatus.WaitEntry, nostatus);
                //}
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = model.ID;
                record.PerUserID = user.UserID;
                record.Status = (int)UserStatus.WaitEntry;
                record.About = "待入职操作！";
                record.CID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "待入职失败，你无权修改！");
            }
        }
        /// <summary>
        /// 更改工种
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="work"></param>
        /// <param name="workClass"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [OperationLogAction("更改工种id:{id}")]
        public async Task<IActionResult> ChangeWork([FromForm]int id, [FromForm]string workName, [FromForm]string workClass)
        {
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower())
            {
                companyId = 0;
            }
            var model = await entryService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            if (companyId > 0 && model.CompanyID != companyId)
            {
                throw new ErrorCodeException(-1, "您无权查看！");
            }
            if (model.Status != (int)UserStatus.Entry)
            {
                throw new ErrorCodeException(-1, "未入职不可更改工种！");
            }
            var has = await entryService.ChangeWork(id, workName, workClass);
            if (has)
            {
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = model.ID;
                record.PerUserID = user.UserID;
                record.Status = model.Status.Value;
                record.About = "更改工种操作！原信息：工种（" + model.WorkName + "），类型（" + model.WorkClass + "）";
                record.CID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                await redisService.IncrByAsync(CoreRedisConstants.DayTotalChangeWork);//更换工种统计
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "人员操作失败，你无权修改！");
            }

        }
        /// <summary>
        /// 更改备注
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        [OperationLogAction("更改备注id:{id}")]
        public async Task<IActionResult> ChangeRemarks([FromForm] int id, [FromForm] string remarks)
        {
            if (id<=0)
            {
                throw new ErrorCodeException(-1, "来源错误！");
            }
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower())
            {
                companyId = 0;
            }
            var model = await entryService.Get(id, companyId);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "该员工不存在或你无权查看！");
            }
            var has = await entryService.UpdateRemarks(id, remarks);
            if (has)
            {
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.Value;
                record.EntryID = id;
                record.PerUserID = user.UserID;
                record.Status = model.Status.Value;
                record.About = "修改备注为：“"+ remarks + "”";
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "人员操作失败，你无权修改！");
            }

        }
        /// <summary>
        /// 更改入职信息
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        [OperationLogAction("更改入职信息id:{m.ID}")]
        public async Task<IActionResult> ChangeEntryInfo([FromForm] Model.Live_UserEntry m)
        {
            if (m == null || m.ID <= 0)
            {
                throw new ErrorCodeException(-1, "来源错误！");
            }
            var user = CurrentUser;
            var userName = "";
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                var userModel = await authService.UserInfo<Model.SYS_ManageUser>();
                userName = userModel.FullName;
            }
            else if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var userModel = await authService.UserInfo<Model.SYS_CompanyUser>();
                userName = userModel.FullName;
            }
            var model = await entryService.View(it=>it.GID==m.GID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在或你无权查看！");
            }
            var has = await entryService.ChangeEntryInfo(m);
            if (has)
            {
                var ops = new StringBuilder();
                ops.AppendFormat("{0}修改入职信息；", userName);
                if (m.PartnerID != model.PartnerID)
                {
                    ops.AppendFormat("服务商由“{0}”改为“{1}”；", model.PartnerName, m.Name);
                }
                if (m.PerUserID != model.PerUserID)
                {
                    ops.AppendFormat("招聘专员由“{0}”改为“{1}”；", model.PersonnelName, m.Phone);
                }
                if (m.FromType != model.FromType)
                {
                    ops.AppendFormat("招聘渠道由“{0}”改为“{1}”；", model.FromType, m.FromType);
                }
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = m.ID;
                record.PerUserID = user.UserID;
                record.Status = model.Status.Value;
                record.About = ops.ToString();
                publisher.Publish(record);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "人员操作失败，你无权修改！");
            }

        }
        private JDYFieldModel GetJDYField(List<JDYFieldModel> list,string name)
        {
            var field = list.Find(it => it.Type == name);
            if (field == null)
            {
                throw new ErrorCodeException(-1, $"未配置简道云{name}字段信息");
            }
            return field;
        }
       
        [HttpPost]
        public async Task<IActionResult> SyncJDYUser([FromForm] int id)
        {
            await jdySyncService.UserSync(id);
            return Ok();
        }
        
        /// <summary>
        /// 更改入职信息
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPut]
        [OperationLogAction("更改入职信息姓名:{data.Entry.Name};id:{data.Entry.ID}")]
        public async Task<IActionResult> ChangeEntry([FromBody]Model.EntryChangeModel data)
        {
            var m = data.Entry;


            if (m.Status==4)
            {
                var limitLeaveDay = new DateTime(2026, 10, 14);
                var dt = DateTime.Today.Date;
                if (dt == limitLeaveDay && DateTime.Now.Hour < 16)
                {
                    throw new ErrorCodeException(-1, "离职流程调整，暂不能处理离职，离职操作2026年10月14日16点（今日）恢复！");
                }
            }
            


            var paras = data.Paras;
            if (m==null || !m.GID.HasValue)
            {
                throw new ErrorCodeException(-1, "来源错误！");
            }
            var user = CurrentUser;
            var userName = "";
            var auth = false;
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                var userModel =await authService.UserInfo<Model.SYS_ManageUser>();
                userName = userModel.FullName;
                auth=await permissionService.ManageLicense("users", "changeentry", user.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var userModel = await authService.UserInfo<Model.SYS_CompanyUser>();
                userName = userModel.FullName;
                auth=await permissionService.CompanyLicense("users", "changeentry", user.UserID);
            }
            if (!auth)
            {
                throw new ErrorCodeException(-1, "您无权修改！");
            }
            var model = await entryService.View(it => it.GID == m.GID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在或你无权查看！");
            }
            if(m.Status==(int)UserStatus.Leave && !m.LeaveTime.HasValue)
            {
                throw new ErrorCodeException(-1, "人员离职需要输入离职时间！");
            }
            if (m.Status != (int)UserStatus.Leave && m.LeaveTime.HasValue)
            {
                throw new ErrorCodeException(-1, "人员非离职操作无需离职时间！");
            }
            var ops = new StringBuilder();
            ops.AppendFormat("{0}修改入职信息；", userName);
            if (m.Sex != model.Sex)
            {
                ops.AppendFormat("性别由“{0}”改为“{1}”；", model.Sex, m.Sex);
            }
            if (m.Age != model.Age)
            {
                ops.AppendFormat("年龄由“{0}”改为“{1}”；", model.Age, m.Age);
            }
            if (m.Education != model.Education)
            {
                ops.AppendFormat("学历由“{0}”改为“{1}”；", model.Education, m.Education);
            }
            if (m.EmergencyContact != model.EmergencyContact)
            {
                ops.AppendFormat("紧急联系人由“{0}”改为“{1}”；", model.EmergencyContact, m.EmergencyContact);
            }
            if (m.EmergencyContactPhone != model.EmergencyContactPhone)
            {
                ops.AppendFormat("紧急号码由“{0}”改为“{1}”；", model.EmergencyContactPhone, m.EmergencyContactPhone);
            }
            if (m.MaritalStatus != model.MaritalStatus)
            {
                ops.AppendFormat("婚姻状态由“{0}”改为“{1}”；", model.MaritalStatus, m.MaritalStatus);
            }
            if (m.MajorDiseases != model.MajorDiseases)
            {
                ops.AppendFormat("身体状态由“{0}”改为“{1}”；", model.MajorDiseases, m.MajorDiseases);
            }
            if (m.BankName != model.BankName)
            {
                ops.AppendFormat("所属行码由“{0}”改为“{1}”；", model.BankName, m.BankName);
            }
            if (m.BankProvince != model.BankProvince)
            {
                ops.AppendFormat("银行所属省份由“{0}”改为“{1}”；", model.BankProvince, m.BankProvince);
            }
            if (m.BankCity != model.BankCity)
            {
                ops.AppendFormat("银行所属城市由“{0}”改为“{1}”；", model.BankCity, m.BankCity);
            }
            if (m.StructureID != model.StructureID)
            {
                ops.AppendFormat("车间由“{0}”改为“{1}”；", model.StructureName, paras.StructureName);
            }
            if (m.PartnerID != model.PartnerID)
            {
                ops.AppendFormat("服务商由“{0}”改为“{1}”；", model.PartnerName, paras.PartnerName);
            }
            if (m.PerUserID != model.PerUserID)
            {
                ops.AppendFormat("招聘专员由“{0}”改为“{1}”；", model.PersonnelName, paras.PersonnelName);
            }
            if (m.FromType != model.FromType)
            {
                ops.AppendFormat("招聘渠道由“{0}”改为“{1}”；", model.FromType, m.FromType);
            }
            if (m.Name != model.Name)
            {
                ops.AppendFormat("姓名由“{0}”改为“{1}”；", model.Name, m.Name);
            }
            if (m.Phone != model.Phone)
            {
                ops.AppendFormat("手机号由“{0}”改为“{1}”；", model.Phone, m.Phone);
            }
            if (m.LeaveTime != model.LeaveTime)
            {
                ops.AppendFormat("离职时间由“{0}”改为“{1}”；", model.LeaveTime, m.LeaveTime);
            }
            if (m.EntryTime != model.EntryTime)
            {
                ops.AppendFormat("离职时间由“{0}”改为“{1}”；", model.EntryTime, m.EntryTime);
            }
            if (m.Status != model.Status)
            {
                ops.AppendFormat("入职状态由“{0}”改为“{1}”；", Tool.GetDescription((UserStatus)model.Status), Tool.GetDescription((UserStatus)m.Status));
            }
            if (m.WorkClass != model.WorkClass)
            {
                ops.AppendFormat("工种类型由“{0}”改为“{1}”；", model.WorkClass, m.WorkClass);
            }
            if (m.WorkName != model.WorkName)
            {
                ops.AppendFormat("工种由“{0}”改为“{1}”；", model.WorkName, m.WorkName);
            }
            if (m.CustomerID != model.CustomerID)
            {
                if (string.IsNullOrEmpty(paras.CustomerName))
                {
                    ops.AppendFormat("入职单位由“{0}”改为“{1}”；", model.CustomerName, paras.CustomerName);
                }
                else
                {
                    ops.AppendFormat("入职单位由“{0}”改为“{1}”；", model.CustomerID, m.CustomerID);
                }
            }
            var has = await entryService.ChangeEntry(m);
            if (has)
            {
                
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = m.ID;
                record.Status = model.Status.Value;
                record.About = ops.ToString();
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = userName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "人员操作失败，你无权修改！");
            }

        }
        /// <summary>
        /// 更改银行卡
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="work"></param>
        /// <param name="workClass"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [OperationLogAction("更改银行卡信息id:{id}")]
        public async Task<IActionResult> ChangeBank([FromForm] int id, [FromForm] string bankNumber, [FromForm] string bankName, [FromForm] string bankProvince, [FromForm] string bankCity)
        {
            if(string.IsNullOrEmpty(bankNumber) || string.IsNullOrEmpty(bankName))
            {
                throw new ErrorCodeException(-1, "请输入银行卡号或者所属行！");
            }
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower())
            {
                companyId = 0;
            }
            var model = await entryService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            if (companyId > 0 && model.CompanyID != companyId)
            {
                throw new ErrorCodeException(-1, "您无权查看！");
            }
            if (model.Status != (int)UserStatus.Entry && model.Status != (int)UserStatus.Leave)
            {
                throw new ErrorCodeException(-1, "未入职不可更改银行卡！");
            }
            var has = await entryService.ChangeBank(id, bankNumber, bankName, bankProvince, bankCity);
            if (has)
            {
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = model.ID;
                record.PerUserID = user.UserID;
                record.Status = model.Status.Value;
                record.About = "更改银行卡操作！银行卡（" + model.BankNumber + "）修改为("+ bankNumber + ")，所属行（" + model.BankName + "）修改为("+ bankName + ")";
                record.CID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "人员操作失败，你无权修改！");
            }

        }
        /// <summary>
        /// 面试未通过
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [OperationLogAction("操作面试未通过id:{m.ID}")]
        public async Task<IActionResult> IntFail([FromBody] Model.Live_UserEntry m)
        {
            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower())
            {
                companyId = 0;
            }
            //var status = m.Status == (int)UserStatus.NotInterviewed ? (int)UserStatus.NotInterviewed : (int)UserStatus.InterviewFailed;
            var status = m.Status.HasValue ? m.Status.Value : (int)UserStatus.InterviewFailed;
            var oldStatus = m.Status == (int)UserStatus.NotInterviewed || m.Status == (int)UserStatus.InterviewFailed ? (int)UserStatus.WaitInterview : (int)UserStatus.WaitEntry;
            var model = await entryService.Get(m.ID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            if (companyId > 0 && model.CompanyID != companyId)
            {
                throw new ErrorCodeException(-1, "您无权查看！");
            }
            if (model.Status == status)
            {
                throw new ErrorCodeException(-1, "该记录已处理！");
            }
            if (model.Status != oldStatus)
            {
                throw new ErrorCodeException(-1, "该记录未处于正常状态，不可操作！");
            }
            model.IntAbout = m.IntAbout;
            model.IntDate = m.IntDate;
            var has = await entryService.UpdateInt(model, status, companyId);
            if (has)
            {
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = model.ID;
                record.PerUserID = user.UserID;
                record.Status = status;
                record.About = " 面试操作！";
                record.CID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "人员操作失败，你无权修改！");
            }
            
        }
        /// <summary>
        /// 离职操作
        /// </summary>
        /// <param name="data"></param> 
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [OperationLogAction("操作离职")]
        public async Task<IActionResult> Leave([FromBody] Model.LeaveModel data)
        {

            var limitLeaveDay = new DateTime(2026, 10, 14);
            var dt = DateTime.Today.Date;
            if (dt == limitLeaveDay && DateTime.Now.Hour<16)
            {
                throw new ErrorCodeException(-1, "离职流程调整，暂不能处理离职，离职操作2026年10月14日16点（今日）恢复！");
            }

           


            var user = CurrentUser;
            var companyId = user.CompanyID;
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower())
            {
                companyId = 0;
            }
            var m = data.entry;
            if (m == null)
            {
                throw new ErrorCodeException(-1, "信息来源错误！");
            }
            if (data.leave == null)
            {
                throw new ErrorCodeException(-1, "请选择一项离职原因！");
            }
            var model = await entryService.Get(m.ID);
            if (model.InjuryStatus == 1)
            {
                throw new ErrorCodeException(-1, "工伤状态下不能离职！");
            }
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该员工不存在！");
            }
            if (companyId > 0 && model.CompanyID != companyId)
            {
                throw new ErrorCodeException(-1, "您无权查看！");
            }
            if (model.Status == (int)UserStatus.Leave)
            {
                throw new ErrorCodeException(-1, "该员工已离职！");
            }
            if (model.Status != (int)UserStatus.Entry)
            {
                throw new ErrorCodeException(-1, "该记录未处于入职状态，不可离职！");
            }
            //FiveNotEntry(model);
            model.LeaveTime = m.LeaveTime;
            model.LeaveAbout=m.LeaveAbout;
            var has = await entryService.UserLeave(model, data.leave, companyId);
            if (has)
            {
                var about = "小程序离职操作！";
                var userId = 0;
                if (user.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    userId = user.UserID;
                    var userInfo = await authService.UserInfo<Model.Live_Personnel>();
                    about = userInfo.FullName + "在小程序离职操作！";
                }
                else
                {
                    if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                    {
                        var userInfo = await authService.UserInfo<Model.SYS_CompanyUser>();
                        about = userInfo.FullName + "在后台离职操作！";
                    }
                    else if (user.PlatformType == Model.Enums.PlatformType.Admin)
                    {
                        about = "总后台离职操作！";
                    }
                    else
                    {
                        about = "后台离职操作(其他)！";
                    }
                }
                if (!string.IsNullOrEmpty(m.LeaveAbout))
                {
                    about += "离职原因：" + m.LeaveAbout;
                }
                var record = new EntryRecordEventData();
                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                record.EntryID = model.ID;
                record.PerUserID = userId;
                record.Status = (int)UserStatus.Leave;
                record.About = about;
                record.LeaveTime = m.LeaveTime;
                record.CID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
                record.OperateFrom = user.PlatformType;
                record.OperateFullName = user.FullName;
                record.OperateUserID = user.UserID;
                publisher.Publish(record);
                if (model.IsDayWork)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("jobID", model.JobDayWorkID.ToString());
                    dic.Add("number", "-1");
                    var result = await httpService.PostAsync(ServiceNames.CustomerService, "/api/DayWorkUserContractRecord/ChangeNumber", dic);
                }
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "离职操作失败，你无权修改！");
            }

        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_UserEntry model)
        {
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                model.CompanyID = CurrentUser.CompanyID;
                model.CompanyGID = CurrentUser.CompanyGID;
            }
            if (!await entryService.Exist(model.ID, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await entryService.Update(model);
            return Ok();
        }
       

        [HttpPost]
        public async Task<IActionResult> InjuryUpdate([FromBody] Live_UserEntry model)
        {
            
            var Vmodel = await entryService.View(model.ID, CurrentUser.CompanyID);
            var info = await authService.UserInfo<Model.Live_Personnel>();
            var paras = new Dictionary<string, string>();
            if(!info.PartnerID.HasValue)
            {
                throw new ErrorCodeException(-1, "账号没有关联服务商");
            }
            paras.Add("id", info.PartnerID.Value.ToString());
            Model.Live_Partner partner = new Live_Partner();
            var obj = await httpService.GetAsync(ServiceNames.BasicService, "/api/Partner/get", paras);
            if (!string.IsNullOrEmpty(obj.Data.ToString()))
            {
                partner = JsonConvert.DeserializeObject<Model.Live_Partner>(obj.Data.ToString());
            }
            //商保配置
            //var paras = new Dictionary<string, string>();
            //paras.Add("isEnabled", "1");
            //paras.Add("gid", Vmodel.CustomerGID.ToString());
            //var obj = await httpService.GetAsync(ServiceNames.CustomerService, "/api/InsuranceCustomerConfig/list", paras);
            //List<Model.Live_V_InsuranceCustomerConfig> configs = new List<Live_V_InsuranceCustomerConfig>();
            //if (!string.IsNullOrEmpty(obj.Data.ToString()))
            //{
            //    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
            //    configs = JsonConvert.DeserializeObject<List<Model.Live_V_InsuranceCustomerConfig>>(ii["items"].ToString());
            //}


            Model.Live_InjuryInfo injury = new Model.Live_InjuryInfo();
            injury.UserEntryID = model.ID;
            injury.InjuryGID = Guid.NewGuid();
            injury.ProcessStatus = 0;
            injury.CompanyID = CurrentUser.CompanyID;
            injury.CompanyGID = CurrentUser.CompanyGID;
            
            injury.CustomerID = Vmodel.CustomerID;
            injury.CustomerGID = Vmodel.CustomerGID;
            injury.CustomerName = Vmodel.CustomerName;
            injury.CustomerAddress = Vmodel.CustomerAddress;

            injury.UserGID = Vmodel.UserGID;
            injury.UserName = Vmodel.Name;
            injury.UserCardNo = Vmodel.CardID;
            injury.UserPhone = Vmodel.Phone;
            injury.UserBank = Vmodel.BankName;
            injury.UserBankNo = Vmodel.BankID; 
            injury.UserSex = Vmodel.Sex;
            injury.UserRegisteredResidence = Vmodel.Address;
            injury.UserWorkType = Vmodel.WorkClass;
            injury.UserType = Vmodel.WorkName;

            injury.PartnerID = partner.PartnerID;
            injury.PartnerGID = partner.PartnerGID;
            injury.PartnerName = partner.PartnerName;

            injury.SealCompanyGID = Vmodel.SigningUnitGID;
            injury.SealCompanyName = Vmodel.SignName;
            injury.SealCompanyNo = Vmodel.SignCompanyNo;
            injury.SealCompanyPhone = Vmodel.SignPhone;
            injury.SealCompanyAddress = Vmodel.SignAddress;
            injury.SealCompanyBank = Vmodel.SignBankName;
            injury.SealCompanyBankNo = Vmodel.SignBankNo;

            injury.CooperationCompanyGID = Vmodel.CooperationUnitGID;
            injury.CooperationCompanyName = Vmodel.CooperationName;

            injury.AgentName = Vmodel.AgentName;
            injury.AgentIdcardNo = Vmodel.AgentIdCardNo;
            injury.AgentBank = Vmodel.AgentBank;
            injury.AgentBankNo = Vmodel.AgentBankNo;


            

            






            //工伤只获取线上员工（IsUser为真），签章单位和商务合作表（Live_Enterprise）

            //商保信息保存在入职表Live_UserEntry,通过联表形式获取商保1和商保2的配置（目前只有2个配置)

            //代理信息是直接在Live_Customer创建字段保存
            injury.CreateTime = DateTime.Now;

            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                model.CompanyID = CurrentUser.CompanyID;
                model.CompanyGID = CurrentUser.CompanyGID;
            }
            if (!await entryService.Exist(model.ID, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var i = await entryService.Update(model);
            var id = await injuryInfoService.Add(injury);
            return Ok(new { id });
        }

        [HttpPost]
        public async Task<IActionResult> InjuryDelete([FromBody] Model.Live_UserEntry model)
        {
            //取消标记后，进入工伤撤回状态，记录保留
            var expable = Expressionable.Create<Model.Live_InjuryInfo>();
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            expable.And(it => it.ProcessStatus!=8 && it.UserEntryID==model.ID);
            var info = await injuryInfoService.View(expable.ToExpression());
            if(info !=null)
            {
                info.ProcessStatus = 9;
                await injuryInfoService.UpdateStatus(info.ID,9,"小程序取消标记");
            }
            var exp = expable.ToExpression();
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                model.CompanyID = CurrentUser.CompanyID;
                model.CompanyGID = CurrentUser.CompanyGID;
            }
            if (!await entryService.Exist(model.ID, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var i = await entryService.Update(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        [OperationLogAction("删除入职信息id：{id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            var model  = await entryService.Get(id,user.CompanyID);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            if(model.Status==(int)UserStatus.Entry || model.Status == (int)UserStatus.WaitEntry || model.Status == (int)UserStatus.WaitInterview)
            {
                throw new ErrorCodeException(-1, "该入职记录的状态不可删除！");
            }
            await entryService.Delete(id);
            return Ok();
        }
       
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [HttpGet]
        public async Task<IActionResult> GetDayOver()
        {
            var user = CurrentUser;
            var entry = await entryService.View(it => it.UserID == user.UserID && it.Status == (int)UserStatus.Entry);
            if (entry != null)
            {
                var now = DateTime.Now;
                if (entry.IsDayOver)//日结工
                {
                    var daywork = new Model.WorkSignUser();
                    daywork.ID = entry.ID;
                    if (entry.IsDaySign)
                    {
                        var days = (now - entry.DayOverStartTime.Value).Days;
                        if(days<0 || days > 1)
                        {
                            entry.IsDaySign=false;
                        }
                    }
                    daywork.IsDaySign = entry.IsDaySign;
                    if (entry.Longitude.HasValue)
                    {
                        daywork.Latitude = entry.Latitude.ToString();
                        daywork.Longitude = entry.Longitude.ToString();
                    }
                    daywork.DayEndTime = entry.DayEndTime;
                    daywork.DayStartTime = entry.DayStartTime;
                    daywork.NightEndTime = entry.NightEndTime;
                    daywork.NightStartTime = entry.NightStartTime;
                    daywork.CompanyName = entry.CompanyName;
                    var startTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd")+" "+ entry.NightStartTime);
                    var endTime = Convert.ToDateTime((entry.NightEndTime.IndexOf("24:") > -1 ? now.AddDays(1).ToString("yyyy-MM-dd") : now.ToString("yyyy-MM-dd")) + " " + entry.NightEndTime.Replace("24:", "00:"));
                    daywork.IsNight = now >= startTime && now <= endTime;
                    if (daywork.IsDaySign)
                    {
                        daywork.DayOverStartTime = entry.DayOverStartTime.HasValue ? entry.DayOverStartTime.Value.ToString("hh:mm") : "未打卡";
                        if (entry.DayOverEndTime.HasValue)
                        {
                            if (daywork.IsNight)
                            {
                                daywork.IsDaySign = false;
                                daywork.DayOverStartTime = "未打卡";
                                daywork.DayOverEndTime = "未打卡";
                            }
                            else
                            {
                                daywork.DayOverEndTime = entry.DayOverEndTime.Value.ToString("hh:mm");
                            }
                        }
                        else
                        {
                            daywork.DayOverEndTime = "未打卡";

                        }
                    }
                    else
                    {
                        daywork.DayOverStartTime = "未打卡";
                        daywork.DayOverEndTime = "未打卡";
                    }
                    return Ok(daywork);
                }
            }
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [HttpGet]
        public async Task<IActionResult> GetUserWork()
        {
            var user = CurrentUser;
            var entry = await entryService.View(it=>it.UserID==user.UserID&&it.Status==(int)UserStatus.Entry);
            if (entry != null)
            {
                var isAppMoney = (DateTime.Now - entry.EntryTime.Value).TotalDays >= entry.EntryDays;
                return Ok(new { customerName = entry.CustomerName, isAppMoney, isDayOver = entry.IsDayOver,ID=entry.ID,appEntryDays= entry.EntryDays, AttendanceType = entry.AttendanceType });
            }
            else
            {
                return Ok(false);
            }
        }
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        /// <summary>
        /// 更新日结签到
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> WorkSign([FromForm] int id)
        {
            var model = await entryService.View(it => it.ID==id);
            if (model != null)
            {
                if (model.Status != (int)UserStatus.Entry)
                {
                    throw new ErrorCodeException(-1, "您未入职无法签到！");
                }
                if (!model.IsDaySign)
                {
                    throw new ErrorCodeException(-1, "您是非日结工，不可签到！");
                }
                var now = DateTime.Now;
                if (model.IsDaySign)
                {
                    var days = (now - model.DayOverStartTime.Value).Days;
                    if (days < 0 || days > 1)
                    {
                        model.IsDaySign = false;
                    }
                }
                if (model.IsDaySign)
                {
                    throw new ErrorCodeException(-1, "您已签到！");
                }
                else
                {
                    await entryService.UpdateDayOver(now, null, true);
                    return Ok(DateTime.Now.ToString("hh:mm"));
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "您未入职！");
            }

        }
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        /// <summary>
        /// 更新日结签到
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Model.RequestResultModel> WorkOver([FromForm] int id)
        {
            var model = await entryService.View(it => it.ID == id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "您未入职！");
            }
            if (model.Status != (int)UserStatus.Entry)
            {
                throw new ErrorCodeException(-1, "您未入职无法签到！");
            }
            if (!model.IsDaySign)
            {
                throw new ErrorCodeException(-1, "您是非日结工，不可签到！");
            }
            var now = DateTime.Now;
            var diff = now - model.DayOverStartTime.Value;
            var day = diff.Days;
            if (model.IsDaySign && (day == 0 || day == 1))
            {
                if (diff.Hours == 0)
                {
                    await entryService.UpdateDayOver(null, now, false);
                    return new Model.RequestResultModel() { Code=-1,Message= "下工成功，由于你上工不足1小时不结算工资！",Data= DateTime.Now.ToString("hh:mm") };
                }
                var cmodel = await httpService.GetAsync<Model.Live_Customer>(ServiceNames.CustomerService , "/api/customer/get?id=" + model.CustomerID);
                if (cmodel == null)
                {
                    throw new ErrorCodeException(-1, "所属公司错误！");
                }
                var mmodel = new Model.Live_PrepaidApplyRecord();
                mmodel.CustomerGID = model.CustomerGID.Value;
                mmodel.CreateTime = now;
                mmodel.ApplyType = PrepaidApplyEnumType.日结工资申请;
                mmodel.UserID = model.UserID;
                var startTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd") + " " + model.DayEndTime);
                var endTime = Convert.ToDateTime((model.DayEndTime.IndexOf("24:") > -1 ? now.AddDays(1).ToString("yyyy-MM-dd") : now.ToString("yyyy-MM-dd")) + " " + model.DayEndTime.Replace("24:", "00:"));
                var startHours = (model.DayOverStartTime.Value - startTime).Hours;
                var endHours = (endTime- now).Hours;
                mmodel.WorkType = (startHours >= 0 || startHours >= -1) && endHours>=-1 ? 1 : 2;
                mmodel.ShiftEndTime = now;
                mmodel.ShiftStartTime = model.DayOverStartTime;
                mmodel.UserEntryID = model.ID;
                mmodel.Status = PrepaidRecordStatusEnumType.待结算;
                mmodel.PerUserID = model.PerUserID;
                var hours = (mmodel.ShiftEndTime.Value - mmodel.ShiftStartTime.Value).Hours;
                var allhours = Convert.ToDecimal((mmodel.ShiftEndTime.Value - mmodel.ShiftStartTime.Value).TotalHours);
                var diffhours = allhours - hours;
                if (diffhours >= 0.5M)
                {
                    if (diffhours >= 0.9M && diffhours * 60 >= 59)
                    {
                        allhours = hours + 1;
                    }
                    else
                    {
                        allhours = hours + 0.5M;
                    }
                }
                if (mmodel.WorkType == 1)
                {
                    if (allhours > cmodel.DayShiftHours.Value) allhours = cmodel.DayShiftHours.Value;
                    mmodel.ShiftHours = allhours;
                    mmodel.LunchHours = cmodel.LunchHours;
                    mmodel.ShiftMoney = cmodel.DayShiftMoney;
                    mmodel.NightShiftSubsidy = 0;
                    mmodel.DinnerHours = cmodel.DinnerHours;
                    mmodel.MoneyHours = mmodel.ShiftHours - mmodel.LunchHours - mmodel.DinnerHours;
                }
                else
                {
                    if (allhours > cmodel.NightShiftHours.Value) allhours = cmodel.NightShiftHours.Value;
                    mmodel.ShiftHours = allhours;
                    mmodel.DinnerHours = cmodel.DinnerHours;
                    mmodel.ShiftMoney = cmodel.NightShiftMoney;
                    mmodel.NightShiftSubsidy = cmodel.NightShiftSubsidy;
                    mmodel.MoneyHours = mmodel.ShiftHours - mmodel.DinnerHours;
                }
                mmodel.ApplyAmount = (int)Math.Round(mmodel.ShiftMoney.Value * mmodel.MoneyHours.Value + mmodel.NightShiftSubsidy.Value);
              
                var res = await httpService.PostAsync(ServiceNames.FinanceService, "/api/prepaidapplyrecord/apply", JsonConvert.SerializeObject(mmodel));
                if(res!=null && res.Code == 200)
                {
                    await entryService.UpdateDayOver(null, now, true);
                }
                else
                {
                    throw new ErrorCodeException(-1, "生成日结记录失败！");
                }
                return new Model.RequestResultModel() { Code = -1, Message = "下工成功！", Data = DateTime.Now.ToString("hh:mm") };
            }
            else
            {
                throw new ErrorCodeException(-1, "您未签到！");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserEntryListByUserID(int userId)
        {
            var models = await entryService.List(f=>f.UserID== userId, new UserPagedModel() { Page = 0 });
            return Ok(models);
        }


        [HttpGet]
        public async Task<IActionResult> GetUserEntryContract(int userEntryId)
        {
            var models = await entryService.GetUserEntryContract(userEntryId);
            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserEntryListBySalaryMonth(Guid customerGID,DateTime salaryMonth)
        {
            // 工资所属月在职过的员工
            var salaryNextMonth = salaryMonth.AddMonths(1);
            var exp = Expressionable.Create<Live_V_UserEntry>();
            exp.And(f => f.CustomerGID == customerGID);
            exp.And(f => f.EntryTime.HasValue && f.EntryTime.Value < salaryNextMonth && (f.LeaveTime == null || f.LeaveTime.Value >= salaryMonth));
            var models = await entryService.List(exp.ToExpression(), new UserPagedModel() { Page = 0 });
            return Ok(models);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetZeroSalaryUserEntryList(Guid customerGID)
        {
            // 获取工资所属月的员工信息
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var lastMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var exp = Expressionable.Create<Live_V_UserEntry>();
            exp.And(f => f.CustomerGID == customerGID);
            exp.And(f => f.EntryTime.HasValue && f.EntryTime.Value <currentMonth && (f.LeaveTime == null || f.LeaveTime.Value >= currentMonth));
            var models = await entryService.List(exp.ToExpression(), new UserPagedModel() { Page = 0 });
            return Ok(models);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUserEntryListByInsuranceStartDate(Guid customerGID, DateTime startDate)
        {

            var leavedateMin = startDate.AddMonths(-1);
            var exp = Expressionable.Create<Live_V_UserEntry>();
            exp.And(f => f.CustomerGID == customerGID);
            exp.And(f => (f.EntryTime.HasValue && f.EntryTime.Value < startDate) &&(f.LeaveTime==null||f.LeaveTime.Value>= leavedateMin));
            var models = await entryService.List(exp.ToExpression(), new UserPagedModel() { Page = 0 });
            return Ok(models);
        }




        [HttpPut]
        public async Task<IActionResult> SocialMark([FromForm] int id, [FromForm] bool mark)
        {
            if (id == 0) throw new ErrorCodeException(-1, "信息输入错误");
            var objs = new Dictionary<string, object>();
            objs.Add("ID", id);
            objs.Add("SocialMark", mark);
            objs.Add("SocialStatus", 1);
            var count = await entryService.DictionarySet(objs);
            if (count > 0)
            {
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "操作失败");
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> UpdateSocial(Model.EntrySocial.UpdateEntrySocialModel social)
        {
            string msg = "";
            try
            {
                List<Model.Live_UserEntry> list = new List<Live_UserEntry>();
                UserPagedModel query = new UserPagedModel();
                query.Page = 0;
                var expable = Expressionable.Create<Model.Live_UserEntry>();

                if(social.entryID.HasValue)
                {
                    expable.And(t => t.ID == social.entryID);
                } 
                else
                {
                    expable.And(t => t.SocialID == social.SocialID);
                }
               
                var item = await entryService.Get(expable.ToExpression());
                if (item != null)
                {
                    Model.Live_UserEntry model = new Live_UserEntry();
                    model.ID = item.ID;
                    model.SocialID = social.SocialID;
                    model.SocialStatus = social.Status;
                    model.SocialGID = social.SocialGID;
                    list.Add(model);
                    await entryService.UpdateSocial(list);
                    msg = "同步成功";
                } else
                {
                    var datajson = JsonConvert.SerializeObject(social);
                    msg = "没有关联客户";
                }
                
                
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return Ok(new { message = msg });
        }


        [HttpPost]
        public async Task<IActionResult> CancelSocial(Model.EntrySocial.UpdateEntrySocialModel social)
        {
            string msg = "";
            try
            {
                List<Model.Live_UserEntry> list = new List<Live_UserEntry>();
                UserPagedModel query = new UserPagedModel();
                query.Page = 0;
                var expable = Expressionable.Create<Model.Live_UserEntry>();

                expable.And(t => t.SocialID == social.SocialID);

                var item = await entryService.Get(expable.ToExpression());
                if (item != null)
                {
                    var objs = new Dictionary<string, object>();
                    objs.Add("ID", item.ID);
                    objs.Add("SocialMark", false);
                    objs.Add("SocialStatus", 0);
                    var count = await entryService.DictionarySet(objs);
                    msg = "取消标记成功";
                }
                else
                {
                    var datajson = JsonConvert.SerializeObject(social);
                    msg = "没有关联客户";
                }

            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return Ok(new { message = msg });
        }


        [HttpPut]
        public async Task<IActionResult> UpdateManuallyInsurance([FromForm]int id)
        {
            string msg = "";
            try
            {
                var model = await entryService.Get(f => f.ID == id);
                if (model==null)
                {
                    throw new ErrorCodeException(-1, "入职信息获取失败，请刷新后重试！");
                }
                model.IsManuallyInsurance = !model.IsManuallyInsurance;
              await  entryService.UpdateManuallyInsurance(model);

            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return Ok(new { message = msg });
        }



        #region 商保增减员
        [HttpGet]
        public async Task<IActionResult> ListUserEntryInsurancePageList( string name,string customerName,string signUnitName, string insuranceCompany, string planName, string cardID,string workName,int? status, UserEntryPlanStatusEnumType? planStatus, InsuranceProcessEnumType? processStatus, bool? isManuallyInsurance, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_V_UserEntryInsuracneStatus>();
            exp.And(f => f.IsUseBusInsurance && f.CusEnable && (f.Status == 2||f.Status==4));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName1.Contains(planName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany1.Contains(insuranceCompany));
            exp.AndIF(isManuallyInsurance.HasValue, f => f.IsManuallyInsurance== isManuallyInsurance.Value);
            exp.AndIF(processStatus.HasValue, f => f.InsuranceProcessStatus== processStatus.Value);
            exp.AndIF(planStatus.HasValue, f => f.PlanStatus1== planStatus.Value);
            exp.AndIF(status.HasValue, f => f.Status== status.Value);
            var models = await entryService.UserEntryInsurancePageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserEntryInsurance(int id)
        {
            var model = await entryService.GetUserEntryInsurance(f=>f.ID==id);
            return Ok(model);
        }
        //

        [HttpGet]
        public async Task<IActionResult> ExportUserEntryInsuranceList(string name, string customerName, string signUnitName, string insuranceCompany, string planName, string cardID, string workName, int? status, UserEntryPlanStatusEnumType? planStatus, InsuranceProcessEnumType? processStatus, bool? isManuallyInsurance)
        {

            var exp = Expressionable.Create<Live_V_UserEntryInsuracneStatus>();
            exp.And(f => f.IsUseBusInsurance && f.CusEnable && (f.Status == 2 || f.Status == 4));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(signUnitName), f => f.SignUnitName.Contains(signUnitName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName1.Contains(planName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany1.Contains(insuranceCompany));
            exp.AndIF(isManuallyInsurance.HasValue, f => f.IsManuallyInsurance == isManuallyInsurance.Value);
            exp.AndIF(processStatus.HasValue, f => f.InsuranceProcessStatus == processStatus.Value);
            exp.AndIF(planStatus.HasValue, f => f.PlanStatus1 == planStatus.Value);
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            var models = await entryService.ListInsurance(exp.ToExpression());

            var exportDataList = models.Select(f => new EntryBusInsuranceRecordExcelTemp()
            {
                CustomerName = f.CustomerName,
                InsuranceCompany = f.InsuranceCompany1,
                IsManuallyInsurance = f.IsManuallyInsurance ? "是" : "否",
                PlanName = f.PlanName1,
                WorkName = f.WorkName,
                SignUnitName = f.SignUnitName,
                Status = f.Status == 2 ? "已入职" : "已离职",
                CardID = f.CardID,
                EntryDate = f.EntryTime?.ToString("yyyy-MM-dd"),
                LeaveDate = f.LeaveTime?.ToString("yyyy-MM-dd"),
                Name = f.Name,
                OrderNo = f.InsuranceId1,
                PlanStatus = f.PlanStatus1.ToString(),
                ProcessStatus = f.InsuranceProcessStatus.ToString(),
                WorkClass = f.WorkClass
            }).OrderBy(f => f.CustomerName).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "参保人员信息");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "参保人员信息.xlsx" });
        }

        [HttpPut]
        public async Task<IActionResult> BusinessRecordAddOrDel([FromForm]int id, [FromForm] bool isAdd)
        {
            var model = await entryService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            BusinessInsuranceAddOrDelEventData eventData = new BusinessInsuranceAddOrDelEventData()
            {
                IsAdd = isAdd,
                UserEntryID = id
            };
            publisher.Publish(eventData);


            return Ok();
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateBusinessInsurance([FromForm] int id, [FromForm] string workName, [FromForm] string workClass, [FromForm] string planName1, [FromForm] string planName2, [FromForm] UserEntryPlanStatusEnumType? planStatus1, [FromForm] UserEntryPlanStatusEnumType? planStatus2)
        {
            var model = await entryService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }


            model.WorkClass = workClass;
            model.WorkName = workName;
            model.PlanName1 = planName1;
            model.PlanName2 = planName2;
            model.PlanStatus1 = planStatus1;
            model.PlanStatus2 = planStatus2;
            await entryService.UpdateBusInsurance(model);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateInsuranceProcessStatus([FromForm] int id, [FromForm] InsuranceProcessEnumType processStatus)
        {
            var model = await entryService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            model.InsuranceProcessStatus = processStatus;
            await entryService.UpdateInsuranceProcessStatus(model);
            return Ok();
        }


        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateBusInsuranceInfo([FromForm] int id, [FromForm] string workName, [FromForm] string workClass, [FromForm] string insuranceCompany1, [FromForm] string insuranceId1, [FromForm] string planName1, [FromForm] UserEntryPlanStatusEnumType planStatus1, [FromForm] InsuranceProcessEnumType insuranceProcessStatus, [FromForm] InsuranceTypeEnumType insuranceType, [FromForm] bool isManuallyInsurance)
        {
            var model = await entryService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            model.WorkClass = workClass;
            model.WorkName = workName;
            model.InsuranceId1 = insuranceId1;
            model.InsuranceCompany1 = insuranceCompany1;
            model.PlanStatus1 = planStatus1;
            model.InsuranceProcessStatus = insuranceProcessStatus;
            model.PlanName1 = planName1;
            model.IsManuallyInsurance = isManuallyInsurance;
            model.InsuranceType = insuranceType;
            await entryService.UpdateInsuranceInfo(model);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> InjuryUserEntryInsuranceList([FromQuery] UserPagedModel query)
        {
            var exp = Expressionable.Create<Live_V_UserEntryInsuracneStatus>();
            exp.And(f => f.IsUseBusInsurance && f.CusEnable && f.InsuranceType != InsuranceTypeEnumType.不参保 && f.PlanStatus1== UserEntryPlanStatusEnumType.已增员);
            exp.AndIF(query.Status.HasValue, f => f.Status == query.Status);
            var models = await entryService.UserEntryInsuranceList(exp.ToExpression());
            return Ok(new { items = models });
        }

        #endregion

        #region 实名
        [HttpPut]
        public async Task<IActionResult> RealName( [FromForm]int id)
        {
           var entry=await entryService.Get(id);
            RealName3EventData eventData = new RealName3EventData(entry.Name, entry.CardID, entry.Phone);
            publisher.Publish(eventData);
            return Ok();
        }

        #endregion


        #region 生成合同

        [HttpPut]
        public async Task<IActionResult> GenerateEntryContract( [FromForm]int id)
        {
            var model = await entryService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要操作的数据不存在");
            }
            UserEntryContractRecordEventData eventData = new UserEntryContractRecordEventData(id, model.Status==2 ? ContractEnumType.入职合同 : ContractEnumType.离职合同, Guid.NewGuid());
            publisher.Publish(eventData);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateContract([FromForm] int id,[FromForm]DateTime? contractDate,[FromForm] DateTime? contractStartDate, [FromForm]DateTime? contractEndDate,[FromForm] DateTime? retireDate, [FromForm] ContractStatus contractStatus)
        {
            var model = await entryService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要操作的数据不存在");
            }
            model.ContractDate = contractDate;
            model.ContractStartDate = contractStartDate;
            model.ContractEndDate = contractEndDate;
            model.RetireDate = retireDate;
            model.ContractStatus =(int)contractStatus;
            await entryService.UpdateContract(model);
            return Ok();
        }


        #endregion

        #region 获取离职原因

        [HttpGet]
        public async Task<IActionResult> ListLeaveReason( int id)
        {
            string msg = "";
            var reasonList=await  entryService.ListLeaveReason(id);
            return Ok(reasonList);
        }
        #endregion

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ffsssff()
        //{
        //     var ss=new CashSignRecordEventData() { 
        //      UserEntryID= 5858,
        //      UserID= 34835
        //     };
        //    publisher.Publish(ss);

        //    return Ok();
        //}

        #region 员工的年度请假配置+批量导入在职员工的请假配置
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> AttendanceCreate()
        {
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            var puser = await authService.UserInfo<Model.SYS_CompanyUser>();
            var data = DateTime.Now;
            //{ id: 1, name: '客服', enabled: true },
            //{ id: 2, name: '服务商', enabled: true },
            //{ id: 3, name: '管理员', enabled: true },
            //{ id: 4, name: '财务', enabled: true },
            //{ id: 5, name: '人事', enabled: false }
            if (puser.UserType == 2)
            {
                expable.And(t => t.PartnerID == puser.PartnerID);
            }
            expable.And(t => SqlFunc.Subqueryable<Model.Live_AttendanceStructure>().Where(s => s.CustomerID == t.CustomerID).Any());
            expable.And(t => t.Status == 2);
            var exp = expable.ToExpression();
            var models = await entryService.List(exp, query);

            return Ok(new { items = models});

        }
        #endregion


        #region 修改CA 状态


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangeCaAuthStatus(CaAuthCardIdDto dto)
        {
            var entryList = await entryService.List(f => f.CardID == dto.CardID);
            if (entryList.Any())
            {
                await entryService.ChangeCaAuth(entryList.Select(f => f.ID).ToList(), dto.IsCaAuth);
            }
            var userList = await userService.List(f => f.CardID == dto.CardID, new UserPagedModel() { Page = 0 });
            if (userList.Any())
            {
                await userService.ChangeCaAuth(userList.Select(f => f.UserID).ToList(), dto.IsCaAuth);
            }
            return Ok();
        }

        #endregion


        #region 入职保险公司限制

        private async Task<InsuranceEntryLimitResult> IsCanEntry(string customerName, string workName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(workName))
                {
                    return new InsuranceEntryLimitResult(true, string.Empty, "参数缺失,跳过验证!", "");
                }

                var workPlan = await busInsuranceWorkPlanService.Get(f => f.CustomerName == customerName && f.WorkName.Trim() == workName.Trim()&&f.IsEnable);
                if (workPlan == null || string.IsNullOrWhiteSpace(workPlan.InsuranceCompany))
                {
                    return new InsuranceEntryLimitResult(true, string.Empty, "工种未配置方案,跳过验证!", "");
                }

                var companyName = workPlan.InsuranceCompany;
                var insuranceCompany = await busInsuranceCompanyService.Get(f => f.IsEnable && f.IsLimitEntry && f.InsuranceCompany == companyName);
                if (insuranceCompany == null)
                {
                    return new InsuranceEntryLimitResult(true, companyName, "保险公司未配置入职限制!", "");
                }

                var timeRange = $"{insuranceCompany.StartTime:hh\\:mm}-{insuranceCompany.EndTime:hh\\:mm}";
                var entryRule = insuranceCompany.IsWorkDay ? $"仅工作日 {timeRange} 可入职" : $"{timeRange} 可入职";

                var now = DateTime.Now;
                if (insuranceCompany.IsWorkDay && !await attendanceCalendarService.IsWorkDaty(now.Date))
                {
                    return new InsuranceEntryLimitResult(false, companyName, entryRule, "非工作日不可入职");
                }

                var time = new TimeSpan(now.Hour, now.Minute, now.Second);
                if (time < insuranceCompany.StartTime || time > insuranceCompany.EndTime)
                {
                    return new InsuranceEntryLimitResult(false, companyName, entryRule, $"当前时间不在允许入职时间 {timeRange} 内");
                }

                return new InsuranceEntryLimitResult(true, companyName, entryRule, string.Empty);
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, $"保险公司入职限制验证异常:{ex.Message}");
            }
        }

        #endregion
    }


}
