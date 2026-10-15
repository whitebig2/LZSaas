using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;

namespace LZ.Basic.Service.Services
{
    public class LabelManagementService : ServicesBase<Live_LabelManagement>, ILabelManagementService, IScopedDependency
    {
        public async Task<List<Model.Live_LabelManagement>> List(Expression<Func<Model.Live_LabelManagement, bool>> exp, BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_LabelManagement>().Where(exp);
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

        public async Task<int> Add(Model.Live_LabelManagement model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Adds(List<Model.Live_LabelManagement> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_LabelManagement model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_LabelManagement>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_LabelManagement, bool>> exp)
        {
            return await base.Context.Queryable<Live_LabelManagement>().AnyAsync(exp);
        }
        public async Task<Model.Live_LabelManagement> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_LabelManagement>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Model.Live_LabelManagement> Get(Expression<Func<Model.Live_LabelManagement, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_LabelManagement>().Where(exp).OrderByDescending(t=>t.CreateTime).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_LabelManagement>().AnyAsync(it => it.ID == id);
        }
    }
}
