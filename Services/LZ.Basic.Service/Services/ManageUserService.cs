using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.EntityFrameworkCore;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Security.Policy;

namespace LZ.Basic.Service.Services
{
    public class ManageUserService(IAuthService authService): ServicesBase<SYS_ManageUser>, IManageUserService, IScopedDependency
    {
        public async Task<List<SYS_ManageUser>> List(Expression<Func<SYS_ManageUser, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_ManageUser>().Where(exp).OrderByDescending(it => it.UserID).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_ManageUser>().Where(exp).OrderByDescending(it => it.UserID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<Model.SYS_ManageUser> Login(string userName, string password)
        {
            return await base.Context.Queryable<Model.SYS_ManageUser>().FirstAsync(it => it.UserName == userName && it.UserPassword == password);
        }
        public async Task<Model.SYS_ManageUser> Info(int uid)
        {
            return await base.Context.Queryable<Model.SYS_ManageUser>().FirstAsync(it => it.UserID==uid);
        }
        public async Task<int> Add(SYS_ManageUser model)
        {
            var id= await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
            if (id > 0)
            {
                await AddUserRole(id, model.RoleIDS);
            }
            return id;
        }
        private async Task<bool> AddUserRole(int uid, string rids)
        {
            if (string.IsNullOrEmpty(rids))
            {
                return false;
            }
            var db = itenant;
            var result = await db.UseTranAsync(async () =>
            {
                await db.DeleteableWithAttr<Model.SYS_ManageRoles>().Where(t => t.UserID == uid).ExecuteCommandAsync();
                var models = new List<Model.SYS_ManageRoles>();
                var rList = rids.Split(',');
                foreach (var rid in rList)
                {
                    var m = new Model.SYS_ManageRoles();
                    m.RGID = new Guid(rid);
                    m.UserID = uid;
                    models.Add(m);
                }
                await db.InsertableWithAttr(models).ExecuteCommandAsync();
                return true;
            });
            if (result.Data)
            {
            }
            return result.Data;
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<SYS_ManageUser>().AnyAsync(it => it.UserID == id);
        }
        public async Task<bool> ExistName(string name)
        {
            return await base.Context.Queryable<SYS_ManageUser>().AnyAsync(it => it.UserName == name);
        }
        public async Task<bool> ExistName(string name,int id)
        {
            return await base.Context.Queryable<SYS_ManageUser>().AnyAsync(it => it.UserName == name && it.UserID!=id);
        }
        public async Task<bool> Update(SYS_ManageUser model)
        {
            var has = await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
            if (has)
            {
                await AddUserRole(model.UserID, model.RoleIDS);
            }
            return has;
        }
        public async Task<bool> Set(int id, bool enabled)
        {
            return await base.Context.Updateable<SYS_ManageUser>().SetColumns(it => it.Enabled == enabled).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_ManageUser> Get(int id)
        {
            return await base.Context.Queryable<SYS_ManageUser>().FirstAsync(it => it.UserID == id);
        }
        public async Task<bool> Delete(int id)
        {
            var has = await base.Context.Deleteable<SYS_ManageUser>().Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await authService.DeleteToken(id, (int)Model.Enums.PlatformType.Admin);
            }
            return has;
        }
        public async Task<int> ChangePassword(int uid,string ypassword, string npassword)
        {
            return await base.Context.Updateable<SYS_ManageUser>().SetColumns(it=>it.UserPassword==npassword).Where(it => it.UserID == uid && it.UserPassword == ypassword).ExecuteCommandAsync();
        }
        public async Task<int> ChangePicture(int uid,string picture)
        {
            return await base.Context.Updateable<SYS_ManageUser>().SetColumns(it => it.Picture == picture).Where(it => it.UserID == uid).ExecuteCommandAsync();
        }
    }
}
