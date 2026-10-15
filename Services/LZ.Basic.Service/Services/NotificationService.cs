using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Services
{
    public class NotificationService : ServicesBase<Live_Notification>, INotificationService, IScopedDependency
    {
        public async Task<List<Model.Live_Notification>> List(Expression<Func<Model.Live_Notification, bool>> exp, BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_Notification>().Where(exp);
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


        public async Task<int> Add(Model.Live_Notification model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<bool> ChangeStatus(int id, int status)
        {
            return await base.Context.Updateable<Live_Notification>().SetColumns(it => it.Status == status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<int> Adds(List<Model.Live_Notification> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_Notification model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_Notification>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_Notification, bool>> exp)
        {
            return await base.Context.Queryable<Live_Notification>().AnyAsync(exp);
        }
        public async Task<Model.Live_Notification> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_Notification>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_Notification>().AnyAsync(it => it.ID == id);
        }
    }
}
