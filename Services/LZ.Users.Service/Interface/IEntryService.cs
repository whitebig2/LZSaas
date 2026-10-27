using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using static LZ.Users.Service.Services.EntryService;

namespace LZ.Users.Service.Interface
{
    public interface IEntryService
    {
        Task<List<Model.Live_V_UserEntry>> List(Expression<Func<Model.Live_V_UserEntry, bool>> exp, Model.PagedModel.UserPagedModel query);
        Task<int> Add(Model.Live_UserEntry model);
        //导入线下员工
        Task<List<int>> Add(List<Model.Live_UserEntry> models);
        /// <summary>
        /// 入职操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isDirectEntry">是否直接入职</param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        //Task<bool> UserEntry(Live_UserEntry model, bool isDirectEntry);
        /// <summary>
        /// 入职事务
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isDirectEntry"></param>
        /// <returns></returns>
        Task<int> UserEntryAffairs(Live_UserEntry model, bool isDirectEntry,ICurrentUser user=null);
        /// <summary>
        /// 批量更改入职信息
        /// </summary>
        /// <param name="models"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task<bool> BatchChangeEntryInfo(List<Model.Live_UserEntry> models, Expression<Func<Model.Live_UserEntry, object>> columns);
        /// <summary>
        /// 更改工种
        /// </summary>
        /// <param name="id"></param>
        /// <param name="work"></param>
        /// <param name="workClass"></param>
        /// <returns></returns>
        Task<bool> ChangeWork(int id, string work, string workClass);
        /// <summary>
        /// 更改银行卡
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bankNumber"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        Task<bool> ChangeBank(int id, string bankNumber, string bankName, string bankProvince, string bankCity);
        /// <summary>
        /// 更改银行所属地
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bankProvince"></param>
        /// <param name="bankCity"></param>
        /// <returns></returns>
        Task<bool> ChangeBankArea(int id, string bankProvince, string bankCity);
        /// <summary>
        /// 更改银行所属地
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bankProvince"></param>
        /// <param name="bankCity"></param>
        /// <returns></returns>
        Task<bool> ChangeUserBankArea(int userId, string bankProvince, string bankCity);
        /// <summary>
        /// 离职操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="leaves"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<bool> UserLeave(Live_UserEntry model, List<Live_UserLeaveReason> leaves, int companyId);
        Task<bool> Update(Model.Live_UserEntry model);
        Task<bool> UpdateBank(Model.Live_User user);
        /// <summary>
        /// 更改入职信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> ChangeEntryInfo(Model.Live_UserEntry model);
        Task<bool> ChangeEntry(Model.Live_UserEntry model);
        Task<bool> UpdateEntryInfo(Model.Live_User user);
        Task<bool> UpdateCustomer(int id, string workClass,string workName,string fromType);
        /// <summary>
        /// 关联同一个手机号的线下员工信息
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="uid"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<bool> SetUser(string phone, int uid, int companyId);
        /// <summary>
        /// 关联同一个身份证的线下员工信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<bool> SetUser(Model.Live_User user, int companyId);
        Task<bool> UpdateStatus(UserStatus status,Expression<Func<Model.Live_UserEntry, bool>> exp);
        Task<bool> UpdateRemarks(int id, string remarks);
        /// <summary>
        /// 更新简道云数据id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        Task<bool> UpdateJDYDataID(int id, string dataId);
        /// <summary>
        /// 更新最新的便签
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        Task<bool> UpdateLastNote(int id, string remarks);
        Task<bool> UpdateContractStatus(ContractStatus status, int id);
        Task<bool> UpdateInjuryStatus(int status, Expression<Func<Model.Live_UserEntry, bool>> exp);
        Task<bool> Delete(int id, int companyId = 0);
        Task<bool> Exist(int id, int companyId = 0);
        Task<bool> Exist(Expression<Func<Model.Live_UserEntry, bool>> exp);
        Task<bool> Set(int id, int status, int companyId = 0);
        Task<Model.Live_UserEntry> Get(int id, int companyId = 0);
        Task<Model.Live_UserEntry> Get(Expression<Func<Model.Live_UserEntry, bool>> exp);
        Task<Live_V_UserEntry> View(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<Model.Live_V_UserEntry> View(int id, int companyId = 0);
        Task<List<Model.CountModel>> StatusCount(Expression<Func<Live_V_UserEntry, bool>> exp);
        /// <summary>
        /// 面试
        /// </summary>
        /// <param name="id"></param>
        /// <param name="intDate"></param>
        /// <param name="about"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<bool> UpdateInt(Model.Live_UserEntry m,int status, int companyId);
        Task<bool> UpdateDayOver(DateTime? DayOverStartTime, DateTime? DayOverEndTime, bool IsDaySign);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        Task<int> UpdateSocial(List<Model.Live_UserEntry> list);
        Task<Live_V_UserEntryContract> GetUserEntryContract(int id);

        Task<List<Live_User>> PartnerUsers(Model.PagedModel.UserPagedModel query, Expression<Func<Live_User, bool>> exp);
        Task<bool> Update(List<Live_UserEntry> models);
        Task<List<Live_UserEntry>> List(Expression<Func<Live_UserEntry, bool>> exp);
        Task<List<Live_V_UserEntry>> ListView(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<bool> UpdateContractInfo(ContractStatus status, DateTime? contractDate, DateTime contractStartDate, DateTime contractEndDate, int id);
        Task<int> UpdateContract(Live_UserEntry model);
        Task<int> UpdateContract(List<Live_UserEntry> models);
        Task<int> UpdateBusInsurance(List<Live_UserEntry> models);
        Task<int> UpdateBusInsurance(Live_UserEntry model);
        Task<Live_V_UserEntry> View(Expression<Func<Live_V_UserEntry, bool>> exp, string strOrderByFileds);
        Task<List<Live_UserLeaveReason>> ListLeaveReason(int id);
        Task<int> UpdateManuallyInsurance(Live_UserEntry model);
        Task<int> UpdateAge(List<Live_UserEntry> list);
        Task<int> UsersCount(Expression<Func<Model.Live_UserEntry, bool>> exp);
        Task<int> UpdateWorkNameAndClass(List<Live_V_UserEntry> model);
        Task<int> UpdateInsurancePlan(List<Live_UserEntry> model);
        Task<List<Live_V_UserEntryInsuracneStatus>> ListInsurance(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp);
        Task<int> UpdateInsuranceInfo(List<Live_UserEntry> models);
        Task<List<string>> ListInsuranceCustomerNames(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp);
        Task<int> UpdateInsuranceProcessStatus(List<Live_UserEntry> models);
        Task<int> UpdateInsuranceProcessStatus(Live_UserEntry model);
        Task<List<Live_V_UserEntryInsuracneStatus>> UserEntryInsurancePageList(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp, PagedModelBase query);
        Task<List<Live_V_UserEntryInsuracneStatus>> UserEntryInsuranceList(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp);
        Task<Live_V_UserEntryInsuracneStatus> GetInsurance(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp);
        Task<int> UpdateInsuranceInfo(Live_UserEntry model);
        Task<Live_V_UserEntryInsuracneStatus> GetUserEntryInsurance(Expression<Func<Live_V_UserEntryInsuracneStatus, bool>> exp);
        Task<Live_V_UserEntryInfo> GetUserEntryInfo(Expression<Func<Live_V_UserEntryInfo, bool>> exp);
        Task<int> ChangeCaAuth(int id, bool isCaAuth);
        Task<int> ChangeCaAuth(List<int> ids, bool isCaAuth);
    }
}
