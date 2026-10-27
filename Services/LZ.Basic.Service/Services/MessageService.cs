using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Services
{
    public class MessageService : ServicesBase<Live_Message>, IMessageService, IScopedDependency
    {
        public async Task<List<Model.Live_Message>> List(Expression<Func<Model.Live_Message, bool>> exp, BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_Message>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }
            else
            {
                sco.OrderByDescending(t => t.CreateTime);
            }
            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        

        public async Task<int> Add(Model.Live_Message model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Adds(List<Model.Live_Message> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_Message model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_Message>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_Message, bool>> exp)
        {
            return await base.Context.Queryable<Live_Message>().AnyAsync(exp);
        }
        public async Task<Model.Live_Message> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_Message>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_Message>().AnyAsync(it => it.ID == id);
        }
    }
}
