using LZ.Company.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.PagedModel;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Company.Service.Services
{
    public class CompanyUserService(IAuthService authService) : ServicesBase<SYS_CompanyUser>, ICompanyUserService, IScopedDependency
    {
        public async Task<List<SYS_CompanyUser>> List(Expression<Func<SYS_CompanyUser, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_CompanyUser>().Where(exp).OrderByDescending(it => it.UserID).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_CompanyUser>().Where(exp).OrderByDescending(it => it.UserID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<Model.SYS_CompanyUser> Login(string userName, string password)
        {
            return await base.Context.Queryable<Model.SYS_CompanyUser>().FirstAsync(it => it.UserName == userName && it.UserPassword == password);
        }

        public async Task<Model.SYS_CompanyUser> Get(Expression<Func<Model.SYS_CompanyUser, bool>> exp)
        {
            return await base.Context.Queryable<Model.SYS_CompanyUser>().Where(exp).FirstAsync();
        }
        public async Task<Model.SYS_CompanyUser> Info(int uid)
        {
            return await base.Context.Queryable<Model.SYS_CompanyUser>().FirstAsync(it => it.UserID==uid);
        }
        public async Task<int> Add(SYS_CompanyUser model)
        {
            var id = await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
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
                await db.DeleteableWithAttr<Model.SYS_CompanyUserRoles>().Where(t => t.UserID == uid).ExecuteCommandAsync();
                var models = new List<Model.SYS_CompanyUserRoles>();
                var rList = rids.Split(',');
                foreach (var rid in rList)
                {
                    var m = new Model.SYS_CompanyUserRoles();
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
            return await base.Context.Queryable<SYS_CompanyUser>().AnyAsync(it => it.UserID == id);
        }
        public async Task<bool> ExistName(string name)
        {
            return await base.Context.Queryable<SYS_CompanyUser>().AnyAsync(it => it.UserName == name);
        }
        public async Task<bool> ExistUserName(string name, int id)
        {
            return await base.Context.Queryable<SYS_CompanyUser>().AnyAsync(it => it.UserName == name && it.UserID!=id);
        }
        public async Task<bool> ExistName(string name,int companyID)
        {
            return await base.Context.Queryable<SYS_CompanyUser>().AnyAsync(it => it.UserName == name&&it.CompanyID==companyID);
        }
        public async Task<bool> Update(SYS_CompanyUser model)
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
            return await base.Context.Updateable<SYS_CompanyUser>().SetColumns(it => it.Enabled == enabled).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_CompanyUser> Get(int id)
        {
            return await base.Context.Queryable<SYS_CompanyUser>().FirstAsync(it => it.UserID == id);
        }
        public async Task<bool> Delete(int id,int companyId)
        {
            var has = await base.Context.Deleteable<SYS_CompanyUser>().Where(it => it.UserID == id && it.CompanyID == companyId).ExecuteCommandAsync() > 0;
            if (has)
            {
                await authService.DeleteToken(id, (int)Model.Enums.PlatformType.MiniAdmin);
            }
            return has;
        }
        public async Task<int> ChangePassword(int uid,string ypassword, string npassword,int companyId)
        {
            return await base.Context.Updateable<SYS_CompanyUser>().SetColumns(it=>it.UserPassword==npassword).Where(it => it.UserID == uid && it.UserPassword == ypassword && it.CompanyID == companyId).ExecuteCommandAsync();
        }
        public async Task<int> ChangePicture(int uid,string picture,int companyId)
        {
            return await base.Context.Updateable<SYS_CompanyUser>().SetColumns(it => it.Picture == picture).Where(it => it.UserID == uid && it.CompanyID == companyId).ExecuteCommandAsync();
        }
    }
}
