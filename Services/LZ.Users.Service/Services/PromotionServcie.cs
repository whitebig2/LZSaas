using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Core;
using System.ComponentModel.Design;
using Consul;
using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using Polly;
using static Dm.net.buffer.ByteArrayBuffer;
using LZ.Model.PagedModel;
using LZ.Core.Services;

namespace LZ.Users.Service.Services
{
    public class PersonnelService(IAuthService authService) : ServicesBase<Live_Personnel>, IPersonnelService, IScopedDependency
    {
        public async Task<List<Live_V_Personnel>> List(Expression<Func<Live_V_Personnel, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_Personnel>().Where(exp).OrderByDescending(it=>it.UserID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_Personnel>().Where(exp).OrderByDescending(it => it.UserID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<Model.Live_Personnel> ExistPhone(string phone, int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.Phone == phone);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Model.Live_Personnel>().FirstAsync(exp);
        }
        public async Task<Model.Live_Personnel> Login(string phone,int companyId)
        {
            return await base.Context.Queryable<Model.Live_Personnel>().FirstAsync(it => it.Phone == phone && it.CompanyID==companyId);
        }
        public async Task<int> Add(Live_Personnel model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> AddOpenID(Live_PersonnelOpenIds model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_Personnel>().AnyAsync(exp);
        }
        public async Task<bool> ExistOpenID(string openId, int uid, int clientId = 0, string type = "")
        {
            var expable = Expressionable.Create<Model.Live_PersonnelOpenIds>();
            expable.AndIF(!string.IsNullOrEmpty(openId), it => it.OpenId == openId);
            expable.And(it => it.UserID == uid);
            expable.AndIF(clientId > 0, it => it.FromID == clientId);
            expable.AndIF(!string.IsNullOrEmpty(type), it => it.OpenIdType == type);
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_PersonnelOpenIds>().AnyAsync(exp);
        }
        public async Task<bool> Exist(Expression<Func<Live_Personnel, bool>> exp)
        {
            return await base.Context.Queryable<Live_Personnel>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_Personnel model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdatePrivateQrcode(int userId,string qrcode)
        {
            return await base.Context.Updateable<Live_Personnel>().SetColumns(it=>it.PrivateQrcode==qrcode).Where(it=>it.UserID==userId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateClientQrcode(int userId, string qrcode)
        {
            return await base.Context.Updateable<Live_Personnel>().SetColumns(it => it.ClientQrcode == qrcode).Where(it => it.UserID == userId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateJDYNumber(List<Model.Live_Personnel> models)
        {
            return await base.Context.Updateable(models).UpdateColumns(it=>it.JDYNumber).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id, bool enabled, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Updateable<Live_Personnel>().SetColumns(it => it.Enabled == enabled).Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> SetBindMP(int id, bool enabled)
        {
            return await base.Context.Updateable<Live_Personnel>().SetColumns(it => it.IsBindMP == enabled).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_Personnel> Get(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId>0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_Personnel>().FirstAsync(exp);
        }
        public async Task<Live_V_Personnel> GetView(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_V_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_V_Personnel>().FirstAsync(exp);
        }

        public async Task<Live_Personnel> Get(Expression<Func<Live_Personnel, bool>> exp)
        {
            return await base.Context.Queryable<Live_Personnel>().FirstAsync(exp);
        }
        public async Task<Live_PersonnelOpenIds> GetOpenID(int uid,int clientId)
        {
            return await base.Context.Queryable<Live_PersonnelOpenIds>().FirstAsync(it => it.FromID == clientId && it.UserID==uid);
        }
        public async Task<Live_PersonnelOpenIds> GetOpenID(string openid)
        {
            return await base.Context.Queryable<Live_PersonnelOpenIds>().FirstAsync(it => it.OpenId == openid);
        }
        public async Task<bool> Delete(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            var has = await base.Context.Deleteable<Live_Personnel>().Where(exp).ExecuteCommandAsync() > 0;
            if (has)
            {
                await base.Context.Deleteable<Live_PersonnelOpenIds>().Where(it => it.UserID == id).ExecuteCommandAsync();
                await base.Context.Deleteable<Live_PersonnelItems>().Where(it => it.UserID == id).ExecuteCommandAsync();
                await authService.DeleteToken(id, (int)Model.Enums.PlatformType.LZMini);
            }
            return has;
        }
        public async Task<int> ChangePicture(int uid,string picture)
        {
            return await base.Context.Updateable<Live_Personnel>().SetColumns(it => it.Picture == picture).Where(it => it.UserID == uid).ExecuteCommandAsync();
        }
    }
}
