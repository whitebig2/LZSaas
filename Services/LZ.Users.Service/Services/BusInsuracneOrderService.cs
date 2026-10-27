using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.PagedModel;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LZ.Users.Service.Services
{
    public class BusInsuracneOrderService : ServicesBase<Live_BusInsuracneOrder>, IBusInsuracneOrderService, IScopedDependency
    {
        public async Task<List<Live_BusInsuracneOrder>> PageList(Expression<Func<Live_BusInsuracneOrder, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusInsuracneOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusInsuracneOrder>> List(Expression<Func<Live_BusInsuracneOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuracneOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_V_BusInsuracneOrderPlan>> ListOrderPlan(Expression<Func<Live_V_BusInsuracneOrderPlan, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusInsuracneOrderPlan>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusInsuracneOrder> Get(Expression<Func<Live_BusInsuracneOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuracneOrder>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }

        public async Task<bool> Update(Live_BusInsuracneOrder model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(Live_BusInsuracneOrder model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Add(List<Live_BusInsuracneOrder> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> AddNav(Live_BusInsuracneOrderExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_BusInsuracneOrderExtension> models)
        {
            return await base.Context.InsertNav(models)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task<Live_BusInsuracneOrderExtension> GetNav(Expression<Func<Live_BusInsuracneOrderExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuracneOrderExtension>()
                .Includes(f => f.Records)
                .OrderByDescending(f => f.CreateTime)
                .FirstAsync(exp);
        }

        public async Task<bool> Update(List<Live_BusInsuracneOrder> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateNav(Live_BusInsuracneOrderExtension model)
        {
            return await base.Context.UpdateNav(model)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }

        public async Task Delete(Expression<Func<Live_BusInsuracneOrder, bool>> exp)
        {
            await base.Context.Deleteable<Live_BusInsuracneOrder>().Where(exp).ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Expression<Func<Live_BusInsuracneOrderExtension, bool>> exp)
        {
            return await base.Context.DeleteNav(exp)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }
    }
}
