using LZ.Model;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IUserService
    {
        Task<Model.Live_User> ExistPhone(string phone, int companyId);
        Task<List<Model.Live_User>> List(Expression<Func<Model.Live_User, bool>> exp, UserPagedModel query);
        Task<int> Add(Model.Live_User model);
        Task<int> AddOpenID(Model.Live_UserOpenIds model);
        Task<int> AddOpenID(List<Live_UserOpenIds> models);
        Task<bool> Update(Model.Live_User model);
        Task<bool> Delete(int id, int companyId);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Model.Live_User, bool>> exp);
        Task<bool> ExistOpenId(Expression<Func<Model.Live_UserOpenIds, bool>> exp);
        Task<bool> ExistName(string name);
        Task<bool> Set(int id, bool enabled);
        //是否设为商城体验员
        Task<bool> SetShopPay(int id, bool enabled);
        //是否设为核销员
        Task<bool> SetUserWrite(Guid id, int type, int shopId);
        Task<bool> SetBindMP(int id, bool enabled);
        Task<bool> SetSignDate(int id);
        Task<bool> SaveQrcode(int id, string qrcode);
        Task<bool> SaveAvatar(int id, string avatar,string name);
        Task<Model.Live_User> Get(int id, int companyId=0);
        Task<Live_User> Get(Expression<Func<Live_User, bool>> exp);
        Task<Model.Live_UserOpenIds> GetOpenID(string openid);
        Task<Model.Live_UserOpenIds> GetOpenID(int uid, int clientId);
        Task<bool> ExistOpenID(string openId,int uid, int clientId = 0, string type = "");
        Task<int> ChangePicture(int uid, string picture);
        Task<bool> ChangeIntegral(int uid, int integral);
        Task<bool> UpdateTurntable(int userId,bool isClear=false);
        Task UpdateUserCache(int uid);
        Task UpdateUserCache(Guid uid);
        Task<bool> ChangeStatus(int uid, int status, int[]? nostatus = null);
        Task<int> ChangeReal(int uid, bool real,string message,int level);
        /// <summary>
        /// 更改银行所属地
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bankNumber"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        Task<bool> ChangeBankArea(int id, string bankProvince, string bankCity);
        Task<int> ChangeCaAuth(int uid, bool isCaAuth);
        Task<int> ChangeCaAuth(List<int> uids, bool isCaAuth);
        Task<Live_V_UserOpenIdsFxq> GetUserFxqApp(Expression<Func<Live_V_UserOpenIdsFxq, bool>> exp);
    }
}
