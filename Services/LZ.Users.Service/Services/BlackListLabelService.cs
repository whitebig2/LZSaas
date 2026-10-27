using LZ.Core.Interface;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class BlackListLabelService : ServicesBase<Live_BlackListLabel>, IBlackListLabelService, IScopedDependency
    {
        public async Task<List<Model.Live_BlackListLabel>> List(Expression<Func<Model.Live_BlackListLabel, bool>> exp, UserPagedModel query,string order="")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_BlackListLabel>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }

            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_BlackListLabel model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_BlackListLabel model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_BlackListLabel>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_BlackListLabel, bool>> exp)
        {
            return await base.Context.Queryable<Live_BlackListLabel>().AnyAsync(exp);
        }
        public async Task<Model.Live_BlackListLabel> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_BlackListLabel>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_BlackListLabel>().AnyAsync(it => it.ID == id);
        }
    }
}
