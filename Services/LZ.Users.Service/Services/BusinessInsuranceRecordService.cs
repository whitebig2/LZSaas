using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.WxOpen.AdvancedAPIs.DataCube;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Users.Service.Interface;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using Consul;

namespace LZ.Users.Service.Services
{
    public class BusinessInsuranceRecordService : ServicesBase<Live_BusinessInsuranceRecord>, IBusinessInsuranceRecordService, IScopedDependency
    {

        private readonly IEntryService _entryService;
        private readonly IBusinessInsuranceConfigService _configService;
        private readonly IBusinessInsuranceOrderService _orderService;
        private readonly ISendMQService _sendMQService;

        public BusinessInsuranceRecordService(IEntryService entryService, IBusinessInsuranceConfigService configService, IBusinessInsuranceOrderService orderService, ISendMQService sendMQService)
        {
            _entryService = entryService;
            _configService = configService;
            _orderService = orderService;
            _sendMQService = sendMQService;
        }

        public async Task<List<Live_BusinessInsuranceRecord>> PageList(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BusinessInsuranceRecord>> List(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<List<Live_V_BusinessInsuranceRecord>> ListView(Expression<Func<Live_V_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusinessInsuranceRecord> Get(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceRecord>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<int> Add(Live_BusinessInsuranceRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_BusinessInsuranceRecord> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }


        public async Task<bool> Update(Live_BusinessInsuranceRecord model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_BusinessInsuranceRecord> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }



        public async Task<bool> Exist(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceRecord>().AnyAsync(exp);
        }



        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BusinessInsuranceRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Deleteable<Live_BusinessInsuranceRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ManageUserEntryPlan(int userEntryID)
        {
            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var userEntry = await _entryService.Get(userEntryID);
            var configList = await _configService.ListView(f => f.CustomerGID == userEntry.CustomerGID&&f.IsEnabled);

            if (userEntry.Status==2)
            {
                if (configList.Any(f => f.IsMain))
                {
                    var mainConfig = configList.First(f => f.IsMain);
                    if (userEntry.WorkClass == "3类")
                    {
                        userEntry.PlanName1 = mainConfig.Plan3;
                    }
                    else if (userEntry.WorkClass == "4类")
                    {
                        userEntry.PlanName1 = mainConfig.Plan4;
                    }
                    else if (userEntry.WorkClass == "5类")
                    {
                        userEntry.PlanName1 = mainConfig.Plan5;
                    }
                    userEntry.InsuranceCompany1 = mainConfig.InsuranceCompany;
                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                }
                if (configList.Any(f => !f.IsMain))
                {
                    var config = configList.First(f => !f.IsMain);
                    if (userEntry.WorkClass == "3类")
                    {
                        userEntry.PlanName2 = config.Plan3;
                    }
                    else if (userEntry.WorkClass == "4类")
                    {
                        userEntry.PlanName2 = config.Plan4;
                    }
                    else if (userEntry.WorkClass == "5类")
                    {
                        userEntry.PlanName2 = config.Plan5;
                    }

                    userEntry.InsuranceCompany2 = config.InsuranceCompany;
                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                }

                var msg = new {Type= "insuranceEmpAdd", Title= "已投保待生效",Name= userEntry.Name, CardID=userEntry.CardID};
                _sendMQService.SendStatisticsMessage(msg.ToCamelCaseJson());

            }
            else if (userEntry.Status==4)
            {
                if (userEntry.PlanStatus1 != null && userEntry.PlanStatus1.Value >= Model.Enums.UserEntryPlanStatusEnumType.已增员)
                {
                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.已减员;
                }
                if (userEntry.PlanStatus2 != null && userEntry.PlanStatus2.Value >= Model.Enums.UserEntryPlanStatusEnumType.已增员)
                {
                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.已减员;
                }


                var msg = new { Type = "insuranceEmpDel", Title = "已减员待生效", Name = userEntry.Name, CardID = userEntry.CardID };
                _sendMQService.SendStatisticsMessage(msg.ToCamelCaseJson());
            }

            return true;

        }

        //public async Task<bool> ManageUserEntryPlan(int userEntryID)
        //{
        //    var today = DateTime.Today;
        //    var nextDay = today.AddDays(1);
        //    var userEntry = await _entryService.Get(userEntryID);


        //    Common.StaticLog.Debug($"商保增减员记录！UserEntryID：{userEntry}-工种：{userEntry.WorkName}-职业类别：{ userEntry.WorkClass}" );

        //    var configList = await _configService.ListView(f => f.CustomerGID == userEntry.CustomerGID && f.IsEnabled);
        //    string planName = string.Empty;
        //    List<Live_BusinessInsuranceRecord> addList = new List<Live_BusinessInsuranceRecord>();
        //    List<Live_BusinessInsuranceRecord> updateList = new List<Live_BusinessInsuranceRecord>();

        //    Live_BusinessInsuranceRecord record;


        //    foreach (var configItem in configList)
        //    {
        //        if (configItem.IsMain)
        //        {
        //            if (userEntry.PlanStatus1 == Model.Enums.UserEntryPlanStatusEnumType.未参保)
        //            {
        //                continue;
        //            }
        //        }
        //        else
        //        {
        //            if (userEntry.PlanStatus2 == Model.Enums.UserEntryPlanStatusEnumType.未参保)
        //            {
        //                continue;
        //            }
        //        }
               
        //        record = null;
        //        planName = string.Empty;
        //        if (userEntry.WorkClass == "3类")
        //        {
        //            planName = configItem.Plan3;
        //        }
        //        else if (userEntry.WorkClass == "4类")
        //        {
        //            planName = configItem.Plan4;
        //        }
        //        else if (userEntry.WorkClass == "5类")
        //        {
        //            planName = configItem.Plan5;
        //        }
        //        // 异常处理
        //        if (string.IsNullOrWhiteSpace(planName))
        //        {
        //            if (configItem.IsMain)
        //            {
        //                userEntry.PlanName1 = planName;
        //                if (userEntry.Status == 2)
        //                {
        //                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.增员异常;
        //                }
        //                else if (userEntry.Status == 4)
        //                {
        //                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.减员异常;
        //                }
        //            }
        //            else
        //            {
        //                userEntry.PlanName2 = planName;
        //                if (userEntry.Status == 2)
        //                {
        //                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.增员异常;
        //                }
        //                else if (userEntry.Status == 4)
        //                {
        //                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.减员异常;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (configItem.IsMain)
        //            {

        //                userEntry.PlanName1 = planName;

        //            }
        //            else
        //            {
        //                userEntry.PlanName2 = planName;
        //            }

        //        }


        //        string insuranceCompany = configItem.InsuranceCompany;
        //        if (!string.IsNullOrWhiteSpace(configItem.InsuranceCompanyOther))
        //        {
        //            var workNameList = configItem.InsuranceCompany.Split(',');
        //            if (workNameList.Contains(userEntry.WorkName))
        //            {
        //                insuranceCompany = configItem.InsuranceCompany;
        //            }
        //            else {
        //                insuranceCompany = configItem.InsuranceCompanyOther;
        //            }
        //        }

        //        if (string.IsNullOrWhiteSpace(userEntry.WorkName))
        //        {
                    
        //        }
        //        // 增员
        //        if (userEntry.Status == 2)
        //            {
        //            // 获取未推送的保单全部作废
        //            var pushNoList = await List(f =>
        //          f.UserEntryID == userEntry.ID
        //          && (f.InsuranceCompany == configItem.InsuranceCompany || f.InsuranceCompany == configItem.InsuranceCompanyOther)
        //           && f.IdCardNo == userEntry.CardID
        //           && f.Status < Model.Enums.BusinessInsuranceStatusEnumType.已增员
        //          );

        //            if (pushNoList.Any())
        //            {
        //                foreach (var item in pushNoList)
        //                {
        //                    item.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
        //                }

        //                updateList.AddRange(pushNoList);
        //            }
        //            //  同一个保险公司 同一个身份证号码 已参保
        //            var existList = await List(f =>
        //            f.UserEntryID == userEntry.ID
        //            && (f.InsuranceCompany == configItem.InsuranceCompany || f.InsuranceCompany == configItem.InsuranceCompanyOther)
        //             && f.IdCardNo == userEntry.CardID
        //             && f.StartDate <= nextDay
        //             && f.EndDate >= today
        //             && f.Status < Model.Enums.BusinessInsuranceStatusEnumType.已减员
        //             && f.Status >= Model.Enums.BusinessInsuranceStatusEnumType.已增员
        //            );
        //            // 记录存在
        //            if (existList.Any())
        //            {
        //                foreach (var item in existList)
        //                {
        //                    if (item.CustomerName == configItem.CustomerName && item.InsuranceSigningUnitName == configItem.InsuranceSigningUnitName && item.WorkClass == userEntry.WorkClass && item.WorkName == userEntry.WorkName)
        //                    {
        //                        if (record == null)
        //                        {
        //                            item.Status = Model.Enums.BusinessInsuranceStatusEnumType.已增员;
        //                            record = item;
        //                        }
        //                        else
        //                        {
        //                            // 相同的方案只能有一个，需要作废其它记录
        //                            record.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        item.Status = Model.Enums.BusinessInsuranceStatusEnumType.待减员匹配;
        //                    }
        //                }
        //                updateList.AddRange(existList);
        //            }
        //            var ageSex = IdCardHelper.GetAgeAndGender(userEntry.CardID);
        //            if (record == null)
        //            {
        //                record = new Live_BusinessInsuranceRecord()
        //                {
        //                    PlanName = planName,
        //                    AddMatchRecordID = null,
        //                    AddPushTime = null,
        //                    Status = Model.Enums.BusinessInsuranceStatusEnumType.待增员匹配,
        //                    AddSysTime = DateTime.Now,
        //                    Age = ageSex.Age,
        //                    Sex = ageSex.Gender,
        //                    CustomerGID = configItem.CustomerGID,
        //                    CustomerName = configItem.CustomerName,
        //                    InsuranceCompany = configItem.InsuranceCompany,
        //                    InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
        //                    WorkClass = userEntry.WorkClass,
        //                    WorkName = userEntry.WorkName,
        //                    CreateTime = DateTime.Now,
        //                    EndDate = DateTime.MinValue,
        //                    EntryDate = userEntry.EntryTime.Value.Date,
        //                    DelPushTime = null,
        //                    DelSysTime = null,
        //                    IdCardNo = userEntry.CardID,
        //                    UserID = userEntry.UserID ?? 0,
        //                    UserEntryID = userEntry.ID,
        //                    InsuranceNo = string.Empty,
        //                    DelMatchRecordID = null,
        //                    InsuranceSigningUnitID = configItem.InsuranceSigningUnitID,
        //                    IsDayOrder = false,
        //                    IsExpired = false,
        //                    IsMain = configItem.IsMain,
        //                    LeaveDate = userEntry.LeaveTime,
        //                    Name = userEntry.Name,
        //                    OrderNo = string.Empty,
        //                    StartDate = DateTime.MinValue,
        //                    ID = Guid.NewGuid(),
        //                    IsDefaultPush = configItem.IsDefaultPush,
        //                    InsuranceId = string.Empty,
        //                    PlanNameId = 0,
        //                    BusinessInsurancePushID = null
        //                };
        //                if (configItem.InsuranceCompany=="大地财险"&&!string.IsNullOrWhiteSpace(configItem.WorkNames))
        //                {
        //                    var workNameList= configItem.WorkNames.Split(',');
        //                    if (workNameList.Contains(record.WorkName))
        //                    {
        //                        record.InsuranceCompany = configItem.InsuranceCompany;
        //                    }
        //                    else {
        //                        record.InsuranceCompany = configItem.InsuranceCompanyOther;
        //                        record.PlanName = "90万+9万";
        //                    }
        //                }
        //                addList.Add(record);
        //            }
        //            if (configItem.IsMain)
        //            {
        //                if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.待增员匹配)
        //                {
        //                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
        //                }
        //                else if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.已增员)
        //                {
        //                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.已增员;
        //                }
        //                else {
        //                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.增员异常;
        //                }
        //            }
        //            else {
        //                if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.待增员匹配)
        //                {
        //                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
        //                }
        //                else if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.已增员)
        //                {
        //                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.已增员;
        //                }
        //                else
        //                {
        //                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.增员异常;
        //                }
        //            }
        //        }
        //        else if (userEntry.Status == 4)
        //        {
        //            // 作废所有未提送的
        //            var pushNoList = await List(f =>
        //                f.UserEntryID == userEntry.ID
        //                && (f.InsuranceCompany == configItem.InsuranceCompany || f.InsuranceCompany == configItem.InsuranceCompanyOther)
        //                 && f.IdCardNo == userEntry.CardID
        //                 && f.Status < Model.Enums.BusinessInsuranceStatusEnumType.已增员
        //                );

        //            foreach (var item in pushNoList)
        //            {
        //                item.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
        //                updateList.Add(item);
        //            }
        //            var pushSucList = await List(f =>
        //                f.UserEntryID == userEntry.ID
        //                && (f.InsuranceCompany == configItem.InsuranceCompany|| f.InsuranceCompany == configItem.InsuranceCompanyOther)
        //                 && f.IdCardNo == userEntry.CardID
        //                 && f.Status >= Model.Enums.BusinessInsuranceStatusEnumType.已增员
        //                    && f.Status <= Model.Enums.BusinessInsuranceStatusEnumType.待减员
        //                 && f.StartDate.Value <= nextDay
        //                 && f.EndDate.Value >= today
        //                );
        //            if (pushSucList.Any())
        //            {
        //                foreach (var item in pushSucList)
        //                {
        //                    item.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
        //                    updateList.Add(item);
        //                }
        //                pushSucList.First().Status = Model.Enums.BusinessInsuranceStatusEnumType.待减员匹配;
        //            }
        //            if (configItem.IsMain)
        //            {
        //                if (pushSucList.Any())
        //                {
        //                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待减员;
        //                }
        //                else 
        //                {
        //                    userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
        //                }
        //            }
        //            else
        //            {
        //                if (pushSucList.Any())
        //                {
        //                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.待减员;
        //                }
        //                else
        //                {
        //                    userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
        //                }
        //            }
        //        }
        //    }
        //    base.Context.BeginTran();
        //    if (addList.Any())
        //    {
        //        await Add(addList);
        //    }
        //    if (updateList.Any())
        //    {
        //        await Update(updateList);
        //    }

        //    if (userEntry.PlanStatus1==null )
        //    {
        //        userEntry.PlanName1 = string.Empty;
        //        userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
        //    }

        //    if (userEntry.PlanStatus2 == null)
        //    {
        //        userEntry.PlanName2 = string.Empty;
        //        userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
        //    }
        //    await  _entryService.Update(userEntry);

        //    base.Context.CommitTran();
        //    return true;

        //}


        public async Task<bool> ManageUserEntryPlanRecord(int userEntryID)
        {
            var today = DateTime.Today;
            var nextDay = today.AddDays(1);
            var userEntry = await _entryService.Get(userEntryID);
            var configList = await _configService.ListView(f => f.CustomerGID == userEntry.CustomerGID && f.IsEnabled);
            string planName = string.Empty;
            List<Live_BusinessInsuranceRecord> addList = new List<Live_BusinessInsuranceRecord>();
            List<Live_BusinessInsuranceRecord> updateList = new List<Live_BusinessInsuranceRecord>();

            Live_BusinessInsuranceRecord record;
            foreach (var configItem in configList)
            {
                if (configItem.IsMain)
                {
                    if (userEntry.PlanStatus1 == Model.Enums.UserEntryPlanStatusEnumType.未参保)
                    {
                        continue;
                    }
                }
                else
                {
                    if (userEntry.PlanStatus2 == Model.Enums.UserEntryPlanStatusEnumType.未参保)
                    {
                        continue;
                    }
                }
                record = null;
                planName = string.Empty;
                if (userEntry.WorkClass == "3类")
                {
                    planName = configItem.Plan3;
                }
                else if (userEntry.WorkClass == "4类")
                {
                    planName = configItem.Plan4;
                }
                else if (userEntry.WorkClass == "5类")
                {
                    planName = configItem.Plan5;
                }
                // 异常处理
                if (string.IsNullOrWhiteSpace(planName))
                {
                    if (configItem.IsMain)
                    {

                        userEntry.PlanName1 = planName;

                        if (userEntry.Status == 2)
                        {
                            userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.增员异常;
                        }
                        else if (userEntry.Status == 4)
                        {
                            userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.减员异常;
                        }
                    }
                    else
                    {
                        userEntry.PlanName2 = planName;
                        if (userEntry.Status == 2)
                        {
                            userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.增员异常;
                        }
                        else if (userEntry.Status == 4)
                        {
                            userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.减员异常;
                        }
                    }
                }
                else
                {
                    if (configItem.IsMain)
                    {

                        userEntry.PlanName1 = planName;

                    }
                    else
                    {
                        userEntry.PlanName2 = planName;
                    }

                }
                // 增员
                if (userEntry.Status == 2||userEntry.Status==4)
                {
                    // 获取未推送的保单全部作废
                    var pushNoList = await List(f =>
                  f.UserEntryID == userEntry.ID
                  && f.InsuranceCompany == configItem.InsuranceCompany
                   && f.IdCardNo == userEntry.CardID
                   && f.Status < Model.Enums.BusinessInsuranceStatusEnumType.已增员
                  );

                    if (pushNoList.Any())
                    {
                        foreach (var item in pushNoList)
                        {
                            item.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
                        }

                        updateList.AddRange(pushNoList);
                    }
                    //  同一个保险公司 同一个身份证号码 可以参保
                    var existList = await List(f =>
                    f.UserEntryID == userEntry.ID
                    && f.InsuranceCompany == configItem.InsuranceCompany
                     && f.IdCardNo == userEntry.CardID
                     && f.StartDate <= today
                     && f.EndDate >= today
                     && f.Status < Model.Enums.BusinessInsuranceStatusEnumType.已减员
                     && f.Status >= Model.Enums.BusinessInsuranceStatusEnumType.已增员
                    );
                    // 记录存在
                    if (existList.Any())
                    {
                        foreach (var item in existList)
                        {
                            if (item.CustomerName == configItem.CustomerName && item.InsuranceSigningUnitName == configItem.InsuranceSigningUnitName && item.WorkClass == userEntry.WorkClass && item.WorkName == userEntry.WorkName)
                            {
                                if (record == null)
                                {
                                    item.Status = Model.Enums.BusinessInsuranceStatusEnumType.已增员;
                                    record = item;
                                }
                                else
                                {
                                    // 相同的方案只能有一个，需要作废其它记录
                                    record.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
                                }
                            }
                            else
                            {
                                item.Status = Model.Enums.BusinessInsuranceStatusEnumType.待减员匹配;
                            }
                        }
                        updateList.AddRange(existList);
                    }
                    var ageSex = IdCardHelper.GetAgeAndGender(userEntry.CardID);
                    if (record == null)
                    {
                        record = new Live_BusinessInsuranceRecord()
                        {
                            PlanName = planName,
                            AddMatchRecordID = null,
                            AddPushTime = null,
                            Status = Model.Enums.BusinessInsuranceStatusEnumType.待增员匹配,
                            AddSysTime = DateTime.Now,
                            Age = ageSex.Age,
                            Sex = ageSex.Gender,
                            CustomerGID = configItem.CustomerGID,
                            CustomerName = configItem.CustomerName,
                            InsuranceCompany = configItem.InsuranceCompany,
                            InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
                            WorkClass = userEntry.WorkClass,
                            WorkName = userEntry.WorkName,
                            CreateTime = DateTime.Now,
                            EndDate = DateTime.MinValue,
                            EntryDate = userEntry.EntryTime.Value.Date,
                            DelPushTime = null,
                            DelSysTime = null,
                            IdCardNo = userEntry.CardID,
                            UserID = userEntry.UserID ?? 0,
                            UserEntryID = userEntry.ID,
                            InsuranceNo = string.Empty,
                            DelMatchRecordID = null,
                            InsuranceSigningUnitID = configItem.InsuranceSigningUnitID,
                            IsDayOrder = false,
                            IsExpired = false,
                            IsMain = configItem.IsMain,
                            LeaveDate = userEntry.LeaveTime,
                            Name = userEntry.Name,
                            OrderNo = string.Empty,
                            StartDate = DateTime.MinValue,
                            ID = Guid.NewGuid(),
                            IsDefaultPush = configItem.IsDefaultPush,
                            InsuranceId = string.Empty,
                            PlanNameId = 0,
                            BusinessInsurancePushID = null
                        };
                        addList.Add(record);
                    }
                    if (configItem.IsMain)
                    {
                        if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.待增员匹配)
                        {
                            userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                        }
                        else if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.已增员)
                        {
                            userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.已增员;
                        }
                    }
                    else
                    {
                        if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.待增员匹配)
                        {
                            userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.待增员;
                        }
                        else if (record.Status == Model.Enums.BusinessInsuranceStatusEnumType.已增员)
                        {
                            userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.已增员;
                        }
                    }
                }
                else if (userEntry.Status == 4)
                {
                    // 作废所有未提送的
                    var pushNoList = await List(f =>
                        f.UserEntryID == userEntry.ID
                        && f.InsuranceCompany == configItem.InsuranceCompany
                         && f.IdCardNo == userEntry.CardID
                         && f.Status < Model.Enums.BusinessInsuranceStatusEnumType.已增员
                        );

                    foreach (var item in pushNoList)
                    {
                        item.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
                        updateList.Add(item);
                    }
                    var pushSucList = await List(f =>
                        f.UserEntryID == userEntry.ID
                        && f.InsuranceCompany == configItem.InsuranceCompany
                         && f.IdCardNo == userEntry.CardID
                         && f.Status >= Model.Enums.BusinessInsuranceStatusEnumType.已增员
                            && f.Status <= Model.Enums.BusinessInsuranceStatusEnumType.待减员
                         && f.StartDate.Value <= today
                         && f.EndDate.Value >= today
                        );
                    if (pushSucList.Any())
                    {
                        foreach (var item in pushSucList)
                        {
                            item.Status = Model.Enums.BusinessInsuranceStatusEnumType.作废;
                            updateList.Add(item);
                        }
                        pushSucList.First().Status = Model.Enums.BusinessInsuranceStatusEnumType.待减员匹配;
                    }
                    if (configItem.IsMain)
                    {
                        if (pushSucList.Any())
                        {
                            userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.待减员;
                        }
                        else
                        {
                            userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
                        }
                    }
                    else
                    {
                        if (pushSucList.Any())
                        {
                            userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.待减员;
                        }
                        else
                        {
                            userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
                        }
                    }
                }
            }
            itenant.BeginTran();
            if (addList.Any())
            {
                await Add(addList);
            }
            if (updateList.Any())
            {
                await Update(updateList);
            }

            if (userEntry.PlanStatus1 == null)
            {
                userEntry.PlanName1 = string.Empty;
                userEntry.PlanStatus1 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
            }

            if (userEntry.PlanStatus2 == null)
            {
                userEntry.PlanName2 = string.Empty;
                userEntry.PlanStatus2 = Model.Enums.UserEntryPlanStatusEnumType.未参保;
            }
            await _entryService.Update(userEntry);

            itenant.CommitTran();
            return true;

        }
    }
}
