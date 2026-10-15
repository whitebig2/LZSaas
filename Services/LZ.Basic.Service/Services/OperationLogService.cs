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
    public class OperationLogService : ServicesBase<SYS_OperationLog>, IOperationLogService, IScopedDependency
    {
        public async Task<List<SYS_OperationLog>> List(Expression<Func<SYS_OperationLog, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_OperationLog>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_OperationLog>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task Insert(Model.SYS_OperationLog model)
        {
            await base.Context.Insertable(model).ExecuteCommandAsync();
        }
    }
}
