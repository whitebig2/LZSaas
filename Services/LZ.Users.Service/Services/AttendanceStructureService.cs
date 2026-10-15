using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Users.Service.Interface;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class AttendanceStructureService : ServicesBase<Live_AttendanceStructure>, IAttendanceStructureService, IScopedDependency
    {
        public async Task<List<Model.Live_V_AttendanceStructure>> List(Expression<Func<Model.Live_V_AttendanceStructure, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_AttendanceStructure>().Where(exp).OrderBy(t=>t.CustomerID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_AttendanceStructure>().Where(exp).OrderBy(t => t.CustomerID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceStructure model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Live_AttendanceStructure> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceStructure model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceStructure>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceStructure> Get(Expression<Func<Model.Live_AttendanceStructure, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceStructure>().Where(exp).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceStructure>().AnyAsync(it => it.ID == id);
        }

        public async Task<Model.Live_AttendanceStructure> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceStructure>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Live_V_AttendanceStructure> GetView(Expression<Func<Model.Live_V_AttendanceStructure, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_AttendanceStructure>().Where(exp).FirstAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_AttendanceStructure, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceStructure>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceStructure>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
