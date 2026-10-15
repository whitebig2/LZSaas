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
using Senparc.Weixin.WxOpen.AdvancedAPIs.DataCube;
using LZ.Core;
using LZ.Core.Interface;
using NPOI.SS.Formula.Functions;
using LZ.Users.Service.Interface;

namespace LZ.Users.Service.Services
{
    public class BusinessInsuranceOrderService : ServicesBase<Live_BusinessInsuranceOrder>, IBusinessInsuranceOrderService, IScopedDependency
    {


        public async Task<List<Live_V_BusinessInsuranceOrder>> PageList(Expression<Func<Live_V_BusinessInsuranceOrder, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await base.Context.Queryable<Live_V_BusinessInsuranceOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_BusinessInsuranceOrder>> List(Expression<Func<Live_BusinessInsuranceOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_V_BusinessInsuranceOrder>> ListView(Expression<Func<Live_V_BusinessInsuranceOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceOrder>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }



        public async Task<bool> AddNav(Live_BusinessInsuranceOrderExtension model)
        {
            return await base.Context.InsertNav(model)
                .Include(f => f.Records)
                .ExecuteCommandAsync();
        }
      

        public async Task<bool> Update(Live_BusinessInsuranceOrder model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_BusinessInsuranceOrder> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BusinessInsuranceOrder> Get(Expression<Func<Live_BusinessInsuranceOrder, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceOrder>().OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<Live_BusinessInsuranceOrderExtension> GetNav(Expression<Func<Live_BusinessInsuranceOrderExtension, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceOrderExtension>()
                .Includes(f => f.Records)
                .Includes(f => f.EmployeeRecords)
                .OrderByDescending(f => f.CreateTime).FirstAsync(exp);
        }

        public async Task<bool> UpdateNav(Live_BusinessInsuranceOrderExtension model)
        {
            return await base.Context.UpdateNav(model)
                .Include(f => f.Records)
                .Include(f => f.EmployeeRecords)
              .ExecuteCommandAsync();
        }
        public async Task<bool> DeleteNav(Expression<Func<Live_BusinessInsuranceOrderExtension, bool>> exp)
        {
            return await base.Context.DeleteNav(exp)
                .Include(f => f.Records)
                .Include(f => f.EmployeeRecords)
                .ExecuteCommandAsync();
        }

     

     

        public async Task<List<Live_V_BusinessInsuranceOrderPlan>> ListOrderPlanView(Expression<Func<Live_V_BusinessInsuranceOrderPlan, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceOrderPlan>().Where(exp).ToListAsync();
        }
        public async Task<Live_V_BusinessInsuranceOrderPlan> GetOrderPlanView(Expression<Func<Live_V_BusinessInsuranceOrderPlan, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_BusinessInsuranceOrderPlan>().Where(exp).FirstAsync();
        }
    }
}
