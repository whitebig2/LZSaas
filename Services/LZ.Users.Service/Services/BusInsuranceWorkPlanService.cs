using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.PagedModel;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace LZ.Users.Service.Services
{


    public class BusInsuranceWorkPlanService : ServicesBase<Live_BusInsuranceWorkPlan>, IBusInsuranceWorkPlanService, IScopedDependency
    {
        public async Task<List<Live_BusInsuranceWorkPlan>> PageList(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_BusInsuranceWorkPlan>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_BusInsuranceWorkPlan>> List(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceWorkPlan>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_BusInsuranceWorkPlan> Get(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusInsuranceWorkPlan>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Update(Live_BusInsuranceWorkPlan model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Add(Live_BusInsuranceWorkPlan model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Add(List<Live_BusInsuranceWorkPlan> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_BusInsuranceWorkPlan> models)
        {
            return await base.Context.Updateable(models).ExecuteCommandAsync() > 0;
        }

        public async Task Delete(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp)
        {
            await base.Context.Deleteable<Live_BusInsuranceWorkPlan>().Where(exp).ExecuteCommandAsync();
        }

        public Task<List<string>> GetCustomerNames()
        {
            return base.Context.Queryable<Live_BusInsuranceWorkPlan>().Select(f => f.CustomerName).Distinct().ToListAsync();
        }


    }
}
