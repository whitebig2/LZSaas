using LZ.Company.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Company.Service.Services
{
    public class LeaveReasonService : ServicesBase<Live_LeaveReason>, ILeaveReasonService, IScopedDependency
    {
        public async Task<List<Live_LeaveReason>> List(Expression<Func<Live_LeaveReason, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_LeaveReason>().Where(exp).OrderBy(it=>it.Sort).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_LeaveReason>().Where(exp).OrderBy(it => it.Sort).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_LeaveReason model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_LeaveReason>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_LeaveReason model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).Where(it => it.ID == model.ID && it.CompanyID == model.CompanyID).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_LeaveReason> Get(int id)
        {
            return await base.Context.Queryable<Live_LeaveReason>().FirstAsync(it=>it.ID==id);
        }
        public async Task<bool> Delete(int id,int companyId)
        {
            return await base.Context.Deleteable<Live_LeaveReason>().Where(it => it.ID == id && it.CompanyID== companyId).ExecuteCommandAsync() > 0;
        }
    }
}
