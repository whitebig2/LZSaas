using LZ.Common.Extensions;
using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.EventData.Insurance;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class EntryService(IEventPublisher publisher, IUserService userService, IInjuryInfoService injuryInfoService, IRedisService redisService) : ServicesBase<Live_UserEntry>, IEntryService, IScopedDependency
    {
        public async Task<List<Live_V_UserEntry>> List(Expression<Func<Live_V_UserEntry, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            if ((query.From == "lzmini" || query.From == "spmini") && query.Status == (int)UserStatus.Leave)
            {
                qb.OrderByDescending(it => it.ID);
            }
            else
            {
                qb.OrderByDescending(it => it.ID);
            }
            //Expression<Func<Live_V_UserEntry, Live_V_UserEntry>> selectExp = it => new Live_V_UserEntry() { Name = it.Name, Sex = it.Sex,Age=it.Age,CustomerID=it.CustomerID,CustomerName=it.CustomerName,PersonnelName=it.PersonnelName,Picture=it.Picture,PartnerID=it.PartnerID,ID=it.ID,GID=it.GID,PartnerName=it.PartnerName,CompanyName=it.CompanyName,FromType=it.FromType,PerUserID=it.PerUserID,PerPartnerID=it.PerPartnerID,Phone=it.Phone,LastNote=it.LastNote,LeaveTime=it.LeaveTime,EntryTime=it.EntryTime,WorkClass=it.WorkClass,WorkName= it.WorkName,CreateTime=it.CreateTime,UserID= it.UserID,UserGID= it.UserGID,IntAbout= it.IntAbout,IntDate= it.IntDate,Status=it.Status,IsCaAuth= it.IsCaAuth,IsReal=it.IsReal,CardID= it.CardID, StructureID=it.StructureID, JDYDataID= it.JDYDataID };
            Expression<Func<Live_V_UserEntry, Live_V_UserEntry>> selectExp = it => new Live_V_UserEntry();
            if (query.Top>0)
            {
                return await qb.Take(query.Top).ToListAsync();
            }
            else if(query.Page == 0)
            {
                return await qb.ToListAsync();
            }
            var list = await qb.ToPageListAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_UserEntry>> List(Expression<Func<Live_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_UserEntry>();
            qb.Where(exp);
            qb.OrderByDescending(it => it.ID);
            return await qb.ToListAsync();

        }


        public async Task<List<Live_V_UserEntry>> ListView(Expression<Func<Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.OrderByDescending(it => it.EntryTime.Value);
            return await qb.ToListAsync();

        }
        public async Task<int> Add(Live_UserEntry model)
        {
            model.GID = Guid.NewGuid();
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<List<int>> Add(List<Live_UserEntry> models)
        {
            return await base.Context.Insertable(models).ExecuteReturnPkListAsync<int>();
        }
        /// <summary>
        /// 入职操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isDirectEntry">是否直接入职</param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        //public async Task<bool> UserEntry(Live_UserEntry model, bool isDirectEntry)
        //{
        //    if (model.IsUser && model.UserID.HasValue)
        //    {
        //        var expable = Expressionable.Create<Model.Live_UserEntry>();
        //        expable.And(it => it.UserID == model.UserID && it.Status == (int)UserStatus.Entry);
        //        var exp = expable.ToExpression();
        //        var oldModel = await Get(exp);
        //        if (oldModel != null)
        //        {
        //            if (isDirectEntry)
        //            {
        //                //await UserLeave(oldModel);
        //                var record = new EntryRecordEventData();
        //                record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
        //                record.EntryID = oldModel.ID;
        //                record.PerUserID = model.PerUserID.HasValue ? model.PerUserID.Value : 0;
        //                record.Status = (int)UserStatus.Leave;
        //                record.About = "该员工已在其他地方入职！";
        //                record.LeaveTime = DateTime.Today;
        //                publisher.Publish(record);
        //            }
        //            else
        //            {
        //                throw new ErrorCodeException(-1, "该员工已入职，请先将该员工做离职！");
        //            }
        //        }
        //    }
        //    model.Status = (int)UserStatus.Entry;
        //    model.ContractStatus = (int)ContractStatus.EntryWaitCreate;
        //    if (model.IsUser && model.UserID.HasValue)
        //    {
        //        await userService.ChangeStatus(model.UserID.Value, model.Status.Value);
        //    }
        //    return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => new Live_UserEntry() { Status = model.Status, ContractStatus = model.ContractStatus, EntryTime = model.EntryTime, FromType = model.FromType, WorkClass = model.WorkClass, WorkName = model.WorkName, CustomerGID = model.CustomerGID, CustomerID = model.CustomerID, ContractDate = model.ContractDate }).Where(it => it.ID == model.ID).ExecuteCommandAsync() > 0;
        //}
        /// <summary>
        /// 入职操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isDirectEntry">是否直接入职</param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<int> UserEntryAffairs(Live_UserEntry model, bool isDirectEntry, ICurrentUser user = null)
        {

            model.Status = (int)UserStatus.Entry;
            var db = itenant;
            var isSend = false;
            Live_UserEntry oldModel = null;
            var result = await db.UseTranAsync(async () =>
            {
                if (model.IsUser && model.UserID.HasValue)
                {
                    model.ContractStatus = (int)ContractStatus.EntryWaitCreate;
                    oldModel = await db.QueryableWithAttr<Model.Live_UserEntry>().FirstAsync(it => (it.UserID == model.UserID || it.CardID == model.CardID) && !SqlFunc.IsNullOrEmpty(it.CardID) && it.Status == (int)UserStatus.Entry && it.CompanyID == model.CompanyID);
                    if (oldModel != null)
                    {
                        if (oldModel.CustomerID == model.CustomerID && oldModel.IsUser)
                        {
                            return -1;
                            //throw new ErrorCodeException(-1, "不可在同一家单位重复入职！");
                        }
                        if (isDirectEntry)
                        {
                            isSend = await db.UpdateableWithAttr<Model.Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { Status = (int)UserStatus.Leave, LeaveTime =DateTime.Today }).Where(it => it.ID == oldModel.ID && it.Status == (int)UserStatus.Entry).ExecuteCommandAsync() > 0;
                        }
                        else
                        {
                            return -2;
                            //throw new ErrorCodeException(-1, "该员工已入职，请先将该员工做离职！");
                        }
                    }
                    await db.UpdateableWithAttr<Model.Live_User>().SetColumns(it => new Model.Live_User() { Status = (int)UserStatus.Entry, IsDayOver = model.IsDayOver, EntryID = 0, IsShowInfo=true }).Where(it => it.UserID == model.UserID).ExecuteCommandAsync();
                }
                if (model.ID == 0)
                {
                    model.GID = Guid.NewGuid();
                    await db.InsertableWithAttr(model).ExecuteCommandIdentityIntoEntityAsync();
                }
                else
                {
                    await db.UpdateableWithAttr(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                    //await db.Updateable<Model.Live_UserEntry>().SetColumns(it => new Live_UserEntry() { Status = model.Status, ContractStatus = model.ContractStatus, EntryTime = model.EntryTime, FromType = model.FromType, WorkClass = model.WorkClass, WorkName = model.WorkName, CustomerGID = model.CustomerGID, CustomerID = model.CustomerID, ContractDate = model.ContractDate, HandUserID = model.HandUserID, OperateModel = model.OperateModel, JointPartnerID = model.JointPartnerID, JointPartnerGID = model.JointPartnerGID, IsDayOver = model.IsDayOver }).Where(it => it.ID == model.ID).ExecuteCommandAsync();
                }

                return model.ID;
            });
            if (result.Data>0)
            {
                if (isSend)
                {
                    if (oldModel.IsUser && oldModel.UserID.HasValue && model.ContractStatus == (int)ContractStatus.EntryComplete)//当天离职入职不推送离职合同消息
                    {
                        var contact = new UserEntryContractRecordEventData(oldModel.ID, ContractEnumType.离职合同, Guid.NewGuid());
                        publisher.Publish(contact);
                    }
                    await redisService.IncrByAsync(CoreRedisConstants.DayTotalChangeCompany);//更换单位的统计数据
                    var record = new EntryRecordEventData();
                    record.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                    record.EntryID = oldModel.ID;
                    record.PerUserID = model.PerUserID.HasValue ? model.PerUserID.Value : 0;
                    record.Status = (int)UserStatus.Leave;
                    record.About = "该员工已在其他地方入职！";
                    record.LeaveTime = DateTime.Today;
                    record.CID = oldModel.CustomerID.HasValue ? oldModel.CustomerID.Value : 0;
                    if (user != null)
                    {
                        record.OperateFrom = user.PlatformType;
                        record.OperateFullName = user.FullName+"(自动)";
                        record.OperateUserID = user.UserID;
                    }
                    publisher.Publish(record);
                    var insurance = new BusinessInsuranceAddOrDelEventData();//发送商保是否增减员
                    insurance.UserEntryID = oldModel.ID;
                    insurance.IsAdd = false;
                    publisher.Publish(insurance);
                }
                var entryInsurance = new BusinessInsuranceAddOrDelEventData();//发送商保是否增减员
                entryInsurance.UserEntryID = model.ID;
                entryInsurance.IsAdd = true;
                publisher.Publish(entryInsurance);
                var entryContact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.入职合同, Guid.NewGuid());
                publisher.Publish(entryContact);
                //var userevent = new UserCustomerEventData();
                //userevent.UserID = model.UserID.HasValue ? model.UserID.Value : 0;
                //userevent.EntryID = model.ID;
                //userevent.CustomerID = model.CustomerID.HasValue ? model.CustomerID.Value : 0;
                //publisher.Publish(userevent);
            }
            else if (result.Data == -1)
            {
                throw new ErrorCodeException(-1, "不可在同一家单位重复入职！");
            }
            else if (result.Data == -2)
            {
                throw new ErrorCodeException(-1, "该员工已入职，请先将该员工做离职！");
            }
            return result.Data;
        }
        /// <summary>
        /// 离职操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<bool> UserLeave(Live_UserEntry model, List<Live_UserLeaveReason> leaves, int companyId)
        {
            var statuss = new int[] { 8, 9 };
            if (await injuryInfoService.Exist(it => it.UserEntryID == model.ID && !statuss.Contains(it.ProcessStatus.Value) && it.CreateTime.Value.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM")))
            {
                throw new ErrorCodeException(-1, "该员工已存在当月工伤记录，不能离职！");
            }
            // 离职则立即修改参保状态为已减员
            if (model.PlanStatus1 != null && model.PlanStatus1.Value == UserEntryPlanStatusEnumType.已增员)
            {
                model.PlanStatus1 = UserEntryPlanStatusEnumType.已减员;
            }
            else {
                model.PlanStatus1 = UserEntryPlanStatusEnumType.未参保;
            }

            if (model.PlanStatus2 != null && model.PlanStatus2.Value == UserEntryPlanStatusEnumType.已增员)
            {
                model.PlanStatus2 = UserEntryPlanStatusEnumType.已减员;
            }
            else
            {
                model.PlanStatus2 = UserEntryPlanStatusEnumType.未参保;
            }
            if (!string.IsNullOrEmpty(model.Tabs) && model.Tabs.IndexOf(TabsEnum.五类过渡参保.ToString())>-1)//离职后去除五类过渡参保标签
            {
                model.Tabs = model.Tabs.Replace($",{TabsEnum.五类过渡参保.ToString()}", "");
            }
            var qb = base.Context.Updateable<Model.Live_UserEntry>();
            qb.SetColumns(it => new Model.Live_UserEntry() { Status = (int)UserStatus.Leave, LeaveTime = model.LeaveTime,LeaveAbout=model.LeaveAbout, PlanStatus1 = model.PlanStatus1, PlanStatus2 = model.PlanStatus2,Tabs= model.Tabs });
            qb.Where(it => it.ID == model.ID);
            if (companyId > 0)
            {
                qb.Where(it => it.CompanyID == companyId);
            }
            var has = await qb.ExecuteCommandAsync() > 0;
            if (has)
            {
                if (model.IsUser && model.UserID.HasValue)
                {
                    await userService.ChangeStatus(model.UserID.Value, (int)UserStatus.Leave);
                    if (model.ContractStatus==(int)ContractStatus.EntryComplete)//当天离职入职不推送离职合同消息
                    {
                        var contact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.离职合同, Guid.NewGuid());
                        publisher.Publish(contact);
                    }
                }
                if (leaves.Count > 0)
                {
                    await base.Context.Insertable(leaves).ExecuteCommandAsync();
                }
                //var insurance = new BusinessInsuranceAddOrDelEventData();//发送商保是否增减员
                //insurance.UserEntryID = model.ID;
                //insurance.IsAdd = false;
                //publisher.Publish(insurance);
            }

            return has;
        }
        

        /// <summary>
        /// 更改工种
        /// </summary>
        /// <param name="id"></param>
        /// <param name="work"></param>
        /// <param name="workClass"></param>
        /// <returns></returns>
        public async Task<bool> ChangeWork(int id, string work,string workClass)
        {
            var statuss = new int[] {8,9 };
            if(await injuryInfoService.Exist(it=>it.UserEntryID==id && !statuss.Contains(it.ProcessStatus.Value) && it.CreateTime.Value.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM")))
            {
                throw new ErrorCodeException(-1, "该员工已存在当月工伤记录，不能更改工种！");
            }
            var has = await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { WorkName = work, WorkClass = workClass, InsuranceProcessStatus= InsuranceProcessEnumType.待匹配,PlanName1=string.Empty,InsuranceCompany1=string.Empty }).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                //var insurance = new BusinessInsuranceAddOrDelEventData();//发送商保是否增减员
                //insurance.UserEntryID = id;
                //insurance.IsAdd = true;
                //publisher.Publish(insurance);
            }

            return has;
        }
        /// <summary>
        /// 批量更改入职信息
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BatchChangeEntryInfo(List<Model.Live_UserEntry> models, Expression<Func<Model.Live_UserEntry, object>> columns)
        {
            var has = await base.Context.Updateable(models).UpdateColumns(columns).ExecuteCommandAsync() > 0;

            return has;
        }
        /// <summary>
        /// 更改银行卡
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bankNumber"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public async Task<bool> ChangeBank(int id, string bankNumber, string bankName,string bankProvince, string bankCity)
        {
            var has = await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { BankNumber = bankNumber, BankName = bankName,BankCity=bankCity,BankProvince= bankProvince }).Where(it => it.ID == id).ExecuteCommandAsync() > 0;

            return has;
        }
        /// <summary>
        /// 更改银行所属地
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bankNumber"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public async Task<bool> ChangeBankArea(int id, string bankProvince, string bankCity)
        {
            var has = await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { BankCity = bankCity, BankProvince = bankProvince }).Where(it => it.ID == id).ExecuteCommandAsync() > 0;

            return has;
        }
        /// <summary>
        /// 更改银行所属地
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bankNumber"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public async Task<bool> ChangeUserBankArea(int userId, string bankProvince, string bankCity)
        {
            var status = new int[] { (int)Model.Enums.UserStatus.WaitEntry, (int)Model.Enums.UserStatus.Entry };
            var has = await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { BankCity = bankCity, BankProvince = bankProvince }).Where(it => it.UserID == userId && status.Contains(it.Status.Value)).ExecuteCommandAsync() > 0;
            return has;
        }
        /// <summary>
        /// 更改入职信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> ChangeEntryInfo(Model.Live_UserEntry model)
        {
            var has = await base.Context.Updateable(model).UpdateColumns(it=>new { it.PartnerID,it.PartnerGID,it.PerUserID,it.FromType,it.StructureID}).Where(it => it.ID == model.ID).ExecuteCommandAsync() > 0;

            return has;
        }
        /// <summary>
        /// 更改入职信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> ChangeEntry(Model.Live_UserEntry model)
        {
            var has = await base.Context.Updateable(model).IgnoreNullColumns(true).ExecuteCommandAsync() > 0;

            return has;
        }
        public async Task<List<Live_User>> PartnerUsers(Model.PagedModel.UserPagedModel query, Expression<Func<Live_User, bool>> exp)
        {
            var count = new RefAsync<int>();

            var sco = base.Context.Queryable<Live_User>().Where(exp);
            if (query.Page == 0)
            {
                return await sco.ToListAsync();

            }
            var users = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return users;
        }

        public async Task<List<Model.CountModel>> StatusCount(Expression<Func<Live_V_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserEntry>().Where(exp).GroupBy(it => it.Status).Select(it => new Model.CountModel() { FID = it.Status.Value, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<bool> Exist(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_UserEntry>().AnyAsync(exp);
        }
        public async Task<bool> Exist(Expression<Func<Live_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_UserEntry>().AnyAsync(exp);
        }
        /// <summary>
        /// 面试
        /// </summary>
        /// <param name="id"></param>
        /// <param name="intDate"></param>
        /// <param name="about"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInt(Model.Live_UserEntry m, int status, int companyId)
        {
            var qb = base.Context.Updateable<Model.Live_UserEntry>();
            if (status == (int)Model.Enums.UserStatus.WaitEntry || status == (int)Model.Enums.UserStatus.InterviewFailed)
            {
                qb.SetColumns(it => new Model.Live_UserEntry() { IntDate = m.IntDate, IntAbout = m.IntAbout, Status = status });
            }
            else
            {
                qb.SetColumns(it => it.Status == status);
            }
            qb.Where(it => it.ID == m.ID);
            if (companyId > 0)
            {
                qb.Where(it => it.CompanyID == companyId);
            }
            var has = await qb.ExecuteCommandAsync() > 0;
            if (has && m.IsUser && m.UserID.HasValue)
            {
                await userService.ChangeStatus(m.UserID.Value, status);
            }
            return has;
        }
        public async Task<bool> UpdateRemarks(int id, string remarks)
        {
            var has = await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.Remarks == remarks).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<bool> UpdateJDYDataID(int id, string jdyDataId)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.JDYDataID == jdyDataId).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateLastNote(int id, string remarks)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.LastNote == remarks).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateBank(Model.Live_User user)
        {
            var model = new Model.Live_UserEntry();
            model.BankImage = user.BankImage;
            model.BankNumber = user.BankNumber;
            model.BankName = user.BankName;
            model.BankID = user.BankID;
            model.BankCardName = user.BankCardName;
            model.BankCardType = user.BankCardType;
            model.BankCity = user.BankCity;
            model.BankProvince= user.BankProvince;
            var status = new int[] { (int)Model.Enums.UserStatus.WaitEntry, (int)Model.Enums.UserStatus.Entry };
            return await base.Context.Updateable(model).UpdateColumns(it => new {  it.BankNumber, it.BankName, it.BankID, it.BankCardName, it.BankCardType, it.BankCity, it.BankProvince }).Where(it => it.UserID == user.UserID && status.Contains(it.Status.Value)).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateEntryInfo(Model.Live_User user)
        {
            var model = new Model.Live_UserEntry();
            model.Name = user.FullName;
            model.Sex = user.Sex;
            model.Age = user.Age;
            model.Phone = user.Phone;
            model.IdcardFrontImage = user.IdcardFrontImage;
            model.IdcardBackImage = user.IdcardBackImage;
            model.BankImage = user.BankImage;
            model.IdcardAddress = user.IdcardAddress;
            model.Nation = user.Nation;
            model.Authority = user.Authority;
            model.CardStartDate = user.CardStartDate;
            model.CardEndDate = user.CardEndDate;
            model.BankNumber = user.BankNumber;
            model.BankName = user.BankName;
            model.BankID = user.BankID;
            model.BankCardName = user.BankCardName;
            model.BankCardType = user.BankCardType;
            model.MaritalStatus = user.MaritalStatus;
            model.MajorDiseases = user.MajorDiseases;
            model.EmergencyContact = user.EmergencyContact;
            model.EmergencyContactPhone = user.EmergencyContactPhone;
            model.CardID = user.CardID;
            model.IdPhoto = user.IdPhoto;
            model.IsBlack = user.IsBlack;
            model.CustomizeField = user.EntryCustomizeField;
            model.BankCity = user.BankCity;
            model.BankProvince = user.BankProvince;
            model.IsReal = user.IsReal;
            model.IsCaAuth = user.IsCaAuth;
            model.City = user.City;
            model.Province = user.Province;
            model.District = user.District;
            model.Address = user.Address;
            model.Education = user.Education;
            var status = new int[] { (int)Model.Enums.UserStatus.WaitEntry, (int)Model.Enums.UserStatus.Entry };
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            if (user.EntryID.HasValue && user.EntryID > 0)
            {
                expable.And(it => it.ID == user.EntryID && it.UserID == user.UserID);
            }
            else
            {
                expable.And(it => it.UserID == user.UserID && status.Contains(it.Status.Value));
            }
            var exp = expable.ToExpression();
            return await base.Context.Updateable(model).UpdateColumns(it => new { it.Name, it.Sex, it.Age, it.Phone, it.IdcardFrontImage, it.IdcardBackImage, it.BankImage, it.IdcardAddress, it.Nation, it.Authority, it.CardStartDate, it.CardEndDate, it.BankNumber, it.BankName, it.BankID, it.BankCardName, it.BankCardType, it.MaritalStatus, it.MajorDiseases, it.EmergencyContact, it.EmergencyContactPhone, it.CardID, it.IdPhoto, it.IsBlack, it.CustomizeField, it.BankCity, it.BankProvince,it.IsReal,it.IsCaAuth, it.City,it.Province,it.District,it.Address,it.Education }).Where(exp).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateCustomer(int id, string workClass, string workName, string fromType)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { WorkClass = workClass, WorkName = workName, FromType = fromType }).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> SetUser(string phone, int uid, int companyId)
        {
            var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
            var model = await Get(it => it.CompanyID == companyId && it.Phone == phone && (it.UserID == null || it.UserID == 0) && !it.IsUser && statuss.Contains(it.Status.Value));
            if (model != null)
            {
                await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.UserID == uid).SetColumns(it =>it.IsUser == true).Where(it => it.CompanyID == companyId && it.Phone == phone && (it.UserID == null || it.UserID == 0) && !it.IsUser && statuss.Contains(it.Status.Value)).ExecuteCommandAsync();
                await userService.ChangeStatus(uid, model.Status.Value);
                var record = new EntryRecordEventData();
                record.UserID = uid;
                record.EntryID = model.ID;
                record.PerUserID = 0;
                record.Status = model.Status.Value;
                record.About = "登录后关联了线下入职记录！";
                publisher.Publish(record);
                return true;
            }
            return false;
        }
        public async Task<bool> SetUser(Model.Live_User user, int companyId)
        {
            try
            {
                var model = await Get(it => it.CompanyID == companyId && it.CardID == user.CardID && (it.UserID == null || it.UserID == 0) && !it.IsUser && it.Status == (int)UserStatus.Entry);
                if (model != null)
                {
                    model.Name = user.FullName;
                    model.Sex = user.Sex;
                    model.Age = user.Age;
                    model.Phone = user.Phone;
                    model.IdcardFrontImage = user.IdcardFrontImage;
                    model.IdcardBackImage = user.IdcardBackImage;
                    model.BankImage = user.BankImage;
                    model.IdcardAddress = user.IdcardAddress;
                    model.Nation = user.Nation;
                    model.Authority = user.Authority;
                    model.CardStartDate = user.CardStartDate;
                    model.CardEndDate = user.CardEndDate;
                    model.BankNumber = user.BankNumber;
                    model.BankName = user.BankName;
                    model.BankID = user.BankID;
                    model.BankCardName = user.BankCardName;
                    model.BankCardType = user.BankCardType;
                    model.MaritalStatus = user.MaritalStatus;
                    model.MajorDiseases = user.MajorDiseases;
                    model.EmergencyContact = user.EmergencyContact;
                    model.IdPhoto = user.IdPhoto;
                    model.IsBlack = user.IsBlack;
                    model.CustomizeField = user.EntryCustomizeField;
                    model.BankCity = user.BankCity;
                    model.BankProvince = user.BankProvince;
                    model.UserID = user.UserID;
                    model.IsUser = true;
                    model.IsReal = user.IsReal;
                    model.IsCaAuth = user.IsCaAuth;
                    await base.Context.Updateable(model).UpdateColumns(it => new { it.Name, it.Sex, it.Age, it.Phone, it.IdcardFrontImage, it.IdcardBackImage, it.BankImage, it.IdcardAddress, it.Nation, it.Authority, it.CardStartDate, it.CardEndDate, it.BankNumber, it.BankName, it.BankID, it.BankCardName, it.BankCardType, it.MaritalStatus, it.MajorDiseases, it.EmergencyContact, it.EmergencyContactPhone, it.IdPhoto, it.IsBlack, it.CustomizeField, it.BankCity, it.BankProvince, it.UserID, it.IsUser,it.IsReal }).Where(it => it.ID == model.ID).ExecuteCommandAsync();
                    await userService.ChangeStatus(user.UserID, model.Status.Value);
                    var record = new EntryRecordEventData();
                    record.UserID = user.UserID;
                    record.EntryID = model.ID;
                    record.PerUserID = 0;
                    record.Status = model.Status.Value;
                    record.About = "实名后关联了线下入职记录！";
                    record.OperateUserID = user.UserID;
                    record.OperateFullName = user.FullName;
                    record.OperateFrom = PlatformType.XFZMini;
                    publisher.Publish(record);
                    var entryContact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.入职合同, Guid.NewGuid());
                    publisher.Publish(entryContact);
                    var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
                    var oldmodel = await Get(it => it.CompanyID == companyId && it.UserID == user.UserID && statuss.Contains(it.Status.Value) && it.ID!=model.ID);
                    if (oldmodel != null)
                    {
                        await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.Status == (int)UserStatus.Cancel).Where(it => it.ID == oldmodel.ID).ExecuteCommandAsync();
                        var oldrecord = new EntryRecordEventData();
                        oldrecord.UserID = user.UserID;
                        oldrecord.EntryID = oldmodel.ID;
                        oldrecord.PerUserID = 0;
                        oldrecord.Status = oldmodel.Status.Value;
                        oldrecord.About = "关联了线下入职记录后取消原记录！";
                        oldrecord.OperateUserID = user.UserID;
                        oldrecord.OperateFullName = user.FullName+"(自动)";
                        oldrecord.OperateFrom = PlatformType.XFZMini;
                        publisher.Publish(oldrecord);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return false;
            }
        }
        public async Task<bool> UpdateStatus(UserStatus status, Expression<Func<Model.Live_UserEntry, bool>> exp)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.Status == (int)status).Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateContractStatus(ContractStatus status, int id)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.ContractStatus == (int)status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateContractInfo(ContractStatus status, DateTime? contractDate, DateTime contractStartDate, DateTime contractEndDate, int id)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>()
                .SetColumns(it => it.ContractStatus == (int)status)
                    .SetColumns(it => it.ContractDate == contractDate)
                    .SetColumns(it => it.ContractStartDate == contractStartDate)
                    .SetColumns(it => it.ContractEndDate == contractEndDate)
                .Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateInjuryStatus(int injuryStatus, Expression<Func<Model.Live_UserEntry, bool>> exp)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => it.InjuryStatus == injuryStatus).Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(Live_UserEntry model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_UserEntry> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateDayOver(DateTime? DayOverStartTime, DateTime? DayOverEndTime, bool IsDaySign)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { DayOverEndTime = DayOverEndTime, DayOverStartTime = DayOverStartTime, IsDaySign = IsDaySign }).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id, int status, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Updateable<Live_UserEntry>().SetColumns(it => it.Status == status).Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_UserEntry> Get(Expression<Func<Live_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_UserEntry>().FirstAsync(exp);
        }
        public async Task<Live_V_UserEntry> View(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_V_UserEntry>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await View(exp);
        }
        public async Task<Live_V_UserEntry> View(Expression<Func<Live_V_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserEntry>().OrderByDescending(it=>it.ID).FirstAsync(exp);
        }

        public async Task<Live_V_UserEntry> View(Expression<Func<Live_V_UserEntry, bool>> exp,string strOrderByFileds)
        {
            return await base.Context.Queryable<Live_V_UserEntry>().OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds).FirstAsync(exp);
        }
        public async Task<Live_UserEntry> Get(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await Get(exp);
        }
        public async Task<bool> Delete(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Deleteable<Live_UserEntry>().Where(exp).ExecuteCommandAsync() > 0;
        }


        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_UserEntry>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public async Task<int> UpdateSocial(List<Model.Live_UserEntry> list)
        {
            return await base.Context.Updateable(list).UpdateColumns(t => new { t.SocialStatus, t.SocialID }).ExecuteCommandAsync();
        }

        public async Task<int> UpdateContract(Live_UserEntry model)
        {
            return await base.Context.Updateable(model).UpdateColumns(t => new { t.ContractStatus, t.RetireDate, t.ContractDate, t.ContractStartDate,t.ContractEndDate }).ExecuteCommandAsync();
        }

        public async Task<int> UpdateContract(List<Live_UserEntry> models)
        {
            return await base.Context.Updateable(models).UpdateColumns(t => new { t.RetireDate, t.ContractDate, t.ContractStartDate, t.ContractEndDate }).ExecuteCommandAsync();
        }


        public async Task<int> UpdateBusInsurance(List<Live_UserEntry> models)
        {
            return await base.Context.Updateable(models).UpdateColumns(t => new { t.PlanName1, t.PlanName2, t.PlanStatus1, t.PlanStatus2,t.InsuranceCompany1,t.InsuranceCompany2,t.InsuranceId1,t.InsuranceId2 }).ExecuteCommandAsync();
        }

   
       

        public async Task<int> UpdateBusInsurance(Live_UserEntry model)
        {
            return await base.Context.Updateable(model).UpdateColumns(t => new {t.WorkClass,t.WorkName, t.PlanName1, t.PlanName2, t.PlanStatus1, t.PlanStatus2 ,t.InsuranceCompany1,t.InsuranceCompany2}).ExecuteCommandAsync();
        }

        public async Task<int> UpdateManuallyInsurance(Live_UserEntry model)
        {
            return await base.Context.Updateable(model).UpdateColumns(t => new {t.IsManuallyInsurance}).ExecuteCommandAsync();
        }

        public async Task<Live_V_UserEntryContract> GetUserEntryContract(int id)
        {
            return await base.Context.Queryable<Live_V_UserEntryContract>()
                  .Where(f => f.ID == id).FirstAsync();
        }

        public async Task<List<Live_UserLeaveReason>> ListLeaveReason(int id)
        {
            return await base.Context.Queryable<Live_UserLeaveReason>()
                  .Where(f => f.EntryID == id).ToListAsync();
        }


        public async Task<int> UpdateAge(List<Model.Live_UserEntry> list)
        {
            return await base.Context.Updateable(list).UpdateColumns(t => new { t.Age }).ExecuteCommandAsync();
        }

        public async Task<int> UsersCount(Expression<Func<Model.Live_UserEntry, bool>> exp)
        {
            return await base.Context.Queryable<Live_UserEntry>().Where(exp).CountAsync();
        }


        public async Task<int> UpdateWorkNameAndClass(List<Live_V_UserEntry> model)
        {
            return await base.Context.Updateable<Live_UserEntry>(model).UpdateColumns(t => new { t.WorkClass}).ExecuteCommandAsync();
        }

        public async Task<int> UpdateInsurancePlan(List<Live_UserEntry> model)
        {
            return await base.Context.Updateable<Live_UserEntry>(model).UpdateColumns(t => new { t.PlanName1, t.InsuranceCompany1,t.IsManuallyInsurance }).ExecuteCommandAsync();
        }
        #region 员工商保视图

        public async Task<List<Live_V_UserEntryInsuracneStatus>> UserEntryInsurancePageList(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_V_UserEntryInsuracneStatus>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_V_UserEntryInsuracneStatus>> UserEntryInsuranceList(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_V_UserEntryInsuracneStatus>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            return list;
        }

        public async Task<Live_V_UserEntryInsuracneStatus> GetUserEntryInsurance(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp)
        {
            var model = await base.Context.Queryable<Live_V_UserEntryInsuracneStatus>().Where(exp).OrderByDescending(it => it.ID).FirstAsync();
            return model;
        }

        /// <summary>
        /// 获取员工信息中商保的信息
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<List<Live_V_UserEntryInsuracneStatus>> ListInsurance(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserEntryInsuracneStatus>().Where(exp).ToListAsync();
        }
        public async Task<Live_V_UserEntryInsuracneStatus> GetInsurance(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserEntryInsuracneStatus>().Where(exp).FirstAsync();
        }
        /// <summary>
        ///  更新 保险公司、参保方案、订单号、参保状态、流程状态
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<int> UpdateInsuranceInfo(List<Live_UserEntry> models)
        {
            
            return await base.Context.Updateable<Live_UserEntry>(models).UpdateColumns(f=>new {  f.InsuranceCompany1, f.PlanName1, f.InsuranceId1,  f.PlanStatus1, f.InsuranceProcessStatus,f.IsManuallyInsurance, f.InsuranceType,f.InsuranceWorkName }).ExecuteCommandAsync();
        }

     

        public async Task<int> UpdateInsuranceInfo(Live_UserEntry model)
        {
            return await base.Context.Updateable<Live_UserEntry>(model).UpdateColumns(f => new {f.WorkName,f.WorkClass,  f.InsuranceCompany1, f.PlanName1, f.InsuranceId1, f.PlanStatus1, f.InsuranceProcessStatus, f.IsManuallyInsurance,f.InsuranceType }).ExecuteCommandAsync();
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<int> UpdateInsuranceProcessStatus(List<Live_UserEntry> models)
        {

            return await base.Context.Updateable<Live_UserEntry>(models).UpdateColumns(f => new { f.PlanStatus1, f.InsuranceProcessStatus }).ExecuteCommandAsync();
        }



        public async Task<int> UpdateInsuranceProcessStatus(Live_UserEntry model)
        {

            return await base.Context.Updateable<Live_UserEntry>(model).UpdateColumns(f => new { f.PlanStatus1, f.InsuranceProcessStatus }).ExecuteCommandAsync();
        }
        public async Task<List<string>> ListInsuranceCustomerNames(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserEntryInsuracneStatus>().Where(exp).Select(f=>f.CustomerName).Distinct().ToListAsync();
        }
        #endregion
        #region 身份信息


        public async Task<Live_V_UserEntryInfo> GetUserEntryInfo(Expression<Func<Live_V_UserEntryInfo, bool>> exp)
        {
            var model = await base.Context.Queryable<Live_V_UserEntryInfo>().Where(exp).OrderByDescending(it => it.ID).FirstAsync();
            return model;
        }

        #endregion

        public async Task<int> ChangeCaAuth(int id, bool isCaAuth)
        {
            return await base.Context.Updateable<Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { IsCaAuth = isCaAuth }).Where(it => it.ID == id).ExecuteCommandAsync();
        }

        public async Task<int> ChangeCaAuth(List<int> ids, bool isCaAuth)
        {
            return await base.Context.Updateable<Live_UserEntry>().SetColumns(it => new Model.Live_UserEntry() { IsCaAuth = isCaAuth }).Where(it => ids.Contains(it.ID)).ExecuteCommandAsync();
        }

    }
}
