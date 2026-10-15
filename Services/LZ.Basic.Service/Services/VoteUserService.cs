using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Basic.Service.Services
{
    public class VoteUserService : ServicesBase<Live_VoteUser>, IVoteUserService, IScopedDependency
    {
        public async Task<List<Live_VoteUser>> List(Expression<Func<Live_VoteUser, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_VoteUser>().Where(exp).OrderBy(it=>it.UserID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_VoteUser>().Where(exp).OrderBy(it => it.UserID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_VoteUser model)
        {
          return  await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_VoteUser, bool>> exp)
        {
            return await base.Context.Queryable<Live_VoteUser>().AnyAsync(exp);
        }
        public async Task<int> AddItems(List<Live_VoteItems> items)
        {
            return await base.Context.Insertable(items).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Live_VoteUser model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_VoteUser> Get(int id)
        {
            return await base.Context.Queryable<Live_VoteUser>().FirstAsync(it=>it.UserID==id);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_VoteUser>().Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }
    }
}
