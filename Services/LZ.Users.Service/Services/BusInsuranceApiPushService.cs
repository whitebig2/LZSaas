using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Users.Service.Services
{
    public class BusInsuranceApiPushService : ServicesBase<Live_BusInsuranceApiPush>, IBusInsuranceApiPushService, IScopedDependency
    {

        public async Task<List<Live_BusInsuranceApiPush>> PageList(Expression<Func<Live_BusInsuranceApiPush, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusInsuranceApiPush>()
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusInsuranceApiPushDetail>> PageDetailList(Expression<Func<Live_BusInsuranceApiPushDetail, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusInsuranceApiPushDetail>()
                .Where(exp)
                .OrderByDescending(it => it.ID)
                .ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusInsuranceApiPush>> List(Expression<Func<Live_BusInsuranceApiPush, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceApiPush>()
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public async Task<List<Live_BusInsuranceApiPushDetail>> ListDetail(Expression<Func<Live_BusInsuranceApiPushDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceApiPushDetail>()
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public async Task<List<Live_BusInsuranceApiPushExtension>> ListNav(Expression<Func<Live_BusInsuranceApiPushExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceApiPushExtension>()
                .Includes(f => f.Details)
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public async Task<Live_BusInsuranceApiPush> Get(Expression<Func<Live_BusInsuranceApiPush, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceApiPush>()
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .FirstAsync();
        }

        public async Task<Live_BusInsuranceApiPushDetail> GetDetail(Expression<Func<Live_BusInsuranceApiPushDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceApiPushDetail>()
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .FirstAsync();
        }

        public async Task<Live_BusInsuranceApiPushExtension> GetNav(Expression<Func<Live_BusInsuranceApiPushExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceApiPushExtension>()
                .Includes(f => f.Details)
                .Where(exp)
                .OrderByDescending(it => it.CreateTime)
                .FirstAsync();
        }

        public async Task<bool> Exist(Expression<Func<Live_BusInsuranceApiPush, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceApiPush>().AnyAsync(exp);
        }

        public async Task<bool> Add(Live_BusInsuranceApiPush model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(List<Live_BusInsuranceApiPush> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> AddNav(Live_BusInsuranceApiPushExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f => f.Details)
                .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_BusInsuranceApiPushExtension> models)
        {
            return await base.Context.InsertNav(models)
                .Include(f => f.Details)
                .ExecuteCommandAsync();
        }

        public async Task<bool> Update(Live_BusInsuranceApiPush model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_BusInsuranceApiPush> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateDetail(Live_BusInsuranceApiPushDetail model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateNav(Live_BusInsuranceApiPushExtension model)
        {
            return await base.Context.UpdateNav(model)
                .Include(f => f.Details, new UpdateNavOptions
                {
                    OneToManyInsertOrUpdate = true,
                })
                .ExecuteCommandAsync();
        }

        public async Task Delete(Expression<Func<Live_BusInsuranceApiPush, bool>> exp)
        {
            await base.Context.Deleteable<Live_BusInsuranceApiPush>().Where(exp).ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Guid id)
        {
            return await base.Context.DeleteNav<Live_BusInsuranceApiPushExtension>(f => f.ID == id)
                .Include(f => f.Details)
                .ExecuteCommandAsync();
        }

        public async Task<bool> DeleteDetail(Guid id)
        {
            var detail = await base.Context.Queryable<Live_BusInsuranceApiPushDetail>().FirstAsync(it => it.ID == id);
            if (detail == null)
            {
                return false;
            }

            var deleted = await base.Context.Deleteable<Live_BusInsuranceApiPushDetail>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
            if (!deleted)
            {
                return false;
            }

            var push = await base.Context.Queryable<Live_BusInsuranceApiPush>().FirstAsync(f => f.ID == detail.BusInsuranceApiPushID);
            if (push == null)
            {
                return true;
            }

            var remainDetails = await base.Context.Queryable<Live_BusInsuranceApiPushDetail>()
                .Where(f => f.BusInsuranceApiPushID == detail.BusInsuranceApiPushID)
                .ToListAsync();
            push.AddCount = remainDetails.Count(f => f.AddBusinessID > 0);
            push.DelCount = remainDetails.Count(f => f.DelBusinessID > 0);
            await base.Context.Updateable(push)
                .UpdateColumns(f => new { f.AddCount, f.DelCount })
                .ExecuteCommandAsync();
            return true;
        }

        public async Task Confirm(Guid id)
        {
            var model = await GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "推送记录不存在");
            }
            if (model.Status != BusInsuranceApiPushStatusEnumType.已推送待确认)
            {
                throw new ErrorCodeException(-1, "仅已推送待确认状态可确认");
            }
            if (model.Details == null || model.Details.Count == 0)
            {
                throw new ErrorCodeException(-1, "没有可确认的明细");
            }

            model.Status = BusInsuranceApiPushStatusEnumType.已确认;
            await Update(model);
        }
    }
}
