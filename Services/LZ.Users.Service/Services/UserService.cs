using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class UserService(IAuthService authService) : ServicesBase<Live_User>, IUserService, IScopedDependency
    {
        public async Task<List<Live_User>> List(Expression<Func<Model.Live_User, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_User>();
            qb.Where(exp);
            qb.OrderByDescending(it => it.UserID);
            if (query.Page == 0)
            {
                if (query.Top > 0)
                {
                    qb.Take(query.Top);
                }
                return await qb.ToListAsync();
            }
            var list = await qb.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<Model.Live_User> ExistPhone(string phone, int companyId)
        {
            return await base.Context.Queryable<Model.Live_User>().FirstAsync(it => it.Phone == phone && it.CompanyID == companyId);
        }
        public async Task<int> Add(Live_User model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> AddOpenID(Live_UserOpenIds model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<int> AddOpenID(List<Live_UserOpenIds> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> ExistOpenID(string openId,int uid,int clientId=0,string type="")
        {
            var expable = Expressionable.Create<Model.Live_UserOpenIds>();
            expable.AndIF(!string.IsNullOrEmpty(openId), it => it.OpenId == openId);
            expable.AndIF(uid > 0, it => it.UserID == uid);
            expable.AndIF(clientId>0, it => it.FromID == clientId);
            expable.AndIF(!string.IsNullOrEmpty(type), it => it.OpenIdType == type);
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_UserOpenIds>().AnyAsync(exp);
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_User>().AnyAsync(it => it.UserID == id);
        }
        public async Task<bool> Exist(Expression<Func<Model.Live_User, bool>> exp)
        {
            return await base.Context.Queryable<Live_User>().AnyAsync(exp);
        }
        public async Task<bool> ExistOpenId(Expression<Func<Model.Live_UserOpenIds, bool>> exp)
        {
            return await base.Context.Queryable<Live_UserOpenIds>().AnyAsync(exp);
        }
        public async Task<bool> ExistName(string name)
        {
            return await base.Context.Queryable<Live_User>().AnyAsync(it => it.UserName == name);
        }
        public async Task<bool> ChangeStatus(int uid,int status, int[]? nostatus= null)
        {
            var db = base.Context.Updateable<Model.Live_User>();
            db.SetColumns(it => new Model.Live_User() { Status = status, IsDayOver = false });
            db.Where(it => it.UserID == uid);
            if (nostatus != null)
            {
                db.Where(it => !nostatus.Contains(it.Status.Value));
            }
            return await db.ExecuteCommandAsync() > 0;
        }
        public async Task<bool> ChangeIntegral(int uid, int integral)
        {
            var has = await base.Context.Updateable<Model.Live_User>().SetColumns(it => it.Integral == it.Integral + integral).Where(it => it.UserID == uid).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(uid);
            }
            return has;
        }
        public async Task<bool> UpdateTurntable(int userId, bool isClear = false)
        {
            var has = false;
            var db = base.Context.Updateable<Model.Live_User>();
            if (isClear)
            {
                db.SetColumns(it => it.TurntableCount == 1);
            }
            else
            {
                db.SetColumns(it => it.TurntableCount == SqlFunc.IsNull(it.TurntableCount, 0) + 1);
            }
            has = await db.SetColumns(it => it.TurntableDate == DateTime.Now).Where(it => it.UserID == userId).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(userId);
            }
            return has;
        }
        public async Task UpdateUserCache(int uid)
        {
            var model=await base.Context.Queryable<Model.Live_User>().FirstAsync(it => it.UserID == uid);
            await authService.UpdateUserInfo<Live_User>(model, model.UserID, Model.Enums.PlatformType.XFZMini);
        }
        public async Task UpdateUserCache(Guid uid)
        {
            var model = await base.Context.Queryable<Model.Live_User>().FirstAsync(it => it.UserGID == uid);
            await authService.UpdateUserInfo<Live_User>(model, model.UserID, Model.Enums.PlatformType.XFZMini);
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
            var has = await base.Context.Updateable<Model.Live_User>().SetColumns(it => new Model.Live_User() { BankCity = bankCity, BankProvince = bankProvince }).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;

            return has;
        }
        public async Task<bool> Update(Live_User model)
        {
            var has = await base.Context.Updateable(model).Where(it => it.UserID == model.UserID && it.CompanyID == model.CompanyID).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            if (has)
            {
                await authService.UpdateUserInfo<Live_User>(model, model.UserID, Model.Enums.PlatformType.XFZMini);
            }
            return has;
        }
        public async Task<bool> Set(int id, bool enabled)
        {
            return await base.Context.Updateable<Live_User>().SetColumns(it => it.Enabled == enabled).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> SetUserWrite(Guid id, int type,int shopId)
        {
            var has = await base.Context.Updateable<Live_User>().SetColumns(it => it.ShopID == shopId).SetColumns(it => it.UserType == type).Where(it => it.UserGID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(id);
            }
            return has;
        }
        public async Task<bool> SetShopPay(int id, bool enabled)
        {
            var has= await base.Context.Updateable<Live_User>().SetColumns(it => it.IsShopPay == enabled).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(id);
            }
            return has;
        }
        public async Task<bool> SetBindMP(int id, bool enabled)
        {
            var has = await base.Context.Updateable<Live_User>().SetColumns(it => it.IsBindMP == enabled).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(id);
            }
            return has;
        }
        public async Task<bool> SetSignDate(int id)
        {
            var has = await base.Context.Updateable<Live_User>().SetColumns(it => it.SignDate == DateTime.Now.Date).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(id);
            }
            return has;
        }
        public async Task<bool> SaveQrcode(int id, string qrcode)
        {
            var has = await base.Context.Updateable<Live_User>().SetColumns(it => it.Qrcode == qrcode).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(id);
            }
            return has;
        }
        public async Task<bool> SaveAvatar(int id, string avatar, string name)
        {
            var has = await base.Context.Updateable<Live_User>().SetColumns(it => new Model.Live_User() { Picture=avatar,NickName=name}).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await UpdateUserCache(id);
            }
            return has;
        }
        public async Task<Live_User> Get(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_User>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_User>().FirstAsync(exp);
        }
        public async Task<Live_User> Get(Expression<Func<Live_User, bool>> exp)
        {
            return await base.Context.Queryable<Live_User>().FirstAsync(exp);
        }
        public async Task<Live_UserOpenIds> GetOpenID(int uid, int clientId)
        {
            return await base.Context.Queryable<Live_UserOpenIds>().FirstAsync(it => it.FromID == clientId && it.UserID == uid);
        }
        public async Task<Live_UserOpenIds> GetOpenID(string openid)
        {
            return await base.Context.Queryable<Live_UserOpenIds>().FirstAsync(it => it.OpenId == openid);
        }
        public async Task<bool> Delete(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_User>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            var has = await base.Context.Deleteable<Live_User>().Where(exp).ExecuteCommandAsync() > 0;
            if (has)
            {
                await base.Context.Deleteable<Live_UserOpenIds>().Where(it => it.UserID == id).ExecuteCommandAsync();
                await authService.DeleteToken(id, (int)Model.Enums.PlatformType.XFZMini);
            }
            return has;
        }
        public async Task<int> ChangePicture(int uid,string picture)
        {
            return await base.Context.Updateable<Live_User>().SetColumns(it => it.Picture == picture).Where(it => it.UserID == uid).ExecuteCommandAsync();
        }
        public async Task<int> ChangeReal(int uid, bool real, string message, int level)
        {
            return await base.Context.Updateable<Live_User>().SetColumns(it => new Model.Live_User() { IsReal = real, RealMessage = message, RealLevel = level }).Where(it => it.UserID == uid && !it.IsReal).ExecuteCommandAsync();
        }

        public async Task<int> ChangeCaAuth(int uid, bool isCaAuth)
        {
            return await base.Context.Updateable<Live_User>().SetColumns(it => new Model.Live_User() { IsCaAuth = isCaAuth }).Where(it => it.UserID == uid ).ExecuteCommandAsync();
        }

        public async Task<int> ChangeCaAuth( List<int> uids, bool isCaAuth)
        {
            return await base.Context.Updateable<Live_User>().SetColumns(it => new Model.Live_User() { IsCaAuth = isCaAuth }).Where(it => uids.Contains(it.UserID)).ExecuteCommandAsync();
        }


        public async Task<Live_V_UserOpenIdsFxq> GetUserFxqApp(Expression<Func<Live_V_UserOpenIdsFxq, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_UserOpenIdsFxq>().Where(exp).FirstAsync();
        }
    }
}
