using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class LaborService : ServicesBase<Live_Labor>, ILaborService, IScopedDependency
    {
        public async Task<List<Live_Labor>> List(Expression<Func<Live_Labor, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_Labor>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_Labor>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<Model.Live_Labor> Login(string phone,int companyId)
        {
            return await base.Context.Queryable<Model.Live_Labor>().FirstAsync(it => it.Phone == phone && it.CompanyID==companyId);
        }
        public async Task<Model.Live_Labor> Info(int uid)
        {
            return await base.Context.Queryable<Model.Live_Labor>().FirstAsync(it => it.UserID==uid);
        }
        public async Task<int> Add(Live_Labor model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_Labor>().AnyAsync(it => it.UserID == id);
        }
        public async Task<bool> ExistName(string name)
        {
            return await base.Context.Queryable<Live_Labor>().AnyAsync(it => it.UserName == name);
        }
        public async Task<bool> Update(Live_Labor model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id, bool enabled)
        {
            return await base.Context.Updateable<Live_Labor>().SetColumns(it => it.Enabled == enabled).Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_Labor> Get(int id)
        {
            return await base.Context.Queryable<Live_Labor>().FirstAsync(it => it.UserID == id);
        }
        public async Task<Live_LaborOpenIds> GetOpenID(string openid)
        {
            return await base.Context.Queryable<Live_LaborOpenIds>().FirstAsync(it => it.OpenId == openid);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_Labor>().Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<int> ChangePicture(int uid,string picture)
        {
            return await base.Context.Updateable<Live_Labor>().SetColumns(it => it.Picture == picture).Where(it => it.UserID == uid).ExecuteCommandAsync();
        }
    }
}
